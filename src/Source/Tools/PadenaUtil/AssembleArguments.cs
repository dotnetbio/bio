using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        public int KmerLength = 13;

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
        /// Force specified kmer (no warning prompt)
        /// </summary>
        public bool ForceKmer = false;

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
            if (reads.Any(s => s.Alphabet.HasAmbiguity))
                throw new ArgumentException(Resources.AmbiguousReadsNotSupported);
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
                FastAFormatter formatter = new FastAFormatter { AutoFlush = true };
                using (formatter.Open(this.OutputFile))
                {
                    foreach (ISequence seq in assembly.AssembledSequences)
                    {
                        formatter.Format(seq);
                    }
                }
                Output.WriteLine(OutputLevel.Information, "Wrote {0} sequences to {1}", assembly.AssembledSequences.Count, this.OutputFile);
            }
            else
            {
                Output.WriteLine(OutputLevel.Information, "Assembled Sequence Results: {0} sequences", assembly.AssembledSequences.Count);
                FastAFormatter formatter = new FastAFormatter {
                    AutoFlush = true,
                    MaxSymbolsAllowedPerLine = Math.Min(80, Console.WindowWidth - 2)
                };
                foreach (ISequence seq in assembly.AssembledSequences)
                    formatter.Format(Console.OpenStandardOutput(), seq);
            }
        }

        /// <summary>
        /// Ensures the sequence contigs have a valid ID. If no ID is present
        /// then one is generated from the index and filename.
        /// </summary>
        /// <param name="sequences"></param>
        private void EnsureContigNames(IList<ISequence> sequences)
        {
            for (int index = 0; index < sequences.Count; index++)
            {
                ISequence inputSequence = sequences[index];
                if (string.IsNullOrEmpty(inputSequence.ID))
                    inputSequence.ID = GenerateSequenceId(index+1);
            }
        }

        /// <summary>
        /// Generates a sequence Id using the output filename, or first input file.
        /// </summary>
        /// <param name="counter">Sequence counter</param>
        /// <returns>Auto-generated sequence id</returns>
        private string GenerateSequenceId(int counter)
        {
            string filename = Path.GetFileNameWithoutExtension(this.OutputFile);
            if (string.IsNullOrEmpty(filename))
                filename = Path.GetFileNameWithoutExtension(this.Filename);
            filename = filename.Replace(" ", "");
            Debug.Assert(!string.IsNullOrEmpty(filename));
            return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", filename, counter);
        }

        /// <summary>
        /// Parses the File.
        /// </summary>
        protected IList<ISequence> ParseFile()
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
        internal static IList<ISequence> ParseFile(string fileName)
        {
            ISequenceParser parser = SequenceParsers.FindParserByFileName(fileName) ?? new FastAParser();
            return parser.Parse(fileName).ToList(); // so we don't read it multiple times.
        }


        /// <summary>
        /// Method to handle status changed event.
        /// </summary>
        protected void AssemblerStatusChanged(string statusMessage)
        {
            if (Verbose)
                Output.WriteLine(OutputLevel.Verbose, statusMessage);
            else if (!Quiet)
            {
                if (statusMessage.StartsWith("Step", StringComparison.OrdinalIgnoreCase)
                    && statusMessage.Contains("Start"))
                {
                    int pos = statusMessage.IndexOf(" - ", StringComparison.OrdinalIgnoreCase);
                    Output.WriteLine(OutputLevel.Information, statusMessage.Substring(0, pos));
                }
            }
        }
        #endregion
    }
}
