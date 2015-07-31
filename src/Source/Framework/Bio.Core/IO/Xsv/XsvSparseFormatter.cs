using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Bio.Extensions;

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
    public class XsvSparseFormatter : ISequenceFormatter
    {
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

        /// <summary>
        /// Creates an XsvSparseFormatter to format ISequences with one 
        /// line per sequence item.
        /// </summary>
        /// <param name="separatorChar">Separator character to be used between sequence item 
        /// position and its symbol.</param>
        /// <param name="sequenceIDPrefixChar">The character to prefix the sequence start 
        /// line with.</param>
        public XsvSparseFormatter(char separatorChar, char sequenceIDPrefixChar) 
        {
            Separator = separatorChar;
            SequenceIDPrefix = sequenceIDPrefixChar;
        }

        /// <summary>
        /// Writes a single data entry.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="data">The data to write.</param>
        public void Format(Stream stream, ISequence data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            // Stream is left open at the end.
            using (StreamWriter writer = stream.OpenWrite())
            {
                this.Write(writer, data, (long)data.Metadata[XsvSparseParser.MetadataOffsetKey]);
            }

        }

        /// <summary>
        /// Writes a set of entries.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="sequences">The data to write.</param>
        public void Format(Stream stream, IEnumerable<ISequence> sequences)
        {
            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            // Stream is closed at the end.
            using (StreamWriter writer = stream.OpenWrite())
            {
                foreach (ISequence sequence in sequences)
                {
                    this.Write(writer, sequence, (long)sequence.Metadata[XsvSparseParser.MetadataOffsetKey]);
                }
            }
        }

        /// <summary>
        /// Writes an ISequence to the location specified by the writer, 
        /// after adding an offset value to the position.
        /// </summary>
        /// <param name="writer">Stream writer</param>
        /// <param name="data">The sequence to format.</param>
        /// <param name="positionOffset">Adds this offset value to the item position within the sequence</param>
        protected void Write(StreamWriter writer, ISequence data, long positionOffset)
        {
            // Check input arguments
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            // write the sequence start line
            writer.WriteLine(
                string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}{5}", SequenceIDPrefix, positionOffset, Separator,
                              (data.Count - (long)data.Metadata[XsvSparseParser.MetadataOffsetKey]), Separator, data.ID).Replace('\n', ' '));

            // for sparse sequences, only write the non-null sequence items
            if (data is SparseSequence)
            {
                foreach (IndexedItem<byte> item in
                    (data as SparseSequence).GetKnownSequenceItems())
                {
                    writer.WriteLine("{0}{1}{2}{3}", (item.Index - (long)data.Metadata[XsvSparseParser.MetadataOffsetKey]), Separator, (char)item.Item, Separator);
                }
            }
            else // for non-sparse sequence, write all sequence items
            {
                for (int i = 0; i < data.Count; i++)
                {
                    writer.WriteLine("{0}{1}{2}{3}", i, Separator, (char)data[i], Separator);
                }
            }
        }
    }
}
