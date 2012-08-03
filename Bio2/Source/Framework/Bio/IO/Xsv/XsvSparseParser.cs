using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Bio.IO.Xsv
{
    /// <summary>
    /// Implements common methods for parsing one or more sparse sequences from 
    /// an XsvSparseReader. This reads sequence items from the reader and 
    /// returns a sparse sequence created for the items. Multiple sparse sequences
    /// are separated by a "comment" line that starts with the sequence prefix 
    /// character.
    /// 
    /// This also returns the optional offset position of the sequence, if 
    /// present, to support aligned sequences such as in a Contig.
    /// 
    /// This is an abstract class and extending classes will have to implement
    /// the GetSparseReader() method.
    /// </summary>
    public abstract class XsvSparseParser : ISequenceParser
    {
        #region fields
        private readonly char separator;
        private readonly char sequenceIdPrefix;

        /// <summary>
        /// Key used internally to store offset data in a sparse sequence
        /// </summary>
        public const string MetadataOffsetKey = "offset";
        #endregion fields

        #region Properties
        /// <summary>
        /// The alphabet to use for parsed ISequence objects.
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        /// Gives the supppported file types.
        /// </summary>
        public string SupportedFileTypes
        {
            get { return Properties.Resource.XsvSparseParserFileTypes; }
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the name of the parser. 
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.XsvSparseParserName; }
        }

        /// <summary>
        /// Gets the description of the parser. 
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.XsvSparseParserDesc; }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Creates a Sparse parser with the given encoding and alphabet
        /// </summary>
        /// <param name="filename">File to be parsed.</param>
        /// <param name="alphabet">Alphabet for the sequence items</param>
        /// <param name="separatorChar">The seperator.</param>
        /// <param name="sequenceIdPrefixchar">Sequence ID Prefix.</param>
        protected XsvSparseParser(string filename, IAlphabet alphabet, char separatorChar, char sequenceIdPrefixchar)
        {
            this.Alphabet = alphabet;
            this.Filename = filename;
            this.separator = separatorChar;
            this.sequenceIdPrefix = sequenceIdPrefixchar;
        }

        /// <summary>
        /// Initializes a new instance of the FastAParser class.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public XsvSparseParser(string filename)
        {
            this.Filename = filename;
        }

        #endregion Constructor

        #region public methods

        /// <summary>
        /// Creates a text reader from the file name and calls Parse(TextReader reader).
        /// </summary>
        /// Flag to indicate whether the resulting sequences should be in readonly mode or not.
        /// If this flag is set to true then the resulting sequences's isReadOnly property 
        /// will be set to true, otherwise it will be set to false.
        /// <returns>A list of sparse sequences that were present in the file.</returns>
        public IEnumerable<ISequence> Parse()
        {
            // Check input arguments
            if (this.Filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            using (StreamReader reader = new StreamReader(this.Filename))
            {
                return this.Parse(reader);
            }
        }

        /// <summary>
        /// Creates a text reader from the file name and calls Parse(TextReader reader).
        /// </summary>
        /// <param name="reader">Stream to be parsed.</param>
        /// <returns>A list of sparse sequences that were present in the file.</returns>
        public IEnumerable<ISequence> Parse(StreamReader reader)
        {
            XsvSparseReader sparseReader = new XsvSparseReader(reader, separator, sequenceIdPrefix);
            List<ISequence> sequenceList = new List<ISequence>();
            while (sparseReader.HasLines)
            {
                sequenceList.Add(ParseOne(sparseReader));
            }

            return sequenceList;
        }

        /// <summary>
        /// Opens the stream for the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public void Open(string filename)
        {
            // if the file is already open throw invalid 
            if (!string.IsNullOrEmpty(this.Filename))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.FileAlreadyOpen, this.Filename));
            }

            // Validate the file - by try to open.
            using (new StreamReader(filename))
            {
            }

            this.Filename = filename;
        }

        /// <summary>
        /// Closes the Writer.
        /// </summary>
        public void Close()
        {
            this.Filename = null;
        }

        /// <summary>
        /// Disposes the writer.
        /// </summary>
        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }

        #endregion public methods

        #region Protected methods
        
        /// <summary>
        /// The common ParseOne method called for parsing sequences from Xsv files. 
        /// This assumes that that the first line has been read into the XsvSparseReader 
        /// (i.e. GoToNextLine() has been called). This adds the offset position present in 
        /// the sequence start line to each position value in the sequence item.
        /// e.g. the following returns a sparse sequence with ID 'Test sequence' of length 100 
        /// with A at position 32 (25+7) and G at position 57 (50+7).
        /// # 7, 100, Test sequence
        /// 25,A
        /// 50,G
        /// </summary>
        /// <param name="sparseReader">The Xsv sparse reader that can read the sparse sequences.
        /// Flag to indicate whether the resulting sequence should be in readonly mode or not.
        /// If this flag is set to true then the resulting sequence's isReadOnly property 
        /// will be set to true, otherwise it will be set to false.
        /// </param>
        /// <returns>The first sequence present starting from the 
        /// current position in the reader as a SparseSequence. The sparse sequence has the ID present in the 
        /// sequence start line, and its length equals the count present in that line. 
        /// Null if EOF has been reached. Throws an exception if the current position did 
        /// not have the sequence start line with the sequence prefix ID character.
        /// </returns>
        protected ISequence ParseOne(XsvSparseReader sparseReader)
        {
            // Check input arguments
            if (sparseReader == null)
            {
                throw new ArgumentNullException("sparseReader");
            }

            if (!sparseReader.HasLines) return null;

            if (sparseReader.SkipCommentLines || !sparseReader.HasCommentLine)
                throw new InvalidDataException(Properties.Resource.XsvOffsetNotFound);

            // create a new sparse sequence
            SparseSequence sequence = new SparseSequence(Alphabet);

            // read the sequence ID, count and offset
            sequence.ID = sparseReader.GetSequenceId();
            long offset = sparseReader.GetSequenceOffset();
            sequence.Count = sparseReader.GetSequenceCount() + offset;
            
            sequence.Metadata.Add(MetadataOffsetKey, offset); 
            // go to first sequence item
            sparseReader.GoToNextLine();

            while (sparseReader.HasLines && !sparseReader.HasCommentLine)
            {
                // add offset to position
                long position = long.Parse(sparseReader.Fields[0], CultureInfo.InvariantCulture) + offset;
                char symbol = sparseReader.Fields[1][0];
                if (sequence.Count <= position)
                    sequence.Count = position + 1; 
                sequence[position] = (byte)symbol;
                sparseReader.GoToNextLine();
            }

            return sequence;
        }

        #endregion Protected methods
    }
}
