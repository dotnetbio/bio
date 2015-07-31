using System;
using System.IO;
using Bio.Algorithms.Assembly;

namespace Bio.IO.Xsv
{
    /// <summary>
    /// Formatter extensions for the XsvContigFormatter
    /// </summary>
    public static class XsvContigFormatterExtensions
    {
        /// <summary>
        /// Write out a set of contigs to the given file.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="contig">Contig to write</param>
        /// <param name="filename">Filename</param>
        public static void Format(this XsvContigFormatter formatter, Contig contig, string filename)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            if (contig == null)
            {
                throw new ArgumentNullException("contig");
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }

            using (var fs = File.Create(filename))
            {
                formatter.Write(fs, contig);
            }
        }

        /// <summary>
        /// Write out a set of contigs to the given file.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="contig">Contig to write</param>
        public static void Format(this XsvContigFormatter formatter, Contig contig)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            if (contig == null)
            {
                throw new ArgumentNullException("contig");
            }
            var fs = ParserFormatterExtensions<ISequenceFormatter>.GetOpenStream(formatter, true);
            if (fs != null)
                formatter.Write(fs, contig);
            else
                throw new Exception("You must open a formatter before calling Write.");
        }
    }
}
