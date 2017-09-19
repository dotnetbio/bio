using System;
using System.IO;

using Bio.IO;
using Bio.Phylogenetics;

namespace Bio
{
    /// <summary>
    /// Phylo Tree Formatter extensions.
    /// </summary>
    public static class PhylogeneticTreeFormatterExtensions
    {
        /// <summary>
        /// Open a file and parse it with the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="filename">Filename</param>
        /// <returns>IDisposable to close stream.</returns>
        public static IDisposable Open(this IPhylogeneticTreeFormatter formatter, string filename)
        {
            return ParserFormatterExtensions<IPhylogeneticTreeFormatter>.Open(formatter, filename);
        }

        /// <summary>
        /// Writes a single data element to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="data">Tree Data</param>
        public static void Format(this IPhylogeneticTreeFormatter formatter, Tree data) 
        {
            var fs = ParserFormatterExtensions<IPhylogeneticTreeFormatter>.GetOpenStream(formatter, true);
            if (fs != null)
                formatter.Format(fs, data);
            else
                throw new Exception("You must open a formatter before calling Write.");
        }

        /// <summary>
        /// Writes a single sequence to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="data">Tree data</param>
        /// <param name="filename">Filename</param>
        public static void Format(this IPhylogeneticTreeFormatter formatter, Tree data, string filename)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }

            using (var fs = File.Create(filename))
                formatter.Format(fs, data);
        }

        /// <summary>
        /// Closes the formatter.
        /// </summary>
        /// <param name="formatter">Formatter.</param>
        public static void Close(this IPhylogeneticTreeFormatter formatter)
        {
            ParserFormatterExtensions<IPhylogeneticTreeFormatter>.Close(formatter);
        }

    }
}
