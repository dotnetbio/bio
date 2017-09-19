using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Bio.Util;

namespace Bio.Matrix
{
    /// <summary>
    /// A matrix that internally represents only non-missing values. If most possible values are missing
    /// this saves memory. The trade off is that access is O(log(RowKeyCount)*log(ColKeyCount)) instead of being constant
    /// like the fastest dense methods.
    /// </summary>
    /// <typeparam name="TRowKey"></typeparam>
    /// <typeparam name="TColKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SparseMatrix<TRowKey, TColKey, TValue> : Matrix<TRowKey, TColKey, TValue>
    {
        internal Dictionary<TRowKey, Dictionary<TColKey, TValue>> _rowToColToVal;
        internal ReadOnlyCollection<TRowKey> _rowKeys;
        internal ReadOnlyCollection<TColKey> _colKeys;
        internal RestrictedAccessDictionary<TRowKey, int> _indexOfRowKey;
        internal RestrictedAccessDictionary<TColKey, int> _indexOfColKey;

        internal TValue _missingValue;


        #pragma warning disable 1591
        public override TValue MissingValue
        #pragma warning restore 1591
        {
            get { return _missingValue; }
        }

        #pragma warning disable 1591
        public override ReadOnlyCollection<TRowKey> RowKeys
        #pragma warning restore 1591
        {
            get
            {
                return _rowKeys;
            }
        }

        #pragma warning disable 1591
        public override ReadOnlyCollection<TColKey> ColKeys
        #pragma warning restore 1591
        {
            get
            {
                return _colKeys;
            }
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

        internal void InitializeIndexMaps()
        {
            var rowKeys = new Dictionary<TRowKey, int>(_rowKeys.Count);
            var colKeys = new Dictionary<TColKey, int>(_colKeys.Count);

            int rowIdx = 0;
            foreach (TRowKey rowKey in RowKeys)
            {
                rowKeys.Add(rowKey, rowIdx);
                rowIdx++;
            }

            int colIdx = 0;
            foreach (TColKey colKey in ColKeys)
            {
                colKeys.Add(colKey, colIdx);
                colIdx++;
            }
            _indexOfRowKey = rowKeys.AsRestrictedAccessDictionary();
            _indexOfColKey = colKeys.AsRestrictedAccessDictionary();
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(int rowIndex, int colIndex, TValue value)
#pragma warning restore 1591
        {
            SetValueOrMissing(RowKeys[rowIndex], ColKeys[colIndex], value);
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(TRowKey rowKey, TColKey colKey, TValue value)
#pragma warning restore 1591
        {
            Helper.CheckCondition<IndexOutOfRangeException>(ContainsRowKey(rowKey), "{0} is not a valid row key.", rowKey);
            Helper.CheckCondition<IndexOutOfRangeException>(ContainsColKey(colKey), "{0} is not a valid column key.", colKey);

            if (!IsMissing(value))
            {
                if (!_rowToColToVal.ContainsKey(rowKey))
                {
                    _rowToColToVal.Add(rowKey, new Dictionary<TColKey, TValue>());
                }
                _rowToColToVal[rowKey][colKey] = value;
            }
            else
            {
                if (_rowToColToVal.ContainsKey(rowKey))
                {
                    Dictionary<TColKey, TValue> row = _rowToColToVal[rowKey];
                    row.Remove(colKey);
                    if (row.Count == 0)
                    {
                        _rowToColToVal.Remove(rowKey);
                    }
                }
            }
        }

        internal SparseMatrix() { }



        /// <summary>
        /// Create SparseMatrix with all missing values.
        /// </summary>
        /// <param name="rowKeySequence">A sequence of row keys. The items will become the RowKeys of the Matrix.</param>
        /// <param name="colKeySequence">A sequence of colKeys. The items will come the ColKeys of the Matrix.</param>
        /// <param name="missingValue">The special value that represents missing</param>
        /// <returns>The empty SparseMatrix created.</returns>
        public static SparseMatrix<TRowKey, TColKey, TValue> CreateEmptyInstance(
            IEnumerable<TRowKey> rowKeySequence, IEnumerable<TColKey> colKeySequence, TValue missingValue)
        {
            var newSparse = new SparseMatrix<TRowKey, TColKey, TValue>();
            newSparse._missingValue = missingValue;
            newSparse._rowKeys = new ReadOnlyCollection<TRowKey>(new List<TRowKey>(rowKeySequence));
            newSparse._colKeys = new ReadOnlyCollection<TColKey>(new List<TColKey>(colKeySequence));
            newSparse._rowToColToVal = new Dictionary<TRowKey, Dictionary<TColKey, TValue>>(newSparse.RowCount);
            newSparse.InitializeIndexMaps();
            return newSparse;
        }



        /// <summary>
        /// Tries to parse a file in sparse format and creates a SpareMatrix
        /// </summary>
        /// <param name="filename">The sparse file</param>
        /// <param name="missingValue">The special value that represents missing in the SparseMatrix</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The resulting matrix</param>
        /// <returns>true, if the file can be parsed; false, otherwise.</returns>
        public static bool TryParseSparseFile(string filename, TValue missingValue,  ParallelOptions parallelOptions, out Matrix<TRowKey, TColKey, TValue> matrix)
        {
            using (TextReader textReader = FileUtils.OpenTextStripComments(filename))
            {
                return TryParseSparseFile(textReader, missingValue, parallelOptions, out matrix);
            }
        }

        /// <summary>
        /// Tries to parse a sparse textReader and creates a SpareMatrix
        /// </summary>
        /// <param name="textReader">The textReader stream in the sparse format</param>
        /// <param name="missingValue">The special value that represents missing in the SparseMatrix</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The resulting matrix</param>
        /// <returns>true, if the textReader can be parsed; false, otherwise.</returns>
        public static bool TryParseSparseFile(TextReader textReader, TValue missingValue,ParallelOptions parallelOptions, out Matrix<TRowKey, TColKey, TValue> matrix)
        {
            matrix = null;
            var variableToCaseIdToNonMissingValue = new Dictionary<TRowKey, Dictionary<TColKey, TValue>>();
            HashSet<TColKey> colKeys = new HashSet<TColKey>();

            string header = "var\tcid\tval";
            bool isFirst = true;
            foreach (string line in FileUtils.ReadEachLine(textReader))
            {
                if (isFirst)
                {
                    if (!line.Equals(header, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Console.Error.WriteLine("First line doesn't match expected header {0}. Start of first line was {1}.", header, line.Substring(0, Math.Min(15, line.Length)));
                        return false;
                    }
                    isFirst = false;
                }
                else
                {
                    string[] fields = line.Split('\t');
                    try
                    {
                        //!!! Use TryParse instead!!
                        TRowKey rowKey = Parser.Parse<TRowKey>(fields[0]);
                        TColKey colKey = Parser.Parse<TColKey>(fields[1]);
                        TValue value = Parser.Parse<TValue>(fields[2]);

                        variableToCaseIdToNonMissingValue.GetValueOrDefault(rowKey)[colKey] = value;
                        colKeys.Add(colKey);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                        return false;
                    }
                }
            }

            var sparseMatrix = new SparseMatrix<TRowKey, TColKey, TValue>();
            sparseMatrix._missingValue = missingValue;
            sparseMatrix._rowToColToVal = variableToCaseIdToNonMissingValue;
            sparseMatrix._rowKeys = new ReadOnlyCollection<TRowKey>(new List<TRowKey>(variableToCaseIdToNonMissingValue.Keys));
            sparseMatrix._colKeys = new ReadOnlyCollection<TColKey>(new List<TColKey>(colKeys));
            sparseMatrix.InitializeIndexMaps();

            matrix = sparseMatrix;

            return true;
        }

        #pragma warning disable 1591
        public override bool IsMissing(int rowIndex, int colIndex)
        #pragma warning restore 1591
        {
            return IsMissing(RowKeys[rowIndex], ColKeys[colIndex]);
        }

        #pragma warning disable 1591
        public override bool IsMissing(TRowKey rowKey, TColKey colKey)
        #pragma warning restore 1591
        {
            //First we need to confirm that these are valid keys
            if (!ContainsRowKey(rowKey) || !ContainsColKey(colKey))
            {
                throw new IndexOutOfRangeException();
            }

            //This works because _rowToColToVal is a dictionary of dictionaries.
            return !(_rowToColToVal.ContainsKey(rowKey) && _rowToColToVal[rowKey].ContainsKey(colKey));
        }


        #pragma warning disable 1591
        public override bool TryGetValue(TRowKey rowKey, TColKey colKey, out TValue value)
        #pragma warning restore 1591
        {
            if (IsMissing(rowKey, colKey))
            {
                value = MissingValue;
                return false;
            }
            else
            {
                value = _rowToColToVal[rowKey][colKey];
                return true;
            }
        }

        #pragma warning disable 1591
        public override bool TryGetValue(int rowIndex, int colIndex, out TValue value)
        #pragma warning restore 1591
        {
            return TryGetValue(RowKeys[rowIndex], ColKeys[colIndex], out value);
        }


    }

    /// <summary>
    /// Extension methods on Matrix related to DenseAnsi.
    /// </summary>
    public static class SparseMatrixExtensions
    {
        /// <summary>
        /// Converts matrix to a SparseMatrix. If matrix is already a SparseMatrix, then returns the given matrix without copying. 
        /// </summary>
        /// <param name="inputMatrix">The matrix to convert from</param>
        /// <returns>A SparseMatrix version of the matrix</returns>
        public static SparseMatrix<TRowKey, TColKey, TValue> AsSparseMatrix<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> inputMatrix)
        {
            if (inputMatrix is SparseMatrix<TRowKey, TColKey, TValue>)
            {
                return (SparseMatrix<TRowKey, TColKey, TValue>)inputMatrix;
            }
            else
            {
                return ToSparseMatrix(inputMatrix);
            }
        }

        /// <summary>
        /// Converts matrix to a SparseMatrix. Even if the matrix is already an SparseMatrix, a new one is created.. 
        /// </summary>
        /// <param name="matrix">The matrix to convert from</param>
        /// <returns>A SparseMatrix version of the matrix</returns>
        public static SparseMatrix<TRowKey, TColKey, TValue> ToSparseMatrix<TRowKey, TColKey, TValue>(this Matrix<TRowKey, TColKey, TValue> matrix)
        {

            var sparseMatrix = SparseMatrix<TRowKey, TColKey, TValue>.CreateEmptyInstance(matrix.RowKeys, matrix.ColKeys, matrix.MissingValue);
            foreach (RowKeyColKeyValue<TRowKey, TColKey, TValue> triple in matrix.RowKeyColKeyValues)
            {
                sparseMatrix[triple.RowKey, triple.ColKey] = matrix[triple.RowKey, triple.ColKey];
            }
            return sparseMatrix;
        }
    }
}
