using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Bio.Util.Logging;

namespace Bio.IO.Gff
{
    /// <summary>
    /// A GffParser reads from a source of text that is formatted according to the GFF flat
    /// file specification, and converts the data to in-memory ISequence objects.  For advanced
    /// users, the ability to select an encoding for the internal memory representation is
    /// provided. There is also a default encoding for each alphabet that may be encountered.
    /// 
    /// Documentation for the latest GFF file format can be found at following location under 
    /// Creative Commons License that is,
    /// Online content created and hosted by the Wellcome Trust Sanger Institute is,
    /// unless otherwise stated, licensed under a Creative Commons Attribution-NonCommercial-NoDerivs 2.5 License.
    /// http://www.sanger.ac.uk/Software/formats/GFF/GFF_Spec.shtml
    /// </summary>
    public class GffParser : ISequenceParser
    {
        #region Fields

        private const string HeaderMark = "##";
        private const string CommentMark = "#";
        private const string CommentSectionKey = "COMMENTSECTION_";
        private const string GffVersionKey = "GFF-VERSION";
        private const string GffSpecVersionKey = "GFF-SPEC-VERSION";
        private const string SourceVersionKey ="SOURCE-VERSION";
        private const string SourceKey = "source";
        private const string VersionKey = "version";
        private const string DateKey = "DATE";
        private const string DateLowerCaseKey = "date";
        private const string TypeKey = "TYPE";
        private const string MultiTypeKey = "TYPE_";
        private const string MultiSeqDataKey = "SEQDATA_";
        private const string SeqDataEnd ="##end-";
        private const string SeqRegKey = "SEQUENCE-REGION";
        private const string MultiSeqRegKey = "SEQUENCE-REGION_";

        private const int MinFieldsPerFeature = 8;
        private const int MaxFieldsPerFeature = 9;

        private List<Tuple<ISequence, List<byte>>> sequences;
        private Sequence commonSeq;

        private List<Tuple<ISequence, List<byte>>> sequencesInHeader;
        // Holds streamReader used to read the file.
        private StreamReader streamReader;

       
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor chooses default encoding based on alphabet.
        /// </summary>
        public GffParser()
        {
        }

        /// <summary>
        /// The alphabet to use for parsed ISequence objects.  If this is not set, an alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet { get; set; }

         /// <summary>
        /// Initializes a new instance of the FastAParser class.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public GffParser(string filename)
        {
            this.Filename = filename;
            this.streamReader = new StreamReader(this.Filename);
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the type of Parser i.e GFF.
        /// This is intended to give developers some information 
        /// of the parser class.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.GFF_NAME;
            }
        }

        /// <summary>
        /// Gets the description of GFF parser.
        /// This is intended to give developers some information 
        /// of the formatter class. This property returns a simple description of what the
        /// GffParser class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.GFFPARSER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        #endregion Properties

        #region Public methods
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

            this.Filename = filename;

            if (this.streamReader != null)
            {
                this.streamReader.Close();
            }

            this.streamReader = new StreamReader(this.Filename);
        }

        /// <summary>
        /// Parses a list of GFF sequences.
        /// </summary>
        /// <returns>The list of parsed ISequence objects.</returns>
        public IEnumerable<ISequence> Parse(StreamReader reader)
        {
            this.streamReader = reader;
            this.Filename = null;
            return this.Parse();
        }

        /// <summary>
        /// Parses a list of GFF sequences using a StreamReader.
        /// </summary>
        /// <returns>The list of parsed ISequence objects.</returns>
        public IEnumerable<ISequence> Parse()
        {
            if (string.IsNullOrEmpty(this.Filename) && this.streamReader == null)
            {
                throw new ArgumentNullException(this.Filename);
            }

            this.sequences = new List<Tuple<ISequence, List<byte>>>();
            sequencesInHeader = new List<Tuple<ISequence,List<byte>>>();

            IAlphabet alphabet = Alphabet ?? Alphabets.DNA;

            commonSeq = new Sequence(alphabet, String.Empty);

            // The GFF spec says that all headers need to be at the top of the file.
            string line= ParseHeaders();
            
            // A feature file with no features? May it never be.
            if (this.streamReader.EndOfStream)
            {
                string message = Properties.Resource.GFFNoFeatures;
                Trace.Report(message);
                throw new InvalidOperationException(message);
            }

            while(line != null)
            {
                line = ParseFeatures(line);
            }

            CopyMetadata();

            return this.sequences.Select(curSeq =>
                new Sequence(curSeq.Item1.Alphabet, curSeq.Item2.ToArray())
                {
                    ID = curSeq.Item1.ID,
                    Metadata = curSeq.Item1.Metadata
                }).Cast<ISequence>().ToList();
        }

        /// <summary>
        /// Closes the Writer.
        /// </summary>
        public void Close()
        {
            this.Filename = null;
            this.streamReader.Close();
        }

        /// <summary>
        /// Shows the supported types.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Properties.Resource.GFF_FILEEXTENSION;
            }
        }

        /// <summary>
        /// Disposes the writer.
        /// </summary>
        public void Dispose()
        {
            commonSeq = null;
            this.streamReader.Dispose();
            this.Filename = null;
            GC.SuppressFinalize(this);
        }

        #endregion Public methods

        #region Private Methods

        // Processes headers, which are a type of comment.
        private string ParseHeaders()
        {
            string comments = string.Empty;
            int commentsCount = 1;
            string line = this.streamReader.ReadLine();
            while (line == "")
            {
                line = this.streamReader.ReadLine();
            }

            while ((line != null) && line.TrimStart().StartsWith(CommentMark, StringComparison.Ordinal))
            {
                Tuple<ISequence,List<byte>> specificSeq = null;

                // process headers, but ignore other comments
                if (line.StartsWith(HeaderMark, StringComparison.Ordinal))
                {
                    string[] fields = line.Substring(3 - 1).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Add if any comments.
                    if (!string.IsNullOrEmpty(comments))
                    {
                        commonSeq.Metadata[CommentSectionKey + commentsCount.ToString(CultureInfo.InvariantCulture)] = comments;
                        comments = string.Empty;
                        commentsCount++;
                    }

                    switch (fields[0].ToUpperInvariant())
                    {
                        case GffVersionKey:
                            if (fields.Length > 1 && fields[1] != "2")
                            {
                                string message = String.Format(
                                        CultureInfo.CurrentCulture,
                                        Properties.Resource.GffUnsupportedVersion);
                                Trace.Report(message);
                                throw new NotSupportedException(message);
                            }

                            // Store "GFF-VERSION" to get keep the order of comments/headers.
                            commonSeq.Metadata[GffVersionKey] = fields[1];

                            break;
                        
                        case SourceVersionKey:

                            MetadataListItem<string> sourceVersion = new MetadataListItem<string>(SourceVersionKey, string.Empty);
                            sourceVersion.SubItems.Add(SourceKey, fields[1]);
                            sourceVersion.SubItems.Add(VersionKey, fields[2]);
                            commonSeq.Metadata[SourceVersionKey] = sourceVersion;

                            break;
                        case DateKey:
                            DateTime date;
                            if (!DateTime.TryParse(fields[1], out date))
                            {
                                string message = String.Format(
                                        CultureInfo.CurrentCulture,
                                        Properties.Resource.ParserInvalidDate);
                                Trace.Report(message);
                                throw new FormatException(message);
                            }

                            commonSeq.Metadata[DateLowerCaseKey] = date;
                            break;
                        case TypeKey:
                            if (fields.Length == 2)
                            {
                                commonSeq.Alphabet = GetAlphabetType(fields[1]);
                                if (commonSeq.Alphabet == null)
                                {
                                    string message = String.Format(
                                            CultureInfo.CurrentCulture,
                                            Properties.Resource.InvalidType);
                                    Trace.Report(message);
                                    throw new FormatException(message);
                                }

                                // Store "TYPE" to get keep the order of comments/headers.
                                commonSeq.Metadata[TypeKey] = fields[1];
                            }
                            else
                            {
                                specificSeq = GetSpecificSequence(fields[2], GetAlphabetType(fields[1]), false);

                                if (specificSeq.Item1.Alphabet == null)
                                {
                                    string message = String.Format(
                                            CultureInfo.CurrentCulture,
                                            Properties.Resource.InvalidType);
                                    Trace.Report(message);
                                    throw new FormatException(message);
                                }

                                // Store "TYPE" to get keep the order of comments/headers.
                                // Store seq id as value.
                                commonSeq.Metadata[MultiTypeKey + fields[2]] = fields[2];
                            }
                            break;
                        case "DNA":
                        case "RNA":
                        case "PROTEIN":
                            line = this.streamReader.ReadLine();

                            // Store seq id as value.
                            commonSeq.Metadata[MultiSeqDataKey + fields[1]] = fields[1];
                            specificSeq = GetSpecificSequence(fields[1], GetAlphabetType(fields[0]), false);
                            
                            long sequenceDataLength = 0;
                            while ((line != null) && line != SeqDataEnd + fields[0])
                            {
                                if (!line.StartsWith(HeaderMark, StringComparison.Ordinal))
                                {
                                    string message = String.Format(
                                            CultureInfo.CurrentCulture,
                                            Properties.Resource.GffInvalidSequence);
                                    Trace.Report(message);
                                    throw new FormatException(message);
                                }
                                byte[] tempSeqData = UTF8Encoding.UTF8.GetBytes(line.Substring(3 - 1).ToCharArray());
                                sequenceDataLength += (long)tempSeqData.Length;

                                specificSeq.Item2.AddRange(tempSeqData);
                                line = this.streamReader.ReadLine();
                            }
                            break;
                        case SeqRegKey:

                            specificSeq = GetSpecificSequence(fields[1], null, false);
                            specificSeq.Item1.Metadata["start"] = fields[2];
                            specificSeq.Item1.Metadata["end"] = fields[3];

                            // Store seq id as value.
                            commonSeq.Metadata[MultiSeqRegKey + fields[1]] = fields[1];
                            break;
                    }
                }
                else
                {
                    comments = string.IsNullOrEmpty(comments) ? line : comments + Environment.NewLine + line;
                }

                line = this.streamReader.ReadLine();
                while (line == "")
                {
                    line = this.streamReader.ReadLine();
                } 
            }

            if (!string.IsNullOrEmpty(comments))
            {
                commonSeq.Metadata[CommentSectionKey + commentsCount.ToString(CultureInfo.InvariantCulture)] = comments;
                comments = string.Empty;
            }
            return line;
        }

        // Parses the consecutive feature lines for one sequence.
        private string ParseFeatures(string line)
        {
            // The non-comment lines contain features, which are each stored as MetadataListItems.
            // The fields of each feature are referred to as sub-items.  For GFF, these have
            // unique keys, but for compatibility with our internal representation of features from
            // GenBank format, each sub-item is a list of strings, rather than a simple string.
            List<MetadataListItem<List<string>>> featureList = null;

            Tuple<ISequence, List<byte>> specificSeq = null;
            while (line == "")
            {
                line = this.streamReader.ReadLine();
            }
                while (line != null)
                {

                    if (line.StartsWith(HeaderMark, StringComparison.Ordinal))
                    {
                        line = this.streamReader.ReadLine();
                    }
                    else
                    {
                        string[] featureFields = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (featureFields.Length < MinFieldsPerFeature ||
                            featureFields.Length > MaxFieldsPerFeature)
                        {
                            string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, this.Name); ;
                            throw new InvalidDataException(message);
                        }

                        // The featureFields array should now contain the following fields:
                        //      featureFields[0]: sequence name
                        //      featureFields[1]: source
                        //      featureFields[2]: feature name
                        //      featureFields[3]: start
                        //      featureFields[4]: end
                        //      featureFields[5]: score
                        //      featureFields[6]: strand
                        //      featureFields[7]: frame
                        //      featureFields[8]: attributes (optional)

                        // Process sequence name.
                        if (specificSeq == null)
                        {
                            specificSeq = GetSpecificSequence(featureFields[0], null);

                            // Retrieve features list, or add empty features list to metadata if this
                            // is the first feature.
                            if (specificSeq.Item1.Metadata.ContainsKey("features"))
                            {
                                featureList = specificSeq.Item1.Metadata["features"] as
                                    List<MetadataListItem<List<string>>>;
                            }
                            else
                            {
                                featureList = new List<MetadataListItem<List<string>>>();
                                specificSeq.Item1.Metadata["features"] = featureList;
                            }

                        }
                        else if (specificSeq.Item1.ID != featureFields[0])
                        {
                            // don't go to next line; current line still needs to be processed
                            break;
                        }

                        // use feature name as key; attributes field is stored as free text
                        string attributes = (featureFields.Length == 9 ? featureFields[8] : string.Empty);
                        MetadataListItem<List<string>> feature = new MetadataListItem<List<string>>(featureFields[2], attributes);

                        // source
                        feature.SubItems.Add(SourceKey, new List<string> { featureFields[1] });

                        // start is an int
                        int ignoreMe;
                        if (!int.TryParse(featureFields[3], out ignoreMe))
                        {
                            string message = String.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resource.GffInvalidField,
                                    "start",
                                    featureFields[3]);
                            Trace.Report(message);
                            throw new InvalidDataException(message);
                        }
                        feature.SubItems.Add("start", new List<string> { featureFields[3] });

                        // end is an int
                        if (!int.TryParse(featureFields[4], out ignoreMe))
                        {
                            string message = String.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resource.GffInvalidField,
                                    "end",
                                    featureFields[4]);
                            Trace.Report(message);
                            throw new InvalidDataException(message);
                        }

                        feature.SubItems.Add("end", new List<string> { featureFields[4] });

                        // source is a double, or a dot as a space holder
                        if (featureFields[5] != ".")
                        {
                            double ignoreMeToo;
                            if (!double.TryParse(featureFields[5], out ignoreMeToo))
                            {
                                string message = String.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resource.GffInvalidField,
                                    "score",
                                    featureFields[5]);
                                Trace.Report(message);
                                throw new InvalidDataException(message);
                            }
                            feature.SubItems.Add("score", new List<string> { featureFields[5] });
                        }

                        // strand is + or -, or a dot as a space holder
                        if (featureFields[6] != ".")
                        {
                            if (featureFields[6] != "+" && featureFields[6] != "-")
                            {
                                string message = String.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resource.GffInvalidField,
                                    "strand",
                                    featureFields[6]);
                                Trace.Report(message);
                                throw new InvalidDataException(message);
                            }
                            feature.SubItems.Add("strand", new List<string> { featureFields[6] });
                        }

                        // frame is an int, or a dot as a space holder
                        if (featureFields[7] != ".")
                        {
                            if (!int.TryParse(featureFields[7], out ignoreMe))
                            {
                                string message = String.Format(
                                CultureInfo.CurrentCulture,
                                    Properties.Resource.GffInvalidField,
                                    "frame",
                                    featureFields[7]);
                                Trace.Report(message);
                                throw new InvalidDataException(message);
                            }

                            feature.SubItems.Add("frame", new List<string> { featureFields[7] });
                        }

                        // done with that one
                        featureList.Add(feature);
                        line = this.streamReader.ReadLine();
                    }

                }

                // if any seqs are left in _sequencesInHeader add it to _sequences
                if (sequencesInHeader.Count > 0)
                {
                    sequences.AddRange(sequencesInHeader);

                    sequencesInHeader.Clear();
                }
                return line;
        }

        // Returns a sequence corresponding to the given sequence name, setting its display
        // ID if it has not yet been set.  If parsing for single sequence and already a sequence is exist and it
        // has already been assigned a display ID that doesn't match sequenceName, and exception
        // is thrown.
        private Tuple<ISequence, List<byte>> GetSpecificSequence(string sequenceName, IAlphabet alphabetType, bool isSeqInFeature = true)
        {
            if (alphabetType == null)
            {
                alphabetType = Alphabets.DNA;
            }

            Tuple<ISequence,List<byte>> seq = null;

            if (!isSeqInFeature)
            {
                // Sequence is referred in header.

                seq = sequencesInHeader.FirstOrDefault(S => S.Item1.ID.Equals(sequenceName));
                if (seq != null) return seq;

                seq = new Tuple<ISequence, List<byte>>(
                    new Sequence(alphabetType, string.Empty)
                        {
                            ID = sequenceName
                        },
                    new List<byte>());

                sequencesInHeader.Add(seq);
            }
            else
            {
                if (sequencesInHeader.Count > 0)
                {
                    seq = sequencesInHeader.FirstOrDefault(S => S.Item1.ID.Equals(sequenceName));
                    if (seq != null)
                    {
                        sequencesInHeader.Remove(seq);
                        sequences.Add(seq);
                    }
                }

                if (sequences.Count == 0)
                {
                    seq = new Tuple<ISequence, List<byte>>(
                        new Sequence(alphabetType, string.Empty)
                            {
                                ID = sequenceName
                            },
                        new List<byte>());

                    sequences.Add(seq);
                }
                else if (seq == null)
                {
                    seq = sequences.FirstOrDefault(S => S.Item1.ID.Equals(sequenceName));

                    if (seq == null)
                    {
                        seq = new Tuple<ISequence, List<byte>>(
                           new Sequence(alphabetType, string.Empty)
                               {
                                   ID = sequenceName
                               },
                               new List<byte>());
                        sequences.Add(seq);
                    }
                }
            }

            return seq;
        }

        /// <summary>
        /// Copy file-scope metadata to all the sequences in the list.
        /// </summary>
        /// Flag to indicate whether the resulting sequences should be in read-only mode or not.
        /// If this flag is set to true then the resulting sequences's isReadOnly property 
        /// will be set to true, otherwise it will be set to false.
        private void CopyMetadata()
        {
            foreach (var seq in sequences)
            {
                foreach (KeyValuePair<string, object> pair in commonSeq.Metadata)
                {
                    seq.Item1.Metadata[pair.Key] = pair.Value;
                }
            }
        }

        /// <summary>
        /// Returns Alphabet type depending on the specified alphabet.
        /// </summary>
        /// <param name="alphabet">Alphabet.</param>
        /// <returns>Returns alphabet type.</returns>
        private static IAlphabet GetAlphabetType(IAlphabet alphabet)
        {
            if (alphabet == Alphabets.DNA)
            {
                return Alphabets.DNA;
            }
            else if (alphabet == Alphabets.RNA)
            {
                return Alphabets.RNA;
            }
            else if (alphabet == Alphabets.Protein)
            {
                return Alphabets.Protein;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Maps the string to a particular Alphabet type and returns
        /// the instance of mapped Alphabet type.
        /// </summary>
        /// <param name="type">The alphabet type.</param>
        /// <returns>Returns the appropriate Alphabet type for the specified string.</returns>
        private static IAlphabet GetAlphabetType(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            type = type.ToUpper(CultureInfo.InvariantCulture);
            switch (type)
            {
                case "DNA":
                    return Alphabets.DNA;
                case "RNA":
                    return Alphabets.RNA;
                case "PROTEIN":
                    return Alphabets.Protein;
                default:
                    return null;
            }
        }
        #endregion
    }
}
