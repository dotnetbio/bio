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
    /// A matrix object that reads double out of a file, as needed, rather than keeping them in memory.
    /// </summary>
    public class PaddedDouble : DenseStructMatrix<double, double>
    {
        /// <summary>
        /// The number of ANSI characters in a file used to write out a double in text format.
        /// </summary>
        static public readonly int BytesPerValue = 1 + double.MinValue.ToString().Length;
        /// <summary>
        /// The format string used to write doubles into ANSI text.
        /// </summary>
        static public readonly string FormatString = "{0," + BytesPerValue.ToString() + "}";

        private PaddedDouble()
        {
        }

#pragma warning disable 1591
        protected override double ValueToStore(double value)
#pragma warning restore 1591
        {
            return value;
        }

#pragma warning disable 1591
        protected override double StoreToValue(double store)
#pragma warning restore 1591
        {
            return store;
        }

#pragma warning disable 1591
        protected override string StoreListToString(List<double> storeList)
#pragma warning restore 1591
        {
            return StoreListToString(storeList, ColCount);
        }
        internal static string StoreListToString(List<double> storeList, int colCount)
        {
            Helper.CheckCondition(storeList.Count == colCount, () => Properties.Resource.ExpectedOneValueForEveryColKey);
            StringBuilder sb = new StringBuilder(colCount);
            //05/18/2009 optimize: do on multiple threads?
            foreach (double store in storeList)
            {
                sb.AppendFormat(FormatString, store.ToString());
            }
            return sb.ToString();
        }

#pragma warning disable 1591
        protected override List<double> StringToStoreList(string line)
#pragma warning restore 1591
        {
            return StringToStoreList(line, ColCount);
        }



#pragma warning disable 1591
        protected override double SparseValToStore(string val)
#pragma warning restore 1591
        {
            Helper.CheckCondition(val.Length == 1, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedValToBeSingleCharacter, val));
            return double.Parse(val);
        }

        /// <summary>
        /// A method that converts a double into a string suitable for writing to a file.
        /// </summary>
        /// <param name="store">The double</param>
        /// <returns>the string</returns>
        public static string StoreToSparseVal(double store)
        {
            return string.Format(FormatString, store.ToString()); //We format twice. Once from double to string and then from string to padded string
        }

#pragma warning disable 1591
        protected override double StoreMissingValue
#pragma warning restore 1591
        {
            get
            {
                return double.NaN;
            }
        }

#pragma warning disable 1591
        public override double MissingValue
#pragma warning restore 1591
        {
            get
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// The special value used to represent missing values. Always double.NaN.
        /// </summary>
        public static readonly double StaticMissingValue = double.NaN;
        /// <summary>
        /// The special value used to represent missing values internally. Always double.NaN.
        /// </summary>
        public static readonly double StaticStoreMissingValue = double.NaN;



        /// <summary>
        /// Create an empty instance of a PaddedDouble file
        /// </summary>
        /// <param name="rowKeySequence">A sequence of row keys. The items will become the RowKeys of the Matrix.</param>
        /// <param name="colKeySequence">A sequence of colKeys. The items will come the ColKeys of the Matrix.</param>
        /// <param name="missingValue">The special value that represents missing</param>
        /// <returns>An empty PaddedDouble instance</returns>
        static public PaddedDouble CreateEmptyInstance(IEnumerable<string> rowKeySequence, IEnumerable<string> colKeySequence, double missingValue)
        {
            Helper.CheckCondition(missingValue.Equals(StaticMissingValue), "For PaddedDouble the missingValue must be '{0}'", StaticMissingValue); //OK to use Equals because double can't be null
            PaddedDouble paddedDouble = new PaddedDouble();
            paddedDouble.InternalCreateEmptyInstance(rowKeySequence, colKeySequence);
            return paddedDouble;
        }


        /// <summary>
        /// Create an instance of PaddedDouble from a sparse input file
        /// </summary>
        /// <param name="inputSparsePattern">The sparse input file</param>
        /// <param name="matrix">The PaddedDouble matrix created</param>
        /// <returns>true if the file parses as PaddedDouble; otherwise, false</returns>
        public static bool TryGetInstanceFromSparse(string inputSparsePattern, out Matrix<string, string, double> matrix)
        {
            PaddedDouble paddedDouble;
            bool b = TryGetInstanceFromSparse(inputSparsePattern, out paddedDouble);
            matrix = paddedDouble;
            return b;
        }

        /// <summary>
        /// Create an instance of PaddedDouble from a sparse input file
        /// </summary>
        /// <param name="inputSparsePattern">The sparse input file</param>
        /// <param name="paddedDouble">The PaddedDouble matrix created</param>
        /// <returns>true if the file parses as PaddedDouble; otherwise, false</returns>
        public static bool TryGetInstanceFromSparse(string inputSparsePattern, out PaddedDouble paddedDouble)
        {
            try
            {
                paddedDouble = GetInstanceFromSparse(inputSparsePattern);
                return true;
            }
            catch
            {
                paddedDouble = null;
                return false;
            }
        }

        /// <summary>
        /// Create an instance of PaddedDouble from a sparse input file
        /// </summary>
        /// <param name="inputSparsePattern">The sparse input file</param>
        /// <returns>A PaddedDouble</returns>
        public static PaddedDouble GetInstanceFromSparse(string inputSparsePattern)
        {
            PaddedDouble paddedDouble = new PaddedDouble();
            paddedDouble.GetInstanceFromSparseInternal(inputSparsePattern);
            return paddedDouble;
        }


        /// <summary>
        /// Create a PaddedDouble object from a sequence of RowKeyColKeyValue triples.
        /// </summary>
        /// <param name="tripleEnumerable">a sequence of RowKeyColKeyValue</param>
        /// <returns>A PaddedDouble object</returns>
        public static PaddedDouble GetInstanceFromSparse(IEnumerable<RowKeyColKeyValue<string, string, double>> tripleEnumerable)
        {
            PaddedDouble paddedDouble = new PaddedDouble();
            paddedDouble.GetInstanceFromSparseInternal(tripleEnumerable);
            return paddedDouble;
        }


        /// <summary>
        /// This awkward method is provided for the sake of MatrixFactory. Right now it simply catches exceptions. Should switch and make it fail silently when doesn't work.
        /// </summary>
        public static bool TryGetInstance(string denseFileName, double mustProvide_StaticMissingValue_OrExceptionWillBeThrown, ParallelOptions parallelOptions, out Matrix<string, string, double> paddedDouble)
        {
            paddedDouble = null;
            if (!mustProvide_StaticMissingValue_OrExceptionWillBeThrown.Equals(StaticMissingValue)) //OK to use Equals because double can't be null
            {
                Console.Error.WriteLine("Missing value {0} doesn't match expected value {1}", StaticMissingValue, mustProvide_StaticMissingValue_OrExceptionWillBeThrown);
                return false;
            }

            try
            {
                paddedDouble = GetInstance(denseFileName, parallelOptions);
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }



        /// <summary>
        /// Creates an instance of PaddedDouble from a file in PaddedDouble format.
        /// </summary>
        /// <param name="paddedDoubleFileName">a file in PaddedDouble format</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>The created PaddedDouble</returns>
        public static PaddedDouble GetInstance(string paddedDoubleFileName, ParallelOptions parallelOptions)
        {
            PaddedDouble paddedDouble = new PaddedDouble();
            paddedDouble.GetInstanceInternal(paddedDoubleFileName, parallelOptions);
            return paddedDouble;
        }


        //!!!seems too specific
        /// <summary>
        /// Returns the contents of a files in PaddedDouble format as a sequence of string arrays in sparse file format.
        /// Saves memory by never creating a PaddedDouble instance.
        /// </summary>
        /// <param name="filePattern">Files in PaddedDouble format</param>
        /// <param name="zeroIsOK">tells if it's OK if not files match parts of the file pattern.</param>
        /// <param name="fileMessageOrNull">A string containing '{0}' to write as each file is opened.</param>
        /// <param name="counterWithMessages">Send status messages to standard output</param>
        /// <returns>A sequence of string arrays. Each string array has three values: the var, the cid, and the val.</returns>
        public static IEnumerable<string[]> EachSparseLine(string filePattern,
            bool zeroIsOK, string fileMessageOrNull, CounterWithMessages counterWithMessages)
        {
            return DenseStructMatrix<double, double>.EachSparseLine(filePattern, StringToStoreList, StoreToSparseVal,
                zeroIsOK, fileMessageOrNull, StaticStoreMissingValue, counterWithMessages);
        }

        static List<double> StringToStoreList(string line, int colCount)
        {
            if (line.Length != colCount * BytesPerValue)
            {
                throw new MatrixFormatException(string.Format("Every data string should have {0} chars per colKey.", BytesPerValue));
            }
            List<double> storeList = new List<double>(colCount);
            for (int i = 0; i < line.Length; i += BytesPerValue)
            {
                storeList.Add(double.Parse(line.Substring(i, BytesPerValue)));
            }
            return storeList;
        }

    }


    /// <summary>
    /// Extension methods on Matrix related to PaddedDouble.
    /// </summary>
    public static class PaddedDoubleExtensions
    {
        /// <summary>
        /// Converts matrix to a PaddedDouble. If matrix is already a dense collection, then returns the given matrix without copying. 
        /// </summary>
        /// <param name="inputMatrix">The matrix to convert from</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>A paddedDouble version of the matrix</returns>
        public static PaddedDouble AsPaddedDouble(this Matrix<string, string, double> inputMatrix, ParallelOptions parallelOptions)
        {
            if (inputMatrix is PaddedDouble)
            {
                return (PaddedDouble)inputMatrix;
            }
            else
            {
                return ToPaddedDouble(inputMatrix, parallelOptions);
            }
        }

        /// <summary>
        /// Converts matrix to a PaddedDouble. Even if the matrix is already an paddedDouble, a new one is created.. 
        /// </summary>
        /// <param name="matrix">The matrix to convert from</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>A paddedDouble version of the matrix</returns>
        public static PaddedDouble ToPaddedDouble(this Matrix<string, string, double> matrix, ParallelOptions parallelOptions)
        {
            var paddedDouble = PaddedDouble.CreateEmptyInstance(matrix.RowKeys, matrix.ColKeys, PaddedDouble.StaticMissingValue);

            //Console.WriteLine("Convert no more than {0} values", matrix.RowCount * matrix.ColCount);
            //CounterWithMessages counterWithMessages = new CounterWithMessages("adding value #{0}", 100000, null);
            Parallel.ForEach(matrix.RowKeyColKeyValues, parallelOptions, triple =>
            {
                //counterWithMessages.Increment();
                paddedDouble[triple.RowKey, triple.ColKey] = triple.Value;
            });
            return paddedDouble;
        }



        /// <summary>
        /// Write a matrix in PaddedDouble file format
        /// </summary>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="filename">The file to write to</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        public static void WritePaddedDouble(this Matrix<string, string, double> matrix, string filename, ParallelOptions parallelOptions)
        {
            FileUtils.CreateDirectoryForFileIfNeeded(filename);
            using (TextWriter writer = File.CreateText(filename))
            {
                matrix.WritePaddedDouble(writer, parallelOptions);
            }
        }

        //Similar code elsewhere, but hard to move static code to common location
        //Would be nice if could work from, say Matrix<string,string,int>

        /// <summary>
        /// Write in PaddedDouble file format to a TextWriter
        /// </summary>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="textWriter">The TextWriter to write to</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        public static void WritePaddedDouble(this Matrix<string, string, double> matrix, TextWriter textWriter, ParallelOptions parallelOptions)
        {
            textWriter.WriteLine("var\t{0}", matrix.ColKeys.StringJoin("\t"));
            foreach (string rowKey in matrix.RowKeys)
            {
                textWriter.Write(rowKey);
                textWriter.Write("\t");
                int rowIndex = matrix.IndexOfRowKey[rowKey];

                List<double> storeList = new List<double>(matrix.ColCount);
                for (int colIndex = 0; colIndex < matrix.ColCount; ++colIndex)
                {
                    double store;
                    if (!matrix.TryGetValue(rowIndex, colIndex, out store))
                    {
                        store = PaddedDouble.StaticStoreMissingValue;
                    }
                    storeList.Add(store);
                }
                Helper.CheckCondition(storeList.Count == matrix.ColCount, () => Properties.Resource.ExpectedOneValueForEveryColKey);
                string s = PaddedDouble.StoreListToString(storeList, matrix.ColCount);
                textWriter.WriteLine(s);
            }
        }


    }

}

