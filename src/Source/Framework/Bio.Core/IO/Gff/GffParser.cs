using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Bio.Extensions;
using Bio.Properties;
using Bio.Util.Logging;

namespace Bio.IO.Gff
{
    /// <summary>
    ///     A GffParser reads from a source of text that is formatted according to the GFF flat
    ///     file specification, and converts the data to in-memory ISequence objects.  For advanced
    ///     users, the ability to select an encoding for the internal memory representation is
    ///     provided. There is also a default encoding for each alphabet that may be encountered.
    ///     Documentation for the latest GFF file format can be found at following location under
    ///     Creative Commons License that is,
    ///     Online content created and hosted by the Wellcome Trust Sanger Institute is,
    ///     unless otherwise stated, licensed under a Creative Commons Attribution-NonCommercial-NoDerivs 2.5 License.
    ///     http://www.sanger.ac.uk/Software/formats/GFF/GFF_Spec.shtml
    /// </summary>
    public class GffParser : ISequenceParser
    {
        #region Constants
        const string HeaderMark = "##";
        const string CommentMark = "#";
        const string CommentSectionKey = "COMMENTSECTION_";
        const string GffVersionKey = "GFF-VERSION";
        const string GffSpecVersionKey = "GFF-SPEC-VERSION";
        const string SourceVersionKey = "SOURCE-VERSION";
        const string SourceKey = "source";
        const string VersionKey = "version";
        const string DateKey = "DATE";
        const string DateLowerCaseKey = "date";
        const string TypeKey = "TYPE";
        const string MultiTypeKey = "TYPE_";
        const string MultiSeqDataKey = "SEQDATA_";
        const string SeqDataEnd = "##end-";
        const string SeqRegKey = "SEQUENCE-REGION";
        const string MultiSeqRegKey = "SEQUENCE-REGION_";
        const int MinFieldsPerFeature = 8;
        const int MaxFieldsPerFeature = 9;
        #endregion

        private Sequence commonSeq;
        private List<Tuple<ISequence, List<byte>>> sequences;
        private List<Tuple<ISequence, List<byte>>> sequencesInHeader;

        /// <summary>
        ///     The alphabet to use for parsed ISequence objects.  If this is not set, an alphabet will
        ///     be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        ///     Gets the type of Parser i.e GFF.
        ///     This is intended to give developers some information
        ///     of the parser class.
        /// </summary>
        public string Name
        {
            get
            {
                return Resource.GFF_NAME;
            }
        }

        /// <summary>
        ///     Gets the description of GFF parser.
        ///     This is intended to give developers some information
        ///     of the formatter class. This property returns a simple description of what the
        ///     GffParser class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Resource.GFFPARSER_DESCRIPTION;
            }
        }

        /// <summary>
        ///     Shows the supported types.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Resource.GFF_FILEEXTENSION;
            }
        }

        /// <summary>
        /// Parse a single sequence - not supported due to the file format.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Sequence</returns>
        public ISequence ParseOne(Stream stream)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Parses a list of GFF sequences.
        /// </summary>
        /// <returns>The list of parsed ISequence objects.</returns>
        public IEnumerable<ISequence> Parse(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.sequences = new List<Tuple<ISequence, List<byte>>>();
            this.sequencesInHeader = new List<Tuple<ISequence, List<byte>>>();

            IAlphabet alphabet = this.Alphabet ?? Alphabets.DNA;
            this.commonSeq = new Sequence(alphabet, String.Empty);

            using (var reader = stream.OpenRead())
            {
                // The GFF spec says that all headers need to be at the top of the file.
                string line = this.ParseHeaders(reader);

                // A feature file with no features? May it never be.
                if (reader.EndOfStream)
                {
                    throw new InvalidOperationException(Resource.GFFNoFeatures);
                }

                while (line != null)
                {
                    line = this.ParseFeatures(reader, line);
                }

                this.CopyMetadata();

                return
                    this.sequences.Select(seq => new Sequence(seq.Item1.Alphabet, seq.Item2.ToArray()) {
                            ID = seq.Item1.ID,
                            Metadata = seq.Item1.Metadata
                        })
                        .Cast<ISequence>()
                        .ToList();
            }
        }

        /// <summary>
        ///     Process the headers.
        /// </summary>
        /// <returns></returns>
        private string ParseHeaders(TextReader reader)
        {
            string comments = string.Empty;
            int commentsCount = 1;
            string line = reader.ReadLine();
            while (line == "")
            {
                line = reader.ReadLine();
            }

            while ((line != null) && line.TrimStart().StartsWith(CommentMark, StringComparison.Ordinal))
            {
                // process headers, but ignore other comments
                if (line.StartsWith(HeaderMark, StringComparison.Ordinal))
                {
                    string[] fields = line.Substring(3 - 1).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // Add if any comments.
                    if (!string.IsNullOrEmpty(comments))
                    {
                        this.commonSeq.Metadata[CommentSectionKey + commentsCount.ToString(CultureInfo.InvariantCulture)
                            ] = comments;
                        comments = string.Empty;
                        commentsCount++;
                    }

                    Tuple<ISequence, List<byte>> specificSeq = null;
                    switch (fields[0].ToUpperInvariant())
                    {
                        case GffVersionKey:
                            if (fields.Length > 1 && fields[1] != "2")
                            {
                                string message = String.Format(
                                    CultureInfo.CurrentCulture,
                                    Resource.GffUnsupportedVersion);
                                Trace.Report(message);
                                throw new NotSupportedException(message);
                            }

                            // Store "GFF-VERSION" to get keep the order of comments/headers.
                            this.commonSeq.Metadata[GffVersionKey] = fields[1];

                            break;

                        case SourceVersionKey:

                            var sourceVersion = new MetadataListItem<string>(SourceVersionKey, string.Empty);
                            sourceVersion.SubItems.Add(SourceKey, fields[1]);
                            sourceVersion.SubItems.Add(VersionKey, fields[2]);
                            this.commonSeq.Metadata[SourceVersionKey] = sourceVersion;

                            break;
                        case DateKey:
                            DateTime date;
                            if (!DateTime.TryParse(fields[1], out date))
                            {
                                string message = String.Format(CultureInfo.CurrentCulture, Resource.ParserInvalidDate);
                                Trace.Report(message);
                                throw new FormatException(message);
                            }

                            this.commonSeq.Metadata[DateLowerCaseKey] = date;
                            break;
                        case TypeKey:
                            if (fields.Length == 2)
                            {
                                this.commonSeq.Alphabet = GetAlphabetType(fields[1]);
                                if (this.commonSeq.Alphabet == null)
                                {
                                    string message = String.Format(CultureInfo.CurrentCulture, Resource.InvalidType);
                                    Trace.Report(message);
                                    throw new FormatException(message);
                                }

                                // Store "TYPE" to get keep the order of comments/headers.
                                this.commonSeq.Metadata[TypeKey] = fields[1];
                            }
                            else
                            {
                                specificSeq = this.GetSpecificSequence(fields[2], GetAlphabetType(fields[1]), false);

                                if (specificSeq.Item1.Alphabet == null)
                                {
                                    string message = String.Format(CultureInfo.CurrentCulture, Resource.InvalidType);
                                    Trace.Report(message);
                                    throw new FormatException(message);
                                }

                                // Store "TYPE" to get keep the order of comments/headers.
                                // Store seq id as value.
                                this.commonSeq.Metadata[MultiTypeKey + fields[2]] = fields[2];
                            }
                            break;
                        case "DNA":
                        case "RNA":
                        case "PROTEIN":
                            line = reader.ReadLine();

                            // Store seq id as value.
                            this.commonSeq.Metadata[MultiSeqDataKey + fields[1]] = fields[1];
                            specificSeq = this.GetSpecificSequence(fields[1], GetAlphabetType(fields[0]), false);

                            long sequenceDataLength = 0;
                            while ((line != null) && line != SeqDataEnd + fields[0])
                            {
                                if (!line.StartsWith(HeaderMark, StringComparison.Ordinal))
                                {
                                    string message = String.Format(
                                        CultureInfo.CurrentCulture,
                                        Resource.GffInvalidSequence);
                                    Trace.Report(message);
                                    throw new FormatException(message);
                                }
                                byte[] tempSeqData = Encoding.UTF8.GetBytes(line.Substring(3 - 1).ToCharArray());
                                sequenceDataLength += tempSeqData.Length;

                                specificSeq.Item2.AddRange(tempSeqData);
                                line = reader.ReadLine();
                            }
                            break;
                        case SeqRegKey:

                            specificSeq = this.GetSpecificSequence(fields[1], null, false);
                            specificSeq.Item1.Metadata["start"] = fields[2];
                            specificSeq.Item1.Metadata["end"] = fields[3];

                            // Store seq id as value.
                            this.commonSeq.Metadata[MultiSeqRegKey + fields[1]] = fields[1];
                            break;
                    }
                }
                else
                {
                    comments = string.IsNullOrEmpty(comments) ? line : comments + Environment.NewLine + line;
                }

                line = reader.ReadLine();
                while (line == "")
                {
                    line = reader.ReadLine();
                }
            }

            if (!string.IsNullOrEmpty(comments))
            {
                this.commonSeq.Metadata[CommentSectionKey + commentsCount.ToString(CultureInfo.InvariantCulture)] =
                    comments;
                comments = string.Empty;
            }
            return line;
        }

        /// <summary>
        /// Parses the consecutive feature lines for one sequence.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private string ParseFeatures(TextReader reader, string line)
        {
            // The non-comment lines contain features, which are each stored as MetadataListItems.
            // The fields of each feature are referred to as sub-items.  For GFF, these have
            // unique keys, but for compatibility with our internal representation of features from
            // GenBank format, each sub-item is a list of strings, rather than a simple string.
            List<MetadataListItem<List<string>>> featureList = null;

            Tuple<ISequence, List<byte>> specificSeq = null;
            while (line == "")
            {
                line = reader.ReadLine();
            }
            while (line != null)
            {
                if (line.StartsWith(HeaderMark, StringComparison.Ordinal))
                {
                    line = reader.ReadLine();
                }
                else
                {
                    string[] featureFields = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (featureFields.Length < MinFieldsPerFeature || featureFields.Length > MaxFieldsPerFeature)
                    {
                        string message = string.Format(
                            CultureInfo.CurrentCulture,
                            Resource.INVALID_INPUT_FILE,
                            this.Name);
                        ;
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
                        specificSeq = this.GetSpecificSequence(featureFields[0], null);

                        // Retrieve features list, or add empty features list to metadata if this
                        // is the first feature.
                        if (specificSeq.Item1.Metadata.ContainsKey("features"))
                        {
                            featureList = specificSeq.Item1.Metadata["features"] as List<MetadataListItem<List<string>>>;
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
                    var feature = new MetadataListItem<List<string>>(featureFields[2], attributes);

                    // source
                    feature.SubItems.Add(SourceKey, new List<string> { featureFields[1] });

                    // start is an int
                    int ignoreMe;
                    if (!int.TryParse(featureFields[3], out ignoreMe))
                    {
                        string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Resource.GffInvalidField,
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
                            Resource.GffInvalidField,
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
                                Resource.GffInvalidField,
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
                                Resource.GffInvalidField,
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
                                Resource.GffInvalidField,
                                "frame",
                                featureFields[7]);
                            Trace.Report(message);
                            throw new InvalidDataException(message);
                        }

                        feature.SubItems.Add("frame", new List<string> { featureFields[7] });
                    }

                    // done with that one
                    featureList.Add(feature);
                    line = reader.ReadLine();
                }
            }

            // if any seqs are left in _sequencesInHeader add it to _sequences
            if (this.sequencesInHeader.Count > 0)
            {
                this.sequences.AddRange(this.sequencesInHeader);

                this.sequencesInHeader.Clear();
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

            Tuple<ISequence, List<byte>> seq = null;

            if (!isSeqInFeature)
            {
                // Sequence is referred in header.

                seq = this.sequencesInHeader.FirstOrDefault(S => S.Item1.ID.Equals(sequenceName));
                if (seq != null)
                {
                    return seq;
                }

                seq = new Tuple<ISequence, List<byte>>(
                    new Sequence(alphabetType, string.Empty) { ID = sequenceName },
                    new List<byte>());

                this.sequencesInHeader.Add(seq);
            }
            else
            {
                if (this.sequencesInHeader.Count > 0)
                {
                    seq = this.sequencesInHeader.FirstOrDefault(S => S.Item1.ID.Equals(sequenceName));
                    if (seq != null)
                    {
                        this.sequencesInHeader.Remove(seq);
                        this.sequences.Add(seq);
                    }
                }

                if (this.sequences.Count == 0)
                {
                    seq =
                        new Tuple<ISequence, List<byte>>(
                            new Sequence(alphabetType, string.Empty) { ID = sequenceName },
                            new List<byte>());

                    this.sequences.Add(seq);
                }
                else if (seq == null)
                {
                    seq = this.sequences.FirstOrDefault(S => S.Item1.ID.Equals(sequenceName));

                    if (seq == null)
                    {
                        seq =
                            new Tuple<ISequence, List<byte>>(
                                new Sequence(alphabetType, string.Empty) { ID = sequenceName },
                                new List<byte>());
                        this.sequences.Add(seq);
                    }
                }
            }

            return seq;
        }

        /// <summary>
        ///     Copy file-scope metadata to all the sequences in the list.
        /// </summary>
        /// Flag to indicate whether the resulting sequences should be in read-only mode or not.
        /// If this flag is set to true then the resulting sequences's isReadOnly property 
        /// will be set to true, otherwise it will be set to false.
        private void CopyMetadata()
        {
            foreach (var seq in this.sequences)
            {
                foreach (var pair in this.commonSeq.Metadata)
                {
                    seq.Item1.Metadata[pair.Key] = pair.Value;
                }
            }
        }

        /// <summary>
        ///     Maps the string to a particular Alphabet type and returns
        ///     the instance of mapped Alphabet type.
        /// </summary>
        /// <param name="type">The alphabet type.</param>
        /// <returns>Returns the appropriate Alphabet type for the specified string.</returns>
        private static IAlphabet GetAlphabetType(string type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            type = type.ToUpperInvariant();
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
    }
}