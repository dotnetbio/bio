using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bio.Algorithms.Alignment.MultipleSequenceAlignment
{
    /// <summary>
    /// Abstract class of Distance Matrix
    /// </summary>
    abstract public class DistanceMatrix : IDistanceMatrix
    {
        #region Fields

        /// <summary>
        /// [_dimension x _dimension] matrix
        /// </summary>
        protected int _dimension;

        /// <summary>
        /// One-dimension array recording the minimum value row index in each column
        /// </summary>
        protected int[] _nearestNeighbors = null;

        /// <summary>
        /// One-dimension array recording the minimum distance in each column
        /// </summary>
        protected float[] _nearestDistances = null;

        #endregion

        #region Properties

        /// <summary>
        /// The dimension of this matrix
        /// </summary>
        virtual public int Dimension 
        { 
            get { return _dimension; } 
        }

        /// <summary>
        /// One-dimension array recording the minimum value row index in each column
        /// </summary>
        virtual public int[] NearestNeighbors
        {
            get { return _nearestNeighbors; }
        }

        /// <summary>
        /// One-dimension array recording the minimum distance in each column
        /// </summary>
        virtual public float[] NearestDistances
        {
            get { return _nearestDistances; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Construct a [dimension x dimension] matrix
        /// </summary>
        /// <param name="dimension">integer dimension</param>
        public DistanceMatrix(int dimension)
        {
            if (dimension <= 0)
            {
                throw new ArgumentException("Negative dimension of matrix.");
            }

            _dimension = dimension;

            try
            {
                _nearestNeighbors = new int[_dimension];
                _nearestDistances = new float[_dimension];
            }
            catch (OutOfMemoryException ex)
            {
                throw new Exception("Out of momery", ex.InnerException);
            }

            Parallel.For(0, _dimension, i =>
            {
                _nearestDistances[i] = float.MaxValue;
            });
        }
        #endregion

        #region Interface methods
        /// <summary>
        /// Access the element of the matrix. The indexer is different
        /// between symmetric and asymmetric matrices, because symmetric 
        /// matrix is stored in a linear list with bottom left corner.
        /// </summary>
        /// <param name="row">zero-based row number</param>
        /// <param name="col">zero-based col number</param>
        abstract public float this[int row, int col] { get; set; }

        /// <summary>
        /// The smallest value in the matrix
        /// </summary>
        public float MinimumValue
        {
            get
            {
                float min = float.MaxValue;

                Parallel.For(0, _dimension, col =>
                //for (int col = 0; col < _dimension; ++col)
                {
                    if (_nearestDistances[col] < min)
                    {
                        min = _nearestDistances[col];
                    }
                //}
                });
                return min;
            }
        }

        /// <summary>
        /// The coordinates of the smallest value in the matrix
        /// </summary>
        public int[] MinimumValueCoordinates
        {
            get
            {
                int bestRow = 0, bestCol = 0;
                float min = float.MaxValue;

                Parallel.For(0, _dimension, col =>
                //for (int col = 0; col < _dimension; ++col)
                {
                    if (_nearestDistances[col] < min)
                    {
                        min = _nearestDistances[col];
                        bestRow = _nearestNeighbors[col];
                        bestCol = col;
                    }
                //}
                });
                return new int[2] { bestRow, bestCol };
            }
        }
        #endregion
    }
}
