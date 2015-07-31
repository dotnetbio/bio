using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bio.Util;
using Bio.Util.Logging;


//!!!would be nice to have a TryGetInstance(filename) that didn't throw MatrixFormatException or any other exception

namespace Bio.Matrix
{
    /// <summary>
    /// An abstract Matrix class for accessing values off disk instead of keeping them memory. For example, RowKeysAnsi
    /// is a subclass that access values from DenseAnsi format. The on-disk file format must be a subtype of DenseStructMatrix, 
    /// for example, DenseAnsi or PaddedDouble. The method is IDisposable and should be used with a 'Using' statement or should be closed.
    /// </summary>
    /// <typeparam name="TValue">The type of the value</typeparam>
    abstract public class RowKeysStructMatrix<TValue> : Matrix<string, string, TValue>, IDisposable
    {
        string DenseStructFileName;
        FileAccess FileAccess;
        FileShare FileShare;

        ThreadLocal<Tuple<Stream, TextReader>> _threadLocalStreamAndTextReader = new ThreadLocal<Tuple<Stream, TextReader>>(() => null);
        Tuple<Stream, TextReader> ThreadLocalStreamAndTextReader
        {
            get
            {
                if (null == _threadLocalStreamAndTextReader.Value)
                {
                    Stream stream = File.Open(DenseStructFileName, FileMode.Open, FileAccess, FileShare);
                    TextReader textReader = new StreamReader(stream);
                    var tuple = new Tuple<Stream, TextReader>(stream, textReader);
                    _threadLocalStreamAndTextReader.Value = tuple;
                    ThreadLocalStreamAndTextReaderBag.Add(tuple);
                }
                return _threadLocalStreamAndTextReader.Value;
            }
        }

        Stream ThreadLocalStream
        {
            get
            {
                return ThreadLocalStreamAndTextReader.Item1;
            }
        }

        TextReader ThreadLocalTextReader
        {
            get
            {
                return ThreadLocalStreamAndTextReader.Item2;
            }
        }
        ConcurrentBag<Tuple<Stream, TextReader>> ThreadLocalStreamAndTextReaderBag = new ConcurrentBag<Tuple<Stream, TextReader>>();


        /// <summary>
        /// A method that converts bytes (read from the on-disk file) into a value.
        /// </summary>
        /// <param name="byteArray">bytes from the on-disk file</param>
        /// <returns>a value</returns>
        protected abstract TValue ByteArrayToValueOrMissing(byte[] byteArray);

        /// <summary>
        /// A method that converts a value (including the special missing value) to an array of bytes.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The byte array</returns>
        protected abstract byte[] ValueOrMissingToByteArray(TValue value);

        /// <summary>
        /// The number of bytes per value in the on-disk file format.
        /// </summary>
        protected abstract int BytesPerValue { get; }

        private Dictionary<string, long> RowKeyToFilePosition;

        /// <summary>
        /// The mapping from column keys to column indexes.
        /// </summary>
        private SerialNumbers<string> ColSerialNumbers;


        private List<string> _rowKeys;
        private IDictionary<string, int> _indexOfRowKey;


        /// <summary>
        /// Write out the RowKeys file for a matrix. This file is an index to the rows of another file.
        /// </summary>
        /// <param name="simpleFileName">The rowKeys file to write to. It must not include any path information. It will be created in the other file's directory.</param>
        public void WriteRowKeys(string simpleFileName)
        {
            Helper.CheckCondition(String.IsNullOrEmpty(Path.GetDirectoryName(simpleFileName)), () => Properties.Resource.FileNameMustNotContainPathInformation);
            string fileName = Path.Combine(Path.GetDirectoryName(DenseStructFileName), simpleFileName);
            FileUtils.CreateDirectoryForFileIfNeeded(fileName);
            using (TextWriter textWriter = File.CreateText(fileName))
            {
                textWriter.WriteLine("rowKey\t{0}", ColKeys.StringJoin("\t")); //!!!const
                textWriter.WriteLine(Path.GetFileName(DenseStructFileName));
                foreach (string rowKey in RowKeys)
                {
                    textWriter.WriteLine("{0}\t{1}", rowKey, RowKeyToFilePosition[rowKey]);
                }
            }
        }


        //!!!similar to GetInstanceFromDenseStructFileNameInternal


        /// <summary>
        /// Get a instance from a file in a RowKeys format
        /// </summary>
        /// <param name="rowKeysStructFileName">The rowKeys file</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="fileAccess">A FileAccess value that specifies the operations that can be performed on the file. Defaults to 'Read'</param>
        /// <param name="fileShare">A FileShare value specifying the type of access other threads have to the file. Defaults to 'Read'</param>
        /// <param name="verbose"></param>
        protected void GetInstanceFromRowKeysStructFileNameInternal(string rowKeysStructFileName, ParallelOptions parallelOptions, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read, bool verbose = true)
        {
            // parallelOptions is not currently used, but it is need so that this method will have the same signature as other, similar methods.
            lock (this)
            {
                string firstLineOrNull = FileUtils.ReadLine(rowKeysStructFileName);
                Helper.CheckCondition(null != firstLineOrNull, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedFileToHaveData, rowKeysStructFileName));
                Helper.CheckCondition(!firstLineOrNull.StartsWith(FileUtils.CommentHeader, StringComparison.Ordinal), Properties.Resource.ExpectedNoCommentsInRowKeysAnsiFiles, rowKeysStructFileName);


                RowKeyToFilePosition = new Dictionary<string, long>();
                FileAccess = fileAccess;
                FileShare = fileShare;


                //using (TextReader textReader = File.OpenText(rowKeysStructFileName))
                using (TextReader textReader = FileUtils.OpenTextStripComments(rowKeysStructFileName))
                {

                    string colKeysLineOrNull = textReader.ReadLine();
                    if (null == colKeysLineOrNull) 
                        throw new MatrixFormatException("Surprised by empty file. " + rowKeysStructFileName);

                    string[] varAndColKeys = colKeysLineOrNull.Split('\t');
                    if (!varAndColKeys[0].Equals("rowKey"))
                    {
                        throw new MatrixFormatException("Expect first row's first value to be 'rowKey'"); //!!!rowKey
                    }

                    ColSerialNumbers = new SerialNumbers<string>(varAndColKeys.Skip(1));
                    _rowKeys = new List<string>();


                    //!!!not really thread-safe
                    string denseStructFileNameInFile = textReader.ReadLine();
                    DenseStructFileName = Path.Combine(Path.GetDirectoryName(rowKeysStructFileName), denseStructFileNameInFile);

                    CounterWithMessages counterWithMessages = verbose ? new CounterWithMessages("Reading rowKey file to find location of rows, #{0}", 10000, null) : null;

                    string line = null;
                    while (null != (line = textReader.ReadLine()))
                    {
                        if (verbose) counterWithMessages.Increment();
                        string[] rowKeyAndPosition = line.Split('\t');
                        if (rowKeyAndPosition.Length != 2) throw new MatrixFormatException("Expect rows to have two columns");
                        string rowKey = rowKeyAndPosition[0];
                        long position = long.Parse(rowKeyAndPosition[1], CultureInfo.CurrentCulture);
                        _rowKeys.Add(rowKey);
                        RowKeyToFilePosition.Add(rowKey, position);
                    }
                }
                //Console.WriteLine("all lines read from file [{0}]", rowKeysStructFileName);

                _indexOfRowKey = RowKeys.Select((key, index) => new { key, index }).ToDictionary(keyAndIndex => keyAndIndex.key, keyAndIndex => keyAndIndex.index);
                ValueTester(rowKeysStructFileName);

            }
        }

        private void ValueTester(string rowKeysStructFileName)
        {
            //Console.WriteLine("Index loaded. Now testing values");


            //Test that can really read values from data file
            if (RowCount > 0 && ColCount > 0)
            {
                try
                {
                    string rowKey = RowKeys[RowCount - 1];

                    //Check the width of the data
                    if (RowCount > 1)
                    {
                        long rowLength = RowKeyToFilePosition[rowKey] - RowKeyToFilePosition[RowKeys[RowCount - 2]] - rowKey.Length - 1 /*tab*/ - 2 /*newline*/;
                        Helper.CheckCondition<MatrixFormatException>(rowLength == BytesPerValue * ColCount, ()=> string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectEachDataRowToHaveNCharacters, BytesPerValue * ColCount));
                    }

                    //Check the length of the file
                    long lastPosition = RowKeyToFilePosition[rowKey] + ColCount * BytesPerValue + 2 /*newline*/;
                    Helper.CheckCondition<MatrixFormatException>(ThreadLocalStream.Length == lastPosition, Properties.Resource.ExpectFileToEndAfterLastValue);

                    //Check that the last rowKey is where it is expected
                    ThreadLocalStream.Position = RowKeyToFilePosition[rowKey] - rowKey.Length - 1;
                    byte[] byteArray = new byte[rowKey.Length];
                    int bytesRead = ThreadLocalStream.Read(byteArray, 0, rowKey.Length);
                    Helper.CheckCondition<MatrixFormatException>(bytesRead == rowKey.Length,  Properties.Resource.ExpectToReadRowKey);
                    string asString = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                    Helper.CheckCondition<MatrixFormatException>(asString == rowKey, Properties.Resource.ExpectRowKeyFileAndMainFileToAgreeOnTheRowkeys);

                    //Read some values
                    GetValueOrMissing(0, 0);
                    GetValueOrMissing(RowCount / 2, ColCount / 2);
                    GetValueOrMissing(RowCount - 1, ColCount - 1);
                }
                catch (MatrixFormatException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new MatrixFormatException(Properties.Resource.RowKeyIndexSeemsToReferToAFileOfTheWrongFormat + rowKeysStructFileName, e);
                }
            }
            //Console.WriteLine("Values tested. Done");
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
            get { return _rowKeys.Count; }
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
            int colIndex = ColSerialNumbers.GetOld(colKey);
            ThreadLocalStream.Position = RowKeyToFilePosition[rowKey] + colIndex * BytesPerValue;
            byte[] byteArray = new byte[BytesPerValue];
            int bytesRead = ThreadLocalStream.Read(byteArray, 0, BytesPerValue);
            Helper.CheckCondition(bytesRead == BytesPerValue, () => Properties.Resource.ExpectedToReadAllBytesOfValue);
            value = ByteArrayToValueOrMissing(byteArray);
            return !IsMissing(value);
        }


#pragma warning disable 1591
        override public bool TryGetValue(int rowIndex, int colIndex, out TValue value)
#pragma warning restore 1591
        {
            return TryGetValue(RowKeys[rowIndex], ColKeys[colIndex], out value);
        }

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
            int colIndex = ColSerialNumbers.GetOld(colKey);
            ThreadLocalStream.Position = RowKeyToFilePosition[rowKey] + colIndex * BytesPerValue;
            byte[] byteArray = ValueOrMissingToByteArray(value);
            Helper.CheckCondition(byteArray.Length == BytesPerValue, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedByteArrayLengthAndBytesPerValueToBeEqual, byteArray.Length, BytesPerValue));
            ThreadLocalStream.Write(byteArray, 0, BytesPerValue);
        }


        /// <summary>
        /// Used by subclasses, such as RowKeyAnsi, to open a file on disk.
        /// The enseStructFileNam is IDisposable and so should be disposed of, for example, with the 'using  statement'.
        /// </summary>
        /// <param name="denseStructFileName">The file on disk to open</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="fileAccess">A FileAccess value that specifies the operations that can be performed on the file. Defaults to 'Read'</param>
        /// <param name="fileShare">A FileShare value specifying the type of access other threads have to the file. Defaults to 'Read'</param>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times"), SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "parallelOptions"), SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void GetInstanceFromDenseStructFileNameInternal(string denseStructFileName, ParallelOptions parallelOptions, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read)
        {
            // parallelOptions is not currently used, but it is need so that this method will have the same signature as other, similar methods.
            lock (this)
            {
                using (FileStream fileStream = File.Open(denseStructFileName, FileMode.Open, fileAccess, fileShare))
                {
                    using (TextReader textReader = new StreamReader(fileStream))
                    {
                        string firstLineOrNull = textReader.ReadLine();
                        Helper.CheckCondition(null != firstLineOrNull, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedFileToHaveData, denseStructFileName));
                        Helper.CheckCondition(!firstLineOrNull.StartsWith(FileUtils.CommentHeader, StringComparison.Ordinal), () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedNoCommentsInRowKeysAnsiFiles, denseStructFileName));
                    }
                }

                RowKeyToFilePosition = new Dictionary<string, long>();

                DenseStructFileName = denseStructFileName;
                FileAccess = fileAccess;
                FileShare = fileShare;
                long position = 0;


                string colKeysLineOrNull = ThreadLocalTextReader.ReadLine();
                var newLineLength = Environment.NewLine.Length;
                position += colKeysLineOrNull.Length + newLineLength;
                string[] varAndColKeys = colKeysLineOrNull.Split('\t');
                if (!varAndColKeys[0].Equals("var")) throw new MatrixFormatException("Expect first row's first value to be 'var'");
                ColSerialNumbers = new SerialNumbers<string>(varAndColKeys.Skip(1));
                _rowKeys = new List<string>();
                if (null == colKeysLineOrNull) throw new MatrixFormatException("Surprised by empty file. " + denseStructFileName);
                CounterWithMessages counterWithMessages = new CounterWithMessages("Reading data file to find location of rows, #{0}", 10000, null);

                while (true)
                {
                    counterWithMessages.Increment();
                    ThreadLocalStream.Position = position;
                    StringBuilder sb = new StringBuilder();
                    while (true)
                    {
                        int i = ThreadLocalStream.ReadByte();
                        if (-1 == i)
                        {
                            goto END;
                        }
                        if ('\t' == (char)i)
                        {
                            break; // real break, not conintue
                        }
                        sb.Append((char)i);
                    }

                    string rowKey = sb.ToString();
                    if (RowKeyToFilePosition.ContainsKey(rowKey))
                    {
                        throw new MatrixFormatException(string.Format(CultureInfo.InvariantCulture, "The rowkey {0} appears more than once", rowKey));
                    }

                    _rowKeys.Add(rowKey);
                    position += rowKey.Length + 1;
                    RowKeyToFilePosition.Add(rowKey, position);
                    position += ColCount * BytesPerValue + newLineLength;
                    if (position > ThreadLocalStream.Length)
                    {
                        throw new MatrixFormatException("File seems too short");
                    }
                }
            END: ;

                _indexOfRowKey = RowKeys.Select((key, index) => new { key, index }).ToDictionary(keyAndIndex => keyAndIndex.key, keyAndIndex => keyAndIndex.index);
            }
        }


        private bool _disposed = false;
        private Object thisLock = new Object();

#pragma warning disable 1591
        public void Dispose()
#pragma warning restore 1591
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#pragma warning disable 1591
        protected virtual void Dispose(bool disposing)
#pragma warning restore 1591
        {
            // Using lock for thread safety.  Do we need thread safety?
            lock (thisLock)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        foreach (var streamAndTextReader in ThreadLocalStreamAndTextReaderBag)
                        {
                            streamAndTextReader.Item1.Dispose();
                            streamAndTextReader.Item2.Dispose();
                        }
                    }
                    _disposed = true;
                }
            }
        }
    }
}

