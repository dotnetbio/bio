using System.Collections.Generic;
using System.IO;
using Bio.IO.AppliedBiosystems.DataTypes;

namespace Bio.IO.AppliedBiosystems.DataParsers
{
    /// <summary>
    /// Context for managing file import.  After import this contains all data associated in the file.
    /// </summary>
    public interface IParserContext
    {
        /// <summary>
        /// File reader.
        /// </summary>
        BinaryReader Reader { get; }

        /// <summary>
        /// Header identifying the abi file version.
        /// </summary>
        Ab1Header Header { get; }

        /// <summary>
        /// Alphabet to use when creating sequences
        /// </summary>
        IAlphabet Alphabet { get; }

        /// <summary>
        /// Data found within the abi file.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        List<IAb1DataItem> DataItems { get; }
    }
}
