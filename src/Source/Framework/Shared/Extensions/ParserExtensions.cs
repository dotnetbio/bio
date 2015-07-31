using System;
using System.Collections.Generic;
using System.IO;

using Bio.IO;

namespace Bio
{
    /// <summary>
    /// Extensions to the IParser(T) to support Open/Close/Dispose semantics.
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Opens a sequence file using the parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="filename">File to open</param>
        /// <returns>Disposable object to close the stream.</returns>
        public static IDisposable Open<T>(this IParser<T> parser, string filename)
        {
            return ParserFormatterExtensions<IParser<T>>.Open(parser, filename);
        }

        /// <summary>
        /// Parses the data from the open file.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <returns>Set of parsed data elements.</returns>
        public static IEnumerable<T> Parse<T>(this IParser<T> parser)
        {
            var fs = ParserFormatterExtensions<IParser<T>>.GetOpenStream(parser, false);
            if (fs == null)
                throw new Exception("You must open a parser before calling Parse.");

            return parser.Parse(fs);
        }

        /// <summary>
        /// Parses the data from the open file.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <returns>Set of parsed data elements.</returns>
        public static T ParseOne<T>(this IParser<T> parser)
        {
            var fs = ParserFormatterExtensions<IParser<T>>.GetOpenStream(parser, false);
            if (fs == null)
                throw new Exception("You must open a parser before calling ParseOne.");

            return parser.ParseOne(fs);
        }

        /// <summary>
        /// Parses the sequences from the given filename.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="fileName">Filename to open/close</param>
        /// <returns>Set of parsed data elements.</returns>
        public static IEnumerable<T> Parse<T>(this IParser<T> parser, string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                foreach (var item in parser.Parse(fs))
                    yield return item;
            }
        }

        /// <summary>
        /// Parses the sequences from the given filename.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="fileName">Filename to open/close</param>
        /// <returns>Set of parsed data elements.</returns>
        public static T ParseOne<T>(this IParser<T> parser, string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                return parser.ParseOne(fs);
            }
        }

        /// <summary>
        /// Closes the parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        public static void Close<T>(this IParser<T> parser)
        {
            ParserFormatterExtensions<IParser<T>>.Close(parser);
        }
    }
}
