using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Bio.Util;

namespace Bio.Matrix
{
    /// <summary>
    /// A wrapper around one or more parent matrices that created a merged view of their columns.
    /// Because it is a view, any changes made to the values of this matrix or the parents are reflected in all.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    public class MergeColsView<TRowKey, TColKey, TValue> : AbstractMatrixView<TRowKey, TColKey, TValue>
    {
        TValue _missingValue;
        ReadOnlyCollection<TRowKey> _rowKeys;
        ReadOnlyCollection<TColKey> _colKeys;
        RestrictedAccessDictionary<TRowKey, int> _indexOfRowKey;
        RestrictedAccessDictionary<TColKey, int> _indexOfColKey;
        Dictionary<TColKey, Matrix<TRowKey, TColKey, TValue>> _colKeyToMatrix;

        /// <summary>
        /// Creates a new view in which the cols of the matrices are merged. The cols will be in the order
        /// of the input matrices. If two matrices contain a rows with the same colKey, an exception is thrown. All matricies must have the same
        /// MissingValue.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <param name="rowsMustMatch">true, to require all matrices to have the same rowKeys in the same order; false, use an intersection of the
        /// rowKeys in the order of RowKeys of the first matrix.</param>
        /// <param name="matrices">One or more matricies with which to concatinate cols.</param>
        public MergeColsView(bool rowsMustMatch, params Matrix<TRowKey, TColKey, TValue>[] matrices)
        {
            Helper.CheckCondition(matrices.All(m => m.IsMissing(matrices[0].MissingValue)), Properties.Resource.ExpectedAllMatricesToHaveSameMissingValue);
            _missingValue = matrices[0].MissingValue;

            IEnumerable<TRowKey> rowKeys = matrices[0].RowKeys;
            var colKeys = matrices[0].ColKeys.ToList();

            _colKeyToMatrix = new Dictionary<TColKey, Matrix<TRowKey, TColKey, TValue>>();
            matrices[0].ColKeys.ForEach(colKey => _colKeyToMatrix.Add(colKey, matrices[0]));

            for (int i = 1; i < matrices.Length; i++)
            {
                if (rowsMustMatch)
                {
                    Helper.CheckCondition(Helper.KeysEqual(matrices[0].IndexOfRowKey, matrices[i].IndexOfRowKey), Properties.Resource.ExpectedRowsOfMatricesToMatch);
                }
                else
                {
                    rowKeys = rowKeys.Intersect(matrices[i].RowKeys);
                }

                colKeys.AddRange(matrices[i].ColKeys);
                matrices[i].ColKeys.ForEach(colKey => _colKeyToMatrix.Add(colKey, matrices[i]));
            }

            Helper.CheckCondition(colKeys.Count == colKeys.Distinct().Count(), Properties.Resource.ExpectedUniqueRowKeysInMatrix);

            _rowKeys = rowKeys.ToList().AsReadOnly();
            _colKeys = colKeys.AsReadOnly();

            _indexOfRowKey = _rowKeys.Select((key, i) => new KeyValuePair<TRowKey, int>(key, i)).ToDictionary().AsRestrictedAccessDictionary();
            _indexOfColKey = _colKeys.Select((key, i) => new KeyValuePair<TColKey, int>(key, i)).ToDictionary().AsRestrictedAccessDictionary();
        }

        internal override void GetMatrixAndIndex(int rowIndex, int colIndex, out Matrix<TRowKey, TColKey, TValue> resultMatrix, out int mappedRowIndex, out int mappedColIndex)
        {
            resultMatrix = null;
            mappedColIndex = mappedRowIndex = -1;

            TRowKey rowKey = _rowKeys[rowIndex];
            TColKey colKey = _colKeys[colIndex];

            resultMatrix = _colKeyToMatrix[colKey];

            mappedRowIndex = resultMatrix.IndexOfRowKey[rowKey];
            mappedColIndex = resultMatrix.IndexOfColKey[colKey];
        }

        internal override void GetMatrixAndKey(TRowKey rowKey, TColKey colKey, out Matrix<TRowKey, TColKey, TValue> m, out TRowKey mappedRowKey, out TColKey mappedColKey)
        {
            mappedColKey = colKey;
            mappedRowKey = rowKey;
            m = _colKeyToMatrix[colKey];
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
