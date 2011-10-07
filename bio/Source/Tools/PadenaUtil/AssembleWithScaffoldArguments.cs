using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Bio;
using Bio.Algorithms.Assembly;
using Bio.Algorithms.Assembly.Padena;
using Bio.Util;

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
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed read file: {0}", Path.GetFullPath(this.Filename));
                Console.Error.WriteLine("            Read/Processing time: {0}", runAlgorithm.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", refFileLength);
            }

            runAlgorithm.Restart();
            ValidateAmbiguousReads(reads);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Time taken for Validating reads: {0}", runAlgorithm.Elapsed);
            }

            ParallelDeNovoAssembler assembler = new ParallelDeNovoAssembler();
            assembler.StatusChanged += new EventHandler<StatusChangedEventArgs>(this.AssemblerStatusChanged);
            assembler.AllowErosion = AllowErosion;
            assembler.AllowKmerLengthEstimation = AllowKmerLengthEstimation;
            assembler.AllowLowCoverageContigRemoval = LowCoverageContigRemovalEnabled;
            assembler.ContigCoverageThreshold = ContigCoverageThreshold;
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
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Compute time: {0}", runAlgorithm.Elapsed);
            }

            runAlgorithm.Restart();
            WriteContigs(assembly);
            runAlgorithm.Stop();
            algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);
            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Write time: {0}", runAlgorithm.Elapsed);
                Console.Error.WriteLine("  Total runtime: {0}", algorithmSpan);
            }
        }

        #endregion
    }
}
