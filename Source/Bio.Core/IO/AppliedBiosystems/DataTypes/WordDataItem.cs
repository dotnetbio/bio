using System;
using Bio.IO.AppliedBiosystems.DataParsers;

namespace Bio.IO.AppliedBiosystems.DataTypes
{
    /// <summary>
    /// Work data item.
    /// </summary>
    public class WordDataItem : DataItem
    {
        /// <summary>
        /// Item value.
        /// 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public ushort[] Value { get; set; }

        /// <summary>
        /// The name of the data item.
        /// </summary>
        public override string Name
        {
            get { return "word"; }
        }

        /// <summary>
        /// Element type definition.
        /// </summary>
        public override int Type
        {
            get { return 3; }
        }

        /// <summary>
        /// Size in bytes of the element.
        /// </summary>
        public override int Size
        {
            get { return 2; }
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
        /// Create an instance of the word type.
        /// </summary>
        /// <returns></returns>
        public override IAb1DataItem Create()
        {
            return new WordDataItem();
        }
    }
}
