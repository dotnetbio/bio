using System;
namespace Bio.Matrix
{

    /// <summary>
    /// An abstract wrapper used to store common code by several of the other matrix views.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    public abstract class AbstractMatrixView<TRowKey, TColKey, TValue> : Matrix<TRowKey, TColKey, TValue>
    {

        internal abstract void GetMatrixAndIndex(int rowIndex, int colIndex, out Matrix<TRowKey, TColKey, TValue> m, out int mappedRowIndex, out int mappedColIndex);
        internal abstract void GetMatrixAndKey(TRowKey rowKey, TColKey colKey, out Matrix<TRowKey, TColKey, TValue> m, out TRowKey mappedRowKey, out TColKey mappedColKey);


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
