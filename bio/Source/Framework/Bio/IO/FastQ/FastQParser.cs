using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Bio.IO.FastQ
{
    /// <summary>
    /// A FastQParser reads from a source of text that is formatted according to the FASTQ 
    /// file specification and converts the data to in-memory QualitativeSequence objects.
    /// </summary>
    public sealed class FastQParser : ISequenceParser
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the FastQParser class.
        /// </summary>
        public FastQParser()
        {
            this.FormatType = FastQFormatType.Illumina_v1_8;
        }

        /// <summary>
        /// Initializes a new instance of the FastQParser class with specified filename.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public FastQParser(string filename)
        {
            this.FormatType = FastQFormatType.Illumina_v1_8;
            this.Open(filename);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the type of parser.
        /// This is intended to give developers name of the parser.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.FastQName;
            }
        }

        /// <summary>
        /// Gets the description of the parser.
        /// This is intended to give developers some information 
        /// of the parser class. This property returns a simple description of what this
        ///  class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.FASTQPARSER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets a comma separated values of the possible FastQ
        /// file extensions.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Properties.Resource.FASTQ_FILEEXTENSION;
            }
        }

        /// <summary>
        /// Gets or sets the alphabet to use for parsed ISequence objects.  If this is not set, the alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        /// Gets or sets the format type to be used.
        /// The FastQFormatType to be used for parsed QualitativeSequence objects.
        /// Default value is Illumina_v1_8
        /// </summary>
        public FastQFormatType FormatType { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Opens the stream for the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public void Open(string filename)
        {
            // if the file is alread open throw invalid 
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
        /// Gets the IEnumerable of ISequence from the file being parsed.
        /// </summary>
        /// <returns>Returns the QualitativeSequences as IEnumerable of ISequence.</returns>
        IEnumerable<ISequence> ISequenceParser.Parse()
        {
            IEnumerable<QualitativeSequence> qualSequences = this.Parse();
            foreach (QualitativeSequence qualSequence in qualSequences)
            {
                yield return qualSequence;
            }
        }

        /// <summary>
        /// Gets the IEnumerable of ISequence from the stream being parsed.
        /// </summary>
        /// <param name="reader">Stream to be parsed.</param>
        /// <returns>Returns the QualitativeSequences as IEnumerable of ISequence.</returns>
        IEnumerable<ISequence> ISequenceParser.Parse(StreamReader reader)
        {
            IEnumerable<QualitativeSequence> qualSequences = this.Parse(reader);
            foreach (QualitativeSequence qualSequence in qualSequences)
            {
                yield return qualSequence;
            }
        }

        /// <summary>
        /// Gets the IEnumerable of QualitativeSequences from the file being parsed.
        /// </summary>
        /// <returns>Returns the QualitativeSequences.</returns>
        public IEnumerable<QualitativeSequence> Parse()
        {
            using (StreamReader streamReader = new StreamReader(this.Filename))
            {
                FastQFormatType formatType = this.FormatType;
                do
                {
                    yield return ParseOne(streamReader, formatType);
                }
                while (!streamReader.EndOfStream);
            }
        }

        /// <summary>
        /// Gets the IEnumerable of QualitativeSequences from the steam being parsed.
        /// </summary>
        /// <param name="reader">Stream to be parsed.</param>
        /// <returns>Returns the QualitativeSequences.</returns>
        public IEnumerable<QualitativeSequence> Parse(StreamReader reader)
        {
            FastQFormatType formatType = this.FormatType;
            do
            {
                yield return ParseOne(reader, formatType);
            }
            while (!reader.EndOfStream);
        }

        /// <summary>
        /// Gets the IEnumerable of QualitativeSequences from the stream being parsed.
        /// </summary>
        /// <param name="streamReader">Stream to be parsed.</param>
        /// <param name="formatType">Fastq format type.</param>
        /// <returns>Returns a QualitativeSequence.</returns>
        private QualitativeSequence ParseOne(StreamReader streamReader, FastQFormatType formatType)
        {
            IAlphabet alphabet = this.Alphabet;

            bool skipBlankLine = true;

            bool tryAutoDetectAlphabet;
            if (alphabet == null)
            {
                tryAutoDetectAlphabet = true;
            }
            else
            {
                tryAutoDetectAlphabet = false;
            }

            if (streamReader.EndOfStream)
            {
                string exMessage = string.Format(
                            CultureInfo.InvariantCulture,
                            Properties.Resource.INVALID_INPUT_FILE,
                            Properties.Resource.FastQName);

                throw new FileFormatException(exMessage);
            }

            string message = string.Empty;

            string line = ReadNextLine(streamReader, skipBlankLine);

            if (line == null || !line.StartsWith("@", StringComparison.Ordinal))
            {
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, this.Name);
                throw new FileFormatException(message);
            }

            // Process header line.
            string id = line.Substring(1).Trim();

            line = ReadNextLine(streamReader, skipBlankLine);

            if (string.IsNullOrEmpty(line))
            {
                string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.FastQ_InvalidSequenceLine, id);
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, this.Name, message1);
                throw new FileFormatException(message);
            }

            // Get sequence from second line.
            byte[] sequenceData = UTF8Encoding.UTF8.GetBytes(line);

            // Goto third line.
            line = ReadNextLine(streamReader, skipBlankLine);

            // Check for '+' symbol in the third line.
            if (line == null || !line.StartsWith("+", StringComparison.Ordinal))
            {
                string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.FastQ_InvalidQualityScoreHeaderLine, id);
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, this.Name, message1);
                throw new FileFormatException(message);
            }

            string qualScoreId = line.Substring(1).Trim();

            if (!string.IsNullOrEmpty(qualScoreId) && !id.Equals(qualScoreId))
            {
                string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.FastQ_InvalidQualityScoreHeaderData, id);
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, this.Name, message1);
                throw new FileFormatException(message);
            }

            // Goto fourth line.
            line = ReadNextLine(streamReader, skipBlankLine);

            if (string.IsNullOrEmpty(line))
            {
                string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.FastQ_EmptyQualityScoreLine, id);
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, this.Name, message1);
                throw new FileFormatException(message);
            }

            // Get the quality scores from the fourth line.
            byte[] qualScores = UTF8Encoding.UTF8.GetBytes(line);

            // Check for sequence length and quality score length.
            if (sequenceData.LongLength() != qualScores.LongLength())
            {
                string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.FastQ_InvalidQualityScoresLength, id);
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, this.Name, message1);
                throw new FileFormatException(message);
            }

            // Auto detect alphabet if alphabet is set to null, else validate with already set alphabet
            if (tryAutoDetectAlphabet)
            {
                alphabet = Alphabets.AutoDetectAlphabet(sequenceData, 0, sequenceData.LongLength(), alphabet);
                if (alphabet == null)
                {
                    throw new FileFormatException(Properties.Resource.CouldNotIdentifyAlphabetType);
                }
            }
            else if (alphabet != null)
            {
                if (!alphabet.ValidateSequence(sequenceData, 0, sequenceData.LongLength()))
                {
                    throw new FileFormatException(Properties.Resource.InvalidAlphabetType);
                }
            }

            QualitativeSequence qualitativeSequence = new QualitativeSequence(alphabet, formatType, sequenceData, qualScores, false);
            qualitativeSequence.ID = id;
            return qualitativeSequence;
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
        /// Gets the next available line from the specified stream reader.
        /// </summary>
        /// <param name="streamReader">Stream reader.</param>
        /// <param name="skipBlankLine">Flag to skip blank lines. If true this method returns 
        /// first non blank line available from the current position, else returns next available line.</param>
        /// <returns></returns>
        private static string ReadNextLine(StreamReader streamReader, bool skipBlankLine)
        {
            string line = streamReader.ReadLine();

            // Continue reading if blank line found.
            while (skipBlankLine && line != null && string.IsNullOrEmpty(line))
            {
                line = streamReader.ReadLine();
            }

            return line;
        }
        #endregion
    }
}
