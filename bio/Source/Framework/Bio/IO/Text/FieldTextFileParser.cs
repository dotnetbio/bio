using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bio.Properties;
using Bio.Util.Logging;
using System;
using System.Text;
using System.Linq;

namespace Bio.IO.Text
{
    /// <summary>
    /// Field parser reads from a source of text (default tab-delimited) 
    /// and converts the data to in-memory ISequence objects.
    /// Example, tab-delimited sequence file contains two columns:
    /// First column contain sequence id and second column contains the sequence.
    /// </summary>
    public sealed class FieldTextFileParser : ISequenceParser
    {

        #region Constructors and Properties

        /// <summary>
        /// Initializes a new instance of the FastAParser class by 
        /// loading the specified filename.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public FieldTextFileParser(string filename)
        {
            this.Open(filename);
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Initialize instance of for Tab (default) parser class.
        /// </summary>
        public FieldTextFileParser()
        {
            //_commonSequenceParser = new CommonSequenceParser();
            ContainsHeader = true;
            Delimiter = '\t';
        }

        /// <summary>
        /// Gets or sets value whether file contains header.
        /// By default first line is considered as header line.
        /// </summary>
        public bool ContainsHeader { get; set; }

        /// <summary>
        /// Gets or sets value of delimiter. The delimiter defines how columns in file are separated.
        /// </summary>
        public char Delimiter { get; set; }

        #endregion

        #region ISequenceParser Members

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
        /// Closes streams used.
        /// </summary>
        public void Close()
        {
            this.Filename = null;
        }

        /// <summary>
        /// Disposes the underlying stream.
        /// </summary>
        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Parses a list of biological sequence texts.
        /// </summary>
        /// <returns>The parsed ISequence objects.</returns>
        public IEnumerable<ISequence> Parse()
        {
            return Parse(this.Filename);
        }

        /// <summary>
        /// Parses a list of biological sequence texts from a reader.
        /// </summary>
        /// <param name="reader">A reader for a biological sequence text.</param>
        /// <returns>The list of parsed ISequence objects.</returns>
        public IEnumerable<ISequence> Parse(StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            string fileLine = reader.ReadLine();
            IList<ISequence> sequences = new List<ISequence>();

            if (ContainsHeader)
            {
                fileLine = reader.ReadLine();
            }

            while (!string.IsNullOrEmpty(fileLine))
            {
                sequences.Add(ParseLine(fileLine));
                fileLine = reader.ReadLine();
            }

            return sequences;
        }

        /// <summary>
        /// Parses a list of biological sequence texts from a file.
        /// </summary>
        /// <param name="filename">The name of a biological sequence file.</param>
        /// <returns>The list of parsed ISequence objects.</returns>
        private IEnumerable<ISequence> Parse(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Gets the type of Parser i.e field parser.
        /// This is intended to give developers some information 
        /// about parser class.
        /// </summary>
        public string Name
        {
            get
            {
                return Resource.FIELDPARSER_NAME;
            }
        }

        /// <summary>
        /// Gets the description of field parser.
        /// This is intended to give developers some information 
        /// of the parser class. This property returns a simple description of what the
        /// FieldParser class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Resource.FIELDPARSER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets a comma separated values of the possible
        /// file extensions for a TAB file.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Resource.FIELDPARSER_FILEEXTENSION;
            }
        }

        /// <summary>
        /// The alphabet to use for parsed ISequence objects.  If this is not set, the alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet { get; set; }
        #endregion

        #region Private Region

        private ISequence ParseLine(string line)
        {
            string[] splitLine = line.Split(Delimiter);
            string message;
            if (splitLine.Length != 2)
            {
                message = string.Format(CultureInfo.InvariantCulture,
                        Resource.INVALID_INPUT_FILE,
                        line);
                Trace.Report(message);
                throw new FileFormatException(message);
            }

            IAlphabet alphabet = Alphabet;
            if (alphabet == null)
            {
                byte[] byteArray = UTF8Encoding.UTF8.GetBytes(splitLine[1]);
                alphabet = Alphabets.AutoDetectAlphabet(byteArray, 0, byteArray.Length, null);

                if (alphabet == null)
                {
                    message = string.Format(CultureInfo.InvariantCulture,
                            Resource.InvalidSymbolInString,
                            splitLine[1]);
                    Trace.Report(message);
                    throw new FileFormatException(message);
                }
            }

            Sequence sequence;
            sequence = new Sequence(alphabet, splitLine[1]) { ID = splitLine[0]};
            
            return sequence;
        }

        #endregion

    }
}
