using System;
using System.Collections.Generic;
using System.IO;

using Bio.IO;
using Bio.Phylogenetics;

namespace Bio
{
    /// <summary>
    /// Generic formatter extensions
    /// </summary>
    public static class FormatterExtensions
    {
        /// <summary>
        /// Open a file and parse it with the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="filename">Filename</param>
        /// <returns>IDisposable to close stream.</returns>
        public static IDisposable Open<T>(this IFormatter<T> formatter, string filename) where T : class
        {
            return ParserFormatterExtensions<IFormatter<T>>.Open(formatter, filename);
        }

        /// <summary>
        /// Writes a set of data to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="data">Data to write.</param>
        public static void Format<T>(this IFormatter<T> formatter, IEnumerable<T> data) where T : class
        {
            var fs = ParserFormatterExtensions<IFormatter<T>>.GetOpenStream(formatter, true);
            if (fs != null)
                formatter.Format(fs, data);
            else
                throw new Exception("You must open a formatter before calling Write.");
        }

        /// <summary>
        /// Writes a single data element to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="data">Data</param>
        public static void Format<T>(this IFormatter<T> formatter, T data) where T : class
        {
            var fs = ParserFormatterExtensions<IFormatter<T>>.GetOpenStream(formatter, true);
            if (fs != null)
                formatter.Format(fs, data);
            else
                throw new Exception("You must open a formatter before calling Write.");
        }

        /// <summary>
        /// Writes a set of data to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="data">Data to write.</param>
        /// <param name="filename">Filename</param>
        public static void Format<T>(this IFormatter<T> formatter, IEnumerable<T> data, string filename) where T : class
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
        /// Writes a single sequence to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="data">Sequence</param>
        /// <param name="filename">Filename</param>
        public static void Format<T>(this IFormatter<T> formatter, T data, string filename) where T : class
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
        public static void Close<T>(this IFormatter<T> formatter) where T : class
        {
            ParserFormatterExtensions<IFormatter<T>>.Close(formatter);
        }

        /// <summary>
        /// Formats the Phylogenetic tree to a string.
        /// </summary>
        /// <param name="formatter">Sequence formatter</param>
        /// <param name="tree">Tree data</param>
        /// <returns></returns>
        public static string FormatString(this IPhylogeneticTreeFormatter formatter, Tree tree)
        {
            return DoFormat(formatter.Format, tree);
        }

        /// <summary>
        /// Formats the Wiggle annotation to a string.
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="data"></param>
        public static string FormatString<T>(this IFormatter<T> formatter, T data)
        {
            return DoFormat(formatter.Format, data);
        }

        /// <summary>
        /// Performs a format to a memory stream and returns the result as a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        static string DoFormat<T>(Action<Stream,T> formatter, T data)
        {
            var str = new MemoryStream(1024);
            formatter(str, data);
            str.Position = 0;
            using (StreamReader sr = new StreamReader(str))
                return sr.ReadToEnd();
        }
    }
}
