using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Extensions;
using Bio.IO.FastA;

namespace ScaffoldUtil
{
    /// <summary>
    /// Command line arguments for ScaffoldGeneration.
    /// </summary>
    internal class ScaffoldArguments
    {
        #region Public Fields
        /// <summary>
        /// Paths of input and output file.
        /// </summary>
        public string[] FilePath = null;

        /// <summary>
        /// Print the help information.
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// Output file.
        /// </summary>
        public string OutputFile = null;

        /// <summary>
        /// Display verbose logging during processing.
        /// </summary>
        public bool Verbose = false;

        /// <summary>
        /// Length of k-mer.
        /// </summary>
        public int KmerLength = 10;

        /// <summary>
        /// Number of paired read required to connect two contigs.
        /// </summary>
        public int Redundancy = 2;

        /// <summary>
        /// Depth for graph traversal.
        /// </summary>
        public int Depth = 10;

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
        #endregion

        #region Public Method

        /// <summary>
        /// Refine layout in the delta alignments.
        /// </summary>
        public void GenerateScaffolds()
        {
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
            FastAParser parser = new FastAParser();
            IEnumerable<ISequence> contigs = parser.Parse(this.FilePath[0]);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed Contig file: {0}", Path.GetFullPath(this.FilePath[0]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runAlgorithm.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", inputFileLength);
            }

            inputFileinfo = new FileInfo(this.FilePath[1]);
            inputFileLength = inputFileinfo.Length;

            runAlgorithm.Restart();
            FastAParser readParser = new FastAParser();
            IEnumerable<ISequence> reads = readParser.Parse(this.FilePath[1]);
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed reads file: {0}", Path.GetFullPath(this.FilePath[1]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runAlgorithm.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", inputFileLength);
            }

            runAlgorithm.Restart();
            IEnumerable<ISequence> scaffolds = null;
            using (GraphScaffoldBuilder scaffoldBuilder = new GraphScaffoldBuilder())
            {
                 scaffolds = scaffoldBuilder.BuildScaffold(reads, contigs.ToList(), this.KmerLength, this.Depth, this.Redundancy);
            } 
            runAlgorithm.Stop();
            timeSpan = timeSpan.Add(runAlgorithm.Elapsed);

            runAlgorithm.Restart();
            this.WriteSequences(scaffolds);
            runAlgorithm.Stop();


            if (this.Verbose)
            {
                Console.Error.WriteLine("  Compute time: {0}", timeSpan);
                Console.Error.WriteLine("  Write() time: {0}", runAlgorithm.Elapsed);
            }
        }

        #endregion

        #region Private Methods
        private void WriteSequences(IEnumerable<ISequence> sequences)
        {
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                new FastAFormatter().Format(sequences, this.OutputFile);
            }
            else
            {
                foreach (ISequence sequence in sequences)
                {
                    Console.WriteLine(sequence.ConvertToString());
                }
            }
        }
        #endregion
    }
}
