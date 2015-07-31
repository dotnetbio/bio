using System;
using System.IO;

using Bio.Algorithms.Assembly;

namespace Bio.IO.Xsv
{
    /// <summary>
    /// XsvContigParser extensions
    /// </summary>
    public static class XsvContigParserExtensions
    {
        /// <summary>
        /// Parse out a Contig from the given file.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="filename">Filename</param>
        /// <returns>Contig</returns>
        public static Contig ParseContig(this XsvContigParser parser, string filename)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }

            using (var fs = File.OpenRead(filename))
            {
                return parser.ParseContig(fs);
            }
        }
    }
}