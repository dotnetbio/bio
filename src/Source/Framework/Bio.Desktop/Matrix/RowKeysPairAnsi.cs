using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio.Util;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace Bio.Matrix
{
    /// <summary>
    /// Be sure to use this is a "Using" to that it gets disposed correctly.
    /// </summary>
    public class RowKeysPairAnsi : RowKeysStructMatrix<UOPair<char>>
    {
#pragma warning disable 1591
        protected override UOPair<char> ByteArrayToValueOrMissing(byte[] byteArray)
#pragma warning restore 1591
        {
            return new UOPair<char>((char)byteArray[0], (char)byteArray[1]);
        }

#pragma warning disable 1591
        protected override byte[] ValueOrMissingToByteArray(UOPair<char> value)
#pragma warning restore 1591
        {
            byte[] byteArray = new byte[] { (byte)value.First, (byte)value.Second };
            return byteArray;
        }


#pragma warning disable 1591
        protected override int BytesPerValue
#pragma warning restore 1591
        {
            get
            {
                return 2;
            }
        }

#pragma warning disable 1591
        public override UOPair<char> MissingValue
#pragma warning restore 1591
        {
            get
            {
               return new UOPair<char>('?', '?');
            }
        }

        /// <summary>
        /// Create an instance of RowKeysPairAnsi from a file in DensePairAnsi format.
        /// The RowKeysPairAnsi is IDisposable and so should be disposed of, for example, with the 'using  statement'.
        /// </summary>
        /// <param name="pairAnsiFileName">The pairAnsi file</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="fileAccess">A FileAccess value that specifies the operations that can be performed on the file. Defaults to 'Read'</param>
        /// <param name="fileShare">A FileShare value specifying the type of access other threads have to the file. Defaults to 'Read'</param>
        /// <returns>A RowKeysPairAnsi object </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static RowKeysPairAnsi GetInstanceFromPairAnsi(string pairAnsiFileName, ParallelOptions parallelOptions, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read)
        {
            RowKeysPairAnsi rowKeysPairAnsi = new RowKeysPairAnsi();
            rowKeysPairAnsi.GetInstanceFromDenseStructFileNameInternal(pairAnsiFileName, parallelOptions, fileAccess, fileShare);
            return rowKeysPairAnsi;
        }

        /// <summary>
        /// Create an instance of RowKeysPairAnsi from a file in RowKeysPairAnsi format.
        /// The RowKeysPairAnsi is IDisposable and so should be disposed of, for example, with the 'using  statement'.
        /// </summary>
        /// <param name="rowKeysAnsiFileName">The RowKeysPairAnsi file</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="fileAccess">A FileAccess value that specifies the operations that can be performed on the file. Defaults to 'Read'</param>
        /// <param name="fileShare">A FileShare value specifying the type of access other threads have to the file. Defaults to 'Read'</param>
        /// <returns>A RowKeysPairAnsi object</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"),SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static RowKeysPairAnsi GetInstanceFromRowKeysAnsi(string rowKeysAnsiFileName, ParallelOptions parallelOptions, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read)
        {
            RowKeysPairAnsi rowKeysPairAnsi = new RowKeysPairAnsi();
            rowKeysPairAnsi.GetInstanceFromRowKeysStructFileNameInternal(rowKeysAnsiFileName, parallelOptions, fileAccess, fileShare);
            return rowKeysPairAnsi;
        }
    }
}
