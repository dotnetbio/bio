using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bio.IO.Wiggle
{
    /// <summary>
    /// Extensions to the WiggleParser to support Open/Close/Dispose semantics.
    /// </summary>
    public static class WiggleParserExtensions
    {
        /// <summary>
        /// Opens a sequence file using the parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="filename">File to open</param>
        /// <returns>Disposable object to close the stream.</returns>
        public static IDisposable Open(this WiggleParser parser, string filename)
        {
            return ParserFormatterExtensions<WiggleParser>.Open(parser, filename);
        }

        /// <summary>
        /// Parses the sequences from the open file.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <returns>Set of parsed sequences.</returns>
        public static IEnumerable<WiggleAnnotation> Parse(this WiggleParser parser)
        {
            var fs = ParserFormatterExtensions<WiggleParser>.GetOpenStream(parser, false);
            if (fs != null)
            {
                foreach (var item in parser.Parse(fs))
                    yield return item;
            }
        }

        /// <summary>
        /// Parses the sequences from the given filename.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <param name="fileName">Filename to open/close</param>
        /// <returns>Set of parsed sequences.</returns>
        public static IEnumerable<WiggleAnnotation> Parse(this WiggleParser parser, string fileName)
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
        /// <param name="parser">Sequence Parser</param>
        /// <param name="fileName">Filename to open/close</param>
        /// <returns>Set of parsed sequences.</returns>
        public static WiggleAnnotation ParseOne(this WiggleParser parser, string fileName)
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
        public static void Close(this WiggleParser parser)
        {
            ParserFormatterExtensions<WiggleParser>.Close(parser);
        }
    }
}
