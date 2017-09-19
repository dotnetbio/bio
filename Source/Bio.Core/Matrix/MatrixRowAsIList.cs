using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.Matrix
{
    class MatrixRowAsIList<TRowKey, TColKey, TValue> : IList<TValue>
    {
        private Matrix<TRowKey, TColKey, TValue> Matrix;
        private int RowIndex;

        public MatrixRowAsIList(Matrix<TRowKey, TColKey, TValue> matrix, int rowIndex)
        {
            // TODO: Complete member initialization
            this.Matrix = matrix;
            this.RowIndex = rowIndex;
        }

        #region IList<TValue> Members

        int IList<TValue>.IndexOf(TValue item)
        {
            throw new NotImplementedException();
        }

        void IList<TValue>.Insert(int index, TValue item)
        {
            throw new NotImplementedException();//!!!don't implement
        }

        void IList<TValue>.RemoveAt(int colIndex)
        {
            throw new NotImplementedException();//!!!don't implement because even marking as missing wouldn't change the index of later items
        }

        TValue IList<TValue>.this[int colIndex]
        {
            get
            {
                return Matrix.GetValueOrMissing(RowIndex, colIndex);
            }
            set
            {
                Matrix.SetValueOrMissing(RowIndex, colIndex, value);
            }
        }

        #endregion

        #region ICollection<TValue> Members

        void ICollection<TValue>.Add(TValue item)
        {
            throw new NotImplementedException(); //don't implement
        }

        void ICollection<TValue>.Clear()
        {
            throw new NotImplementedException(); //don't implement
        }

        bool ICollection<TValue>.Contains(TValue item)
        {
            throw new NotImplementedException();
        }

        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            for (int colIndex = 0; colIndex < Matrix.ColCount; ++colIndex)
            {
                array[arrayIndex + colIndex] = Matrix.GetValueOrMissing(RowIndex, colIndex);
            }
        }

        int ICollection<TValue>.Count
        {
            get
            {
                return Matrix.ColCount;
            }
        }

        bool ICollection<TValue>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<TValue>.Remove(TValue item)
        {
            throw new NotImplementedException(); //don't implement
        }

        #endregion

        #region IEnumerable<TValue> Members

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            for (int colIndex = 0; colIndex < Matrix.ColCount; ++colIndex)
            {
                yield return Matrix.GetValueOrMissing(RowIndex, colIndex);
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TValue>)this).GetEnumerator();
        }

        #endregion
    }
}
