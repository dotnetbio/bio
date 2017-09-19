using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Bio.Matrix
{
    /// <summary>
    /// Be sure to use this is a "Using" to that it gets disposed correctly.
    /// </summary>
    public class RowKeysAnsi : RowKeysStructMatrix<char>
    {
#pragma warning disable 1591
        protected override char ByteArrayToValueOrMissing(byte[] byteArray)
#pragma warning restore 1591
        {
            return (char)byteArray[0];
        }

#pragma warning disable 1591
        protected override byte[] ValueOrMissingToByteArray(char value)
#pragma warning restore 1591
        {
            byte[] byteArray = new byte[] { (byte)value };
            return byteArray;
        }


#pragma warning disable 1591
        protected override int BytesPerValue
#pragma warning restore 1591
        {
            get
            {
                return 1;
            }
        }

#pragma warning disable 1591
        public override char MissingValue
#pragma warning restore 1591
        {
            get
            {
                return '?';
            }
        }

        /// <summary>
        /// Create an instance of RowKeysAnsi from a file in DenseAnsi format.
        /// The RowKeysAnsi is IDisposable and so should be disposed of, for example, with the 'using  statement'.
        /// </summary>
        /// <param name="denseAnsiFileName">The DenseAnsi file</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="fileAccess">A FileAccess value that specifies the operations that can be performed on the file. Defaults to 'Read'</param>
        /// <param name="fileShare">A FileShare value specifying the type of access other threads have to the file. Defaults to 'Read'</param>
        /// <returns>A RowKeysAnsi object</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static RowKeysAnsi GetInstanceFromDenseAnsi(string denseAnsiFileName, ParallelOptions parallelOptions, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read)
        {
            RowKeysAnsi rowKeysAnsi = new RowKeysAnsi();
            rowKeysAnsi.GetInstanceFromDenseStructFileNameInternal(denseAnsiFileName, parallelOptions, fileAccess, fileShare);
            return rowKeysAnsi;
        }


        /// <summary>
        /// Create an instance of RowKeysAnsi from a file in RowKeysAnsi format.
        /// The RowKeysAnsi is IDisposable and so should be disposed of, for example, with the 'using  statement'.
        /// </summary>
        /// <param name="rowKeysAnsiFileName">The rowKeys ansi file</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="fileAccess">A FileAccess value that specifies the operations that can be performed on the file. Defaults to 'Read'</param>
        /// <param name="fileShare">A FileShare value specifying the type of access other threads have to the file. Defaults to 'Read'</param>
        /// <param name="verbose"></param>
        /// <returns>a RowKeysAnsi instance</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static RowKeysAnsi GetInstanceFromRowKeysAnsi(string rowKeysAnsiFileName, ParallelOptions parallelOptions, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read, bool verbose = true)
        {
            RowKeysAnsi rowKeysAnsi = new RowKeysAnsi();
            rowKeysAnsi.GetInstanceFromRowKeysStructFileNameInternal(rowKeysAnsiFileName, parallelOptions, fileAccess, fileShare, verbose);
            return rowKeysAnsi;
        }

    }
}
