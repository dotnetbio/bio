using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bio.Algorithms.Assembly.Padena.Scaffold;
using Bio.Algorithms.Kmer;
using Bio.Util;

namespace Bio.Algorithms.Assembly.Padena.Utility
{
    /// <summary>
    /// Utility for aligning reads to contigs.
    /// </summary>
    public static class ReadAlignment
    {
        /// <summary>
        /// Aligns reads to contigs using kmer method of alignment.
        /// </summary>
        /// <param name="contigs">List of contig sequences.</param>
        /// <param name="reads">List of read sequences.</param>
        /// <param name="kmerLength">Kmer Length.</param>
        /// <returns>List of Contig.</returns>
        public static IList<Contig> ReadContigAlignment(IList<ISequence> contigs, IList<ISequence> reads, int kmerLength)
        {
            KmerIndexerDictionary map = SequenceToKmerBuilder.BuildKmerDictionary(reads, kmerLength);
            IList<ContigIndex> contigDatas;
            contigDatas = contigs.AsParallel().Select(contig =>
            {
                IEnumerable<ISequence> kmers = SequenceToKmerBuilder.GetKmerSequences(contig, kmerLength);
                ContigIndex index = new ContigIndex(contig);
                foreach (ISequence kmer in kmers)
                {
                    IList<KmerIndexer> positions;
                    if (map.TryGetValue(kmer, out positions) ||
                        map.TryGetValue(kmer.GetReverseComplementedSequence(), out positions))
                    {
                        index.ContigReadMatchIndexes.Add(positions);
                    }
                    else
                    {
                        index.ContigReadMatchIndexes.Add(new List<KmerIndexer>());
                    }
                }

                return index;
            }).ToList();
            
            return contigDatas.Select(contigData =>
            {
                IList<Task<IList<ReadMap>>> tasks =
                    new List<Task<IList<ReadMap>>>();

                // Stores information about contigs for which tasks has been generated.
                IList<long> visitedReads = new List<long>();

                // Creates Task for every read in nodes for a given contig.
                for (int index = 0; index < contigData.ContigReadMatchIndexes.Count; index++)
                {
                    int readPosition = index;
                    foreach (KmerIndexer kmer in contigData.ContigReadMatchIndexes[index])
                    {
                        long contigIndex = kmer.SequenceIndex;
                        if (!visitedReads.Contains(contigIndex))
                        {
                            visitedReads.Add(contigIndex);
                            tasks.Add(
                                Task<IList<ReadMap>>.Factory.StartNew(t => MapRead(readPosition, contigData.ContigReadMatchIndexes, contigIndex, kmerLength), TaskCreationOptions.AttachedToParent));
                        }
                    }
                }

                Contig contigOutputStructure = new Contig();
                contigOutputStructure.Consensus = contigData.ContigSequence;

                for (int index = 0; index < visitedReads.Count; index++)
                {
                    foreach (ReadMap maps in tasks[index].Result)
                    {
                        Contig.AssembledSequence assembledSeq = new Contig.AssembledSequence()
                        {
                            Length = maps.Length,
                            Position = maps.StartPositionOfContig,
                            ReadPosition = maps.StartPositionOfRead,
                            Sequence = reads.ElementAt(visitedReads[index])
                        };

                        if (new string(
                            contigOutputStructure.Consensus.GetSubSequence(
                            assembledSeq.Position, assembledSeq.Length).Select(a => (char)a).ToArray()).
                            Equals(new string(assembledSeq.Sequence.GetSubSequence(assembledSeq.ReadPosition, assembledSeq.Length)
                            .Select(a => (char)a).ToArray())))
                        {
                            assembledSeq.IsComplemented = false;
                            assembledSeq.IsReversed = false;
                        }
                        else
                        {
                            assembledSeq.IsComplemented = true;
                            assembledSeq.IsReversed = true;
                        }

                        contigOutputStructure.Sequences.Add(assembledSeq);
                    }
                }

                return contigOutputStructure;
            }).ToList();
        }

        #region Private methods

        /// <summary>
        /// Traverse through list of contig-read match indexes for given read.
        /// </summary>
        /// <param name="position">Position from where list of 
        ///  indexes should be traversed.</param>
        /// <param name="contigReadMatch">List for contig-read match indexes.</param>
        /// <param name="contigIndex">Index of contig sequence.</param>
        /// <param name="kmerLength">Length of kmer.</param>
        /// <returns>List of read maps.</returns>
        private static IList<ReadMap> MapRead(
            int position,
            IList<IList<KmerIndexer>> contigReadMatch,
            long contigIndex,
            int kmerLength)
        {
            IList<ReadMap> readMaps = new List<ReadMap>();
            for (int index = position; index < contigReadMatch.Count; index++)
            {
                foreach (KmerIndexer kmer in contigReadMatch[index])
                {
                    if (kmer.SequenceIndex == contigIndex)
                    {
                        FindContinuous(kmer, readMaps, index, kmerLength);
                    }
                }
            }

            return readMaps;
        }

        /// <summary>
        /// Merge continuous positions of a read in kmer indexes.
        /// </summary>
        /// <param name="kmer">Position of contig kmer.</param>
        /// <param name="readMaps">Alignment between read and contig.</param>
        /// <param name="position">Position of kmer in read.</param>
        /// <param name="kmerLength">Length of kmer.</param>
        private static void FindContinuous(
            KmerIndexer kmer,
            IList<ReadMap> readMaps,
            int position,
            int kmerLength)
        {
            // Create new object ReadInformation as read is encountered first time.
            if (readMaps.Count == 0)
            {
                foreach (int pos in kmer.Positions)
                {
                    ReadMap readMap = new ReadMap();
                    readMap.StartPositionOfContig = position;
                    readMap.StartPositionOfRead = pos;
                    readMap.Length = kmerLength;
                    readMaps.Add(readMap);
                }
            }
            else
            {
                // Merge current kmer node with previous kmer node of DeBruijn Graph, 
                // if they are continuous in either right or left traversal of graph.
                bool isMerged = false;
                foreach (int pos in kmer.Positions)
                {
                    foreach (ReadMap read in readMaps)
                    {
                        if (IsContinousRight(read, pos, position, kmerLength) ||
                            IsContinousLeft(read, pos, position, kmerLength))
                        {
                            read.Length++;
                            if (read.StartPositionOfRead > pos)
                            {
                                read.StartPositionOfRead = pos;
                            }

                            isMerged = true;
                            break;
                        }
                    }

                    // If not continuous a new object ReadMap is created to store new overlap.
                    if (isMerged == false)
                    {
                        ReadMap readmap = new ReadMap();
                        readmap.StartPositionOfContig = position;
                        readmap.StartPositionOfRead = pos;
                        readmap.Length = kmerLength;
                        readMaps.Add(readmap);
                    }
                }
            }
        }

        /// <summary>
        ///  Find if positions occur simultaneously of read in contig, 
        ///  if contig is traced from right direction.
        /// </summary>
        /// <param name="map">Map from previous position of read.</param>
        /// <param name="readPosition">Position of read.</param>
        /// <param name="contigPosition">Position of contig.</param>
        /// <param name="length">Length of kmer.</param>
        /// <returns>True if continuous position of reads in contig.</returns>
        private static bool IsContinousRight(
            ReadMap map,
            int readPosition,
            int contigPosition,
            int length)
        {
            return (map.Length - length + map.StartPositionOfContig + 1) == contigPosition &&
                    (map.StartPositionOfRead + map.Length - length + 1) == readPosition;
        }

        /// <summary>
        ///  Find if positions occur simultaneously of read in contig, 
        ///  if contig is traced from left direction.
        /// </summary>
        /// <param name="map">Map from previous position of read.</param>
        /// <param name="readPosition">Position of read.</param>
        /// <param name="contigPosition">Position of contig.</param>
        /// <param name="length">Length of kmer.</param>
        /// <returns>True if continuous position of reads in contig.</returns>
        private static bool IsContinousLeft(
            ReadMap map,
            int readPosition,
            int contigPosition,
            int length)
        {
            return (map.Length - length + map.StartPositionOfContig + 1) == contigPosition &&
                (map.StartPositionOfRead - 1) == readPosition;
        }

        #endregion

        #region Private Class

        /// <summary>
        /// Stores information of kmer reads map with contigs.
        /// </summary>
        private class ContigIndex
        {
            #region Field Variables

            /// <summary>
            /// Contig stored in form of kmer maps of reads
            /// ------------------------------  Contig
            /// --------                        read1
            /// --------                        read5
            ///         --------                read2
            ///                 --------        read3
            ///                         ------  read4.
            /// </summary>
            private IList<IList<KmerIndexer>> contigReadMatchIndexes = new List<IList<KmerIndexer>>();

            #endregion

            /// <summary>
            /// Initializes a new instance of the ContigIndex class.
            /// </summary>
            /// <param name="contig">Contig sequence.</param>
            public ContigIndex(ISequence contig)
            {
                this.ContigSequence = contig;
            }

            /// <summary>
            /// Gets the value of Read as indexes of contig overlap.
            /// </summary>
            public IList<IList<KmerIndexer>> ContigReadMatchIndexes
            {
                get { return this.contigReadMatchIndexes; }
            }

            /// <summary>
            /// Gets the value of Contig sequence.
            /// </summary>
            public ISequence ContigSequence
            {
                get;
                private set;
            }
        }

        #endregion
    }
}

