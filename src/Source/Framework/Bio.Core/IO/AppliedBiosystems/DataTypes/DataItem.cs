using Bio.IO.AppliedBiosystems.DataParsers;

namespace Bio.IO.AppliedBiosystems.DataTypes
{
    /// <summary>
    /// Data within applied biosystems format is broken up into individual data items, this is a base class containing common properties.
    /// </summary>
    public abstract class DataItem : IAb1DataItem
    {
        #region Implementation of IAb1DataItem

        /// <summary>
        /// The name of the data item.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Element type definition.
        /// </summary>
        public abstract int Type { get; }

        /// <summary>
        /// Size in bytes of the element.
        /// </summary>
        public abstract int Size { get; }

        /// <summary>
        /// Directory entry defining this element.
        /// </summary>
        public Ab1DirectoryEntry Entry { get; set; }

        /// <summary>
        /// Accept a visitor.
        /// </summary>
        /// <param name="visitor"></param>
        public abstract void Accept(IAb1DataVisitor visitor);

        /// <summary>
        /// Create an instance of the derived type.
        /// </summary>
        /// <returns></returns>
        public abstract IAb1DataItem Create();

        #endregion
    }
}
