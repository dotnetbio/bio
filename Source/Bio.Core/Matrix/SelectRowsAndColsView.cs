using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bio.Util;
using System;
using System.Globalization;

namespace Bio.Matrix
{
    /// <summary>
    /// A wrapper around a parent matrix that can select a subset of rows and cols. It can also change the order
    /// of the row keys and col keys.
    /// Because it is a view, any changes made to the values of this matrix or the parent matrix are reflected in both.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    [Serializable]
    public class SelectRowsAndColsView<TRowKey, TColKey, TValue> : Matrix<TRowKey, TColKey, TValue>
    {
        internal SelectRowsAndColsView()
        {
        }

        /// <summary>
        /// The matrix that this view wraps.
        /// </summary>
        public Matrix<TRowKey, TColKey, TValue> ParentMatrix { get; internal set; }

        private ReadOnlyCollection<TRowKey> _rowKeys;
#pragma warning disable 1591
        override public ReadOnlyCollection<TRowKey> RowKeys { get { return _rowKeys; } }
#pragma warning restore 1591

        private ReadOnlyCollection<TColKey> _colKeys;
#pragma warning disable 1591
        override public ReadOnlyCollection<TColKey> ColKeys { get { return _colKeys; } }
#pragma warning restore 1591

        private IDictionary<TRowKey, int> _indexOfRowKey;
#pragma warning disable 1591
        override public IDictionary<TRowKey, int> IndexOfRowKey { get { return _indexOfRowKey.AsRestrictedAccessDictionary(); } }

        private IDictionary<TColKey, int> _indexOfColKey;
#pragma warning disable 1591
        override public IDictionary<TColKey, int> IndexOfColKey { get { return _indexOfColKey.AsRestrictedAccessDictionary(); } }
#pragma warning restore 1591

        /// <summary>
        /// A read-only list that maps a row index for this matrix into a row index of the parent matrix
        /// </summary>
        public ReadOnlyCollection<int> IndexOfParentRowKey;
        /// <summary>
        /// A read-only list that maps a col index for this matrix into a col index of the parent matrix
        /// </summary>
        public ReadOnlyCollection<int> IndexOfParentColKey;




        internal void SetUp(Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<TRowKey> rowKeySequence, IEnumerable<TColKey> colKeySequence)
        {
            ParentMatrix = parentMatrix;
            _rowKeys = new ReadOnlyCollection<TRowKey>(rowKeySequence.ToList());
            _colKeys = new ReadOnlyCollection<TColKey>(colKeySequence.ToList());

            Helper.CheckCondition(_rowKeys.All(row => parentMatrix.IndexOfRowKey.ContainsKey(row)), () => Properties.Resource.ExpectedMatrixViewRowKeysToBeSubsetOfParentMatrix);
            Helper.CheckCondition(_colKeys.All(col => parentMatrix.IndexOfColKey.ContainsKey(col)), () => Properties.Resource.ExpectedMatrixViewColKeysToBeSubsetOfParentMatrix);

            //!!!Matrix - these lines appear in many places
            _indexOfRowKey = RowKeys.Select((key, index) => new { key, index }).ToDictionary(pair => pair.key, pair => pair.index);
            _indexOfColKey = ColKeys.Select((key, index) => new { key, index }).ToDictionary(pair => pair.key, pair => pair.index);

            IndexOfParentRowKey = new ReadOnlyCollection<int>(_rowKeys.Select(rowKey => parentMatrix.IndexOfRowKey[rowKey]).ToList());
            IndexOfParentColKey = new ReadOnlyCollection<int>(_colKeys.Select(colKey => parentMatrix.IndexOfColKey[colKey]).ToList());
        }



#pragma warning disable 1591
        public override bool IsMissing(int rowIndex, int colIndex)
#pragma warning restore 1591
        {
            return ParentMatrix.IsMissing(IndexOfParentRowKey[rowIndex], IndexOfParentColKey[colIndex]);
        }

#pragma warning disable 1591
        public override bool IsMissing(TRowKey rowKey, TColKey colKey)
#pragma warning restore 1591
        {
            return ParentMatrix.IsMissing(rowKey, colKey);
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
            return ParentMatrix.TryGetValue(IndexOfParentRowKey[rowIndex], IndexOfParentColKey[colIndex], out value);
        }

#pragma warning disable 1591
        public override bool Remove(TRowKey rowKey, TColKey colKey)
#pragma warning restore 1591
        {
            Helper.CheckCondition(ContainsRowAndColKeys(rowKey, colKey), () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedRowKeyAndColKeyToBeInMatrix, rowKey, colKey));
            return ParentMatrix.Remove(rowKey, colKey);
        }

#pragma warning disable 1591
        public override bool Remove(int rowIndex, int colIndex)
#pragma warning restore 1591
        {
            //IndexOfParentRowKey will raise an error if the indexs are invalid, so we don't need to double check
            return ParentMatrix.Remove(IndexOfParentRowKey[rowIndex], IndexOfParentColKey[colIndex]);
        }

#pragma warning disable 1591
        public override TValue MissingValue
#pragma warning restore 1591
        {
            get { return ParentMatrix.MissingValue; }
        }


#pragma warning disable 1591
        public override void SetValueOrMissing(TRowKey rowKey, TColKey colKey, TValue value)
#pragma warning restore 1591
        {
            //This is needed because rows and columns are are OK in the parent may not be OK here
            Helper.CheckCondition(ContainsRowAndColKeys(rowKey, colKey), () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedRowKeyAndColKeyToBeInMatrix, rowKey, colKey));
            ParentMatrix.SetValueOrMissing(rowKey, colKey, value);
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(int rowIndex, int colIndex, TValue value)
#pragma warning restore 1591
        {
            //IndexOfParentRowKey will raise an error if the indexs are invalid, so we don't need to double check
            ParentMatrix.SetValueOrMissing(IndexOfParentRowKey[rowIndex], IndexOfParentColKey[colIndex], value);
        }


        internal bool ColsUnchanged()
        {
            if (ColCount != ParentMatrix.ColCount)
            {
                return false;
            }
            return ColKeys.SequenceEqual(ParentMatrix.ColKeys);
        }

    }
}
