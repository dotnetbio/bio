using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;

namespace Bio.IO.Xsv
{ 
    /// <summary>
    /// This class will write a sparse sequence to a character separated value file,
    /// with one line per sequence item. The sequence ID, the sequence count and 
    /// offset (if provided) will be written as a comment to a sequence start line.
    /// Multiple sparse sequences can be written with the sequence start line
    /// acting as delimiters.
    /// E.g. formatting with '#' as sequence prefix and ',' as character separator
    /// #0,100, A sparse sequence of length 100 with 2 items
    /// 12,A
    /// 29,T
    /// #3,10, A sparse sequence of length 10 at offset 3 with 1 item
    /// 2,G
    /// #0,10, A sparse sequence of length 15 with no items
    /// </summary>
    [PartNotDiscoverable]
    public class XsvSparseFormatter : ISequenceFormatter
    {
        #region fields

        private TextWriter writer;

        #endregion fields

        #region Public Properties

        /// <summary>
        /// The character to separate the position and sequence item symbol on each line
        /// </summary>
        public char Separator { get; protected set; }
        
        /// <summary>
        /// this prefix will be printed at the start of the line with 
        /// the offset, count and sequence ID. This is treated as the comment 
        /// character prefix in the underlying XsvTextReader.
        /// </summary>
        public char SequenceIDPrefix { get; set; }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gives the supported file types.
        /// </summary>
        public string SupportedFileTypes
        {
            get { return Properties.Resource.XsvSparseParserFileTypes; }
        }

        /// <summary>
        /// Gets the name of the sequence formatter being
        /// implemented. This is intended to give the
        /// developer some information of the formatter type.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.XsvSparseFormatterName;
            }
        }

        /// <summary>
        /// Gets the description of the sequence formatter being
        /// implemented. This is intended to give the
        /// developer some information of the formatter.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.XsvSparseFormatterDesc; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an XsvSparseFormatter to format ISequences with one 
        /// line per sequence item.
        /// </summary>
        /// <param name="separatorChar">Seprator character to be used between sequence item 
        /// position and its symbol.</param>
        /// <param name="sequenceIDPrefixChar">The character to prefix the sequence start 
        /// line with.</param>
        public XsvSparseFormatter(char separatorChar, char sequenceIDPrefixChar) 
        {
            Separator = separatorChar;
            SequenceIDPrefix = sequenceIDPrefixChar;
        }

        /// <summary>
        /// Initializes a new instance of the FastAParser class.
        /// </summary>
        /// <param name="filename">File to be parsed.</param>
        /// <param name="separatorChar">separator Char.</param>
        /// <param name="sequenceIDPrefixChar">sequence ID Prefix Char.</param>
        public XsvSparseFormatter(string filename, char separatorChar, char sequenceIDPrefixChar)
        {
            this.Filename = filename;
            this.writer = new StreamWriter(filename);
            Separator = separatorChar;
            SequenceIDPrefix = sequenceIDPrefixChar;
        }
       
        #endregion

        #region Public Methods

        /// <summary>
        /// Writes an ISequence to the location specified by the writer.
        /// </summary>
        /// <param name="sequence">The sequence to format.</param>
        public void Write(ISequence sequence) 
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            Format(sequence, (long)sequence.Metadata[XsvSparseParser.MetadataOffsetKey]);
        }

        /// <summary>
        /// Write a collection of ISequences to a writer.
        /// </summary>
        /// <param name="sequences">The sequences to write.</param>
        public void Write(IEnumerable<ISequence> sequences) 
        {
            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            if (this.Filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            foreach(ISequence sequence in sequences) 
            {
                Format(sequence, (long)sequence.Metadata[XsvSparseParser.MetadataOffsetKey]);
            }
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public void Open(string filename)
        {
            if (this.writer != null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotClosed);
            }

            this.Filename = filename;
            this.writer = new StreamWriter(this.Filename);
        }

        /// <summary>
        /// Opens the specified stream for writing sequences.
        /// </summary>
        /// <param name="outStream">StreamWriter to use.</param>
        public void Open(StreamWriter outStream)
        {
            if (this.writer != null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotClosed);
            }

            this.Filename = null;
            this.writer = outStream;
        }

        /// <summary>
        /// Closes the formatter.
        /// </summary>
        public void Close()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Disposes the formatter.
        /// </summary>
        public void Dispose()
        {
            if (writer != null)
            {
                writer.Dispose();
            }

            GC.SuppressFinalize(this);
        }
        #endregion Public Methods

        #region Protected methods
        /// <summary>
        /// Writes an ISequence to the location specified by the writer, 
        /// after adding an offset value to the position.
        /// </summary>
        /// <param name="sequence">The sequence to format.</param>
        /// <param name="positionOffset">Adds this offset value to the item position within the sequence</param>
        protected void Format(ISequence sequence, long positionOffset)
        {
            // Check input arguments
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            // write the sequence start line
            writer.WriteLine(
                string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}", SequenceIDPrefix, positionOffset, Separator,
                              (sequence.Count - (long)sequence.Metadata[XsvSparseParser.MetadataOffsetKey]), Separator, sequence.ID).Replace('\n', ' '));

            // for sparse sequences, only write the non-null sequence items
            if (sequence is SparseSequence)
            {
                foreach (IndexedItem<byte> item in
                    (sequence as SparseSequence).GetKnownSequenceItems())
                {
                    writer.WriteLine("{0}{1}{2}{3}", (item.Index - (long)sequence.Metadata[XsvSparseParser.MetadataOffsetKey]), Separator, (char)item.Item, Separator);
                }
            }
            else // for non-sparse sequence, write all sequence items
            {
                for (int i = 0; i < sequence.Count; i++)
                {
                    writer.WriteLine("{0}{1}{2}{3}", i, Separator, (char)sequence[i], Separator);
                }
            }
        }
        #endregion Protected Methods
    }
}
