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
            this.AutoDetectFastQFormat = true;
        }

        /// <summary>
        /// Initializes a new instance of the FastQParser class with specified filename.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public FastQParser(string filename)
        {
            this.AutoDetectFastQFormat = true;
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
        /// Gets or sets a value indicating whether this parser should detect 
        /// the format type or use the format type from the FormatType property.
        /// <para></para>
        /// In other words,
        /// <para></para>
        /// If this flag is true then FastQParser will ignore the FormatType property 
        /// and try to identify the FastQFormatType for each sequence data it parses.
        /// By default this property is set to true.
        /// <para></para>
        /// If this flag is false then FastQParser will parse the sequence data 
        /// according to the FastQFormatType specified in FormatType property.
        /// </summary>
        public bool AutoDetectFastQFormat { get; set; }

        /// <summary>
        /// Gets or sets the format type to be used.
        /// The FastQFormatType to be used for parsed QualitativeSequence objects.
        /// Set AutoDetectFastQFormat property to false, otherwise the FastQ parser
        /// will ignore this property and try to identify the FastQFormatType for 
        /// each sequence data it parses.
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
                do
                {
                    yield return ParseOne(streamReader);
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
            do
            {
                yield return ParseOne(reader);
            }
            while (!reader.EndOfStream);
        }

        /// <summary>
        /// Gets the IEnumerable of QualitativeSequences from the stream being parsed.
        /// </summary>
        /// <param name="streamReader">Stream to be parsed.</param>
        /// <returns>Returns a QualitativeSequence.</returns>
        private QualitativeSequence ParseOne(StreamReader streamReader)
        {
            IAlphabet alphabet = this.Alphabet;
            bool autoDetectFastQFormat = this.AutoDetectFastQFormat;
            FastQFormatType formatType = this.FormatType;
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

            string line = streamReader.ReadLine();

            // Continue reading if blank line found.
            while (skipBlankLine && line != null && string.IsNullOrEmpty(line))
            {
                line = streamReader.ReadLine();
            }

            if (line == null || !line.StartsWith("@", StringComparison.Ordinal))
            {
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, this.Name);
                throw new FileFormatException(message);
            }

            // Process header line.
            string id = line.Substring(1).Trim();

            line = streamReader.ReadLine();

            // Continue reading if blank line found.
            while (skipBlankLine && line != null && string.IsNullOrEmpty(line))
            {
                line = streamReader.ReadLine();
            }

            if (string.IsNullOrEmpty(line))
            {
                string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.FastQ_InvalidSequenceLine, id);
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, this.Name, message1);
                throw new FileFormatException(message);
            }

            // Get sequence from second line.
            byte[] sequenceData = UTF8Encoding.UTF8.GetBytes(line);

            // Goto third line.
            line = streamReader.ReadLine();

            // Continue reading if blank line found.
            while (skipBlankLine && line != null && string.IsNullOrEmpty(line))
            {
                line = streamReader.ReadLine();
            }

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
            line = streamReader.ReadLine();

            // Continue reading if blank line found.
            while (skipBlankLine && line != null && string.IsNullOrEmpty(line))
            {
                line = streamReader.ReadLine();
            }

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

            // Identify fastq format type if AutoDetectFastQFormat property is set to true.
            if (autoDetectFastQFormat)
            {
                formatType = IdentifyFastQFormatType(qualScores);
            }

            QualitativeSequence qualitativeSequence = new QualitativeSequence(alphabet, formatType, sequenceData, qualScores, false);

            qualitativeSequence.ID = id;

            // Update the propeties so that next parse will use this data.
            this.FormatType = formatType;

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
        /// Identifies Alphabet for the sepecified quality scores.
        /// This method returns,
        ///  Illumina - if the quality scores are in the range of 64 to 104
        ///  Solexa   - if the quality scores are in the range of 59 to 104
        ///  Sanger   - if the quality scores are in the range of 33 to 126.
        /// </summary>
        /// <param name="qualScores">Quality scores.</param>
        /// <returns>Returns appropriate FastQFormatType for the specified quality scores.</returns>
        private static FastQFormatType IdentifyFastQFormatType(byte[] qualScores)
        {
            FastQFormatType formatType = FastQFormatType.Illumina;
            foreach (byte qualScore in qualScores.Distinct())
            {
                if (qualScore >= QualitativeSequence.SangerMinQualScore && qualScore <= QualitativeSequence.SangerMaxQualScore)
                {
                    if (formatType == FastQFormatType.Illumina)
                    {
                        if (qualScore >= QualitativeSequence.IlluminaMinQualScore && qualScore <= QualitativeSequence.IlluminaMaxQualScore)
                        {
                            continue;
                        }

                        if (qualScore >= QualitativeSequence.SolexaMinQualScore && qualScore <= QualitativeSequence.SolexaMaxQualScore)
                        {
                            formatType = FastQFormatType.Solexa;
                            continue;
                        }

                        if (qualScore >= QualitativeSequence.SangerMinQualScore && qualScore <= QualitativeSequence.SangerMaxQualScore)
                        {
                            formatType = FastQFormatType.Sanger;
                            continue;
                        }
                    }

                    if (formatType == FastQFormatType.Solexa)
                    {
                        if (qualScore >= QualitativeSequence.SolexaMinQualScore && qualScore <= QualitativeSequence.SolexaMaxQualScore)
                        {
                            continue;
                        }

                        if (qualScore >= QualitativeSequence.SangerMinQualScore && qualScore <= QualitativeSequence.SangerMaxQualScore)
                        {
                            formatType = FastQFormatType.Sanger;
                            continue;
                        }
                    }
                }
                else
                {
                    string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidQualityScore, qualScore);
                    string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, Properties.Resource.FastQName, message1);
                    throw new FileFormatException(message);
                }
            }

            return formatType;
        }
        #endregion
    }
}
