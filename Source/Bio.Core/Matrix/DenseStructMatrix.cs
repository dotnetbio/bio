using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bio.Util;
using Bio.Util.Logging;
using System.Globalization;

//!!!would be nice to have a TryGetInstance(filename) that didn't throw MatrixFormatException or any other exception

namespace Bio.Matrix
{
    /// <summary>
    /// An abstract class for implementing a Matrix class that stores its values internally as a struct
    /// and in a file as a fixed number of characters. It is the superclass of, for example, DenseAnsi.
    /// </summary>
    /// <typeparam name="TStore">The struct used to store a value internally. For example, for DenseAnsi this is (8-bit) byte.</typeparam>
    /// <typeparam name="TValue">The type of values exposed externally. For example, for DenseAnsi this is (16-bit) char.</typeparam>
    abstract public class DenseStructMatrix<TStore, TValue> : Matrix<string, string, TValue> where TStore : struct
    {
#pragma warning disable 1591
        protected abstract TStore ValueToStore(TValue value);
#pragma warning restore 1591
#pragma warning disable 1591
        protected abstract TValue StoreToValue(TStore store);
#pragma warning restore 1591
        /// <summary>
        /// Implementor can assume that there is one value for every colKey
        /// </summary>
        protected abstract string StoreListToString(List<TStore> storeList);
#pragma warning disable 1591
        protected abstract List<TStore> StringToStoreList(string line);
#pragma warning restore 1591
#pragma warning disable 1591
        protected abstract TStore SparseValToStore(string val);
#pragma warning restore 1591
#pragma warning disable 1591
        protected abstract TStore StoreMissingValue { get; }
#pragma warning restore 1591

        // Warning may be shorter than ColCount and missing stores are missing.
        //!!!would be better if this was thread-safe

        /// <summary>
        /// The dictionary that maps rowKeys into the stored version of a row. For example, for DenseAnsi the store list is a List of bytes.
        /// </summary>
        public Dictionary<string, List<TStore>> RowKeyToStoreList;
        internal SerialNumbers<string> ColSerialNumbers;
#pragma warning disable 1591
        protected List<string> _rowKeys;
#pragma warning restore 1591
#pragma warning disable 1591
        protected IDictionary<string, int> _indexOfRowKey;
#pragma warning restore 1591


#pragma warning disable 1591
        public override void SetValueOrMissing(int rowIndex, int colIndex, TValue value)
#pragma warning restore 1591
        {
            SetValueOrMissing(RowKeys[rowIndex], ColKeys[colIndex], value);
        }

#pragma warning disable 1591
        public override void SetValueOrMissing(string rowKey, string colKey, TValue value)
#pragma warning restore 1591
        {
            if (IsMissing(value))
            {
                TValue oldValue;
                if (TryGetValue(rowKey, colKey, out oldValue)) //Is there a (non-missing) value?
                {
                    //Yes, there is
                    var storeList = RowKeyToStoreList[rowKey];
                    int colIndex = ColSerialNumbers.GetOld(colKey);

                    lock (storeList) //To be thread-safe, we have to lock the whole row for writes
                    {
                        if (colIndex < storeList.Count)
                        {
                            storeList[colIndex] = StoreMissingValue;
                        }
                    }
                }
            }
            else
            {
                var storeList = RowKeyToStoreList[rowKey];
                int colIndex = ColSerialNumbers.GetOld(colKey);

                lock (storeList) //To be thread-safe, we have to lock the whole row for writes
                {
                    if (colIndex >= storeList.Count)
                    {
                        storeList.AddRange(Enumerable.Repeat(StoreMissingValue, ColSerialNumbers.Count - storeList.Count));
                    }
                    storeList[colIndex] = ValueToStore(value);
                }
            }
        }

#pragma warning disable 1591
        override public ReadOnlyCollection<string> RowKeys
#pragma warning restore 1591
        {
            get { return new ReadOnlyCollection<string>(_rowKeys); }
        }

#pragma warning disable 1591
        override public ReadOnlyCollection<string> ColKeys
#pragma warning restore 1591
        {
            get { return new ReadOnlyCollection<string>(ColSerialNumbers.ItemList); }
        }

#pragma warning disable 1591
        override public IDictionary<string, int> IndexOfRowKey
#pragma warning restore 1591
        {
            get
            {
                return _indexOfRowKey.AsRestrictedAccessDictionary();
            }
        }

#pragma warning disable 1591
        override public IDictionary<string, int> IndexOfColKey
#pragma warning restore 1591
        {
            get { return ColSerialNumbers.ItemToSerialNumber.AsRestrictedAccessDictionary(); }
        }

#pragma warning disable 1591
        override public int RowCount
#pragma warning restore 1591
        {
            get { return RowKeyToStoreList.Count; }
        }

#pragma warning disable 1591
        override public int ColCount
#pragma warning restore 1591
        {
            get { return ColSerialNumbers.Count; }
        }

#pragma warning disable 1591
        override public bool TryGetValue(string rowKey, string colKey, out TValue value)
#pragma warning restore 1591
        {
            List<TStore> colIndexToVal = RowKeyToStoreList[rowKey];
            int colIndex = ColSerialNumbers.GetOld(colKey);
            if (colIndex < colIndexToVal.Count)
            {
                value = StoreToValue(colIndexToVal[colIndex]);
                return !IsMissing(value);
            }
            else
            {
                value = MissingValue;
                return false;
            }
        }

#pragma warning disable 1591
        override public bool TryGetValue(int rowIndex, int colIndex, out TValue value)
#pragma warning restore 1591
        {
            return TryGetValue(RowKeys[rowIndex], ColKeys[colIndex], out value);
        }

#pragma warning disable 1591
        public override IEnumerable<RowKeyColKeyValue<string, string, TValue>> RowKeyColKeyValues
#pragma warning restore 1591
        {
            get
            {
                foreach (var rowKeyAndColIndexToVal in RowKeyToStoreList)
                {
                    string rowKey = rowKeyAndColIndexToVal.Key;
                    var structArray = rowKeyAndColIndexToVal.Value;

                    for (int colIndex = 0; colIndex < structArray.Count; ++colIndex)
                    {
                        string colKey = ColSerialNumbers.ItemList[colIndex];
                        TValue val = StoreToValue(structArray[colIndex]);
                        if (!IsMissing(val))
                        {
                            yield return new RowKeyColKeyValue<string, string, TValue>(rowKey, colKey, val);
                        }
                    }
                }
            }
        }

#pragma warning disable 1591
        public override IEnumerable<TValue> Values
#pragma warning restore 1591
        {
            get
            {
                foreach (var structArray in RowKeyToStoreList.Values)
                {
                    for (int cidIndex = 0; cidIndex < structArray.Count; ++cidIndex)
                    {
                        TValue val = StoreToValue(structArray[cidIndex]);
                        if (!MissingValue.Equals(val))
                        {
                            yield return val;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes the matrix to a file, creating a directory if needed.
        /// The first line is "var" TAB and then the tab-delimited col keys.
        /// Next is one line per row key. Each line is the row key TAB and then all the row's values with no delimiters.
        /// Delimiters are not needed because each value is represented with a fixed number of characters.
        /// Values may include the fixed-number-of-characters version of the special Missing value.
        /// </summary>
        /// <param name="denseStructFileName">The name of the file to write to.</param>
        /// <param name="parallelOptions">Options for controlling any parallelism.</param>
        public void Write(string denseStructFileName, ParallelOptions parallelOptions)
        {
            FileUtils.CreateDirectoryForFileIfNeeded(denseStructFileName);
            using (TextWriter writer = File.CreateText(denseStructFileName))
            {
                Write(writer, parallelOptions);
            }
        }

        /// <summary>
        /// Writes the matrix to textWriter.
        /// The first line is "var" TAB and then the tab-delimited col keys.
        /// Next is one line per row key. Each line is the row key TAB and then all the row's values with no delimiters.
        /// Delimiters are not needed because each value is represented with a fixed number of characters.
        /// Values may include the fixed-number-of-characters version of the special Missing value.
        /// </summary>
        /// <param name="textWriter">The textWriter to write to.</param>
        /// <param name="parallelOptions">Options for controlling any parallelism.</param>
        public void Write(TextWriter textWriter, ParallelOptions parallelOptions)
        {
            textWriter.WriteLine(Helper.CreateTabString("var", ColSerialNumbers.ItemList.StringJoin("\t")));

            var valueStringQuery =
                from rowKey in RowKeys.AsParallel().AsOrdered().WithDegreeOfParallelism(parallelOptions.MaxDegreeOfParallelism)
                let colIndexToVal = FullLengthStoreList(rowKey)
                let valString = StoreListToString(colIndexToVal)
                select rowKey + "\t" + valString;

            CounterWithMessages counterWithMessages = new CounterWithMessages("Writing denseStructMatrix {0} of {1}", 1000, RowCount);
            foreach (var rowKeyAndValString in valueStringQuery)
            {
                counterWithMessages.Increment();
                textWriter.WriteLine(rowKeyAndValString);
            }
        }

        private List<TStore> FullLengthStoreList(string rowKey)
        {
            List<TStore> colIndexToVal = RowKeyToStoreList[rowKey];
            //Fill out the line with values for colKeys's that were seen after this variable was read
            colIndexToVal.AddRange(Enumerable.Repeat(StoreMissingValue, ColCount - colIndexToVal.Count));
            return colIndexToVal;
        }

        internal void GetInstanceInternal(string denseStructFileName, ParallelOptions parallelOptions)
        {
            using (TextReader textReader = FileUtils.OpenTextStripComments(denseStructFileName))
            {
                //!!!similar code in mergedense method
                string header = textReader.ReadLine();
                ColSerialNumbers = ColSerialNumbersFromHeader(header, denseStructFileName);

                CounterWithMessages counterWithMessages = new CounterWithMessages("Reading " + denseStructFileName + " {0}", 1000, null, true);

                //We use ReadEachIndexedLine so that we can process the lines out of order (which is fastest) put still recover the original order of the rowKeys
                var indexRowKeyStructListQuery =
                    from lineAndIndex in FileUtils.ReadEachIndexedLine(textReader)//!!!05/18/2009 .AsParallel().WithDegreeOfParallelism(parallelOptions.DegreeOfParallelism)
                    select new { index = lineAndIndex.Value, rowKeyAndStructList = CreateRowKeyAndStructList(lineAndIndex.Key, denseStructFileName, counterWithMessages) };

                RowKeyToStoreList = new Dictionary<string, List<TStore>>(counterWithMessages.Index + 1);
                _indexOfRowKey = new Dictionary<string, int>();
                foreach (var rowKeyAndStructList in indexRowKeyStructListQuery)
                {
                    RowKeyToStoreList.Add(rowKeyAndStructList.rowKeyAndStructList.Key, rowKeyAndStructList.rowKeyAndStructList.Value);
                    _indexOfRowKey.Add(rowKeyAndStructList.rowKeyAndStructList.Key, rowKeyAndStructList.index);
                }

                _rowKeys =
                    (from rowKeyAndIndex in _indexOfRowKey
                     orderby rowKeyAndIndex.Value
                     select rowKeyAndIndex.Key)
                     .ToList();
            }
        }

        //05/18/2009 seems slow to use the static methods
        private KeyValuePair<string, List<TStore>> CreateRowKeyAndStructList(string line, string denseStructFileName,
            CounterWithMessages counterWithMessages)
        {
            return CreateRowKeyAndStructList(line, denseStructFileName, ColCount, (line1, colCount) => StringToStoreList(line1), counterWithMessages);
        }

        static private KeyValuePair<string, List<TStore>> CreateRowKeyAndStructList(string line, string denseStructFileName,
            int colCount, StaticStringToStoreListDelegate stringToStoreListDelegate,
            CounterWithMessages counterWithMessages)
        {
            counterWithMessages.Increment();
            string valueString;
            string rowKey = SplitVarLine(line, denseStructFileName, colCount, out valueString);

            List<TStore> structList = stringToStoreListDelegate(valueString, colCount);
            if (structList.Count != colCount) throw new MatrixFormatException("Every data string should have a value per col. " + rowKey);
            return new KeyValuePair<string, List<TStore>>(rowKey, structList);
        }

        /// <summary>
        /// From a file denseStruct format (for example, DenseAnsi format), returns a sequence of
        /// the rowKeys. This method scans the file on disk, making it more efficient than first loading it into memory.
        /// </summary>
        /// <param name="denseStructFileName">The name of a file in denseStruct format</param>
        /// <returns>A sequence of rowKeys.</returns>
        public static IEnumerable<string> RowKeysInFile(string denseStructFileName)
        {
            using (TextReader textReader = FileUtils.OpenTextStripComments(denseStructFileName))
            {
                string header = textReader.ReadLine();
                Helper.CheckCondition(null != header, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedFileToHaveHeader, denseStructFileName));
                string[] columns = header.Split(new char[] { '\t' }, 2);
                Helper.CheckCondition(columns.Length > 0 && columns[0] == "var", () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedFileToHaveHeader, denseStructFileName));

                string line;
                while (null != (line = textReader.ReadLine()))
                {
                    string[] fields = line.Split('\t');
                    Helper.CheckCondition(fields.Length == 2, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedEveryVarLineToHaveOneTab, denseStructFileName, fields.Length));
                    string rowKey = fields[0];
                    yield return rowKey;
                }
            }
        }

        /// <summary>
        /// From a file denseStruct format (for example, DenseAnsi format), returns a sequence of
        /// the colKeys. This method scans the file on disk, making it more efficient than first loading it into memory.
        /// </summary>
        /// <param name="denseStructFileName">The name of a file in denseStruct format</param>
        /// <returns>A sequence of colKeys.</returns>
        public static string[] ColKeysInFile(string denseStructFileName)
        {
            using (TextReader textReader = FileUtils.OpenTextStripComments(denseStructFileName))
            {
                string header = textReader.ReadLine();
                Helper.CheckCondition(null != header, Properties.Resource.ExpectedHeaderAsFirstLineOfFile, denseStructFileName);
                string[] columns = header.Split(new char[] { '\t' }, 2);
                Helper.CheckCondition(columns.Length > 0 && columns[0] == "var", () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.Expected_var_AsFirstColumnOfHeader, columns[0], denseStructFileName));

                return columns[1].Split('\t');
            }
        }

        //!!!This seems too specific. Better would be one that returns RowKeyColKeyValue from a file and then that could be changed to string[] outside this class.
        internal static IEnumerable<string[]> EachSparseLine(string filePattern,
            StaticStringToStoreListDelegate staticStringiToStoreListDelegate,
            Converter<TStore, string> StoreToSparseValueDelegate,
            bool zeroIsOK, string fileMessageOrNull, TStore storeMissingValue, CounterWithMessages counterWithMessages)
        {
            foreach (string fileName in FileUtils.GetFiles(filePattern, zeroIsOK))
            {
                if (null != fileMessageOrNull)
                {
                    Console.WriteLine(fileMessageOrNull, fileName);
                }

                using (TextReader textReader = FileUtils.OpenTextStripComments(fileName))
                {
                    string header = textReader.ReadLine();
                    SerialNumbers<string> colSerialNumberCollection = ColSerialNumbersFromHeader(header, fileName);

                    string line;
                    while (null != (line = textReader.ReadLine()))
                    {
                        var rowKeyAndStructList = CreateRowKeyAndStructList(line, filePattern, colSerialNumberCollection.Count, staticStringiToStoreListDelegate, counterWithMessages);
                        string rowKey = rowKeyAndStructList.Key;
                        List<TStore> structList = rowKeyAndStructList.Value;
                        for (int colIndex = 0; colIndex < colSerialNumberCollection.Count; ++colIndex)
                        {
                            TStore store = structList[colIndex];
                            if (!store.Equals(storeMissingValue)) //OK to use Equals because TStore can't be null
                            {
                                string val = StoreToSparseValueDelegate(store);
                                string[] stringArray = new string[] { rowKey, colSerialNumberCollection.GetItem(colIndex), val };
                                yield return stringArray;
                            }
                        }
                    }
                }
            }
        }

        internal static SerialNumbers<string> ColKeysInFilePattern(string denseStructFilePattern)
        {
            var cidQuery =
                from fileName in FileUtils.GetFiles(denseStructFilePattern, /*zeroIsOK*/ false)
                from colKey in ColKeysInFile(fileName)
                orderby colKey
                select colKey;

            SerialNumbers<string> colSerialNumbers = new SerialNumbers<string>(cidQuery);
            return colSerialNumbers;
        }

        internal static List<string> RowKeysInFilePattern(string denseStructFilePattern)
        {
            var rowKeyQuery =
                from fileName in FileUtils.GetFiles(denseStructFilePattern, /*zeroIsOK*/ false)
                from rowKey in RowKeysInFile(fileName)
                select rowKey;

            var rowKeyList = rowKeyQuery.Distinct().ToList();
            return rowKeyList;
        }

        #region Factory Methods

        //internal void InternalGetInstance(string inputDenseByteArrayPattern, string outputDenseByteArrayFileName, string cidDensePattern, string varDensePattern)
        //{
        //    Helper.CheckCondition(false, "test this function before trusting it");
        //    CidSerialNumbers = ColKeysInFilePattern(cidDensePattern);
        //    _rowKeys = RowKeysInFilePattern(varDensePattern);

        //    //Need to preallocate all the space
        //    var allMissingQuery = Enumerable.Repeat(StoreMissingValue, ColCount);

        //    RowKeyToStoreList = RowKeysInFilePattern(varDensePattern)
        //        .ToDictionary(var => var, var => allMissingQuery.ToList());


        //    foreach (string denseStructFileName in FileUtils.GetFiles(inputDenseByteArrayPattern, /*zeroIsOk*/ false))
        //    {
        //        Console.WriteLine(denseStructFileName);
        //        using (TextReader textReader = File.OpenText(denseStructFileName))
        //        {
        //            string header = textReader.ReadLine();
        //            SerialNumbers<string> inputSerialNumbers = CidSerialNumbersFromHeader(header, denseStructFileName);
        //            List<int> inputSerialNumberToCidSerialNumber =
        //                (from cid in inputSerialNumbers.ItemList
        //                 select CidSerialNumbers.GetOld(cid)
        //                ).ToList();

        //            string line;
        //            while (null != (line = textReader.ReadLine()))
        //            {

        //                string inputValueString;
        //                string var = SplitVarLine(line, denseStructFileName, inputSerialNumbers.Count, out inputValueString);

        //                Helper.CheckCondition(RowKeyToStoreList.ContainsKey(var), "A var in the input was not seen in varDensePattern. " + var);
        //                List<TStore> storeList = StringToStoreList(inputValueString); //This file's store array
        //                List<TStore> valueList = RowKeyToStoreList[var]; //The store array for all the files

        //                for (int inputSerialNumber = 0; inputSerialNumber < inputSerialNumbers.Count; ++inputSerialNumber)
        //                {
        //                    TStore newByte = storeList[inputSerialNumber];
        //                    int cidSerialNumber = inputSerialNumberToCidSerialNumber[inputSerialNumber];
        //                    TStore oldByte = valueList[cidSerialNumber];

        //                    Helper.CheckCondition(oldByte.Equals(StoreMissingValue) || oldByte.Equals(newByte), "Value is given two different values (var={0},cid={1},file={2})", var, inputSerialNumbers.GetItem(inputSerialNumber), denseStructFileName);
        //                    valueList[cidSerialNumber] = newByte;
        //                }
        //            }
        //        }
        //    }

        //    _indexOfRowKey = RowKeys.Select((key, index) => new { key, index }).ToDictionary(keyAndIndex => keyAndIndex.key, keyAndIndex => keyAndIndex.index);
        //}

        private static string SplitVarLine(string line, string denseFileNameForErrorMessage, int colCount, out string valueString)
        {
            string[] fields = line.Split('\t');
            if (fields.Length != 2)
            {
                throw new MatrixFormatException("Every var line should have exactly one tab. " + denseFileNameForErrorMessage);
            }
            string rowKey = fields[0];
            valueString = fields[1];

            return rowKey;
        }

        private static SerialNumbers<string> ColSerialNumbersFromHeader(string headerOrNull, string denseFileNameForErrorMessage)
        {
            //Helper.CheckCondition(null != headerOrNull, "input file must contain a first line. " + denseFileNameForErrorMessage);
            MatrixFormatException.CheckCondition(null != headerOrNull, "input file must contain a first line. " + denseFileNameForErrorMessage);
            string[] columns = headerOrNull.Split('\t');
            //Helper.CheckCondition(columns.Length > 0 && columns[0] == "var", "The first column in the file should be 'var'. " + denseFileNameForErrorMessage);
            MatrixFormatException.CheckCondition(columns.Length > 0 && (columns[0] == "var" || columns[0] == "row"), "The first column in the file should be 'var'. " + denseFileNameForErrorMessage);
            SerialNumbers<string> inputSerialNumbers = new SerialNumbers<string>(columns.Skip(1));
            return inputSerialNumbers;
        }
        //!!!matrix: when combining Matrix's be sure the missing values are compatible
        internal void InternalCreateEmptyInstance(IEnumerable<string> rowKeySequence, IEnumerable<string> colKeySequence)
        {
            ColSerialNumbers = new SerialNumbers<string>(colKeySequence);
            _rowKeys = rowKeySequence.ToList();
            _indexOfRowKey = RowKeys.Select((key, index) => new { key, index }).ToDictionary(rowKeyAndIndex => rowKeyAndIndex.key, rowKeyAndIndex => rowKeyAndIndex.index);
            CreateEmptyRowKeyToStoreList();
        }

        private void CreateEmptyRowKeyToStoreList()
        {
            RowKeyToStoreList = new Dictionary<string, List<TStore>>();

            foreach (string rowKey in RowKeys)
            {
                RowKeyToStoreList.Add(rowKey, new List<TStore>());
            }
        }

        internal void GetInstanceFromSparseInternal(string inputSparsePattern)
        {
            ColSerialNumbers = new SerialNumbers<string>();
            RowKeyToStoreList = new Dictionary<string, List<TStore>>();

            //!!!05/18/2009 optimize could be made faster with plinq using an index read to keep the order of the rowKeys (aka vars)
            foreach (List<string[]> linesWithSameRowKey in SparseGroupedByVar(inputSparsePattern, /*zeroIsOK*/ false, "File {0}"))
            {
                Debug.Assert(linesWithSameRowKey.Count > 0); // real assert
                string rowKey = linesWithSameRowKey[0][0];

                List<TStore> storeList = Enumerable.Repeat(StoreMissingValue, ColSerialNumbers.Count).ToList();
                RowKeyToStoreList.Add(rowKey, storeList);

                foreach (string[] rowKeyColKeyVal in linesWithSameRowKey)
                {
                    string colKey = rowKeyColKeyVal[1];
                    string valAsString = rowKeyColKeyVal[2];
                    TStore store = SparseValToStore(valAsString);
                    Helper.CheckCondition(!store.Equals(StoreMissingValue), () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ErrorConvertingValGaveMissingValue, valAsString, StoreMissingValue)); //OK to use Equals because TScore can't be null

                    int colIndex = ColSerialNumbers.GetNewOrOld(colKey);
                    if (colIndex < storeList.Count)
                    {
                        Helper.CheckCondition(storeList[colIndex].Equals(StoreMissingValue), () => string.Format(CultureInfo.InvariantCulture, "Each pair of keys, i,e,.<{0},{1}>, should only be seen once", rowKey, colKey));
                        storeList[colIndex] = store;
                    }
                    else
                    {
                        Helper.CheckCondition(colIndex == storeList.Count, () => Properties.Resource.ErrorInputDataShouldBeGroupedByVar);
                        storeList.Add(store);
                    }
                }
                storeList.TrimExcess();
            }

            _rowKeys = RowKeyToStoreList.Keys.ToList();
            _indexOfRowKey = RowKeys.Select((key, index) => new { key, index }).ToDictionary(keyAndIndex => keyAndIndex.key, keyAndIndex => keyAndIndex.index);
        }

        //Similar code above but doesn't pad out a variable's line until it needs to

        /// <summary>
        /// Create a DenseStructMatrix from a sequences of RowKeyColKeyValue triples.
        /// </summary>
        /// <param name="tripleEnumerable">The squences of RowKeyColKeyValue triples</param>
        public void GetInstanceFromSparseInternal(IEnumerable<RowKeyColKeyValue<string, string, TValue>> tripleEnumerable)
        {
            ColSerialNumbers = new SerialNumbers<string>();
            RowKeyToStoreList = new Dictionary<string, List<TStore>>();

            foreach (var triple in tripleEnumerable)
            {
                string rowKey = triple.RowKey;
                List<TStore> storeList;
                if (!RowKeyToStoreList.TryGetValue(rowKey, out storeList))
                {
                    storeList = Enumerable.Repeat(StoreMissingValue, ColSerialNumbers.Count).ToList();
                    RowKeyToStoreList[rowKey] = storeList;
                }

                string colKey = triple.ColKey;
                TValue value = triple.Value;
                TStore store = ValueToStore(value);
                Helper.CheckCondition(!store.Equals(StoreMissingValue), () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ErrorConvertingValGaveMissingValue, value, StoreMissingValue));

                int colIndex = ColSerialNumbers.GetNewOrOld(colKey);
                if (colIndex < storeList.Count)
                {
                    Helper.CheckCondition(storeList[colIndex].Equals(StoreMissingValue), () => string.Format(CultureInfo.InvariantCulture, "Each pair of keys, i,e,.<{0},{1}>, should only be seen once", rowKey, colKey));
                    storeList[colIndex] = store;
                }
                else
                {
                    while (colIndex > storeList.Count)
                    {
                        storeList.Add(StoreMissingValue);
                    }
                    // REVIEW:  is this a bogus check?  Only a failure of storeList.Add could make this not be true
                    Helper.CheckCondition(colIndex == storeList.Count, () => Properties.Resource.RealAssert);
                    storeList.Add(store);
                }
            }

            foreach (var storeList in RowKeyToStoreList.Values)
            {
                storeList.TrimExcess();
            }

            _rowKeys = RowKeyToStoreList.Keys.ToList();
            _indexOfRowKey = RowKeys.Select((key, index) => new { key, index }).ToDictionary(keyAndIndex => keyAndIndex.key, keyAndIndex => keyAndIndex.index);
        }

        internal void InternalGetInstance(Matrix<string, string, TValue> inputMatrix, ParallelOptions parallelOptions)
        {
            var selectRowsAndColsView = inputMatrix as SelectRowsAndColsView<string, string, TValue>;
            if (null != selectRowsAndColsView && selectRowsAndColsView.ParentMatrix is DenseStructMatrix<TStore, TValue>) //We optimize this case
            {
                var parentMatrix = (DenseStructMatrix<TStore, TValue>)selectRowsAndColsView.ParentMatrix;
                Parallel.ForEach(RowKeys, parallelOptions, rowKey =>
                {
                    List<TStore> oldStoreList = parentMatrix.RowKeyToStoreList[rowKey];
                    List<TStore> newStoreList = RowKeyToStoreList[rowKey];
                    foreach (int oldColIndex in selectRowsAndColsView.IndexOfParentColKey)
                    {
                        newStoreList.Add(oldStoreList[oldColIndex]);
                    }
                });
            }
            else
            {
                CounterWithMessages counterWithMessages = new CounterWithMessages("Creating new DenseStructMatrix, working on row #{0} of {1}", 1000, RowCount);
                Parallel.ForEach(RowKeys, parallelOptions, rowKey =>
                {
                    counterWithMessages.Increment();
                    foreach (string colKey in ColKeys)
                    {
                        TValue value;
                        if (inputMatrix.TryGetValue(rowKey, colKey, out value))
                        {
                            this[rowKey, colKey] = value;
                        }
                    }
                });
            }
        }
        #endregion

        /// <summary>
        /// The type of functions that can convert a string of a row's value (with no delimiters from a denseStruct file)
        /// to a list of the structs used to represent those values internally.
        /// Raises an exception if the line is the wrong length.
        /// </summary>
        /// <param name="line">The rows values represented as a string with no delimiters.</param>
        /// <param name="colCount">The number of colKeys.</param>
        /// <returns>The list of structed used to represent the values internally.</returns>
        public delegate List<TStore> StaticStringToStoreListDelegate(string line, int colCount);

        /// <summary>
        /// The type of functions that can convert a list of structs (the internal represenation of a row's values) to
        /// a string of the values without delimiters, suitable to writing to a denseStruct file.
        /// </summary>
        /// <param name="storeList">A list of structs used to internally represent the values of a row.</param>
        /// <param name="colCount">ColCount</param>
        /// <returns></returns>
        public delegate string StoreListToStringDelegate(List<TStore> storeList, int colCount);

        //!!!This seems too specific. Better would be one that returns RowKeyColKeyValue from a file and then that could be changed to string[] outside this class.
        private static IEnumerable<string[]> EachSparseLine(string filePattern, bool zeroIsOK, string fileMessageOrNull)
        {
            foreach (string fileName in FileUtils.GetFiles(filePattern, zeroIsOK))
            {
                if (null != fileMessageOrNull)
                {
                    Console.WriteLine(fileMessageOrNull, fileName);
                }
                using (TextReader textReader = FileUtils.OpenTextStripComments(fileName))
                {
                    string header = textReader.ReadLine();
                    Helper.CheckCondition(header != null, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedFileToHaveHeader, fileName));
                    Helper.CheckCondition(header.Equals("var\tcid\tval", StringComparison.InvariantCultureIgnoreCase), () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedHeaderToBe_var_cid_val, header));
                    string line;
                    while (null != (line = textReader.ReadLine()))
                    {
                        string[] fields = line.Split('\t');
                        Helper.CheckCondition(fields.Length == 3, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedThreeFields, fields.Length, line));
                        yield return fields;
                    }
                }
            }
        }

        // Assumes, but doesn't check that variables are together and that they don't span files
        //!!!This seems too specific. Better would be one that returns RowKeyColKeyValue from a file and then that could be changed to string[] outside this class.
        private static IEnumerable<List<string[]>> SparseGroupedByVar(string filePattern, bool zeroIsOK, string fileMessageOrNull)
        {
            List<string[]> group = null;
            string currentRowKey = null;
            foreach (string[] varCidVal in EachSparseLine(filePattern, zeroIsOK, fileMessageOrNull))
            {
                string rowKey = varCidVal[0];
                if (currentRowKey != rowKey)
                {
                    if (currentRowKey != null)
                    {
                        yield return group;
                    }
                    currentRowKey = rowKey;
                    group = new List<string[]>();
                }
                group.Add(varCidVal);
            }
            if (currentRowKey != null)
            {
                yield return group;
            }
        }

        internal string ColKeyToString(string colKey, StoreListToStringDelegate storeListToStringDelegate)
        {
            int colIndex = ColSerialNumbers.GetOld(colKey);
            List<TStore> storeByColKey = new List<TStore>(ColCount);
            foreach (List<TStore> sequenceByVar in RowKeyToStoreList.Values)
            {
                storeByColKey.Add(sequenceByVar[colIndex]);
            }

            return storeListToStringDelegate(storeByColKey, RowCount);
        }
    }
}

