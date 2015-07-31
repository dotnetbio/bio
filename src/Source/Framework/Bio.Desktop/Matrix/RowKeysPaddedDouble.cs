using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace Bio.Matrix
{
    /// <summary>
    /// Be sure to use this is a "Using" to that it gets disposed correctly.
    /// </summary>
    public class RowKeysPaddedDouble : RowKeysStructMatrix<double>
    {

#pragma warning disable 1591
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected override double ByteArrayToValueOrMissing(byte[] byteArray)
#pragma warning restore 1591
        {
            return double.Parse(System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
        }

#pragma warning disable 1591
        protected override byte[] ValueOrMissingToByteArray(double value)
#pragma warning restore 1591
        {
            //System.Text.ASCIIEncoding  encoding=new System.Text.ASCIIEncoding();
            string s = PaddedDouble.StoreToSparseVal(value);
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(s);
            return byteArray;
        }


#pragma warning disable 1591
        protected override int BytesPerValue
#pragma warning restore 1591
        {
            get
            {
                return PaddedDouble.BytesPerValue;
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
        /// Create an instance of RowKeysPaddedDouble from a file in PaddedDouble format.
        /// The RowKeysPaddedDouble is IDisposable and so should be disposed of, for example, with the 'using  statement'.
        /// </summary>
        /// <param name="paddedDoubleFileName">The PaddedDouble file</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="fileAccess">A FileAccess value that specifies the operations that can be performed on the file. Defaults to 'Read'</param>
        /// <param name="fileShare">A FileShare value specifying the type of access other threads have to the file. Defaults to 'Read'</param>
        /// <returns>A RowKeysPaddedDouble object</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static RowKeysPaddedDouble GetInstanceFromPaddedDouble(string paddedDoubleFileName, ParallelOptions parallelOptions, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read)
        {
            RowKeysPaddedDouble rowKeysPaddedDouble = new RowKeysPaddedDouble();
            rowKeysPaddedDouble.GetInstanceFromDenseStructFileNameInternal(paddedDoubleFileName, parallelOptions, fileAccess, fileShare);
            return rowKeysPaddedDouble;
        }

        /// <summary>
        /// Create an instance of RowKeysPaddedDouble from a file in RowKeysPaddedDouble format.
        /// The RowKeysPaddedDouble is IDisposable and so should be disposed of, for example, with the 'using  statement'.
        /// </summary>
        /// <param name="rowKeysFileName">The RowKeysPaddedDouble file</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="fileAccess">A FileAccess value that specifies the operations that can be performed on the file. Defaults to 'Read'</param>
        /// <param name="fileShare">A FileShare value specifying the type of access other threads have to the file. Defaults to 'Read'</param>
        /// <returns>A RowKeysPaddedDouble object</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static RowKeysPaddedDouble GetInstanceFromRowKeys(string rowKeysFileName, ParallelOptions parallelOptions, FileAccess fileAccess = FileAccess.Read, FileShare fileShare = FileShare.Read)
        {
            RowKeysPaddedDouble rowKeysPaddedDouble = new RowKeysPaddedDouble();
            rowKeysPaddedDouble.GetInstanceFromRowKeysStructFileNameInternal(rowKeysFileName, parallelOptions, fileAccess, fileShare);
            return rowKeysPaddedDouble;
        }
    }
}
