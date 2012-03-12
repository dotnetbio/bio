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
        /// Parse a set of ISequenceRange objects from a data source.
        /// </summary>
        IList<ISequenceRange> ParseRange(string dataSource);

        /// <summary>
        /// Parse a set of ISequenceRange objects into a SequenceRange
        /// grouping from a data source.
        /// </summary>
        SequenceRangeGrouping ParseRangeGrouping(string dataSource);

        /// <summary>
        /// Parse a set of ISequenceRange objects from a reader.
        /// </summary>
        IList<ISequenceRange> ParseRange(TextReader reader);

        /// <summary>
        /// Parse a set of ISequenceRange objects into a SequenceRange
        /// grouping from a reader.
        /// </summary>
        SequenceRangeGrouping ParseRangeGrouping(TextReader reader);
    }
}
