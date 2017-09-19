using System;
using System.Collections.Generic;
using System.IO;

using Bio.IO.SAM;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Parser extensions for the BAM parsers.
    /// </summary>
    public static class BAMParserExtensions
    {
        /// <summary>
        /// Returns an iterator over a set of SAMAlignedSequences retrieved from a parsed BAM file.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="filename">Filename</param>
        /// <returns>IEnumerable SAMAlignedSequence object.</returns>
        public static IEnumerable<SAMAlignedSequence> Parse(this BAMParser parser, string filename)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            using (var fs = File.OpenRead(filename))
            {
                foreach (var item in parser.Parse(fs))
                    yield return item;
            }
        }

        /// <summary>
        /// Parses specified BAM file using index file.
        /// Index file is assumed to be in the same location as that of the specified bam file with the name "filename".bai
        /// For example, if the specified bam file name is D:\BAMdata\sample.bam then index file name will be taken as D:\BAMdata\sample.bam.bai
        /// If index file is not available then this method throw an exception.
        /// </summary>
        /// <param name="parser">BAM parser</param>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="refSeqName">Name of reference sequence.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments for specified reference sequence.</returns>
        public static SequenceAlignmentMap ParseRange(this BAMParser parser, string fileName, string refSeqName)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (refSeqName == null)
            {
                throw new ArgumentNullException("refSeqName");
            }

            using (var bamStream = File.OpenRead(fileName))
            {
                string bamIndexFileName = GetBAMIndexFileName(fileName);
                using (FileStream bamIndexFile = File.OpenRead(bamIndexFileName))
                using (var bamIndexStorage = new BAMIndexStorage(bamIndexFile))
                {
                    return parser.GetAlignment(bamStream, bamIndexStorage, refSeqName);
                }
            }
        }


        /// <summary>
        /// Attempts to find the name of an index file for the given BAM file name, throws an error if none is found.
        /// </summary>
        /// <param name="fileName">The name of the BAM file.</param>
        /// <returns>The name of the index file for the given BAM file.</returns>
        private static string GetBAMIndexFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            //Try Name+".bai"
            string possibleName = fileName + ".bai";
            if (File.Exists(possibleName))
            {
                return possibleName;
            }

            //Try to remove .bam and replace it with .bai
            possibleName = Path.GetFileNameWithoutExtension(fileName) + ".bai";
            if (File.Exists(possibleName))
                return possibleName;

            throw new FileNotFoundException("Could not find BAM Index file for: " 
                + fileName + " you may need to create an index file before parsing it.");
        }

        /// <summary>
        /// Parses specified BAM file using index file.
        /// Index file is assumed to be in the same location as that of the specified bam file with the name "filename".bai
        /// For example, if the specified bam file name is D:\BAMdata\sample.bam then index file name will be taken as D:\BAMdata\sample.bam.bai
        /// If index file is not available then this method throw an exception.
        /// </summary>
        /// <param name="parser">BAM parser</param>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="refSeqName">Name of reference sequence.</param>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments overlaps with the specified start 
        /// and end co-ordinate of the specified reference sequence.</returns>
        public static SequenceAlignmentMap ParseRange(this BAMParser parser, string fileName, string refSeqName, int start, int end)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (refSeqName == null)
            {
                throw new ArgumentNullException("refSeqName");
            }

            using (FileStream bamStream = File.OpenRead(fileName))
            {
                string bamIndexFileName = GetBAMIndexFileName(fileName);
                using (FileStream bamIndexFile = File.OpenRead(bamIndexFileName))
                using (BAMIndexStorage bamIndexStorage = new BAMIndexStorage(bamIndexFile))
                {
                    return parser.GetAlignment(bamStream, bamIndexStorage, refSeqName, start, end);
                }
            }
        }

        /// <summary>
        /// Parses specified BAM file using index file.
        /// Index file is assumed to be in the same location as that of the specified bam file with the name "filename".bai
        /// For example, if the specified bam file name is D:\BAMdata\sample.bam then index file name will be taken as D:\BAMdata\sample.bam.bai
        /// If index file is not available then this method throw an exception.
        /// </summary>
        /// <param name="parser">BAM parser</param>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="refSeqIndex">Index of reference sequence.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments for specified reference sequence.</returns>
        public static SequenceAlignmentMap ParseRange(this BAMParser parser, string fileName, int refSeqIndex)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            using (FileStream bamStream = File.OpenRead(fileName))
            {
                string bamIndexFileName = GetBAMIndexFileName(fileName);
                using (FileStream bamIndexFile = File.OpenRead(bamIndexFileName))
                using (BAMIndexStorage bamIndexStorage = new BAMIndexStorage(bamIndexFile))
                {
                    return parser.GetAlignment(bamStream, bamIndexStorage, refSeqIndex);
                }
            }
        }

        /// <summary>
        /// Parses specified BAM file using index file.
        /// Index file is assumed to be in the same location as that of the specified bam file with the name "filename".bai
        /// For example, if the specified bam file name is D:\BAMdata\sample.bam then index file name will be taken as D:\BAMdata\sample.bam.bai
        /// If index file is not available then this method throw an exception.
        /// </summary>
        /// <param name="parser">BAM parser</param>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="refSeqIndex">Index of reference sequence.</param>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments overlaps with the specified start 
        /// and end co-ordinate of the specified reference sequence.</returns>
        public static SequenceAlignmentMap ParseRange(this BAMParser parser, string fileName, int refSeqIndex, int start, int end)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            using (FileStream bamStream = File.OpenRead(fileName))
            {
                string bamIndexFileName = GetBAMIndexFileName(fileName);
                using (FileStream bamIndexFile = File.OpenRead(bamIndexFileName))
                using (BAMIndexStorage bamIndexStorage = new BAMIndexStorage(bamIndexFile))
                {
                    return parser.GetAlignment(bamStream, bamIndexStorage, refSeqIndex, start, end);
                }
            }
        }

        /// <summary>
        /// Parses specified BAM file using index file.
        /// </summary>
        /// <param name="parser">BAM parser</param>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="range">SequenceRange object which contains reference sequence name and start and end co-ordinates.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments for specified reference sequence and for specified range.</returns>
        public static SequenceAlignmentMap ParseRange(this BAMParser parser, string fileName, SequenceRange range)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            if (string.IsNullOrEmpty(range.ID))
            {
                throw new ArgumentException("Reference sequence name (range.ID) can't empty or null.");
            }

            int start = range.Start >= int.MaxValue ? int.MaxValue : (int)range.Start;
            int end = range.End >= int.MaxValue ? int.MaxValue : (int)range.End;

            using (FileStream bamStream = File.OpenRead(fileName))
            {
                string bamIndexFileName = GetBAMIndexFileName(fileName);
                using (FileStream bamIndexFile = File.OpenRead(bamIndexFileName))
                using (BAMIndexStorage bamIndexStorage = new BAMIndexStorage(bamIndexFile))
                {
                    if (start == 0 && end == int.MaxValue)
                        return parser.GetAlignment(bamStream, bamIndexStorage, range.ID);
                    return parser.GetAlignment(bamStream, bamIndexStorage, range.ID, start, end);

                }
            }
        }
    }
}
