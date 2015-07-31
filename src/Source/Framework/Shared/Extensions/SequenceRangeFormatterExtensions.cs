using System;
using System.Collections.Generic;
using System.IO;

using Bio.IO;

namespace Bio
{
    /// <summary>
    /// Extensions for the SequenceRange Formatters.
    /// </summary>
    public static class SequenceRangeFormatterExtensions
    {
        /// <summary>
        /// Formats a given ISequenceRange to the given formatter and filename.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="ranges">ISequenceRange elements</param>
        /// <param name="filename">Filename to write to</param>
        public static void Format(this ISequenceRangeFormatter formatter, IList<ISequenceRange> ranges, string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            using (FileStream fs = File.Create(filename))
            {
                formatter.Format(fs, ranges);
            }
        }

        /// <summary>
        /// Writes out a grouping of ISequenceRange objects to a specified
        /// text writer.
        /// </summary>
        public static void Format(this ISequenceRangeFormatter formatter, SequenceRangeGrouping rangeGroup, string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");    
            }

            using (FileStream fs = File.Create(filename))
            {
                formatter.Format(fs, rangeGroup);
            }
        }
    }
}