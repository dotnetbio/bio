using System.Collections.Generic;
using System.IO;
using Bio.IO.AppliedBiosystems.DataTypes;

namespace Bio.IO.AppliedBiosystems.DataParsers
{
    /// <summary>
    /// Simple parser context.
    /// </summary>
    public class ParserContext : IParserContext
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ParserContext()
        {
            DataItems = new List<IAb1DataItem>();
        }

        #region Implementation of IParserContext

        /// <summary>
        /// File reader.
        /// </summary>
        public BinaryReader Reader { get; set; }

        /// <summary>
        /// Header identifying the abi file version.
        /// </summary>
        public Ab1Header Header { get; set; }

        /// <summary>
        /// Alphabet to use when creating sequences
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        /// Data found within the abi file.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<IAb1DataItem> DataItems { get; private set; }

        #endregion
    }
}
