using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Padena;
using Bio.IO.FastA;
using Bio.Util;

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
        /// Enable removal of low coverage contigs.
        /// </summary>
        public bool LowCoverageContigRemovalEnabled = false;

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

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed read file: {0}", Path.GetFullPath(this.Filename));
                Console.Error.WriteLine("            Read/Processing time: {0}", runAlgorithm.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", refFileLength);
                Console.Error.WriteLine("            k-mer Length        : {0}", this.KmerLength);
            }

            runAlgorithm.Restart();
            ValidateAmbiguousReads(reads);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Time taken for Validating reads: {0}", runAlgorithm.Elapsed);
            }

            using (ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler())
            {
                assembler.StatusChanged += new EventHandler<StatusChangedEventArgs>(this.AssemblerStatusChanged);
                assembler.AllowErosion = this.AllowErosion;
                assembler.AllowKmerLengthEstimation = this.AllowKmerLengthEstimation;
                assembler.AllowLowCoverageContigRemoval = this.LowCoverageContigRemovalEnabled;
                assembler.ContigCoverageThreshold = this.ContigCoverageThreshold;
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
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("  Compute time: {0}", runAlgorithm.Elapsed);
                }

                runAlgorithm.Restart();
                this.WriteContigs(assembly);
                runAlgorithm.Stop();
                algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);
                if (this.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("  Write contigs time: {0}", runAlgorithm.Elapsed);
                    Console.Error.WriteLine("  Total runtime: {0}", algorithmSpan);
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
            }
            else
            {
                foreach (ISequence seq in assembly.AssembledSequences)
                {
                    Console.WriteLine(seq.ID);
                    Console.WriteLine(new string(seq.Select(a => (char)a).ToArray()));
                }
            }
        }

        /// <summary>
        /// Parses the File.
        /// </summary>
        protected IEnumerable<ISequence> ParseFile()
        {
            // TODO: Add other parsers.
            FastAParser parser = new FastAParser(this.Filename);
            return parser.Parse();
        }

        /// <summary>
        /// Method to handle status changed event.
        /// </summary>
        /// <param name="statusMessage">Status message.</param>
        protected void AssemblerStatusChanged(object sender, StatusChangedEventArgs statusEventArgs)
        {
            Console.Error.Write(statusEventArgs.StatusMessage);
        }

        /// <summary>
        /// Checks for ambiguous reads if any, if found ArgumentException.
        /// </summary>
        /// <param name="reads">Input reads.</param>
        protected static void ValidateAmbiguousReads(IEnumerable<ISequence> reads)
        {
            foreach (ISequence seq in reads)
            {
                if (seq.Alphabet.HasAmbiguity)
                {
                    throw new ArgumentException(Properties.Resources.AmbiguousReadsNotSupported);
                }
            }
        }
        #endregion
    }
}
