using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bio.Util;

namespace Bio.Matrix
{
    /// <summary>
    /// A wrapper around a parent matrix that allows a matrix to be hashed. This, for example, allows a matrix to be
    /// used as the key of a dictionary. Two matricies will hash together if they are MatrixEqual.
    /// 
    /// Every time it needs to confirm that two matrices really are equal, it will call MatrixEqual which
    /// can require a scan of every value.
    /// 
    /// The wrapper doesn't allow it's values to be changed. Any changes in the values of its parent matrix will *not* be
    /// reflected in its hashcode and so can give unexpected results.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    public class HashableView<TRowKey, TColKey, TValue> : Matrix<TRowKey, TColKey, TValue>
    {

        private Matrix<TRowKey, TColKey, TValue> _parentMatrix;
        /// <summary>
        /// The matrix that this view wraps.
        /// </summary>
        public Matrix<TRowKey, TColKey, TValue> ParentMatrix
        {
            get
            {
                return _parentMatrix;
            }
            internal set
            {
                _parentMatrix = value;
                _hashCode = MatrixHashCode(_parentMatrix);
            }
        }


        internal HashableView()
        {
        }

#pragma warning disable 1591
        public override ReadOnlyCollection<TRowKey> RowKeys
#pragma warning restore 1591
        {
            get { return ParentMatrix.RowKeys; }
        }

#pragma warning disable 1591
        public override ReadOnlyCollection<TColKey> ColKeys
#pragma warning restore 1591
        {
            get { return ParentMatrix.ColKeys; }
        }

#pragma warning disable 1591
        public override IDictionary<TRowKey, int> IndexOfRowKey
#pragma warning restore 1591
        {
            get { return ParentMatrix.IndexOfRowKey; }
        }

#pragma warning disable 1591
        public override IDictionary<TColKey, int> IndexOfColKey
#pragma warning restore 1591
        {
            get { return ParentMatrix.IndexOfColKey; }
        }

#pragma warning disable 1591
        public override bool TryGetValue(TRowKey rowKey, TColKey colKey, out TValue value)
#pragma warning restore 1591
        {
            return ParentMatrix.TryGetValue(rowKey, colKey, out value);
        }

#pragma warning disable 1591
        public override bool TryGetValue(int rowIndex, int colIndex, out TValue value)
#pragma warning restore 1591
        {
            return ParentMatrix.TryGetValue(rowIndex, colIndex, out value);
        }

        /// <summary>
        /// The wrapper doesn't allow its values to be changed. This method throws an exception.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <returns>This method throws an exception.</returns>
        public override bool Remove(TRowKey rowKey, TColKey colKey)
        {
            throw new Exception("Values may not be removed from a HashableView");
        }

#pragma warning disable 1591
        public override TValue MissingValue
#pragma warning restore 1591
        {
            get { return ParentMatrix.MissingValue; }
        }

        /// <summary>
        /// The wrapper doesn't allow its values to be changed. This method throws an exception.
        /// </summary>
        /// <param name="rowIndex">The index for the row of interest</param>
        /// <param name="colIndex">The index for the col of interest</param>
        /// <param name="value">The value to set</param>
        /// <returns>This method throws an exception.</returns>
        public override void SetValueOrMissing(int rowIndex, int colIndex, TValue value)
        {
            throw new Exception("Values may not be removed from a HashableView");
        }

        /// <summary>
        /// The wrapper doesn't allow its values to be changed. This method throws an exception.
        /// </summary>
        /// <param name="rowKey">The key for the row of interest. The key must exist in RowKeys (and, thus, IndexOfRowKey)</param>
        /// <param name="colKey">The key for the col of interest. The key must exist in ColKeys (and, thus, IndexOfColKey)</param>
        /// <param name="value">Value that will be set.</param>
        /// <returns>This method throws an exception.</returns>
        public override void SetValueOrMissing(TRowKey rowKey, TColKey colKey, TValue value)
        {
            throw new Exception("Values may not be removed from a HashableView");
        }


        /// <summary>
        /// Returns the hashcode of the matrix. This values is computed only once from when the HashableView is constructed.
        /// Two matricies will hash together if they are MatrixEqual.
        /// 
        /// Every time the class needs to confirm that two matrices really are equal, it will call Equals which calls MatrixEqual which
        /// can require a scan of every value.
        /// </summary>
        /// <returns>a hashcode based on rowKeys, colKeys, special missing value, and values of the matrix.</returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }

        int _hashCode;

        private int MatrixHashCode(Matrix<TRowKey, TColKey, TValue> matrix)
        {
            int hashCode = 99382823 ^ MissingValue.GetHashCode();

            foreach (TRowKey rowKey in matrix.RowKeys)
            {
                hashCode = Helper.WrapAroundLeftShift(hashCode, 1) ^ rowKey.GetHashCode();
            }
            foreach (TColKey colKey in matrix.ColKeys)
            {
                hashCode = Helper.WrapAroundLeftShift(hashCode, 1) ^ colKey.GetHashCode();
            }
            for (int rowIndex = 0; rowIndex < RowCount; ++rowIndex)
            {
                for (int colIndex = 0; colIndex < ColCount; ++colIndex)
                {
                    TValue value = GetValueOrMissing(rowIndex, colIndex);
                    hashCode = Helper.WrapAroundLeftShift(hashCode, 1) ^ value.GetHashCode();
                }
            }
            return hashCode;
        }

        /// <summary>
        /// Two HashableView matrices are equal if they are MatrixEquals, that is, they have the same
        /// RowKeys and ColKeys (in the same order), the same special MissingValue, and the same values.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            HashableView<TRowKey, TColKey, TValue> other = obj as HashableView<TRowKey, TColKey, TValue>;
            if (other == null)
            {
                return false;
            }
            else
            {
                return _hashCode == other._hashCode && this.MatrixEquals(other);
            }
        }
    }
}
