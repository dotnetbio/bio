using System;
using System.IO;

using Bio.Algorithms.Alignment;
using Bio.IO.SAM;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Extensions specific to the BAMFormatter class.
    /// </summary>
    public static class BAMFormatterExtensions
    {
        /// <summary>
        /// Write out the given SequenceAlignmentMap to the file
        /// </summary>
        /// <param name="formatter">BAMFormatter</param>
        /// <param name="sam">SequenceAlignmentMap</param>
        /// <param name="filename">File to write to</param>
        /// <param name="indexFilename">BAM index file</param>
        public static void Format(this BAMFormatter formatter, ISequenceAlignment sam, string filename, string indexFilename)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            if (sam == null)
            {
                throw new ArgumentNullException("sam");
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }
            if (string.IsNullOrWhiteSpace(indexFilename))
            {
                throw new ArgumentNullException("indexFilename");
            }

            if (filename == indexFilename)
            {
                throw new ArgumentException("Use different filenames for index and alignment.", "indexFilename");
            }

            using (var fs = File.Create(filename))
            using (var bamIndexFile = new BAMIndexStorage(File.Create(indexFilename)))
            {
                formatter.Format(fs, bamIndexFile, sam);
            }
        }

        /// <summary>
        /// Write out the given SequenceAlignmentMap to the file
        /// </summary>
        /// <param name="formatter">BAMFormatter</param>
        /// <param name="sam">SequenceAlignmentMap</param>
        /// <param name="filename">File to write to</param>
        public static void Format(this BAMFormatter formatter, SequenceAlignmentMap sam, string filename)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            if (sam == null)
            {
                throw new ArgumentNullException("sam");
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }

            using (var fs = File.Create(filename))
            {
                // Create the IndexFile if necessary
                if (formatter.CreateIndexFile)
                {
                    using (var bamIndexFile = new BAMIndexStorage(
                        File.Create(filename + Properties.Resource.BAM_INDEXFILEEXTENSION)))
                    {
                        formatter.Format(fs, bamIndexFile, sam);
                    }
                }
                else
                {
                    formatter.Format(fs, sam);
                }
            }
        }

    }
}
