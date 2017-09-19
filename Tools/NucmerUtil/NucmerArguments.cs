using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bio;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.SuffixTree;
using Bio.Extensions;
using Bio.IO;
using Bio.IO.FastA;
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
        public bool Forward = false;

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
        /// Get the total number of delta for validation purpose
        /// </summary>
        private long _toltalDeltas;

        /// <summary>
        /// Count the number of processed queries.
        /// </summary>
        long _queryCount;

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

            runNucmer.Restart();
            IEnumerable<ISequence> referenceSequence = Parse(this.FilePath[0]);
            runNucmer.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("  Processed Reference FastA file: {0}", Path.GetFullPath(this.FilePath[0]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runNucmer.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", refFileLength);
                Console.Error.WriteLine();
            }

            refFileinfo = new FileInfo(this.FilePath[1]);
            refFileLength = refFileinfo.Length;

            runNucmer.Restart();
            IEnumerable<ISequence> querySequence = ParseWithPosition(this.FilePath[1]);
            runNucmer.Stop();

            if (this.Verbose)
            {
                Console.Error.WriteLine("  Processed Query FastA file: {0}", Path.GetFullPath(this.FilePath[1]));
                Console.Error.WriteLine("            Read/Processing time: {0}", runNucmer.Elapsed);
                Console.Error.WriteLine("            File Size           : {0}", refFileLength);
                if (!string.IsNullOrEmpty(this.OutputFile))
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("  Writing output to             : {0}", Path.GetFullPath(this.OutputFile));
                }
                Console.Error.WriteLine();

                if (Forward)
                    Console.Error.WriteLine("  Including forward strand of query sequence only.");
                else if (Reverse)
                    Console.Error.WriteLine("  Including reverse strand of query sequence only.");
                else
                    Console.Error.WriteLine("  Including both forward and reverse strands of query sequence.");
            }

            // Remove any existing output file - we will overwrite it.
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                if (File.Exists(this.OutputFile))
                    File.Delete(this.OutputFile);
            }

            // Align the DNA between clustered matches and create a delta.
            if (this.NotExtend)
            {
                runNucmer.Restart();
                IList<List<IList<Cluster>>> clusters = this.GetCluster(referenceSequence, querySequence);
                runNucmer.Stop();
                nucmerSpan = nucmerSpan.Add(runNucmer.Elapsed);

                runNucmer.Restart();
                this.WriteCluster(clusters, referenceSequence, querySequence);
                runNucmer.Stop();
            }
            // Normal operation.
            else
            {
                runNucmer.Restart();
                IEnumerable<IEnumerable<DeltaAlignment>> delta = this.GetDelta(referenceSequence, querySequence);
                runNucmer.Stop();
                nucmerSpan = nucmerSpan.Add(runNucmer.Elapsed);

                runNucmer.Restart();
                this.WriteDelta(delta);
                runNucmer.Stop();
            }

            if (this.Verbose)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("                Compute time: {0}", nucmerSpan);
                Console.Error.WriteLine("                  Write time: {0}", runNucmer.Elapsed);
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
            foreach (ISequence rcSequence in sequenceList.Select(seq => seq.GetReverseComplementedSequence()))
            {
                if (rcSequence != null)
                {
                    rcSequence.MarkAsReverseComplement();
                    yield return rcSequence;
                }
            }
        }

        /// <summary>
        /// Given a list of sequences, create a new list with the original sequence followed
        /// by the Reverse Complement of that sequence.
        /// </summary>
        /// <param name="sequenceList">List of sequence.</param>
        /// <returns>Returns the List of sequence.</returns>
        private static IEnumerable<ISequence> AddReverseComplementsToSequenceList(IEnumerable<ISequence> sequenceList)
        {
            foreach (ISequence seq in sequenceList)
            {
                yield return seq;

                ISequence rcSequence = seq.GetReverseComplementedSequence();
                if (rcSequence != null)
                {
                    rcSequence.MarkAsReverseComplement();
                    yield return rcSequence;
                }
            }
        }

        /// <summary>
        /// Returns the cluster.
        /// </summary>
        /// <param name="referenceSequence">The Reference sequences.</param>
        /// <param name="originalQuerySequences">The Query sequences.</param>
        /// <returns>Returns list of clusters.</returns>
        private IList<List<IList<Cluster>>> GetCluster(IEnumerable<ISequence> referenceSequence, IEnumerable<ISequence> originalQuerySequences)
        {
            var clusters = new List<List<IList<Cluster>>>();
            var clusters1 = new List<IList<Cluster>>();

            IEnumerable<ISequence> querySequences = 
                Forward ? originalQuerySequences
                        : (Reverse
                            ? ReverseComplementSequenceList(originalQuerySequences)
                            : AddReverseComplementsToSequenceList(originalQuerySequences));

            _queryCount += querySequences.Count();

            foreach (var sequence in referenceSequence)
            {
                NUCmer nucmer = new NUCmer(sequence)
                {
                    FixedSeparation = FixedSeparation,
                    BreakLength = BreakLength,
                    LengthOfMUM = MinMatch,
                    MaximumSeparation = MaxGap,
                    MinimumScore = MinCluster,
                    SeparationFactor = (float) DiagFactor
                };

                clusters1.AddRange(querySequences.Select(qs => nucmer.GetClusters(qs, !MaxMatch, qs.IsMarkedAsReverseComplement())));
            }

            clusters.Add(clusters1);

            return clusters;
        }

        /// <summary>
        /// Gets the Delta for list of query sequences.
        /// </summary>
        /// <param name="referenceSequence">The reference sequence.</param>
        /// <param name="originalQuerySequences">The query sequence.</param>
        /// <returns>Returns list of IEnumerable Delta Alignment.</returns>
        private IEnumerable<IEnumerable<DeltaAlignment>> GetDelta(IEnumerable<ISequence> referenceSequence, IEnumerable<ISequence> originalQuerySequences)
        {
            IEnumerable<ISequence> querySequences =
                Forward ? originalQuerySequences
                : (Reverse
                    ? ReverseComplementSequenceList(originalQuerySequences)
                    : AddReverseComplementsToSequenceList(originalQuerySequences));

            foreach (ISequence refSeq in referenceSequence)
            {
                NUCmer nucmer = new NUCmer(refSeq) 
                {
                    FixedSeparation = FixedSeparation,
                    BreakLength = BreakLength,
                    LengthOfMUM = MinMatch,
                    MaximumSeparation = MaxGap,
                    MinimumScore = MinCluster,
                    SeparationFactor = (float) DiagFactor
                };

                foreach (ISequence qs in querySequences)
                {
                    _queryCount++;
                    yield return nucmer.GetDeltaAlignments(qs, !MaxMatch, qs.IsMarkedAsReverseComplement());
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
            if (Helper.IsZippedFasta(fileName))
            {
                var parser = new GZipFastAParser();
                return parser.Parse(fileName);
            }
            else
            {
                var parser = new FastAParser();
                return parser.Parse(fileName);
            }
        }

        /// <summary>
        /// Parses the file.
        /// </summary>
        /// <param name="fileName">The FileName.</param>
        /// <returns>List of sequence.</returns>
        private static IEnumerable<ISequence> ParseWithPosition(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                FastASequencePositionParser parser = new FastASequencePositionParser(stream);
                return parser.Parse().ToList();
            }
        }

        /// <summary>
        /// Writes cluster for query sequences.
        /// </summary>
        /// <param name="clusters">The Clusters.</param>
        /// <param name="referenceSequence">The reference sequence.</param>
        /// <param name="querySequence">The query sequence.</param>
        private void WriteCluster(IList<List<IList<Cluster>>> clusters, 
            IEnumerable<ISequence> referenceSequence, IEnumerable<ISequence> querySequence)
        {
            TextWriter textWriterConsoleOutSave = Console.Out;
            if (!string.IsNullOrEmpty(this.OutputFile))
            {
                FileStream fileStreamConsoleOut = new FileStream(this.OutputFile, FileMode.Create);
                Console.SetOut(new StreamWriter(fileStreamConsoleOut) { AutoFlush = true });
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
             if (!string.IsNullOrEmpty(this.OutputFile))
            {
                Console.SetOut(new StreamWriter(this.OutputFile));
            }

            long sequenceCount = 0;
            long deltaPositionInFile = 0;

            foreach (IEnumerable<DeltaAlignment> align in delta)
            {
                sequenceCount++;
                // Note: Repeat Resolution step of comparative assembly needs all delta 
                //       alignments to belong to a query sequence should be consecutively placed.
                foreach (DeltaAlignment deltaAlignment in align)
                {
                    deltaAlignment.Id = deltaPositionInFile;
                    string deltaString =  Helper.GetString(deltaAlignment);
                    deltaPositionInFile += deltaString.Length;
                    Console.Write(deltaString);
                    _toltalDeltas++;
                }

                Console.Out.Flush();

                if (sequenceCount % 1000 == 0)
                {
                    Console.Error.WriteLine("  Processed {0} sequences - {1}", sequenceCount, DateTime.Now);
                }
            }

            Console.Error.WriteLine("  Processed {0} sequences - {1}", sequenceCount, DateTime.Now);
            Console.Error.WriteLine("  Total Number of Delta(s) = {0} by processing {1} query sequence(s)", _toltalDeltas, _queryCount);

            Console.SetOut(textWriterConsoleOutSave);
        }
        #endregion
    }
}
