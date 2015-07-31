using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bio.Util;
using System;

namespace Bio.Matrix
{
    /// <summary>
    /// A wrapper around one or more parent matrices that created a merged view of their rows.
    /// Because it is a view, any changes made to the values of this matrix or the parents are reflected in all.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    [Serializable]
    public class MergeRowsView<TRowKey, TColKey, TValue> : AbstractMatrixView<TRowKey, TColKey, TValue>
    {
        TValue _missingValue;
        ReadOnlyCollection<TRowKey> _rowKeys;
        ReadOnlyCollection<TColKey> _colKeys;
        RestrictedAccessDictionary<TRowKey, int> _indexOfRowKey;
        RestrictedAccessDictionary<TColKey, int> _indexOfColKey;
        Dictionary<TRowKey, Matrix<TRowKey, TColKey, TValue>> _rowKeyToMatrix;



        /// <summary>
        /// Creates a new view in which the rows of the matrices are merged. The rows will be in the order
        /// of the input matrices. If two matrices contain a rows with the same rowKey, an exception is thrown. All matricies must have the same
        /// MissingValue.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <param name="colsMustMatch">true, to require all matrices to have the same colKeys in the same order; false, use an intersection of the
        /// colKeys in the order of ColKeys of the first matrix.</param>
        /// <param name="matrices">One or more matricies with which to concatinate rows.</param>
        public MergeRowsView(bool colsMustMatch, params Matrix<TRowKey, TColKey, TValue>[] matrices)
        {
            Helper.CheckCondition(matrices.Length > 0, () => Properties.Resource.ExpectedNonZeroLengthArrayOfMatrices);
            Helper.CheckCondition(matrices.All(m => m.IsMissing(matrices[0].MissingValue)), () => Properties.Resource.ExpectedAllMatricesToHaveSameMissingValue);
            _missingValue = matrices[0].MissingValue;

            IEnumerable<TColKey> colKeys = matrices[0].ColKeys;
            var rowKeys = matrices[0].RowKeys.ToList();

            _rowKeyToMatrix = new Dictionary<TRowKey, Matrix<TRowKey, TColKey, TValue>>();
            matrices[0].RowKeys.ForEach(rowKey => _rowKeyToMatrix.Add(rowKey, matrices[0]));

            for (int i = 1; i < matrices.Length; i++)
            {
                if (colsMustMatch)
                {
                    Helper.CheckCondition(Helper.KeysEqual(matrices[0].IndexOfColKey, matrices[i].IndexOfColKey), () => Properties.Resource.ExpectedColumnsToMatch);
                }
                else
                {
                    colKeys = colKeys.Intersect(matrices[i].ColKeys);
                }

                //rowKeys.AddRange(matrices[i].RowKeys);
                foreach (var rowKey in matrices[i].RowKeys)
                {
                    if (_rowKeyToMatrix.ContainsKey(rowKey))
                    {
                        var prevMatrix = _rowKeyToMatrix[rowKey];
                        // check that all the values are identical. not that GetValueOrMissing().Equals is not safe because missing value may be null.
                        Helper.CheckCondition<System.ArgumentException>(colKeys.All(colKey =>
                            prevMatrix.IsMissing(rowKey, colKey) ?
                                matrices[i].IsMissing(rowKey, colKey) :
                                prevMatrix[rowKey, colKey].Equals(matrices[i].GetValueOrMissing(rowKey, colKey))), Properties.Resource.ThereAreDuplicateRowsWhichDoNotHDupRowsNonIdenticalValues);
                    }
                    else
                    {
                        _rowKeyToMatrix.Add(rowKey, matrices[i]);
                        rowKeys.Add(rowKey);
                    }
                }
                //matrices[i].RowKeys.ForEach(rowKey => _rowKeyToMatrix.Add(rowKey, matrices[i]));
            }

            Helper.CheckCondition(rowKeys.Count == rowKeys.Distinct().Count(), () => Properties.Resource.ExpectedUniqueRowKeysInMatrix);

            _rowKeys = rowKeys.AsReadOnly();
            _colKeys = colKeys.ToList().AsReadOnly();

            _indexOfRowKey = _rowKeys.Select((key, i) => new KeyValuePair<TRowKey, int>(key, i)).ToDictionary().AsRestrictedAccessDictionary();
            _indexOfColKey = _colKeys.Select((key, i) => new KeyValuePair<TColKey, int>(key, i)).ToDictionary().AsRestrictedAccessDictionary();
        }

#pragma warning disable 1591
        protected override void GetMatrixAndIndex(int rowIndex, int colIndex, out Matrix<TRowKey, TColKey, TValue> m, out int mappedRowIndex, out int mappedColIndex)
#pragma warning restore 1591
        {
            m = null;
            mappedColIndex = mappedRowIndex = -1;

            TRowKey rowKey = _rowKeys[rowIndex];
            TColKey colKey = _colKeys[colIndex];

            m = _rowKeyToMatrix[rowKey];

            mappedRowIndex = m.IndexOfRowKey[rowKey];
            mappedColIndex = m.IndexOfColKey[colKey];
        }

#pragma warning disable 1591
        protected override void GetMatrixAndKey(TRowKey rowKey, TColKey colKey, out Matrix<TRowKey, TColKey, TValue> m, out TRowKey mappedRowKey, out TColKey mappedColKey)
#pragma warning restore 1591
        {
            mappedColKey = colKey;
            mappedRowKey = rowKey;

            bool isOK = _rowKeyToMatrix.TryGetValue(rowKey, out m);
            Helper.CheckCondition<KeyNotFoundException>(isOK, "RowKey '{0}' not found", rowKey);
            //m = _rowKeyToMatrix[rowKey];
        }

#pragma warning disable 1591
        public override ReadOnlyCollection<TRowKey> RowKeys
#pragma warning restore 1591
        {
            get { return _rowKeys; }
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
            get { return _indexOfRowKey; }
        }

#pragma warning disable 1591
        public override IDictionary<TColKey, int> IndexOfColKey
#pragma warning restore 1591
        {
            get { return _indexOfColKey; }
        }

#pragma warning disable 1591
        public override TValue MissingValue
#pragma warning restore 1591
        {
            get { return _missingValue; }
        }

    }
}
