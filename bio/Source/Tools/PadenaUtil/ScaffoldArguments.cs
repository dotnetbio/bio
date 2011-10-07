using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.IO.FastA;

namespace PadenaUtil
{
    /// <summary>
    /// This class defines Scaffolding Option.
    /// </summary>
    internal class ScaffoldArguments
    {
        #region Public Fields

        /// <summary>
        /// Length of k-mer.
        /// </summary>
        public int KmerLength = -1;

        /// <summary>
        /// Help.
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// Input file of reads and contigs.
        /// </summary>
        public string[] FileNames = null;

        /// <summary>
        /// Output file.
        /// </summary>
        public string OutputFile = string.Empty;

        /// <summary>
        /// Clone Library Name
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

        /// <summary>
        /// Display verbose logging during processing.
        /// </summary>
        public bool Verbose = false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates the Scaffold.
        /// </summary>
        public void GenerateScaffold()
        {
            if (!string.IsNullOrEmpty(this.CloneLibraryName))
            {
                CloneLibrary.Instance.AddLibrary(this.CloneLibraryName, (float)this.MeanLengthOfInsert, (float)this.StandardDeviationOfInsert);
            }

            if (this.FileNames.Length != 2)
            {
                Console.Error.WriteLine("\nError: A reference file and 1 query file are required.");
                Environment.Exit(-1);
            }

            TimeSpan algorithmSpan = new TimeSpan();
            Stopwatch runAlgorithm = new Stopwatch();
            FileInfo refFileinfo = null;

            using (GraphScaffoldBuilder scaffoldBuilder = new GraphScaffoldBuilder())
            {
                refFileinfo = new FileInfo(this.FileNames[0]);
                long refFileLength = refFileinfo.Length;

                runAlgorithm.Restart();
                IEnumerable<ISequence> contigs = this.ParseFile(this.FileNames[0]);
                runAlgorithm.Stop();
                algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);

                if (this.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("  Processed contigs file: {0}", Path.GetFullPath(this.FileNames[0]));
                    Console.Error.WriteLine("            Read/Processing time: {0}", runAlgorithm.Elapsed);
                    Console.Error.WriteLine("            File Size           : {0}", refFileLength);
                }

                refFileinfo = new FileInfo(this.FileNames[1]);
                refFileLength = refFileinfo.Length;

                runAlgorithm.Restart();
                IEnumerable<ISequence> reads = this.ParseFile(this.FileNames[1]);
                runAlgorithm.Stop();
                algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);

                if (this.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("  Processed reads file: {0}", Path.GetFullPath(this.FileNames[1]));
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

                runAlgorithm.Restart();
                IEnumerable<ISequence> scaffolds = scaffoldBuilder.BuildScaffold(reads, contigs.ToList(), this.KmerLength, this.Depth, this.Redundancy);
                runAlgorithm.Stop();
                algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);
                if (this.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("  Compute time: {0}", runAlgorithm.Elapsed);
                }

                runAlgorithm.Restart();
                WriteContigs(scaffolds);
                runAlgorithm.Stop();
                algorithmSpan = algorithmSpan.Add(runAlgorithm.Elapsed);
                if (this.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("  Write time: {0}", runAlgorithm.Elapsed);
                    Console.Error.WriteLine("  Total runtime: {0}", algorithmSpan);
                }

            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// It writes Contigs to the file.
        /// </summary>
        /// <param name="scaffolds">The list of scaffolds sequence.</param>
        private void WriteContigs(IEnumerable<ISequence> scaffolds)
        {
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                using (FastAFormatter formatter = new FastAFormatter(this.OutputFile))
                {
                    formatter.AutoFlush = true;

                    foreach (ISequence seq in scaffolds)
                    {
                        formatter.Write(seq);
                    }
                }
            }
            else
            {
                foreach (ISequence seq in scaffolds)
                {
                    Console.WriteLine(seq.ID);
                    Console.WriteLine(new string(seq.Select(a => (char)a).ToArray()));
                }
            }
        }

        /// <summary>
        /// It parses the file.
        /// </summary>
        private IEnumerable<ISequence> ParseFile(string fileName)
        {
            // TODO: Add other parsers.
            FastAParser parser = new FastAParser(fileName);
            return parser.Parse();
        }

        /// <summary>
        /// Checks for ambiguous reads if any, if found ArgumentException.
        /// </summary>
        /// <param name="reads">Input reads.</param>
        private static void ValidateAmbiguousReads(IEnumerable<ISequence> reads)
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
