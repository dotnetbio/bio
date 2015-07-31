using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Bio.Extensions;

namespace Bio.IO.FastQ
{
    /// <summary>
    /// Writes a QualitativeSequence to a file. The output is formatted
    /// according to the FastQ file format.
    /// </summary>
    public sealed class FastQFormatter : ISequenceFormatter
    {
        /// <summary>
        /// Initializes a new instance of the FastQFormatter class.
        /// </summary>
        public FastQFormatter()
        {
            this.FormatType = FastQFormatType.Illumina_v1_8;
            this.AutoFlush = true;
        }

        /// <summary>
        /// Gets the name of this formatter.
        /// This is intended to give developers name of the formatter.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.FastQName; }
        }

        /// <summary>
        /// Gets the description of this formatter.
        /// This is intended to give developers some information 
        /// of the formatter class. This property returns a simple description of what this
        ///  class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.FASTQFORMATTER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets the file extension supported by this formatter.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Properties.Resource.FASTQ_FILEEXTENSION;
            }
        }

        /// <summary>
        /// Gets or sets the format type to be used.
        /// The FastQFormatType to be used for formatting QualitativeSequence objects.
        /// Default value is Illumina_v1_8
        /// </summary>
        public FastQFormatType FormatType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the FastQFormatter will flush its buffer 
        /// to the underlying stream after every call to Write method.
        /// </summary>
        public bool AutoFlush { get; set; }

        /// <summary>
        /// Writes the specified QualitativeSequence in FastQ format to the file.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="data">QualitativeSequence to write.</param>
        void IFormatter<ISequence>.Format(Stream stream, ISequence data)
        {
            var qualitativeSequence = data as QualitativeSequence;
            if (qualitativeSequence == null)
            {
                throw new ArgumentNullException("data", Properties.Resource.FastQ_NotAQualitativeSequence);
            }

            this.Format(stream, qualitativeSequence);
        }

        /// <summary>
        /// Writes a set of entries.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="sequences">The data to write.</param>
        public void Format(Stream stream, IEnumerable<ISequence> sequences)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            using (var writer = stream.OpenWrite())
            {
                foreach (var sequence in sequences.OfType<QualitativeSequence>())
                {
                    this.Format(writer, sequence);
                }
            }
        }

        /// <summary>
        /// Writes the specified QualitativeSequence in FastQ format to the file.
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        /// <param name="qualitativeSequence">QualitativeSequence to write.</param>
        public void Format(Stream stream, IQualitativeSequence qualitativeSequence)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (qualitativeSequence == null)
            {
                throw new ArgumentNullException("qualitativeSequence");
            }

            using(var writer = stream.OpenWrite())
            {
                this.Format(writer, qualitativeSequence);
            }
        }

        /// <summary>
        /// Write out a sequence to the given stream writer.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="qualitativeSequence"></param>
        private void Format(StreamWriter writer, IQualitativeSequence qualitativeSequence)
        {
            string header = qualitativeSequence.ID;
            const string LengthStr = " length=";

            if (qualitativeSequence.ID.Contains(LengthStr))
            {
                int startIndex = qualitativeSequence.ID.LastIndexOf(LengthStr, StringComparison.OrdinalIgnoreCase);
                header = header.Substring(0, startIndex + 8) + qualitativeSequence.Count;
            }

            // Write to stream.
            writer.WriteLine("@" + header);
            byte[] sequenceBytes = qualitativeSequence.ToArray();
            writer.WriteLine(Encoding.UTF8.GetString(sequenceBytes, 0, sequenceBytes.Length));
            writer.WriteLine("+" + header);
            byte[] qualityValues = QualitativeSequence.ConvertEncodedQualityScore(qualitativeSequence.FormatType, this.FormatType, qualitativeSequence.GetEncodedQualityScores());
            writer.WriteLine(Encoding.UTF8.GetString(qualityValues, 0, qualityValues.Length));

            if (this.AutoFlush)
            {
                writer.Flush();
            }
        }
    }
}
