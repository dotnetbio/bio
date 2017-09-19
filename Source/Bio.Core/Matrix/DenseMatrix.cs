using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Bio.IO;
using Bio.Util;
using System.Globalization;

namespace Bio.Matrix
{
    /// <summary>
    /// A Matrix that stores the values internally in a .NET 2-D array.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    [Serializable]
    public class DenseMatrix<TRowKey, TColKey, TValue> : Matrix<TRowKey, TColKey, TValue>
    {

        internal DenseMatrix()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DenseMatrix class that wraps a .NET 2-D array
        /// </summary>
        /// <param name="valueArray">The 2-D .NET array to wrap.</param>
        /// <param name="rowKeySequence">A sequence of row keys. The items will become the RowKeys of the Matrix.</param>
        /// <param name="colKeySequence">A sequence of colKeys. The items will come the ColKeys of the Matrix.</param>
        /// <param name="missingValue">The special value that represents missing.</param>
        public DenseMatrix(ref TValue[,] valueArray, IEnumerable<TRowKey> rowKeySequence, IEnumerable<TColKey> colKeySequence, TValue missingValue)
        {
            _rowKeys = new ReadOnlyCollection<TRowKey>(rowKeySequence.ToList());
            _colKeys = new ReadOnlyCollection<TColKey>(colKeySequence.ToList());
            var valueArray2 = valueArray;
            Helper.CheckCondition(valueArray.GetLength(0) == _rowKeys.Count, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedRowKeysCountToEqualValueArrayCount, _rowKeys.Count, valueArray2.GetLength(0)));
            Helper.CheckCondition(valueArray.GetLength(1) == _colKeys.Count, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedColumnKeysCountToEqualValueArrayCount, _colKeys.Count, valueArray2.GetLength(1)));

            //!!!Matrix - these lines appear in many places
            _indexOfRowKey = RowKeys.Select((key, index) => new { key, index }).ToDictionary(pair => pair.key, pair => pair.index);
            _indexOfColKey = ColKeys.Select((key, index) => new { key, index }).ToDictionary(pair => pair.key, pair => pair.index);

            ValueArray = valueArray;
            _missingValue = missingValue;
        }

        /// <summary>
        /// Initializes a new instance of the DenseMatrix class that wraps a .NET 2-D array
        /// </summary>
        /// <param name="valueArray">The 2-D .NET array to wrap.</param>
        /// <param name="rowKeySequence">A sequence of row keys. The items will become the RowKeys of the Matrix.</param>
        /// <param name="colKeySequence">A sequence of colKeys. The items will come the ColKeys of the Matrix.</param>
        /// <param name="missingValue">The special value that represents missing.</param>
        public DenseMatrix(TValue[,] valueArray, IEnumerable<TRowKey> rowKeySequence, IEnumerable<TColKey> colKeySequence, TValue missingValue)
            : this(ref valueArray, rowKeySequence, colKeySequence, missingValue)
        {
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(TRowKey rowKey, TColKey colKey, TValue value)
#pragma warning restore 1591
        {
            SetValueOrMissing(IndexOfRowKey[rowKey], IndexOfColKey[colKey], value);
        }

        #region IMatrix<TRowKey,TColKey,TValue> Members

        internal ReadOnlyCollection<TRowKey> _rowKeys;
#pragma warning disable 1591
        override public ReadOnlyCollection<TRowKey> RowKeys { get { return _rowKeys; } }//return new ReadOnlyCollection<TRowKey>(_rowKeys); } }
#pragma warning restore 1591

        internal ReadOnlyCollection<TColKey> _colKeys;
#pragma warning disable 1591
        override public ReadOnlyCollection<TColKey> ColKeys { get { return _colKeys; } }
#pragma warning restore 1591

        internal IDictionary<TRowKey, int> _indexOfRowKey;
#pragma warning disable 1591
        override public IDictionary<TRowKey, int> IndexOfRowKey { get { return _indexOfRowKey.AsRestrictedAccessDictionary(); } }
#pragma warning restore 1591

        internal IDictionary<TColKey, int> _indexOfColKey;
#pragma warning disable 1591
        override public IDictionary<TColKey, int> IndexOfColKey { get { return _indexOfColKey.AsRestrictedAccessDictionary(); } }
#pragma warning restore 1591

        /// <summary>
        /// The 2-D .NET array used to store values.
        /// </summary>
        public TValue[,] ValueArray;

#pragma warning disable 1591
        public override void SetValueOrMissing(int rowIndex, int colIndex, TValue value)
#pragma warning restore 1591
        {
            ValueArray[rowIndex, colIndex] = value;
        }

        internal TValue _missingValue;
#pragma warning disable 1591
        override public TValue MissingValue
#pragma warning restore 1591
        {
            get { return _missingValue; }
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Create a DenseMatrix from a file in RFile format.
        /// </summary>
        /// <param name="rFileName">a file in RFile format</param>
        /// <param name="missingValue">The special value that represents 'missing'</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The DenseMatrix created</param>
        /// <returns>True if the file is parsable; otherwise false</returns>
        public static bool TryParseRFileWithDefaultMissing(string rFileName, TValue missingValue, ParallelOptions parallelOptions, out Matrix<TRowKey, TColKey, TValue> matrix)
        {
            string errorMsg;
            bool result = TryParseRFileWithDefaultMissing(rFileName, missingValue, parallelOptions, out matrix, out errorMsg);
            if (!string.IsNullOrEmpty(errorMsg))
                Console.Error.WriteLine(errorMsg);
            return result;
        }

        /// <summary>
        /// Create a DenseMatrix from a file in RFile format.
        /// </summary>
        /// <param name="rFileName">a file in RFile format with tab delimited columns</param>
        /// <param name="missingValue">The special value that represents 'missing'</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The DenseMatrix created</param>
        /// <returns>True if the file is parsable; otherwise false</returns>
        public static bool TryParseTabbedRFileWithDefaultMissing(string rFileName, TValue missingValue, ParallelOptions parallelOptions, out Matrix<TRowKey, TColKey, TValue> matrix)
        {
            string errorMsg;
            return TryParseRFileWithDefaultMissing(rFileName, missingValue, new char[] { '\t' }, parallelOptions, out matrix, out errorMsg);
        }

        /// <summary>
        /// Create a DenseMatrix from a file in RFile format.
        /// </summary>
        /// <param name="rFileName">a file in RFile format with space or tab delimited columns</param>
        /// <param name="missingValue">The special value that represents 'missing'</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The DenseMatrix created</param>
        /// <param name="errorMsg">If the file is not parsable, an error message about the problem.</param>
        /// <returns>True if the file is parsable; otherwise false</returns>
        public static bool TryParseRFileWithDefaultMissing(string rFileName, TValue missingValue, ParallelOptions parallelOptions,
            out Matrix<TRowKey, TColKey, TValue> matrix, out string errorMsg)
        {
            return TryParseRFileWithDefaultMissing(rFileName, missingValue, new char[] { ' ', '\t' }, parallelOptions, out matrix, out errorMsg);
        }

        /// <summary>
        /// Create a DenseMatrix from a file in RFile format.
        /// </summary>
        /// <param name="rFileName">a file in RFile format with tab delimited columns</param>
        /// <param name="missingValue">The special value that represents 'missing'</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="result">The DenseMatrix created</param>
        /// <param name="errorMsg">If the file is not parsable, an error message about the problem.</param>
        /// <returns>True if the file is parsable; otherwise false</returns>
        public static bool TryParseTabbedRFileWithDefaultMissing(string rFileName, TValue missingValue, ParallelOptions parallelOptions,
            out Matrix<TRowKey, TColKey, TValue> result, out string errorMsg)
        {
            return TryParseRFileWithDefaultMissing(rFileName, missingValue, new char[] { '\t' }, parallelOptions, out result, out errorMsg);
        }

        /// <summary>
        /// Create a DenseMatrix from a file in RFile format.
        /// </summary>
        /// <param name="rFileName">a file in RFile format with delimited columns</param>
        /// <param name="missingValue">The special value that represents 'missing'</param>
        /// <param name="separatorArray">An array of character delimiters</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="result">The DenseMatrix created</param>
        /// <param name="errorMsg">If the file is not parsable, an error message about the problem.</param>
        /// <returns>True if the file is parsable; otherwise false</returns>
        public static bool TryParseRFileWithDefaultMissing(string rFileName, TValue missingValue,
            char[] separatorArray, ParallelOptions parallelOptions, out Matrix<TRowKey, TColKey, TValue> result, out string errorMsg)
        {
            return TryParseRFileWithDefaultMissing(rFileName.ToNamedStreamCreatorFromFileName(), missingValue, separatorArray, parallelOptions, out result, out errorMsg);
        }


        /// <summary>
        /// Create a DenseMatrix from a file in RFile format.
        /// </summary>
        /// <param name="namedStreamCreator">a namedStreamCreator with delimited columns</param>
        /// <param name="missingValue">The special value that represents 'missing'</param>
        /// <param name="separatorArray">An array of character delimiters</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="result">The DenseMatrix created</param>
        /// <param name="errorMsg">If the file is not parsable, an error message about the problem.</param>
        /// <returns>True if the file is parsable; otherwise false</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parallelOptions"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#")]
        public static bool TryParseRFileWithDefaultMissing(INamedStreamCreator namedStreamCreator, TValue missingValue,
            char[] separatorArray, ParallelOptions parallelOptions, out Matrix<TRowKey, TColKey, TValue> result, out string errorMsg)
        {
            if (namedStreamCreator == null)
            {
                throw new ArgumentNullException("namedStreamCreator");
            }

            errorMsg = "";
            var matrix = new DenseMatrix<TRowKey, TColKey, TValue>();
            result = matrix;
            matrix._missingValue = missingValue;

            int rowCount = namedStreamCreator.ReadEachUncommentedLine().Count();


            using (var textReader = namedStreamCreator.OpenUncommentedText())
            {
                string firstLine = textReader.ReadLine();
                //Helper.CheckCondition(null != firstLine, "Expect file to have first line. ");
                if (null == firstLine)
                {
                    errorMsg = "Expect file to have first line. ";
                    return false;
                }
                Debug.Assert(rowCount >= 0); // real assert

                List<string> unparsedRowNames = new List<string>(rowCount);
                List<string> unparsedColNames = firstLine.Split(separatorArray).ToList();
                try
                {
                    matrix.ValueArray = new TValue[rowCount, unparsedColNames.Count];
                }
                catch (Exception e)
                {
                    errorMsg = e.Message;
                    return false;
                }
                string line;
                int rowIndex = -1;

                //while (null != (line = textReader.ReadLine()))
                while (!string.IsNullOrEmpty(line = textReader.ReadLine()))
                {
                    ++rowIndex;
                    string[] fields = line.Split(separatorArray);
                    //Helper.CheckCondition(fields.Length >= 1, string.Format("Expect each line to have at least one field (file={0}, rowIndex={1})", rFileName, rowIndex));
                    if (fields.Length < 2)
                    {
                        errorMsg = string.Format(CultureInfo.InvariantCulture, "Expect each line to have at least two field (file={0}, rowIndex={1})", namedStreamCreator.Name, rowIndex);
                        return false;
                    }

                    string rowKey = fields[0];
                    unparsedRowNames.Add(rowKey);

                    // if the first data row has same length as header row, then header row much contain a name for the column of row names. Remove it and proceed.
                    if (rowIndex == 0 && fields.Length == unparsedColNames.Count)
                    {
                        unparsedColNames.RemoveAt(0);
                    }

                    //Helper.CheckCondition(fields.Length == matrix.ColKeys.Count + 1, string.Format("Line has {0} fields instead of the epxected {1} fields (file={2}, rowKey={3}, rowIndex={4})", fields.Length, matrix.ColKeys.Count + 1, rFileName, rowKey, rowIndex));
                    if (fields.Length != unparsedColNames.Count + 1)
                    {
                        errorMsg = string.Format(CultureInfo.InvariantCulture, "Line has {0} fields instead of the expected {1} fields (file={2}, rowKey={3}, rowIndex={4})", fields.Length, unparsedColNames.Count + 1, namedStreamCreator.Name, rowKey, rowIndex);
                        return false;
                    }

                    //for (int colIndex = 0; colIndex < matrix.ValueArray.GetLength(0); ++colIndex)
                    for (int colIndex = 0; colIndex < unparsedColNames.Count; ++colIndex)
                    {
                        TValue r;
                        if (!Parser.TryParse<TValue>(fields[colIndex + 1], out r))
                        {
                            errorMsg = string.Format(CultureInfo.InvariantCulture, "Unable to parse {0} because field {1} cannot be parsed into an instance of type {2}", namedStreamCreator.Name, fields[colIndex + 1], typeof(TValue));
                            return false;
                        }
                        matrix.ValueArray[rowIndex, colIndex] = r;
                    }
                }

                IList<TRowKey> rowKeys;
                if (!Parser.TryParseAll<TRowKey>(unparsedRowNames, out rowKeys))
                {
                    errorMsg = string.Format(CultureInfo.InvariantCulture, "Unable to parse {0} because row names cannot be parsed into an instance of type {1}", namedStreamCreator.Name, typeof(TRowKey));
                    return false;
                }
                IList<TColKey> colKeys;
                if (!Parser.TryParseAll<TColKey>(unparsedColNames, out colKeys))
                {
                    errorMsg = string.Format(CultureInfo.InvariantCulture, "Unable to parse {0} because col names cannot be parsed into an instance of type {1}", namedStreamCreator.Name, typeof(TColKey));
                    return false;
                }
                matrix._rowKeys = new ReadOnlyCollection<TRowKey>(rowKeys);
                matrix._colKeys = new ReadOnlyCollection<TColKey>(colKeys);
            }

            //In the case of sparse files, many of the row keys will be the same and so we return false
            if (matrix._rowKeys.Count != matrix._rowKeys.Distinct().Count())
            {
                errorMsg = string.Format(CultureInfo.InvariantCulture, "Some rows have the same values as other (look for blank rows). " + namedStreamCreator.Name);
                return false;
            }


            matrix._indexOfRowKey = matrix.RowKeys.Select((key, index) => new { key, index }).ToDictionary(pair => pair.key, pair => pair.index);
            matrix._indexOfColKey = matrix.ColKeys.Select((key, index) => new { key, index }).ToDictionary(pair => pair.key, pair => pair.index);

            return true;
        }

        /// <summary>
        /// Initalizes a new instance of the DenseMatrix class filled with the default value of TValue. For example, if TValue is double,
        /// the matrix will be filled with 0.0's.
        /// </summary>
        /// <param name="rowKeySequence">A sequence of row keys. The items will become the RowKeys of the Matrix.</param>
        /// <param name="colKeySequence">A sequence of colKeys. The items will come the ColKeys of the Matrix.</param>
        /// <param name="missingValue">The special Missing value.</param>
        /// <returns>A new instance of DenseMatrix</returns>
        public static DenseMatrix<TRowKey, TColKey, TValue> CreateDefaultInstance(IEnumerable<TRowKey> rowKeySequence, IEnumerable<TColKey> colKeySequence, TValue missingValue)
        {
            var matrix = new DenseMatrix<TRowKey, TColKey, TValue>();
            matrix._rowKeys = new ReadOnlyCollection<TRowKey>(rowKeySequence.ToList());
            matrix._colKeys = new ReadOnlyCollection<TColKey>(colKeySequence.ToList());
            try
            {
                matrix._indexOfRowKey = matrix.RowKeys.Select((key, index) => new { key, index }).ToDictionary(pair => pair.key, pair => pair.index);
                matrix._indexOfColKey = matrix.ColKeys.Select((key, index) => new { key, index }).ToDictionary(pair => pair.key, pair => pair.index);
            }
            catch (ArgumentException)
            {
                var rowSet = matrix.RowKeys.ToHashSet();
                if (rowSet.Count != matrix.RowKeys.Count)
                {
                    var set = new HashSet<TRowKey>();
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The following row keys are duplicates: {0}", matrix.RowKeys.Where(key => !set.Add(key)).StringJoin(",")));
                }
                var colSet = matrix.ColKeys.ToHashSet();
                if (colSet.Count != matrix.ColKeys.Count)
                {
                    var set = new HashSet<TColKey>();
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The following col keys are duplicates: {0}", matrix.ColKeys.Where(key => !set.Add(key)).StringJoin(",")));
                }
                throw;
            }
            matrix._missingValue = missingValue;
            matrix.ValueArray = new TValue[matrix.RowCount, matrix.ColCount];
            return matrix;
        }

        #endregion

#pragma warning disable 1591
        override public bool TryGetValue(int rowIndex, int colIndex, out TValue value)
#pragma warning restore 1591
        {
            value = ValueArray[rowIndex, colIndex];
            return !IsMissing(value);
        }

#pragma warning disable 1591
        public override bool TryGetValue(TRowKey rowKey, TColKey colKey, out TValue value)
#pragma warning restore 1591
        {
            return TryGetValue(IndexOfRowKey[rowKey], IndexOfColKey[colKey], out value);
        }

#pragma warning disable 1591
        public override bool Remove(TRowKey rowKey, TColKey colKey)
#pragma warning restore 1591
        {
            return Remove(IndexOfRowKey[rowKey], IndexOfColKey[colKey]);
        }

#pragma warning disable 1591
        public override bool Remove(int rowIndex, int colIndex)
#pragma warning restore 1591
        {
            TValue oldvalue = ValueArray[rowIndex, colIndex];
            ValueArray[rowIndex, colIndex] = MissingValue;
            return !IsMissing(oldvalue);
        }
    }
}
