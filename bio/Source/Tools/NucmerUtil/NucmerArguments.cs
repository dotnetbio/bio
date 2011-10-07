using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.SuffixTree;
using Bio.IO.FastA;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;
using Bio.Util;

namespace NucmerUtil
{
    /// <summary>
    /// Command line arguments for NUCmer.
    /// </summary>
    internal class NucmerArguments
    {
        #region Public Fields

        /// <summary>
        /// Paths of reference and query file.
        /// </summary>
        public string[] FilePath = null;

        /// <summary>
        /// Use anchor matches that are unique in both the reference and query.
        /// </summary>
        public bool Mum = false;

        /// <summary>
        /// Use anchor matches that are unique in the reference but not necessarily unique in the query (default behavior).
        /// </summary>
        public bool MumReference = true;

        /// <summary>
        /// Use all anchor matches regardless of their uniqueness.
        /// </summary>
        public bool MaxMatch = false;

        /// <summary>
        /// Distance an alignment extension will attempt to extend poor scoring regions before giving up (default 200).
        /// </summary>
        public int BreakLength = 200;

        /// <summary>
        /// Minimum cluster length (default 65).
        /// </summary>
        public int MinCluster = 65;

        /// <summary>
        /// Maximum diagonal difference factor for clustering, i.e. diagonal difference / match separation (default 0.12).
        /// </summary>
        public double DiagFactor = 0.12;

        /// <summary>
        /// Align only the forward strands of each sequence.
        /// </summary>
        public bool ForwardAndReverse = false;

        /// <summary>
        /// Maximum gap between two adjacent matches in a cluster (default 90).
        /// </summary>
        public int MaxGap = 90;

        /// <summary>
        /// Print the help information.
        /// </summary>
        public bool Help = false;

        /// <summary>
        /// Minimum length of an maximal exact match (default 20).
        /// </summary>
        public int MinMatch = 20;

        /// <summary>
        /// Output file.
        /// </summary>
        public string OutputFile = null;

        /// <summary>
        /// Align only the reverse strand of the query sequence to the forward strand of the reference.
        /// </summary>
        public bool Reverse = false;

        /// <summary>
        /// Toggle the outward extension of alignments from their anchoring clusters. Setting --noextend will 
        /// prevent alignment extensions but still align the DNA between clustered matches and create the .delta file 
        /// (default --extend).
        /// </summary>
        public bool NotExtend = false;

        /// <summary>
        /// Display verbose logging during processing.
        /// </summary>
        public bool Verbose = false;

        /// <summary>
        ///  Gets or sets maximum fixed diagonal difference
        /// </summary>
        public int FixedSeparation = 5;
        #endregion

        #region Private Fields

        /// <summary>
        /// Time taken to get reverse compliment.
        /// </summary>
        private static TimeSpan timeTakenToGetReverseComplement = new TimeSpan();

        /// <summary>
        /// Time taken to parse query sequences.
        /// </summary>
        private static TimeSpan timeTakenToParseQuerySequences = new TimeSpan();

        /// <summary>
        /// Get the total number of delta for validation purpose
        /// </summary>
        private long toltalDeltas;

        /// <summary>
        /// Count the number of processed queries.
        /// </summary>
        long queryCount = 0;

        #endregion

        #region Public Method

        /// <summary>
        /// Align the sequences.
        /// </summary>
        public void Align()
        {
            TimeSpan nucmerSpan = new TimeSpan();
            Stopwatch runNucmer = new Stopwatch();
            FileInfo refFileinfo = new FileInfo(this.FilePath[0]);
            long refFileLength = refFileinfo.Length;
            refFileinfo = null;

            runNucmer.Restart();
            IEnumerable<ISequence> referenceSequence = Parse(this.FilePath[0]);
            runNucmer.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed Reference FastA file: {0}", Path.GetFullPath(this.FilePath[0]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runNucmer.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", refFileLength);
            }

            refFileinfo = new FileInfo(this.FilePath[1]);
            refFileLength = refFileinfo.Length;

            runNucmer.Restart();
            IEnumerable<ISequence> querySequence = ParseWithPosition(this.FilePath[1]);
            runNucmer.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed Query FastA file: {0}", Path.GetFullPath(this.FilePath[1]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runNucmer.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", refFileLength);
            }

            if (!this.NotExtend)
            {
                runNucmer.Restart();
                IEnumerable<IEnumerable<DeltaAlignment>> delta = this.GetDelta(referenceSequence, querySequence);
                runNucmer.Stop();
                nucmerSpan = nucmerSpan.Add(runNucmer.Elapsed);

                if (!string.IsNullOrEmpty(this.OutputFile))
                {
                    FileInfo file = new FileInfo(this.OutputFile);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                runNucmer.Restart();
                this.WriteDelta(delta);
                runNucmer.Stop();
            }
            else
            {
                runNucmer.Restart();
                IList<List<IList<Cluster>>> clusters = this.GetCluster(referenceSequence, querySequence);
                runNucmer.Stop();
                nucmerSpan = nucmerSpan.Add(runNucmer.Elapsed);

                runNucmer.Restart();
                this.WriteCluster(clusters, referenceSequence, querySequence);
                runNucmer.Stop();
            }

            if (this.Verbose)
            {
                if (this.Reverse || this.ForwardAndReverse)
                {
                    Console.Error.WriteLine("            Read/Processing time: {0}", timeTakenToParseQuerySequences);
                    Console.Error.WriteLine("         Reverse Complement time: {0}", timeTakenToGetReverseComplement);
                }

                Console.Error.WriteLine("  Compute time: {0}", nucmerSpan);
                Console.Error.WriteLine("  Write() time: {0}", runNucmer.Elapsed);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Given a list of sequences, create a new list with only the Reverse Complements
        /// of the original sequences.
        /// </summary>
        /// <param name="sequenceList">List of sequence.</param>
        /// <returns>Returns the list of sequence.</returns>
        private static IEnumerable<ISequence> ReverseComplementSequenceList(IEnumerable<ISequence> sequenceList)
        {
            Stopwatch stopWatchInterval = new Stopwatch();
            stopWatchInterval.Restart();
            foreach (ISequence seq in sequenceList)
            {
                stopWatchInterval.Stop();

                // Add the query sequence parse time.
                timeTakenToParseQuerySequences = timeTakenToParseQuerySequences.Add(stopWatchInterval.Elapsed);

                stopWatchInterval.Restart();
                ISequence seqReverseComplement = seq.GetReverseComplementedSequence();
                stopWatchInterval.Stop();

                // Add the reverse complement time.
                timeTakenToGetReverseComplement = timeTakenToGetReverseComplement.Add(stopWatchInterval.Elapsed);

                if (seqReverseComplement != null)
                {
                    seqReverseComplement.ID = seqReverseComplement.ID + " Reverse";
                }

                yield return seqReverseComplement;
            }

            // Stop watch if there are not query sequences left.
            stopWatchInterval.Stop();
        }

        /// <summary>
        /// Given a list of sequences, create a new list with the orginal sequence followed
        /// by the Reverse Complement of that sequence.
        /// </summary>
        /// <param name="sequenceList">List of sequence.</param>
        /// <returns>Returns the List of sequence.</returns>
        private static IEnumerable<ISequence> AddReverseComplementsToSequenceList(IEnumerable<ISequence> sequenceList)
        {
            Stopwatch stopWatchInterval = new Stopwatch();
            stopWatchInterval.Restart();
            foreach (ISequence seq in sequenceList)
            {
                stopWatchInterval.Stop();

                // Add the query sequence parse time.
                timeTakenToParseQuerySequences = timeTakenToParseQuerySequences.Add(stopWatchInterval.Elapsed);

                stopWatchInterval.Restart();
                ISequence seqReverseComplement = seq.GetReverseComplementedSequence();

                stopWatchInterval.Stop();

                // Add the reverse complement time.
                timeTakenToGetReverseComplement = timeTakenToGetReverseComplement.Add(stopWatchInterval.Elapsed);
                if (seqReverseComplement != null)
                {
                    seqReverseComplement.ID = seqReverseComplement.ID + " Reverse";
                }

                yield return seq;
                yield return seqReverseComplement;
            }

            // Stop watch if there are not query sequences left.
            stopWatchInterval.Stop();
        }

        /// <summary>
        /// Returns the cluster.
        /// </summary>
        /// <param name="referenceSequence">The Reference sequence.</param>
        /// <param name="querySequence">The Query sequence.</param>
        /// <returns>Returns list of clusters.</returns>
        private IList<List<IList<Cluster>>> GetCluster(IEnumerable<ISequence> referenceSequence, IEnumerable<ISequence> querySequence)
        {
            IList<List<IList<Cluster>>> clusters = new List<List<IList<Cluster>>>();
            List<IList<Cluster>> clusters1 = new List<IList<Cluster>>();

            if (this.MaxMatch)
            {
                Parallel.ForEach(referenceSequence, sequence =>
                {
                    NUCmer nucmer = new NUCmer((Sequence)sequence);
                    nucmer.FixedSeparation = FixedSeparation;
                    nucmer.BreakLength = BreakLength;
                    nucmer.LengthOfMUM = MinMatch;
                    nucmer.MaximumSeparation = MaxGap;
                    nucmer.MinimumScore = MinCluster;
                    nucmer.SeparationFactor = (float)DiagFactor;

                    foreach (ISequence qrySequence in querySequence)
                    {
                        clusters1.Add(nucmer.GetClusters(qrySequence, false));
                    }
                });

                clusters.Add(clusters1);
            }
            else
            {
                Parallel.ForEach(referenceSequence, sequence =>
                {
                    NUCmer nucmer = new NUCmer((Sequence)sequence);
                    nucmer.FixedSeparation = FixedSeparation;
                    nucmer.BreakLength = BreakLength;
                    nucmer.LengthOfMUM = MinMatch;
                    nucmer.MaximumSeparation = MaxGap;
                    nucmer.MinimumScore = MinCluster;
                    nucmer.SeparationFactor = (float)DiagFactor;

                    foreach (ISequence qrySequence in querySequence)
                    {
                        clusters1.Add(nucmer.GetClusters(qrySequence));
                    }
                });

                clusters.Add(clusters1);
            }

            return clusters;
        }

        /// <summary>
        /// Gets the modified sequence.
        /// </summary>
        /// <param name="querySequence">The query sequence.</param>
        /// <returns>Returns a list of sequence.</returns>
        private IEnumerable<ISequence> GetSequence(IEnumerable<ISequence> querySequence)
        {
            if (this.Reverse)
            {
                return ReverseComplementSequenceList(querySequence);
            }
            else if (this.ForwardAndReverse)
            {
                return AddReverseComplementsToSequenceList(querySequence);
            }
            else
            {
                return querySequence;
            }
        }

        /// <summary>
        /// Gets the Delta for list of query sequences.
        /// </summary>
        /// <param name="referenceSequence">The reference sequence.</param>
        /// <param name="querySequence">The query sequence.</param>
        /// <returns>Returns list of IEnumerable Delta Alignment.</returns>
        private IEnumerable<IEnumerable<DeltaAlignment>> GetDelta(IEnumerable<ISequence> referenceSequence, IEnumerable<ISequence> querySequence)
        {
            if (this.MaxMatch)
            {
                foreach (ISequence refSeq in referenceSequence)
                {
                    NUCmer nucmer = new NUCmer((Sequence)refSeq);
                    nucmer.FixedSeparation = FixedSeparation;
                    nucmer.BreakLength = BreakLength;
                    nucmer.LengthOfMUM = MinMatch;
                    nucmer.MaximumSeparation = MaxGap;
                    nucmer.MinimumScore = MinCluster;
                    nucmer.SeparationFactor = (float)DiagFactor;

                    foreach (ISequence qrySequence in querySequence)
                    {
                        queryCount++;
                        yield return nucmer.GetDeltaAlignments(qrySequence, false);
                    }
                }

            }
            else
            {
                foreach (ISequence refSeq in referenceSequence)
                {
                    NUCmer nucmer = new NUCmer((Sequence)refSeq);
                    nucmer.FixedSeparation = FixedSeparation;
                    nucmer.BreakLength = BreakLength;
                    nucmer.LengthOfMUM = MinMatch;
                    nucmer.MaximumSeparation = MaxGap;
                    nucmer.MinimumScore = MinCluster;
                    nucmer.SeparationFactor = (float)DiagFactor;

                    foreach (ISequence qrySequence in querySequence)
                    {
                        queryCount++;
                        yield return nucmer.GetDeltaAlignments(qrySequence);
                    }
                }

            }

        }

        /// <summary>
        /// Parses the file.
        /// </summary>
        /// <param name="fileName">The FileName.</param>
        /// <returns>List of sequence.</returns>
        private static IEnumerable<ISequence> Parse(string fileName)
        {
            FastAParser parser = new FastAParser(fileName);
            return parser.Parse();
        }

        /// <summary>
        /// Parses the file.
        /// </summary>
        /// <param name="fileName">The FileName.</param>
        /// <returns>List of sequence.</returns>
        private static IEnumerable<ISequence> ParseWithPosition(string fileName)
        {
            FastASequencePositionParser parser = new FastASequencePositionParser(fileName);
            return parser.Parse();
        }

        /// <summary>
        /// Writes cluster for query sequences.
        /// </summary>
        /// <param name="clusters">The Clusters.</param>
        /// <param name="referenceSequence">The reference sequence.</param>
        /// <param name="querySequence">The query sequence.</param>
        private void WriteCluster(
            IList<List<IList<Cluster>>> clusters,
            IEnumerable<ISequence> referenceSequence,
            IEnumerable<ISequence> querySequence)
        {
            TextWriter textWriterConsoleOutSave = Console.Out;
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                FileStream fileStreamConsoleOut = new FileStream(this.OutputFile, FileMode.Create);
                StreamWriter streamWriterConsoleOut = new StreamWriter(fileStreamConsoleOut);
                Console.SetOut(streamWriterConsoleOut);
                streamWriterConsoleOut.AutoFlush = true;
            }

            int refIndex = 0;
            foreach (ISequence refSeq in referenceSequence)
            {
                List<IList<Cluster>> queryCluster = clusters[refIndex++];
                int queryIndex = 0;
                foreach (ISequence querySeq in querySequence)
                {
                    IEnumerable<Cluster> cluster = queryCluster[queryIndex++];
                    if (cluster != null)
                    {
                        Console.WriteLine(refSeq.ID + " " + querySeq.ID);
                        foreach (Cluster c in cluster)
                        {
                            foreach (MatchExtension ext in c.Matches)
                            {
                                Console.WriteLine(ext.ReferenceSequenceOffset + " " + ext.QuerySequenceOffset + " " + ext.Length);
                            }
                        }
                    }
                }
            }

            Console.SetOut(textWriterConsoleOutSave);
        }

         /// <summary>
        /// Writes delta for query sequences.
        /// </summary>
        /// <param name="delta">The Deltas.</param>
        private void WriteDelta(IEnumerable<IEnumerable<DeltaAlignment>> delta)
        {
            TextWriter textWriterConsoleOutSave = Console.Out;
            StreamWriter streamWriterConsoleOut = null;
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                streamWriterConsoleOut = new StreamWriter(this.OutputFile);
                Console.SetOut(streamWriterConsoleOut);
            }

            long sequenceCount = 0;
            long deltaPositionInFile = 0;


            foreach (IEnumerable<DeltaAlignment> align in delta)
            {
                sequenceCount++;
                //Note: Repeate Resolution step of comparative assembly needs all delta 
                //      alignemnts belogs to a query sequence should be consecutively placed.
                foreach (DeltaAlignment deltaAlignment in align)
                {
                    deltaAlignment.Id = deltaPositionInFile;
                    string deltaString =  Helper.GetString(deltaAlignment);
                    deltaPositionInFile += deltaString.Length;
                    Console.Write(deltaString);
                    toltalDeltas++;
                }

                Console.Out.Flush();

                if (sequenceCount % 1000 == 0)
                {
                    Console.Error.WriteLine("Processed {0} sequences - {1}", sequenceCount, DateTime.Now);
                }
            }

            Console.Error.WriteLine("Processed {0} sequences - {1}", sequenceCount, DateTime.Now);

            Console.SetOut(textWriterConsoleOutSave);
            Console.WriteLine(string.Format("\tTotal Number of Delta(s) = {0} by processing {1} query sequence(s)", toltalDeltas, queryCount));
        }
        #endregion
    }
}
