using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.IO;

namespace Bio
{
    /// <summary>
    /// Extensions to the ISequenceAlignmentParser to support Open/Close/Dispose semantics.
    /// </summary>
    public static class SequenceAlignmentParserExtensions
    {
        /// <summary>
        /// Opens a sequence file using the parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="filename">File to open</param>
        /// <returns>Disposable object to close the stream.</returns>
        public static IDisposable Open(this ISequenceAlignmentParser parser, string filename)
        {
            return ParserFormatterExtensions<ISequenceAlignmentParser>.Open(parser, filename);
        }

        /// <summary>
        /// Parses the sequences from the open file.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <returns>Set of parsed sequences.</returns>
        public static IEnumerable<ISequenceAlignment> Parse(this ISequenceAlignmentParser parser)
        {
            var fs = ParserFormatterExtensions<ISequenceAlignmentParser>.GetOpenStream(parser, false);
            if (fs == null)
                throw new Exception("You must open a parser before calling Parse.");

            return parser.Parse(fs);
        }

        /// <summary>
        /// Parses the sequences from the open file.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <returns>Set of parsed sequences.</returns>
        public static ISequenceAlignment ParseOne(this ISequenceAlignmentParser parser)
        {
            var fs = ParserFormatterExtensions<ISequenceAlignmentParser>.GetOpenStream(parser, false);
            return parser.ParseOne(fs);
        }

        /// <summary>
        /// Parses the sequences from the open file.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <param name="filename">File to parse</param>
        /// <returns>Set of parsed sequences.</returns>
        public static ISequenceAlignment ParseOne(this ISequenceAlignmentParser parser, string filename)
        {
            using (var fs = File.OpenRead(filename))
                return parser.ParseOne(fs);
        }

        /// <summary>
        /// Parses the sequences from the open file.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <param name="filename">File to parse</param>
        /// <returns>Set of parsed sequences.</returns>
        public static T ParseOne<T>(this ISequenceAlignmentParser parser, string filename)
            where T : ISequenceAlignment
        {
            using (var fs = File.OpenRead(filename))
                return (T) parser.ParseOne(fs);
        }

        /// <summary>
        /// Parses the sequences from the given filename.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <param name="fileName">Filename to open/close</param>
        /// <returns>Set of parsed sequences.</returns>
        public static IEnumerable<ISequenceAlignment> Parse(this ISequenceAlignmentParser parser, string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                foreach (var item in parser.Parse(fs))
                    yield return item;
            }
        }

        /// <summary>
        /// Closes the parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        public static void Close(this ISequenceAlignmentParser parser)
        {
            ParserFormatterExtensions<ISequenceAlignmentParser>.Close(parser);
        }
    }
}
