using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bio.Algorithms.Alignment;
using Bio.Util;

namespace Bio.IO.SAM
{
    /// <summary>
    /// Writes a SequenceAlignmentMap to a particular location, usually a file. 
    /// The output is formatted according to the SAM file format. 
    /// A method is also provided for quickly accessing the content in string 
    /// form for applications that do not need to first write to file.
    /// Documentation for the latest BAM file format can be found at
    /// http://samtools.sourceforge.net/SAM1.pdf
    /// </summary>
    public class SAMFormatter : ISequenceAlignmentFormatter
    {
        #region Constants
        /// <summary>
        /// Holds the format string needed for writing aligned sequence.
        /// </summary>
        private const string AlignedSequenceFormat = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}";

        /// <summary>
        /// Holds the format string needed for writing optional fields of aligned sequence.
        /// </summary>
        private const string OptionalFieldFormat = "\t{0}:{1}:{2}";
        #endregion
        #region Properties
        /// <summary>
        /// Gets the name of the sequence alignment formatter being
        /// implemented. This is intended to give the developer some
        /// information of the formatter type.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.SAM_NAME;
            }
        }

        /// <summary>
        /// Gets the description of the sequence alignment formatter being
        /// implemented. This is intended to give the developer some 
        /// information of the formatter.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.SAMFORMATTER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets the file extensions that the formatter implementation
        /// will support.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Properties.Resource.SAM_FILEEXTENSION;
            }
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Writes specified SAMAlignedHeader to specified text writer.
        /// </summary>
        /// <param name="header">Header to write.</param>
        /// <param name="writer">Text writer.</param>
        public static void WriteHeader(SAMAlignmentHeader header, TextWriter writer)
        {
            if (header == null)
            {
                return;
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            string message = header.IsValid();
            if (!string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(message);
            }

            StringBuilder headerLine = null;
            for (int i = 0; i < header.RecordFields.Count; i++)
            {
                headerLine = new StringBuilder();
                headerLine.Append("@");
                headerLine.Append(header.RecordFields[i].Typecode);
                for (int j = 0; j < header.RecordFields[i].Tags.Count; j++)
                {
                    headerLine.Append("\t");
                    headerLine.Append(header.RecordFields[i].Tags[j].Tag);
                    headerLine.Append(":");
                    headerLine.Append(header.RecordFields[i].Tags[j].Value);
                }

                writer.WriteLine(headerLine.ToString());
            }

            foreach (string comment in header.Comments)
            {
                headerLine = new StringBuilder();
                headerLine.Append("@CO");
                headerLine.Append("\t");
                headerLine.Append(comment);
                writer.WriteLine(headerLine.ToString());
            }

            writer.Flush();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Writes an ISequenceAlignment to the location specified by the writer.
        /// </summary>
        /// <param name="sequenceAlignment">The sequence alignment to format.</param>
        /// <param name="writer">The TextWriter used to write the formatted sequence alignment text.</param>
        public void Format(ISequenceAlignment sequenceAlignment, TextWriter writer)
        {
            if (sequenceAlignment == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameSequenceAlignment);
            }

            if (writer == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameWriter);
            }

            #region Write alignment header
            SAMAlignmentHeader header = sequenceAlignment.Metadata[Helper.SAMAlignmentHeaderKey] as SAMAlignmentHeader;
            if (header != null)
            {
                WriteHeader(header, writer);
            }

            #endregion

            #region Write aligned sequences
            foreach (IAlignedSequence alignedSequence in sequenceAlignment.AlignedSequences)
            {
                WriteSAMAlignedSequence(alignedSequence, writer);
            }
            #endregion

            writer.Flush();
        }

        /// <summary>
        /// Writes an ISequenceAlignment to the specified file.
        /// </summary>
        /// <param name="sequenceAlignment">The sequence alignment to format.</param>
        /// <param name="filename">The name of the file to write the formatted sequence alignment text.</param>
        public void Format(ISequenceAlignment sequenceAlignment, string filename)
        {
            if (sequenceAlignment == null)
            {
                throw new ArgumentNullException("sequenceAlignment");
            }

            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            using (TextWriter writer = new StreamWriter(filename))
            {
                Format(sequenceAlignment, writer);
            }
        }

        /// <summary>
        /// Writes an SequenceAlignmentMap to the specified file.
        /// </summary>
        /// <param name="sequenceAlignmentMap">SequenceAlignmentMap object to format.</param>
        /// <param name="filename">The name of the file to write the formatted sequence alignment text.</param>
        public void Format(SequenceAlignmentMap sequenceAlignmentMap, string filename)
        {
            if (sequenceAlignmentMap == null)
            {
                throw new ArgumentNullException("sequenceAlignmentMap");
            }

            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            Format(sequenceAlignmentMap as ISequenceAlignment, filename);
        }

        /// <summary>
        /// Writes an sequenceAlignmentMap to the location specified by the writer.
        /// </summary>
        /// <param name="sequenceAlignmentMap">SequenceAlignmentMap object to format.</param>
        /// <param name="writer">The TextWriter used to write the formatted sequence alignment text.</param>
        public void Format(SequenceAlignmentMap sequenceAlignmentMap, TextWriter writer)
        {
            if (sequenceAlignmentMap == null)
            {
                throw new ArgumentNullException("sequenceAlignmentMap");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            Format(sequenceAlignmentMap as ISequenceAlignment, writer);
        }

        /// <summary>
        /// Write a collection of ISequenceAlignments to a writer.
        /// Note that SAM format supports only one ISequenceAlignment object per file.
        /// Thus first ISequenceAlignment in the collection will be written to the file.
        /// </summary>
        /// <param name="sequenceAlignments">The sequence alignments to write.</param>
        /// <param name="writer">The TextWriter used to write the formatted sequence alignments.</param>
        public void Format(ICollection<ISequenceAlignment> sequenceAlignments, TextWriter writer)
        {
            throw new NotSupportedException(Properties.Resource.SAM_FormatMultipleAlignmentsNotSupported);
        }

        /// <summary>
        /// Write a collection of ISequenceAlignments to a file.
        /// </summary>
        /// <param name="sequenceAlignments">The sequenceAlignments to write.</param>
        /// <param name="filename">The name of the file to write the formatted sequence alignments.</param>
        public void Format(ICollection<ISequenceAlignment> sequenceAlignments, string filename)
        {
            throw new NotSupportedException(Properties.Resource.SAM_FormatMultipleAlignmentsNotSupported);
        }

        /// <summary>
        /// Converts an ISequenceAlignment to a formatted string.
        /// </summary>
        /// <param name="sequenceAlignment">The sequence alignment to format.</param>
        /// <returns>A string of the formatted text.</returns>
        public string FormatString(ISequenceAlignment sequenceAlignment)
        {
            if (sequenceAlignment == null)
            {
                throw new ArgumentNullException("sequenceAlignment");
            }

            using (TextWriter writer = new StringWriter())
            {
                Format(sequenceAlignment, writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Writes SAMAlignedSequence to specified text writer.
        /// </summary>
        /// <param name="alignedSequence">SAM aligned sequence to write</param>
        /// <param name="writer">Text writer.</param>
        public static void WriteSAMAlignedSequence(IAlignedSequence alignedSequence, TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (alignedSequence == null)
            {
                throw new ArgumentNullException("alignedSequence");
            }

            SAMAlignedSequenceHeader alignedHeader = alignedSequence.Metadata[Helper.SAMAlignedSequenceHeaderKey] as SAMAlignedSequenceHeader;
            if (alignedHeader == null)
            {
                throw new ArgumentException(Properties.Resource.SAM_AlignedSequenceHeaderMissing);
            }

            ISequence sequence = alignedSequence.Sequences[0];
            if (sequence.Alphabet != Alphabets.DNA)
            {
                throw new ArgumentException(Properties.Resource.SAMFormatterSupportsDNAOnly);
            }

            
            List<int> dotSymbolIndices = new List<int>(alignedHeader.DotSymbolIndices);
            List<int> equalSymbolIndices = new List<int>(alignedHeader.EqualSymbolIndices);
            string seq = "*";
            if (sequence.Count > 0)
            {
                char[] symbols = new char[sequence.Count];
                for (int i = 0; i < sequence.Count; i++)
                {
                    char symbol = (char)sequence[i];

                    if (dotSymbolIndices.Count > 0)
                    {
                        if (dotSymbolIndices.Contains(i))
                        {
                            symbol = '.';
                            dotSymbolIndices.Remove(i);
                        }
                    }

                    if (equalSymbolIndices.Count > 0)
                    {
                        if (equalSymbolIndices.Contains(i))
                        {
                            symbol = '=';
                            equalSymbolIndices.Remove(i);
                        }
                    }

                    symbols[i] = symbol;
                }

                seq = new string(symbols);
            }
         
            string qualValues = "*";

            QualitativeSequence qualSeq = sequence as QualitativeSequence;
            if (qualSeq != null)
            {
                byte[] bytes = qualSeq.QualityScores.ToArray();
                qualValues = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
            }

          
            writer.Write(AlignedSequenceFormat,
                alignedHeader.QName, (int)alignedHeader.Flag, alignedHeader.RName,
                alignedHeader.Pos, alignedHeader.MapQ, alignedHeader.CIGAR,
                alignedHeader.MRNM.Equals(alignedHeader.RName) ? "=" : alignedHeader.MRNM,
                alignedHeader.MPos, alignedHeader.ISize, seq, qualValues);

            for (int j = 0; j < alignedHeader.OptionalFields.Count; j++)
            {
                writer.Write(OptionalFieldFormat, alignedHeader.OptionalFields[j].Tag,
                    alignedHeader.OptionalFields[j].VType, alignedHeader.OptionalFields[j].Value);
            }

            writer.WriteLine();
        }
        #endregion
    }
}
