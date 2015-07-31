using System;
using System.Collections.Generic;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Asymmetric Distance matrix class: m[i,j] != m[j,i]
    /// ASymmetricDistanceMatrix[i,j] is proportional to the 'evolutionary' distance
    /// from sequence i to sequence j, i.e. the higher the value, the more divergent
    /// the two sequences.
    /// 
    /// The matrix data is stored in float rectangle
    /// </summary>
    public class AsymmetricDistanceMatrix : DistanceMatrix
    {
        #region Member Variables
        // The float rectangle
        private float[,] _distanceMatrix;

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
        /// <param name="dimension">integer dimension of the matrix</param>
        public AsymmetricDistanceMatrix(int dimension) : base(dimension)
        {
            try
            {
                _distanceMatrix = new float[dimension, dimension];
            }
            catch (OutOfMemoryException ex)
            {
                throw new Exception("out of memory when creating the matrix", ex.InnerException);
            }
        }

        #region Indexer
        /// <summary>
        /// Convert 2d matrix to 1d linear List
        /// Take advantage of symmetry matrix that only right top corner is stored.
        /// </summary>
        /// <param name="row">zero-baed row number</param>
        /// <param name="col">zero-baed col number</param>
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
                return _distanceMatrix[row, col];
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
                _distanceMatrix[row, col] = value;

                // Update one-dimension nearestNeighbors and nearestDistances.
                if (_nearestDistances[col] > value)
                {
                    _nearestDistances[col] = value;
                    _nearestNeighbors[col] = row;
                }
            }
        }
        #endregion
    }
}

