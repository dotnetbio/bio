using System.Collections.Generic;
using System.IO;

namespace Bio.IO
{
    /// <summary>
    ///     Writes out SequenceRange lists or groupings to a data stream.
    /// </summary>
    public interface ISequenceRangeFormatter : IFormatter
    {
        /// <summary>
        ///     Writes out a list of ISequenceRange objects to a specified
        ///     stream.
        /// </summary>
        void Format(Stream stream, IList<ISequenceRange> ranges);

        /// <summary>
        ///     Writes out a grouping of ISequenceRange objects to a specified
        ///     text writer.
        /// </summary>
        void Format(Stream stream, SequenceRangeGrouping rangeGroup);
    }
}