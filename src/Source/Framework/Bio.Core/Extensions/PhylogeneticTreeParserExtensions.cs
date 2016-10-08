using System;
using System.IO;

using Bio.IO;
using Bio.Phylogenetics;

namespace Bio
{
    /// <summary>
    /// Extensions for the Newick parser and variations.
    /// </summary>
    public static class PhylogeneticTreeParserExtensions
    {
        /// <summary>
        /// Opens a sequence file using the parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="filename">File to open</param>
        /// <returns>Disposable object to close the stream.</returns>
        public static IDisposable Open(this IPhylogeneticTreeParser parser, string filename)
        {
            return ParserFormatterExtensions<IPhylogeneticTreeParser>.Open(parser, filename);
        }

        /// <summary>
        /// Parses the sequences from the open file.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <returns>Set of parsed sequences.</returns>
        public static Tree Parse(this IPhylogeneticTreeParser parser)
        {
            var fs = ParserFormatterExtensions<IPhylogeneticTreeParser>.GetOpenStream(parser, false);
            if (fs == null)
                throw new Exception("You must open a parser before calling Parse.");

            return parser.Parse(fs);
        }

        /// <summary>
        /// Parses the sequences from the open file.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <param name="filename">Filename to open</param>
        /// <returns>Set of parsed sequences.</returns>
        public static Tree Parse(this IPhylogeneticTreeParser parser, string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                return parser.Parse(fs);
            }
        }

        /// <summary>
        /// Closes the parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        public static void Close(this IPhylogeneticTreeParser parser)
        {
            ParserFormatterExtensions<IPhylogeneticTreeParser>.Close(parser);
        }

    }
}
