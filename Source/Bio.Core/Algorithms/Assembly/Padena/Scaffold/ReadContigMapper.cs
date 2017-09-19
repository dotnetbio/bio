using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Bio.Algorithms.Kmer;
using Bio.Properties;
using Bio.Util;

namespace Bio.Algorithms.Assembly.Padena.Scaffold
{
    /// <summary>
    /// Maps reads to contigs.
    /// Class map reads to contigs using kmer method of alignment.
    /// Each Sequence is broken in kmers. And these kmer are aligned with 
    /// kmer of other sequences to generate un-gapped alignments.
    /// </summary>
    public class ReadContigMapper : IReadContigMapper
    {
        /// <summary>
        /// Public method mapping Reads to Contigs.
        /// </summary>
        /// <param name="contigs">List of sequences of contigs.</param>
        /// <param name="reads">List of input reads.</param>
        /// <param name="kmerLength">Length of kmer.</param>
        /// <returns>Contig Read Map.</returns>
        public ReadContigMap Map(IList<ISequence> contigs, IEnumerable<ISequence> reads, int kmerLength)
        {
            KmerIndexerDictionary map = SequenceToKmerBuilder.BuildKmerDictionary(contigs, kmerLength);
            ReadContigMap maps = new ReadContigMap();
            Parallel.ForEach(reads, readSequence =>
            {
                IEnumerable<ISequence> kmers = SequenceToKmerBuilder.GetKmerSequences(readSequence, kmerLength);
                ReadIndex read = new ReadIndex(readSequence);
                foreach (ISequence kmer in kmers)
                {
                    IList<KmerIndexer> positions;
                    if (map.TryGetValue(kmer, out positions) ||
                        map.TryGetValue(kmer.GetReverseComplementedSequence(), out positions))
                    {
                        read.ContigReadMatchIndexes.Add(positions);
                    }
                }

                IList<Task<IList<ReadMap>>> tasks =
                    new List<Task<IList<ReadMap>>>();

                // Stores information about contigs for which tasks has been generated.
                IList<long> visitedContigs = new List<long>();

                // Creates Task for every read in nodes for a given contig.
                for (int index = 0; index < read.ContigReadMatchIndexes.Count; index++)
                {
                    int readPosition = index;
                    foreach (KmerIndexer kmer in read.ContigReadMatchIndexes[index])
                    {
                        long contigIndex = kmer.SequenceIndex;
                        if (!visitedContigs.Contains(contigIndex))
                        {
                            visitedContigs.Add(contigIndex);
                            tasks.Add(
                                Task<IList<ReadMap>>.Factory.StartNew(
                                t => MapRead(
                                    readPosition, 
                                    read.ContigReadMatchIndexes, 
                                    contigIndex, 
                                    read.ReadSequence.Count, 
                                    kmerLength),
                                TaskCreationOptions.AttachedToParent));
                        }
                    }
                }

                var overlapMaps = new Dictionary<ISequence, IList<ReadMap>>();
                for (int index = 0; index < visitedContigs.Count; index++)
                {
                    overlapMaps.Add(contigs.ElementAt(visitedContigs[index]), tasks[index].Result);
                }

                lock (maps)
                {
                    if (!maps.ContainsKey(read.ReadSequence.ID))
                    {
                        maps.Add(read.ReadSequence.ID, overlapMaps);
                    }
                    else
                    {
                        throw new ArgumentException(
                            string.Format(CultureInfo.CurrentCulture, Resource.DuplicatingReadIds, read.ReadSequence.ID));
                    }
                }
            });

            return maps;
        }

        /// <summary>
        /// Traverse through list of contig-read match indexes for given read.
        /// </summary>
        /// <param name="position">Position from where list of 
        ///  indexes should be traversed.</param>
        /// <param name="contigReadMatch">List for contig-read match indexes.</param>
        /// <param name="contigIndex">Index of contig sequence.</param>
        /// <param name="readLength">Length of read.</param>
        /// <param name="kmerLength">Length of kmer.</param>
        /// <returns>List of read maps.</returns>
        private static IList<ReadMap> MapRead(
            long position,
            IList<IList<KmerIndexer>> contigReadMatch,
            long contigIndex,
            long readLength,
            int kmerLength)
        {
            IList<ReadMap> readMaps = new List<ReadMap>();
            for (long index = position; index < contigReadMatch.Count; index++)
            {
                foreach (KmerIndexer kmer in contigReadMatch[(int)index])
                {
                    if (kmer.SequenceIndex == contigIndex)
                    {
                        FindContinuous(kmer, readMaps, index, kmerLength, readLength);
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
        /// <param name="readLength">Length of the read.</param>
        private static void FindContinuous(
            KmerIndexer kmer,
            IList<ReadMap> readMaps,
            long position,
            int kmerLength,
            long readLength)
        {
            // Create new object ReadInformation as read is encountered first time.
            if (readMaps.Count == 0)
            {
                foreach (int pos in kmer.Positions)
                {
                    ReadMap readMap = new ReadMap
                    {
                        StartPositionOfContig = pos,
                        StartPositionOfRead = position,
                        Length = kmerLength
                    };
                    SetReadContigOverlap(readLength, readMap);
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
                        if (IsContinousRight(read, position, pos, kmerLength) ||
                            IsContinousLeft(read, position, pos, kmerLength))
                        {
                            read.Length++;
                            if (read.StartPositionOfContig > pos)
                            {
                                read.StartPositionOfContig = pos;
                            }

                            SetReadContigOverlap(readLength, read);
                            isMerged = true;
                            break;
                        }
                    }

                    // If not continuous a new object ReadMap is created to store new overlap.
                    if (isMerged == false)
                    {
                        ReadMap readmap = new ReadMap
                        {
                            StartPositionOfContig = pos,
                            StartPositionOfRead = position,
                            Length = kmerLength
                        };
                        SetReadContigOverlap(readLength, readmap);
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
            long readPosition,
            long contigPosition,
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
            long readPosition,
            long contigPosition,
            int length)
        {
            return (map.Length - length + map.StartPositionOfRead + 1) == readPosition &&
                (map.StartPositionOfContig - 1) == contigPosition;
        }

        /// <summary>
        /// Determines whether read is full or partial overlap between read and contig.
        /// Overlap of read and contig
        /// FullOverlap
        /// ------------- Contig
        ///    ------     Read
        /// PartialOverlap
        /// -------------       Contig
        ///            ------   Read.
        /// </summary>
        /// <param name="length">Length of read.</param>
        /// <param name="read">Map of read to contig.</param>
        private static void SetReadContigOverlap(long length, ReadMap read)
        {
            if (length == read.Length)
            {
                read.ReadOverlap = ContigReadOverlapType.FullOverlap;
            }
            else
            {
                read.ReadOverlap = ContigReadOverlapType.PartialOverlap;
            }
        }

        /// <summary>
        /// Stores information of kmer reads map with contigs.
        /// </summary>
        internal class ReadIndex
        {
            /// <summary>
            /// Contig stored in form of kmer maps of reads
            /// ------------------------------  Contig
            /// --------                        read1
            /// --------                        read5
            ///         --------                read2
            ///                 --------        read3
            ///                         ------  read4.
            /// </summary>
            private readonly IList<IList<KmerIndexer>> contigReadMatchIndexes = new List<IList<KmerIndexer>>();

            /// <summary>
            /// Read Sequence.
            /// </summary>
            private readonly ISequence readSequence;

            /// <summary>
            /// Initializes a new instance of the ReadIndex class.
            /// </summary>
            /// <param name="read">Read sequence.</param>
            public ReadIndex(ISequence read)
            {
                this.readSequence = read;
            }

            /// <summary>
            /// Gets the value of Read as indexes of contig overlap.
            /// </summary>
            public IList<IList<KmerIndexer>> ContigReadMatchIndexes
            {
                get { return this.contigReadMatchIndexes; }
            }

            /// <summary>
            /// Gets the value of read sequence.
            /// </summary>
            public ISequence ReadSequence
            {
                get
                {
                    return this.readSequence;
                }
            }
        }
    }
}
