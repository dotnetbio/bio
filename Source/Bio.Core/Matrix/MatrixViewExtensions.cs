using System;
using System.Collections.Generic;
using System.Linq;
using Bio.Util;

namespace Bio.Matrix
{
    /// <summary>
    /// Provides a set of static methods for creating view on a Matrix. A view is a light-weight wrapper around a matrix such
    /// that value changes to either the matrix or its view(s) will be reflected in both.
    /// </summary>
    public static class MatrixViewExtensions
    {
        #region SelectRowAndColsView

        /// <summary>
        /// Create a view of a parent matrix with a subset of the rows (perhaps in a different order) and
        /// a subset of the cols (perhaps in a different order). If the subsets happen to include all
        /// rows and cols in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="rowKeySequence">A sequence of rowKeys that specifies the subset of rows to include and their desired order.</param>
        /// <param name="colKeySequence">A sequence of colKeys that specifies the subset of cols to include and their desired order.</param>
        /// <returns>A matrix with the desired rows and cols in their desired order.</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectRowsAndColsView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<TRowKey> rowKeySequence, IEnumerable<TColKey> colKeySequence)
        {
            //If the new won't change anything, just return the parent
            if (parentMatrix.RowKeys.SequenceEqual(rowKeySequence) && parentMatrix.ColKeys.SequenceEqual(colKeySequence))
            {
                return parentMatrix;
            }

            //!!!Could check of this is a SelectRowsAndColsView of a SelectRowsAndColsView and simplify (see TransposeView for an example)

            var matrixView = new SelectRowsAndColsView<TRowKey, TColKey, TValue>();
            matrixView.SetUp(parentMatrix, rowKeySequence, colKeySequence);
            return matrixView;
        }

        /// <summary>
        /// Create a view of a parent matrix with a subset of the rows (perhaps in a different order).
        /// If the subset happens to include all rows in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="rowKeySequence">A sequence of rowKeys that specifies the subset of rows to include and their desired order.</param>
        /// <returns>A matrix with the desired rows in their desired order.</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectRowsView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix,
            IEnumerable<TRowKey> rowKeySequence)
        {
            return parentMatrix.SelectRowsAndColsView(rowKeySequence, parentMatrix.ColKeys);
        }

        /// <summary>
        /// Create a view of a parent matrix with a subset of the cols (perhaps in a different order).
        /// If the subset happens to include all
        /// cols in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="colKeySequence">A sequence of colKeys that specifies the subset of cols to include and their desired order.</param>
        /// <returns>A matrix with the desired cols in their desired order.</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectColsView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix,
            IEnumerable<TColKey> colKeySequence)
        {
            return parentMatrix.SelectRowsAndColsView(parentMatrix.RowKeys, colKeySequence);
        }


        /// <summary>
        /// Create a view of a parent matrix with a subset of the rows (perhaps in a different order) and
        /// a subset of the cols (perhaps in a different order). If the subsets happen to include all
        /// rows and cols in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="rowIndexEnumerable">A sequence of row indexes that specifies the subset of rows to include and their desired order.</param>
        /// <param name="colIndexSequence">A sequence of col indexes that specifies the subset of cols to include and their desired order.</param>
        /// <returns>A matrix with the desired rows and cols in their desired order.</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectRowsAndColsView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<int> rowIndexEnumerable, IEnumerable<int> colIndexSequence)
        {
            return parentMatrix.SelectRowsAndColsView(
                rowIndexEnumerable.Select(rowIndex => parentMatrix.RowKeys[rowIndex]),
                colIndexSequence.Select(colIndex => parentMatrix.ColKeys[colIndex]));
        }

        /// <summary>
        /// Create a view of a parent matrix with a subset of the rows (perhaps in a different order).
        /// If the subsets happen to include all rows in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="rowIndexEnumerable">A sequence of row indexes that specifies the subset of rows to include and their desired order.</param>
        /// <returns>A matrix with the desired rows and cols in their desired order.</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectRowsView<TRowKey, TColKey, TValue>(
            this Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<int> rowIndexEnumerable)
        {
            return parentMatrix.SelectRowsAndColsView(
                rowIndexEnumerable.Select(rowIndex => parentMatrix.RowKeys[rowIndex]),
                parentMatrix.ColKeys);
        }

        /// <summary>
        /// Create a view of a parent matrix with a subset of the cols (perhaps in a different order).
        /// If the subsets happen to include all cols in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="colIndexSequence">A sequence of col indexes that specifies the subset of cols to include and their desired order.</param>
        /// <returns>A matrix with the desired cols in their desired order.</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectColsView<TRowKey, TColKey, TValue>(
            this Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<int> colIndexSequence)
        {
            return parentMatrix.SelectRowsAndColsView(
                parentMatrix.RowKeys,
                colIndexSequence.Select(colIndex => parentMatrix.ColKeys[colIndex]));
        }

        /// <summary>
        /// Create a view of a parent matrix with a subset of the rows (perhaps in a different order).
        /// If the subset happens to include all rows in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="rowKeyParams">a rowKey(s) that specifies the subset of row(s) to include (and their desired order).</param>
        /// <returns>A matrix with the desired row(s in their desired order).</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectRowsView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, params TRowKey[] rowKeyParams)
        {
            return SelectRowsView(parentMatrix, (IEnumerable<TRowKey>)rowKeyParams);
        }

        /// <summary>
        /// Create a view of a parent matrix with a subset of the rows (perhaps in a different order).
        /// If the subset happens to include all rows in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="rowIndexParams">a row index(es) that specifies the subset of row(s) to include (and their desired order).</param>
        /// <returns>A matrix with the desired row(s in their desired order).</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectRowsView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, params int[] rowIndexParams)
        {
            return SelectRowsView(parentMatrix, (IEnumerable<int>)rowIndexParams);
        }

        /// <summary>
        /// Create a view of a parent matrix with a subset of the cols (perhaps in a different order).
        /// If the subset happens to include all cols in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="colKeyParams">a colKey(s) that specifies the subset of col(s) to include (and their desired order).</param>
        /// <returns>A matrix with the desired col(s in their desired order).</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectColsView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, params TColKey[] colKeyParams)
        {
            return SelectColsView(parentMatrix, (IEnumerable<TColKey>)colKeyParams);
        }

        /// <summary>
        /// Create a view of a parent matrix with a subset of the cols (perhaps in a different order).
        /// If the subset happens to include all cols in the same order, the parent matrix is returned.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="colIndexParams">a col index(es) that specifies the subset of col(s) to include (and their desired order).</param>
        /// <returns>A matrix with the desired col(s in their desired order).</returns>
        static public Matrix<TRowKey, TColKey, TValue> SelectColsView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, params int[] colIndexParams)
        {
            return SelectColsView(parentMatrix, (IEnumerable<int>)colIndexParams);
        }
        #endregion

        /// <summary>
        /// Creates a view of a parent matrix that converts values. For example, if the parent has values '0' ... '9'
        /// of type character and doubles are needed, this view can convert the characters to double when reading.
        /// When double values are assigned, they would be converted to characters and stored in the parent matrix.
        /// If a conversion is impossible,an exception is raised.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <remarks>If two layers of ConvertValueView are applied to a parent matrix, such that they cancel each other out,
        /// the parent matrix is returned.</remarks>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValueParent">The type of the parent's value, for example, char</typeparam>
        /// <typeparam name="TValueView">The type of the wrapper's value, for example, double</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="converter">A conversion instance, for example, ValueConvertion{char,int}.CharToInt</param>
        /// <param name="missingValue">The special Missing value for the wrapping matrix.</param>
        /// <returns>A matrix with values of the desired type</returns>
        static public Matrix<TRowKey, TColKey, TValueView> ConvertValueView<TRowKey, TColKey, TValueParent, TValueView>(
            this Matrix<TRowKey, TColKey, TValueParent> parentMatrix, ValueConverter<TValueParent, TValueView> converter, TValueView missingValue)
        {
            // catch the case where we are converting back to the original matrix
            var asConvertValueView = parentMatrix as ConvertValueView<TRowKey, TColKey, TValueParent, TValueView>;
            if (asConvertValueView != null &&
                asConvertValueView.ViewValueToParentValue.Equals(converter.ConvertForward) &&
                asConvertValueView.ParentValueToViewValue.Equals(converter.ConvertBackward))
            {
                return asConvertValueView.ParentMatrix;
            }

            var matrixView = new ConvertValueView<TRowKey, TColKey, TValueView, TValueParent>();
            matrixView.SetUp(parentMatrix, converter, missingValue);
            return matrixView;
        }

        /// <summary>
        /// Create a view of the parent matrix in which rows become cols and cols become rows.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <remarks>If two layers of TransposeView are applied to a parent matrix, the parent matrix is returned.</remarks>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <returns>A transposed matrix</returns>
        static public Matrix<TColKey, TRowKey, TValue> TransposeView<TRowKey, TColKey, TValue>(
            this Matrix<TRowKey, TColKey, TValue> parentMatrix)
        {
            //If this is a transpose of a transpose, simplify
            TransposeView<TRowKey, TColKey, TValue> parentTransposeViewOrNull = parentMatrix as TransposeView<TRowKey, TColKey, TValue>;
            if (null != parentTransposeViewOrNull)
            {
                return parentTransposeViewOrNull.ParentMatrix;
            }

            var transposeView = new TransposeView<TColKey, TRowKey, TValue>();
            transposeView.ParentMatrix = parentMatrix;
            return transposeView;
        }

        /// <summary>
        /// Create a new view of the parent matrix in which the columns are renamed
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="newKeyAndOldKeySequence">A sequence of pair mapping from new keys to the old keys, for example, a dictionary
        /// More than one new key can map to an old key.
        /// or a list of KeyValuePair's. Any parent column with a colKey that isn't mentioned will be inaccessable.
        /// The new cols will be in the order of given in the sequence.</param>
        /// <remarks>If newKeyAndOldKeySequence maps every key to itself, in the same order, the parent matrix will be returned.</remarks>
        /// <returns>A matrix with renamed columns.</returns>
        static public Matrix<TRowKey, TColKey, TValue> RenameColsView<TRowKey, TColKey, TValue>(
            this Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<KeyValuePair<TColKey, TColKey>> newKeyAndOldKeySequence)
        {
            if (newKeyAndOldKeySequence.All(kvp => kvp.Key.Equals(kvp.Value)) && newKeyAndOldKeySequence.Select(pair => pair.Key).SequenceEqual(parentMatrix.ColKeys))
            {
                return parentMatrix;
            }
            return new RenameColsView<TRowKey, TColKey, TValue>(parentMatrix, newKeyAndOldKeySequence);
        }

        /// <summary>
        /// Create a new view of the parent matrix in which the rows are renamed.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="newKeyAndOldKeySequence">A sequence of pair mapping from new keys to the old keys, for example, a dictionary
        /// More than one new key can map to an old key.
        /// or a list of KeyValuePair's. Any parent row with a rowKey that isn't mentioned will be inaccessable.
        /// The new rows will be in the order of given in the sequence.</param>
        /// <remarks>If newKeyAndOldKeySequence maps every key to itself, in the same order, the parent matrix will be returned.</remarks>
        /// <returns>A matrix with renamed rows.</returns>
        static public Matrix<TRowKey, TColKey, TValue> RenameRowsView<TRowKey, TColKey, TValue>(
            this Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<KeyValuePair<TRowKey, TRowKey>> newKeyAndOldKeySequence)
        {
            return parentMatrix.TransposeView().RenameColsView(newKeyAndOldKeySequence).TransposeView();
        }

        /// <summary>
        /// Creates a new view in which the rows of the parent matrix is merged with the rows of the otherMatrices. The rows will be in the order
        /// of the input matrices. If two matrices contain a rows with the same rowKey, an exception is thrown. All matricies must have the same
        /// MissingValue.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="colsMustMatch">true, to require all matrices to have the same colKeys in the same order; false, use an intersection of the
        /// colKeys in the order of parentMatrix.ColKeys.</param>
        /// <param name="otherMatrices">zero or more other matricies with which to concatinate rows.</param>
        /// <remarks>If no other matrices are given, returns the parent matrix.</remarks>
        /// <returns>A matrix containing the rows of parentMatrix and otherMatrices.</returns>
        static public Matrix<TRowKey, TColKey, TValue> MergeRowsView<TRowKey, TColKey, TValue>(
            this Matrix<TRowKey, TColKey, TValue> parentMatrix, bool colsMustMatch, params Matrix<TRowKey, TColKey, TValue>[] otherMatrices)
        {
            if (otherMatrices.Length == 0)
            {
                return parentMatrix;
            }
            return new MergeRowsView<TRowKey, TColKey, TValue>(colsMustMatch, (new List<Matrix<TRowKey, TColKey, TValue>> { parentMatrix }).Concat(otherMatrices).ToArray());
        }

        /// <summary>
        /// Creates a new view in which the cols of the parent matrix is merged with the cols of the otherMatrices. The cols will be in the order
        /// of the input matrices. If two matrices contain a cols with the same colKey, an exception is thrown. All matricies must have the same
        /// MissingValue.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="rowsMustMatch">true, to require all matrices to have the same rowKeys in the same order; false, use an intersection of the
        /// rowKeys in the order of parentMatrix.RowKeys.</param>
        /// <param name="otherMatrices">zero or more other matricies with which to concatinate cols.</param>
        /// <remarks>If no other matrices are given, returns the parent matrix.</remarks>
        /// <returns>A matrix containing the cols of parentMatrix and otherMatrices.</returns>
        public static Matrix<TRowKey, TColKey, TValue> MergeColsView<TRowKey, TColKey, TValue>(
            this Matrix<TRowKey, TColKey, TValue> parentMatrix, bool rowsMustMatch, params Matrix<TRowKey, TColKey, TValue>[] otherMatrices)
        {
            if (otherMatrices.Length == 0)
            {
                return parentMatrix;
            }
            return new MergeColsView<TRowKey, TColKey, TValue>(rowsMustMatch, (new List<Matrix<TRowKey, TColKey, TValue>> { parentMatrix }).Concat(otherMatrices).ToArray());
        }


        /// <summary>
        /// Creates a new view in which the columns of the matrix (but not the column keys) are permuted.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="colKeySequence">The colKeys of the columns in their new order. Every colKey must be mentioned exactly once.</param>
        /// <remarks>If the permutation puts every column back in the same place, the parent matrix is returned.</remarks>
        /// <returns>A new matrix with permuted columns.</returns>
        static public Matrix<TRowKey, TColKey, TValue> PermuteColValuesForEachRowView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<TColKey> colKeySequence)
        {
            return parentMatrix.PermuteColValuesForEachRowView(
                colKeySequence.Select(colKey => parentMatrix.IndexOfColKey[colKey]));
        }

        /// <summary>
        /// Creates a new view in which the columns of the matrix (but not the column keys) are permuted.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="colIndexSequence">The indexes of the columns in their new order. Every col index must be mentioned exactly once.</param>
        /// <remarks>If the permutation puts every column back in the same place, the parent matrix is returned.</remarks>
        /// <returns>A new matrix with permuted columns.</returns>
        static public Matrix<TRowKey, TColKey, TValue> PermuteColValuesForEachRowView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<int> colIndexSequence)
        {
            //If the new won't change anything, just return the parent
            if (colIndexSequence.SequenceEqual(Enumerable.Range(0, parentMatrix.ColCount)))
            {
                return parentMatrix;
            }

            //If this is a permutation of a permutation, simplify
            PermuteValuesView<TRowKey, TColKey, TValue> parentPermuteValuesViewOrNull = parentMatrix as PermuteValuesView<TRowKey, TColKey, TValue>;
            if (null != parentPermuteValuesViewOrNull)
            {
                var colIndexSequenceTwo = colIndexSequence.Select(i => parentPermuteValuesViewOrNull.IndexOfParentCol[i]);
                var matrixView = new PermuteValuesView<TRowKey, TColKey, TValue>();
                matrixView.SetUp(parentPermuteValuesViewOrNull.ParentMatrix, colIndexSequenceTwo);
                return matrixView;
            }
            else
            {
                var matrixView = new PermuteValuesView<TRowKey, TColKey, TValue>();
                matrixView.SetUp(parentMatrix, colIndexSequence);
                return matrixView;
            }
        }


        /// <summary>
        /// Creates a new view in which the columns of the matrix (but not the column keys) are permuted.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="random">a Random object from which a permutation is drawn</param>
        /// <remarks>If the permutation puts every column back in the same place, the parent matrix is returned.</remarks>
        /// <returns>A new matrix with permuted columns.</returns>
        static public Matrix<TRowKey, TColKey, TValue> PermuteColValuesForEachRowView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, ref Random random)
        {
            return PermuteColValuesForEachRowView(parentMatrix, Enumerable.Range(0, parentMatrix.ColCount).Shuffle(random));
        }


        /// <summary>
        /// Creates a new view in which the rows of the matrix (but not the row keys) are permuted.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="rowIndexSequence">The indexes of the rows in their new order. Every row index must be mentioned exactly once.</param>
        /// <remarks>If the permutation puts every row back in the same place, the parent matrix is returned.</remarks>
        /// <returns>A new matrix with permuted rows.</returns>
        static public Matrix<TRowKey, TColKey, TValue> PermuteRowValuesForEachColView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, IEnumerable<int> rowIndexSequence)
        {
            return parentMatrix.TransposeView().PermuteColValuesForEachRowView(rowIndexSequence).TransposeView();
        }


        /// <summary>
        /// Creates a new view in which the rows of the matrix (but not the row keys) are permuted.
        /// This is a 'view' in the sense that changes to the values in either matrix will be reflected in both.
        /// </summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <param name="random">a Random object from which a permutation is drawn</param>
        /// <remarks>If the permutation puts every row back in the same place, the parent matrix is returned.</remarks>
        /// <returns>A new matrix with permuted rows.</returns>
        static public Matrix<TRowKey, TColKey, TValue> PermuteRowValuesForEachColView<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> parentMatrix, ref Random random)
        {
            return PermuteRowValuesForEachColView(parentMatrix, Enumerable.Range(0, parentMatrix.RowCount).Shuffle(random));
        }



        /// <summary>
        /// Creates a view of the parent matrix which is hashable. For example, it can be used as the key of a dictionary.
        /// This is a 'view' in the sense that no copying an every little extra memory is used.
        /// 
        /// Two HashableView matricies are equal if their RowKeys and ColKeys (in order), MissingValue, and values are equal.
        /// They will have the same hashcode if they are equal.
        /// 
        /// The hashcode is computed only once when the the HashableView is contructed, so a HashableView does not allow
        /// its values to be changed. Also, changing values of the parent will give unexpected results.
        /// 
        /// When used by Dictionary or HashSet, a full call of MatrixEquals (which looks at every value) is needed to confirm
        /// that two matrices with the same hashcode are really equal.
        ///</summary>
        /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
        /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
        /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
        /// <param name="parentMatrix">The matrix to wrap.</param>
        /// <returns>A hashable matrix</returns>
        static public Matrix<TRowKey, TColKey, TValue> HashableView<TRowKey, TColKey, TValue>(
            this Matrix<TRowKey, TColKey, TValue> parentMatrix)
        {
            //If this is a HashableView of a HashableView, simplify
            HashableView<TRowKey, TColKey, TValue> parentHashableViewOrNull = parentMatrix as HashableView<TRowKey, TColKey, TValue>;
            if (null != parentHashableViewOrNull)
            {
                return parentHashableViewOrNull.ParentMatrix;
            }

            var hashableView = new HashableView<TRowKey, TColKey, TValue>();
            hashableView.ParentMatrix = parentMatrix;
            return hashableView;
        }
    }

}
