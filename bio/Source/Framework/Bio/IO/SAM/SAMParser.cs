using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bio.Algorithms.Alignment;
using Bio.Util;
using Bio.Util.Logging;

namespace Bio.IO.SAM
{
    /// <summary>
    /// A SAMParser reads from a source of text that is formatted according to the SAM
    /// file specification, and converts the data to in-memory SequenceAlignmentMap object.
    /// For advanced users, the ability to select an encoding for the internal memory representation is
    /// provided. There is also a default encoding for each alphabet that may be encountered.
    /// Documentation for the latest SAM file format can be found at
    /// http://samtools.sourceforge.net/SAM1.pdf
    /// </summary>
    public class SAMParser : IDisposable, ISequenceAlignmentParser
    {
        #region  Constants
        /// <summary>
        /// Constant to hold SAM alignment header line pattern.
        /// </summary>
        public const string HeaderLinePattern = "(@..){1}((\t..:[^\t]+)+)";

        /// <summary>
        /// Constant to hold SAM optional filed line pattern.
        /// </summary>
        public const string OptionalFieldLinePattern = "..:.:([^\t\n\r]+)";

        /// <summary>
        /// Holds the qualitative value type.
        /// </summary>
        private const FastQFormatType QualityFormatType = FastQFormatType.Sanger;

        #endregion

        #region Private Fields
        /// <summary>
        /// Holds optional field regular expression object.
        /// </summary>
        private static Regex OptionalFieldRegex = new Regex(OptionalFieldLinePattern);

        /// <summary>
        /// Constant for tab and space delimeter.
        /// </summary>
        private static char[] tabDelim = "\t".ToCharArray();

        /// <summary>
        ///  Constant for colon delimeter.
        /// </summary>
        private static char[] colonDelim = ":".ToCharArray();
        #endregion

        #region Constructors
        /// <summary>
        /// The default constructor which chooses the default encoding based on the alphabet.
        /// </summary>
        public SAMParser()
        {
            RefSequences = new List<ISequence>();
        }

        
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the name of the sequence alignment parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser type.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.SAM_NAME; }
        }

        /// <summary>
        /// Gets the description of the sequence alignment parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.SAMPARSER_DESCRIPTION; }
        }

        /// <summary>
        /// The alphabet to use for sequences in parsed SequenceAlignmentMap objects.
        /// Always returns DNA.
        /// </summary>
        public IAlphabet Alphabet
        {
            get
            {
                return Alphabets.DNA;
            }
            set
            {
                throw new NotSupportedException(Properties.Resource.SAMParserAlphabetCantBeSet);
            }
        }


        /// <summary>
        /// Gets the file extensions that the parser implementation
        /// will support.
        /// </summary>
        public string SupportedFileTypes
        {
            get { return Properties.Resource.SAM_FILEEXTENSION; }
        }

        /// <summary>
        /// Reference sequences, used to resolve "=" symbol in the sequence data.
        /// </summary>
        public IList<ISequence> RefSequences { get; private set; }
        #endregion

        #region Public Static Methods

        /// <summary>
        /// Parses SAM alignment header from specified file.
        /// </summary>
        /// <param name="fileName">file name.</param>
        public static SAMAlignmentHeader ParseSAMHeader(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            using (StreamReader reader = new StreamReader(fileName))
            {
                return ParseSAMHeader(reader);
            }
        }

        /// <summary>
        /// Parses SAM alignment header from specified text reader.
        /// </summary>
        /// <param name="reader">Text reader.</param>
        public static SAMAlignmentHeader ParseSAMHeader(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            List<string> headerStrings = new List<string>();
            SAMAlignmentHeader samHeader = null;
            string line = ReadNextLine(reader);
            if (line!= null && line.StartsWith(@"@", StringComparison.OrdinalIgnoreCase))
            {
                while (line != null && line.StartsWith(@"@", StringComparison.OrdinalIgnoreCase))
                {
                    headerStrings.Add(line);
                    line = ReadNextLine(reader);
                }

                samHeader = ParseSamHeader(headerStrings);
            }

            return samHeader;
        }

        /// <summary>
        /// Parse a single sequence.
        /// </summary>
        /// <param name="bioText">A string representing a sequence alignment text.</param>
        public static SAMAlignedSequence ParseSequence(string bioText)
        {
            return ParseSequence(bioText, Alphabets.DNA, null);
        }

        /// <summary>
        /// Parases sequence data and quality values and updates SAMAlignedSequence instance.
        /// </summary>
        /// <param name="alignedSeq">SAM aligned Sequence.</param>
        /// <param name="alphabet">Alphabet of the sequence to be created.</param>
        /// <param name="sequencedata">Sequence data.</param>
        /// <param name="qualitydata">Quality values.</param>
        /// <param name="refSeq">Reference sequence if known.</param>
        public static void ParseQualityNSequence(SAMAlignedSequence alignedSeq, IAlphabet alphabet, string sequencedata, string qualitydata, ISequence refSeq)
        {
            if (alignedSeq == null)
            {
                throw new ArgumentNullException("alignedSeq");
            }

            if (string.IsNullOrWhiteSpace(sequencedata))
            {
                throw new ArgumentNullException("sequencedata");
            }

            if (string.IsNullOrWhiteSpace(qualitydata))
            {
                throw new ArgumentNullException("qualitydata");
            }

            bool isQualitativeSequence = true;
            string message = string.Empty;
            byte[] qualScores = null;
            FastQFormatType fastQType = QualityFormatType;

            if (sequencedata.Equals("*"))
            {
                return;
            }

            if (qualitydata.Equals("*"))
            {
                isQualitativeSequence = false;
            }

            if (isQualitativeSequence)
            {
                // Get the quality scores from the fourth line.
                qualScores = ASCIIEncoding.ASCII.GetBytes(qualitydata);

                // Check for sequence length and quality score length.
                if (sequencedata.Length != qualitydata.Length)
                {
                    string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.FastQ_InvalidQualityScoresLength, alignedSeq.QName);
                    message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, Properties.Resource.SAM_NAME, message1);
                    Trace.Report(message);
                    throw new FileFormatException(message);
                }
            }

            // get "." symbol indexes.
            int index = sequencedata.IndexOf('.', 0);
            while (index > -1)
            {
                alignedSeq.DotSymbolIndexes.Add(index++);
                index = sequencedata.IndexOf('.', index);
            }

            // replace "." with N
            if (alignedSeq.DotSymbolIndexes.Count > 0)
            {
                sequencedata = sequencedata.Replace('.', 'N');
            }

            // get "=" symbol indexes.
            index = sequencedata.IndexOf('=', 0);
            while (index > -1)
            {
                alignedSeq.EqualSymbolIndexes.Add(index++);
                index = sequencedata.IndexOf('=', index);
            }

            // replace "=" with corresponding symbol from refSeq.
            if (alignedSeq.EqualSymbolIndexes.Count > 0)
            {
                if (refSeq == null)
                {
                    throw new ArgumentException(Properties.Resource.RefSequenceNofFound);
                }

                for (int i = 0; i < alignedSeq.EqualSymbolIndexes.Count; i++)
                {
                    index = alignedSeq.EqualSymbolIndexes[i];
                    sequencedata = sequencedata.Remove(index, 1);
                    sequencedata = sequencedata.Insert(index, ((char)refSeq[index]).ToString());
                }
            }

            ISequence sequence = null;
            if (isQualitativeSequence)
            {
                QualitativeSequence qualSeq = new QualitativeSequence(alphabet, fastQType, sequencedata, ASCIIEncoding.ASCII.GetString(qualScores));
                qualSeq.ID = alignedSeq.QName;
                sequence = qualSeq;
            }
            else
            {
                sequence = new Sequence(alphabet, sequencedata);
                sequence.ID = alignedSeq.QName;
            }

            alignedSeq.QuerySequence = sequence;
        }
        #endregion

        #region Private Static Methods
       
        // validates header.
        private static bool ValidateHeaderLineFormat(string headerline)
        {
            if (headerline.Length >= 3)
            {
                if (!headerline.StartsWith("@CO", StringComparison.OrdinalIgnoreCase))
                {
                    string headerPattern = HeaderLinePattern;
                    return Helper.IsValidRegexValue(headerPattern, headerline);
                }
            }

            return false;
        }

        private static SAMAlignmentHeader ParseSamHeader(List<string> headerStrings)
        {
            SAMAlignmentHeader samHeader = new SAMAlignmentHeader();

            foreach (string headerString in headerStrings)
            {
                string[] tokens = headerString.Split(tabDelim, StringSplitOptions.RemoveEmptyEntries);
                string recordTypecode = tokens[0].Substring(1);
                // Validate the header format.
                ValidateHeaderLineFormat(headerString);

                SAMRecordField headerLine = null;
                if (string.Compare(recordTypecode, "CO", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    List<string> tags = new List<string>();
                    headerLine = new SAMRecordField(recordTypecode);
                    for (int i = 1; i < tokens.Length; i++)
                    {
                        string tagToken = tokens[i];
                        string tagName = tagToken.Substring(0, 2);
                        tags.Add(tagName);
                        headerLine.Tags.Add(new SAMRecordFieldTag(tagName, tagToken.Substring(3)));
                    }

                    samHeader.RecordFields.Add(headerLine);
                }
                else
                {
                    samHeader.Comments.Add(headerString.Substring(4));
                }
            }

            string message = samHeader.IsValid();
            if (!string.IsNullOrEmpty(message))
            {
                throw new FormatException(message);
            }

            return samHeader;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Parses a list of sequence alignment texts from a reader.
        /// </summary>
        /// <param name="reader">A reader for a sequence alignment text.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        IList<ISequenceAlignment> ISequenceAlignmentParser.Parse(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            List<ISequenceAlignment> alignments = new List<ISequenceAlignment>();

            alignments.Add(Parse(reader));
            
            return alignments;
        }
        
        /// <summary>
        /// Parses a list of sequence alignment texts from a file.
        /// </summary>
        /// <param name="fileName">The name of a sequence alignment file.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        IList<ISequenceAlignment> ISequenceAlignmentParser.Parse(string fileName)
        {
            ISequenceAlignment alignment = Parse(fileName);
            return new List<ISequenceAlignment>() { alignment };
        }

        /// <summary>
        /// Parses a sequence alignment texts from a reader.
        /// </summary>
        /// <param name="reader">A reader for a sequence alignment text.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        public ISequenceAlignment ParseOne(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return Parse(reader);
        }

        /// <summary>
        /// Parses a sequence alignment texts from a file.
        /// </summary>
        /// <param name="fileName">The name of a sequence alignment file.</param>
        /// <returns>ISequenceAlignment object.</returns>
        public ISequenceAlignment ParseOne(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            return Parse(fileName);
        }

        /// <summary>
        /// Parses a sequence alignment texts from a file.
        /// </summary>
        /// <param name="fileName">file name.</param>
        /// <returns>SequenceAlignmentMap object.</returns>
        public SequenceAlignmentMap Parse(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            FileInfo fileInfo = new FileInfo(fileName);
            using (StreamReader reader = new StreamReader(fileName))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses a sequence alignment texts from a file.
        /// </summary>
        /// <param name="reader">Text reader.</param>
        /// <returns>SequenceAlignmentMap object.</returns>
        public SequenceAlignmentMap Parse(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return ParseOneWithSpecificFormat(reader);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parses alignments in SAM format from a reader into a SequenceAlignmentMap object.
        /// </summary>
        /// <param name="reader">A reader for a biological sequence alignment text.</param>
        /// <returns>A new SequenceAlignmentMap instance containing parsed data.</returns>
        protected SequenceAlignmentMap ParseOneWithSpecificFormat(TextReader reader)
        {

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // Parse the header lines and store them in a string. 
            // This is being done as parsing the header using the textreader is parsing an extra line.
            List<string> headerStrings = new List<string>();
            string line = ReadNextLine(reader);
            while (line != null && line.StartsWith(@"@", StringComparison.OrdinalIgnoreCase))
            {
                headerStrings.Add(line);
                line = ReadNextLine(reader);
            }

            // Parse the alignment header strings.
            SAMAlignmentHeader header = ParseSamHeader(headerStrings);
            SequenceAlignmentMap sequenceAlignmentMap = new SequenceAlignmentMap(header);
            
            // Parse aligned sequences 
            while (line != null && !line.StartsWith(@"@", StringComparison.OrdinalIgnoreCase))
            {
                SAMAlignedSequence alignedSeq = ParseSequence(line, Alphabet, RefSequences);
                sequenceAlignmentMap.QuerySequences.Add(alignedSeq);
                line = ReadNextLine(reader);
            }

            return sequenceAlignmentMap;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Parse a single sequencer.
        /// </summary>
        /// <param name="bioText">sequence alignment text.</param>
        /// <param name="alphabet">Alphabet of the sequences.</param>
        /// <param name="referenceSequences">Reference sequences.</param>
        private static SAMAlignedSequence ParseSequence(string bioText, IAlphabet alphabet, IList<ISequence> referenceSequences)
        {
            const int optionalTokenStartingIndex = 11;
            string[] tokens = bioText.Split(tabDelim, StringSplitOptions.RemoveEmptyEntries);

            SAMAlignedSequence alignedSeq = new SAMAlignedSequence();

            alignedSeq.QName = tokens[0];
            alignedSeq.Flag = SAMAlignedSequenceHeader.GetFlag(tokens[1]);
            alignedSeq.RName = tokens[2];
            alignedSeq.Pos = int.Parse(tokens[3], CultureInfo.InvariantCulture);
            alignedSeq.MapQ = int.Parse(tokens[4], CultureInfo.InvariantCulture);
            alignedSeq.CIGAR = tokens[5];
            alignedSeq.MRNM = tokens[6].Equals("=") ? alignedSeq.RName : tokens[6];
            alignedSeq.MPos = int.Parse(tokens[7], CultureInfo.InvariantCulture);
            alignedSeq.ISize = int.Parse(tokens[8], CultureInfo.InvariantCulture);

            ISequence refSeq = null;

            if (referenceSequences != null && referenceSequences.Count > 0)
            {
                refSeq = referenceSequences.FirstOrDefault(R => string.Compare(R.ID, alignedSeq.RName, StringComparison.OrdinalIgnoreCase) == 0);
            }

            ParseQualityNSequence(alignedSeq, alphabet, tokens[9], tokens[10], refSeq);
            SAMOptionalField optField = null;
            string message;
            for (int i = optionalTokenStartingIndex; i < tokens.Length; i++)
            {
                optField = new SAMOptionalField();
                if (!Helper.IsValidRegexValue(OptionalFieldRegex, tokens[i]))
                {
                    message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidOptionalField, tokens[i]);
                    throw new FormatException(message);
                }

                string[] opttokens = tokens[i].Split(colonDelim, StringSplitOptions.RemoveEmptyEntries);
                optField.Tag = opttokens[0];
                optField.VType = opttokens[1];
                optField.Value = opttokens[2];

                alignedSeq.OptionalFields.Add(optField);
            }

            return alignedSeq;
        }

        /// <summary>
        /// Reads next line considering
        /// </summary>
        /// <returns>The read line.</returns>
        private static string ReadNextLine(TextReader reader)
        {
            if (reader.Peek() == -1)
            {
                return null;
            }

            var line = reader.ReadLine();
            while (string.IsNullOrWhiteSpace(line) && reader.Peek() != -1)
            {
                line = reader.ReadLine();
            }

            return line;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// If the TextReader was opened by this object, dispose it.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">If true disposes resourses held by this instance.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clear unmanaged resources.
            }
        }
        #endregion
    }
}
