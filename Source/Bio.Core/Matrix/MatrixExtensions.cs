using System.Collections.Generic;
using System.IO;
using Bio.Util;

namespace Bio.Matrix
{
    /// <summary>
    /// Provides a set of static methods for Matrix objects.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Converts matrix to a DenseMatrix. If the inputMatrix is a DenseMatrix, then returns the given matrix without copying. The copy could, therefore,
        /// be either shallow or deep.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="inputMatrix">The matrix to convert.</param>
        /// <returns>A DenseMatrix with same rowKeys, colKeys, missing and nonmissing values, and special missing value. If the inputMatrix is a DenseMatrix, then it will be returned unchanged.</returns>
        public static DenseMatrix<TRowKey, TColKey, TValue> AsDenseMatrix<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> inputMatrix)
        {
            if (inputMatrix is DenseMatrix<TRowKey, TColKey, TValue>)
            {
                return (DenseMatrix<TRowKey, TColKey, TValue>)inputMatrix;
            }
            else
            {
                return ToDenseMatrix(inputMatrix);
            }
        }

        /// <summary>
        /// Converts matrix to a new DenseMatrix. Even if the inputMatrix is a DenseMatrix, a new DenseMatrix is created. The copy is, thus,
        /// always deep if TValue is an atomic type.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="inputMatrix">The matrix to convert.</param>
        /// <returns>A new DenseMatrix with same rowKeys, colKeys, missing and nonmissing values, and special missing value.</returns>
        public static DenseMatrix<TRowKey, TColKey, TValue> ToDenseMatrix<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> inputMatrix)
        {
            //Special code for materializing views

            //SelectRowsAndColsView
            var selectRowsAndColsView = inputMatrix as SelectRowsAndColsView<TRowKey, TColKey, TValue>;
            if (null != selectRowsAndColsView && selectRowsAndColsView.ParentMatrix is DenseMatrix<TRowKey, TColKey, TValue>)
            {
                return MaterializeSelectRowsColsViewToDenseMatrix(selectRowsAndColsView);
            }
            //Transpose
            var transposeView = inputMatrix as TransposeView<TRowKey, TColKey, TValue>;
            if (transposeView != null && transposeView.ParentMatrix is DenseMatrix<TColKey, TRowKey, TValue>)
            {
                return MaterializeTransposeViewToDenseMatrix(transposeView);
            }

            var denseMatrix = DenseMatrix<TRowKey, TColKey, TValue>.CreateDefaultInstance(inputMatrix.RowKeys, inputMatrix.ColKeys, inputMatrix.MissingValue);
            foreach (TRowKey rowKey in inputMatrix.RowKeys)
            {
                foreach (TColKey colKey in inputMatrix.ColKeys)
                {
                    denseMatrix.SetValueOrMissing(rowKey, colKey, inputMatrix.GetValueOrMissing(rowKey, colKey));
                }
            }
            return denseMatrix;
        }

        private static DenseMatrix<TRowKey, TColKey, TValue> MaterializeTransposeViewToDenseMatrix<TRowKey, TColKey, TValue>(TransposeView<TRowKey, TColKey, TValue> transposeView)
        {
            DenseMatrix<TColKey, TRowKey, TValue> parent = (DenseMatrix<TColKey, TRowKey, TValue>)transposeView.ParentMatrix;

            var matrix = new DenseMatrix<TRowKey, TColKey, TValue>();
            matrix._rowKeys = transposeView.RowKeys;
            matrix._colKeys = transposeView.ColKeys;
            matrix._indexOfRowKey = transposeView.IndexOfRowKey;
            matrix._indexOfColKey = transposeView.IndexOfColKey;
            matrix._missingValue = parent.MissingValue;
            matrix.ValueArray = new TValue[matrix.RowCount, matrix.ColCount];

            for (int newRowIndex = 0; newRowIndex < matrix.RowCount; ++newRowIndex)
            {
                for (int newColIndex = 0; newColIndex < matrix.ColCount; ++newColIndex)
                {
                    matrix.ValueArray[newRowIndex, newColIndex] = parent.ValueArray[newColIndex, newRowIndex];
                }
            }

            return matrix;
        }


        private static DenseMatrix<TRowKey, TColKey, TValue> MaterializeSelectRowsColsViewToDenseMatrix<TRowKey, TColKey, TValue>(SelectRowsAndColsView<TRowKey, TColKey, TValue> selectRowsAndColsView)
        {
            DenseMatrix<TRowKey, TColKey, TValue> parent = (DenseMatrix<TRowKey, TColKey, TValue>)selectRowsAndColsView.ParentMatrix;

            var matrix = new DenseMatrix<TRowKey, TColKey, TValue>();
            matrix._rowKeys = selectRowsAndColsView.RowKeys;
            matrix._colKeys = selectRowsAndColsView.ColKeys;
            matrix._indexOfRowKey = selectRowsAndColsView.IndexOfRowKey;
            matrix._indexOfColKey = selectRowsAndColsView.IndexOfColKey;
            matrix._missingValue = parent.MissingValue;
            matrix.ValueArray = new TValue[matrix.RowCount, matrix.ColCount];

            IList<int> oldRowIndexList = selectRowsAndColsView.IndexOfParentRowKey;
            IList<int> oldColIndexList = selectRowsAndColsView.IndexOfParentColKey;

            for (int newRowIndex = 0; newRowIndex < matrix.RowCount; ++newRowIndex)
            {
                int oldRowIndex = oldRowIndexList[newRowIndex];

                for (int newColIndex = 0; newColIndex < matrix.ColCount; ++newColIndex)
                {
                    matrix.ValueArray[newRowIndex, newColIndex] = parent.ValueArray[oldRowIndex, oldColIndexList[newColIndex]];
                }
            }

            return matrix;
        }


        /// <summary>
        /// Writes the matrix to a file in dense format. A directory will be created if needed.
        /// The first line is "var" TAB and then the tab-delimited col keys.
        /// Next is one line per row key. Each line is the row key TAB and then the tab-limited values.
        /// Values may include the special Missing value.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="filename">The filename to write to.</param>
        public static void WriteDense<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> matrix, string filename)
        {
            FileUtils.CreateDirectoryForFileIfNeeded(filename);
            using (TextWriter textWriter = File.CreateText(filename))
            {
                matrix.WriteDense(textWriter);
            }
        }

        /// <summary>
        /// Writes the matrix to a file in sparse format. A directory will be created if needed.
        /// The first line is "var" TAB "cid" TAB "val"
        /// Next is one line per nonmissing value. Each line is: rowKey TAB colKey TAB value
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="filename">The filename to write to.</param>
        public static void WriteSparse<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> matrix, string filename)
        {
             FileUtils.CreateDirectoryForFileIfNeeded(filename);
            using (TextWriter writer = File.CreateText(filename))
            {
                matrix.WriteSparse(writer);
            }
        }

        /// <summary>
        /// Writes the matrix to textWriter in sparse format.
        /// The first line is "var" TAB "cid" TAB "val"
        /// Next is one line per nonmissing value. Each line is: rowKey TAB colKey TAB value
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="textWriter">The textWriter to write to.</param>
        public static void WriteSparse<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> matrix, TextWriter textWriter)
        {
            textWriter.WriteLine("var\tcid\tval");
            foreach (RowKeyColKeyValue<TRowKey, TColKey, TValue> rowColVal in matrix.RowKeyColKeyValues)
            {
                textWriter.WriteLine(Helper.CreateTabString(rowColVal.RowKey, rowColVal.ColKey, rowColVal.Value));
            }
        }



    }
}
