namespace SequenceAssembler
{
    #region -- Using Directive --

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Bio;
    using Bio.Algorithms.Alignment;
    using Bio.Algorithms.Assembly;
    using Bio.Algorithms.Assembly.Padena;
    using Bio.Algorithms.Assembly.Padena.Utility;
    using Bio.Algorithms.MUMmer;
    using Bio.IO;
    using Bio.IO.FastA;
    using Bio.SimilarityMatrices;
    using Bio.Web;
    using Bio.Web.Blast;
    using SequenceAssembler.Properties;
    using MSA = Bio.Algorithms.Alignment.MultipleSequenceAlignment;
    
    #endregion -- Using Directive --

    /// <summary>
    /// Controller is the heart of the applications. It controls
    /// interaction between various user-controls, manages buisness logic.
    /// Controller will be hosted by main window which holds user-controls.
    /// The interaction between these controls happens mainly through eventing.
    /// </summary>
    public class Controller : IDisposable
    {
        #region -- Private Members --

        /// <summary>
        /// Minimum size for loading DV
        /// </summary>
        private const int minimumSizeforEnforceDV = 20 * 1024; // 20 MB

        /// <summary>
        /// Name of the blast service which was executed last
        /// </summary>
        private IBlastServiceHandler lastExecutedBlastService;

        /// <summary>
        /// Name of the database to which a blast search was done last
        /// </summary>
        private string lastBlastDatabaseName;

        /// <summary>
        /// Sleep time for the background thread searching for a online database.
        /// </summary>
        private const int SearchSleepTime = 100;

        /// <summary>
        /// Maximum number of poll we do before we exit search.
        /// </summary>
        private const int MaxPollAttempts = 10;

        /// <summary>
        /// Instance of IAssembler. IAssembler will contain both
        /// the sequence and consensus view.
        /// </summary>
        private IAssembler assembler;

        /// <summary>
        /// Instance of IFileMenu. IFileMenu will handle the dialogue for
        /// users to select sequence files.
        /// </summary>
        private IFileMenu fileMenu;

        /// <summary>
        /// Handles parsing sequence files in a background thread.
        /// </summary>
        private BackgroundWorker fileParserThread;

        /// <summary>
        /// Handles assembling sequences in a background thread.
        /// </summary>
        private BackgroundWorker assemblerThread;

        /// <summary>
        /// Prevents IDisposable.Dispose() getting executed multiple times.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Describes the collection of file names to be removed.
        /// </summary>
        private Collection<string> removeFilesName;

        /// <summary>
        /// Instance of IWebServiceRenderer. This IWebServiceRenderer instance
        /// will render the webservice output on the UI.
        /// </summary>
        private IWebServicePresenter webservicePresenter;

        /// <summary>
        /// ID of Blast search job.
        /// </summary>
        private string requestIdentifier;

        #endregion -- Private Members --

        #region -- Public Events --

        /// <summary>
        /// This event will be raised when a Pop-up will be displayed
        /// by the application.
        /// </summary>
        public event EventHandler PopupOpened;

        /// <summary>
        /// This event will be raised when a Pop-up will be closed
        /// by the application.
        /// </summary>
        public event EventHandler<PopupEventArgs> PopupClosed;

        /// <summary>
        /// This event will be raised when a Pop-up will be closed
        /// by the application.
        /// </summary>
        public event EventHandler SearchCompleted;

        /// <summary>
        /// This event will be raised when the user wants to exit the application.
        /// </summary>
        public event EventHandler WindowClosed;

        #endregion -- Public Events --

        #region -- Properties --

        /// <summary>
        /// Gets or sets an instance of IAssembler.
        /// </summary>
        public IAssembler Assembler
        {
            get
            {
                return this.assembler;
            }

            set
            {
                if (this.assembler != value && value != null)
                {
                    this.assembler = value;
                    this.InitializeAssembler();
                }
            }
        }

        /// <summary>
        /// Gets or sets an instance of FileMenu.
        /// </summary>
        public IFileMenu FileMenu
        {
            get
            {
                return this.fileMenu;
            }

            set
            {
                if (this.fileMenu != value && value != null)
                {
                    this.fileMenu = value;
                    this.InitializeFileType();
                }
            }
        }

        /// <summary>
        /// Gets or sets an instance of IWebServiceRenderer.
        /// </summary>
        public IWebServicePresenter WebServicePresenter
        {
            get
            {
                return this.webservicePresenter;
            }

            set
            {
                if (this.webservicePresenter != value && value != null)
                {
                    this.webservicePresenter = value;
                }
            }
        }

        #endregion -- Properties --

        #region -- Public Static Methods --
        /// <summary>
        /// This method checks the file selected for import is present in the specified collection or not.
        /// </summary>
        /// <param name="fileInfo">File info collection.</param>
        /// <param name="file">File to be verified</param>
        /// <returns>Returns true if the file is present in the collection else returns false.</returns>
        public static bool ContainsFile(Collection<FileSystemInfo> fileInfo, FileSystemInfo file)
        {
            if (fileInfo != null && file != null && fileInfo.Count > 0)
            {
                foreach (FileInfo info in fileInfo)
                {
                    if (info.FullName == file.FullName || info.LastWriteTime == file.LastWriteTime)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion -- Public Static Methods --

        #region -- Public Methods --
        /// <summary>
        /// Loads files into the sequence viewer.
        /// </summary>
        /// <param name="filesToLoad">Files to load.</param>
        public void LoadFiles(IList<string> filesToLoad)
        {
            Collection<string> filenames = new Collection<string>();
            Collection<FileSystemInfo> fileInfos = new Collection<FileSystemInfo>();

            foreach (string filename in filesToLoad)
            {
                FileInfo finfo = new FileInfo(filename);

                // Check for already loaded files.
                if (ContainsFile(fileInfos, finfo))
                {
                    continue;
                }

                filenames.Add(filename);
                fileInfos.Add(finfo);

                // Add file into to the loaded file collection.
                if (this.FileMenu != null)
                {
                    this.FileMenu.FileInfo.Add(finfo);
                }
            }

            if (filenames.Count > 0)
            {
                ImportFileEventArgs args = new ImportFileEventArgs(filenames, null, fileInfos, null);
                this.OnFileImported(null, args);
            }
            else
            {
                this.PopupClosed(this, new PopupEventArgs(true));
            }
        }

        /// <summary>
        /// Show file open dialog with the file browser open by default
        /// </summary>
        public void ShowOpenFileDialog()
        {
            this.fileMenu.ShowOpenFileDialog();
        }

        /// <summary>
        /// Method to dispose on all objects 
        /// implementing IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion -- Public Methods --

        #region -- Private Methods --


        /// <summary>
        /// This method will query the Framework abstraction
        /// to figure out the parsers supported by the framwork.
        /// </summary>
        /// <returns>List of all parsers and the file extensions the parsers support.</returns>
        private static Collection<string> QuerySupportedFileType()
        {
            Collection<string> fileExtensions = new Collection<string>();
            foreach (ISequenceParser parser in SequenceParsers.All)
            {
                fileExtensions.Add(parser.Name + "," + parser.SupportedFileTypes);
            }

            return fileExtensions;
        }

        /// <summary>
        /// This method will query the Framework abstraction
        /// to figure out the alignment algorithms supported by the framework.
        /// </summary>
        /// <returns>List of all alignment algorithms supported by the framwork.</returns>
        private static IEnumerable<string> QueryAlignmentAlgorithm(bool pairwiseAlignersOnly = false)
        {
            Collection<string> algorithms = new Collection<string>();
            foreach (ISequenceAligner aligner in SequenceAligners.All)
            {
                if (!pairwiseAlignersOnly)
                {
                    algorithms.Add(aligner.Name);
                }
                else
                {
                    if (aligner is IPairwiseSequenceAligner)
                    {
                        algorithms.Add(aligner.Name);
                    }
                }
            }

            IEnumerable<string> algos = algorithms.OrderBy(alg => alg);

            return algos;
        }

        /// <summary>
        /// This method will query the Framework abstraction
        /// to figure out the different webservices supported by the framework.
        /// </summary>
        /// <returns>List of all webservices supported by the framework.</returns>
        private static IEnumerable<string> QueryWebServiceNames()
        {
            Collection<string> webserviceNames = new Collection<string>();
            foreach (IBlastServiceHandler blastService in 
                WebServices.All.Where(service => service is IBlastServiceHandler))
            {
                webserviceNames.Add(blastService.Name);
            }

            IEnumerable<string> webservices = webserviceNames.OrderBy(webservice => webservice);
            return webservices;
        }

        /// <summary>
        /// This method calls framework method to parse files.
        /// The output of parser will be passed to IAssembler
        /// to display sequence tree view.
        /// </summary>
        /// <param name="filePaths">List of files to be parsed.</param>
        /// <param name="worker">The File parser thread</param>
        /// <param name="e">Import File event data</param>
        /// <returns>
        ///  Collection of ISequence.
        /// </returns>
        private static Collection<ParsedFileInfo> ParseFiles(Collection<string> filePaths, BackgroundWorker worker, ImportFileEventArgs e)
        {
            Collection<ParsedFileInfo> importedSequence = new Collection<ParsedFileInfo>();
            foreach (string filePath in filePaths)
            {
                if (worker != null && !worker.CancellationPending)
                {
                    // check whether the file exists or not.
                    if (!File.Exists(filePath))
                    {
                        string message = string.Format(CultureInfo.CurrentCulture, Resource.FileNotFound, filePath);
                        throw new FileNotFoundException(message);
                    }

                    string parserName = e.ParserName;
                    ISequenceParser parser = null;

                    if (string.IsNullOrEmpty(parserName) || parserName == Resource.AutoString)
                    {
                        parser = SequenceParsers.FindParserByFileName(filePath);
                        // check for the parser.
                        if (parser == null)
                        {
                            string message = string.Format(CultureInfo.CurrentCulture, Resource.ParserNotFound, filePath);
                            throw new FileFormatException(message);
                        }
                    }
                    else
                    {
                        parser = SequenceParsers.FindParserByName(filePath, parserName);
                    }

                    parser.Alphabet = e.Molecule;
                    CreateImportedSequence(filePath, importedSequence, parser);
                    RecentFilesManager.AddFile(filePath);
                }
                else
                {
                    importedSequence = null;
                    break;
                }
            }

            return importedSequence;
        }

        /// <summary>
        /// Creates the imported sequence collection for the given parser, filepath.
        /// </summary>
        /// <param name="parser">File parser like Fasta, gff, gb etc</param>
        /// <param name="filePath">Path fo the file</param>
        /// <param name="importedSequence">Collection of the Parsed file Info</param>
        private static void CreateImportedSequence(string filePath, Collection<ParsedFileInfo> importedSequence, ISequenceParser parser = null)
        {
            if (parser == null)
            {
                parser = SequenceParsers.FindParserByFileName(filePath);
            }

            if (parser == null)
            {
                throw new FileFormatException(Properties.Resource.BAD_FILE_ERROR);
            }

            IEnumerable<ISequence> sequenceList = parser.Parse();

            Collection<ISequence> sequences = new Collection<ISequence>();
            foreach (ISequence sequence in sequenceList)
            {
                sequences.Add(sequence);
            }

            ParsedFileInfo info = new ParsedFileInfo(filePath, sequences);
            importedSequence.Add(info);
        }

        /// <summary>
        /// Parses all the supported formats
        /// </summary>
        /// <param name="filePath">file path of teh file to be parsed.</param>
        /// <param name="importedSequence">Collection of the parsed file info</param>
        /// <param name="worker">The File parser thread</param>
        private static void ParseAllFormats(string filePath, Collection<ParsedFileInfo> importedSequence, BackgroundWorker worker)
        {
            foreach (ISequenceParser parser in SequenceParsers.All)
            {
                if (worker != null && !worker.CancellationPending)
                {
                    try
                    {
                        CreateImportedSequence(filePath, importedSequence, parser);
                        break;
                    }
                    catch (Exception)
                    {
                        if (SequenceParsers.All.Count - 1 != SequenceParsers.All.IndexOf(parser))
                        {
                            continue;
                        }
                        else
                        {
                            throw new Exception(Properties.Resource.BAD_FILE_ERROR);
                        }
                    }
                }
                else
                {
                    importedSequence = null;
                    break;
                }
            }
        }

        /// <summary>
        /// This method runs assembly on the list of sequences passed.
        /// Additionally the user is allowed to select the
        /// alignment algorithm.
        /// </summary>
        /// <param name="input">Input for the assembly process.</param>
        /// <param name="worker">The Assembly parser thread</param>
        /// <returns>IDeNovoAssembly instance.</returns>
        private static AssemblyOutput RunAssembly(AssemblyInputEventArgs input, BackgroundWorker worker)
        {
            double mergeThreshold = input.MergeThreshold;
            List<ISequence> sequence = input.Sequences.ToList();

            AssemblyOutput outPut = new AssemblyOutput();
            outPut.StartTime = DateTime.Now;

            if (input.AssemblerUsed == AssemblerType.PaDeNA)
            {
                outPut.AssemblerUsed = AssemblerType.PaDeNA;

                ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler();

                assembler.KmerLength = input.KmerLength;
                assembler.DanglingLinksThreshold = input.DanglingLinksThreshold;
                assembler.RedundantPathLengthThreshold = input.RedundantPathLengthThreshold;
                assembler.AllowErosion = input.ErosionEnabled;
                if (input.ErosionThreshold >= 0)
                {
                    assembler.ErosionThreshold = (int)Math.Round(input.ErosionThreshold, 0);
                }

                assembler.AllowLowCoverageContigRemoval = input.LowCoverageContigRemovalEnabled;

                if (input.LowCoverageContigRemovalThreshold >= 0)
                {
                    assembler.ContigCoverageThreshold = input.LowCoverageContigRemovalThreshold;
                }

                IDeNovoAssembly result;
                if(input.GenerateScaffolds)
                {
                    assembler.Depth = input.Depth;
                    assembler.ScaffoldRedundancy = input.ScaffoldRedundancy;
                    result = assembler.Assemble(sequence, true);
                }
                else
                {
                    result = assembler.Assemble(sequence);
                }

                outPut.Contigs = ReadAlignment.ReadContigAlignment(result.AssembledSequences.ToList(), sequence, input.KmerLength);
                outPut.NoOfContigs = outPut.Contigs.Count();
                outPut.NoOfUnassembledSequence = 0; // Always 0 for PaDeNA
                outPut.TotalLength = outPut.Contigs.Sum(c => c.Length);
                outPut.AlgorithmNameUsed = Properties.Resource.PaDeNAString;
            }
            else
            {
                outPut.AssemblerUsed = AssemblerType.SimpleSequenceAssembler;

                OverlapDeNovoAssembler assemble = new OverlapDeNovoAssembler();
                assemble.OverlapAlgorithm = ChooseAlgorithm(input.Algorithm);

                // Special casing for SW alignment.
                if (assemble.OverlapAlgorithm is SmithWatermanAligner)
                {
                    // If we set the Threshold value lesser than the Max score, then the result will be “JUNK”.
                    // So setting the threshold value to 25 approximately supports sequence length of 15,0000.
                    mergeThreshold = 25;
                }

                assemble.MergeThreshold = mergeThreshold;
                if (null == input.AlignerInput.SimilarityMatrix)
                {
                    assemble.OverlapAlgorithm.SimilarityMatrix = new DiagonalSimilarityMatrix(input.MatchScore, input.MismatchScore);
                }
                else
                {
                    assemble.OverlapAlgorithm.SimilarityMatrix = input.AlignerInput.SimilarityMatrix;
                }

                assemble.OverlapAlgorithm.GapOpenCost = input.AlignerInput.GapCost;
                assemble.OverlapAlgorithm.GapExtensionCost = input.AlignerInput.GapExtensionCost;
                assemble.ConsensusResolver = new SimpleConsensusResolver(input.ConsensusThreshold);
                assemble.AssumeStandardOrientation = false;

                AssignAlignerParameter(assemble.OverlapAlgorithm, input.AlignerInput);

                IOverlapDeNovoAssembly assemblyOutput = (IOverlapDeNovoAssembly)assemble.Assemble(sequence);

                if (worker != null && worker.CancellationPending == true)
                {
                    return null;
                }

                outPut.SequenceAssembly = assemblyOutput;
                outPut.AlgorithmNameUsed = input.Algorithm;
                outPut.NoOfContigs = assemblyOutput.Contigs.Count;
                outPut.NoOfUnassembledSequence = assemblyOutput.UnmergedSequences.Count;
                outPut.TotalLength = assemblyOutput.Contigs.Sum(c => c.Length) + assemblyOutput.UnmergedSequences.Sum(s => s.Count);
            }

            outPut.NoOfSequence = sequence.Count;
            outPut.EndTime = DateTime.Now;

            return outPut;
        }

        /// <summary>
        /// Runs and alignment algorithm and return the result
        /// </summary>
        /// <param name="input">Arguments for alignment including the sequences to be aligned</param>
        /// <param name="worker">instance of the worker on which this is running</param>
        /// <returns>Alignment output</returns>
        private static IList<ISequenceAlignment> RunAlignment(AlignerInputEventArgs input, BackgroundWorker worker)
        {
            ISequenceAligner aligner = ChooseAlgorithm(input.SelectedAlgorithm);
            IList<ISequenceAlignment> alignedResult = new List<ISequenceAlignment>();

            if (aligner is MSA.PAMSAMMultipleSequenceAligner)
            {
                // handling pamsam seperatly
                MSA.PAMSAMMultipleSequenceAligner pamsamAligner = new MSA.PAMSAMMultipleSequenceAligner(
                    input.SequencesToAlign,
                    input.KmerLength,
                    input.DistanceFunctionName,
                    input.UpdateDistanceMethodsType,
                    input.ProfileAlignerName,
                    input.ProfileScoreFunctionName,
                    input.SimilarityMatrix,
                    input.GapOpenPenalty,
                    input.GapExtendPenalty,
                    input.NumberOfPartitions,
                    input.DegreeOfParallelism
                    );
                if (pamsamAligner.AlignedSequencesC.Count > 0)
                {
                    SequenceAlignment alignment = new SequenceAlignment();
                    AlignedSequence alignedSequenceItem = new AlignedSequence();

                    foreach (ISequence sequence in pamsamAligner.AlignedSequencesC)
                    {
                        alignedSequenceItem.Sequences.Add(sequence);
                    }

                    alignment.AlignedSequences.Add(alignedSequenceItem);
                    alignedResult = new List<ISequenceAlignment> { alignment };
                }
            }
            else
            {
                AssignAlignerParameter(aligner, input);
                aligner.GapOpenCost = input.GapCost;
                aligner.GapExtensionCost = input.GapExtensionCost;

                if (input.SimilarityMatrix != null)
                {
                    aligner.SimilarityMatrix = input.SimilarityMatrix;
                }

                alignedResult = aligner.Align(input.SequencesToAlign).AsParallel().Where(alignment => alignment.AlignedSequences.Count > 0).ToList();
            }

            if (alignedResult.Count == 0)
            {
                throw new WarningException(Properties.Resource.NoAlignmentPossible);
            }

            return alignedResult;
        }

        /// <summary>
        /// This method when passed the algorithm name instantiates
        /// the framework class which implements.
        /// </summary>
        /// <param name="algorithmName">Nmae of the algorithm.</param>
        /// <returns>Class which instantiates the algorithm.</returns>
        private static ISequenceAligner ChooseAlgorithm(string algorithmName)
        {
            foreach (ISequenceAligner aligner in SequenceAligners.All)
            {
                if (aligner.Name.Equals(algorithmName))
                {
                    ISequenceAligner pairWise = aligner as ISequenceAligner;
                    if (pairWise != null)
                    {
                        return pairWise;
                    }
                }
            }

            if (algorithmName == Properties.Resource.PAMSAM)
            {
                return new MSA.PAMSAMMultipleSequenceAligner();
            }

            return null;
        }

        /// <summary>
        /// This method formats a collection of ISequence to the given
        /// file path.
        /// </summary>
        /// <param name="sequences">
        /// Collection of ISequence that has to be written to the disk
        /// </param>
        /// <param name="fileName">
        /// Path of the file where ISequences have to be written to.
        /// </param>
        private static void SaveFile(ICollection<ISequence> sequences, string fileName)
        {
            ISequenceFormatter formatter = SequenceFormatters.FindFormatterByFileName(fileName);

            if (formatter == null)
            {
                formatter = new FastAFormatter(fileName);
            }

            // TODO: write all seuqences, remove elementat() call
            foreach (ISequence seq in sequences)
            {
                formatter.Write(seq);
            }

            formatter.Close();
        }

        /// <summary>
        /// Returns the instance of IWebService depending on the service name.
        /// </summary>
        /// <param name="webserviceName">Name of the webservice</param>
        /// <returns>Instance of the web service</returns>
        private static IBlastServiceHandler GetWebServiceInstance(string webserviceName)
        {
            foreach (IBlastServiceHandler webService in
                WebServices.All.Where(service => service is IBlastServiceHandler))
            {
                if (webService.Name.Equals(webserviceName))
                {
                    return webService;
                }
            }

            return null;
        }

        /// <summary>
        /// Assign the aligner specific paramaters
        /// </summary>
        /// <param name="sequenceAligner">Sequece Aligner object</param>
        /// <param name="alignerInput">Aligner Input object</param>
        private static void AssignAlignerParameter(
            ISequenceAligner sequenceAligner,
            AlignerInputEventArgs alignerInput)
        {
            if (sequenceAligner is NucmerPairwiseAligner)
            {
                NucmerPairwiseAligner nucmer = sequenceAligner as NucmerPairwiseAligner;

                nucmer.LengthOfMUM = alignerInput.LengthOfMUM;
                nucmer.FixedSeparation = alignerInput.FixedSeparation;
                nucmer.MaximumSeparation = alignerInput.MaximumSeparation;
                nucmer.MinimumScore = alignerInput.MinimumScore;
                nucmer.SeparationFactor = alignerInput.SeparationFactor;
                nucmer.BreakLength = alignerInput.BreakLength;
            }
            else if (sequenceAligner is MUMmerAligner)
            {
                MUMmerAligner mummer = sequenceAligner as MUMmerAligner;

                mummer.LengthOfMUM = alignerInput.LengthOfMUM;
            }
        }

        /// <summary>
        /// This method initializes IFileMenu and registers for events from
        /// IFileMenu.
        /// </summary>
        private void InitializeFileType()
        {
            Collection<string> fileTypes = QuerySupportedFileType();
            this.fileMenu.DisplayFileTypes(fileTypes);

            // Register for IFilemenu events
            this.fileMenu.SaveWorkspace += new EventHandler(this.OnFileMenuSaveWorkspace);
            this.fileMenu.PopupOpened += new EventHandler(this.OnPopUpOpened);
            this.fileMenu.PopupClosed += new EventHandler(this.OnPopUpClosed);
            this.fileMenu.ImportFile += new EventHandler<ImportFileEventArgs>(this.OnFileImported);
            this.fileMenu.CancelImport += new EventHandler(this.OnFileMenuCancelImport);
            this.fileMenu.CloseWindow += new EventHandler(this.OnFileMenuClose);
        }

        /// <summary>
        /// This event Saves the seleted contig or sequence
        /// </summary>
        /// <param name="sender">File Menu Element</param>
        /// <param name="e">Event Data</param>
        private void OnFileMenuSaveWorkspace(object sender, EventArgs e)
        {
            if (this.assembler != null)
            {
                this.assembler.SaveWorkspace();
            }
        }

        /// <summary>
        /// This event handler would stop the importing process of the files
        /// </summary>
        /// <param name="sender">IFileMenu instance</param>
        /// <param name="e">Event data</param>
        private void OnFileMenuCancelImport(object sender, EventArgs e)
        {
            if (this.fileParserThread.IsBusy)
            {
                this.fileParserThread.CancelAsync();
            }
        }

        /// <summary>
        /// This event is fired when IFileMenu raises this event
        /// indicating that user has selected sequence files to parse.
        /// </summary>       
        /// <param name="sender">Event data</param>
        /// <param name="e">IFileMenu instance</param>
        private void OnFileImported(object sender, ImportFileEventArgs e)
        {
            // Create a background worker thread and do the parsing
            // in the background thread.            
            this.fileParserThread = new BackgroundWorker();
            this.fileParserThread.WorkerSupportsCancellation = true;
            this.fileParserThread.DoWork += new DoWorkEventHandler(this.OnParseFileStarted);
            this.fileParserThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.OnParseCompleted);
            this.fileParserThread.RunWorkerAsync(e);
        }

        /// <summary>
        /// This event is fired when the parsing of files are completely
        /// done. This event asks IFileMenu to close import file dialogue
        /// and also asks IAssembler to display sequence tree view.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnParseCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                Collection<ParsedFileInfo> sequences = e.Result as Collection<ParsedFileInfo>;
                if (sequences != null)
                {
                    this.BuildSequenceView(sequences);
                }

                // This is an error scenario, display it to the users.
                string errorMessage = e.Result as string;
                if (errorMessage != null)
                {
                    foreach (string file in this.removeFilesName)
                    {
                        FileInfo info = new FileInfo(file);
                        this.fileMenu.RemoveFileInfo(info);
                    }

                    this.fileMenu.CloseDialogue();
                    MessageDialogBox.Show(errorMessage, Properties.Resource.CAPTION, MessageDialogButton.OK);
                }
            }
            else
            {
                // when the import is cancelled, remove the animation
                this.fileMenu.OnCancelImport();
                if (this.removeFilesName != null && this.removeFilesName.Count > 0)
                {
                    foreach (string file in this.removeFilesName)
                    {
                        FileInfo info = new FileInfo(file);
                        this.fileMenu.RemoveFileInfo(info);
                    }
                }
            }
        }

        /// <summary>
        /// This event is fired when the user clicks on Exit option in the 
        /// load menu.This event asks controller to close the window.
        /// Controller inturn informs its parent to close the window.
        /// </summary>
        /// <param name="sender">IFileMenu instance.</param>
        /// <param name="e">Event data.</param>
        private void OnFileMenuClose(object sender, EventArgs e)
        {
            if (this.WindowClosed != null)
            {
                this.WindowClosed(this, e);
            }
        }

        /// <summary>
        /// This method concatenates old sequences with new sequences
        /// and makes a call to IAssembler to display sequence tree view.
        /// </summary>
        /// <param name="sequences">Collection of new sequences</param>
        private void BuildSequenceView(Collection<ParsedFileInfo> sequences)
        {
            IEnumerable<ParsedFileInfo> sequence = sequences.AsEnumerable<ParsedFileInfo>();
            this.assembler.DisplaySequenceTreeView(sequence);
        }

        /// <summary>
        /// This event is fired by fileParserThread when the thread is invoked.
        /// This event parses sequence files and extracts a collection
        /// of ISequence.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnParseFileStarted(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker != null)
            {
                try
                {
                    ImportFileEventArgs fileArgs = e.Argument as ImportFileEventArgs;
                    if (fileArgs != null)
                    {
                        Collection<ParsedFileInfo> infos = ParseFiles(fileArgs.FileNames, worker, fileArgs);

                        // remove the file names info, when the import is cancelled
                        if (worker.CancellationPending)
                        {
                            this.removeFilesName = (e.Argument as ImportFileEventArgs).FileNames;
                            e.Cancel = true;
                            return;
                        }

                        e.Result = infos;
                    }
                }
                catch (FileNotFoundException ex)
                {
                    this.removeFilesName = (e.Argument as ImportFileEventArgs).FileNames;
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    e.Result = ex.Message;
                }
                catch (FileFormatException ex)
                {
                    this.removeFilesName = (e.Argument as ImportFileEventArgs).FileNames;
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    e.Result = ex.Message;
                }
                catch (Exception)
                {
                    this.removeFilesName = (e.Argument as ImportFileEventArgs).FileNames;
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    ImportFileEventArgs fileArgs = e.Argument as ImportFileEventArgs;
                    e.Result = Properties.Resource.BAD_FILE_ERROR + " [" + fileArgs.ParserName + "]";
                }
            }
        }

        /// <summary>
        /// This event is fired when a pop-up is to be displayed 
        /// in the application. Controller in turn will raise
        /// a event which will inform its parent to take action
        /// before a pop-up is opened.
        /// </summary>
        /// <param name="sender">User control</param>
        /// <param name="e">Event data</param>
        private void OnPopUpClosed(object sender, EventArgs e)
        {
            if (this.PopupClosed != null)
            {
                PopupEventArgs args = null;
                args = new PopupEventArgs(true);

                this.PopupClosed(sender, args);
            }
        }

        /// <summary>
        /// This event is fired when a pop-up is to be displayed 
        /// in the application. Controller in turn will raise
        /// a event which will inform its parent to take action
        /// before a pop-up is opened.
        /// </summary>
        /// <param name="sender">User control</param>
        /// <param name="e">Event data</param>
        private void OnPopUpOpened(object sender, EventArgs e)
        {
            if (this.PopupOpened != null)
            {
                this.PopupOpened(sender, e);
            }
        }

        /// <summary>
        /// This event is fired when user wants to assemble the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates algorithm implementation to perform assembly.
        /// </summary>
        /// <param name="sender">IAssembler instance.</param>
        /// <param name="e">AssemblyInput data.</param>
        private void OnRunAssemblerAlgorithm(object sender, AssemblyInputEventArgs e)
        {
            if (e.Sequences != null && (!string.IsNullOrEmpty(e.Algorithm)))
            {
                this.assemblerThread = new BackgroundWorker();
                this.assemblerThread.WorkerSupportsCancellation = true;
                this.assemblerThread.DoWork += new DoWorkEventHandler(this.OnAssembleStarted);
                this.assemblerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.OnAssemblerCompleted);
                this.assemblerThread.RunWorkerAsync(e);
            }
        }

        /// <summary>
        /// This event is fired when user wants to align the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates algorithm implementation to perform alignment.
        /// </summary>
        /// <param name="sender">IAssembler instance.</param>
        /// <param name="e">AssemblyInput data.</param>
        private void OnRunAlignerAlgorithm(object sender, AlignerInputEventArgs e)
        {
            if (e.SequencesToAlign != null && (!string.IsNullOrEmpty(e.SelectedAlgorithm)))
            {
                this.assemblerThread = new BackgroundWorker();
                this.assemblerThread.WorkerSupportsCancellation = true;
                this.assemblerThread.DoWork += new DoWorkEventHandler(this.OnAlignmentStarted);
                this.assemblerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.OnAlignmentCompleted);
                this.assemblerThread.RunWorkerAsync(e);
            }
        }

        /// <summary>
        /// This event is fired when the assembling the sequence is completed.
        /// This event asks IAssembler to display consensus view.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnAssemblerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            AssemblyOutput assemblerResult = e.Result as AssemblyOutput;
            if (this.assembler != null && assemblerResult != null)
            {
                this.assembler.BuildConsensusView(assemblerResult);
            }

            // This is an error scenario, display it to the users.
            string errorMessage = e.Result as string;
            if (errorMessage != null)
            {
                this.assembler.Cleanup();
                MessageDialogBox.Show(errorMessage, Properties.Resource.CAPTION, MessageDialogButton.OK);
            }
        }

        /// <summary>
        /// This method is fired when the aligning the sequence is completed.
        /// This method asks IAssembler to display alignment view.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnAlignmentCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            AlignmentOutput alignerResult = e.Result as AlignmentOutput;
            if (this.assembler != null && alignerResult != null)
            {
                this.assembler.BuildAlignmentView(alignerResult);
            }

            // This is an error scenario, display it to the users.
            string errorMessage = e.Result as string;
            if (errorMessage != null)
            {
                this.assembler.Cleanup();
                MessageDialogBox.Show(errorMessage, Properties.Resource.CAPTION, MessageDialogButton.OK);
            }
        }

        /// <summary>
        /// This event is fired by assemblerThread when the thread is invoked.
        /// This event assembles a collection of ISequences.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnAssembleStarted(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            try
            {
                if (worker != null)
                {
                    AssemblyInputEventArgs assemblerInput = e.Argument as AssemblyInputEventArgs;
                    if (assemblerInput != null)
                    {
                        AssemblyOutput assemblerResult = RunAssembly(assemblerInput, worker);
                        if (worker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        e.Result = assemblerResult;
                    }
                }
            }
            catch (Exception ex)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                e.Result = ex.Message;
            }
        }

        /// <summary>
        /// This method is fired by a BackgroundWorker when the thread is invoked.
        /// This method align a collection of ISequences.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnAlignmentStarted(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            try
            {
                if (worker != null)
                {
                    AlignerInputEventArgs alignerInput = e.Argument as AlignerInputEventArgs;
                    if (alignerInput != null)
                    {
                        AlignmentOutput output = new AlignmentOutput();

                        output.StartTime = DateTime.Now;
                        IList<ISequenceAlignment> alignerResult = RunAlignment(alignerInput, worker);
                        output.EndTime = DateTime.Now;
                        
                        output.AlignerName = alignerInput.SelectedAlgorithm;
                        output.AlignerResult = alignerResult;
                        output.InputSequenceCount = alignerInput.SequencesToAlign.Count;

                        if (worker.CancellationPending == true)
                        {
                            e.Cancel = true;
                            return;
                        }

                        e.Result = output;
                    }
                }
            }
            catch (Exception ex)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                e.Result = ex.Message;
            }
        }

        /// <summary>
        /// This method is called when the user prompts for a sequence search on
        /// the NCBI database. This method creates a type of web-service object
        /// and passes parameters to it. And waits for the result.
        /// </summary>
        /// <param name="sender">IAssembler instance</param>
        /// <param name="e">Event data.</param>
        private void OnExecuteSearch(object sender, WebServiceInputEventArgs e)
        {
            this.requestIdentifier = string.Empty;

            if (this.PopupClosed != null)
            {
                this.PopupClosed(sender, new PopupEventArgs(true));
            }

            if (e != null && !string.IsNullOrEmpty(e.WebServiceName))
            {
                IBlastServiceHandler blastServiceHandler = GetWebServiceInstance(e.WebServiceName);
                lastExecutedBlastService = blastServiceHandler;
                e.ServiceParameters.Settings.TryGetValue("DATABASE", out lastBlastDatabaseName);

                blastServiceHandler.Configuration = e.Configuration;

                // Make sure if the event handler was already added, it is removed
                // otherwise the handler will be invoked multiple times when the
                // event is raised.
                blastServiceHandler.RequestCompleted -=
                        new EventHandler<BlastRequestCompletedEventArgs>(this.OnBlastRequestCompleted);

                blastServiceHandler.RequestCompleted +=
                        new EventHandler<BlastRequestCompletedEventArgs>(this.OnBlastRequestCompleted);

                try
                {
                    this.requestIdentifier = blastServiceHandler.SubmitRequest(
                            this.assembler.SelectedSequence,
                            e.ServiceParameters);
                }
                catch (Exception ex)
                {
                    MessageDialogBox.Show(
                            ex.Message,
                            Properties.Resource.CAPTION,
                            MessageDialogButton.OK);

                    this.assembler.SearchCompleted(false);
                }

                return;
            }
        }

        /// <summary>
        /// Invoked by Azure Blast Handler when it is done executing the request.
        /// It sets the output of search in the Result property of event argument.
        /// </summary>
        /// <param name="sender">Azure Blast Handler</param>
        /// <param name="e">Event Arguments</param>
        private void OnBlastRequestCompleted(object sender, BlastRequestCompletedEventArgs e)
        {
            RunWorkerCompletedEventArgs ev = null;
            if (e.IsSearchSuccessful)
            {
                ev = new RunWorkerCompletedEventArgs(
                        e.SearchResult,
                        null,
                        e.IsCanceled);
            }
            else
            {
                ev = new RunWorkerCompletedEventArgs(
                        e.ErrorMessage,
                        null,
                        e.IsCanceled);
            }

            this.OnSearchCompleted(sender, ev);
        }

        /// <summary>
        /// This event is fired when the search on NCBI database is completed.
        /// This event asks IWebServicePresneter to display BLAST outputs.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnSearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<BlastResult> results = e.Result as List<BlastResult>;
            if (results != null && !(e.Result is string))
            {
                this.webservicePresenter.DisplayWebServiceOutput(results, this.lastExecutedBlastService, this.lastBlastDatabaseName);
                if (this.SearchCompleted != null)
                {
                    this.SearchCompleted(this, new EventArgs());
                }
                this.assembler.SearchCompleted(true);
            }
            else
            {
                if (e.Result is string)
                {
                    MessageDialogBox.Show(e.Result.ToString(), Properties.Resource.CAPTION, MessageDialogButton.OK);
                }
                this.assembler.SearchCompleted(false);
            }
        }

        /// <summary>
        /// This method will inform IAssembler of the alignment
        /// algorithms supported by the frame-work.
        /// </summary>
        private void InitializeAssembler()
        {
            IEnumerable<string> algorithms = QueryAlignmentAlgorithm(true);
            IEnumerable<string> webserviceNames = QueryWebServiceNames();
            this.assembler.PopupOpened += new EventHandler(this.OnPopUpOpened);
            this.assembler.PopupClosed += new EventHandler(this.OnPopUpClosed);
            this.assembler.InitializeSupportedFileTypes(QuerySupportedFileType());
            this.assembler.InitializeAlignmentAlgorithms(algorithms);
            this.assembler.InitializeSupportedWebServices(webserviceNames);
            this.assembler.RunAssemblerAlgorithm += this.OnRunAssemblerAlgorithm;
            this.assembler.RunAlignerAlgorithm += this.OnRunAlignerAlgorithm;
            this.assembler.ImportCompleted += new EventHandler(this.OnAssemblerImportCompleted);
            this.assembler.CancelAssembly += new EventHandler(this.OnAssemblerCancelAssembly);
            this.assembler.SaveSequence += new EventHandler<SaveSequenceInputEventArgs>(this.OnSaveSequenceClicked);
            this.assembler.FileUnloaded += new EventHandler<FileUnloadedEventArgs>(this.OnFileUnload);
            this.assembler.ExecuteSearchStarted += new EventHandler<WebServiceInputEventArgs>(this.OnExecuteSearch);
            this.assembler.CancelSearch += new EventHandler<WebServiceInputEventArgs>(this.OnAssemblerCancelSearch);
            this.assembler.UpdateSaveItemStatus += new EventHandler(this.OnAssemblerUpdateSaveItemStatus);
        }

        /// <summary>
        /// The event would update the Save item Status,Depeneding on whether a save 
        /// operation can be performed or not, enables or disables the save item.
        /// </summary>
        /// <param name="sender">Assembler pane</param>
        /// <param name="e">Event Data</param>
        private void OnAssemblerUpdateSaveItemStatus(object sender, EventArgs e)
        {
            if (this.fileMenu != null)
            {
                this.fileMenu.UpdateSaveStatus(this.assembler.SaveItemEnabled);
            }
        }

        /// <summary>
        /// This event is fired, when the Search event is cancelled
        /// </summary>
        /// <param name="sender">Cancel Button</param>
        /// <param name="e">event data</param>
        private void OnAssemblerCancelSearch(object sender, WebServiceInputEventArgs e)
        {
            IBlastServiceHandler blastServiceHandler =
                    GetWebServiceInstance(e.WebServiceName);
            try
            {
                blastServiceHandler.CancelRequest(this.requestIdentifier);
            }
            catch (Exception ex)
            {
                MessageDialogBox.Show(
                    String.Format(Properties.Resource.BLAST_CANCEL_FAILED, ex.Message),
                    Properties.Resource.CAPTION,
                    MessageDialogButton.OK);
            }
            finally
            {
                this.assembler.SearchCompleted(false);
            }
        }

        /// <summary>
        /// This method would remove the file from the file info collection so that, 
        /// User can reload the file
        /// </summary>
        /// <param name="sender">Assembler Instance</param>
        /// <param name="e">File Unload Events Args</param>
        private void OnFileUnload(object sender, FileUnloadedEventArgs e)
        {
            this.fileMenu.RemoveFileInfo(e.RemoveFileInfo);
        }

        /// <summary>
        /// This method will cancel the Assembling of sequences
        /// </summary>
        /// <param name="sender">IAssembler Instance</param>
        /// <param name="e">Event Data</param>
        private void OnAssemblerCancelAssembly(object sender, EventArgs e)
        {
            if (this.assemblerThread.IsBusy)
            {
                this.assemblerThread.CancelAsync();
            }
        }

        /// <summary>
        /// This event is raised by IAssembler when the 
        /// user wants to save a contig or a particular sequence.
        /// </summary>
        /// <param name="sender">IAssembler instance</param>
        /// <param name="e">Save data.</param>
        private void OnSaveSequenceClicked(object sender, SaveSequenceInputEventArgs e)
        {
            try
            {
                if (e.Sequences != null && !string.IsNullOrEmpty(e.FileName))
                {
                    SaveFile(e.Sequences, e.FileName);
                }
            }
            catch (Exception)
            {
                MessageDialogBox.Show(Properties.Resource.FORMATTING_ERROR, Properties.Resource.CAPTION, MessageDialogButton.OK);
            }
        }

        /// <summary>
        /// This Event will close the file menu, 
        /// </summary>
        /// <param name="sender">IAssembler Instance</param>
        /// <param name="e">Event Data</param>
        private void OnAssemblerImportCompleted(object sender, EventArgs e)
        {
            this.fileMenu.CloseDialogue();
            if (this.PopupClosed != null)
            {
                PopupEventArgs args = new PopupEventArgs(true);
                this.PopupClosed(sender, args);
            }
        }

        /// <summary>
        /// This method calls dispose on all objects 
        /// implementing IDisposable interface. This method
        /// has logic to prevent disposing objects multiple times.
        /// </summary>
        /// <param name="disposable">
        /// If true then it indicates non-framework
        /// code is calling dispose.
        /// </param>
        private void Dispose(bool disposable)
        {
            if (!this.disposed)
            {
                if (disposable)
                {
                    if (this.fileParserThread != null)
                    {
                        this.fileParserThread.Dispose();
                    }

                    if (this.assemblerThread != null)
                    {
                        this.assemblerThread.Dispose();
                    }

                    this.disposed = true;
                }
            }
        }

        #endregion -- Private Methods --
    }
}
