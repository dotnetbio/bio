using System;

namespace Bio.Algorithms.Kmer
{
    /// <summary>
    /// Interface to store the node value.
    /// </summary>
    public interface IKmerData : IComparable
    {
        /// <summary>
        /// Returns the kmer data of the node value.
        /// </summary>
        /// <param name="kmerLength">Kmer length of the node value.</param>
        /// <returns>The sequence (uncompressed kmer).</returns>
        byte[] GetKmerData(int kmerLength);

        /// <summary>
        /// Returns the reverse complement of the node value.
        /// </summary>
        /// <param name="kmerLength">Kmer length of the node value.</param>
        /// <returns>The reverse complement of the kmer.</returns>
        byte[] GetReverseComplementOfKmerData(int kmerLength);

        /// <summary>
        /// Gets the kmer data as byte[].
        /// </summary>
        /// <returns>The sequence (uncompressed kmer).</returns>
        byte[] GetOriginalSymbols(int kmerLength, bool orientation);

        /// <summary>
        /// Creates the kmers from sequence.
        /// </summary>
        /// <param name="sequence">Sequence to be stored in the De-Bruijn node.</param>
        /// <param name="from">Start postition from where the kmer to be extracted.</param>
        /// <param name="kmerLength">Kmer length of the node value.</param>
        bool SetKmerData(ISequence sequence, long from, int kmerLength);

        /// <summary>
        /// Creates the kmers from sequence.
        /// </summary>
        /// <param name="sequence">Sequence to be stored in the De-Bruijn node.</param>
        /// <param name="kmerLength">Kmer length of the node value.</param>
        void SetKmerData(byte[] sequence, int kmerLength);

        /// <summary>
        /// Gets the reverse complement of the kmer value.
        /// </summary>
        /// <param name="kmerLength">Kmer length of the node value.</param>
        /// <param name="orientation">Orientation of connecting edge.</param>
        /// <returns>Returns the reverse complement of the sequence.</returns>
        byte[] GetReverseComplementOfOriginalSymbols(int kmerLength, bool orientation);

        /// <summary>
        /// Indicates whether the node value is palindrome or not.
        /// </summary>
        /// <param name="kmerLength">Kmer length of the node value.</param>
        /// <returns>True if the value is palindrome otherwise false.</returns>
        bool IsPalindrome(int kmerLength);

        /// <summary>
        ///  Gets first symbols in this kmer data.
        /// </summary>
        /// <param name="kmerLength">Kmer length of the node value.</param>
        /// <param name="orientation">Orientation of connecting edge.</param>
        /// <returns>Returns the first symbol of the sequence.</returns>
        byte GetFirstSymbol(int kmerLength, bool orientation);

        /// <summary>
        /// Gets the last symbol in this kmer data.
        /// </summary>
        /// <param name="kmerLength">Kmer length of the node value.</param>
        /// <param name="orientation">Orientation of connecting edge.</param>
        /// <returns>Returns the last symbol of the sequence.</returns>
        byte GetLastSymbol(int kmerLength, bool orientation);

        /// <summary>
        /// Compares this instance to a specified instance of IKmerData and returns an indication of their relative values.
        /// </summary>
        /// <param name="kmer">Instance of the IKmerData to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance. Zero This
        /// instance is equal to value. Greater than zero This instance is greater than
        /// value.
        /// </returns>
        int CompareTo(IKmerData kmer);
    }
}
