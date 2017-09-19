using System.Collections.ObjectModel;
using Bio.Util;
using System.Collections.Generic;

namespace Bio.Matrix
{
    /// <summary>
    /// A wrapper around a parent matrix that switches the rows with the columns.
    /// Because it is a view, any changes made to the values of this matrix or the parent matrix are reflected in both.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    public class TransposeView<TRowKey, TColKey, TValue> : Matrix<TRowKey, TColKey, TValue>
    {
        /// <summary>
        /// The matrix that this view wraps.
        /// </summary>
        public Matrix<TColKey, TRowKey, TValue> ParentMatrix { get; internal set; }


        internal TransposeView()
        {
        }

        #pragma warning disable 1591
        public override ReadOnlyCollection<TRowKey> RowKeys
        #pragma warning restore 1591
        {
            get { return ParentMatrix.ColKeys; }
        }

        #pragma warning disable 1591
        public override ReadOnlyCollection<TColKey> ColKeys
        #pragma warning restore 1591
        {
            get { return ParentMatrix.RowKeys; }
        }

        #pragma warning disable 1591
        public override IDictionary<TRowKey, int> IndexOfRowKey
        #pragma warning restore 1591
        {
            get { return ParentMatrix.IndexOfColKey; }
        }

        #pragma warning disable 1591
        public override IDictionary<TColKey, int> IndexOfColKey
        #pragma warning restore 1591
        {
            get { return ParentMatrix.IndexOfRowKey; }
        }

        #pragma warning disable 1591
        public override bool TryGetValue(TRowKey rowKey, TColKey colKey, out TValue value)
        #pragma warning restore 1591
        {
            return ParentMatrix.TryGetValue(colKey, rowKey, out value);
        }

        #pragma warning disable 1591
        public override bool TryGetValue(int rowIndex, int colIndex, out TValue value)
        #pragma warning restore 1591
        {
            return ParentMatrix.TryGetValue(colIndex, rowIndex, out value);
        }

        #pragma warning disable 1591
        public override bool Remove(TRowKey rowKey, TColKey colKey)
        #pragma warning restore 1591
        {
            return ParentMatrix.Remove(colKey, rowKey);
        }

        #pragma warning disable 1591
        public override TValue MissingValue
        #pragma warning restore 1591
        {
            get { return ParentMatrix.MissingValue; }
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(int rowIndex, int colIndex, TValue value)
#pragma warning restore 1591
        {
            ParentMatrix.SetValueOrMissing(colIndex, rowIndex, value);
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(TRowKey rowKey, TColKey colKey, TValue value)
#pragma warning restore 1591
        {
            ParentMatrix.SetValueOrMissing(colKey, rowKey, value);
        }
    }
}
