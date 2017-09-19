using System;
using System.Collections.Generic;
using System.IO;

using Bio.IO;

namespace Bio
{
    /// <summary>
    /// SequenceRange Parser extensions.
    /// </summary>
    public static class SequenceRangeParserExtensions
    {
        /// <summary>
        /// Parse a set of ISequenceRange objects from a stream.
        /// </summary>
        public static IList<ISequenceRange> ParseRange(this ISequenceRangeParser parser, string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                return parser.ParseRange(fs);
            }
        }

        /// <summary>
        /// Parse a set of ISequenceRange objects into a SequenceRange
        /// grouping from a stream.
        /// </summary>
        public static SequenceRangeGrouping ParseRangeGrouping(this ISequenceRangeParser parser, string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                return parser.ParseRangeGrouping(fs);
            }
        }
    }
}
