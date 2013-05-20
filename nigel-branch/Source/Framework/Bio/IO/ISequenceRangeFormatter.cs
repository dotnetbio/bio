using System.Collections.Generic;
using System.IO;
using System.ComponentModel.Composition;


namespace Bio.IO
{
    /// <summary>
    /// Writes out SequenceRange lists or groupings to a file.
    /// </summary>
    [InheritedExport(".NetBioSequenceRangeFormattersExport", typeof(ISequenceRangeFormatter))]
    public interface ISequenceRangeFormatter
    {
        /// <summary>
        /// Writes out a list of ISequenceRange objects to a specified
        /// file location.
        /// </summary>
        void Format(IList<ISequenceRange> ranges, string fileName);

        /// <summary>
        /// Writes out a list of ISequenceRange objects to a specified
        /// text writer.
        /// </summary>
        void Format(IList<ISequenceRange> ranges, TextWriter writer);

        /// <summary>
        /// Writes out a grouping of ISequenceRange objects to a specified
        /// file location.
        /// </summary>
        void Format(SequenceRangeGrouping rangeGroup, string fileName);

        /// <summary>
        /// Writes out a grouping of ISequenceRange objects to a specified
        /// text writer.
        /// </summary>
        void Format(SequenceRangeGrouping rangeGroup, TextWriter writer);

        /// <summary>
        /// Gets the name of the sequence range formatter being
        /// implemented. This is intended to give the
        /// developer some information of the formatter type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the sequence range formatter being
        /// implemented. This is intended to give the
        /// developer some information of the formatter.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the file extensions that the formatter implementation
        /// will support.
        /// </summary>
        string FileTypes { get; }
    }
}
