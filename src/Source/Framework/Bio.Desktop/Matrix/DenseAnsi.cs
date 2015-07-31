using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bio.Util;
using Bio.Util.Logging;
using System.Globalization;

//!!!would be nice to have a TryGetInstance(filename) that didn't throw MatrixFormatException or any other exception

namespace Bio.Matrix
{
    /// <summary>
    /// A Matrix that presents values externally as a (16-bit) char and internally as a (8-bit) byte.
    /// For this class, the special Missing value must be '?'.
    /// </summary>
    public class DenseAnsi : DenseStructMatrix<byte, char>
    {
        private DenseAnsi()
        {
        }

#pragma warning disable 1591
        protected override byte ValueToStore(char value)
#pragma warning restore 1591
        {
            return (byte)value;
        }

        static internal byte StaticValueToStore(char value)
        {
            return (byte)value;
        }


#pragma warning disable 1591
        protected override char StoreToValue(byte store)
#pragma warning restore 1591
        {
            return (char)store;
        }

#pragma warning disable 1591
        protected override string StoreListToString(List<byte> storeList)
#pragma warning restore 1591
        {
            return StoreListToString(storeList, ColCount);
        }
        internal static string StoreListToString(List<byte> storeList, int colCount)
        {
            Helper.CheckCondition(storeList.Count == colCount, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedStoreListCountToEqualColCount, storeList.Count, colCount));
            StringBuilder sb = new StringBuilder(colCount);
            //05/18/2009 optimize: do on multiple threads?
            foreach (byte store in storeList)
            {
                sb.Append((char)store);
            }
            return sb.ToString();
        }

#pragma warning disable 1591
        protected override List<byte> StringToStoreList(string line)
#pragma warning restore 1591
        {
            return StringToStoreList(line, ColCount);
        }

#pragma warning disable 1591
        protected override byte SparseValToStore(string val)
#pragma warning restore 1591
        {
            Helper.CheckCondition(val.Length == 1, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ErrorConvertingSparseValToStore, val.Length));
            return (byte)val[0];
        }
        internal static string StoreToSparseVal(byte store)
        {
            return ((char)store).ToString();
        }

#pragma warning disable 1591
        protected override byte StoreMissingValue
#pragma warning restore 1591
        {
            get
            {
                return (byte)'?';
            }
        }

        /// <summary>
        /// Always '?'
        /// </summary>
        public override char MissingValue
        {
            get
            {
                return '?';
            }
        }

        /// <summary>
        /// Always '?'
        /// </summary>
        public static readonly char StaticMissingValue = '?';

        /// <summary>
        /// Always (byte)'?'
        /// </summary>
        public static readonly byte StaticStoreMissingValue = (byte)'?';

        private static IEnumerable<string[]> EachSparseLine(string filePattern,
            bool zeroIsOK, string fileMessageOrNull, CounterWithMessages counterWithMessages)
        {
            return DenseStructMatrix<byte, char>.EachSparseLine(filePattern, StringToStoreList, StoreToSparseVal,
                zeroIsOK, fileMessageOrNull, StaticStoreMissingValue, counterWithMessages);
        }

        private static List<byte> StringToStoreList(string line, int colCount)
        {
            if (line.Length != colCount)
            {
                throw new MatrixFormatException("Every data string should have one char per colKey.");
            }
            List<byte> storeList = new List<byte>(colCount);
            for (int i = 0; i < line.Length; ++i)
            {
                storeList.Add((byte)line[i]);
            }
            return storeList;
        }

        private string SequenceByColKey(string colKey)
        {
            return ColKeyToString(colKey, StoreListToString);
        }

        #region Factory Methods
        /// <summary>
        /// Creates an DenseAnsi with values missing.
        /// </summary>
        /// <param name="rowKeySequence">A sequence of row keys. The items will become the RowKeys of the Matrix.</param>
        /// <param name="colKeySequence">A sequence of colKeys. The items will come the ColKeys of the Matrix.</param>
        /// <param name="missingValue">The special Missing value, which must be '?'</param>
        /// <returns>A new, empty, DenseAnsi</returns>
        static public DenseAnsi CreateEmptyInstance(IEnumerable<string> rowKeySequence, IEnumerable<string> colKeySequence, char missingValue)
        {
            Helper.CheckCondition(missingValue.Equals(StaticMissingValue), "For DenseAnsi the missingValue must be '{0}'", StaticMissingValue); //OK to use Equals because char can't be null
            DenseAnsi denseAnsi = new DenseAnsi();
            denseAnsi.InternalCreateEmptyInstance(rowKeySequence, colKeySequence);
            return denseAnsi;
        }

        //!!! do all three of these methods really need to be public?

        /// <summary>
        /// Creates a Matrix, internally in DenseAnsi format, from file(s) in sparse format.
        /// </summary>
        /// <param name="inputSparsePattern">The name of a file (or a pattern matching several files). The file(s) are in sparse format.</param>
        /// <param name="matrix">The new matrix, or null if the method fails.</param>
        /// <returns>true if the file(s) is read and the Matrix is created; otherwise, false </returns>
        public static bool TryGetInstanceFromSparse(string inputSparsePattern, out Matrix<string, string, char> matrix)
        {
            DenseAnsi denseAnsi;
            bool b = TryGetInstanceFromSparse(inputSparsePattern, out denseAnsi);
            matrix = denseAnsi;
            return b;
        }

        /// <summary>
        /// Creates a DenseAnsi from file(s) in sparse format.
        /// </summary>
        /// <param name="inputSparsePattern">The name of a file (or a pattern matching several files). The file(s) are in sparse format.</param>
        /// <param name="denseAnsi">The new DenseAnsi (or null, if the method fails).</param>
        /// <returns>true if the file(s) is read and the DenseAnsi is created; otherwise, false </returns>
        public static bool TryGetInstanceFromSparse(string inputSparsePattern, out DenseAnsi denseAnsi)
        {
            try
            {
                denseAnsi = GetInstanceFromSparse(inputSparsePattern);
                return true;
            }
            catch
            {
                denseAnsi = null;
                return false;
            }
        }

        /// <summary>
        /// Parse a file in DenseAnsi format such that the values are returned as doubles.
        /// </summary>
        /// <param name="filename">The file to parse</param>
        /// <param name="missingValue">The special value that represents missing</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">The matrix created from parsing the file</param>
        /// <returns>True if the file can be parsed; false, otherwise.</returns>
        public static bool TryParseDenseAnsiFormatAsDoubleMatrix(string filename, double missingValue, ParallelOptions parallelOptions, out Matrix<string, string, double> matrix)
        {
            matrix = null;
            Matrix<string, string, char> sscMatrix;
            if (!DenseAnsi.TryGetInstance(filename, '?', parallelOptions, out sscMatrix))
                return false;
            matrix = sscMatrix.ConvertValueView(ValueConverter.CharToDouble, missingValue);

            return true;
        }

        /// <summary>
        /// Tries to read a file in DenseAnsi format and then creates a DenseAnsi class with values of the desired type.
        /// </summary>
        /// <typeparam name="TValue">The type of values wanted, for example, double</typeparam>
        /// <param name="filename">The dense ansi formatted file to read from.</param>
        /// <param name="missingValue">The special value that represents missing</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="matrix">A matrix that internally stores values in a DenseAnsi object.</param>
        /// <returns>true, if the file can be parsed; false, otherwise.</returns>
        public static bool TryParseDenseAnsiFormatAsGenericMatrix<TValue>(string filename, TValue missingValue, ParallelOptions parallelOptions, out Matrix<string, string, TValue> matrix)
        {
            matrix = null;
            Matrix<string, string, char> sscMatrix;
            if (!DenseAnsi.TryGetInstance(filename, '?', parallelOptions, out sscMatrix))
                return false;
            matrix = sscMatrix.ConvertValueView(new CharToGenericConverter<TValue>(), missingValue);

            return true;
        }

        /// <summary>
        /// Creates a DenseAnsi from file(s) in sparse format. Throws an exception if the class cannot be created.
        /// </summary>
        /// <param name="inputSparsePattern">The name of a file (or a pattern matching several files). The file(s) are in sparse format.</param>
        /// <returns>The new DenseAnsi.</returns>
        public static DenseAnsi GetInstanceFromSparse(string inputSparsePattern)
        {
            DenseAnsi denseAnsi = new DenseAnsi();
            denseAnsi.GetInstanceFromSparseInternal(inputSparsePattern);
            return denseAnsi;
        }

        // Tries to parse a DenseAnsi file as a DenseAnsi-backed matrix with a Converter to type TValue. This generic
        // version using CharToGeneric, which does conversion through reflection on a Parse method. This is not very efficient.
        internal static bool TryGetInstance<TValue1>(string filename, TValue1 missingValue, ParallelOptions parallelOptions, out Matrix<string, string, TValue1> matrix)
        {
            matrix = null;
            Matrix<string, string, char> sscMatrix;
            if (!DenseAnsi.TryGetInstance(filename, '?', parallelOptions, out sscMatrix))
                return false;
            matrix = sscMatrix.ConvertValueView(new CharToGenericConverter<TValue1>(), missingValue);
            return true;
        }

        internal static bool TryGetInstance(string filename, string missingValue, ParallelOptions parallelOptions, out Matrix<string, string, string> matrix)
        {
            matrix = null;
            Matrix<string, string, char> sscMatrix;
            if (!DenseAnsi.TryGetInstance(filename, '?', parallelOptions, out sscMatrix))
                return false;
            matrix = sscMatrix.ConvertValueView(ValueConverter.CharToString, missingValue);

            return true;
        }

        internal static bool TryGetInstance(string filename, int missingValue, ParallelOptions parallelOptions, out Matrix<string, string, int> matrix)
        {
            matrix = null;
            Matrix<string, string, char> sscMatrix;
            if (!DenseAnsi.TryGetInstance(filename, '?', parallelOptions, out sscMatrix))
                return false;
            matrix = sscMatrix.ConvertValueView(ValueConverter.CharToInt, missingValue);

            return true;
        }

        internal static bool TryGetInstance(string filename, double missingValue, ParallelOptions parallelOptions, out Matrix<string, string, double> matrix)
        {
            matrix = null;
            Matrix<string, string, char> sscMatrix;
            if (!DenseAnsi.TryGetInstance(filename, '?', parallelOptions, out sscMatrix))
                return false;
            matrix = sscMatrix.ConvertValueView(ValueConverter.CharToDouble, missingValue);

            return true;
        }

        /// <summary>
        /// This awkward method is provided for the sake of MatrixFactory. Right now it simply catches exceptions. Should switch and make it fail silently when doesn't work.
        /// </summary>
        public static bool TryGetInstance(string denseFileName, char mustProvide_StaticMissingValue_OrExceptionWillBeThrown, ParallelOptions parallelOptions, out Matrix<string, string, char> denseAnsi)
        {
            denseAnsi = null;
            if (!mustProvide_StaticMissingValue_OrExceptionWillBeThrown.Equals(StaticMissingValue)) //OK to use Equals because char can't be null
            {
                Console.Error.WriteLine("Missing value {0} doesn't match expected value {1}", StaticMissingValue, mustProvide_StaticMissingValue_OrExceptionWillBeThrown);
                return false;
            }
            try
            {
                denseAnsi = GetInstance(denseFileName, parallelOptions);
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Creates a DenseAnsi object from a file in dense ansi format.
        /// </summary>
        /// <param name="denseAnsiFileName">a file in dense ansi format</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>the DenseAnsi object</returns>
        public static DenseAnsi GetInstance(string denseAnsiFileName, ParallelOptions parallelOptions)
        {
            DenseAnsi denseAnsi = new DenseAnsi();
            denseAnsi.GetInstanceInternal(denseAnsiFileName, parallelOptions);
            return denseAnsi;
        }
        #endregion
        /// <summary>
        /// Create a DenseAnsi object from a sequence of RowKeyColKeyValue triples.
        /// </summary>
        /// <param name="tripleEnumerable">a sequence of RowKeyColKeyValue</param>
        /// <returns>A DenseAnsi object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static DenseAnsi GetInstanceFromSparse(IEnumerable<RowKeyColKeyValue<string, string, char>> tripleEnumerable)
        {
            DenseAnsi denseAnsi = new DenseAnsi();
            denseAnsi.GetInstanceFromSparseInternal(tripleEnumerable);
            return denseAnsi;
        }


        /// <summary>
        /// Write this DenseAnsi matrix to file.
        /// </summary>
        /// <param name="filename">The file to write to.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        public void WriteDenseAnsi(string filename, ParallelOptions parallelOptions)
        {
            Write(filename, parallelOptions);
        }

        /// <summary>
        /// Write this DenseAnsi matrix to a textWriter
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="verbose">ignored</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "verbose"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void WriteDenseAnsi(TextWriter textWriter, ParallelOptions parallelOptions, bool verbose = false)
        {
            Write(textWriter, parallelOptions);
        }
    }

    /// <summary>
    /// Extension methods on Matrix related to DenseAnsi.
    /// </summary>
    public static class DenseAnsiExtensions
    {
        /// <summary>
        /// Converts matrix to a DenseAnsi. If matrix is already a DenseAnsi, then returns the given matrix without copying. 
        /// </summary>
        /// <param name="inputMatrix">The matrix to convert from</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>A denseAnsi version of the matrix</returns>
        public static DenseAnsi AsDenseAnsi(this Matrix<string, string, char> inputMatrix, ParallelOptions parallelOptions)
        {
            if (inputMatrix is DenseAnsi)
            {
                return (DenseAnsi)inputMatrix;
            }
            else
            {
                return inputMatrix.ToDenseAnsi(parallelOptions);
            }
        }

        /// <summary>
        /// Converts matrix to a DenseAnsi. Even if the matrix is already an denseAnsi, a new one is created.. 
        /// </summary>
        /// <param name="matrix">The matrix to convert from</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>A denseAnsi version of the matrix</returns>
        public static DenseAnsi ToDenseAnsi(this Matrix<string, string, char> matrix, ParallelOptions parallelOptions)
        {
            var denseAnsi = DenseAnsi.CreateEmptyInstance(matrix.RowKeys, matrix.ColKeys, DenseAnsi.StaticMissingValue);

            //Console.WriteLine("Convert no more than {0} values", matrix.RowCount * matrix.ColCount);
            //CounterWithMessages counterWithMessages = new CounterWithMessages("adding value #{0}", 100000, null);
            Parallel.ForEach(matrix.RowKeyColKeyValues, parallelOptions, triple =>
            {
                //counterWithMessages.Increment();
                denseAnsi[triple.RowKey, triple.ColKey] = triple.Value;
            }
            );
            return denseAnsi;
        }

        /// <summary>
        /// Converts matrix to a DenseAnsi. If matrix is already a dense collection, then returns the given matrix without copying. 
        /// </summary>
        /// <param name="inputMatrix">The matrix to convert from</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>A denseAnsi version of the matrix</returns>
        public static DenseAnsi AsDenseAnsi<T>(this Matrix<string, string, T> inputMatrix, ParallelOptions parallelOptions)
        {
            return inputMatrix.ToDenseAnsi(parallelOptions);
        }

        /// <summary>
        /// Converts matrix to a DenseAnsi. Even if the matrix is already an denseAnsi, a new one is created.. 
        /// </summary>
        /// <param name="matrix">The matrix to convert from</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>A denseAnsi version of the matrix</returns>
        public static DenseAnsi ToDenseAnsi<T>(this Matrix<string, string, T> matrix, ParallelOptions parallelOptions)
        {
            var matrix2 = matrix.ConvertValueView(new CharToGenericConverter<T>().Inverted, '?');
            return matrix2.ToDenseAnsi(parallelOptions);
        }

        /// <summary>
        /// Writes a matrix in DenseAnsi format to a file. Converts the values of the matrix to char (on the fly). Does not need to convert to DenseAnsi format.
        /// </summary>
        /// <typeparam name="T">The type of the values of the matrix, for example, double</typeparam>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="filename">The file to write to</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        public static void WriteDenseAnsi<T>(this Matrix<string, string, T> matrix, string filename, ParallelOptions parallelOptions)
        {
            FileUtils.CreateDirectoryForFileIfNeeded(filename);
            using (TextWriter writer = File.CreateText(filename))
            {
                matrix.WriteDenseAnsi(writer, parallelOptions);
            }
        }

        //Similar code elsewhere, but hard to move static code to common location
        /// <summary>
        /// Writes a matrix in DenseAnsi format to a textWriter. Converts the values of the matrix to char (on the fly). Does not need to convert to DenseAnsi format.
        /// </summary>
        /// <typeparam name="T">The type of the values of the matrix, for example, double</typeparam>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="textWriter">The stream to write to</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="verbose">If true, may write out messages to the console telling how far along the writing is.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static void WriteDenseAnsi<T>(this Matrix<string, string, T> matrix, TextWriter textWriter, ParallelOptions parallelOptions, bool verbose = false)
        {
            Matrix<string, string, char> matrixInternal = matrix.ConvertValueView(ValueConverter.GetCharToGeneric<T>().Inverted, DenseAnsi.StaticMissingValue);
            matrixInternal.WriteDenseAnsi(textWriter, parallelOptions, verbose);
        }

        /// <summary>
        /// Writes a matrix with char values in DenseAnsi format to a textWriter. Does not need to convert to DenseAnsi format.
        /// </summary>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="textWriter">The stream to write to</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="verbose">If true, may write out messages to the console telling how far along the writing is.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "verbose"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parallelOptions"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static void WriteDenseAnsi(this Matrix<string, string, char> matrix, TextWriter textWriter, ParallelOptions parallelOptions, bool verbose = false)
        {

            textWriter.WriteLine("var\t{0}", matrix.ColKeys.StringJoin("\t"));
            foreach (string rowKey in matrix.RowKeys)
            {
                textWriter.Write(rowKey);
                textWriter.Write("\t");
                int rowIndex = matrix.IndexOfRowKey[rowKey];

                List<byte> storeList = new List<byte>(matrix.ColCount);
                for (int colIndex = 0; colIndex < matrix.ColCount; ++colIndex)
                {
                    char value;
                    byte store;
                    if (matrix.TryGetValue(rowIndex, colIndex, out value))
                    {
                        store = DenseAnsi.StaticValueToStore(value);
                    }
                    else
                    {
                        store = DenseAnsi.StaticStoreMissingValue;
                    }
                    storeList.Add(store);
                }

                Helper.CheckCondition(storeList.Count == matrix.ColCount, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedStoreListCountToEqualColCount, storeList.Count, matrix.ColCount));
                string s = DenseAnsi.StoreListToString(storeList, matrix.ColCount);
                textWriter.WriteLine(s);
            }
        }
    }
}
