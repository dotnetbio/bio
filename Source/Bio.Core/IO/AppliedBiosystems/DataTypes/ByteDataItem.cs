using System;
using Bio.IO.AppliedBiosystems.DataParsers;

namespace Bio.IO.AppliedBiosystems.DataTypes
{
    /// <summary>
    /// Array of bytes data item.
    /// </summary>
    public class ByteDataItem : DataItem
    {
        /// <summary>
        /// Byte value;
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] Value { get; set; }

        /// <summary>
        /// The name of the data item.
        /// </summary>
        public override string Name
        {
            get { return "byte"; }
        }

        /// <summary>
        /// Element type definition.
        /// </summary>
        public override int Type
        {
            get { return 1; }
        }

        /// <summary>
        /// Size in bytes of the element.
        /// </summary>
        public override int Size
        {
            get { return 1; }
        }

        /// <summary>
        /// Accept a visitor.
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept(IAb1DataVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");
            visitor.Visit(this);
        }

        /// <summary>
        /// Create an instance of the byyte type.
        /// </summary>
        /// <returns></returns>
        public override IAb1DataItem Create()
        {
            return new ByteDataItem();
        }
    }
}
