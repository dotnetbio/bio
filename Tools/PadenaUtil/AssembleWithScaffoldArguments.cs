using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Bio;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Padena;
using PadenaUtil.Properties;

namespace PadenaUtil
{
    /// <summary>
    /// This class defines AssembleWithScaffold options.
    /// </summary>
    internal class AssembleWithScaffoldArguments : AssembleArguments
    {
        #region Public Fields

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
        /// Number of paired read required to connect two contigs.
        /// </summary>
        public int Redundancy = 2;

        /// <summary>
        /// Depth for graph traversal.
        /// </summary>
        public int Depth = 10;

        #endregion

        #region Public methods

        /// <summary>
        /// It assembles the sequences.
        /// </summary>
        public override void AssembleSequences()
        {
            TimeSpan algorithmSpan = new TimeSpan();
            Stopwatch runAlgorithm = new Stopwatch();
            FileInfo refFileinfo = new FileInfo(this.Filename);
            long refFileLength = refFileinfo.Length;

            Output.WriteLine(OutputLevel.Information, Resources.AssemblyScaffoldStarting);

            if (!string.IsNullOrEmpty(this.CloneLibraryName))
            {
                CloneLibrary.Instance.AddLibrary(this.CloneLibraryName, (float)this.MeanLengthOfInsert, (float)this.StandardDeviationOfInsert);
            }

            runAlgorithm.Restart();
            IEnumerable<ISequence> reads = ParseFile();
            runAlgorithm.Stop();
            algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);

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

            ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler();
            assembler.StatusChanged += this.AssemblerStatusChanged;
            assembler.AllowErosion = AllowErosion;
            assembler.AllowKmerLengthEstimation = AllowKmerLengthEstimation;
            if (ContigCoverageThreshold != -1)
            {
                assembler.AllowLowCoverageContigRemoval = true;
                assembler.ContigCoverageThreshold = ContigCoverageThreshold;
            }
            assembler.DanglingLinksThreshold = DangleThreshold;
            assembler.ErosionThreshold = ErosionThreshold;
            if (!this.AllowKmerLengthEstimation)
            {
                assembler.KmerLength = this.KmerLength;
            }

            assembler.RedundantPathLengthThreshold = RedundantPathLengthThreshold;
            runAlgorithm.Restart();
            IDeNovoAssembly assembly = assembler.Assemble(reads, true);
            runAlgorithm.Stop();
            algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);
            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Compute time: {0}", runAlgorithm.Elapsed);
            }

            runAlgorithm.Restart();
            WriteContigs(assembly);
            runAlgorithm.Stop();
            algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);
            if (this.Verbose)
            {
                Output.WriteLine(OutputLevel.Verbose);
                Output.WriteLine(OutputLevel.Verbose, "Write contigs time: {0}", runAlgorithm.Elapsed);
                Output.WriteLine(OutputLevel.Verbose, "Total runtime: {0}", algorithmSpan);
            }
        }

        #endregion
    }
}
