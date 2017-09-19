using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of distance matrix generator interface.
    /// 
    /// The generator takes a list of sequences as input, and generates
    /// a distance matrix, a square matrix, with its dimension equals to 
    /// the number of input sequences.
    /// 
    /// The generator measures the distance between each pair of sequences, 
    /// and deposits it into the matrix. The matrix is then used for 
    /// Hierarchical Clustering method to generate a binary guide tree.
    /// 
    /// The inputs sequences can be either aligned or unaligned.
    /// 
    /// In the case of unaligned seqences, Kmer Counting method is used to
    /// estimate the distances between sequences; in the case of aligned
    /// sequences, Kimura method is used instead.
    /// 
    /// </summary>
    public interface IDistanceMatrixGenerator
    {

        /// <summary>
        /// The distance matrix generated
        /// </summary>
        IDistanceMatrix DistanceMatrix { get; }

        /// <summary>
        /// The method name to generate the matrix
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Generate the distance matrix from a set of sequences
        /// </summary>
        /// <param name="sequences">a set of sequences (aligned or unaligned)</param>
        void GenerateDistanceMatrix(IList<ISequence> sequences);
    }
}
