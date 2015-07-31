using System.Collections.Generic;

namespace Bio.Algorithms.Kmer
{
    /// <summary>
    /// Structure that maintains sequence index, count information 
    /// and orientation for k-mer.
    /// </summary>
    public class KmerIndexer
    {
        /// <summary>
        /// Initializes a new instance of the KmerIndexer class.
        /// </summary>
        /// <param name="sequenceIndex">Index of source sequence.</param>
        /// <param name="positions">List of k-mer positions.</param>
        public KmerIndexer(long sequenceIndex, IList<long> positions)
        {
            this.SequenceIndex = sequenceIndex;
            this.Positions = positions;
        }

        /// <summary>
        /// Gets the starting position within sequence.
        /// </summary>
        public IList<long> Positions { get; private set; }

        /// <summary>
        /// Gets sequence index.
        /// </summary>
        public long SequenceIndex { get; private set; }
    }
}
