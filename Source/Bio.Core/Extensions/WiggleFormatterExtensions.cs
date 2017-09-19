using System;
using System.Collections.Generic;
using System.IO;

namespace Bio.IO.Wiggle
{
    /// <summary>
    /// Extension methods for sequence formatters
    /// </summary>
    public static class WiggleFormatterExtensions
    {
        /// <summary>
        /// Open a file and parse it with the sequence formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="filename">Filename</param>
        /// <returns>IDisposable to close stream.</returns>
        public static IDisposable Open(this WiggleFormatter formatter, string filename)
        {
            return ParserFormatterExtensions<WiggleFormatter>
                .Open(formatter, filename);
        }

        /// <summary>
        /// Writes a set of sequences to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="annotations">Wiggle annotations to write.</param>
        public static void Format(this WiggleFormatter formatter, IEnumerable<WiggleAnnotation> annotations)
        {
            var fs = ParserFormatterExtensions<WiggleFormatter>.GetOpenStream(formatter, true);
            if (fs != null)
                formatter.Format(fs, annotations);
            else
                throw new Exception("You must open a formatter before calling Write.");
        }

        /// <summary>
        /// Writes a single sequence to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="annotation">Wiggle Annotation</param>
        public static void Format(this WiggleFormatter formatter, WiggleAnnotation annotation)
        {
            var fs = ParserFormatterExtensions<WiggleFormatter>.GetOpenStream(formatter, true);
            if (fs != null)
                formatter.Format(fs, annotation);
            else
                throw new Exception("You must open a formatter before calling Write.");
        }

        /// <summary>
        /// Writes a set of sequences to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="annotations">Wiggle annotations to write.</param>
        /// <param name="filename">Filename to write to</param>
        public static void Format(this WiggleFormatter formatter, IEnumerable<WiggleAnnotation> annotations, string filename)
        {
            using (var fs = File.Create(filename))
                formatter.Format(fs, annotations);
        }

        /// <summary>
        /// Writes a single sequence to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="annotation">Wiggle Annotation</param>
        /// <param name="filename">Filename</param>
        public static void Format(this WiggleFormatter formatter, WiggleAnnotation annotation, string filename)
        {
            using (var fs = File.Create(filename))
                formatter.Format(fs,annotation);
        }

        /// <summary>
        /// Closes the formatter.
        /// </summary>
        /// <param name="formatter">Formatter.</param>
        public static void Close(this WiggleFormatter formatter)
        {
            ParserFormatterExtensions<WiggleFormatter>.Close(formatter);
        }
    }
}