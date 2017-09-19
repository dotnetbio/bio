using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bio.Util;
using System;
using System.Globalization;

namespace Bio.Matrix
{
    /// <summary>
    /// A wrapper around a parent matrix that allows col keys to be renamed and/or expanded. If expanded, then multiple keys will
    /// point to the same underlying row, meaning change to a value in one row will be reflected in another row.
    /// Also, because it is a view, any changes made to the values of this matrix or the parent matrix are reflected in both.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    public class RenameColsView<TRowKey, TColKey, TValue> : AbstractMatrixView<TRowKey, TColKey, TValue>
    {
        /// <summary>
        /// The matrix that this view wraps.
        /// </summary>
        public Matrix<TRowKey, TColKey, TValue> ParentMatrix { get; internal set; }

        /// <summary>
        /// A read-only dictionary giving the mapping from the col keys of this matrix to the col keys of its parent matrix.
        /// </summary>
        public IDictionary<TColKey, TColKey> NewKeyToOldKey { get; private set; }
        /// <summary>
        /// A read-only dictionary giving the mapping from the col index of this matrix to the col index of it's parent matrix
        /// </summary>
        public ReadOnlyCollection<int> NewIndexToOldIndex { get; private set; }
        private ReadOnlyCollection<TColKey> _colKeys;
        private RestrictedAccessDictionary<TColKey, int> _indexOfColKey;


        internal RenameColsView(Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<KeyValuePair<TColKey, TColKey>> newKeyAndOldKeySequence)
        {
            RenameColsViewInternal(parentMatrix, newKeyAndOldKeySequence);
        }


        private void RenameColsViewInternal(Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<KeyValuePair<TColKey, TColKey>> newNameAndOldNameSequence)
        {
            NewKeyToOldKey = newNameAndOldNameSequence.ToDictionary(pair => pair.Key, pair => pair.Value).AsRestrictedAccessDictionary();

            var unmatchedCols = NewKeyToOldKey.Values.Except(parentMatrix.ColKeys).ToList();
            Helper.CheckCondition(unmatchedCols.Count == 0, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedEveryRemappedColKeyToBeInOriginalMatrix, unmatchedCols.StringJoin(",")));

            ParentMatrix = parentMatrix;

            _colKeys = NewKeyToOldKey.Select(pair => pair.Key).ToList().AsReadOnly();
            _indexOfColKey = _colKeys.Select((key, idx) => new KeyValuePair<TColKey, int>(key, idx)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value).AsRestrictedAccessDictionary();

            NewIndexToOldIndex =
                (
                from newIndex in Enumerable.Range(0, ColCount)
                let newKey = ColKeys[newIndex]
                let oldKey = NewKeyToOldKey[newKey]
                let oldIndex = parentMatrix.IndexOfColKey[oldKey]
                select oldIndex
                ).ToList().AsReadOnly();

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
            get { return _colKeys; }
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
            get { return _indexOfColKey; }
        }

        private bool TryGetBaseColIndex(int colIndex, out int oldColIndex)
        {
            if (colIndex < 0 || colIndex >= NewIndexToOldIndex.Count)
            {
                oldColIndex = -1;
                return false;
            }
            else
            {
                oldColIndex = NewIndexToOldIndex[colIndex];
                return true;
            }
        }

        private bool TryGetOldColName(TColKey colKey, out TColKey oldColKey)
        {
            return NewKeyToOldKey.TryGetValue(colKey, out oldColKey);
        }

#pragma warning disable 1591
        protected override void GetMatrixAndIndex(int rowIndex, int colIndex, out Matrix<TRowKey, TColKey, TValue> m, out int mappedRowIndex, out int mappedColIndex)
#pragma warning restore 1591
        {
            m = ParentMatrix;
            mappedRowIndex = rowIndex;
            if (!TryGetBaseColIndex(colIndex, out mappedColIndex))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is not a mapped index.", colIndex));
        }

#pragma warning disable 1591
        protected override void GetMatrixAndKey(TRowKey rowKey, TColKey colKey, out Matrix<TRowKey, TColKey, TValue> m, out TRowKey mappedRowKey, out TColKey mappedColKey)
#pragma warning restore 1591
        {
            m = ParentMatrix;
            mappedRowKey = rowKey;
            if (!TryGetOldColName(colKey, out mappedColKey))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is not a mapped key.", colKey));
        }

#pragma warning disable 1591
        public override TValue MissingValue
#pragma warning restore 1591
        {
            get { return ParentMatrix.MissingValue; }
        }

    }
}
