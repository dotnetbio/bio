using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Bio.IO.FastQ
{
    /// <summary>
    /// Writes a QualitativeSequence to a file. The output is formatted
    /// according to the FastQ file format.
    /// </summary>
    public sealed class FastQFormatter : ISequenceFormatter
    {
        #region Member variables
        /// <summary>
        /// Holds stream writer used for writing to file.
        /// </summary>
        private StreamWriter streamWriter = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the FastQFormatter class.
        /// </summary>
        public FastQFormatter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FastQFormatter class with specified filename.
        /// </summary>
        /// <param name="filename">FastQ filename.</param>
        public FastQFormatter(string filename)
        {
            this.Open(filename);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

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
        /// Gets or sets a value indicating whether the FastQFormatter will flush its buffer 
        /// to the underlying stream after every call to Write method.
        /// </summary>
        public bool AutoFlush { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Opens the specified stream for writing sequences.
        /// </summary>
        /// <param name="outStream">StreamWriter to use.</param>
        public void Open(StreamWriter outStream)
        {
            if (this.streamWriter != null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotClosed);
            }

            this.Filename = null;
            this.streamWriter = outStream;
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public void Open(string filename)
        {
            if (this.streamWriter != null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotClosed);
            }

            this.Filename = filename;
            this.streamWriter = new StreamWriter(this.Filename);
        }

        /// <summary>
        /// Writes the specified QualitativeSequence in FastQ format to the file.
        /// </summary>
        /// <param name="sequence">QualitativeSequence to write.</param>
        public void Write(ISequence sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            QualitativeSequence qualitativeSequence = sequence as QualitativeSequence;

            if (qualitativeSequence == null)
            {
                throw new ArgumentNullException("sequence", Properties.Resource.FastQ_NotAQualitativeSequence);
            }

            this.Write(qualitativeSequence);
        }

        /// <summary>
        /// Writes the specified QualitativeSequence in FastQ format to the file.
        /// </summary>
        /// <param name="qualitativeSequence">QualitativeSequence to write.</param>
        public void Write(QualitativeSequence qualitativeSequence)
        {
            if (qualitativeSequence == null)
            {
                throw new ArgumentNullException("qualitativeSequence");
            }

            if (this.streamWriter == null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotOpened);
            }

            string header = qualitativeSequence.ID;
            string lengthStr = " length=";

            if (qualitativeSequence.ID.Contains(lengthStr))
            {
                int startIndex = qualitativeSequence.ID.LastIndexOf(lengthStr, StringComparison.OrdinalIgnoreCase);
                header = header.Substring(0, startIndex + 8) + qualitativeSequence.Count;
            }

            // Write to stream.
            this.streamWriter.WriteLine("@" + header);
            byte[] sequenceBytes = qualitativeSequence.ToArray();
            this.streamWriter.WriteLine(UTF8Encoding.UTF8.GetString(sequenceBytes, 0, sequenceBytes.Length));
            this.streamWriter.WriteLine("+" + header);
            byte[] qualityValues = qualitativeSequence.QualityScores.ToArray();
            this.streamWriter.WriteLine(UTF8Encoding.UTF8.GetString(qualityValues, 0, qualityValues.Length));

            if (this.AutoFlush)
            {
                this.streamWriter.Flush();
            }
        }

        /// <summary>
        /// Clears all buffer of underlying stream and any buffered data will be written to the file. 
        /// </summary>
        public void Flush()
        {
            if (this.streamWriter == null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotOpened);
            }

            this.streamWriter.Flush();
        }

        /// <summary>
        /// Closes the current formatter and underlying stream.
        /// </summary>
        public void Close()
        {
            if (this.streamWriter == null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotOpened);
            }

            this.Flush();
            this.streamWriter.Close();
            this.streamWriter.Dispose();
            this.streamWriter = null;
        }

        /// <summary>
        /// Disposes this formatter and underlying stream.
        /// </summary>
        public void Dispose()
        {
            if (this.streamWriter != null)
            {
                this.Close();
            }

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
