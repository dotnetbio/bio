using System.Collections.Generic;
using System.IO;

namespace Bio.IO
{
    /// <summary>
    /// The interface defining the methods for parsing ISequenceRange
    /// objects from files or readers.
    /// </summary>
    public interface ISequenceRangeParser : IParser
    {
        /// <summary>
        /// Parse a set of ISequenceRange objects from a stream.
        /// </summary>
        IList<ISequenceRange> ParseRange(Stream stream);

        /// <summary>
        /// Parse a set of ISequenceRange objects into a SequenceRange
        /// grouping from a stream.
        /// </summary>
        SequenceRangeGrouping ParseRangeGrouping(Stream stream);
    }
}
