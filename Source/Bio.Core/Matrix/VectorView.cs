using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bio.Util;

namespace Bio.Matrix
{
    internal class RowView<TRowKey, TColKey, TValue> : IDictionary<TColKey, TValue>
    {
        Matrix<TRowKey, TColKey, TValue> _matrix;
        TRowKey _rowKey;

        internal RowView(Matrix<TRowKey, TColKey, TValue> matrix, TRowKey rowKey)
        {
            if (!matrix.ContainsRowKey(rowKey))
            {
                throw new IndexOutOfRangeException(string.Format("{0} is not a row key in the matrix", rowKey));
            }
            _matrix = matrix;
            _rowKey = rowKey;
        }


        // Count of the nonmissing items in the row. Requires a linear scan.
        public int Count
        {
            get
            {
                int count = _matrix.ColKeys.Where(colKey => !_matrix.IsMissing(_rowKey, colKey)).Count();
                return count;
            }
        }

        public TValue this[TColKey colKey]
        {
            get
            {
                return _matrix[_rowKey, colKey];
            }
            set
            {
                _matrix[_rowKey, colKey] = value;
            }
        }


        public bool Remove(TColKey colKey)
        {
            return _matrix.Remove(_rowKey, colKey);
        }

 
        public bool ContainsKey(TColKey colKey)
        {
            return _matrix.ContainsColKey(colKey) && !_matrix.IsMissing(_rowKey, colKey);
        }

        public bool Remove(KeyValuePair<TColKey, TValue> pair)
        {
            if (Contains(pair))
            {
                return Remove(pair.Key);
            }
            return false;
        }



        public IEnumerator<KeyValuePair<TColKey, TValue>> GetEnumerator()
        {
            foreach (TColKey colKey in _matrix.ColKeys)
            {
                TValue value;
                if (_matrix.TryGetValue(_rowKey, colKey, out value))
                {
                    yield return new KeyValuePair<TColKey, TValue>(colKey, value);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void Add(TColKey colKey, TValue value)
        {
            _matrix[_rowKey, colKey] = value;
        }

        ICollection<TColKey> IDictionary<TColKey, TValue>.Keys
        {
            get
            {
                return this.Select(pair => pair.Key).ToList().AsReadOnly();
            }
        }

        public bool TryGetValue(TColKey colKey, out TValue value)
        {
            //A matrix gives an error if you ask for a key it doesn't know.
            //A dictionary does not.
            if (!_matrix.ContainsRowAndColKeys(_rowKey, colKey))
            {
                value = default(TValue);
                return false;
            }

            return _matrix.TryGetValue(_rowKey, colKey, out value);
        }

        ICollection<TValue> IDictionary<TColKey, TValue>.Values
        {
            get
            {
                return this.Select(pair => pair.Value).ToList().AsReadOnly();
            }
        }


        public void Add(KeyValuePair<TColKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public void Clear()
        {
            foreach (TColKey colKey in _matrix.ColKeys)
            {
                _matrix.Remove(_rowKey, colKey);
            }
        }

        public bool Contains(KeyValuePair<TColKey, TValue> pair)
        {
            Helper.CheckCondition(!_matrix.IsMissing(pair.Value), () => Properties.Resource.MatrixSpecialValueUseError);
            return _matrix.GetValueOrMissing(_rowKey, pair.Key).Equals(pair.Value);
        }

        public void CopyTo(KeyValuePair<TColKey, TValue>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TColKey, TValue> keyVal in this)
            {
                array[arrayIndex++] = keyVal;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }


    }

    internal class ColView<TRowKey, TColKey, TValue> : IDictionary<TRowKey, TValue>
    {
        Matrix<TRowKey, TColKey, TValue> _matrix;
        TColKey _colKey;

        internal ColView(Matrix<TRowKey, TColKey, TValue> matrix, TColKey colKey)
        {
            if (!matrix.ContainsColKey(colKey))
            {
                throw new IndexOutOfRangeException(string.Format("{0} is not a col key in the matrix", colKey));
            }

            _matrix = matrix;
            _colKey = colKey;
        }


        // Count of the nonmissing items in the row. Requires a linear scan.
        public int Count
        {
            get
            {
                int count = _matrix.RowKeys.Where(rowKey => !_matrix.IsMissing(rowKey, _colKey)).Count();
                return count;
            }
        }

        public TValue this[TRowKey rowKey]
        {
            get
            {
                return _matrix[rowKey, _colKey];
            }
            set
            {
                _matrix[rowKey, _colKey] = value;
            }
        }


        public bool Remove(TRowKey rowKey)
        {
            return _matrix.Remove(rowKey, _colKey);
        }


        public bool ContainsKey(TRowKey rowKey)
        {
            return _matrix.ContainsRowKey(rowKey) && !_matrix.IsMissing(rowKey, _colKey);
        }

        public bool Remove(KeyValuePair<TRowKey, TValue> pair)
        {
            if (Contains(pair))
            {
                return Remove(pair.Key);
            }
            return false;
        }



        public IEnumerator<KeyValuePair<TRowKey, TValue>> GetEnumerator()
        {
            foreach (TRowKey rowKey in _matrix.RowKeys)
            {
                TValue value;
                if (_matrix.TryGetValue(rowKey, _colKey, out value))
                {
                    yield return new KeyValuePair<TRowKey, TValue>(rowKey, value);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void Add(TRowKey rowKey, TValue value)
        {
            _matrix[rowKey, _colKey] = value;
        }

        ICollection<TRowKey> IDictionary<TRowKey, TValue>.Keys
        {
            get
            {
                return this.Select(pair => pair.Key).ToList().AsReadOnly();
            }
        }

        public bool TryGetValue(TRowKey rowKey, out TValue value)
        {
            //A matrix gives an error if you ask for a key it doesn't know.
            //A dictionary does not.
            if (!_matrix.ContainsRowAndColKeys(rowKey, _colKey))
            {
                value = default(TValue);
                return false;
            }
            return _matrix.TryGetValue(rowKey, _colKey, out value);
        }

        ICollection<TValue> IDictionary<TRowKey, TValue>.Values
        {
            get
            {
                return this.Select(pair => pair.Value).ToList().AsReadOnly();
            }
        }


        public void Add(KeyValuePair<TRowKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public void Clear()
        {
            foreach (TRowKey rowKey in _matrix.RowKeys)
            {
                _matrix.Remove(rowKey, _colKey);
            }
        }

        public bool Contains(KeyValuePair<TRowKey, TValue> pair)
        {
            Helper.CheckCondition(!_matrix.IsMissing(pair.Value), Properties.Resource.MatrixSpecialValueUseError);
            return _matrix.GetValueOrMissing(pair.Key, _colKey).Equals(pair.Value);
        }

        public void CopyTo(KeyValuePair<TRowKey, TValue>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TRowKey, TValue> keyVal in this)
            {
                array[arrayIndex++] = keyVal;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
