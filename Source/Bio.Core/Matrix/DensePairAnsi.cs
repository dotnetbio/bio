using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bio.Util;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace Bio.Matrix
{

    /// <summary>
    /// A Matrix that presents values externally as two sorted (16-bit) chars and internally as two bytes.
    /// For this class, the special Missing value must be the pair '?','?'.
    /// </summary>
    public class DensePairAnsi : DenseStructMatrix<UOPair<byte>, UOPair<char>>
    {
        internal DensePairAnsi()
        {
        }

#pragma warning disable 1591
        protected override UOPair<byte> ValueToStore(UOPair<char> value)
#pragma warning restore 1591
        {
            return new UOPair<byte>((byte)value.First, (byte)value.Second);
        }

        internal static UOPair<byte> StaticValueToStore(UOPair<char> value)
        {
            return new UOPair<byte>((byte)value.First, (byte)value.Second);
        }

#pragma warning disable 1591
        protected override UOPair<char> StoreToValue(UOPair<byte> store)
#pragma warning restore 1591
        {
            return new UOPair<char>((char)store.First, (char)store.Second);
        }

#pragma warning disable 1591
        protected override string StoreListToString(List<UOPair<byte>> storeList)
#pragma warning restore 1591
        {
            return StoreListToString(storeList, ColCount);
        }

        internal static string StoreListToString(List<UOPair<byte>> storeList, int colCount)
        {
            Helper.CheckCondition(storeList.Count == colCount, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedStoreListCountToEqualColCount, storeList.Count, colCount));
            StringBuilder sb = new StringBuilder(colCount * 2);
            //05/18/2009 optimize: do on multiple threads?
            foreach (UOPair<byte> store in storeList)
            {
                sb.Append((char)store.First);
                sb.Append((char)store.Second);
            }
            return sb.ToString();
        }

#pragma warning disable 1591
        protected override List<UOPair<byte>> StringToStoreList(string line)
#pragma warning restore 1591
        {
            if (line.Length != ColCount * 2)
            {
                new MatrixFormatException("Every data string should have two chars per colKey.");
            }
            List<UOPair<byte>> storeList = new List<UOPair<byte>>(ColCount);
            //05/18/2009 optimize: do on multiple threads?
            for (int i = 0; i < ColCount; ++i)
            {
                if (line[i + i].CompareTo(line[i + i + 1]) == 1)
                {
                    new MatrixFormatException("The char pairs in the densePairAnsi file must be sorted");
                }
                storeList.Add(new UOPair<byte>((byte)line[i + i], (byte)line[i + i + 1]));

            }
            return storeList;
        }

#pragma warning disable 1591
        protected override UOPair<byte> SparseValToStore(string val)
#pragma warning restore 1591
        {
            Helper.CheckCondition(val.Length == 2, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedValToContainTwoCharacters, val.Length));
            if (val[0].CompareTo(val[1]) == 1)
            {
                new MatrixFormatException("The char pairs in the sparse file must be sorted. " + val);
            }
            UOPair<byte> store = new UOPair<byte>((byte)val[0], (byte)val[1]);
            return store;
        }

        /// <summary>
        /// For DensePairAnsi the missing value is always the same: ?,?. This is a static version of that missing value.
        /// </summary>
        public static readonly UOPair<char> StaticMissingValue = new UOPair<char>('?', '?');
#pragma warning disable 1591
        override public UOPair<char> MissingValue
#pragma warning restore 1591
        {
            get { return StaticMissingValue; }
        }

        /// <summary>
        /// For DensePairAnsi the missing value is always the same: ?,?. This is a static version of that missing value.
        /// </summary>
        public static readonly UOPair<byte> StaticStoreMissingValue = new UOPair<byte>((byte)'?', (byte)'?');
#pragma warning disable 1591
        protected override UOPair<byte> StoreMissingValue
#pragma warning restore 1591
        {
            get { return StaticStoreMissingValue; }
        }

        /// <summary>
        /// Creates an DensePairAnsi with values missing.
        /// </summary>
        /// <param name="rowKeySequence">A sequence of row keys. The items will become the RowKeys of the Matrix.</param>
        /// <param name="colKeySequence">A sequence of colKeys. The items will come the ColKeys of the Matrix.</param>
        /// <param name="missingValue">The special Missing value, which must be (UO '?','?')</param>
        /// <returns>A new, empty, DensePairAnsi</returns>
        static public DensePairAnsi CreateEmptyInstance(IEnumerable<string> rowKeySequence, IEnumerable<string> colKeySequence, UOPair<char> missingValue)
        {
            //OK to use Equals because UOPair<char> can't be null.
            Helper.CheckCondition(missingValue.Equals(StaticMissingValue), () => Properties.Resource.DensePairAnsiMissingValueSignatureMustBe);
            DensePairAnsi densePairAnsi = new DensePairAnsi();
            densePairAnsi.InternalCreateEmptyInstance(rowKeySequence, colKeySequence);
            return densePairAnsi;
        }

        /// <summary>
        /// Parses directly from the SparseFile format. Useful if memory must be concerved.
        /// </summary>
        /// <param name="inputSparseFileName">Name of a file in Sparse format.</param>
        /// <returns>An instance of DensePairAnsi.</returns>
        public static DensePairAnsi GetInstanceFromSparse(string inputSparseFileName)
        {
            DensePairAnsi densePairAnsi = new DensePairAnsi();
            densePairAnsi.GetInstanceFromSparseInternal(inputSparseFileName);
            return densePairAnsi;
        }

        /// <summary>
        /// Create a DensePairAnsi object from a sequence of RowKeyColKeyValue triples.
        /// </summary>
        /// <param name="tripleEnumerable">a sequence of RowKeyColKeyValue</param>
        /// <returns>A DensePairAnsi object</returns>
        public static DensePairAnsi GetInstanceFromSparse(IEnumerable<RowKeyColKeyValue<string, string, UOPair<char>>> tripleEnumerable)
        {
            DensePairAnsi densePairAnsi = new DensePairAnsi();
            densePairAnsi.GetInstanceFromSparseInternal(tripleEnumerable);
            return densePairAnsi;
        }

        internal static void MergeDensePairAnsiFiles(string inputDensePairAnsiPattern, string outputDensePairAnsiFileName, byte[] map, ParallelOptions parallelOptions)
        {

            List<string> colKeyList = new List<string>();

            List<Dictionary<string, string>> rowKeyToLineList = new List<Dictionary<string, string>>();
            foreach (string fileName in FileUtils.GetFiles(inputDensePairAnsiPattern, /*zeroIsOK*/ false))
            {
                Console.WriteLine(Properties.Resource.ProgressStatus_Reading, fileName);

                Dictionary<string, string> rowKeyToLine = new Dictionary<string, string>();
                rowKeyToLineList.Add(rowKeyToLine);

                Predicate<string> keepDelegate = OnlyKeepKeysThatHaveAppearedBefore(rowKeyToLineList);

                using (TextReader textReader = File.OpenText(fileName))
                {
                    string header = textReader.ReadLine();
                    Helper.CheckCondition(null != header, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedFileToHaveHeader, fileName));
                    int colKeyCountBefore = colKeyList.Count;
                    string[] headerFields = header.Split('\t');
                    colKeyList.AddRange(headerFields.Skip(1));
                    Helper.CheckCondition(colKeyCountBefore + headerFields.Length - 1 == colKeyList.Count, () => Properties.Resource.ExpectedNoOverlapBetweenRowKeys);

                    //CounterWithMessages counterWithMessages = new CounterWithMessages("reading line {0}", 10000, null);
                    string line;
                    while (null != (line = textReader.ReadLine()))
                    {
                        //counterWithMessages.Increment();
                        string[] fields = line.Split('\t');
                        Helper.CheckCondition(fields.Length == 2, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedTwoFieldsFoundN, fields.Length));
                        if (keepDelegate(fields[0]))
                        {
                            rowKeyToLine.Add(fields[0], fields[1]);
                        }
                    }
                }
            }

            FileUtils.CreateDirectoryForFileIfNeeded(outputDensePairAnsiFileName);
            using (TextWriter textWriter = File.CreateText(outputDensePairAnsiFileName))
            {
                var rowKeyList = rowKeyToLineList[rowKeyToLineList.Count - 1].Keys;

                //CounterWithMessages counterWithMessages = new CounterWithMessages("writing line {0} of {1}", 10000, rowKeyList.Count);

                textWriter.WriteLine("var\t" + colKeyList.StringJoin("\t"));
                foreach (string rowKey in rowKeyList)
                {
                    //counterWithMessages.Increment();
                    textWriter.Write(rowKey + "\t");
                    foreach (var rowKeyToLine in rowKeyToLineList)
                    {
                        foreach (char c in rowKeyToLine[rowKey])
                        {
                            textWriter.Write((char)map[(byte)c]);
                        }
                    }
                    textWriter.WriteLine();
                }
            }
        }

        private static Predicate<string> OnlyKeepKeysThatHaveAppearedBefore(List<Dictionary<string, string>> rowKeyToLineList)
        {
            Predicate<string> keepDelegate;
            if (rowKeyToLineList.Count == 1)
            {
                keepDelegate = (rowKey => true);
            }
            else
            {
                keepDelegate = (rowKey => rowKeyToLineList[rowKeyToLineList.Count - 2].ContainsKey(rowKey));
            }
            return keepDelegate;
        }

        //!!! very, very similar to other code

        /// <summary>
        /// Creates a DensePairAnsi object from a sequence of groupings. A grouping here is a row 
        /// </summary>
        /// <param name="snpGroupsCidToNucPairEnumerable"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static DensePairAnsi GetInstance(
            IEnumerable<IGrouping<string, KeyValuePair<string, UOPair<char>>>> snpGroupsCidToNucPairEnumerable)
        {
            if (snpGroupsCidToNucPairEnumerable == null)
            {
                throw new ArgumentNullException("snpGroupsCidToNucPairEnumerable");
            }

            DensePairAnsi densePairAnsi = new DensePairAnsi();

            densePairAnsi.ColSerialNumbers = new SerialNumbers<string>();
            densePairAnsi.RowKeyToStoreList = new Dictionary<string, List<UOPair<byte>>>();

            foreach (var snpGroupsCidToNucPair in snpGroupsCidToNucPairEnumerable)
            {
                List<UOPair<byte>> storeArray = Enumerable.Repeat(StaticStoreMissingValue, densePairAnsi.ColSerialNumbers.Count).ToList();
                string var = snpGroupsCidToNucPair.Key;
                densePairAnsi.RowKeyToStoreList.Add(var, storeArray);

                foreach (var cidAndNucPair in snpGroupsCidToNucPair)
                {
                    UOPair<char> valPair = cidAndNucPair.Value;

                    int colIndex = densePairAnsi.ColSerialNumbers.GetNewOrOld(cidAndNucPair.Key);
                    if (colIndex < storeArray.Count)
                    {
                        Helper.CheckCondition(storeArray[colIndex] == StaticStoreMissingValue, () => string.Format(CultureInfo.InvariantCulture, "Each pair of keys, i,e,.<{0},{1}>, should only be seen once", var, cidAndNucPair.Key));
                        storeArray[colIndex] = densePairAnsi.ValueToStore(valPair);
                    }
                    else
                    {
                        Debug.Assert(colIndex == storeArray.Count); // real assert
                        storeArray.Add(densePairAnsi.ValueToStore(valPair));
                    }
                }
                storeArray.TrimExcess();
            }

            densePairAnsi._rowKeys = densePairAnsi.RowKeyToStoreList.Keys.ToList();
            densePairAnsi._indexOfRowKey = densePairAnsi.RowKeys.Select((key, index) => new { key, index }).ToDictionary(keyAndIndex => keyAndIndex.key, keyAndIndex => keyAndIndex.index);
            return densePairAnsi;
        }

        /// <summary>
        /// Creates a DensePairAnsi object from a file in dense pair ansi format.
        /// </summary>
        /// <param name="densePairAnsiFileName">a file in dense pair ansi format</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>the DensePairAnsi object</returns>
        public static DensePairAnsi GetInstance(string densePairAnsiFileName, ParallelOptions parallelOptions)
        {
            DensePairAnsi densePairAnsi = new DensePairAnsi();
            densePairAnsi.GetInstanceInternal(densePairAnsiFileName, parallelOptions);
            return densePairAnsi;
        }
    }

    /// <summary>
    /// Extension methods on Matrix related to DensePairAnsi.
    /// </summary>
    public static class DensePairAnsiExtensions
    {
        /// <summary>
        /// Converts matrix to a DensePairAnsi. If matrix is already a DensePairAnsi, then returns the given matrix without copying. 
        /// </summary>
        /// <param name="inputMatrix">The matrix to convert from</param>
        /// <param name="parallelOptions">Options for controlling any parallelism.</param>
        /// <returns>A densePairAnsi version of the matrix</returns>
        public static DensePairAnsi AsDensePairAnsi(this Matrix<string, string, UOPair<char>> inputMatrix, ParallelOptions parallelOptions)
        {
            if (inputMatrix is DensePairAnsi)
            {
                return (DensePairAnsi)inputMatrix;
            }
            else
            {
                return ToDensePairAnsi(inputMatrix, parallelOptions);
            }
        }

        /// <summary>
        /// Converts matrix to a DensePairAnsi. Even if the matrix is already an densePairAnsi, a new one is created.. 
        /// </summary>
        /// <param name="matrix">The matrix to convert from</param>
        /// <param name="parallelOptions">Options for controlling any parallelism.</param>
        /// <returns>A densePairAnsi version of the matrix</returns>
        public static DensePairAnsi ToDensePairAnsi(this Matrix<string, string, UOPair<char>> matrix, ParallelOptions parallelOptions)
        {
            var densePairAnsi = DensePairAnsi.CreateEmptyInstance(matrix.RowKeys, matrix.ColKeys, DensePairAnsi.StaticMissingValue);

            //Console.WriteLine("Convert no more than {0} values", matrix.RowCount * matrix.ColCount);
            //CounterWithMessages counterWithMessages = new CounterWithMessages("adding value #{0}", 100000, null);
            Parallel.ForEach(matrix.RowKeyColKeyValues, parallelOptions, triple =>
            {
                //counterWithMessages.Increment();
                densePairAnsi[triple.RowKey, triple.ColKey] = triple.Value;
            });

            return densePairAnsi;
        }

        /// <summary>
        /// Write a matrix to DensePairAnsi file format.
        /// </summary>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="filename">The file to write to</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        public static void WriteDensePairAnsi(this Matrix<string, string, UOPair<char>> matrix, string filename, ParallelOptions parallelOptions)
        {
            FileUtils.CreateDirectoryForFileIfNeeded(filename);
            using (TextWriter writer = File.CreateText(filename))
            {
                matrix.WriteDensePairAnsi(writer, parallelOptions);
            }
        }

        //Similar code elsewhere, but hard to move static code to common location
        //Would be nice to convert UOPair<double>, etc to UOPair<char>

        /// <summary>
        /// Write a matrix to DensePairAnsi file format
        /// </summary>
        /// <param name="matrix">The matrix to write</param>
        /// <param name="textWriter">The TextWriter to write to</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        public static void WriteDensePairAnsi(this Matrix<string, string, UOPair<char>> matrix, TextWriter textWriter, ParallelOptions parallelOptions)
        {
            textWriter.WriteLine("var\t{0}", matrix.ColKeys.StringJoin("\t"));
            foreach (string rowKey in matrix.RowKeys)
            {
                textWriter.Write(rowKey);
                textWriter.Write("\t");
                int rowIndex = matrix.IndexOfRowKey[rowKey];
                List<UOPair<byte>> storeList = new List<UOPair<byte>>(matrix.ColCount);
                for (int colIndex = 0; colIndex < matrix.ColCount; ++colIndex)
                {
                    UOPair<char> value;
                    UOPair<byte> store;
                    if (matrix.TryGetValue(rowIndex, colIndex, out value))
                    {
                        store = DensePairAnsi.StaticValueToStore(value);
                    }
                    else
                    {
                        store = DensePairAnsi.StaticStoreMissingValue;
                    }
                    storeList.Add(store);
                }
                Helper.CheckCondition(storeList.Count == matrix.ColCount, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedStoreListCountToEqualColCount, storeList.Count, matrix.ColCount));
                string s = DensePairAnsi.StoreListToString(storeList, matrix.ColCount);
                textWriter.WriteLine(s);
            }
        }
    }
}
