using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Implementation of Distance matrix Interface.
    /// 
    /// A distance matrix is a matrix with pairwise distances on a set of sequences.
    /// It is therefore a square matrix with the dimension equals to the number of
    /// sequences, where rows and columns are sequences and elements are distances.
    /// 
    /// A distance matrix could be symmetry (d(A,B) = d(B,A)) or 
    /// asymmetry (d(A,B)!= d(B,A)). 
    /// 
    /// The distance, DistanceMatrix[i,j], is proportional to the 'evolutionary' 
    /// distance between two sequences, i.e. the higher the value, the more divergent
    /// the two sequences. Thus the minimum value of the matrix corresponds to the 
    /// closest pair in the dataset.
    /// 
    /// </summary>
    public interface IDistanceMatrix
    {
        /// <summary>
        /// The dimention of distance matrix is [Dimention x Dimention]
        /// </summary>
        int Dimension { get; }

        /// <summary>
        /// Access the element by indexer
        /// </summary>
        /// <param name="row">zero-based row index</param>
        /// <param name="col">zero-based col index</param>
        float this[int row, int col] { get; set; }

        /// <summary>
        /// Return the minimum distance value in the matrix
        /// </summary>
        float MinimumValue { get; }

        /// <summary>
        /// Return the coordinate of the minimum distance value in the matrix
        /// </summary>
        int[] MinimumValueCoordinates { get; }

        /// <summary>
        /// One-dimension array recording the minimum value row index in each column
        /// </summary>
        int[] NearestNeighbors { get; }

        /// <summary>
        /// One-dimension array recording the minimum distance in each column
        /// </summary>
        float[] NearestDistances { get; }
    }
}
