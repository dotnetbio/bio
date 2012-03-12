using BiodexExcel.Visualizations.Common.DialogBox;

namespace BiodexExcel
{
    #region -- Using Directive --

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;
    using Bio;
    using Bio.Algorithms.Alignment;
    using Bio.Algorithms.Assembly;
    using Bio.Algorithms.MUMmer;
    using Bio.IO;
    using Bio.IO.FastA;
    using Bio.IO.FastQ;
    using Bio.IO.GenBank;
    using Bio.IO.Gff;
    using Bio.Web;
    using Bio.Web.Blast;
    using BiodexExcel.Properties;
    using BiodexExcel.Visualizations.Common;
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.Excel;
    using Microsoft.Office.Tools.Ribbon;
    using Microsoft.Vbe.Interop;
    using Microsoft.Win32;

    #endregion -- Using Directive --

    /// <summary>
    /// BioRibbon represents the .NET Bio tab in the Excel main ribbon. This class is the heart
    /// of the workbench and controls the entire workflow of the Ribbon.
    /// </summary>
    public partial class BioRibbon
    {
        #region -- Private Members --

        /// <summary>
        /// Maximum width of any column when importing a file.
        /// </summary>
        private const int MaximumColumnWidth = 50;

        /// <summary>
        /// Dictionary key for genbank metadata
        /// </summary>
        private const string GenbankMetadataKey = "GenBank";

        /// <summary>
        /// Dictionary key for getting offset values
        /// </summary>
        private const string StartOffsetString = "StartOffsets";

        /// <summary>
        /// Dictionary key for getting offset values
        /// </summary>
        private const string EndOffsetString = "EndOffsets";

        /// <summary>
        /// URL for .NET Bio homepage
        /// </summary>
        private const string MBFHomePage = "http://bio.codeplex.com";

        /// <summary>
        /// Required version of NodeXL for venn tool
        /// </summary>
        private const String NodeXLTemplateRequiredVersion = "1.0.1.108";

        /// <summary>
        /// Flag to indicate if NodeXL is installed or not.
        /// </summary>
        private bool isNodeXLInstalled;

        /// <summary>
        /// Stores the number of english alphabets.
        /// </summary>
        private const int NumberOfAlphabets = 26;

        /// <summary>
        /// Stores the max length of a worksheet name.
        /// </summary>
        private const int MaxWorksheetNameLength = 30;

        /// <summary>
        /// Maximum number of columns supported by excel.
        /// </summary>
        private const int MaxExcelColumns = 15999;

        /// <summary>
        /// Indicates the zoom level for all the worksheets.
        /// </summary>
        private const int ZoomLevel = 70;

        /// <summary>
        /// Stores the value of the default color.
        /// </summary>
        private const double DefaultColorValue = 16777215.0;

        /// <summary>
        /// Key to get access to the cells context menu.
        /// </summary>
        private const string Cell = "Cell";

        /// <summary>
        /// Name of the macro which will draw a chart of frequencies of alphabets in the sequence.
        /// </summary>
        private const string DisplayChartMacroName = "DisplayChart";

        /// <summary>
        /// Maximum number of times object model call should be retried.
        /// (Retry 15 times in span of 10 minutes)
        /// </summary>
        private const int MaxRetryCount = 15;

        /// <summary>
        /// Base interval time between object model call retries
        /// </summary>
        private const int RetryInterval = 5000; // 1 Seconds

        /// <summary>
        /// Error code when object model call fails (VBA_E_IGNORE)
        /// </summary>
        private const int VbaIgnoreErrorCode = -2146777998;

        /// <summary>
        /// Indicates the maximum number of sequence characters that will be displayed in a single line.
        /// </summary>
        private static int maxNumberOfCharacters = 80;

        /// <summary>
        /// List of headers which will be displayed in a Sequence range sheet.
        /// </summary>
        private static List<string> rangeHeaders = new List<string>() { Properties.Resources.CHROM_ID, Properties.Resources.CHROM_START, Properties.Resources.CHROM_END, Properties.Resources.BED_NAME, Properties.Resources.BED_SCORE, Properties.Resources.BED_STRAND, Properties.Resources.BED_THICK_START, Properties.Resources.BED_THICK_END, Properties.Resources.BED_ITEM_RGB, Properties.Resources.BED_BLOCK_COUNT, Properties.Resources.BED_BLOCK_SIZES, Properties.Resources.BED_BLOCK_STARTS };

        /// <summary>
        /// List of headers which will be displayed in a BLAST sheet.
        /// </summary>
        private static string[] blastHeaders = { Resources.QUERY_ID, Resources.SUBJECT_ID, Resources.IDENTITY, Resources.ALIGNMENT, Resources.LENGTH, Resources.QSTART, Resources.QEND, Resources.SSTART, Resources.SEND, Resources.EVALUE, Resources.SCORE, Resources.GAPS };

        /// <summary>
        /// Lists the name of the first 40 columns.
        /// </summary>
        private static string[] columnAZ = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        /// <summary>
        /// Default background color for cells containing "A".
        /// </summary>
        private static Color defaultAColor = Color.Gold;

        /// <summary>
        /// Default background color for cells containing "G".
        /// </summary>
        private static Color defaultGColor = Color.Green;

        /// <summary>
        /// Default background color for cells containing "C".
        /// </summary>
        private static Color defaultCColor = Color.Crimson;

        /// <summary>
        /// Default background color for cells containing "T".
        /// </summary>
        private static Color defaultTColor = Color.Teal;

        /// <summary>
        /// Default background color for cells containing "U".
        /// </summary>
        private static Color defaultUColor = Color.Teal;

        /// <summary>
        /// Stores the default background color of the cells.
        /// </summary>
        private Color transparentColor;

        /// <summary>
        /// Refrences the background thread which will performs sequence assembly
        /// in the background.
        /// </summary>
        private BackgroundWorker assemblerThread;

        /// <summary>
        /// Refrences the background thread which will performs sequence alignment
        /// in the background.
        /// </summary>
        private BackgroundWorker alignerThread;

        /// <summary>
        /// Name of the webservice currently running.
        /// </summary>
        private string webserviceName;

        /// <summary>
        /// Number of times the BLAST service has been run in this instance. This number
        /// is appended to the name of the sheet.
        /// </summary>
        private int currentBlastSheetNumber = 1;

        /// <summary>
        /// Maintains a count of the number of times sequence files are imported in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int currentFileNumber = 1;

        /// <summary>
        /// Maintains a count of the number of times range-sequence files are merged in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int currentMergeSheetNumber = 1;

        /// <summary>
        /// Sheet number to append to a alignment result sheet name
        /// </summary>
        private int currentAlignResultSheetNumber = 1;

        /// <summary>
        /// Maintains a count of the number of times sequences were assembled in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int currentConsensusSheetNumber = 1;

        /// <summary>
        /// Maintains a count of the number of times intersect operation were performed in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int intersectSheetNumber = 1;

        /// <summary>
        /// Maintains a count of the number of times subtract operation were performed in this instance.
        /// This number is appended to the name of the sheet.
        /// </summary>
        private int subtractSheetNumber = 1;

        /// <summary>
        /// Stores the list of all file-names that have been imported into excel sheet.
        /// </summary>
        private List<string> fileNames = new List<string>();

        /// <summary>
        /// Identifier of the blast job submitted.
        /// </summary>
        private string requestIdentifier = string.Empty;

        /// <summary>
        /// Stores a list of all the controls which have to be enabled when a sheet
        /// which containing a sequence is being viewed.
        /// </summary>
        private List<RibbonControl> sequenceDataEnabledControls = new List<RibbonControl>();

        /// <summary>
        /// Stores a list of all the controls which have to be enabled when a sheet
        /// which containing a sequence-range is being viewed.
        /// </summary>
        private List<RibbonControl> sequenceRangeEnabledControls = new List<RibbonControl>();

        /// <summary>
        /// Stores a list of all the controls which have to be enabled when a sheet conatining contigs is being viewed.
        /// </summary>
        private List<RibbonControl> contigEnabledControls = new List<RibbonControl>();

        /// <summary>
        /// Indicates a value whether all sequence sheet should be re-aligned or not.
        /// </summary>
        private bool alignAllSequenceSheet = false;

        /// <summary>
        /// Stores mapping of a particulat molecule name and the colors used. 
        /// </summary>
        private Dictionary<byte, Color> colorMap = new Dictionary<byte, Color>();

        /// <summary>
        /// Holds the number of sequences we import onto a single worksheet.
        /// </summary>
        private int sequencesPerWorksheet = 1;

        /// <summary>
        /// Stores information regarding the enabled state of 
        /// Cancel button in Blast Search (btnCancelSearch)
        /// </summary>
        private bool cancelSearchButtonState = false;

        /// <summary>
        /// Stores information regarding the enabled state of 
        /// Cancel button in Alignment (btnCancelAlign)
        /// </summary>
        private bool cancelAlignButtonState = false;

        /// <summary>
        /// Stores information regarding the enabled state of 
        /// Cancel button in Assembly
        /// </summary>
        private bool cancelAssemblyButtonState = false;

        /// <summary>
        /// Key containing installation path of .NET Bio
        /// </summary>
        public static string MBFInstallationPath
        {
            get
            {
                //typical path is Program Files\Microsoft Biology Initiative\Microsoft Biology Framework
                var assembly = Assembly.GetEntryAssembly();

                if (assembly != null)
                    return Path.GetDirectoryName(assembly.Location);

                string codeBase = Assembly.GetCallingAssembly().CodeBase.ToString();
                Uri uri = new Uri(codeBase);

                // BioExcel specific
                if (codeBase.Contains("exce..vsto"))
                {
                    //look into [HKEY_CURRENT_USER\Software\Microsoft\Office\Excel\Addins\BioExcel]
                    RegistryKey regKeyAppRoot = Registry.CurrentUser.OpenSubKey
                        (@"Software\Microsoft\Office\Excel\Addins\BioExcel");
                    uri = new Uri(regKeyAppRoot.GetValue("Manifest").ToString());
                }
                return Uri.UnescapeDataString(Path.GetDirectoryName(uri.AbsolutePath));

            }
        }

        #endregion -- Private Members --

        #region -- Constructor --

        /// <summary>
        /// Initializes a new instance of the BioRibbon class.
        /// </summary>
        public BioRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
            this.BuildRibbonTabs();
            this.btnCancelAlign.Click += new RibbonControlEventHandler(this.OnCancelAlignClicked);
            this.btnCancelAssemble.Click += new RibbonControlEventHandler(this.OnCancelAssembleClicked);
            this.btnCancelSearch.Click += new RibbonControlEventHandler(this.OnCancelSearchClicked);
            this.btnMaxColumn.Click += new RibbonControlEventHandler(this.OnSizeChangedClicked);
            this.btnRunChartMacro.Click += new RibbonControlEventHandler(this.RunMacro);
            this.btnAbout.Click += new RibbonControlEventHandler(this.OnAboutClick);
            this.btnMerge.Click += new RibbonControlEventHandler(this.OnMergeRangeSequence);
            this.btnIntersect.Click += new RibbonControlEventHandler(this.OnIntersectClick);
            this.btnSubtract.Click += new RibbonControlEventHandler(this.OnSubtractClick);
            this.btnConfigureColor.Click += new RibbonControlEventHandler(this.OnConfigureColorClick);
            this.btnConfigureImport.Click += new RibbonControlEventHandler(this.OnConfigureImportOptions);
            this.btnVennDiagram.Click += new RibbonControlEventHandler(OnVennDiagramClick);
            this.btnHomePage.Click += new RibbonControlEventHandler(OnHomePageClick);
            this.btnAssemble.Click += new RibbonControlEventHandler(OnAssembleClick);

            this.BuildColorScheme();

            this.SetScreenTips();
        }

        #endregion -- Constructor --

        #region -- Private Static Methods --

        /// <summary>
        /// This method writes a given value onto a given range on a excel sheet.
        /// </summary>
        /// <param name="range">Range instance.</param>
        /// <param name="value">Value to be written to that range.</param>
        private static void WriteRangeValue(Range range, string value)
        {
            range.set_Value(XlRangeValueDataType.xlRangeValueDefault, value);
        }

        /// <summary>
        /// This method generates the column name string when given the column number
        /// as input.
        /// </summary>
        /// <param name="number">Column number</param>
        /// <returns>Column name</returns>
        private static string GetColumnString(long number)
        {
            StringBuilder value = new StringBuilder();

            while (number > 0)
            {
                long mod = number % NumberOfAlphabets;
                if (mod == 0)
                {
                    value.Append("Z");
                    number = number / NumberOfAlphabets;
                    number--;
                }
                else
                {
                    value.Insert(0, columnAZ[mod - 1]);
                    number = number / NumberOfAlphabets;
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// This method displays a single row of NCBI\EBI result data.
        /// </summary>
        /// <param name="currentsheet">The worksheet instance.</param>
        /// <param name="rowNumber">The row number from where the BLAST result rendering has to begin</param>
        /// <param name="values">The result values.</param>
        private static void DisplayValues(Worksheet currentsheet, int rowNumber, params string[] values)
        {
            int columnNumber = 2;

            foreach (string value in values)
            {
                Range range = currentsheet.get_Range(GetColumnString(columnNumber) + rowNumber, Type.Missing);
                WriteRangeValue(range, value);
                columnNumber++;
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
        private static IDeNovoAssembly RunAssembly(AssemblyInputEventArgs input, BackgroundWorker worker)
        {
            double mergeThreshold = input.MergeThreshold;
            List<ISequence> sequence = input.Sequences.ToList();

            OverlapDeNovoAssembler assemble = new OverlapDeNovoAssembler();
            assemble.OverlapAlgorithm = input.Aligner;

            // Special casing for SW alignment.
            if (assemble.OverlapAlgorithm is SmithWatermanAligner)
            {
                // If we set the Threshold value lesser than the Max score, then the result will be “JUNK”.
                // So setting the threshold value to 25 approximately supports sequence length of 15,0000.
                mergeThreshold = 25;
            }

            assemble.MergeThreshold = mergeThreshold;
            assemble.OverlapAlgorithm.SimilarityMatrix = input.AlignerInput.SimilarityMatrix;

            assemble.OverlapAlgorithm.GapOpenCost = input.AlignerInput.GapCost;
            assemble.OverlapAlgorithm.GapExtensionCost = input.AlignerInput.GapExtensionCost;

            assemble.ConsensusResolver = new SimpleConsensusResolver(input.ConsensusThreshold);
            assemble.AssumeStandardOrientation = false;

            AssignAlignerParameter(assemble.OverlapAlgorithm, input.AlignerInput);

            IDeNovoAssembly assemblyOutput = assemble.Assemble(sequence);

            if (worker != null && worker.CancellationPending == true)
            {
                return null;
            }

            return assemblyOutput;
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
        /// This method checks if the BLAST search has yielded
        /// any results or not.
        /// </summary>
        /// <param name="blastResults">List of BlastResult objects.</param>
        /// <returns>
        /// true: if the search has results, false otherwise.
        /// </returns>
        private static bool BlastHasResults(List<BlastResult> blastResults)
        {
            bool resultsFound = false;

            foreach (BlastResult result in blastResults)
            {
                foreach (BlastSearchRecord record in result.Records)
                {
                    if (null != record.Hits
                            && 0 < record.Hits.Count)
                    {
                        foreach (Hit hit in record.Hits)
                        {
                            if (null != hit.Hsps
                                    && 0 < hit.Hsps.Count)
                            {
                                foreach (Hsp hsp in hit.Hsps)
                                {
                                    resultsFound = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return resultsFound;
        }

        /// <summary>
        /// Extracts the metadata in a SequenceRange object.
        /// </summary>
        /// <param name="range">Range object whose metadata has to be extracted.</param>
        /// <param name="key">Key for the metadata.</param>
        /// <returns>Value of the metadata.</returns>
        private static object ExtractRangeMetadata(ISequenceRange range, string key)
        {
            object metadataValue = null;

            if (range != null && range.Metadata != null && !string.IsNullOrEmpty(key) && range.Metadata.ContainsKey(key))
            {
                metadataValue = range.Metadata[key];
            }

            return metadataValue;
        }

        /// <summary>
        /// Gets a list of worksheets when given a list of worksheet names.
        /// </summary>
        /// <param name="sheetNames">List of sheet names</param>
        /// <returns>List of reference to worksheets with given names.</returns>
        private static List<Worksheet> GetWorksheetList(List<string> sheetNames)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Sheets sheets = workBook.Worksheets;

            List<Worksheet> worksheetList = new List<Worksheet>();
            foreach (string sheetName in sheetNames)
            {
                worksheetList.Add(GetSheetReference(sheetName));
            }

            return worksheetList;
        }

        /// <summary>
        ///  Gets a reference to worksheet which has the same name as the sheet name.
        /// </summary>
        /// <param name="sheetName">Name of the worksheet.</param>
        /// <returns>Reference to worksheet.</returns>
        private static Worksheet GetSheetReference(string sheetName)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Sheets sheets = workBook.Worksheets;
            Worksheet finalSheet = null;
            for (int i = 1; i <= sheets.Count; i++)
            {
                Worksheet sheet = sheets[i] as Worksheet;
                if (sheet != null && sheet.Name.Equals(sheetName))
                {
                    finalSheet = sheet;
                    break;
                }
            }

            return finalSheet;
        }

        #endregion --Private Static Methods--

        #region -- Private Methods --

        /// <summary>
        /// Navigate to .NET Bio homepage using default browser.
        /// </summary>
        void OnHomePageClick(object sender, RibbonControlEventArgs e)
        {
            System.Diagnostics.Process.Start(MBFHomePage);
        }

        /// <summary>
        /// Set tool tips for UI items.
        /// </summary>
        private void SetScreenTips()
        {
            this.btnAbout.ScreenTip = Resources.MENU_ABOUT_TIP;
            this.btnAbout.SuperTip = Resources.MENU_ABOUT_SUPERTIP;
            this.btnConfigureColor.SuperTip = Resources.MENU_CONF_COLOR_SUPERTIP;
            this.btnMaxColumn.SuperTip = Resources.MENU_CONF_COLWRAP_SUPERTIP;
            this.splitConfiguration.ScreenTip = Resources.MENU_CONF_TIP;
            this.splitConfiguration.SuperTip = Resources.MENU_CONF_SUPERTIP;
            this.splitOperate.ScreenTip = Resources.MENU_BED_TIP;
            this.splitOperate.SuperTip = Resources.MENU_BED_SUPERTIP;
            this.splitChart.ScreenTip = Resources.MENU_CHART_TIP;
            this.splitChart.SuperTip = Resources.MENU_CHART_SUPERTIP;
            this.btnCancelSearch.ScreenTip = Resources.MENU_CANCELBLAST_TIP;
            this.btnCancelAlign.ScreenTip = Resources.MENU_CANCELALIGN_TIP;
            this.splitAligners.ScreenTip = Resources.MENU_ALIGNERS_TIP;
            this.splitAligners.SuperTip = Resources.MENU_ALIGNERS_SUPERTIP;
            this.btnAssemble.ScreenTip = Resources.MENU_ASSEMBLERS_TIP;
            this.btnAssemble.SuperTip = Resources.MENU_ASSEMBLERS_SUPERTIP;
            this.splitWebService.ScreenTip = Resources.MENU_BLAST_TIP;
            this.splitImport.ScreenTip = Resources.MENU_IMPORT_TIP;
            this.splitWebService.SuperTip = Resources.MENU_BLAST_SUPERTIP;
            this.splitExport.ScreenTip = Resources.MENU_EXPORT_TIP;
            this.splitExport.SuperTip = Resources.MENU_EXPORT_SUPERTIP;
            this.splitImport.SuperTip = Resources.MENU_IMPORT_SUPERTIP;
            this.btnVennDiagram.ScreenTip = Resources.MENU_VENN_TIP;
            this.btnVennDiagram.SuperTip = Resources.MENU_VENN_SUPERTIP;

            this.btnMerge.Description = Resources.MENU_MERGE_DESC;
            this.btnMerge.ScreenTip = Resources.SCREEN_TIP_MERGE;
            this.btnSubtract.Description = Resources.MENU_SUBTRACT_DESC;
            this.btnSubtract.ScreenTip = Resources.SCREEN_TIP_SUBTRACT;
            this.btnIntersect.Description = Resources.MENU_INTERSECT_DESC;
            this.btnIntersect.ScreenTip = Resources.SCREEN_TIP_INTERSECT;
        }

        /// <summary>
        /// This method is called when the entire ribbon is loaded.
        /// At this point of time all our UI elements will be properly 
        /// initialized.
        /// </summary>
        /// <param name="sender">.NET Bio Ribbon.</param>
        /// <param name="e">Event data.</param>
        private void OnRibbonLoad(object sender, RibbonUIEventArgs e)
        {
            this.CheckForNodeXL();
            EnableAllControls();

            // Adds a event handler every time a new sheet is selected.
            Globals.ThisAddIn.Application.WorkbookActivate += new AppEvents_WorkbookActivateEventHandler(this.OnWorkBookOpen);
            Globals.ThisAddIn.Application.SheetChange += new AppEvents_SheetChangeEventHandler(SequenceCache.OnSheetDataChanged);
            Globals.ThisAddIn.Application.WorkbookDeactivate += new AppEvents_WorkbookDeactivateEventHandler(this.OnWorkbookDeactivate);
            SequenceCache.Initialize();
        }

        /// <summary>
        /// Get the location of nodeXL template file
        /// </summary>
        /// <returns>Path to nodeXL template file</returns>
        private string GetNodeXLTemplatePath()
        {
            string programFilesFolder;

            if (IntPtr.Size == 4)
            {
                // NodeXL is running within 32-bit Excel.
                programFilesFolder = Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles);
            }
            else
            {
                // NodeXL is running within 64-bit Excel.
                programFilesFolder = System.Environment.GetEnvironmentVariable(
                    "ProgramFiles(x86)");
            }

            if (string.IsNullOrWhiteSpace(programFilesFolder))
            {
                programFilesFolder = string.Empty;
            }

            string templatePath = Path.Combine(programFilesFolder, @"Microsoft Research\Microsoft NodeXL Excel Template\NodeXLGraph.xltx");
            if (File.Exists(templatePath))
                return templatePath;

            templatePath = Path.Combine(Globals.ThisAddIn.Application.TemplatesPath, "NodeXLGraph.xltx");
            if (File.Exists(templatePath))
                return templatePath;

            return string.Empty;
        }

        /// <summary>
        /// Check if NodeXL is installed and set appropriate flags and messages.
        /// </summary>
        private void CheckForNodeXL()
        {
            if (File.Exists(GetNodeXLTemplatePath())) // check if it exists at all
            {
                isNodeXLInstalled = true;
            }
            else
            {
                this.btnVennDiagram.SuperTip = Resources.MENU_VENN_NodeXLmissing;
            }
        }

        /// <summary>
        /// This method builds a dictionary of all the molecule types supported by .NET Bio
        /// and associates a default color to each of the molecule types.
        /// </summary>
        private void BuildColorScheme()
        {
            System.Windows.Media.Color transparentShade = System.Windows.Media.Colors.Transparent;
            this.transparentColor = Color.FromArgb(transparentShade.A, transparentShade.R, transparentShade.G, transparentShade.B);
            foreach (byte nucleotide in DnaAlphabet.Instance)
            {
                if (!this.colorMap.ContainsKey(nucleotide))
                {
                    this.colorMap.Add(nucleotide, this.transparentColor);
                }
            }

            foreach (byte nucleotide in RnaAlphabet.Instance)
            {
                string nucleotideString = new string(new char[] { (char)nucleotide });
                if (!this.colorMap.ContainsKey(nucleotide))
                {
                    this.colorMap.Add(nucleotide, this.transparentColor);
                }
            }

            foreach (byte acid in ProteinAlphabet.Instance)
            {
                string nucleotideString = new string(new char[] { (char)acid });
                if (!this.colorMap.ContainsKey(acid))
                {
                    this.colorMap.Add(acid, this.transparentColor);
                }
            }

            byte alphabet = DnaAlphabet.Instance.A;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultAColor;
                //this.colorMap[(byte)'a'] = defaultAColor;
            }

            alphabet = DnaAlphabet.Instance.G;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultGColor;
            }

            alphabet = DnaAlphabet.Instance.T;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultTColor;
            }

            alphabet = RnaAlphabet.Instance.U;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultUColor;
            }

            alphabet = DnaAlphabet.Instance.C;
            if (this.colorMap.ContainsKey(alphabet))
            {
                this.colorMap[alphabet] = defaultCColor;
            }
        }

        /// <summary>
        /// This method pops up a dialog for the user to change import
        /// options.
        /// </summary>
        /// <param name="sender">Configure import options button</param>
        /// <param name="e">Event data</param>
        private void OnConfigureImportOptions(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;

            ImportConfiguration configuration = new ImportConfiguration() { SequencesPerWorksheet = sequencesPerWorksheet };
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(configuration);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            configuration.Activated += new EventHandler(OnWPFWindowActivated);
            if (configuration.ShowDialog() == true)
            {
                sequencesPerWorksheet = configuration.SequencesPerWorksheet;
            }
        }

        /// <summary>
        /// This method pops up a dialog for the user which enables him
        /// to select the color scheme for molecule types.
        /// </summary>
        /// <param name="sender">Configure color button.</param>
        /// <param name="e">Event data.</param>
        private void OnConfigureColorClick(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            Worksheet sheet = Globals.ThisAddIn.Application.ActiveSheet as Worksheet;

            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            ColorConfiguration configuration = new ColorConfiguration(Globals.ThisAddIn.Application, this.colorMap);
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(configuration);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            configuration.Activated += new EventHandler(OnWPFWindowActivated);
            if (configuration.Show())
            {
                this.colorMap = configuration.ColorMap;

                foreach (Range selectionArea in (Globals.ThisAddIn.Application.Selection as Range).Areas)
                {
                    this.FillBackGroundColor(selectionArea);
                }
            }
        }

        #region -- Enable-Disable Controls --

        /// <summary>
        /// This method enables the given list of controls.
        /// </summary>
        /// <param name="controlsList">List of controls which have to be enabled</param>
        private void EnableAllControls()
        {
            List<RibbonGroup> groups = new List<RibbonGroup>();

            groups.Add(this.groupImportExport);
            groups.Add(this.groupAlignment);
            groups.Add(this.groupWebService);
            groups.Add(this.groupCharts);
            groups.Add(this.grpGenomicInterval);
            groups.Add(this.groupConfig);
            groups.Add(this.groupAssembly);

            foreach (RibbonGroup group in groups)
            {
                foreach (RibbonControl control in group.Items)
                {
                    control.Enabled = true;
                }
            }

            this.btnVennDiagram.Enabled = this.isNodeXLInstalled;
            this.btnCancelSearch.Enabled = this.cancelSearchButtonState;
            this.btnCancelAlign.Enabled = this.cancelAlignButtonState;
            this.btnCancelAssemble.Enabled = this.cancelAssemblyButtonState;
        }

        /// <summary>
        /// This method disables all controls in the ribbon including generic controls.
        /// </summary>
        private void DisableAllControls()
        {
            List<RibbonGroup> groups = new List<RibbonGroup>();

            groups.Add(this.groupImportExport);
            groups.Add(this.groupAlignment);
            groups.Add(this.groupWebService);
            groups.Add(this.groupCharts);
            groups.Add(this.grpGenomicInterval);
            groups.Add(this.groupConfig);
            groups.Add(this.groupAssembly);
            groups.Add(this.groupNodeXL);

            foreach (RibbonGroup group in groups)
            {
                foreach (RibbonControl control in group.Items)
                {
                    control.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Raised when a workbook switches to another one or when its closed.
        /// </summary>
        /// <param name="Wb">workbook being deactivated</param>
        private void OnWorkbookDeactivate(Workbook Wb)
        {
            if (Globals.ThisAddIn.Application.Workbooks.Count == 1) // If count is one and is deactivating, it is closing!
            {
                this.DisableAllControls();
            }
        }

        /// <summary>
        /// This event is fired when a workbook is activated.
        /// this event enables all controls in the Generic group.
        /// </summary>
        /// <param name="workBook">Workbook being opened.</param>
        private void OnWorkBookOpen(Workbook workBook)
        {
            this.EnableAllControls();

            this.LoadSheetNames(Globals.ThisAddIn.Application.ActiveWorkbook);
        }

        #endregion -- Enable-Disable groups --

        /// <summary>
        /// This method Intersect the intervals of 2 queries
        /// </summary>
        /// <param name="sender">btnIntersect instance.</param>
        /// <param name="e">Event data</param>
        private void OnIntersectClick(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            InputSelection inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            inputs.SequenceLabels = new string[] { Resources.InputSelection_SequenceLabel_BED1, Resources.InputSelection_SequenceLabel_BED2, Resources.Export_BED_SequenceRangeString }; // Set labels for the sequences
            inputs.GetInputSequenceRanges(DoBEDIntersect, true, true, true);
        }

        /// <summary>
        /// Callback method from input selection model which will actually do the selected operation.
        /// </summary>
        /// <param name="e">Event Argument</param>
        private void DoBEDIntersect(InputSequenceRangeSelectionEventArg e)
        {
            this.ScreenUpdate(false);

            Workbook currentWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Sheets sheets = currentWorkbook.Worksheets;
            IntersectOutputType intersectOutputType = IntersectOutputType.OverlappingIntervals;
            List<SequenceRangeGrouping> result = new List<SequenceRangeGrouping>();

            if ((bool)e.Parameter[InputSelection.OVERLAP])
            {
                intersectOutputType = IntersectOutputType.OverlappingPiecesOfIntervals;
            }

            for (int i = 1; i < e.Sequences.Count; i++)
            {
                if (0 == result.Count)
                {
                    result.Add(e.Sequences[i - 1].Intersect(
                        e.Sequences[i],
                        (long)e.Parameter[InputSelection.MINIMUMOVERLAP],
                        intersectOutputType,
                        true));
                }
                else
                {
                    result.Add(result[result.Count - 1].Intersect(
                        e.Sequences[i],
                        (long)e.Parameter[InputSelection.MINIMUMOVERLAP],
                        intersectOutputType,
                        true));
                }
            }

            string sheetName = Resources.INTERSECT_SHEET + this.intersectSheetNumber.ToString(CultureInfo.CurrentCulture);
            this.intersectSheetNumber++;
            this.WriteSequenceRange(currentWorkbook,
                    sheetName,
                    result[result.Count - 1],
                    e.Data,
                    true,
                    false);

            this.ScreenUpdate(true);
        }

        /// <summary>
        /// This method Subtracts two query regions.
        /// </summary>
        /// <param name="sender">btnSubtract instance.</param>
        /// <param name="e">Event data</param>
        private void OnSubtractClick(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();

            InputSelection inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            inputs.MaximumSequenceCount = 2;
            inputs.SequenceLabels = new string[] { Resources.InputSelection_SequenceLabel_BED1, Resources.InputSelection_SequenceLabel_BED2, Resources.Export_BED_SequenceRangeString };
            inputs.GetInputSequenceRanges(DoBEDSubtract, false, true, true);
        }

        /// <summary>
        /// Callback method from input selection model which will actually do the selected operation.
        /// </summary>
        /// <param name="e">Event Argument</param>
        private void DoBEDSubtract(InputSequenceRangeSelectionEventArg e)
        {
            this.ScreenUpdate(false);

            Workbook currentWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Sheets allSheets = currentWorkbook.Worksheets;
            SubtractOutputType subtractOutputType = SubtractOutputType.NonOverlappingPiecesOfIntervals;
            SequenceRangeGrouping result = null;

            if ((bool)e.Parameter[InputSelection.OVERLAP])
            {
                subtractOutputType = SubtractOutputType.IntervalsWithNoOverlap;
            }

            result = e.Sequences[0].Subtract(
                    e.Sequences[1],
                    (long)e.Parameter[InputSelection.MINIMUMOVERLAP],
                    subtractOutputType,
                    true);

            string sheetName = Resources.SUBTRACT_SHEET + this.subtractSheetNumber.ToString(CultureInfo.CurrentCulture);
            this.subtractSheetNumber++;
            this.WriteSequenceRange(currentWorkbook,
                    sheetName,
                    result,
                    e.Data,
                    true,
                    false);

            this.ScreenUpdate(true);
        }

        /// <summary>
        /// Clear cached list of sheet name and load the new names.
        /// </summary>
        /// <param name="book">Workbook which contains a bunch of sheets.</param>
        private void LoadSheetNames(Workbook book)
        {
            this.fileNames.Clear();

            foreach (Worksheet sheet in book.Sheets)
            {
                this.fileNames.Add(sheet.Name);
            }
        }

        /// <summary>
        /// OnMergeRangeSequence Merges the overlapping intervals of a query.
        /// </summary>
        /// <param name="sender">btnMerge Instance.</param>
        /// <param name="e">Event data</param>
        private void OnMergeRangeSequence(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();

            InputSelection inputs = new InputSelection();
            inputs.MinimumSequenceCount = 1;
            inputs.SequenceLabels = new string[] { Resources.InputSelection_SequenceLabel_BED1, Resources.Export_BED_SequenceRangeString };
            inputs.GetInputSequenceRanges(DoBEDMerge, false, false, true);
        }

        /// <summary>
        /// Callback method from input selection model which will actually do the selected operation.
        /// </summary>
        /// <param name="e">Event Argument</param>
        private void DoBEDMerge(InputSequenceRangeSelectionEventArg e)
        {
            this.ScreenUpdate(false);

            Workbook currentWorkbook = Globals.ThisAddIn.Application.ActiveWorkbook;

            if (e.Sequences.Count > 1)
            {
                SequenceRangeGrouping referenceGrouping = e.Sequences[0]; // First item is mentioned as reference sequence when calling the dialog.
                for (int i = 1; i < e.Sequences.Count; i++)
                {
                    SequenceRangeGrouping rangeGrouping = e.Sequences[i];
                    referenceGrouping = referenceGrouping.MergeOverlaps(
                            rangeGrouping,
                            (long)e.Parameter[InputSelection.MINIMUMOVERLAP],
                            true);
                }

                string sheetName = Resources.MERGED_SHEET + this.currentMergeSheetNumber.ToString(CultureInfo.CurrentCulture);
                this.currentMergeSheetNumber++;
                this.WriteSequenceRange(currentWorkbook,
                        sheetName,
                        referenceGrouping,
                        e.Data,
                        false,
                        true);
            }
            else
            {
                SequenceRangeGrouping mergedOverlap = e.Sequences[0];

                if (mergedOverlap != null)
                {
                    string sheetName = Resources.MERGED_SHEET + this.currentMergeSheetNumber.ToString(CultureInfo.CurrentCulture);
                    this.currentMergeSheetNumber++;
                    mergedOverlap = mergedOverlap.MergeOverlaps((long)e.Parameter[InputSelection.MINIMUMOVERLAP], true);
                    this.WriteSequenceRange(currentWorkbook,
                            sheetName,
                            mergedOverlap,
                            e.Data,
                            false,
                            true);
                }
            }

            this.ScreenUpdate(true);
        }

        /// <summary>
        /// Formats and writes the query region (Output of Merge/Subtract/Intersect) operations
        /// </summary>
        /// <param name="resultWorkbook">Workbook to which Range has to be written
        /// </param>
        /// <param name="resultSheetname">New worksheet name</param>
        /// <param name="resultGroup">Output group</param>
        /// <param name="groupsData">
        /// Complete input groups information
        /// Contains individual Group, sheet and addresses of ISequenceRange
        /// </param>
        private void WriteSequenceRange(
                Workbook resultWorkbook,
                string resultSheetname,
                SequenceRangeGrouping resultGroup,
                Dictionary<SequenceRangeGrouping, GroupData> groupsData,
                bool showMetadata,
                bool showBasePairCount)
        {
            if (resultGroup.GroupIDs.Count() > 0)
            {
                int baseRowIndex = 2, baseColumnIndex = 2;
                int dataRowIndex = 0, dataColumnIndex = 0;
                int totalColumnCount = 0;
                object[,] values;
                List<string> hyperlinks = null;
                List<ISequenceRange> resultSequenceRanges = null;
                Dictionary<SequenceRangeGrouping, Dictionary<string, int>> groupSheetIndices = null;
                Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>> columnData = null;
                Dictionary<ISequenceRange, string> rangedata = null;
                Dictionary<SequenceRangeGrouping, SequenceRangeGrouping> allSheetData = null;
                Dictionary<SequenceRangeGrouping, int> allSheetCount = null;
                SequenceRangeGrouping groupToMerge = null;
                SequenceRangeGrouping referenceGroup = null;
                SequenceRangeGrouping queryGroup = null;
                SequenceRangeGrouping sheetGroup = null;
                int sheetCount = 0;
                Range activeRange = null;
                Worksheet resultWorksheet = resultWorkbook.Worksheets.Add(
                        Type.Missing,
                        resultWorkbook.Worksheets.get_Item(resultWorkbook.Worksheets.Count),
                        Type.Missing,
                        Type.Missing) as Worksheet;
                ((Microsoft.Office.Interop.Excel._Worksheet)resultWorksheet).Activate();
                resultWorksheet.Name = resultSheetname;
                activeRange = resultWorksheet.get_Range(GetColumnString(baseColumnIndex) + baseRowIndex, Type.Missing);

                rangedata = groupsData.Values.Select(gd => gd.Metadata) // Get the Metadata
                        .SelectMany(sd => sd.Values).ToList() // Get the Dictionary
                        .SelectMany(rd => rd).ToList().ToDictionary(k => k.Key, v => v.Value); // Convert to dictionary

                groupSheetIndices = new Dictionary<SequenceRangeGrouping, Dictionary<string, int>>();
                baseRowIndex = WriteSequenceRangeHeader(resultWorksheet,
                        groupSheetIndices,
                        groupsData,
                        baseRowIndex,
                        baseColumnIndex,
                        ref totalColumnCount,
                        showMetadata,
                        showBasePairCount);
                totalColumnCount -= (baseColumnIndex - 1);

                foreach (string resultGroupKey in resultGroup.GroupIDs)
                {
                    resultSequenceRanges = resultGroup.GetGroup(resultGroupKey);
                    dataRowIndex = 0;
                    values = new object[resultSequenceRanges.Count, totalColumnCount];
                    activeRange = resultWorksheet.get_Range(GetColumnString(baseColumnIndex) + baseRowIndex, Missing.Value);
                    activeRange = activeRange.get_Resize(resultSequenceRanges.Count, totalColumnCount);

                    foreach (ISequenceRange resultSequenceRange in resultSequenceRanges)
                    {
                        referenceGroup = null;
                        queryGroup = null;
                        dataColumnIndex = 0;
                        allSheetData = new Dictionary<SequenceRangeGrouping, SequenceRangeGrouping>();
                        allSheetCount = new Dictionary<SequenceRangeGrouping, int>();

                        values[dataRowIndex, dataColumnIndex] = resultSequenceRange.ID;
                        dataColumnIndex++;
                        values[dataRowIndex, dataColumnIndex] = resultSequenceRange.Start;
                        dataColumnIndex++;
                        values[dataRowIndex, dataColumnIndex] = resultSequenceRange.End;
                        dataColumnIndex++;

                        if (showMetadata)
                        {
                            for (int index = 3; index < rangeHeaders.Count; index++)
                            {
                                values[dataRowIndex, dataColumnIndex] =
                                        ExtractRangeMetadata(resultSequenceRange, rangeHeaders[index]);
                                dataColumnIndex++;
                            }
                        }

                        columnData = PrepareSequenceRowRange(groupsData, groupSheetIndices, rangedata, resultSequenceRange);

                        foreach (KeyValuePair<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>> columnGroup in columnData)
                        {
                            if (showBasePairCount)
                            {
                                // Get the parent ranges for Group's range in a column
                                groupToMerge = new SequenceRangeGrouping(columnGroup.Value.Item3);
                                if (1 < columnGroup.Value.Item3.Count)
                                {
                                    groupToMerge = groupToMerge.MergeOverlaps(0, false);
                                }

                                // Render data for Group's range in a column
                                values[dataRowIndex, columnGroup.Key] = groupToMerge.GroupRanges.Sum(sr => sr.End - sr.Start);
                                values[dataRowIndex, columnGroup.Key + 1] = columnGroup.Value.Item3.Count;
                            }
                            else
                            {
                                values[dataRowIndex, columnGroup.Key] = columnGroup.Value.Item3.Count;
                            }

                            // Let the hyperlink added
                            hyperlinks = new List<string>();
                            foreach (ISequenceRange range in columnGroup.Value.Item3)
                            {
                                hyperlinks.AddRange(rangedata[range].Split(','));
                            }

                            ShowHyperlink(hyperlinks,
                                    activeRange,
                                    columnGroup.Key,
                                    dataRowIndex,
                                    showBasePairCount);

                            if (showBasePairCount)
                            {
                                // Calculate data for all group
                                if (allSheetData.TryGetValue(columnGroup.Value.Item1, out sheetGroup))
                                {
                                    allSheetData[columnGroup.Value.Item1] = sheetGroup.MergeOverlaps(groupToMerge, 0, false);
                                }
                                else
                                {
                                    allSheetData[columnGroup.Value.Item1] = groupToMerge;
                                }

                                // Build up reference & query groups (later get common range using this)
                                if (columnGroup.Value.Item2)
                                {
                                    if (null == referenceGroup)
                                    {
                                        referenceGroup = groupToMerge;
                                    }
                                    else
                                    {
                                        referenceGroup = referenceGroup.MergeOverlaps(groupToMerge, 0, false);
                                    }
                                }
                                else
                                {
                                    if (null == queryGroup)
                                    {
                                        queryGroup = groupToMerge;
                                    }
                                    else
                                    {
                                        queryGroup = queryGroup.MergeOverlaps(groupToMerge, 0, false);
                                    }
                                }
                            }

                            // Calculate range count for all group
                            if (allSheetCount.TryGetValue(columnGroup.Value.Item1, out sheetCount))
                            {
                                allSheetCount[columnGroup.Value.Item1] += columnGroup.Value.Item3.Count;
                            }
                            else
                            {
                                allSheetCount[columnGroup.Value.Item1] = columnGroup.Value.Item3.Count;
                            }
                        }

                        // Render all columns in SequenceRangeGrouping
                        foreach (KeyValuePair<SequenceRangeGrouping, int> allData in allSheetCount)
                        {
                            dataColumnIndex = groupSheetIndices[allData.Key].Values.Min() - (showBasePairCount ? 2 : 1);
                            if (showBasePairCount)
                            {
                                values[dataRowIndex, dataColumnIndex] = allSheetData[allData.Key].GroupRanges.Sum(sr => sr.End - sr.Start);
                                dataColumnIndex++;
                            }
                            values[dataRowIndex, dataColumnIndex] = allData.Value;
                        }

                        if (showBasePairCount)
                        {
                            // Render common column in SequenceRangeGrouping
                            if (null != referenceGroup && null != queryGroup)
                            {
                                referenceGroup = referenceGroup.Intersect(
                                        queryGroup,
                                        0,
                                        IntersectOutputType.OverlappingPiecesOfIntervals,
                                        false);
                                values[dataRowIndex, totalColumnCount - 1] = referenceGroup.GroupRanges.Sum(sr => sr.End - sr.Start);
                            }
                            else
                            {
                                values[dataRowIndex, totalColumnCount - 1] = 0;
                            }
                        }

                        dataRowIndex++;
                    }

                    activeRange.set_Value(Missing.Value, values);

                    baseRowIndex += dataRowIndex;
                }

                resultWorksheet.Columns.AutoFit();
                NormalizeColumWidths(resultWorksheet.UsedRange);
                this.EnableAllControls();
            }
            else
            {
                MessageBox.Show(Properties.Resources.NO_RESULT, Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        Regex regexAddress = new Regex(
                @"(?<Sheet>^.[^!]*)!\$(?<Column>.[^$]*)\$(?<Row>.[^$]*)$",
                RegexOptions.IgnoreCase);

        Regex regexRangeAddress = new Regex(
                @"(?<Sheet>^.[^!]*)!\$(?<Column>.[^$]*)\$(?<Row>.[^$]*):\$(?<Column1>.[^$]*)\$(?<Row1>.[^$]*)$",
                RegexOptions.IgnoreCase);

        /// <summary>
        /// There are range that can be merged (row-wise)
        /// </summary>
        /// <param name="hyperlinkList">Hyperlink list</param>
        /// <param name="activeRange">Active Range Object</param>
        /// <param name="columnIndex">Column index in Active range</param>
        /// <param name="dataRowIndex">Data row index in Active range</param>
        /// <param name="showBasePairCount">Show base pair count</param>
        private void ShowHyperlink(List<string> hyperlinkList,
                Range activeRange,
                int columnIndex,
                int dataRowIndex,
                bool showBasePairCount)
        {
            // make sure the address are arranged in order
            hyperlinkList.Sort();

            string rangeAddress = string.Empty, lastAddress = string.Empty;
            string currentSheet = string.Empty, lastSheet = string.Empty;
            string currentColumn = string.Empty, currentColumn1 = string.Empty;
            string lastColumn = string.Empty, lastColumn1 = string.Empty;
            int currentRow = 0, currentRow1 = 0;
            int lastRow = 0, lastRow1 = 0;
            int adjustmentRowCount = 0;
            Match currentMatch = null, lastMatch = null;

            foreach (string hyperlink in hyperlinkList)
            {
                if (string.IsNullOrEmpty(lastAddress))
                {
                    lastAddress = hyperlink;
                    continue;
                }

                // Check if the lastAddress and hyperlink are adjacent (differ by one row)
                // They must be in same format 
                // (<Sheet>!$<Column>$<Row)
                currentMatch = regexAddress.Match(hyperlink);
                if (currentMatch.Success)
                {
                    currentSheet = currentMatch.Groups["Sheet"].Value;
                    currentColumn = currentMatch.Groups["Column"].Value;
                    currentColumn1 = currentColumn;
                    int.TryParse(currentMatch.Groups["Row"].Value, out currentRow);
                    currentRow1 = currentRow;
                }
                else
                {
                    // (<Sheet>!$<Column>$<Row>:$<Column1>$<Row1>)
                    currentMatch = regexRangeAddress.Match(hyperlink);
                    if (currentMatch.Success)
                    {
                        currentSheet = currentMatch.Groups["Sheet"].Value;
                        currentColumn = currentMatch.Groups["Column"].Value;
                        currentColumn1 = currentMatch.Groups["Column1"].Value;
                        int.TryParse(currentMatch.Groups["Row"].Value, out currentRow);
                        int.TryParse(currentMatch.Groups["Row1"].Value, out currentRow1);
                    }
                    else
                    {
                        currentSheet = currentColumn = currentColumn1 = string.Empty;
                        currentRow = currentRow1 = -1;
                    }
                }

                lastMatch = regexAddress.Match(lastAddress);
                if (lastMatch.Success)
                {
                    lastSheet = lastMatch.Groups["Sheet"].Value;
                    lastColumn = lastMatch.Groups["Column"].Value;
                    lastColumn1 = lastColumn;
                    int.TryParse(lastMatch.Groups["Row"].Value, out lastRow);
                    lastRow1 = lastRow;
                }
                else
                {
                    lastMatch = regexRangeAddress.Match(lastAddress);
                    if (lastMatch.Success)
                    {
                        lastSheet = lastMatch.Groups["Sheet"].Value;
                        lastColumn = lastMatch.Groups["Column"].Value;
                        lastColumn1 = lastMatch.Groups["Column1"].Value;
                        int.TryParse(lastMatch.Groups["Row"].Value, out lastRow);
                        int.TryParse(lastMatch.Groups["Row1"].Value, out lastRow1);
                    }
                    else
                    {
                        lastSheet = lastColumn = lastColumn1 = string.Empty;
                        lastRow = lastRow1 = -1;
                    }
                }

                if (0 == string.Compare(currentSheet, lastSheet, true)
                    && 0 == string.Compare(currentColumn, lastColumn, true)
                    && 0 == string.Compare(currentColumn1, lastColumn1, true)
                    && currentRow1 == lastRow + adjustmentRowCount + 1)
                {
                    lastAddress = string.Concat(currentSheet,
                            "!$",
                            lastColumn,
                            "$",
                            lastRow,
                            ":$",
                            currentColumn1,
                            "$",
                            currentRow1);
                    adjustmentRowCount++;
                    continue;
                }

                if (string.IsNullOrEmpty(rangeAddress))
                {
                    rangeAddress = HandleSpecialChars(lastAddress);
                }
                else
                {
                    rangeAddress = string.Concat(rangeAddress, ",", HandleSpecialChars(lastAddress));
                }

                lastAddress = hyperlink;
                adjustmentRowCount = 0;
            }

            if (string.IsNullOrEmpty(rangeAddress))
            {
                rangeAddress = HandleSpecialChars(lastAddress);
            }
            else
            {
                rangeAddress = string.Concat(rangeAddress, ",", HandleSpecialChars(lastAddress));
            }

            // Add hyperlink
            activeRange.Hyperlinks.Add(
                activeRange.get_Range(GetColumnString(columnIndex + (showBasePairCount ? 2 : 1)) + (dataRowIndex + 1), Missing.Value),
                    string.Empty,
                    rangeAddress,
                    Missing.Value,
                    Missing.Value);
        }

        /// <summary>
        /// Convert a range address into proper format by handling special chars
        /// </summary>
        /// <param name="rangeAddress">Source range address</param>
        /// <returns>Converted range address</returns>
        private string HandleSpecialChars(string rangeAddress)
        {
            int sheetNameSplitterIndex = rangeAddress.LastIndexOf('!');
            string sheetName = rangeAddress.Substring(0, sheetNameSplitterIndex).Replace("'", "''");
            rangeAddress = string.Format("'{0}'!{1}", sheetName, rangeAddress.Substring(sheetNameSplitterIndex + 1));
            return rangeAddress;
        }

        /// <summary>
        /// Prepares a row of SequenceRange for writing.
        /// </summary>
        /// <param name="groupsData">
        /// Complete input groups information
        /// Contains individual Group, sheet and addresses of ISequenceRange
        /// </param>
        /// <param name="groupSheetIndices">
        /// Complete indices.
        /// Contains individual column of each sheet of each Group
        /// </param>
        /// <param name="rangedata">Sequence and address list</param>
        /// <param name="resultSequenceRange">Query region that has to be prepared</param>
        /// <returns>Prepared data ready for output</returns>
        private static Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>> PrepareSequenceRowRange(
                Dictionary<SequenceRangeGrouping, GroupData> groupsData,
                Dictionary<SequenceRangeGrouping, Dictionary<string, int>> groupSheetIndices,
                Dictionary<ISequenceRange, string> rangedata,
                ISequenceRange resultSequenceRange)
        {
            Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>> columnData = null;

            columnData = new Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>>();
            foreach (ISequenceRange parentRange in resultSequenceRange.ParentSeqRanges)
            {
                PrepareSequenceRangeRow(groupsData, groupSheetIndices, rangedata, columnData, parentRange);
            }

            return columnData;
        }

        /// <summary>
        /// Prepares a row of SequenceRange for writing.
        /// </summary>
        /// <param name="groupsData">
        /// Complete input groups information
        /// Contains individual Group, sheet and addresses of ISequenceRange
        /// </param>
        /// <param name="groupSheetIndices">
        /// Complete indices.
        /// Contains individual column of each sheet of each Group
        /// </param>
        /// <param name="rangedata">Sequence and address list</param>
        /// <param name="columnData">Data ready for output</param>
        /// <param name="parentRange">Query region that has to be prepared</param>
        private static void PrepareSequenceRangeRow(
                Dictionary<SequenceRangeGrouping, GroupData> groupsData,
                Dictionary<SequenceRangeGrouping, Dictionary<string, int>> groupSheetIndices,
                Dictionary<ISequenceRange, string> rangedata,
                Dictionary<int, Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>> columnData,
                ISequenceRange parentRange)
        {
            var grp = groupsData.Keys.Where(s => s.GroupRanges.Contains(parentRange));
            if (0 == grp.Count())
            {
                foreach (ISequenceRange grandParent in parentRange.ParentSeqRanges)
                {
                    PrepareSequenceRangeRow(groupsData, groupSheetIndices, rangedata, columnData, grandParent);
                }

                return;
            }

            Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>> parentType = null; // Where the parent is ref / query
            List<ISequenceRange> parentRanges = null;
            SequenceRangeGrouping group = null;
            List<SequenceRangeGrouping> inputGroups = groupsData.Keys.ToList();

            // Regular expression to read the sheet name from address
            Regex regexSheetname = new Regex(@"(?<Sheetname>^.[^!]*)", RegexOptions.IgnoreCase);
            Match matchSheetname = null;
            Dictionary<string, int> sheetIndices = null;
            string sheetName = string.Empty;
            int sheetIndex;

            group = grp.First();
            if (groupSheetIndices.TryGetValue(group, out sheetIndices))
            {
                matchSheetname = regexSheetname.Match(rangedata[parentRange]);
                if (matchSheetname.Success)
                {
                    sheetName = matchSheetname.Groups["Sheetname"].Value;
                }

                if (sheetIndices.TryGetValue(sheetName, out sheetIndex))
                {
                    if (columnData.TryGetValue(sheetIndex, out parentType))
                    {
                        parentRanges = parentType.Item3;
                    }
                    else
                    {
                        parentRanges = new List<ISequenceRange>();
                        parentType = new Tuple<SequenceRangeGrouping, bool, List<ISequenceRange>>(
                                group,
                                inputGroups.IndexOf(group) == 0,
                                parentRanges);
                        columnData.Add(sheetIndex, parentType);
                    }

                    parentRanges.Add(parentRange);
                }
            }
        }

        /// <summary>
        /// Build the header for Merge operations on BED inputs
        /// </summary>
        /// <param name="currentSheet">Current worksheet object</param>
        /// <param name="groupSheetIndices">
        /// Complete indices.
        /// Contains individual column of each sheet of each Group
        /// </param>
        /// <param name="groupsData">
        /// Complete input groups information
        /// Contains individual Group, sheet and addresses of ISequenceRange
        /// </param>
        /// <param name="baseRowIndex">Base row index</param>
        /// <param name="baseColumnIndex"></param>
        /// <returns>Current index of sheet row</returns>
        private int WriteSequenceRangeHeader(
                Worksheet currentSheet,
                Dictionary<SequenceRangeGrouping, Dictionary<string, int>> groupSheetIndices,
                Dictionary<SequenceRangeGrouping, GroupData> groupsData,
                int baseRowIndex,
                int baseColumnIndex,
                ref int sheetColumnIndex,
                bool showMetadata,
                bool showBasePairCount)
        {
            int sheetRowIndex = baseRowIndex + 2;
            Dictionary<string, int> sheetIndices = null;
            sheetColumnIndex = baseColumnIndex;
            Range activeRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);

            activeRange.Cells.Font.Bold = true;
            activeRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            WriteRangeValue(activeRange, Resources.CHROM_ID);
            sheetColumnIndex++;

            activeRange = activeRange.Next;
            activeRange.Cells.Font.Bold = true;
            activeRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            WriteRangeValue(activeRange, Resources.CHROM_START);
            sheetColumnIndex++;

            activeRange = activeRange.Next;
            activeRange.Cells.Font.Bold = true;
            activeRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            WriteRangeValue(activeRange, Resources.CHROM_END);
            sheetColumnIndex++;

            if (showMetadata)
            {
                for (int index = 3; index < rangeHeaders.Count; index++)
                {
                    activeRange = activeRange.Next;
                    activeRange.Cells.Font.Bold = true;
                    activeRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    WriteRangeValue(activeRange, rangeHeaders[index]);
                    sheetColumnIndex++;
                }
            }

            sheetRowIndex -= 2;
            foreach (KeyValuePair<SequenceRangeGrouping, GroupData> groupData in groupsData)
            {
                activeRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
                activeRange = activeRange.get_Resize(1,
                    (groupData.Value.Metadata.Count * (showBasePairCount ? 2 : 1)) + (showBasePairCount ? 2 : 1));
                activeRange.Merge();
                activeRange.Cells.Font.Bold = true;
                activeRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                WriteRangeValue(activeRange, groupData.Value.Name);

                WriteSequeceRangeHeader(
                        currentSheet,
                        activeRange,
                        Resources.ALL_SHEETS,
                        ref sheetRowIndex,
                        ref sheetColumnIndex,
                        showBasePairCount);

                foreach (string sheetname in groupData.Value.Metadata.Keys)
                {
                    if (!groupSheetIndices.TryGetValue(groupData.Key, out sheetIndices))
                    {
                        sheetIndices = new Dictionary<string, int>();
                        groupSheetIndices[groupData.Key] = sheetIndices;
                    }

                    sheetIndices[sheetname] = sheetColumnIndex - baseColumnIndex;
                    WriteSequeceRangeHeader(
                            currentSheet,
                            activeRange,
                            sheetname,
                            ref sheetRowIndex,
                            ref sheetColumnIndex,
                            showBasePairCount);
                }
            }

            if (showBasePairCount)
            {
                activeRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
                activeRange.Cells.Font.Bold = true;
                activeRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                WriteRangeValue(activeRange, Resources.COMMON);

                sheetRowIndex++;
                activeRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
                activeRange.Cells.Font.Bold = true;
                activeRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                WriteRangeValue(activeRange, Resources.BASE_PAIR_COUNT);
                sheetRowIndex += 2;
            }
            else
            {
                sheetRowIndex += 3;
            }

            return sheetRowIndex;
        }

        /// <summary>
        /// Write the column header for a sheet
        /// </summary>
        /// <param name="currentSheet">Current worksheet object</param>
        /// <param name="currentRange">Current range object</param>
        /// <param name="sequenceGroupName">Sequence group Name</param>
        /// <param name="sheetRowIndex">Sheet row index</param>
        /// <param name="sheetColumnIndex">Sheet column index</param>
        private static void WriteSequeceRangeHeader(
                Worksheet currentSheet,
                Range currentRange,
                string sequenceGroupName,
                ref int sheetRowIndex,
                ref int sheetColumnIndex,
                bool showBasePairCount)
        {
            sheetRowIndex++;
            currentRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
            currentRange = currentRange.get_Resize(1, showBasePairCount ? 2 : 1);
            currentRange.Merge();
            currentRange.Cells.Font.Bold = true;
            currentRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            WriteRangeValue(currentRange, sequenceGroupName);
            sheetRowIndex++;

            if (showBasePairCount)
            {
                currentRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
                currentRange.Cells.Font.Bold = true;
                currentRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                WriteRangeValue(currentRange, Resources.BASE_PAIR_COUNT);
                sheetColumnIndex++;
            }

            currentRange = currentSheet.get_Range(GetColumnString(sheetColumnIndex) + sheetRowIndex, Type.Missing);
            currentRange.Cells.Font.Bold = true;
            currentRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            WriteRangeValue(currentRange, Resources.NUMBER_OF_RANGES);
            sheetColumnIndex++;

            sheetRowIndex -= 2;
        }

        /// <summary>
        /// Limits column widts of given range to a specific width
        /// </summary>
        /// <param name="range">Range of which the column size should be limited</param>
        private void NormalizeColumWidths(Range range)
        {
            foreach (Range column in range.Columns)
            {
                if (column.EntireColumn.ColumnWidth > MaximumColumnWidth)
                {
                    column.ColumnWidth = MaximumColumnWidth;
                }
            }
        }

        /// <summary>
        /// Displays About dialog.
        /// </summary>
        /// <param name="sender">About button.</param>
        /// <param name="e">Event data</param>
        private void OnAboutClick(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            this.ResetStatus();
            AboutDialog dialog = new AboutDialog();
            dialog.Activated += new EventHandler(OnWPFWindowActivated);
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(dialog);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            dialog.ShowDialog();
        }

        /// <summary>
        /// Create a Venn diagram out of two/three SequenceRangeGrouping objects.
        /// This method gets the user input for creating the diagram
        /// </summary>
        void OnVennDiagramClick(object sender, RibbonControlEventArgs e)
        {
            InputSelection inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            inputs.MaximumSequenceCount = 3;
            inputs.PromptForSequenceName = false;
            inputs.SequenceLabels = new string[] { Resources.InputSelection_SequenceLabel_Venn1, Resources.InputSelection_SequenceLabel_Venn2, Resources.InputSelection_SequenceLabel_Venn3 };
            inputs.GetInputSequenceRanges(DoDrawVenn, false, false, false);
        }

        /// <summary>
        /// Call venn diagram module to create the diagran and display it using NodeXL
        /// </summary>
        /// <param name="selectedSequences">List of two or three sequenceRangeGrouping objects depending on the user selection</param>
        /// <param name="args">Any custom argument passed</param>
        private void DoDrawVenn(InputSequenceRangeSelectionEventArg e)
        {
            // Call VennToNodeXL which will do the calculations and add the NodeXL workbook to the application object passed.
            if (e.Sequences.Count == 2)
            {
                Tools.VennDiagram.VennToNodeXL.CreateNodeXLVennDiagramWorkbookFromSequenceRangeGroupings(Globals.ThisAddIn.Application, e.Sequences[0], e.Sequences[1]);
            }
            else // if sequence count is not 2, its 3. InputSequenceDialog guarentees it as we set min=2 and max=3.
            {
                Tools.VennDiagram.VennToNodeXL.CreateNodeXLVennDiagramWorkbookFromSequenceRangeGroupings(Globals.ThisAddIn.Application, e.Sequences[0], e.Sequences[1], e.Sequences[2]);
            }
        }

        /// <summary>
        /// Change the wait cursor to normal one once the window is loaded.
        /// </summary>
        private void OnWPFWindowActivated(object sender, EventArgs e)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlDefault;
        }

        /// <summary>
        /// Gets a valid file name according to the excel rules for naming a worksheet.
        /// </summary>
        /// <param name="fileName">The name of the file being imported</param>
        /// <returns>A valid name for worksheets</returns>
        private string GetValidFileNames(string fileName)
        {
            string file = fileName;
            StringBuilder sb = new StringBuilder();

            int index = 0;

            // Removes all characters apart from letters or digits.
            for (; index < file.Length; index++)
            {
                if (char.IsLetterOrDigit(file[index]) || file[index] == '_')
                {
                    sb.Append(file[index]);
                }
            }

            if (sb.Length == 0)
            {
                sb = new StringBuilder(Properties.Resources.SHEET_NAME + this.currentFileNumber.ToString(CultureInfo.CurrentCulture));

                string validFileName = sb.ToString();
                this.fileNames.Add(validFileName);
                return validFileName;
            }

            // Ensures that the file name is not greater than "MaxWorksheetNameLength" characters.
            int maxLength = sb.Length > MaxWorksheetNameLength ? MaxWorksheetNameLength : sb.Length;

            if (this.fileNames.Contains(sb.ToString(0, maxLength)))
            {
                sb = new StringBuilder(sb.ToString(0, maxLength));
                int count = 0;

                // If this file name is already present, start adding indexes to it.
                // For e.g if file name is abcd, then on first import the new name will be abcd1,
                // then on further imports it will become abcd2,abcd3,abcd4.....
                while (this.fileNames.Contains(sb.ToString()))
                {
                    if (count != 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }

                    count++;
                    sb.Append(count);
                }

                // If it has exceeded "MaxWorksheetNameLength" characters then start removing the second
                // character. 
                if (sb.Length > MaxWorksheetNameLength)
                {
                    while (this.fileNames.Contains(sb.ToString()) || sb.Length > MaxWorksheetNameLength)
                    {
                        if (sb.Length > 3)
                        {
                            sb.Remove(2, 1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // If it still doesnt have a unique name, then the new name is Seq_1 or Seq_2.......
                    if (this.fileNames.Contains(sb.ToString()))
                    {
                        sb = new StringBuilder(Properties.Resources.SHEET_NAME + this.currentFileNumber.ToString(CultureInfo.CurrentCulture));
                        this.currentFileNumber++;
                    }
                }

                this.fileNames.Add(sb.ToString());
                return sb.ToString();
            }
            else
            {
                string validFileName = sb.ToString(0, maxLength);
                this.fileNames.Add(validFileName);
                return validFileName;
            }
        }

        /// <summary>
        /// This method populates the BioRibbon tab with UI controls.
        /// </summary>
        private void BuildRibbonTabs()
        {
            this.AddWebServiceDropDowns();
            this.AddParsersDropDowns();
            this.AddFormattersDropDowns();
            this.AddAlignersDropDown();
        }

        /// <summary>
        /// This method retrives all the supported aligners in the framework and
        /// populates the Align drop down.
        /// </summary>
        private void AddAlignersDropDown()
        {
            foreach (ISequenceAligner aligner in SequenceAligners.All)
            {
                RibbonButton button = Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromExcel;
                button.Tag = aligner;
                button.Click += new RibbonControlEventHandler(this.OnAlignmentButtonClicked);
                button.Label = string.Format(aligner.Name);
                button.Description = string.Format(aligner.Description);
                if (aligner is IPairwiseSequenceAligner)
                {
                    button.ScreenTip = string.Format(Resources.SCREEN_TIP_PAIRWISEALIGNERS, button.Label);
                }
                else
                {
                    button.ScreenTip = string.Format(Resources.SCREEN_TIP_ALIGNERS, button.Label);
                }
                this.splitAligners.Items.Add(button);
            }
        }

        /// <summary>
        /// This method retrives all the supported formatters in the framework and
        /// populates the formatters drop down.
        /// </summary>
        private void AddFormattersDropDowns()
        {
            foreach (ISequenceFormatter formatter in SequenceFormatters.All)
            {
                RibbonButton button = Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromText;
                button.Label = string.Format(formatter.Name.ToUpper(CultureInfo.CurrentCulture));
                button.ShowImage = true;
                button.ScreenTip = string.Format(Resources.SCREEN_TIP_EXPORT_MENU, button.Label);
                button.Tag = formatter;
                button.Click += new RibbonControlEventHandler(OnExportClick);
                this.splitExport.Items.Add(button);
            }

            foreach (ISequenceRangeFormatter formatter in SequenceRangeFormatters.All)
            {
                RibbonButton button = Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromText;
                button.Label = string.Format(formatter.Name.ToUpper(CultureInfo.CurrentCulture));
                button.ShowImage = true;
                button.ScreenTip = string.Format(Resources.SCREEN_TIP_EXPORT_MENU, button.Label);
                button.Tag = formatter;
                button.Click += new RibbonControlEventHandler(OnExportClick);
                this.splitExport.Items.Add(button);
            }
        }

        /// <summary>
        /// This method retrives all the supported parsers in the framework and
        /// populates the parsers drop down.
        /// </summary>
        private void AddParsersDropDowns()
        {
            foreach (ISequenceParser parser in SequenceParsers.All)
            {
                RibbonButton button = Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromText;
                button.Click += new RibbonControlEventHandler(this.ReadSequenceFiles);
                button.Label = string.Format(parser.Name.ToUpper(CultureInfo.CurrentCulture));
                button.ShowImage = true;
                button.Tag = parser;
                button.ScreenTip = string.Format(Resources.SCREEN_TIP_IMPORT_MENU, button.Label);
                this.splitImport.Items.Add(button);
            }

            foreach (ISequenceRangeParser parser in SequenceRangeParsers.All)
            {
                RibbonButton button = Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromText;
                button.Click += new RibbonControlEventHandler(this.ReadSequenceFiles);
                button.Label = string.Format(parser.Name.ToUpper(CultureInfo.CurrentCulture));
                button.ShowImage = true;
                button.Tag = parser;
                button.ScreenTip = string.Format(Resources.SCREEN_TIP_IMPORT_MENU, button.Label);
                this.splitImport.Items.Add(button);
            }
        }

        /// <summary>
        /// This method retrieves all the supported Web-Service in the framework and
        /// populates the Web-Services drop down.
        /// </summary>
        private void AddWebServiceDropDowns()
        {
            foreach (IBlastServiceHandler blastService in
                WebServices.All.Where(service => service is IBlastServiceHandler))
            {
                RibbonButton button = Factory.CreateRibbonButton();
                button.ControlSize = RibbonControlSize.RibbonControlSizeLarge;
                button.Image = Resources.FromWeb;
                button.Click += new RibbonControlEventHandler(this.OnExecuteBlastSearch);
                button.Label = string.Format(Properties.Resources.SERVICE_LABEL, blastService.Name);
                button.Tag = blastService.Name;
                button.Description = string.Format(Properties.Resources.SERVICE_DESC, blastService.Name);
                button.ShowImage = true;
                button.ScreenTip = string.Format(Resources.SCREEN_TIP_BLAST_SEARCH, blastService.Name);
                this.splitWebService.Items.Add(button);
            }
        }

        /// <summary>
        /// This method runs the DisplayChart macro which draws a chart of alphabet frequencies.
        /// </summary>
        /// <param name="sender">btnRunChartMacro instance.</param>
        /// <param name="e">Event data.</param>
        private void RunMacro(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            vbext_ProcKind procType;
            string macroName = string.Empty;

            // Get the list of all Macros defined in this workbook.
            try
            {
                foreach (VBComponent vbc in Globals.ThisAddIn.Application.ActiveWorkbook.VBProject.VBComponents)
                {
                    CodeModule codeModule = vbc.CodeModule;
                    int currentLine = codeModule.CountOfDeclarationLines + 1;
                    while (currentLine < codeModule.CountOfLines)
                    {
                        string procName = codeModule.get_ProcOfLine(currentLine, out procType);

                        // Check if the name matches the Display Chart macro name.
                        if (procName.Contains(DisplayChartMacroName))
                        {
                            // If it does use this particular macro.
                            macroName = procName;
                            break;
                        }

                        currentLine += codeModule.get_ProcCountLines(procName, procType);
                    }
                }
            }
            catch
            {
                macroName = string.Empty;
            }

            try
            {
                // If macro not found, then raise an error.
                if (!string.IsNullOrEmpty(macroName))
                {
                    // If found, run the macro.
                    Globals.ThisAddIn.Application.Run(macroName, Missing.Value, Missing.Value, Missing.Value,
                                     Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                     Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                     Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                     Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                     Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                     Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                     Missing.Value, Missing.Value, Missing.Value);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.MACRO_MISSING, Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method is called when the user wishes to configure the max number of sequence
        /// characters per line.
        /// </summary>
        /// <param name="sender">btnMaxColumn instance.</param>
        /// <param name="e">Event data.</param>
        private void OnSizeChangedClicked(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            this.ResetStatus();
            MaxColumnsDialog dialog = new MaxColumnsDialog(maxNumberOfCharacters, this.alignAllSequenceSheet);
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(dialog);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            dialog.Activated += new EventHandler(OnWPFWindowActivated);
            if (dialog.Show())
            {
                maxNumberOfCharacters = dialog.MaxNumber;
            }
        }

        /// <summary>
        /// This method is called when the user wants to cancel a
        /// ongoing web service search.
        /// </summary>
        /// <param name="sender">Cancel search button</param>
        /// <param name="e">Event data</param>
        private void OnCancelSearchClicked(object sender, RibbonControlEventArgs e)
        {
            IBlastServiceHandler blastServiceHandler =
                    this.GetWebServiceInstance(this.webserviceName);
            try
            {
                blastServiceHandler.CancelRequest(this.requestIdentifier);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    String.Format(Properties.Resources.BLAST_CANCEL_FAILED, ex.Message),
                    Properties.Resources.CAPTION,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            this.btnCancelSearch.Enabled = this.cancelSearchButtonState = false;
            this.ChangeStatusBar(string.Format(Properties.Resources.WEBSERVICE_STATUS_BAR, Resources.CANCELLED));
        }

        /// <summary>
        /// This method is called when the user wants to cancel a
        /// ongoing sequence alignment.
        /// </summary>
        /// <param name="sender">Cancel align button</param>
        /// <param name="e">Event data</param>
        private void OnCancelAlignClicked(object sender, RibbonControlEventArgs e)
        {
            if (this.alignerThread != null && this.alignerThread.IsBusy)
            {
                this.alignerThread.CancelAsync();
            }

            this.btnCancelAlign.Enabled = this.cancelAlignButtonState = false;
            this.ChangeStatusBar(string.Format(Properties.Resources.ALIGNMENT_STATUS_BAR, Resources.CANCELLED));
        }

        /// <summary>
        /// This method is called when the user wants to cancel a
        /// ongoing sequence assembly
        /// </summary>
        /// <param name="sender">Cancel align button</param>
        /// <param name="e">Event data</param>
        private void OnCancelAssembleClicked(object sender, RibbonControlEventArgs e)
        {
            if (this.assemblerThread != null && this.assemblerThread.IsBusy)
            {
                this.assemblerThread.CancelAsync();
            }

            this.btnCancelAssemble.Enabled = this.cancelAssemblyButtonState = false;
            this.ChangeStatusBar(string.Format(Properties.Resources.ASSEMBLER_STATUS_BAR, Resources.CANCELLED));
        }

        /// <summary>
        /// This method is called when the user wants to start a alignment operation.
        /// This method extracts the sequences present in the selected excel sheets 
        /// and runs alignment on them.
        /// </summary>
        /// <param name="sender">Align button</param>
        /// <param name="e">Event data</param>
        private void OnAlignmentButtonClicked(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            ISequenceAligner aligner = (sender as RibbonButton).Tag as ISequenceAligner;

            InputSelection inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            if (aligner is IPairwiseSequenceAligner)
            {
                inputs.MaximumSequenceCount = 2;
            }

            inputs.GetInputSequences(DoAlignment, false, aligner);
        }

        /// <summary>
        /// Start an alignment on selected sequences
        /// </summary>
        /// <param name="selectedSequences">List of sequences selected</param>
        /// <param name="args">Any other arguments</param>
        private void DoAlignment(List<ISequence> selectedSequences, params object[] args)
        {
            this.btnCancelAlign.Enabled = true;
            this.cancelAlignButtonState = true;

            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;

            AssemblyInputDialog alignerDialog = new AssemblyInputDialog(true, selectedSequences[0].Alphabet, args[0] as ISequenceAligner);
            System.Windows.Interop.WindowInteropHelper assemblyInputHelper = new System.Windows.Interop.WindowInteropHelper(alignerDialog);
            assemblyInputHelper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            alignerDialog.Activated += new EventHandler(OnWPFWindowActivated);
            if (alignerDialog.Show())
            {
                AlignerInputEventArgs alignerInput = alignerDialog.GetAlignmentInput();
                if (alignerInput != null) // If fetching parameters were successful
                {
                    alignerInput.Sequences = selectedSequences;
                    alignerInput.Aligner = alignerDialog.Aligner;

                    this.ChangeStatusBar(string.Format(Properties.Resources.ALIGNMENT_STATUS_BAR, Resources.ALIGNING));

                    this.alignerThread = new BackgroundWorker();
                    this.alignerThread.WorkerSupportsCancellation = true;
                    this.alignerThread.DoWork += new DoWorkEventHandler(this.OnRunAlignerAlgorithm);
                    this.alignerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.OnAlignerCompleted);
                    this.alignerThread.RunWorkerAsync(alignerInput);
                }
            }
        }

        private void OnRunAlignerAlgorithm(object sender, DoWorkEventArgs e)
        {
            AlignerInputEventArgs alignerInput = e.Argument as AlignerInputEventArgs;

            AssignAlignerParameter(alignerInput.Aligner, alignerInput);
            alignerInput.Aligner.GapOpenCost = alignerInput.GapCost;
            alignerInput.Aligner.GapExtensionCost = alignerInput.GapExtensionCost;

            if (alignerInput.SimilarityMatrix != null)
            {
                alignerInput.Aligner.SimilarityMatrix = alignerInput.SimilarityMatrix;
            }

            try
            {
                IList<ISequenceAlignment> alignedResult = alignerInput.Aligner.Align(alignerInput.Sequences);
                if (alignerThread.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                if (alignedResult.Count > 0)
                {
                    e.Result = alignedResult;
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show(Properties.Resources.NO_RESULT, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.ResetStatus();
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ChangeStatusBar(string.Format(Properties.Resources.ALIGNMENT_STATUS_BAR, Resources.ERROR));
            }
            finally
            {
                this.btnCancelAlign.Enabled = false;
                this.cancelAlignButtonState = false;
            }
        }

        /// <summary>
        /// Display the alignment result on sheet
        /// </summary>
        /// <param name="e">Results of alignment</param>
        private void OnAlignerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            try
            {
                BuildAlignmentResultView(e.Result as IList<ISequenceAlignment>);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.ChangeStatusBar(string.Format(Properties.Resources.ALIGNMENT_STATUS_BAR, Resources.DONE));
            }
        }

        /// <summary>
        /// This method displays the output of a assembly process.
        /// </summary>
        /// <param name="assemblerResult">Result of the assembly process.</param>
        private void BuildAlignmentResultView(IList<ISequenceAlignment> alignedResult)
        {
            if (alignedResult.Count == 0 || alignedResult[0].AlignedSequences.Count == 0)
            {
                MessageBox.Show(Resources.NO_RESULT, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Workbook activeWorkBook = (Workbook)Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet activesheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;
            Worksheet currentsheet = (Worksheet)activeWorkBook.Worksheets.Add(Type.Missing, activesheet, Type.Missing, Type.Missing);

            ((Microsoft.Office.Interop.Excel._Worksheet)currentsheet).Activate();
            currentsheet.Name = this.GetValidFileNames(
                    Resources.Alignment_AlignedSequencesHeading + this.currentAlignResultSheetNumber.ToString(CultureInfo.CurrentCulture));
            currentsheet.Cells.Font.Name = "Courier New";
            this.currentAlignResultSheetNumber++;

            foreach (ISequenceAlignment sequenceAlignment in alignedResult)
            {
                int startingColumn = 3;
                int rowNumber = 1;

                foreach (IAlignedSequence alignedSequence in sequenceAlignment.AlignedSequences)
                {
                    string[,] rangeData;
                    int alignResultNumber = 1, sequenceNumber = 1;
                    int columnIndex = 0;
                    Range currentRange;

                    Range header = currentsheet.get_Range("B" + rowNumber.ToString(), Type.Missing);
                    WriteRangeValue(header, Resources.Alignment_StartOffsetString);

                    rowNumber++;

                    #region get max length of all sequences selected
                    long numberofCharacters = 1;
                    long maxSequenceLength = 0;
                    foreach (ISequence currSeq in alignedSequence.Sequences)
                    {
                        if (maxSequenceLength < currSeq.Count)
                        {
                            maxSequenceLength = currSeq.Count;
                        }
                    }

                    // calculate number of alphabets in one cell
                    if (maxSequenceLength > MaxExcelColumns)
                    {
                        if (maxSequenceLength % MaxExcelColumns == 0)
                        {
                            numberofCharacters = maxSequenceLength / MaxExcelColumns;
                        }
                        else
                        {
                            numberofCharacters = maxSequenceLength / MaxExcelColumns;
                            numberofCharacters++;
                        }
                    }
                    #endregion

                    // write to sheet
                    int currentSequenceIndex = 0;
                    List<int> startOffsets = null;
                    List<int> endOffsets = null;

                    if (alignedSequence.Metadata.ContainsKey(StartOffsetString))
                    {
                        startOffsets = alignedSequence.Metadata[StartOffsetString] as List<int>;
                    }

                    if (startOffsets == null)
                    {
                        startOffsets = new List<int>();
                    }

                    if (alignedSequence.Metadata.ContainsKey(EndOffsetString))
                    {
                        endOffsets = alignedSequence.Metadata[EndOffsetString] as List<int>;
                    }

                    if (endOffsets == null)
                    {
                        endOffsets = new List<int>();
                    }


                    foreach (ISequence currSeq in alignedSequence.Sequences)
                    {
                        // write header
                        header = currentsheet.get_Range("A" + rowNumber.ToString(), Type.Missing);
                        if (string.IsNullOrWhiteSpace(currSeq.ID))
                        {
                            WriteRangeValue(header, Resources.Alignment_AlignedSequencesHeading + "_" + (currentSequenceIndex + 1));
                        }
                        else
                        {
                            WriteRangeValue(header, currSeq.ID);
                        }

                        int startOffset = -1;

                        if (startOffsets.Count > currentSequenceIndex)
                        {
                            startOffset = startOffsets[currentSequenceIndex] + 1;
                            header = currentsheet.get_Range("B" + rowNumber.ToString(), Type.Missing);
                            WriteRangeValue(header, startOffset.ToString());
                        }

                        rangeData = new string[1, maxSequenceLength > MaxExcelColumns
                                                    ? MaxExcelColumns
                                                    : maxSequenceLength];

                        columnIndex = 0;

                        for (long i = 0; i < currSeq.Count; i += numberofCharacters, columnIndex++)
                        {
                            var tempSeq = currSeq.GetSubSequence(i, numberofCharacters);
                            //tempSeqToLoop.RangeStart = i;
                            //tempSeqToLoop.RangeLength = numberofCharacters;
                            rangeData[0, columnIndex] = tempSeq.ConvertToString();
                        }

                        // dump to sheet
                        currentRange = currentsheet.get_Range(GetColumnString(startingColumn) + rowNumber.ToString(), Type.Missing);
                        if (columnIndex > 1)
                        {
                            currentRange = currentRange.get_Resize(1, columnIndex);
                            currentRange.set_Value(Missing.Value, rangeData);

                            this.FillBackGroundColor(currentRange);

                            int endOffset = -1;

                            if (endOffsets.Count > currentSequenceIndex)
                            {
                                endOffset = endOffsets[currentSequenceIndex] + 1;
                                header = currentsheet.get_Range(GetColumnString(startingColumn + columnIndex) + rowNumber.ToString(), Type.Missing);
                                WriteRangeValue(header, endOffset.ToString());
                            }

                            string rangeName = "AlignedSeq_" + alignResultNumber.ToString() + "_" + currSeq.ID + "_" + sequenceNumber;
                            CreateNamedRange(currentsheet, currentRange, rangeName, startingColumn, rowNumber, columnIndex, 1);
                        }

                        rowNumber++;
                        sequenceNumber++;

                        currentSequenceIndex++;
                    }
                    header = currentsheet.get_Range(GetColumnString(startingColumn + columnIndex) + (rowNumber - currentSequenceIndex - 1).ToString(), Type.Missing);
                    WriteRangeValue(header, Resources.Alignment_EndOffsetString);

                    rowNumber += 3;
                    alignResultNumber++;
                }
            }
            currentsheet.Columns.AutoFit();
        }

        /// <summary>
        /// Create a named range with the specified parameters
        /// </summary>
        /// <param name="currentsheet">Sheet in which name range has to be created</param>
        /// <param name="currentRange">Range to be named</param>
        /// <param name="rangeName">Name to be given</param>
        /// <param name="startingCol">Starting column</param>
        /// <param name="startingRow">Starting row</param>
        /// <param name="cols">Total number of colums</param>
        /// <param name="rows">Total number of rows</param>
        private void CreateNamedRange(Worksheet currentsheet, Range currentRange, string rangeName, int startingCol, int startingRow, int cols, int rows)
        {
            StringBuilder formulaBuilder = new StringBuilder();
            formulaBuilder.Append("=");
            formulaBuilder.Append(currentsheet.Name);
            formulaBuilder.Append("!$");
            formulaBuilder.Append(GetColumnString(startingCol) + "$" + startingRow);
            formulaBuilder.Append(":$");
            formulaBuilder.Append(GetColumnString(startingCol + (cols - 1)) + "$" + (startingRow + (rows - 1)).ToString());
            string sequenceFormula = formulaBuilder.ToString();

            currentsheet.Names.Add(GetValidFileNames(rangeName), sequenceFormula, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }

        /// <summary>
        /// This method is called when the user wants to start an assembler operation.
        /// </summary>
        private void OnAssembleClick(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();

            InputSelection inputs = new InputSelection();
            inputs.MinimumSequenceCount = 2;
            inputs.GetInputSequences(DoAssembly, false);
        }

        /// <summary>
        /// Callback method from input selection model which will actually do the selected operation
        /// </summary>
        /// <param name="selectedSequences">List of sequences depending on the user selections made</param>
        /// <param name="args">Any arguments passed when calling the selection model</param>
        private void DoAssembly(List<ISequence> selectedSequences, params object[] args)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            AssemblyInputDialog assemblerDialog = new AssemblyInputDialog(false, selectedSequences[0].Alphabet);
            System.Windows.Interop.WindowInteropHelper assemblyInputHelper = new System.Windows.Interop.WindowInteropHelper(assemblerDialog);
            assemblyInputHelper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            assemblerDialog.Activated += new EventHandler(OnWPFWindowActivated);
            if (assemblerDialog.Show())
            {
                AssemblyInputEventArgs eventArgs = new AssemblyInputEventArgs(selectedSequences, assemblerDialog.Aligner);
                eventArgs.ConsensusThreshold = assemblerDialog.ConsensusThreshold;
                eventArgs.MatchScore = assemblerDialog.MatchScore;
                eventArgs.MergeThreshold = assemblerDialog.MergeThreshold;
                eventArgs.MismatchScore = assemblerDialog.MisMatchScore;

                eventArgs.AlignerInput = assemblerDialog.GetAlignmentInput();

                if (eventArgs.AlignerInput != null) // If fetching parameters were successful
                {
                    this.OnRunAssemblerAlgorithm(eventArgs);
                }
            }
        }

        /// <summary>
        /// This method is called when the user wants to use NCBI\EBI databases
        /// for a search operation. This methos pops-up a UI where the user
        /// can configure the parameters required to run NCBI\EBI.
        /// </summary>
        /// <param name="sender">ExecuteBlast button.</param>
        /// <param name="e">Event data</param>
        private void OnExecuteBlastSearch(object sender, RibbonControlEventArgs e)
        {
            this.ResetStatus();
            this.webserviceName = (sender as RibbonButton).Tag as string;

            InputSelection inputs = new InputSelection();
            inputs.SequenceLabels = new string[] { Resources.InputSelection_SequenceLabel_Blast };
            inputs.MinimumSequenceCount = 1;
            if (WebServices.BioHPCBlast != null && !WebServices.BioHPCBlast.Name.Equals(this.webserviceName))
            {
                inputs.MaximumSequenceCount = 1;
            }
            inputs.GetInputSequences(OnExecuteSearch, false);
        }

        /// <summary>
        /// This method is called when the user wants to import a Range-Sequence file
        /// into excel. The user chooses a particular Sequence file which will
        /// be parsed by parsers available in our framework and is then imported
        /// into a excel file.
        /// </summary>
        /// <param name="parser">SequenceRangeParser instance.</param>
        /// <param name="fileName">Name of the file</param>
        private void ReadRangeSequence(ISequenceRangeParser parser, string fileName)
        {
            SequenceRangeGrouping rangeGroup = parser.ParseRangeGrouping(fileName);
            List<ISequenceRange> sequences = rangeGroup.Flatten();
            if (sequences == null || sequences.Count == 0)
            {
                string strMessage = string.Format(Properties.Resources.PARSE_ERROR, Path.GetFileName(fileName), string.Empty);
                strMessage = strMessage.TrimEnd(" :".ToCharArray());
                MessageBox.Show(strMessage, Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet workSheet = workBook.Worksheets.Add(Type.Missing, workBook.Worksheets.get_Item(workBook.Worksheets.Count), Type.Missing, Type.Missing) as Worksheet;
            ((Microsoft.Office.Interop.Excel._Worksheet)workSheet).Activate();

            string validName = this.GetValidFileNames(Path.GetFileNameWithoutExtension(fileName));
            workSheet.Name = validName;

            int rowNumber = 2;
            int initialRowNumber = rowNumber;
            int columnNumber = 2;
            int maxColumnNumber = 13;

            foreach (string header in rangeHeaders)
            {
                Range range = workSheet.get_Range(GetColumnString(columnNumber) + rowNumber, Type.Missing);
                WriteRangeValue(range, header);
                columnNumber++;
            }

            rowNumber++;
            columnNumber = 2;

            Range activeSelectedRange = workSheet.get_Range(GetColumnString(columnNumber) + rowNumber, Missing.Value);
            activeSelectedRange = activeSelectedRange.get_Resize(sequences.Count, 12);
            object[,] values = new object[sequences.Count, 12];

            for (int i = 0; i < sequences.Count; i++)
            {
                ISequenceRange range = sequences[i];
                values[i, 0] = range.ID;
                values[i, 1] = range.Start;
                values[i, 2] = range.End;
                values[i, 3] = ExtractRangeMetadata(range, Properties.Resources.BED_NAME);
                values[i, 4] = ExtractRangeMetadata(range, Properties.Resources.BED_SCORE);

                object value = ExtractRangeMetadata(range, Properties.Resources.BED_STRAND);
                if (value != null)
                {
                    values[i, 5] = value.ToString();
                }

                values[i, 6] = ExtractRangeMetadata(range, Properties.Resources.BED_THICK_START);
                values[i, 7] = ExtractRangeMetadata(range, Properties.Resources.BED_THICK_END);
                values[i, 8] = ExtractRangeMetadata(range, Properties.Resources.BED_ITEM_RGB);
                values[i, 9] = ExtractRangeMetadata(range, Properties.Resources.BED_BLOCK_COUNT);
                string strValue = ExtractRangeMetadata(range, Properties.Resources.BED_BLOCK_SIZES) as string;

                // As excel is not handling more than 4000 chars in a single cell.
                if (strValue != null && strValue.Length > 4000)
                {
                    strValue = strValue.Substring(0, 4000);
                }

                values[i, 10] = strValue;

                strValue = ExtractRangeMetadata(range, Properties.Resources.BED_BLOCK_STARTS) as string;

                // As excel is not handling more than 4000 chars in a single cell.
                if (strValue != null && strValue.Length > 4000)
                {
                    strValue = strValue.Substring(0, 4000);
                }

                values[i, 11] = strValue;

                rowNumber++;
            }

            activeSelectedRange.set_Value(Missing.Value, values);

            workSheet.Columns.AutoFit();
            NormalizeColumWidths(workSheet.UsedRange);
            this.EnableAllControls();

            StringBuilder sb = new StringBuilder();
            sb.Append("='");
            sb.Append(workSheet.Name);
            sb.Append("'!");
            sb.Append("$");
            sb.Append(GetColumnString(columnNumber) + "$" + initialRowNumber);
            sb.Append(":$");
            sb.Append(GetColumnString(maxColumnNumber) + "$" + (rowNumber - 1).ToString(CultureInfo.CurrentCulture));
            string formula = sb.ToString();

            Name dataWithHeader = workSheet.Names.Add(Properties.Resources.SEQUENCERANGEDATA_PRESELECTION + workSheet.Name, formula, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            dataWithHeader.RefersToRange.Select();
            //added default from UI as auto detect and ignore space
            SequenceCache.Add(activeSelectedRange, rangeGroup, "_Auto Detect_NB");
        }

        /// <summary>
        /// This method is called when the user wants to import a Sequence file
        /// or a Query region file into excel.
        /// </summary>
        /// <param name="sender">Import button.</param>
        /// <param name="e">Event data</param>
        private void ReadSequenceFiles(object sender, RibbonControlEventArgs e)
        {
            IParser parser = (sender as RibbonButton).Tag as IParser;
            if (parser == null)
                return;

            this.ResetStatus();
            var openFileDialog1 = new System.Windows.Forms.OpenFileDialog
            {
                Multiselect = true,
                Filter = string.Format("{0} ({1})|{1}|All Files (*.*)|*.*", parser.Name,
                                        parser.SupportedFileTypes.Replace(".", "*.").Replace(',', ';'))
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int currentRow = 1;
                foreach (string fileName in openFileDialog1.FileNames)
                {
                    try
                    {
                        this.ScreenUpdate(false);

                        ISequenceParser sequenceParser = parser as ISequenceParser;
                        if (sequenceParser != null)
                        {
                            try
                            {
                                sequenceParser.Open(fileName);
                                this.ReadSequences(sequenceParser, fileName, ref currentRow);
                            }
                            finally
                            {
                                sequenceParser.Close();
                            }
                        }
                        else
                        {
                            ISequenceRangeParser rangeParser = parser as ISequenceRangeParser;
                            this.ReadRangeSequence(rangeParser, fileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format(Properties.Resources.PARSE_ERROR, Path.GetFileName(fileName), ex.Message), Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.ScreenUpdate(true);
                    }
                }
            }
        }

        /// <summary>
        /// This method is called when the user wants to import a Sequence file
        /// into excel. The user chooses a particular Sequence file which will
        /// be parsed by parsers available in our framework and is then imported
        /// into a excel file.
        /// </summary>
        /// <param name="parser">SequenceParser instance.</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="currentRow">Current row of insertion</param>
        private void ReadSequences(ISequenceParser parser, string fileName, ref int currentRow)
        {
            ScreenUpdate(false);

            if (sequencesPerWorksheet == 1)
            {
                ImportSequencesOnePerSheet(parser, fileName);
            }
            else if (sequencesPerWorksheet <= 0)
            {
                ImportSequencesAllInOneSheet(parser, fileName, ref currentRow);
            }
            else if (sequencesPerWorksheet > 0)
            {
                ImportSequencesAcrossSheets(parser, fileName, ref currentRow);
            }

            ScreenUpdate(true);
        }

        /// <summary>
        /// This method breaks the sequences across multiple worksheets.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="fileName"></param>
        /// <param name="currentRow"></param>
        private void ImportSequencesAcrossSheets(ISequenceParser parser, string fileName, ref int currentRow)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            int sequenceCount = 0;
            Worksheet worksheet = null;

            Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;
            Globals.ThisAddIn.Application.EnableEvents = false;

            try
            {
                foreach (ISequence sequence in parser.Parse())
                {
                    if (worksheet == null || sequenceCount++ >= sequencesPerWorksheet)
                    {
                        if(worksheet != null)
                            worksheet.Cells[1, 1].EntireColumn.AutoFit(); // Autofit first column

                        currentRow = 1;
                        sequenceCount = 1;
                        worksheet = workBook.Worksheets.Add(Type.Missing, workBook.Worksheets.Item[workBook.Worksheets.Count], Type.Missing, Type.Missing) as Worksheet;
                        if (worksheet == null)
                            return;

                        // Get a name for the worksheet.
                        string validName = GetValidFileNames(string.IsNullOrEmpty(sequence.ID)
                                ? Path.GetFileNameWithoutExtension(fileName)
                                : sequence.ID);
                        worksheet.Name = validName;
                        ((_Worksheet)worksheet).Activate();
                    }


                    // If sequence ID cannot be used as a sheet name, update the sequence DisplayID with the string used as sheet name.
                    if (string.IsNullOrEmpty(sequence.ID))
                        sequence.ID = Path.GetFileNameWithoutExtension(fileName) + "_" + sequenceCount;

                    WriteOneSequenceToWorksheet(parser, ref currentRow, sequence, worksheet);
                }
                
                if (worksheet != null)
                    worksheet.Cells[1, 1].EntireColumn.AutoFit(); // Autofit first column
            }
            finally
            {
                this.EnableAllControls();
                Globals.ThisAddIn.Application.EnableEvents = true;
            }
        }

        /// <summary>
        /// This method imports a set of sequences, one per worksheet.
        /// </summary>
        /// <param name="parser">SequenceParser instance.</param>
        /// <param name="fileName">Name of the file</param>
        private void ImportSequencesOnePerSheet(ISequenceParser parser, string fileName)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            foreach (ISequence sequence in parser.Parse())
            {
                Worksheet worksheet = workBook.Worksheets.Add(Type.Missing, workBook.Worksheets.Item[workBook.Worksheets.Count], Type.Missing, Type.Missing) as Worksheet;
                if (worksheet == null)
                    return;

                string validName = GetValidFileNames(
                    string.IsNullOrEmpty(sequence.ID) 
                        ? Path.GetFileNameWithoutExtension(fileName) 
                        : sequence.ID);

                // If sequence ID cannot be used as a sheet name, update the sequence DisplayID with the string used as sheet name.
                if (string.IsNullOrEmpty(sequence.ID))
                    sequence.ID = validName;

                worksheet.Name = validName;
                ((_Worksheet)worksheet).Activate();
                Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;
                Globals.ThisAddIn.Application.EnableEvents = false;

                try
                {
                    int currentRow = 1;
                    WriteOneSequenceToWorksheet(parser, ref currentRow, sequence, worksheet);
                    currentFileNumber++;
                }
                finally
                {
                    worksheet.Cells[1, 1].EntireColumn.AutoFit(); // Autofit first column
                    this.EnableAllControls();
                    Globals.ThisAddIn.Application.EnableEvents = true;
                }
            }
        }

        /// <summary>
        /// This method imports a set of sequences, one sequence per row.
        /// </summary>
        /// <param name="parser">SequenceParser instance.</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="currentRow">Next row of insertion</param>
        private void ImportSequencesAllInOneSheet(ISequenceParser parser, string fileName, ref int currentRow)
        {
            Workbook workBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = workBook.Worksheets.Add(Type.Missing, workBook.Worksheets.Item[workBook.Worksheets.Count], Type.Missing, Type.Missing) as Worksheet;
            worksheet.Name = this.GetValidFileNames(Path.GetFileNameWithoutExtension(fileName));
            ((_Worksheet)worksheet).Activate();
            Globals.ThisAddIn.Application.ActiveWindow.Zoom = ZoomLevel;
            Globals.ThisAddIn.Application.EnableEvents = false;

            try
            {
                int sequenceCount = 0;
                foreach (ISequence sequence in parser.Parse())
                {
                    sequenceCount++;
                    if (string.IsNullOrEmpty(sequence.ID))
                        sequence.ID = Path.GetFileNameWithoutExtension(fileName) + "_" + sequenceCount;
                    WriteOneSequenceToWorksheet(parser, ref currentRow, sequence, worksheet);
                    currentFileNumber++;
                }
            }
            finally
            {
                worksheet.Cells[1, 1].EntireColumn.AutoFit(); // Autofit first column
                this.EnableAllControls();
                Globals.ThisAddIn.Application.EnableEvents = true;
            }
        }

        /// <summary>
        /// This writes out a single sequence with header, data, quality scores and metadata into the worksheet.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="currentRow"></param>
        /// <param name="sequence"></param>
        /// <param name="worksheet"></param>
        private void WriteOneSequenceToWorksheet(ISequenceParser parser, ref int currentRow, ISequence sequence, Worksheet worksheet)
        {
            // Write the header (sequence id)
            if (!string.IsNullOrEmpty(sequence.ID))
            {
                string[,] formattedMetadata = ExcelImportFormatter.SequenceIDHeaderToRange(sequence);
                Range range = WriteToSheet(worksheet, formattedMetadata, currentRow, 1);
                if (range != null)
                {
                    range.WrapText = false;
                    currentRow += range.Rows.Count;
                }
            }

            // Write out the data
            Range dataRange;
            if (sequence.Count > 0)
            {
                currentRow = WriteSequence(worksheet, sequence, currentRow, out dataRange);
                if (dataRange != null)
                    dataRange.Columns.AutoFit(); // Autofit columns with sequence data
            }

            // Write quality values if file is FastQ
            if (sequence is QualitativeSequence)
            {
                currentRow++;
                worksheet.Cells[currentRow, 1].Value2 = Resources.Sequence_QualityScores;
                currentRow = WriteQualityValues(sequence as QualitativeSequence, worksheet, currentRow, 2, out dataRange);
                if (dataRange != null)
                    dataRange.Columns.AutoFit(); // Autofit columns with quality scores
            }

            // Write out the metadata 
            Range metadataRange;
            WriteMetadata(sequence, parser, worksheet, currentRow, out metadataRange);
            if (metadataRange != null)
            {
                metadataRange.WrapText = false;
                currentRow += metadataRange.Rows.Count;
            }

            currentRow++; // Add space row between sequences
        }

        /// <summary>
        /// Formats the metadata depending on the sequence type and displays it.
        /// </summary>
        /// <param name="sequence">Sequence which is holding the metadata</param>
        /// <param name="parserUsed">Parser used to read data</param>
        /// <param name="worksheet">Sheet on to which the metadata should be written.</param>
        /// <param name="startingRow">Row we are on</param>
        /// <param name="metadataRange">Will have the range to which the metadata was written</param>
        private void WriteMetadata(ISequence sequence, ISequenceParser parserUsed, Worksheet worksheet, int startingRow, out Range metadataRange)
        {
            if (parserUsed is GenBankParser)
            {
                if (sequence.Metadata.ContainsKey(GenbankMetadataKey))
                {
                    string[,] formattedMetadata = ExcelImportFormatter.GenBankMetadataToRange(sequence.Metadata[GenbankMetadataKey] as GenBankMetadata);
                    metadataRange = WriteToSheet(worksheet, formattedMetadata, startingRow, 1);
                }
                else
                {
                    metadataRange = null;
                }
            }
            else if (parserUsed is GffParser)
            {
                string[,] formattedMetadata = ExcelImportFormatter.GffMetaDataToRange(sequence);
                metadataRange = WriteToSheet(worksheet, formattedMetadata, startingRow, 1);
            }
            else
            {
                metadataRange = null;
                // No error on this.
                //MessageBox.Show(Resources.MetadataFormatError, Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Write quality scores to the sheet if its a FastQ file
        /// </summary>
        /// <param name="sequence">Sequence of which the quality scores should be written</param>
        /// <param name="worksheet">Worksheet to which to write</param>
        /// <param name="startingRow">Starting row</param>
        /// <param name="startingColumn">Column we wrote to</param>
        /// <param name="dataRange">Range where quality values were written</param>
        /// <returns>Index of row after last written row</returns>
        private int WriteQualityValues(QualitativeSequence sequence, Worksheet worksheet, int startingRow, int startingColumn, out Range dataRange)
        {
            string[,] qualityScores = ExcelImportFormatter.FastQQualityValuesToRange(sequence, maxNumberOfCharacters);
            dataRange = WriteToSheet(worksheet, qualityScores, startingRow, startingColumn);

            return startingRow + qualityScores.GetLength(0) + 1;
        }

        /// <summary>
        /// Writes the given array to a worksheet
        /// </summary>
        /// <param name="worksheet">Sheet to which to write to</param>
        /// <param name="data">Data to be written</param>
        /// <param name="row">Starting row</param>
        /// <param name="col">Starting col</param>
        private Range WriteToSheet(Worksheet worksheet, string[,] data, int row, int col)
        {
            Range range = worksheet.Range[GetColumnString(col) + (row).ToString(), Type.Missing];
            range = range.Resize[data.GetLength(0), data.GetLength(1)];
            range.set_Value(Missing.Value, data);

            return range;
        }

        /// <summary>
        /// This method writes a sequence to a given worksheet.
        /// </summary>
        /// <param name="worksheet">The worksheet instance</param>
        /// <param name="sequence">The sequence which has to be imported into the excel sheet.</param>
        /// <param name="initialRowNumber">Initial row number</param>
        /// <param name="sequenceDataRange">The row number from where the sequence rendering has to begin</param>
        /// <returns>Index of last row where sequence data was written</returns>
        private int WriteSequence(Worksheet worksheet, ISequence sequence, int initialRowNumber, out Range sequenceDataRange)
        {
            int counts = 0;
            int maxColumnNumber = 0;

            Range heading = worksheet.Range["A" + initialRowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing];
            WriteRangeValue(heading, Resources.SEQUENCE_DATA);

            int rowNumber = initialRowNumber;
            int rowCount = (int)Math.Ceiling((decimal)sequence.Count / maxNumberOfCharacters);
            long columnCount = sequence.Count > maxNumberOfCharacters ? maxNumberOfCharacters : sequence.Count;
            var rangeData = new string[rowCount, columnCount];

            // Put the data into the rows.
            while (counts < sequence.Count)
            {
                int columnNumber = 1;
                for (int i = 0; (i < maxNumberOfCharacters) && (counts < sequence.Count); i++, counts++, columnNumber++)
                    rangeData[rowNumber - initialRowNumber, i] = new string(new[] { (char)sequence[counts] });

                if (columnNumber > maxColumnNumber)
                    maxColumnNumber = columnNumber;

                rowNumber++;
            }

            if (sequence.Count > 0)
            {
                Range range = worksheet.Range["B" + initialRowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing];
                range = range.Resize[rowCount, columnCount];
                range.set_Value(Missing.Value, rangeData);

                this.FillBackGroundColor(range);

                StringBuilder sb = new StringBuilder();
                sb.Append("='");
                sb.Append(worksheet.Name);
                sb.Append("'!");
                sb.Append("$B$" + initialRowNumber);
                sb.Append(":$");
                sb.Append(GetColumnString(maxColumnNumber) + "$" + (rowNumber - 1).ToString(CultureInfo.CurrentCulture));
                string formula = sb.ToString();

                worksheet.Names.Add(Resources.SEQUENCEDATA_PRESELECTION + GetValidFileNames(sequence.ID), 
                    formula, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                Range fullSelection = worksheet.Range["B" + initialRowNumber.ToString(CultureInfo.CurrentCulture), GetColumnString(maxColumnNumber) + (rowNumber - 1).ToString(CultureInfo.CurrentCulture)];
                fullSelection.Select();
                sequenceDataRange = fullSelection;
                //added default from UI as auto detect and ignore space
                SequenceCache.Add(fullSelection, sequence, "_Auto Detect_NB");
            }
            else
            {
                sequenceDataRange = null;
            }

            return rowNumber;
        }

        /// <summary>
        /// Changes the status bar to reflect the status of the alignment and web-service.
        /// </summary>
        /// <param name="status">Status to be displayed.</param>
        private void ChangeStatusBar(string status)
        {
            bool isCompleted = false;
            int retryCount = 1;

            while (!isCompleted && retryCount <= MaxRetryCount)
            {
                try
                {
                    Globals.ThisAddIn.Application.StatusBar = status;
                    isCompleted = true;
                }
                catch (COMException ex)
                {
                    if (VbaIgnoreErrorCode == ex.ErrorCode && retryCount < MaxRetryCount)
                    {
                        Thread.Sleep(RetryInterval * retryCount);
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Enables or disables the screen update
        /// Retry the operation till it succeeds / all retries times out.
        /// </summary>
        /// <param name="isEnable">Is enabled</param>
        private void ScreenUpdate(bool isEnable)
        {
            bool isCompleted = false;
            int retryCount = 1;

            while (!isCompleted && retryCount <= MaxRetryCount)
            {
                try
                {
                    Globals.ThisAddIn.Application.ScreenUpdating = isEnable;
                    isCompleted = true;
                }
                catch (COMException ex)
                {
                    if (VbaIgnoreErrorCode == ex.ErrorCode && retryCount < MaxRetryCount)
                    {
                        Thread.Sleep(RetryInterval * retryCount);
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the instance of IWebService depending on the service name.
        /// </summary>
        /// <param name="webserviceName">Name of the webservice</param>
        /// <returns>Instance of the web service</returns>
        private IBlastServiceHandler GetWebServiceInstance(string webserviceName)
        {
            foreach (IBlastServiceHandler blastService in
                WebServices.All.Where(service => service is IBlastServiceHandler))
            {
                if (blastService.Name.Equals(this.webserviceName))
                {
                    return blastService;
                }
            }

            return null;
        }

        #region -- Web Service --

        /// <summary>
        /// This method is called when the user prompts for a sequence search on
        /// the NCBI database. This method creates a type of web-service object
        /// and passes parameters to it. And waits for the result.
        /// </summary>
        /// <param name="sender">IAssembler instance</param>
        /// <param name="e">Event data.</param>
        private void OnExecuteSearch(List<ISequence> sequences, params object[] args)
        {
            Globals.ThisAddIn.Application.Cursor = XlMousePointer.xlWait;
            BlastDialog dialog = new BlastDialog(this.webserviceName);
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(dialog);
            helper.Owner = (IntPtr)Globals.ThisAddIn.Application.Hwnd;
            dialog.Activated += new EventHandler(OnWPFWindowActivated);
            dialog.ShowDialog();

            if (dialog.WebServiceInputArgs != null)
            {
                WebServiceInputEventArgs e = dialog.WebServiceInputArgs;

                this.btnCancelSearch.Enabled = this.cancelSearchButtonState = true;
                this.requestIdentifier = string.Empty;

                if (e != null && !string.IsNullOrEmpty(this.webserviceName))
                {
                    IBlastServiceHandler blastServiceHandler =
                            this.GetWebServiceInstance(this.webserviceName);

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
                        if (WebServices.BioHPCBlast != null && WebServices.BioHPCBlast.Name.Equals(this.webserviceName))
                        {
                            this.requestIdentifier = blastServiceHandler.SubmitRequest(sequences, e.ServiceParameters);
                        }
                        else
                        {
                            this.requestIdentifier = blastServiceHandler.SubmitRequest(sequences[0], e.ServiceParameters);
                        }

                        this.ChangeStatusBar(string.Format(Properties.Resources.WEBSERVICE_STATUS_BAR, Resources.SEARCHING));
                    }
                    catch (Exception ex)
                    {
                        this.btnCancelSearch.Enabled = this.cancelSearchButtonState = false;
                        MessageBox.Show(
                                ex.Message,
                                Properties.Resources.CAPTION,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// This event is fired when the search on NCBI database is completed.
        /// This event asks IWebServicePresneter to display BLAST outputs.
        /// </summary>
        /// <param name="sender">BackgroundWorker instance.</param>
        /// <param name="e">Event data.</param>
        private void OnSearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.btnCancelSearch.Enabled = this.cancelSearchButtonState = false;
            if (e.Cancelled)
            {
                return;
            }

            this.ScreenUpdate(false);

            List<BlastResult> blastResults = e.Result as List<BlastResult>;

            if (blastResults != null)
            {
                if (BlastHasResults(blastResults))
                {
                    Workbook activeWorkBook = (Workbook)Globals.ThisAddIn.Application.ActiveWorkbook;
                    Worksheet activesheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;
                    Worksheet currentsheet = (Worksheet)activeWorkBook.Worksheets.Add(Type.Missing, activesheet, Type.Missing, Type.Missing);

                    currentsheet.Name = Properties.Resources.BLAST_RESULTS + this.currentBlastSheetNumber.ToString();
                    this.currentBlastSheetNumber++;

                    int rowNumber = 2;
                    int columnNumber = 2;

                    foreach (string header in blastHeaders)
                    {
                        Range range = currentsheet.get_Range(GetColumnString(columnNumber) + rowNumber, Type.Missing);
                        range.Cells.Font.Bold = true;
                        WriteRangeValue(range, header);
                        columnNumber++;
                    }

                    rowNumber++;

                    foreach (BlastResult result in blastResults)
                    {
                        foreach (BlastSearchRecord record in result.Records)
                        {
                            if (null != record.Hits
                                    && 0 < record.Hits.Count)
                            {
                                foreach (Hit hit in record.Hits)
                                {
                                    if (null != hit.Hsps
                                            && 0 < hit.Hsps.Count)
                                    {
                                        foreach (Hsp hsp in hit.Hsps)
                                        {
                                            DisplayValues(
                                                    currentsheet,
                                                    rowNumber,
                                                    record.IterationQueryId,
                                                    hit.Id,
                                                    hsp.IdentitiesCount.ToString(CultureInfo.CurrentCulture),
                                                    hsp.AlignmentLength.ToString(CultureInfo.CurrentCulture),
                                                    hit.Length.ToString(CultureInfo.CurrentCulture),
                                                    hsp.QueryStart.ToString(),
                                                    hsp.QueryEnd.ToString(),
                                                    hsp.HitStart.ToString(CultureInfo.CurrentCulture),
                                                    hsp.HitEnd.ToString(),
                                                    hsp.EValue.ToString(CultureInfo.CurrentCulture),
                                                    hsp.BitScore.ToString(CultureInfo.CurrentCulture));
                                            rowNumber++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    currentsheet.Columns.AutoFit();
                    this.EnableAllControls();
                }
                else
                {
                    MessageBox.Show(Properties.Resources.BLAST_NO_RESULT, Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.ChangeStatusBar(string.Format(Properties.Resources.WEBSERVICE_STATUS_BAR, Resources.DONE));
            }
            else if (e.Result is string)
            {
                this.ChangeStatusBar(string.Format(Properties.Resources.WEBSERVICE_STATUS_BAR, Resources.DONE));
                MessageBox.Show(e.Result.ToString(), Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.ScreenUpdate(true);
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

        #endregion

        #region -- Assembly --

        /// <summary>
        /// This event is fired when user wants to assemble the sequences.
        /// This event will be raised by IAssembler. The controller class
        /// instantiates algorithm implementation to perform assembly.
        /// </summary>
        /// <param name="e">Assembly input arguments.</param>
        private void OnRunAssemblerAlgorithm(AssemblyInputEventArgs e)
        {
            if (e.Sequences != null && e.Aligner != null)
            {
                this.btnCancelAssemble.Enabled = this.cancelAssemblyButtonState = true;
                this.ChangeStatusBar(string.Format(Properties.Resources.ASSEMBLER_STATUS_BAR, Resources.ASSEMBLING));
                this.assemblerThread = new BackgroundWorker();
                this.assemblerThread.WorkerSupportsCancellation = true;
                this.assemblerThread.DoWork += new DoWorkEventHandler(this.OnAssembleStarted);
                this.assemblerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.OnAssemblerCompleted);
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

            IDeNovoAssembly assemblerResult = e.Result as IDeNovoAssembly;
            if (assemblerResult != null)
            {
                this.BuildConsensusView(assemblerResult);
            }

            this.btnCancelAssemble.Enabled = this.cancelAssemblyButtonState = false;
            this.ChangeStatusBar(string.Format(Properties.Resources.ASSEMBLER_STATUS_BAR, Resources.DONE));

            // This is an error scenario, display it to the users.
            string errorMessage = e.Result as string;
            if (errorMessage != null)
            {
                this.ChangeStatusBar(string.Format(Properties.Resources.ASSEMBLER_STATUS_BAR, Resources.ERROR));
                MessageBox.Show(errorMessage, Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method displays the output of a assembly process.
        /// </summary>
        /// <param name="assemblerResult">Result of the assembly process.</param>
        private void BuildConsensusView(IDeNovoAssembly assemblerResult)
        {
            IOverlapDeNovoAssembly overlapAssemblerResult = assemblerResult as IOverlapDeNovoAssembly;
            if (overlapAssemblerResult != null)
            {
                this.ScreenUpdate(false);
                Workbook activeWorkBook = (Workbook)Globals.ThisAddIn.Application.ActiveWorkbook;
                Worksheet activesheet = (Worksheet)Globals.ThisAddIn.Application.ActiveSheet;
                Worksheet currentsheet = (Worksheet)activeWorkBook.Worksheets.Add(Type.Missing, activesheet, Type.Missing, Type.Missing);
                string[,] rangeData;
                int rowNumber = 1;
                int contigNumber = 1;
                int rowCount, rowIndex, columnIndex;

                ((Microsoft.Office.Interop.Excel._Worksheet)currentsheet).Activate();
                currentsheet.Name = this.GetValidFileNames(
                        "ConsensusView" + this.currentConsensusSheetNumber.ToString(CultureInfo.CurrentCulture));
                currentsheet.Cells.Font.Name = "Courier New";
                this.currentConsensusSheetNumber++;
                foreach (Contig contig in overlapAssemblerResult.Contigs)
                {
                    // Write Header
                    Range header = currentsheet.get_Range("A" + rowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing);
                    WriteRangeValue(header, "Contig" + contigNumber.ToString(CultureInfo.CurrentCulture));

                    ISequence contigSequence = contig.Consensus;
                    Range currentRange = currentsheet.get_Range("B" + rowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing);

                    long numberofCharacters = 1;
                    if (contigSequence.Count > MaxExcelColumns)
                    {
                        if (contigSequence.Count % MaxExcelColumns == 0)
                        {
                            numberofCharacters = contigSequence.Count / MaxExcelColumns;
                        }
                        else
                        {
                            numberofCharacters = contigSequence.Count / MaxExcelColumns;
                            numberofCharacters++;
                        }
                    }

                    int columnCount = 1;

                    rowCount = (int)Math.Ceiling((decimal)contigSequence.Count / (decimal)MaxExcelColumns);
                    rowIndex = 0;
                    columnIndex = 0;
                    rangeData = new string[
                            rowCount,
                            contigSequence.Count > MaxExcelColumns
                                ? MaxExcelColumns
                                : contigSequence.Count];

                    for (long i = 0; i < contigSequence.Count; i += numberofCharacters)
                    {
                        if (MaxExcelColumns == columnIndex)
                        {
                            columnIndex = 0;
                            rowIndex++;
                        }

                        ISequence tempSeq = contigSequence.GetSubSequence(i, numberofCharacters);

                        string subsequence = tempSeq.ConvertToString();
                        rangeData[rowIndex, columnIndex] = subsequence;

                        columnIndex++;
                        columnCount++;
                    }

                    StringBuilder formulaBuilder = new StringBuilder();
                    string formula = string.Empty;
                    string name = string.Empty;
                    if (columnCount > 1)
                    {
                        currentRange = currentRange.get_Resize(1, columnCount - 1);
                        currentRange.set_Value(Missing.Value, rangeData);
                        this.FillBackGroundColor(currentRange);
                        formulaBuilder.Append("=");
                        formulaBuilder.Append(currentsheet.Name);
                        formulaBuilder.Append("!");
                        formulaBuilder.Append("$B$" + rowNumber);
                        formulaBuilder.Append(":$");
                        formulaBuilder.Append(GetColumnString(columnCount) + "$" + rowNumber.ToString(CultureInfo.CurrentCulture));
                        formula = formulaBuilder.ToString();
                        name = Properties.Resources.CONTIG + contigNumber.ToString(CultureInfo.CurrentCulture);

                        currentsheet.Names.Add(GetValidFileNames(name), formula, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    }

                    rowNumber++;

                    int sequenceNumber = 1;
                    foreach (Contig.AssembledSequence assembled in contig.Sequences)
                    {
                        int initialRowNumber = rowNumber;
                        columnCount = 1;

                        ISequence assembledSequence = assembled.Sequence;

                        // Write Header
                        Range sequenceHeader = currentsheet.get_Range("A" + rowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing);
                        WriteRangeValue(sequenceHeader, assembledSequence.ID);

                        if (assembled.IsComplemented && assembled.IsReversed)
                        {
                            assembledSequence = assembled.Sequence.GetReverseComplementedSequence();
                            sequenceHeader.Cells.AddComment(Resources.SEQUENCE_REVERSECOMPLEMENT);
                        }
                        else if (assembled.IsReversed)
                        {
                            assembledSequence = assembled.Sequence.GetReversedSequence();
                            sequenceHeader.Cells.AddComment(Resources.SEQUENCE_REVERSE);
                        }
                        else if (assembled.IsComplemented)
                        {
                            assembledSequence = assembled.Sequence.GetComplementedSequence();
                            sequenceHeader.Cells.AddComment(Resources.SEQUENCE_COMPLEMENT);
                        }

                        long startingColumn = assembled.Position / numberofCharacters;
                        startingColumn++;
                        currentRange = currentsheet.get_Range(GetColumnString(startingColumn + 1) + rowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing);

                        long startingIndex = 0;

                        if (numberofCharacters > 1)
                        {
                            long cellStartIndex = (startingColumn - 1) * numberofCharacters;
                            long endingIndex = cellStartIndex + numberofCharacters - 1;
                            long startextractCharacters = endingIndex - assembled.Position + 1;
                            long numberOfSpaces = Math.Abs(assembled.Position - cellStartIndex);

                            string firstcell = assembledSequence.GetSubSequence(0, startextractCharacters).ConvertToString();
                            StringBuilder sb = new StringBuilder();

                            for (int i = 1; i <= numberOfSpaces; i++)
                            {
                                sb.Append(" ");
                            }

                            sb.Append(firstcell);
                            WriteRangeValue(currentRange, sb.ToString());
                            startingIndex = startextractCharacters;
                            currentRange = currentRange.Next;
                        }

                        rowCount = (int)Math.Ceiling((decimal)assembledSequence.Count / (decimal)MaxExcelColumns);
                        rowIndex = 0;
                        columnIndex = 0;
                        rangeData = new string[
                                rowCount,
                                assembledSequence.Count > MaxExcelColumns
                                    ? MaxExcelColumns
                                    : assembledSequence.Count];

                        for (long i = startingIndex; i < assembledSequence.Count; i += numberofCharacters)
                        {
                            if (MaxExcelColumns == columnIndex)
                            {
                                columnIndex = 0;
                                rowIndex++;
                            }

                            ISequence tempSeq = assembledSequence.GetSubSequence(i, numberofCharacters);

                            string derivedSequence = tempSeq.ConvertToString();
                            rangeData[rowIndex, columnIndex] = derivedSequence;

                            columnIndex++;
                            columnCount++;
                        }

                        if (columnCount > 1)
                        {
                            currentRange = currentRange.get_Resize(1, columnCount - 1);
                            currentRange.set_Value(Missing.Value, rangeData);

                            this.FillBackGroundColor(currentRange);

                            formulaBuilder = new StringBuilder();
                            formulaBuilder.Append("=");
                            formulaBuilder.Append(currentsheet.Name);
                            formulaBuilder.Append("!$");
                            formulaBuilder.Append(GetColumnString(startingColumn + 1) + "$" + initialRowNumber);
                            formulaBuilder.Append(":$");
                            formulaBuilder.Append(GetColumnString(startingColumn + columnCount - 1) + "$" + rowNumber.ToString(CultureInfo.CurrentCulture));
                            string sequenceFormula = formulaBuilder.ToString();
                            name = Properties.Resources.CONTIG + contigNumber.ToString() + "_" + assembledSequence.ID + sequenceNumber.ToString(CultureInfo.CurrentCulture);
                            currentsheet.Names.Add(GetValidFileNames(name), sequenceFormula, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        }

                        rowNumber++;
                        sequenceNumber++;
                    }

                    contigNumber++;
                    rowNumber++;
                }

                int unmerged = 1;
                foreach (ISequence sequence in overlapAssemblerResult.UnmergedSequences)
                {
                    // Write Header
                    Range sequenceHeader = currentsheet.get_Range("A" + rowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing);
                    WriteRangeValue(sequenceHeader, "Unmerged Sequence_" + sequence.ID);

                    long numberofCharacters = 1;
                    if (sequence.Count > MaxExcelColumns)
                    {
                        if (sequence.Count % MaxExcelColumns == 0)
                        {
                            numberofCharacters = sequence.Count / MaxExcelColumns;
                        }
                        else
                        {
                            numberofCharacters = sequence.Count / MaxExcelColumns;
                            numberofCharacters++;
                        }
                    }

                    Range currentRange = currentsheet.get_Range("B" + rowNumber.ToString(CultureInfo.CurrentCulture), Type.Missing);

                    int columnCount = 1;
                    rowCount = (int)Math.Ceiling((decimal)sequence.Count / (decimal)MaxExcelColumns);
                    rowIndex = 0;
                    columnIndex = 0;
                    rangeData = new string[
                            rowCount,
                            sequence.Count > MaxExcelColumns
                                ? MaxExcelColumns
                                : sequence.Count];

                    for (long i = 0; i < sequence.Count; i += numberofCharacters)
                    {
                        if (MaxExcelColumns == columnIndex)
                        {
                            columnIndex = 0;
                            rowIndex++;
                        }

                        ISequence tempSeq = sequence.GetSubSequence(i, numberofCharacters);

                        string subsequence = tempSeq.ConvertToString();
                        rangeData[rowIndex, columnIndex] = subsequence;

                        columnIndex++;
                        columnCount++;
                    }

                    if (columnCount > 1)
                    {
                        currentRange = currentRange.get_Resize(1, columnCount - 1);
                        currentRange.set_Value(Missing.Value, rangeData);

                        this.FillBackGroundColor(currentRange);

                        StringBuilder formulaBuilder = new StringBuilder();
                        formulaBuilder.Append("=");
                        formulaBuilder.Append(currentsheet.Name);
                        formulaBuilder.Append("!");
                        formulaBuilder.Append("$B$" + rowNumber);
                        formulaBuilder.Append(":$");
                        formulaBuilder.Append(GetColumnString(columnCount) + "$" + rowNumber.ToString(CultureInfo.CurrentCulture));
                        string formula = formulaBuilder.ToString();
                        string name = Properties.Resources.UNMERGED_SEQUENCE + unmerged.ToString(CultureInfo.CurrentCulture);
                        currentsheet.Names.Add(GetValidFileNames(name), formula, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    }

                    rowNumber++;
                    unmerged++;
                }

                currentsheet.Columns.AutoFit();
                this.EnableAllControls();
                this.ScreenUpdate(true);
            }
        }

        /// <summary>
        /// This method fills the background color of a particular range depending
        /// on the content within the range.
        /// </summary>
        /// <param name="currentRange">Range whose background color has to be filled.</param>
        private void FillBackGroundColor(Range currentRange)
        {
            ScreenUpdate(false);
            if (currentRange.FormatConditions.Count > 0)
            {
                currentRange.FormatConditions.Delete();
            }

            foreach (KeyValuePair<byte, Color> color in this.colorMap)
            {
                if (color.Value == null) // skip if no color specified
                {
                    continue;
                }
                if (color.Value.A == 0) // skip if transparent
                {
                    continue;
                }

                Range keyExists = currentRange.Find(
                    new string(new char[] { (char)color.Key }),
                    Missing.Value,
                    Missing.Value,
                    XlLookAt.xlWhole,
                    Missing.Value,
                    XlSearchDirection.xlNext,
                    false,
                    Missing.Value,
                    Missing.Value);

                if (null != keyExists)
                {
                    FormatCondition cond = (FormatCondition)currentRange.FormatConditions.Add(
                            XlFormatConditionType.xlCellValue,
                            XlFormatConditionOperator.xlEqual,
                            new string(new char[] { (char)color.Key }),
                             Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value,
                            Missing.Value);

                    cond.Interior.Color = ColorTranslator.ToWin32(color.Value);
                }
            }

            ScreenUpdate(true);
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
                        IDeNovoAssembly assemblerResult = RunAssembly(assemblerInput, worker);
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

        #endregion

        /// <summary>
        /// This event is triggered when any of the buttons in ribbon is clicked.
        /// </summary>
        private void RibbonControl_Click(object sender, RibbonControlEventArgs e)
        {
            ResetStatus();
        }

        /// <summary>
        /// Reset the status to Ready
        /// </summary>
        private void ResetStatus()
        {
            this.ChangeStatusBar(Resources.STATUS_READY);
        }

        #region Export options

        /// <summary>
        /// Export data from sheets to a particulat sequence file format
        /// </summary>
        void OnExportClick(object sender, RibbonControlEventArgs e)
        {
            ISequenceFormatter formatter = ((sender as RibbonButton).Tag as ISequenceFormatter);
            if (formatter is FastAFormatter || formatter is FastQFormatter || formatter is GenBankFormatter || formatter is GffFormatter)
            {
                InputSelection sequenceSelection = new InputSelection();

                if (formatter is GenBankFormatter)
                {
                    sequenceSelection.MaximumSequenceCount = 1;
                    sequenceSelection.MinimumSequenceCount = 1;
                }

                sequenceSelection.GetSequencesForExport(DoExportSequence, formatter);
            }
            else
            {
                // as its not a ISequenceFormatter try to cast it to ISequenceRangeFormatter
                ISequenceRangeFormatter rangeformatter = ((sender as RibbonButton).Tag as ISequenceRangeFormatter);

                if (rangeformatter is ISequenceRangeFormatter)
                {
                    InputSelection sequenceSelection = new InputSelection();
                    sequenceSelection.SequenceLabels = new string[] { Resources.Export_BED_SequenceRangeString };
                    sequenceSelection.MaximumSequenceCount = 1;
                    sequenceSelection.PromptForSequenceName = false;
                    sequenceSelection.GetInputSequenceRanges(DoExportRangeSequence, false, false, false, rangeformatter);
                }
            }
        }

        /// <summary>
        /// Method which will export a parsed BED file
        /// </summary>
        /// <param name="e">Event Arguments</param>
        private void DoExportRangeSequence(InputSequenceRangeSelectionEventArg e)
        {
            TextWriter sequenceWriter = null;
            //todo: Below line used hard coded BEDFormatter, have to get it from args later on
            ISequenceRangeFormatter formatter = new Bio.IO.Bed.BedFormatter();
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();

            saveDialog.Filter = "BED Files|*.BED";

            try
            {
                if (e.Sequences[0].GroupIDs.Count() > 0)
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get a textwriter to the file which will append text to it.
                        sequenceWriter = new StreamWriter(saveDialog.FileName, false);
                        formatter.Format(e.Sequences[0], sequenceWriter);
                    }
                }
                else
                {
                    MessageBox.Show(Resources.EMPTY_SEQUENCE_RANGE, Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sequenceWriter != null)
                {
                    sequenceWriter.Close();
                }
            }
        }

        /// <summary>
        /// Method which will export the parsed data to the appropriate file format
        /// </summary>
        /// <param name="sequences"></param>
        /// <param name="args"></param>
        private void DoExportSequence(List<ISequence> sequences, params object[] args)
        {
            bool openedFormatter = false;
            ISequenceFormatter formatter = args[0] as ISequenceFormatter;
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();

            if (formatter is FastAFormatter)
                saveDialog.Filter = "FastA Files|*.FastA";
            else if (formatter is FastQFormatter)
                saveDialog.Filter = "FastQ Files|*.FastQ";
            else if (formatter is GenBankFormatter)
                saveDialog.Filter = "GenBank Files|*.gbk";
            else if (formatter is GffFormatter)
                saveDialog.Filter = "Gff Files|*.gff";
            try
            {
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Remove file is exists
                    if (File.Exists(saveDialog.FileName))
                        File.Delete(saveDialog.FileName);

                    // Get a textwriter to the file which will append text to it.
                    formatter.Open(saveDialog.FileName);
                    openedFormatter = true;

                    // Check the formatter chosen, Loop through if there are multiple sequences selected and append to the file
                    if (formatter is FastAFormatter || formatter is FastQFormatter || formatter is GenBankFormatter)
                    {
                        foreach (ISequence currentSequence in sequences)
                        {
                            formatter.Write(currentSequence);
                        }
                    }
                    else if (formatter is GffFormatter)
                    {
                        GffFormatter gffFormatter = formatter as GffFormatter;
                        if (gffFormatter != null)
                        {
                            gffFormatter.ShouldWriteSequenceData = true;
                        }

                        foreach (ISequence currentSequence in sequences)
                        {
                            formatter.Write(currentSequence);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (openedFormatter)
                    formatter.Close();
            }
        }

        #endregion

        private void btnUserManual_Click(object sender, RibbonControlEventArgs e)
        {
            string userGuidePath = MBFInstallationPath + Properties.Resources.UserGuideRelativePath;

            if (File.Exists(userGuidePath))
            {
                System.Diagnostics.Process.Start(userGuidePath);
            }
            else
            {
                MessageBox.Show(Properties.Resources.NoUserGuidePresent, Properties.Resources.CAPTION, MessageBoxButtons.OK);
            }
        }

        #endregion -- Private Methods --

    }
}
