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
            this.BaseSequence = sequence;
            this.Length = kmerLength;
            this.Kmers = kmers;
        }

        /// <summary>
        /// Initializes a new instance of the KmersOfSequence class.
        /// Takes k-mer sequence and k-mer length.
        /// </summary>
        /// <param name="sequence">Source sequence.</param>
        /// <param name="kmerLength">Length of k-mer.</param>
        public KmersOfSequence(ISequence sequence, int kmerLength)
        {
            this.BaseSequence = sequence;
            this.Length = kmerLength;
            this.Kmers = null;
        }

        /// <summary>
        /// Gets the length of associated k-mers.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the set of associated Kmers.
        /// </summary>
        public HashSet<KmerPositions> Kmers { get; private set; }

        /// <summary>
        /// Gets the base sequence.
        /// </summary>
        public ISequence BaseSequence { get; private set; }

        #endregion

        /// <summary>
        /// Returns the associated k-mers as a list of k-mer sequences.
        /// </summary>
        /// <returns>List of k-mer sequences.</returns>
        public IEnumerable<ISequence> KmersToSequences()
        {
            return new List<ISequence>(this.Kmers.Select(k => this.BaseSequence.GetSubSequence(k.Positions.First(), this.Length)));
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

            return this.BaseSequence.GetSubSequence(kmer.Positions.First(), this.Length);
        }

        #region Nested Structure
        /// <summary>
        /// Contains information regarding k-mer
        /// position in the base sequence.
        /// </summary>
        public class KmerPositions
        {
            /// <summary>
            /// Initializes a new instance of the KmerPositions class.
            /// </summary>
            /// <param name="positions">List of positions.</param>
            public KmerPositions(IEnumerable<long> positions)
            {
                Positions = new List<long>(positions);
            }

            /// <summary>
            /// Gets the list of positions for the k-mer.
            /// </summary>
            public List<long> Positions { get; private set; }

            /// <summary>
            /// Gets the number of positions.
            /// </summary>
            public int Count
            {
                get { return this.Positions.Count; }
            }
        }
        #endregion
    }
}
