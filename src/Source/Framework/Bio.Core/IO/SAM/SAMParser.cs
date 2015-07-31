using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bio.Algorithms.Alignment;
using Bio.Extensions;
using Bio.Util;
using Bio.Util.Logging;

namespace Bio.IO.SAM
{
    /// <summary>
    /// A SAMParser reads from a source of text that is formatted according to the SAM
    /// file specification (v1.4-r985), and converts the data to in-memory SequenceAlignmentMap object.
    /// For advanced users, the ability to select an encoding for the internal memory representation is
    /// provided. There is also a default encoding for each alphabet that may be encountered.
    /// Documentation for the latest SAM file format can be found at
    /// http://samtools.sourceforge.net/SAM1.pdf
    /// </summary>
    public class SAMParser : ISequenceAlignmentParser
    {
        /// <summary>
        /// Constant to hold SAM alignment header line pattern.
        /// </summary>
        public const string HeaderLinePattern = "(@..){1}((\t..:[^\t]+)+)";

        /// <summary>
        /// An asterisk encoded as a byte
        /// </summary>
        public const byte AsteriskAsByte=42;//* in ascii

        /// <summary>
        /// Constant to hold SAM optional filed line pattern.
        /// </summary>
        public const string OptionalFieldLinePattern = "..:.:([^\t\n\r]+)";

        /// <summary>
        /// Holds the qualitative value type.
        /// </summary>
        public const FastQFormatType QualityFormatType = FastQFormatType.Sanger;

        /// <summary>
        /// Holds optional field regular expression object.
        /// </summary>
        private static readonly Regex OptionalFieldRegex = new Regex(OptionalFieldLinePattern);

        /// <summary>
        /// Constant for tab and space delimiter.
        /// </summary>
        private static readonly char[] TabDelim = { '\t' };

        /// <summary>
        ///  Constant for colon delimiter.
        /// </summary>
        private static readonly char[] ColonDelim = {':'};

        /// <summary>
        /// The default constructor which chooses the default encoding based on the alphabet.
        /// </summary>
        public SAMParser()
        {
            RefSequences = new List<ISequence>();
        }

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
        /// Always returns singleton instance of SAMDNAAlpabet.
        /// </summary>
        public IAlphabet Alphabet
        {
            get
            {
                return SAMDnaAlphabet.Instance;
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

        /// <summary>
        /// Parses SAM alignment header from specified stream.
        /// </summary>
        /// <param name="stream">stream.</param>
        public static SAMAlignmentHeader ParseSAMHeader(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (StreamReader reader = stream.OpenRead())
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

            var headerStrings = new List<string>();
            SAMAlignmentHeader samHeader = null;
            string line = ReadNextLine(reader);
            if (line != null && line.StartsWith(@"@", StringComparison.OrdinalIgnoreCase))
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
            return ParseSequence(bioText, SAMDnaAlphabet.Instance);
        }

        /// <summary>
        /// Parses sequence data and quality values and updates SAMAlignedSequence instance.
        /// </summary>
        /// <param name="alignedSeq">SAM aligned Sequence.</param>
        /// <param name="alphabet">Alphabet of the sequence to be created.</param>
        /// <param name="sequencedata">Sequence data.</param>
        /// <param name="qualitydata">Quality values.</param>
        public static void ParseQualityNSequence(SAMAlignedSequence alignedSeq, IAlphabet alphabet, string sequencedata, string qualitydata)
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
                qualScores = Encoding.UTF8.GetBytes(qualitydata);

                // Check for sequence length and quality score length.
                if (sequencedata.Length != qualitydata.Length)
                {
                    string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.FastQ_InvalidQualityScoresLength, alignedSeq.QName);
                    message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, Properties.Resource.SAM_NAME, message1);
                    throw new Exception(message);
                }
            }

            alignedSeq.QuerySequence = isQualitativeSequence
                                     ? (ISequence) new QualitativeSequence(alphabet, fastQType, sequencedata,
                                           Encoding.UTF8.GetString(qualScores, 0, qualScores.Length)) { ID = alignedSeq.QName }
                                     : new Sequence(alphabet, sequencedata) { ID = alignedSeq.QName };
        }

        /// <summary>
        /// Parses sequence data and quality values and updates SAMAlignedSequence instance.
        /// </summary>
        /// <param name="alignedSeq">SAM aligned Sequence.</param>
        /// <param name="alphabet">Alphabet of the sequence to be created.</param>
        /// <param name="sequencedata">Sequence data.</param>
        /// <param name="qualitydata">Quality values.</param>
        /// <param name="validate">Validation needed</param>
        public static void ParseQualityNSequence(SAMAlignedSequence alignedSeq, IAlphabet alphabet, byte[] sequencedata, byte[] qualitydata,bool validate=true)
        {
            if (alignedSeq == null)
            {
                throw new ArgumentNullException("alignedSeq");
            }

            if (sequencedata==null || sequencedata.Length==0)
            {
                throw new ArgumentNullException("sequencedata");
            }

            if (qualitydata==null || qualitydata.Length==0)
            {
                throw new ArgumentNullException("qualitydata");
            }

            bool isQualitativeSequence = true;
            string message = string.Empty;
            FastQFormatType fastQType = QualityFormatType;
            if(sequencedata.Length==1 && sequencedata[0]==AsteriskAsByte)
            {
                return;
            }

            if (qualitydata.Length==1 && qualitydata[0]==AsteriskAsByte)
            {
                isQualitativeSequence = false;
            }

            if (isQualitativeSequence)
            {

                // Check for sequence length and quality score length.
                if (sequencedata.Length != qualitydata.Length)
                {
                    string message1 = string.Format(CultureInfo.CurrentCulture, Properties.Resource.FastQ_InvalidQualityScoresLength, alignedSeq.QName);
                    message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.IOFormatErrorMessage, Properties.Resource.SAM_NAME, message1);
                    Trace.Report(message);
                    throw new Exception(message);
                }
            }

            alignedSeq.QuerySequence = isQualitativeSequence
                                     ? (ISequence) new QualitativeSequence(alphabet, fastQType, sequencedata, qualitydata, validate) { ID = alignedSeq.QName }
                                     : new Sequence(alphabet, sequencedata, validate) { ID = alignedSeq.QName };
        }

        // validates header.
        private static bool ValidateHeaderLineFormat(string headerline)
        {
            const string HeaderPattern = HeaderLinePattern;
            if (headerline.Length >= 3)
            {
                if (!headerline.StartsWith("@CO", StringComparison.OrdinalIgnoreCase))
                {
                    return Helper.IsValidRegexValue(HeaderPattern, headerline);
                }
            }

            return false;
        }

        private static SAMAlignmentHeader ParseSamHeader(List<string> headerStrings)
        {
            SAMAlignmentHeader samHeader = new SAMAlignmentHeader();

            foreach (string headerString in headerStrings)
            {
                string[] tokens = headerString.Split(TabDelim, StringSplitOptions.RemoveEmptyEntries);
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

            IList<ReferenceSequenceInfo> referenceSeqsInfo = samHeader.GetReferenceSequencesInfoFromSQHeader();
            foreach (var item in referenceSeqsInfo)
            {
                samHeader.ReferenceSequences.Add(item);
            }

            string message = samHeader.IsValid();
            if (!string.IsNullOrEmpty(message))
            {
                throw new FormatException(message);
            }

            return samHeader;
        }

        /// <summary>
        /// Parses a list of sequence alignment texts from a stream.
        /// </summary>
        /// <param name="stream">A stream for a sequence alignment text.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        IEnumerable<ISequenceAlignment> IParser<ISequenceAlignment>.Parse(Stream stream)
        {
            yield return ParseOne(stream);
        }

        /// <summary>
        /// Parses a sequence alignment texts from a stream.
        /// </summary>
        /// <param name="stream">A stream for a sequence alignment text.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        public ISequenceAlignment ParseOne(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            return Parse(stream);
        }

        /// <summary>
        /// Parses a sequence alignment texts from a file.
        /// </summary>
        /// <param name="stream">Text reader.</param>
        /// <returns>SequenceAlignmentMap object.</returns>
        public SequenceAlignmentMap Parse(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var reader = stream.OpenRead())
            {
                return ParseOneWithSpecificFormat(reader);
            }
        }

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

            List<string> refSeqNames = null;
            bool hasSQHeader = header.ReferenceSequences.Count > 0;
            if (!hasSQHeader)
            {
                refSeqNames = new List<string>();
            }

            // Parse aligned sequences 
            // If the SQ header is not present in header then get the reference sequences information from reads.
            while (line != null && !line.StartsWith(@"@", StringComparison.OrdinalIgnoreCase))
            {
                SAMAlignedSequence alignedSeq = ParseSequence(line, this.Alphabet);
                if (!hasSQHeader)
                {
                    if (!alignedSeq.RName.Equals("*", StringComparison.OrdinalIgnoreCase)
                        && !refSeqNames.Contains(alignedSeq.RName, StringComparer.OrdinalIgnoreCase))
                    {
                        refSeqNames.Add(alignedSeq.RName);
                    }
                }

                sequenceAlignmentMap.QuerySequences.Add(alignedSeq);
                line = ReadNextLine(reader);
            }

            if (!hasSQHeader)
            {
                foreach (string refname in refSeqNames)
                {
                    header.ReferenceSequences.Add(new ReferenceSequenceInfo(refname, 0));
                }
            }

            return sequenceAlignmentMap;
        }

        /// <summary>
        /// Parse a single sequencer.
        /// </summary>
        /// <param name="bioText">sequence alignment text.</param>
        /// <param name="alphabet">Alphabet of the sequences.</param>
        private static SAMAlignedSequence ParseSequence(string bioText, IAlphabet alphabet)
        {
            const int optionalTokenStartingIndex = 11;
            string[] tokens = bioText.Split(TabDelim, StringSplitOptions.RemoveEmptyEntries);

            SAMAlignedSequence alignedSeq = new SAMAlignedSequence
            {
                QName = tokens[0],
                Flag = SAMAlignedSequenceHeader.GetFlag(tokens[1]),
                RName = tokens[2],
                Pos = int.Parse(tokens[3]),
                MapQ = int.Parse(tokens[4]),
                CIGAR = tokens[5]
            };

            alignedSeq.MRNM = tokens[6].Equals("=") ? alignedSeq.RName : tokens[6];
            alignedSeq.MPos = int.Parse(tokens[7]);
            alignedSeq.ISize = int.Parse(tokens[8]);

            ParseQualityNSequence(alignedSeq, alphabet, tokens[9], tokens[10]);

            for (int i = optionalTokenStartingIndex; i < tokens.Length; i++)
            {
                SAMOptionalField optField = new SAMOptionalField();
                if (!Helper.IsValidRegexValue(OptionalFieldRegex, tokens[i]))
                {
                    throw new FormatException(string.Format(Properties.Resource.InvalidOptionalField, tokens[i]));
                }

                string[] opttokens = tokens[i].Split(ColonDelim, StringSplitOptions.RemoveEmptyEntries);
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
    }
}
