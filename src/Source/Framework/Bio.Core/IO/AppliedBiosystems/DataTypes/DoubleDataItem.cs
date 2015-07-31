using System;
using Bio.IO.AppliedBiosystems.DataParsers;

namespace Bio.IO.AppliedBiosystems.DataTypes
{
    /// <summary>
    /// Double data item.
    /// </summary>
    public class DoubleDataItem : DataItem
    {
        /// <summary>
        /// Item value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public double[] Value { get; set; }

        /// <summary>
        /// The name of the data item.
        /// </summary>
        public override string Name
        {
            get { return "double"; }
        }

        /// <summary>
        /// Element type definition.
        /// </summary>
        public override int Type
        {
            get { return 8; }
        }

        /// <summary>
        /// Size in bytes of the element.
        /// </summary>
        public override int Size
        {
            get { return 8; }
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
        /// Create an instance of the double type.
        /// </summary>
        /// <returns></returns>
        public override IAb1DataItem Create()
        {
            return new DoubleDataItem();
        }
    }
}
