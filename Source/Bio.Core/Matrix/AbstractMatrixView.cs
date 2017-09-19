using System;
using System.Diagnostics.CodeAnalysis;

namespace Bio.Matrix
{

    /// <summary>
    /// An abstract wrapper used to store common code by several of the other matrix views.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    [Serializable]
    public abstract class AbstractMatrixView<TRowKey, TColKey, TValue> : Matrix<TRowKey, TColKey, TValue>
    {

        /// <summary>
        /// Given an rowIndex and colIndex returns the appropriate parent matrix and its cooresponding rowIndex and colIndex.
        /// </summary>
        /// <param name="rowIndex">a rowIndex into the current matrix</param>
        /// <param name="colIndex">a colIndex into the current matrix</param>
        /// <param name="m">the appropriate parment matrix</param>
        /// <param name="mappedRowIndex">the cooresponding rowIndex in the parent</param>
        /// <param name="mappedColIndex">the corresponding colIndex in the parent</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#"), SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
        protected abstract void GetMatrixAndIndex(int rowIndex, int colIndex, out Matrix<TRowKey, TColKey, TValue> m, out int mappedRowIndex, out int mappedColIndex);

        /// <summary>
        /// Given an rowKey and colKey returns the appropriate parent matrix and its cooresponding rowKey and colKey.
        /// </summary>
        /// <param name="rowKey">a rowKey into the current matrix</param>
        /// <param name="colKey">a colKey into the current matrix</param>
        /// <param name="m">the appropriate parment matrix</param>
        /// <param name="mappedRowKey">the cooresponding rowKey in the parent</param>
        /// <param name="mappedColKey">the corresponding colKey in the parent</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
        protected abstract void GetMatrixAndKey(TRowKey rowKey, TColKey colKey, out Matrix<TRowKey, TColKey, TValue> m, out TRowKey mappedRowKey, out TColKey mappedColKey);


        #pragma warning disable 1591
        public override bool TryGetValue(TRowKey rowKey, TColKey colKey, out TValue value)
        #pragma warning restore 1591
        {
            Matrix<TRowKey, TColKey, TValue> m;
            TRowKey r;
            TColKey c;
            GetMatrixAndKey(rowKey, colKey, out m, out r, out c);

            return m.TryGetValue(r, c, out value);
        }

        #pragma warning disable 1591
        public override bool TryGetValue(int rowIndex, int colIndex, out TValue value)
        #pragma warning restore 1591
        {
            Matrix<TRowKey, TColKey, TValue> m;
            int r, c;

            GetMatrixAndIndex(rowIndex, colIndex, out m, out r, out c);
            
            return  m.TryGetValue(r, c, out value);
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(int rowIndex, int colIndex, TValue value)
#pragma warning restore 1591
        {
            Matrix<TRowKey, TColKey, TValue> m;
            int r, c;
            GetMatrixAndIndex(rowIndex, colIndex, out m, out r, out c);
            m.SetValueOrMissing(r, c, value);
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(TRowKey rowKey, TColKey colKey, TValue value)
#pragma warning restore 1591
        {
            Matrix<TRowKey, TColKey, TValue> m;
            TRowKey r;
            TColKey c;
            GetMatrixAndKey(rowKey, colKey, out m, out r, out c);
            m.SetValueOrMissing(r, c, value);
        }
    }
}
