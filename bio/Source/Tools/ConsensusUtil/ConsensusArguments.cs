using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Assembly.Comparative;
using Bio.IO.FastA;
using Bio.Util;

namespace ConsensusUtil
{
    /// <summary>
    /// Command line arguments for ConsensusGeneration.
    /// </summary>
    internal class ConsensusArguments
    {
        #region Public Fields

        /// <summary>
        /// Paths of deltaalignment and query file.
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
        
        #endregion

        #region Public Method

        /// <summary>
        /// Refine layout in the delta alignments.
        /// </summary>
        public void GenerateConsensus()
        {
            TimeSpan timeSpan = new TimeSpan();
            Stopwatch runAlgorithm = new Stopwatch();
            
            runAlgorithm.Restart();
            FileInfo inputFileinfo = new FileInfo(this.FilePath[1]);
            long inputFileLength = inputFileinfo.Length;
            inputFileinfo = null;
            runAlgorithm.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed Query FastA file: {0}", Path.GetFullPath(this.FilePath[1]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runAlgorithm.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", inputFileLength);
            }

            inputFileinfo = new FileInfo(this.FilePath[0]);
            inputFileLength = inputFileinfo.Length;
            inputFileinfo = null;
            runAlgorithm.Restart();

            using (DeltaAlignmentCollection deltaCollection = new DeltaAlignmentCollection(this.FilePath[0], this.FilePath[1]))
            {
                runAlgorithm.Stop();

                if (this.Verbose)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("  Processed DeltaAlignment file: {0}", Path.GetFullPath(this.FilePath[0]));
                    Console.Error.WriteLine("            Read/Processing time: {0}", runAlgorithm.Elapsed);
                    Console.Error.WriteLine("            File Size           : {0}", inputFileLength);
                }

                runAlgorithm.Restart();
                IEnumerable<ISequence> consensus = ConsensusGeneration.GenerateConsensus(deltaCollection);
                runAlgorithm.Stop();
                timeSpan = timeSpan.Add(runAlgorithm.Elapsed);

                runAlgorithm.Restart();
                this.WriteSequences(consensus);
                runAlgorithm.Stop();
            }

            if (this.Verbose)
            {
                Console.Error.WriteLine("  Compute time: {0}", timeSpan);
                Console.Error.WriteLine("  Write() time: {0}", runAlgorithm.Elapsed);
            }
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Write sequences to the file
        /// </summary>
        /// <param name="sequences"></param>
        private void WriteSequences(IEnumerable<ISequence> sequences)
        {
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                using (FastAFormatter ff = new FastAFormatter(this.OutputFile))
                {
                    foreach (ISequence sequence in sequences)
                    {
                        ff.Write(sequence);
                    }
                }
            }
            else
            {
                foreach (ISequence sequence in sequences)
                {
                    Console.WriteLine(new string(sequence.Select(a => (char)a).ToArray()));
                }
            }
        }
        #endregion
    }
}
