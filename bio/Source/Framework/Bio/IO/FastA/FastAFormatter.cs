using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Bio.IO.FastA
{
    /// <summary>
    /// Writes an ISequence the file specified while creating an instance of this class.
    /// The output is formatter according to the FastA format.
    /// </summary>
    public sealed class FastAFormatter : ISequenceFormatter
    {
        #region Member variables
        /// <summary>
        /// Default Maximum symbols allowed per line. 
        /// As per FastA format, recommended value is 80.
        /// </summary>
        private const int DefaultMaxSymbolsAllowedPerLine = 80;

        /// <summary>
        /// Holds stream writer used for writing to file.
        /// </summary>
        private StreamWriter streamWriter = null;

        /// <summary>
        /// Buffer used while writing to file.
        /// </summary>
        private byte[] buffer = null;
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the FastAFormatter class.
        /// </summary>
        public FastAFormatter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FastAFormatter class with specified filename.
        /// </summary>
        /// <param name="filename">FastA filename.</param>
        public FastAFormatter(string filename)
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
            get
            {
                return Properties.Resource.FastAName;
            }
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
                return Properties.Resource.FASTAFORMATTER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets the file extension supported by this formatter.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Properties.Resource.FASTA_FILEEXTENSION;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the FastAFormatter will flush its buffer 
        /// to the underlying stream after every call to Write(ISequence).
        /// </summary>
        public bool AutoFlush { get; set; }

        /// <summary>
        /// Gets or sets the maximum symbols allowed per line.
        /// Default value is 80.
        /// Note that the FastA format recommends that all lines 
        /// should be less than 80 symbol in length.
        /// </summary>
        public int MaxSymbolsAllowedPerLine { get; set; }
        #endregion

        #region Method
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
            this.MaxSymbolsAllowedPerLine = DefaultMaxSymbolsAllowedPerLine;
        }

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
            this.MaxSymbolsAllowedPerLine = DefaultMaxSymbolsAllowedPerLine;
        }

        /// <summary>
        /// Writes the Multiple sequence in FastA format to the file.
        /// Note that if the sequence contains more than the MaxSymbolsAllowedPerLine 
        /// value then it will split the symbols in the specified sequence in to multiple lines, 
        /// where each line will contain maximum of MaxSymbolsAllowedPerLine symbols.
        /// </summary>
        /// <param name="sequences">Sequences to write.</param>
        [Obsolete("Use the IEnumerable overload instead")]
        public void Write(ICollection<ISequence> sequences)
        {
            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            foreach (ISequence sequence in sequences)
            {
                Write(sequence);
            }

            this.streamWriter.Flush();
        }
        /// <summary>
        /// Writes the Multiple sequence in FastA format to the file.
        /// Note that if the sequence contains more than the MaxSymbolsAllowedPerLine 
        /// value then it will split the symbols in the specified sequence in to multiple lines, 
        /// where each line will contain maximum of MaxSymbolsAllowedPerLine symbols.
        /// </summary>
        /// <param name="sequences">Sequences to write.</param>
        public void Write(IEnumerable<ISequence> sequences)
        {
            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            foreach (ISequence sequence in sequences)
            {
                Write(sequence);
            }

            this.streamWriter.Flush();
        }
        /// <summary>
        /// Writes the specified sequence in FastA format to the file.
        /// Note that if the sequence contains more than the MaxSymbolsAllowedPerLine 
        /// value then it will split the symbols in the specified sequence in to multiple lines, 
        /// where each line will contain maximum of MaxSymbolsAllowedPerLine symbols.
        /// </summary>
        /// <param name="sequence">Sequence to write.</param>
        public void Write(ISequence sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            if (this.streamWriter == null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotOpened);
            }

            int maxLineSize = this.MaxSymbolsAllowedPerLine;

            int bufferIndex = 0;
            if (this.buffer == null)
            {
                this.buffer = new byte[maxLineSize];
            }

            // Buffer resize is required as MaxSymbolsAllowedPerLine can be modified 
            if (this.buffer.Length < maxLineSize)
            {
                Array.Resize(ref this.buffer, maxLineSize);
            }

            this.streamWriter.WriteLine(">" + sequence.ID);

            for (long index = 0; index < sequence.Count; index += maxLineSize)
            {
                for (bufferIndex = 0; bufferIndex < maxLineSize && index + bufferIndex < sequence.Count; bufferIndex++)
                {
                    this.buffer[bufferIndex] = sequence[index + bufferIndex];
                }

                string line = UTF8Encoding.UTF8.GetString(this.buffer, 0, bufferIndex);
                this.streamWriter.WriteLine(line);
            }

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

        /// <summary>
        /// Format the sequence to a FastA string.
        /// </summary>
        /// <param name="sequence">The sequence to be formatted.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatString(ISequence sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(">" + sequence.ID);
            foreach (byte item in sequence)
            {
                stringBuilder.Append((char)item);
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}
