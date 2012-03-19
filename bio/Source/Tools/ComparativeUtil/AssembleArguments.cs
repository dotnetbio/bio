using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Comparative;
using Bio.IO;
using Bio.IO.FastA;
using Bio.Util;
using ComparativeUtil.Properties;

namespace ComparativeUtil
{
    /// <summary>
    /// Class for Assemble options.
    /// </summary>
    internal class AssembleArguments
    {
        #region Public Fields

        /// <summary>
        /// Paths of input and output file.
        /// </summary>
        public string[] FilePath = null;

        /// <summary>
        /// Use anchor matches that are unique in both the reference and query.
        /// </summary>
        public int KmerLength = 10;

        /// <summary>
        /// Run scaffolding step after generating contigs
        /// </summary>
        public bool Scaffold = false;

        /// <summary>
        /// Minimum length of MUM.
        /// </summary>
        public int MumLength = 20;

        /// <summary>
        /// Print the help information.
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// Output file.
        /// </summary>
        public string OutputFile = null;

        /// <summary>
        /// Clone Library Name.
        /// </summary>
        public string CloneLibraryName = string.Empty;

        /// <summary>
        /// Mean Length of clone library.
        /// </summary>
        public double MeanLengthOfInsert = 0;

        /// <summary>
        /// Standard Deviation of Clone Library.
        /// </summary>
        public double StandardDeviationOfInsert = 0;

        /// <summary>
        /// Display verbose logging during processing.
        /// </summary>
        public bool Verbose = false;
        #endregion

        #region Public methods

        /// <summary>
        /// It assembles the sequences.
        /// </summary>
        public virtual void AssembleSequences()
        {
            if (this.FilePath.Length != 2)
            {
                Output.WriteLine(OutputLevel.Error, "Error: A reference file and 1 query file are required.");
                return;
            }

            TimeSpan timeSpan = new TimeSpan();
            Stopwatch runAlgorithm = new Stopwatch();
            FileInfo inputFileinfo = new FileInfo(this.FilePath[0]);
            long inputFileLength = inputFileinfo.Length;
            inputFileinfo = null;

            if (!string.IsNullOrEmpty(this.CloneLibraryName))
            {
                CloneLibrary.Instance.AddLibrary(this.CloneLibraryName, (float)this.MeanLengthOfInsert, (float)this.StandardDeviationOfInsert);
            }

            runAlgorithm.Restart();

            // Parse input files
            IEnumerable<ISequence> referenceSequences = ParseFile(this.FilePath[0]);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Processed reference file: {0}", Path.GetFullPath(this.FilePath[0]));
                Output.WriteLine(OutputLevel.Verbose, "   Read/Processing time : {0}", runAlgorithm.Elapsed);
                Output.WriteLine(OutputLevel.Verbose, "   File Size            : {0}", inputFileLength);
            }

            inputFileinfo = new FileInfo(this.FilePath[1]);
            inputFileLength = inputFileinfo.Length;
            runAlgorithm.Restart();
            FastASequencePositionParser queryParser = new FastASequencePositionParser(this.FilePath[1], true);
            queryParser.CacheSequencesForRandomAccess();
            IEnumerable<ISequence> reads = queryParser.Parse();
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Processed reads file   : {0}", Path.GetFullPath(this.FilePath[1]));
                Output.WriteLine(OutputLevel.Verbose, "   Read/Processing time: {0}", runAlgorithm.Elapsed);
                Output.WriteLine(OutputLevel.Verbose, "   File Size           : {0}", inputFileLength);
            }

            runAlgorithm.Restart();
            ValidateAmbiguousReads(reads);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Time taken for Validating reads: {0}", runAlgorithm.Elapsed);
                Output.WriteLine(OutputLevel.Verbose);
            }

            runAlgorithm.Restart();
            ComparativeGenomeAssembler assembler = new ComparativeGenomeAssembler();
            assembler.StatusChanged += this.AssemblerStatusChanged;
            assembler.ScaffoldingEnabled = this.Scaffold;
            assembler.KmerLength = this.KmerLength;
            assembler.LengthOfMum = this.MumLength;
            IEnumerable<ISequence> assemblerResult = assembler.Assemble(referenceSequences, queryParser);
            runAlgorithm.Stop();
            timeSpan = timeSpan.Add(runAlgorithm.Elapsed);

            runAlgorithm.Restart();

            this.WriteContigs(assemblerResult);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose, "Assemble time: {0}", timeSpan);
                Output.WriteLine(OutputLevel.Verbose, "Write time: {0}", runAlgorithm.Elapsed);
            }
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Helper method to parse the given filename into a set
        /// of ISequence elements. This routine will load sequences from
        /// any support sequence parser in .NET Bio.
        /// </summary>
        /// <param name="fileName">Filename to load data from</param>
        /// <returns>Enumerable set of ISequence elements</returns>
        protected static IEnumerable<ISequence> ParseFile(string fileName)
        {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(fileName);
            if (parser == null)
                throw new Exception("Could not locate an appropriate sequence parser for " + fileName);

            return parser.Parse();
        }

        /// <summary>
        /// It Writes the contigs to the file.
        /// </summary>
        /// <param name="assembly">IDeNovoAssembly parameter is the result of running De Novo Assembly on a set of two or more sequences. </param>
        protected void WriteContigs(IEnumerable<ISequence> assembly)
        {
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                using (FastAFormatter formatter = new FastAFormatter(this.OutputFile))
                {
                    formatter.AutoFlush = true;

                    foreach (ISequence seq in assembly)
                    {
                        formatter.Write(seq);
                    }
                }
                Output.WriteLine(OutputLevel.Information, Resources.OutPutWrittenToFileSpecified);
            }
            else
            {
                Output.WriteLine(OutputLevel.Information, "Assembled Sequence Results:");
                foreach (ISequence seq in assembly)
                {
                    Output.WriteLine(OutputLevel.Results, seq.ID);
                    Output.WriteLine(OutputLevel.Results, new string(seq.Select(a => (char)a).ToArray()));
                }
            }
        }

        /// <summary>
        /// Method to handle status changed event.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="statusEventArgs">Status message.</param>
        protected void AssemblerStatusChanged(object sender, StatusChangedEventArgs statusEventArgs)
        {
            if (Verbose)
                Output.WriteLine(OutputLevel.Verbose, statusEventArgs.StatusMessage);
            else
            {
                if (statusEventArgs.StatusMessage.StartsWith("Step", StringComparison.OrdinalIgnoreCase)
                    && statusEventArgs.StatusMessage.Contains("Start"))
                {
                    int pos = statusEventArgs.StatusMessage.IndexOf(" - ", StringComparison.OrdinalIgnoreCase);
                    Output.WriteLine(OutputLevel.Information, statusEventArgs.StatusMessage.Substring(0, pos));
                }
            }
        }

        /// <summary>
        /// Checks for ambiguous reads if any, if found ArgumentException.
        /// </summary>
        /// <param name="reads">Input reads.</param>
        private static void ValidateAmbiguousReads(IEnumerable<ISequence> reads)
        {
            if (reads.Any(s => s.Alphabet.HasAmbiguity))
                throw new ArgumentException(Properties.Resources.AmbiguousReadsNotSupported);
        }
        #endregion
    }
}
