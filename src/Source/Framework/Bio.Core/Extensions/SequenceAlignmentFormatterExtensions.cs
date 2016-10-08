using System;
using System.Collections.Generic;
using System.IO;

using Bio.Algorithms.Alignment;
using Bio.IO;
using Bio.IO.BAM;
using Bio.IO.SAM;

namespace Bio
{
    /// <summary>
    /// Extension methods for sequence formatters
    /// </summary>
    public static class SequenceAlignmentFormatterExtensions
    {
        /// <summary>
        /// Open a file and parse it with the sequence formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="filename">Filename</param>
        /// <returns>IDisposable to close stream.</returns>
        public static IDisposable Open(this ISequenceAlignmentFormatter formatter, string filename)
        {
            return ParserFormatterExtensions<ISequenceAlignmentFormatter>.Open(formatter, filename);
        }

        /// <summary>
        /// Writes a set of sequences to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="sequences">Sequences to write.</param>
        public static void Format(this ISequenceAlignmentFormatter formatter, IEnumerable<ISequenceAlignment> sequences)
        {
            var fs = ParserFormatterExtensions<ISequenceAlignmentFormatter>.GetOpenStream(formatter, true);
            if (fs != null)
                formatter.Format(fs, sequences);
            else
                throw new Exception("You must open a formatter before calling Write.");
        }

        /// <summary>
        /// Writes a single sequence to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="sequence">Sequence</param>
        public static void Format(this ISequenceAlignmentFormatter formatter, ISequenceAlignment sequence)
        {
            var fs = ParserFormatterExtensions<ISequenceAlignmentFormatter>.GetOpenStream(formatter, true);
            if (fs != null)
                formatter.Format(fs, sequence);
            else
                throw new Exception("You must open a formatter before calling Write.");
        }

        /// <summary>
        /// Writes a set of sequences to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="sequences">Sequences to write.</param>
        /// <param name="fileName">Filename to write to</param>
        public static void Format(this ISequenceAlignmentFormatter formatter, IEnumerable<ISequenceAlignment> sequences, string fileName)
        {
            using (FileStream fs = File.OpenWrite(fileName))
            {
                formatter.Format(fs, sequences);
            }
        }

        /// <summary>
        /// Writes a sequence to the formatter.
        /// </summary>
        /// <param name="formatter">Formatter</param>
        /// <param name="sequence">Sequence to write.</param>
        /// <param name="fileName">Filename to write to</param>
        public static void Format(this ISequenceAlignmentFormatter formatter, ISequenceAlignment sequence, string fileName)
        {
            // In case this extensions method is in scope, we will forward
            // to the BAMFormatter extension to properly handle the index
            // file.
            if (formatter is BAMFormatter
                && sequence is SequenceAlignmentMap)
            {
                BAMFormatterExtensions.Format((BAMFormatter)formatter, 
                    (SequenceAlignmentMap)sequence, fileName);
            }
            else
            {
                using (FileStream fs = File.Create(fileName))
                {
                    formatter.Format(fs, sequence);
                }
            }
        }

        /// <summary>
        /// Closes the formatter.
        /// </summary>
        /// <param name="formatter">Formatter.</param>
        public static void Close(this ISequenceAlignmentFormatter formatter)
        {
            ParserFormatterExtensions<ISequenceAlignmentFormatter>.Close(formatter);
        }
    }
}