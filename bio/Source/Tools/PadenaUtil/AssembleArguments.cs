using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Padena;
using Bio.IO;
using Bio.IO.FastA;
using Bio.Util;
using PadenaUtil.Properties;

namespace PadenaUtil
{
    /// <summary>
    /// Class for Assemble options.
    /// </summary>
    internal class AssembleArguments
    {
        #region Public Fields

        /// <summary>
        /// Length of k-mer.
        /// </summary>
        public int KmerLength = 10;

        /// <summary>
        /// Threshold for removing dangling ends in graph.
        /// </summary>
        public int DangleThreshold = -1;

        /// <summary>
        /// Length Threshold for removing redundant paths in graph.
        /// </summary>
        public int RedundantPathLengthThreshold = -1;

        /// <summary>
        /// Threshold for eroding low coverage ends.
        /// </summary>
        public int ErosionThreshold = -1;

        /// <summary>
        /// Bool to do erosion or not.
        /// </summary>
        public bool AllowErosion = false;

        /// <summary>
        /// Whether to estimate kmer length.
        /// </summary>
        public bool AllowKmerLengthEstimation = false;

        /// <summary>
        /// Threshold used for removing low-coverage contigs.
        /// </summary>
        public int ContigCoverageThreshold = -1;

        /// <summary>
        /// Help.
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// Input file of reads.
        /// </summary>
        public string Filename = string.Empty;

        /// <summary>
        /// Output file.
        /// </summary>
        public string OutputFile = string.Empty;

        /// <summary>
        /// Display verbose logging during processing.
        /// </summary>
        public bool Verbose = false;

        /// <summary>
        /// Quiet flag (no logging)
        /// </summary>
        public bool Quiet = false;

        #endregion

        #region Public methods

        /// <summary>
        /// It assembles the sequences.
        /// </summary>
        public virtual void AssembleSequences()
        {
            TimeSpan algorithmSpan = new TimeSpan();
            Stopwatch runAlgorithm = new Stopwatch();
            FileInfo refFileinfo = new FileInfo(this.Filename);
            long refFileLength = refFileinfo.Length;

            runAlgorithm.Restart();
            IEnumerable<ISequence> reads = this.ParseFile();
            runAlgorithm.Stop();
            algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);

            Output.WriteLine(OutputLevel.Information, Resources.AssemblyStarting);

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Processed read file: {0}", Path.GetFullPath(this.Filename));
                Output.WriteLine(OutputLevel.Verbose, "   Read/Processing time: {0}", runAlgorithm.Elapsed);
                Output.WriteLine(OutputLevel.Verbose, "   File Size           : {0}", refFileLength);
                Output.WriteLine(OutputLevel.Verbose, "   k-mer Length        : {0}", this.KmerLength);
            }

            runAlgorithm.Restart();
            ValidateAmbiguousReads(reads);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Time taken for Validating reads: {0}", runAlgorithm.Elapsed);
            }

            using (ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler())
            {
                assembler.StatusChanged += this.AssemblerStatusChanged;
                assembler.AllowErosion = this.AllowErosion;
                assembler.AllowKmerLengthEstimation = this.AllowKmerLengthEstimation;
                if (ContigCoverageThreshold != -1)
                {
                    assembler.AllowLowCoverageContigRemoval = true;
                    assembler.ContigCoverageThreshold = ContigCoverageThreshold;
                }
                assembler.DanglingLinksThreshold = this.DangleThreshold;
                assembler.ErosionThreshold = this.ErosionThreshold;
                if (!this.AllowKmerLengthEstimation)
                {
                    assembler.KmerLength = this.KmerLength;
                }

                assembler.RedundantPathLengthThreshold = this.RedundantPathLengthThreshold;
                runAlgorithm.Restart();
                IDeNovoAssembly assembly = assembler.Assemble(reads);
                runAlgorithm.Stop();
                algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);
                if (this.Verbose)
                {
                    Output.WriteLine(OutputLevel.Verbose);
                    Output.WriteLine(OutputLevel.Verbose, "Compute time: {0}", runAlgorithm.Elapsed);
                }

                runAlgorithm.Restart();
                this.WriteContigs(assembly);
                runAlgorithm.Stop();
                algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);
                if (this.Verbose)
                {
                    Output.WriteLine(OutputLevel.Verbose);
                    Output.WriteLine(OutputLevel.Verbose, "Write contigs time: {0}", runAlgorithm.Elapsed);
                    Output.WriteLine(OutputLevel.Verbose, "Total runtime: {0}", algorithmSpan);
                }
            }
        }
        #endregion

        #region Protected Members

        /// <summary>
        /// It Writes the contigs to the file.
        /// </summary>
        /// <param name="assembly">IDeNovoAssembly parameter is the result of running De Novo Assembly on a set of two or more sequences. </param>
        protected void WriteContigs(IDeNovoAssembly assembly)
        {
            if (assembly.AssembledSequences.Count == 0)
            {
                Output.WriteLine(OutputLevel.Results, "No sequences assembled.");
                return;
            }

            EnsureContigNames(assembly.AssembledSequences);

            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                using (FastAFormatter formatter = new FastAFormatter(this.OutputFile))
                {
                    formatter.AutoFlush = true;

                    foreach (ISequence seq in assembly.AssembledSequences)
                    {
                        formatter.Write(seq);
                    }
                }
                Output.WriteLine(OutputLevel.Information, "Wrote {0} sequences to {1}", assembly.AssembledSequences.Count, this.OutputFile);
            }
            else
            {
                Output.WriteLine(OutputLevel.Information, "Assembled Sequence Results:");
                foreach (ISequence seq in assembly.AssembledSequences)
                {
                    Output.WriteLine(OutputLevel.Results, seq.ID);
                    Output.WriteLine(OutputLevel.Results, new string(seq.Select(a => (char)a).ToArray()));
                }
            }
        }

        /// <summary>
        /// Ensures the sequence contigs have a valid ID. If no ID is present
        /// then one is generated from the index and filename.
        /// </summary>
        /// <param name="sequences"></param>
        private void EnsureContigNames(IList<ISequence> sequences)
        {
            string filename = Path.GetFileNameWithoutExtension(Filename) + "_{0}";
            for (int index = 0; index < sequences.Count; index++)
            {
                ISequence inputSequence = sequences[index];
                if (string.IsNullOrEmpty(inputSequence.ID))
                    inputSequence.ID = string.Format(filename, index+1);
            }
        }

        /// <summary>
        /// Parses the File.
        /// </summary>
        protected IEnumerable<ISequence> ParseFile()
        {
            return ParseFile(this.Filename);
        }

        /// <summary>
        /// Helper method to parse the given filename into a set
        /// of ISequence elements. This routine will load sequences from
        /// any support sequence parser in .NET Bio.
        /// </summary>
        /// <param name="fileName">Filename to load data from</param>
        /// <returns>Enumerable set of ISequence elements</returns>
        internal static IEnumerable<ISequence> ParseFile(string fileName)
        {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(fileName);
            if (parser == null)
                parser = new FastAParser(fileName);

            return parser.Parse();
        }


        /// <summary>
        /// Method to handle status changed event.
        /// </summary>
        protected void AssemblerStatusChanged(object sender, StatusChangedEventArgs statusEventArgs)
        {
            if (Verbose)
                Output.WriteLine(OutputLevel.Verbose,  statusEventArgs.StatusMessage);
            else if (!Quiet)
            {
                if (statusEventArgs.StatusMessage.StartsWith("Step", StringComparison.OrdinalIgnoreCase)
                    && statusEventArgs.StatusMessage.Contains("Start"))
                {
                    int pos = statusEventArgs.StatusMessage.IndexOf(" - ", StringComparison.OrdinalIgnoreCase);
                    Output.WriteLine(OutputLevel.Information, statusEventArgs.StatusMessage.Substring(0,pos));
                }
            }
        }

        /// <summary>
        /// Checks for ambiguous reads if any, if found ArgumentException.
        /// </summary>
        /// <param name="reads">Input reads.</param>
        protected static void ValidateAmbiguousReads(IEnumerable<ISequence> reads)
        {
            if (reads.Any(s => s.Alphabet.HasAmbiguity))
                throw new ArgumentException(Properties.Resources.AmbiguousReadsNotSupported);
        }
        #endregion
    }
}
