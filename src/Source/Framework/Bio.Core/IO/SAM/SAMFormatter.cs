using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bio.Algorithms.Alignment;
using Bio.Extensions;
using Bio.Util;

namespace Bio.IO.SAM
{
    /// <summary>
    /// Writes a SequenceAlignmentMap to a particular location, usually a file. 
    /// The output is formatted according to the SAM file format specification 1.4. 
    /// A method is also provided for quickly accessing the content in string 
    /// form for applications that do not need to first write to file.
    /// Documentation for the latest SAM file format can be found at
    /// http://samtools.sourceforge.net/SAM1.pdf
    /// </summary>
    public class SAMFormatter : ISequenceAlignmentFormatter
    {
        /// <summary>
        /// Holds the format string needed for writing aligned sequence.
        /// </summary>
        private const string AlignedSequenceFormat = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}";

        /// <summary>
        /// Holds the format string needed for writing optional fields of aligned sequence.
        /// </summary>
        private const string OptionalFieldFormat = "\t{0}:{1}:{2}";

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

        /// <summary>
        /// Writes an ISequenceAlignment to the location specified by the stream.
        /// </summary>
        /// <param name="stream">The Stream used to write the formatted sequence alignment text.</param>
        /// <param name="sequenceAlignment">The sequence alignment to format.</param>
        public void Format(Stream stream, ISequenceAlignment sequenceAlignment)
        {
            if (sequenceAlignment == null)
            {
                throw new ArgumentNullException(Properties.Resource.ParameterNameSequenceAlignment);
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var writer = stream.OpenWrite())
            {
                SAMAlignmentHeader header = sequenceAlignment.Metadata[Helper.SAMAlignmentHeaderKey] as SAMAlignmentHeader;
                if (header != null)
                {
                    WriteHeader(writer, header);
                }

                foreach (IAlignedSequence alignedSequence in sequenceAlignment.AlignedSequences)
                {
                    WriteSAMAlignedSequence(writer, alignedSequence);
                }
            }
        }

        /// <summary>
        /// Write a collection of ISequenceAlignments to a file.
        /// </summary>
        /// <param name="stream">The name of the file to write the formatted sequence alignments.</param>
        /// <param name="sequenceAlignments">The sequenceAlignments to write.</param>
        public void Format(Stream stream, IEnumerable<ISequenceAlignment> sequenceAlignments)
        {
            throw new NotSupportedException(Properties.Resource.SAMMultipleAlignmentsOutputNotSupported);
        }


        /// <summary>
        /// Writes specified SAMAlignedHeader to specified text writer.
        /// </summary>
        /// <param name="writer">Text Writer</param>
        /// <param name="header">Header to write.</param>
        public static void WriteHeader(TextWriter writer, SAMAlignmentHeader header)
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
            foreach (SAMRecordField record in header.RecordFields)
            {
                headerLine = new StringBuilder();
                headerLine.Append("@");
                headerLine.Append(record.Typecode);
                foreach (SAMRecordFieldTag tag in record.Tags)
                {
                    headerLine.Append("\t");
                    headerLine.Append(tag.Tag);
                    headerLine.Append(":");
                    headerLine.Append(tag.Value);
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

        /// <summary>
        /// Writes SAMAlignedSequence to specified text writer.
        /// </summary>
        /// <param name="writer">Text writer.</param>
        /// <param name="alignedSequence">SAM aligned sequence to write</param>
        public static void WriteSAMAlignedSequence(TextWriter writer, IAlignedSequence alignedSequence)
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
            if (!(sequence.Alphabet is DnaAlphabet))
            {
                throw new ArgumentException(Properties.Resource.SAMFormatterSupportsDNAOnly);
            }

            string seq = "*";
            if (sequence.Count > 0)
            {
                char[] symbols = new char[sequence.Count];
                for (int i = 0; i < sequence.Count; i++)
                {
                   symbols[i]  = (char)sequence[i];
                }

                seq = new string(symbols);
            }
         
            string qualValues = "*";

            QualitativeSequence qualSeq = sequence as QualitativeSequence;
            if (qualSeq != null)
            {
                byte[] bytes = qualSeq.GetEncodedQualityScores();

                // if FormatType is not sanger then convert the quality scores to sanger.
                if (qualSeq.FormatType != FastQFormatType.Sanger)
                {
                    bytes = QualitativeSequence.ConvertEncodedQualityScore(qualSeq.FormatType, FastQFormatType.Sanger, bytes);
                }

                qualValues = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
          
            writer.Write(AlignedSequenceFormat,
                alignedHeader.QName, (int)alignedHeader.Flag, alignedHeader.RName,
                alignedHeader.Pos, alignedHeader.MapQ, alignedHeader.CIGAR,
                alignedHeader.MRNM.Equals(alignedHeader.RName) ? "=" : alignedHeader.MRNM,
                alignedHeader.MPos, alignedHeader.ISize, seq, qualValues);

            foreach (SAMOptionalField t in alignedHeader.OptionalFields)
            {
                writer.Write(OptionalFieldFormat, t.Tag,
                    t.VType, t.Value);
            }

            writer.WriteLine();
        }
    }
}
