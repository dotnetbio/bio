using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Symmetric Distance matrix class: m[i,j] = m[j,i]
    /// SymmetricDistanceMatrix[i,j] is proportional to the 'evolutionary' distance
    /// between two sequences, i.e. the higher the value, the more divergent the two
    /// sequences
    /// 
    /// Due to the symmetry of the matrix, only the bottom left corner is stored, e.g.
    /// i less or equal to j, return _DistanceMatrix[i,j]
    /// i greater than j, return _DistanceMatrix[j,i]
    /// 
    /// The corner of symmetry matrix is stored in a linear List.
    /// The 2d index and 1d index are converted back and forth.
    /// 
    /// </summary>
    public class SymmetricDistanceMatrix : DistanceMatrix
    {
        #region Member Variables
        /// <summary>
        /// The bottom left corner of the matrix is stored in a linear List
        /// </summary>
        private List<float> _linearDistanceMatrix = null;

        /// <summary>
        /// The dimension of this matrix
        /// </summary>
        override public int Dimension
        {
            get { return _dimension; }
        }

        /// <summary>
        /// One-dimension array recording the minimum value row index in each column
        /// </summary>
        override public int[] NearestNeighbors
        {
            get { return _nearestNeighbors; }
        }

        /// <summary>
        /// One-dimension array recording the minimum distance in each column
        /// </summary>
        override public float[] NearestDistances
        {
            get { return _nearestDistances; }
        }
        #endregion

        /// <summary>
        /// Symmetry matrix: (Dimension x Dimension) square matrix
        /// stores in linear _LinearDistanceMatrix.
        /// </summary>
        /// <param name="dimension">positive integer</param>
        public SymmetricDistanceMatrix(int dimension) : base(dimension)
        {
            try
            {
                _linearDistanceMatrix = new List<float>((dimension * (dimension + 1)) / 2);
            }
            catch (OutOfMemoryException ex)
            {
                throw new Exception("Out of momery", ex.InnerException);
            }

            for (int i = 0; i < _linearDistanceMatrix.Capacity; ++i)
            {
                _linearDistanceMatrix.Add(0);
            }
        }

        #region Indexer
        /// <summary>
        /// Convert 2d matrix to 1d linear List
        /// Take advantage of symmetry matrix that only left bottom  corner is stored.
        /// </summary>
        /// <param name="row">zero-based row index</param>
        /// <param name="col">zero-based column index</param>
        public override float this[int row, int col]
        {
            get
            {
                if (row < 0 || col <0)
                {
                    throw new ArgumentException("Negative indices.");
                }
                if (row >= Dimension || col >= Dimension)
                {
                    throw new ArgumentException("Indices exceeds the matrix dimension");
                }
                if (row <= col)
                {
                    return _linearDistanceMatrix[Summation(_dimension, row) + col];
                }
                else
                {
                    return _linearDistanceMatrix[Summation(_dimension, col) + row];
                }
            }

            set
            {
                if (row < 0 || col <0)
                {
                    throw new ArgumentException("Negative indices.");
                }
                if (row >= Dimension || col >= Dimension)
                {
                    throw new ArgumentException("Indices exceeds the matrix dimension");
                }
                if (row <= col)
                {
                    _linearDistanceMatrix[Summation(_dimension, row) + col] = value;
                }
                else
                {
                    _linearDistanceMatrix[Summation(_dimension, col) + row] = value;
                }

                // Update one-dimension nearestNeighbors and nearestDistances.
                if (_nearestDistances[col] >= value)
                {
                    _nearestDistances[col] = value;
                    _nearestNeighbors[col] = row;
                }
            }
        }
        #endregion

        #region Private Methods
        
        /// <summary>
        /// Function to convert the row/col number (whichever is bigger) to linear index
        /// </summary>
        /// <param name="n">the dimension of the matrix</param>
        /// <param name="current">current row/col number</param>
        private static int Summation(int n, int current)
        {
            int _result = 0;
            for (int i = 0; i < current; ++i)
            {
                _result += n - i - 1;
            }
            return _result;
        }

        /// <summary>
        /// Update NearestNeighbors[col] and NearestDistances[col]
        /// </summary>
        /// <param name="col">zero-based column index</param>
        private void UpdateColumnNearestVectors(int col)
        {
            Parallel.For(0, _dimension, row =>
            {
                if (row != col && this[row, col] < _nearestDistances[col])
                {
                    _nearestDistances[col] = this[row, col];
                    _nearestNeighbors[col] = row;
                }
            });
        }
        #endregion
    }
}
