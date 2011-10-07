using System;
using System.Collections.Generic;
using System.Linq;

namespace Bio.Algorithms.Kmer
{
    /// <summary>
    /// Contains base sequence, and information regarding associated k-mers.
    /// </summary>
    public class KmersOfSequence
    {
        #region Fields
        /// <summary>
        /// Holds an instance of sequence used for building k-mer.
        /// </summary>
        private ISequence baseSequence;

        /// <summary>
        /// Length of k-mer.
        /// </summary>
        private int length;

        /// <summary>
        /// Positions and count of k-mers occurring in base sequence.
        /// </summary>
        private HashSet<KmerPositions> kmers;
        #endregion

        #region Constructors, Properties
        /// <summary>
        /// Initializes a new instance of the KmersOfSequence class.
        /// Takes k-mer sequence and occurring position.
        /// </summary>
        /// <param name="sequence">Source sequence.</param>
        /// <param name="kmerLength">Length of k-mer.</param>
        /// <param name="kmers">Set of associated k-mers.</param>
        public KmersOfSequence(ISequence sequence, int kmerLength, HashSet<KmerPositions> kmers)
        {
            this.baseSequence = sequence;
            this.length = kmerLength;
            this.kmers = kmers;
        }

        /// <summary>
        /// Initializes a new instance of the KmersOfSequence class.
        /// Takes k-mer sequence and k-mer length.
        /// </summary>
        /// <param name="sequence">Source sequence.</param>
        /// <param name="kmerLength">Length of k-mer.</param>
        public KmersOfSequence(ISequence sequence, int kmerLength)
        {
            this.baseSequence = sequence;
            this.length = kmerLength;
            this.kmers = null;
        }

        /// <summary>
        /// Gets the length of associated k-mers.
        /// </summary>
        public int Length
        {
            get { return this.length; }
        }

        /// <summary>
        /// Gets the set of associated Kmers.
        /// </summary>
        public HashSet<KmerPositions> Kmers
        {
            get { return this.kmers; }
        }

        /// <summary>
        /// Gets the base sequence.
        /// </summary>
        public ISequence BaseSequence
        {
            get { return this.baseSequence; }
        }
        #endregion

        /// <summary>
        /// Returns the associated k-mers as a list of k-mer sequences.
        /// </summary>
        /// <returns>List of k-mer sequences.</returns>
        public IEnumerable<ISequence> KmersToSequences()
        {
            return new List<ISequence>(this.kmers.Select(k => this.baseSequence.GetSubSequence(k.Positions.First(), this.length)));
        }

        /// <summary>
        /// Builds the sequence corresponding to input kmer, 
        /// using base sequence.
        /// </summary>
        /// <param name="kmer">Input k-mer.</param>
        /// <returns>Sequence corresponding to input k-mer.</returns>
        public ISequence KmerToSequence(KmerPositions kmer)
        {
            if (kmer == null)
            {
                throw new ArgumentNullException("kmer");
            }

            return this.baseSequence.GetSubSequence(kmer.Positions.First(), this.length);
        }

        #region Nested Structure
        /// <summary>
        /// Contains information regarding k-mer
        /// position in the base sequence.
        /// </summary>
        public class KmerPositions
        {
            /// <summary>
            /// List of positions.
            /// </summary>
           private List<long> kmerPositions = new List<long>();

            /// <summary>
            /// Initializes a new instance of the KmerPositions class.
            /// </summary>
            /// <param name="positions">List of positions.</param>
            public KmerPositions(IList<long> positions)
            {
                this.kmerPositions.AddRange(positions);
            }

            /// <summary>
            /// Gets the list of positions for the k-mer.
            /// </summary>
            public IList<long> Positions
            {
                get { return this.kmerPositions; }
            }

            /// <summary>
            /// Gets the number of positions.
            /// </summary>
            public int Count
            {
                get { return this.kmerPositions.Count; }
            }
        }
        #endregion
    }
}
