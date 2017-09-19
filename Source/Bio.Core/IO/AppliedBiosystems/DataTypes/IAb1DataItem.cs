using Bio.IO.AppliedBiosystems.DataParsers;

namespace Bio.IO.AppliedBiosystems.DataTypes
{
    /// <summary>
    /// Data item interface.
    /// </summary>
    public interface IAb1DataItem
    {
        /// <summary>
        /// The name of the data item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Element type definition.
        /// </summary>
        int Type { get; }

        /// <summary>
        /// Size in bytes of the element.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Directory entry defining this element.
        /// </summary>
        Ab1DirectoryEntry Entry { get; set; }

        /// <summary>
        /// Accept a visitor.
        /// </summary>
        /// <param name="visitor"></param>
        void Accept(IAb1DataVisitor visitor);

        /// <summary>
        /// Create an instance of the derived type.
        /// </summary>
        /// <returns></returns>
        IAb1DataItem Create();
    }
}
