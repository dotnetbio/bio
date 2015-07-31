using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bio.Util;
using Bio.Util.Logging;
using System.ComponentModel;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// A GenBankParser reads from a source of text that is formatted according to the GenBank flat
    /// file specification, and converts the data to in-memory ISequence objects.  For advanced
    /// users, the ability to select an encoding for the internal memory representation is
    /// provided. There is also a default encoding for each alphabet that may be encountered.
    /// Documentation for the latest GenBank file format can be found at
    /// ftp.ncbi.nih.gov/genbank/gbrel.txt
    /// </summary>
    public sealed class GenBankParser : ISequenceParser
    {
        #region Fields

        // the standard indent for data is different from the indent for data in the features section
        private const int DataIndent = 12;
        private const int FeatureDataIndent = 21;
        private Sequence sequenceWithData;
        private int lineNumber = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor chooses default encoding based on alphabet.
        /// </summary>
        public GenBankParser()
        {
            LocationBuilder = new LocationBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the GenBankParser class.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public GenBankParser(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new InvalidDataException();
            }

            Filename = filename;
            LocationBuilder = new LocationBuilder();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// The alphabet to use for parsed ISequence objects.  If this is not set, an alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        /// Location builder is used to build location objects from the location string 
        /// present in the features.
        /// By default an instance of LocationBuilder class is used to build location objects.
        /// </summary>
        public ILocationBuilder LocationBuilder { get; set; }

        /// <summary>
        /// Supported file types.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Properties.Resource.GENBANK_FILEEXTENSION;
            }
        }

        /// <summary>
        /// Gets the type of Parser i.e GenBank.
        /// This is intended to give developers some information 
        /// of the parser class.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.GENBANK_NAME;
            }
        }

        /// <summary>
        /// Gets the description of GenBank parser.
        /// This is intended to give developers some information 
        /// of the formatter class. This property returns a simple description of what the
        /// GenBankParser class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.GENBANKPARSER_DESCRIPTION;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses a single GenBank text from a reader into a sequence.
        /// </summary>
        /// <returns>A new Sequence instance containing parsed data.</returns>
        public IEnumerable<ISequence> Parse()
        {
            if (string.IsNullOrEmpty(Filename))
            {
                throw new InvalidDataException();
            }

            using (StreamReader stream = new StreamReader(Filename))
            {
                Sequence sequence;
                string line = null;
                int noOfSequence = 0;
                do
                {
                    IAlphabet alphabet = Alphabet;

                    if (alphabet == null)
                    {
                        alphabet = Alphabets.DNA;
                    }

                    sequence = new Sequence(alphabet, string.Empty);

                    sequence.Metadata[Helper.GenBankMetadataKey] = new GenBankMetadata();

                    // parse the file
                    line = ParseHeaders(ref sequence, noOfSequence, line, stream);
                    line = ParseFeatures(line, ref sequence, stream);
                    ParseSequence(ref line, ref sequence, stream);
                    ISequence finalSequence = CopyMetadata(sequence);
                    noOfSequence++;

                    yield return finalSequence;
                }
                while (line != null);
            }
        }

        /// <summary>
        /// Parses a single GenBank text from a reader into a sequence.
        /// </summary>
        /// <returns>A new Sequence instance containing parsed data.</returns>
        public IEnumerable<ISequence> Parse(StreamReader reader)
        {
            Sequence sequence;
            string line = null;
            int noOfSequence = 0;

            while (!reader.EndOfStream)
            {
                IAlphabet alphabet = Alphabet;

                if (alphabet == null)
                {
                    alphabet = Alphabets.DNA;
                }

                sequence = new Sequence(alphabet, string.Empty);

                sequence.Metadata[Helper.GenBankMetadataKey] = new GenBankMetadata();

                // parse the file
                line = ParseHeaders(ref sequence, noOfSequence, line, reader);
                line = ParseFeatures(line, ref sequence, reader);
                ParseSequence(ref line, ref sequence, reader);
                ISequence finalSequence = CopyMetadata(sequence);
                noOfSequence++;

                yield return finalSequence;
            }
        }

        /// <summary>
        /// Opens the stream for the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public void Open(string filename)
        {
            // if the file is already open throw invalid 
            if (!string.IsNullOrEmpty(Filename))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.FileAlreadyOpen, this.Filename));
            }

            // Validate the file - by try to open.
            using (new StreamReader(filename))
            {
            }

            Filename = filename;
        }

        /// <summary>
        /// Closes the Writer.
        /// </summary>
        public void Close()
        {
            Filename = null;
        }

        /// <summary>
        /// Disposes the writer.
        /// </summary>
        public void Dispose()
        {
            Filename = null;
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Returns the alphabet depending on the specified molecule type.
        /// </summary>
        /// <param name="moleculeType">Molecule type.</param>
        /// <returns>IAlphabet instance.</returns>
        private static IAlphabet GetAlphabet(MoleculeType moleculeType)
        {
            switch (moleculeType)
            {
                case MoleculeType.DNA:
                case MoleculeType.NA:
                    return Alphabets.DNA;
                case MoleculeType.RNA:
                    return Alphabets.RNA;
                case MoleculeType.Protein:
                    return Alphabets.Protein;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Reads the next line of text, storing it in the Line property.  If SkipBlankLines is
        /// true, any lines containing only white space are skipped.
        /// </summary>
        /// <param name="line">The current line.</param>
        /// <param name="streamReader">The stream reader.</param>
        /// <returns>The next line.</returns>
        private string GoToNextLine(string line, StreamReader streamReader)
        {
            if (line == string.Empty)
            {
                line = streamReader.ReadLine();
                lineNumber++;

                while (line == string.Empty)
                {
                    if (streamReader.EndOfStream)
                    {
                        return null;
                    }

                    line = streamReader.ReadLine();
                    lineNumber++;
                }

                return line;
            }

            line = streamReader.ReadLine();
            lineNumber++;

            while (line == string.Empty)
            {
                if (streamReader.EndOfStream)
                {
                    return null;
                }

                line = streamReader.ReadLine();
                lineNumber++;
            }

            return line;
        }

        /// <summary>
        /// Adds a qualifier to the feature object. The sub-items of a feature are referred to as qualifiers.  These do not have unique
        /// keys, so they are stored as lists in the SubItems dictionary.
        /// </summary>
        /// <param name="feature">The feature to which qualifier is to be added.</param>
        /// <param name="qualifierKey">The qualifier key to be added to the feature.</param>
        /// <param name="qualifierValue">The qualifier value.</param>
        private static void AddQualifierToFeature(FeatureItem feature, string qualifierKey, string qualifierValue)
        {
            if (!feature.Qualifiers.ContainsKey(qualifierKey))
            {
                feature.Qualifiers[qualifierKey] = new List<string>();
            }

            feature.Qualifiers[qualifierKey].Add(qualifierValue);
        }

        /// <summary>
        /// Parses the GenBank headers from the GenBank file.
        /// parses everything before the features section
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="noOfSequence">The current sequence index.</param>
        /// <param name="line">parse line</param>
        /// <param name="stream">The stream reader.</param>
        /// <returns>The parsed line.</returns>
        private string ParseHeaders(ref Sequence sequence, int noOfSequence, string line, StreamReader stream)
        {
            GenBankMetadata metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
            string data;
            string[] tokens;

            // only allow one locus line
            bool haveParsedLocus = false;
            string lineData;
            if (noOfSequence == 0)
            {
                line = string.Empty;
                line = GoToNextLine(line, stream);
            }

            // parse until we hit the features or sequence section
            bool haveFinishedHeaders = false;

            while ((line != null) && !haveFinishedHeaders)
            {
                switch (GetLineHeader(line, DataIndent))
                {
                    case "LOCUS":
                        if (haveParsedLocus)
                        {
                            string message = String.Format(CultureInfo.CurrentCulture, Properties.Resource.ParserSecondLocus);
                            Trace.Report(message);
                            throw new InvalidDataException(message);
                        }

                        line = ParseLocusByTokens(line, ref sequence, stream);
                        metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
                        haveParsedLocus = true;
                        // don't go to next line; current line still needs to be processed
                        break;

                    case "VERSION":
                        lineData = GetLineData(line, DataIndent);

                        tokens = lineData.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        // first token contains accession and version
                        Match m = Regex.Match(tokens[0], @"^(?<accession>\w+)\.(?<version>\d+)$");
                        metadata.Version = new GenBankVersion();

                        if (m.Success)
                        {
                            metadata.Version.Version = m.Groups["version"].Value;
                            // The first token in the data from the accession line is referred to as
                            // the primary accession number, and should be the one used here in the
                            // version line.
                            string versionLineAccession = m.Groups["accession"].Value;
                            if (metadata.Accession == null)
                            {
                                ApplicationLog.WriteLine("WARN: VERSION processed before ACCESSION");
                            }
                            else
                            {
                                if (!versionLineAccession.Equals(metadata.Accession.Primary))
                                {
                                    ApplicationLog.WriteLine("WARN: VERSION tag doesn't match ACCESSION");
                                }
                                else
                                {
                                    metadata.Version.Accession = metadata.Accession.Primary;
                                }
                            }
                        }

                        // second token contains primary ID
                        m = Regex.Match(tokens[1], @"^GI:(?<primaryID>.*)");
                        if (m.Success)
                        {
                            metadata.Version.GiNumber = m.Groups["primaryID"].Value;
                        }

                        line = GoToNextLine(line, stream);
                        break;

                    case "PROJECT":
                        lineData = GetLineData(line, DataIndent);
                        tokens = lineData.Split(':');

                        if (tokens.Length == 2)
                        {
                            metadata.Project = new ProjectIdentifier { Name = tokens[0] };
                            tokens = tokens[1].Split(',');
                            for (int i = 0; i < tokens.Length; i++)
                            {
                                metadata.Project.Numbers.Add(tokens[i]);
                            }
                        }
                        else
                        {
                            ApplicationLog.WriteLine("WARN: unexpected PROJECT header: " + line);
                        }

                        line = GoToNextLine(line, stream);
                        break;

                    case "SOURCE":
                        line = ParseSource(line, ref sequence, stream);
                        metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
                        // don't go to next line; current line still needs to be processed
                        break;

                    case "REFERENCE":
                        line = ParseReferences(line, ref sequence, stream);   // can encounter more than one
                        metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
                        // don't go to next line; current line still needs to be processed
                        break;

                    case "COMMENT":
                        line = ParseComments(line, ref sequence, stream);   // can encounter more than one
                        metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
                        // don't go to next line; current line still needs to be processed
                        break;

                    case "PRIMARY":
                        // This header is followed by sequence info in a table format that could be
                        // stored in a custom object.  The first line contains column headers.
                        // For now, just validate the presence of the headers, and save the data
                        // as a string.
                        lineData = GetLineData(line, DataIndent);
                        tokens = lineData.Split("\t ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        // Validating for minimum two headers.
                        if (tokens.Length != 4)
                        {
                            string message = String.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resource.ParserPrimaryLineError,
                                    line);
                            Trace.Report(message);
                            throw new InvalidDataException(message);
                        }

                        string primaryData = ParseMultiLineData(ref line, Environment.NewLine, DataIndent, stream);
                        metadata.Primary = primaryData;

                        // don't go to next line; current line still needs to be processed
                        break;

                    // all the following are extracted the same way - possibly multiline
                    case "DEFINITION":
                        metadata.Definition = ParseMultiLineData(ref line, " ", DataIndent, stream);
                        break;
                    case "ACCESSION":
                        data = ParseMultiLineData(ref line, " ", DataIndent, stream);
                        metadata.Accession = new GenBankAccession();
                        string[] accessions = data.Split(' ');
                        metadata.Accession.Primary = accessions[0];

                        for (int i = 1; i < accessions.Length; i++)
                        {
                            metadata.Accession.Secondary.Add(accessions[i]);
                        }

                        break;

                    case "DBLINK":
                        data = ParseMultiLineData(ref line, "\n", DataIndent, stream);
                        metadata.DbLinks = new List<CrossReferenceLink>();
                        foreach (string link in data.Split('\n'))
                        {
                            tokens = link.Split(':');

                            if (tokens.Length == 2)
                            {
                                CrossReferenceLink newLink = new CrossReferenceLink();
                                //metadata.DbLink = new CrossReferenceLink();
                                if (string.Compare(tokens[0], CrossReferenceType.Project.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    newLink.Type = CrossReferenceType.Project;
                                }
                                else if (string.Compare(tokens[0], CrossReferenceType.BioProject.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    newLink.Type = CrossReferenceType.BioProject;
                                }
                                else
                                {
                                    newLink.Type = CrossReferenceType.None;
                                    DescriptionAttribute[] attributes = (DescriptionAttribute[])CrossReferenceType.TraceAssemblyArchive.GetType().GetField(CrossReferenceType.TraceAssemblyArchive.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
                                    if (attributes != null && attributes.Length > 0)
                                    {
                                        if (string.Compare(tokens[0], attributes[0].Description, StringComparison.OrdinalIgnoreCase) == 0)
                                        {
                                            newLink.Type = CrossReferenceType.TraceAssemblyArchive;
                                        }
                                    }
                                }
                                tokens = tokens[1].Split(',');
                                for (int i = 0; i < tokens.Length; i++)
                                {
                                    newLink.Numbers.Add(tokens[i]);
                                }
                                metadata.DbLinks.Add(newLink);
                            }
                            else
                            {
                                ApplicationLog.WriteLine("WARN: unexpected DBLINK header: " + line);
                            }
                        }
                        break;

                    case "DBSOURCE":
                        metadata.DbSource = ParseMultiLineData(ref line, " ", DataIndent, stream);
                        break;

                    case "KEYWORDS":
                        metadata.Keywords = ParseMultiLineData(ref line, " ", DataIndent, stream);
                        break;

                    case "SEGMENT":
                        data = ParseMultiLineData(ref line, " ", DataIndent, stream);
                        const string delimeter = "of";
                        tokens = data.Split(delimeter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2)
                        {
                            metadata.Segment = new SequenceSegment();
                            int outvalue;
                            if (int.TryParse(tokens[0].Trim(), out outvalue))
                            {
                                metadata.Segment.Current = outvalue;
                            }
                            else
                            {
                                ApplicationLog.WriteLine("WARN: unexpected SEGMENT header: " + line);
                            }

                            if (int.TryParse(tokens[1].Trim(), out outvalue))
                            {
                                metadata.Segment.Count = outvalue;
                            }
                            else
                            {
                                ApplicationLog.WriteLine("WARN: unexpected SEGMENT header: " + line);
                            }
                        }
                        else
                        {
                            ApplicationLog.WriteLine("WARN: unexpected SEGMENT header: " + line);
                        }

                        break;

                    // all the following indicate sections beyond the headers parsed by this method
                    case "FEATURES":
                    case "BASE COUNT":
                    case "ORIGIN":
                    case "CONTIG":
                        haveFinishedHeaders = true;
                        break;

                    default:
                        string lineHeader = GetLineHeader(line, DataIndent);
                        lineData = GetLineData(line, DataIndent);
                        ApplicationLog.WriteLine(ToString() + "WARN: unknown {0} -> {1}", lineHeader, lineData);
                        string errMessage = String.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resource.ParseHeaderError,
                                    lineHeader);
                        Trace.Report(errMessage);
                        throw new InvalidDataException(errMessage);
                }
            }

            // check for required features
            if (!haveParsedLocus)
            {
                string message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, Name);
                Trace.Report(message);
                throw new InvalidDataException(message);
            }

            return line;
        }

        /// <summary>
        /// Parses the GenBank LOCUS using a token based approach which provides more flexibility for 
        /// GenBank documents that do not follow the standard 100%.
        /// </summary>
        /// <param name="line">parse line</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="stream">The stream reader.</param>
        /// <returns>The parsed line.</returns>
        private string ParseLocusByTokens(string line, ref Sequence sequence, StreamReader stream)
        {
            string lineData = GetLineData(line, DataIndent);
            var locusInfo = new GenBankLocusTokenParser().Parse(lineData);
            IAlphabet alphabet = GetAlphabet(locusInfo.MoleculeType);

            if (Alphabet != null && Alphabet != alphabet)
            {
                Trace.Report(Properties.Resource.ParserIncorrectAlphabet);
                throw new InvalidDataException(Properties.Resource.ParserIncorrectAlphabet);
            }

            sequence.ID = locusInfo.Name;
            var metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
            metadata.Locus = locusInfo;
            line = GoToNextLine(line, stream);
            return line;
        }

        /// <summary>
        /// Parses the GenBank Reference information from the GenBank file.
        /// </summary>
        /// <param name="line">parse line</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="stream">The stream reader.</param>
        /// <returns>The parsed line.</returns>
        private string ParseReferences(string line, ref Sequence sequence, StreamReader stream)
        {
            GenBankMetadata metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
            IList<CitationReference> referenceList = metadata.References;
            CitationReference reference = null;

            while (line != null)
            {
                string lineHeader = GetLineHeader(line, DataIndent);
                if (lineHeader == "REFERENCE")
                {
                    // add previous reference
                    if (reference != null)
                    {
                        referenceList.Add(reference);
                    }

                    // check for start/end e.g. (bases 1 to 118), or prose notes
                    string lineData = GetLineData(line, DataIndent);

                    Match m = Regex.Match(lineData, @"^(?<number>\d+)(\s+\((?<location>.*)\))?");
                    if (!m.Success)
                    {
                        string message = String.Format(
                                CultureInfo.CurrentCulture,
                                Properties.Resource.ParserReferenceError,
                                lineData);
                        Trace.Report(message);
                        throw new InvalidDataException(message);
                    }

                    // create new reference
                    string number = m.Groups["number"].Value;
                    string location = m.Groups["location"].Value;
                    reference = new CitationReference();
                    int outValue;
                    if (!int.TryParse(number, out outValue))
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.InvalidRefNumber, number));
                    reference.Number = outValue;
                    reference.Location = location;
                    line = GoToNextLine(line, stream);
                }
                else if (line.StartsWith(" ", StringComparison.Ordinal))
                {
                    switch (lineHeader)
                    {
                        // all the following are extracted the same way - possibly multiline
                        case "AUTHORS":
                            reference.Authors = ParseMultiLineData(ref line, " ", DataIndent, stream);
                            break;
                        case "CONSRTM":
                            reference.Consortiums = ParseMultiLineData(ref line, " ", DataIndent, stream);
                            break;
                        case "TITLE":
                            reference.Title = ParseMultiLineData(ref line, " ", DataIndent, stream);
                            break;
                        case "JOURNAL":
                            reference.Journal = ParseMultiLineData(ref line, " ", DataIndent, stream);
                            break;
                        case "REMARK":
                            reference.Remarks = ParseMultiLineData(ref line, " ", DataIndent, stream);
                            break;
                        case "MEDLINE":
                            reference.Medline = ParseMultiLineData(ref line, " ", DataIndent, stream);
                            break;
                        case "PUBMED":
                            reference.PubMed = ParseMultiLineData(ref line, " ", DataIndent, stream);
                            break;

                        default:
                            string message = String.Format(
                                    CultureInfo.CurrentCulture,
                                    Properties.Resource.ParserInvalidReferenceField,
                                    lineHeader);
                            Trace.Report(message);
                            throw new InvalidDataException(message);
                    }
                }
                else
                {
                    // add last reference
                    if (reference != null)
                    {
                        referenceList.Add(reference);
                    }

                    // don't go to next line; current line still needs to be processed
                    break;
                }
            }

            return line;
        }

        /// <summary>
        /// Parses the GenBank Comments from the GenBank file.
        /// </summary>
        /// <param name="line">parse line</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="stream">The stream reader.</param>
        /// <returns>The parsed line.</returns>
        private string ParseComments(string line, ref Sequence sequence, StreamReader stream)
        {
            IList<string> commentList = ((GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey]).Comments;

            string lineHeader = GetLineHeader(line, DataIndent);
            while ((line != null) && lineHeader == "COMMENT")
            {
                string data = ParseMultiLineData(ref line, Environment.NewLine, DataIndent, stream);
                commentList.Add(data);
                lineHeader = GetLineHeader(line, DataIndent);

                // don't go to next line; current line still needs to be processed
            }

            return line;
        }

        /// <summary>
        /// Parses the GenBank source data from the GenBank file.
        /// </summary>
        /// <param name="line">parse line</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="stream">The stream reader.</param>
        /// <returns>The parsed line.</returns>
        private string ParseSource(string line, ref Sequence sequence, StreamReader stream)
        {
            string source = string.Empty;
            string organism = string.Empty;
            string classLevels = string.Empty;

            while (line != null)
            {
                string lineHeader = GetLineHeader(line, DataIndent);
                string lineData;
                if (lineHeader == "SOURCE")
                {
                    // data can be multiline. spec says last line must end with period
                    // (note: this doesn't apply unless multiline)
                    bool lastDotted = true;
                    lineData = GetLineData(line, DataIndent);
                    source = lineData;

                    line = GoToNextLine(line, stream);
                    lineHeader = GetLineHeader(line, DataIndent);
                    while ((line != null) && (lineHeader == string.Empty))
                    {
                        source += " " + GetLineData(line, DataIndent);
                        lastDotted = (source.EndsWith(".", StringComparison.Ordinal));
                        line = GoToNextLine(line, stream);
                        lineHeader = GetLineHeader(line, DataIndent);
                    }

                    if (!lastDotted && Trace.Want(Trace.SeqWarnings))
                    {
                        Trace.Report("GenBank.ParseSource", Properties.Resource.OutOfSpec, source);
                    }

                    // don't go to next line; current line still needs to be processed
                }
                else if (line[0] == ' ')
                {
                    if (lineHeader != "ORGANISM")
                    {
                        string message = String.Format(
                                CultureInfo.CurrentCulture,
                                Properties.Resource.ParserInvalidSourceField,
                                lineHeader);
                        Trace.Report(message);
                        throw new InvalidDataException(message);
                    }

                    lineData = GetLineData(line, DataIndent);

                    // this also can be multiline
                    organism = lineData;

                    line = GoToNextLine(line, stream);
                    lineHeader = GetLineHeader(line, DataIndent);
                    while ((line != null) && (lineHeader == string.Empty))
                    {
                        if (line.EndsWith(";", StringComparison.Ordinal) || line.EndsWith(".", StringComparison.Ordinal))
                        {
                            if (!String.IsNullOrEmpty(classLevels))
                            {
                                classLevels += " ";
                            }

                            lineData = GetLineData(line, DataIndent);
                            classLevels += lineData;
                        }
                        else
                        {
                            organism += " " + lineData;
                        }

                        line = GoToNextLine(line, stream);
                        lineHeader = GetLineHeader(line, DataIndent);
                    }

                    // don't go to next line; current line still needs to be processed
                }
                else
                {
                    // don't go to next line; current line still needs to be processed
                    break;
                }
            }

            GenBankMetadata metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
            metadata.Source = new SequenceSource { CommonName = source };
            if (!string.IsNullOrEmpty(organism))
            {
                int index = organism.IndexOf(" ", StringComparison.Ordinal);
                if (index > 0)
                {
                    metadata.Source.Organism.Genus = organism.Substring(0, index);
                    if (organism.Length > index)
                    {
                        index++;
                        metadata.Source.Organism.Species = organism.Substring(index, organism.Length - index);
                    }
                }
                else
                {
                    metadata.Source.Organism.Genus = organism;
                }
            }

            metadata.Source.Organism.ClassLevels = classLevels;
            string genus;
            if (classLevels.Trim().Length > 0)
            {
                genus = classLevels.TrimEnd('.').Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                if (!genus.Equals(metadata.Source.Organism.Genus.Trim()))
                {
                    metadata.Source.Organism.Species = organism;
                    metadata.Source.Organism.Genus = genus;
                }
            }

            return line;
        }

        /// <summary>
        /// Parses the GenBank features from the GenBank file.
        /// </summary>
        /// <param name="line">parse line</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="stream">The stream reader.</param>
        /// <returns>The parsed line.</returns>
        private string ParseFeatures(string line, ref Sequence sequence, StreamReader stream)
        {
            ILocationBuilder locBuilder = LocationBuilder;
            if (locBuilder == null)
            {
                throw new InvalidOperationException(Properties.Resource.NullLocationBuild);
            }

            // set data indent for features
            string lineData;

            // The sub-items of a feature are referred to as qualifiers.  These do not have unique
            // keys, so they are stored as lists in the SubItems dictionary.
            SequenceFeatures features = new SequenceFeatures();
            IList<FeatureItem> featureList = features.All;

            while (line != null)
            {
                string lineHeader = GetLineHeader(line, FeatureDataIndent);
                if (String.IsNullOrEmpty(line) || lineHeader == "FEATURES")
                {
                    line = GoToNextLine(line, stream);
                    continue;
                }

                if (line[0] != ' ')
                {
                    // start of non-feature text
                    break;
                }

                if (lineHeader == null)
                {
                    string message = Properties.Resource.GenbankEmptyFeature;
                    Trace.Report(message);
                    throw new InvalidDataException(message);
                }

                // check for multi-line location string
                lineData = GetLineData(line, FeatureDataIndent);
                string featureKey = lineHeader;
                string location = lineData;
                line = GoToNextLine(line, stream);
                lineData = GetLineData(line, FeatureDataIndent);
                lineHeader = GetLineHeader(line, FeatureDataIndent);
                while ((line != null) && (lineHeader == string.Empty) &&
                    (lineData != string.Empty) && !lineData.StartsWith("/", StringComparison.Ordinal))
                {
                    location += lineData;
                    GetLineData(line, FeatureDataIndent);
                    line = GoToNextLine(line, stream);
                    lineData = GetLineData(line, FeatureDataIndent);
                    lineHeader = GetLineHeader(line, FeatureDataIndent);
                }

                // create features as MetadataListItems
                FeatureItem feature = new FeatureItem(featureKey, locBuilder.GetLocation(location));

                // process the list of qualifiers, which are each in the form of
                // /key="value"
                string qualifierKey = string.Empty;
                string qualifierValue = string.Empty;
                bool quotationMarkStarted = false;

                while (line != null)
                {
                    lineData = GetLineData(line, FeatureDataIndent);
                    lineHeader = GetLineHeader(line, FeatureDataIndent);
                    if ((lineHeader == string.Empty) && (lineData != null))
                    {
                        // '/' denotes a continuation of the previous line
                        // Note that, if there are multiple lines of qualifierValue, 
                        // sometimes a line break will happen such that a "/" which is 
                        // part of the qualifierValue will start a continuation line. 
                        // This is identified by verifying open and closing double quotes.
                        if (lineData.StartsWith("/", StringComparison.Ordinal) && !quotationMarkStarted)
                        {
                            // new qualifier; save previous if this isn't the first
                            if (!String.IsNullOrEmpty(qualifierKey))
                            {
                                AddQualifierToFeature(feature, qualifierKey, qualifierValue);
                            }

                            // set the key and value of this qualifier
                            int equalsIndex = lineData.IndexOf('=');
                            if (equalsIndex < 0)
                            {
                                // no value, just key (this is allowed, see NC_005213.gbk)
                                qualifierKey = lineData.Substring(1);
                                qualifierValue = string.Empty;
                            }
                            else if (equalsIndex > 0)
                            {
                                qualifierKey = lineData.Substring(1, equalsIndex - 1);
                                qualifierValue = lineData.Substring(equalsIndex + 1);
                                quotationMarkStarted = qualifierValue[0] == '"';
                                if (qualifierValue[qualifierValue.Length - 1] == '"')
                                {
                                    quotationMarkStarted = false;
                                }
                            }
                            else
                            {
                                string message = String.Format(
                                        CultureInfo.CurrentCulture,
                                        Properties.Resource.GenbankInvalidFeature,
                                        line);
                                Trace.Report(message);
                                throw new InvalidDataException(message);
                            }
                        }
                        else
                        {
                            // Continuation of previous line; "note" gets a line break, and
                            // everything else except "translation" and "transl_except" gets a
                            // space to separate words.
                            if (qualifierKey == "note")
                            {
                                qualifierValue += Environment.NewLine;
                            }
                            else if (qualifierKey != "translation" && qualifierKey != "transl_except")
                            {
                                qualifierValue += " ";
                            }

                            qualifierValue += lineData;
                            if (qualifierValue[qualifierValue.Length - 1] == '"')
                            {
                                quotationMarkStarted = false;
                            }
                        }

                        line = GoToNextLine(line, stream);
                    }
                    else if (line.StartsWith("\t", StringComparison.Ordinal))
                    {
                        // this seems to be data corruption; but BioPerl test set includes
                        // (old, 2003) NT_021877.gbk which has this problem, so we
                        // handle it
                        ApplicationLog.WriteLine("WARN: nonstandard line format at line {0}: '{1}'", lineNumber, line);
                        qualifierValue += " " + line.Trim();
                        if (qualifierValue[qualifierValue.Length - 1] == '"')
                        {
                            quotationMarkStarted = false;
                        }

                        line = GoToNextLine(line, stream);
                    }
                    else
                    {
                        break;
                    }
                }

                // add last qualifier
                if (!String.IsNullOrEmpty(qualifierKey))
                {
                    AddQualifierToFeature(feature, qualifierKey, qualifierValue);
                }

                // still add feature, even if it has no qualifiers
                featureList.Add(StandardFeatureMap.GetStandardFeatureItem(feature));
            }

            if (featureList.Count > 0)
            {
                ((GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey]).Features = features;
            }

            return line;
        }

        /// <summary>
        /// Parses the GenBank Sequence from the GenBank file. 
        /// Handle optional BASE COUNT, then ORIGIN and sequence data.
        /// </summary>
        /// <param name="line">parse line</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="stream">The stream reader.</param>
        private void ParseSequence(ref string line, ref Sequence sequence, StreamReader stream)
        {
            GenBankMetadata metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];

            while (line != null)
            {
                if (line.StartsWith("//", StringComparison.Ordinal))
                {
                    line = GoToNextLine(line, stream);
                    break;
                    // end of sequence record
                }

                // set data indent for sequence headers
                string lineHeader = GetLineHeader(line, DataIndent);
                switch (lineHeader)
                {
                    case "BASE COUNT":
                        // The BASE COUNT linetype is obsolete and was removed
                        // from the GenBank flat-file format in October 2003.  But if it is
                        // present, we will use it.  We get the untrimmed version since it
                        // starts with a right justified column.
                        metadata.BaseCount = line.Substring(DataIndent);
                        line = GoToNextLine(line, stream);
                        break;

                    case "ORIGIN":
                        // Change Note: The original implementation would validate the alphabet every line
                        // which would greatly impact performance on large sequences.  This updates the method
                        // to improve performance by validating the alphabet after parsing the sequence.
                        ParseOrigin(ref line, metadata, stream);
                        break;

                    case "CONTIG":
                        metadata.Contig = ParseMultiLineData(ref line, Environment.NewLine, DataIndent, stream);
                        // don't go to next line; current line still needs to be processed
                        break;

                    default:
                        string message = String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resource.ParserUnexpectedLineInSequence,
                            line);
                        Trace.Report(message);
                        throw new InvalidDataException(message);
                }
            }
        }

        /// <summary>
        /// Parses the GenBank Origin data from the GenBank file. 
        /// </summary>
        /// <param name="line">parse line</param>
        /// <param name="metadata">The GenBank metadata.</param>
        /// <param name="stream">The stream reader.</param>
        private void ParseOrigin(ref string line, GenBankMetadata metadata, StreamReader stream)
        {
            // The origin line can contain optional data; don't put empty string into
            // metadata.
            string lineData = GetLineData(line, DataIndent);
            if (!String.IsNullOrEmpty(lineData))
            {
                metadata.Origin = lineData;
            }

            line = GoToNextLine(line, stream);
            IAlphabet alphabet = null;

            var sequenceBuilder = new StringBuilder();
            while ((line != null) && line[0] == ' ')
            {
                // Using a regex is too slow.
                int len = line.Length;
                int k = 10;
                while (k < len)
                {
                    string seqData = line.Substring(k, Math.Min(10, len - k));

                    sequenceBuilder.Append(seqData);
                    k += 11;
                }

                line = GoToNextLine(line, stream);
            }

            var sequenceString = sequenceBuilder.ToString().Trim();
            if (!string.IsNullOrEmpty(sequenceString))
            {
                if (Alphabet == null)
                {
                    byte[] tempData = UTF8Encoding.UTF8.GetBytes(sequenceString.ToUpper(CultureInfo.InvariantCulture));
                    alphabet = Alphabets.AutoDetectAlphabet(tempData, 0, tempData.Length, alphabet);

                    if (alphabet == null)
                    {
                        var message = String.Format(CultureInfo.InvariantCulture, Properties.Resource.InvalidSymbolInString, line);
                        Trace.Report(message);
                        throw new InvalidDataException(message);
                    }
                }
                else
                {
                    alphabet = Alphabet;
                }

                sequenceWithData = new Sequence(alphabet, sequenceString);
            }
        }

        /// <summary>
        /// Parses the GenBank Origin data from the GenBank file. 
        /// returns a string of the data for a header block that spans multiple lines
        /// </summary>
        /// <param name="line">parse line</param>
        /// <param name="lineBreakSubstitution">The line break string to be substituted.</param>
        /// <param name="dataIndent">The data indent for the line.</param>
        /// <param name="stream">The stream reader.</param>
        /// <returns>The parsed line.</returns>
        private string ParseMultiLineData(ref string line, string lineBreakSubstitution, int dataIndent, StreamReader stream)
        {
            string lineData = GetLineData(line, dataIndent);
            string data = lineData;
            line = GoToNextLine(line, stream);
            string lineHeader = GetLineHeader(line, dataIndent);

            // while succeeding lines start with no header, add to data
            while ((line != null) && (lineHeader == string.Empty))
            {
                lineData = GetLineData(line, dataIndent);
                data += lineBreakSubstitution + lineData;
                line = GoToNextLine(line, stream);
                lineHeader = GetLineHeader(line, dataIndent);
            }

            return data;
        }

        /// <summary>
        /// Copy file-scope metadata to all the sequences in the list.
        /// </summary>
        /// Flag to indicate whether the resulting sequences should be in read-only mode or not.
        /// If this flag is set to true then the resulting sequences's isReadOnly property 
        /// will be set to true, otherwise it will be set to false.
        /// <param name="sequences">The sequence.</param>
        /// <returns>The metadata sequence.</returns>
        private ISequence CopyMetadata(ISequence sequences)
        {
            sequenceWithData.Metadata[Helper.GenBankMetadataKey] = new GenBankMetadata();
            sequenceWithData.ID = sequences.ID;
            foreach (KeyValuePair<string, object> pair in sequences.Metadata)
            {
                sequenceWithData.Metadata[pair.Key] = pair.Value;
            }

            return sequenceWithData;
        }

        /// <summary>
        /// Gets the Line Header. 
        /// </summary>
        /// <param name="line">The Line to be processed.</param>
        /// <param name="dataIndent">The Indent for Header Calculation.</param>
        /// <returns>Returns the header.</returns>
        private string GetLineHeader(string line, int dataIndent)
        {
            string lineHeader;
            if (line.Length >= dataIndent)
            {
                lineHeader = line.Substring(0, dataIndent).Trim();
            }
            else
            {
                lineHeader = line.Trim();
            }

            return lineHeader;
        }

        /// <summary>
        /// Gets the line Data.
        /// </summary>
        /// <param name="line">The Line to be processed.</param>
        /// <param name="dataIndent">The Indent for line Data Calculation.</param>
        /// <returns>Returns the line Data(excluding the line header).</returns>
        private string GetLineData(string line, int dataIndent)
        {
            string lineData;
            if (line.Length >= dataIndent)
            {
                lineData = line.Substring(dataIndent).Trim();
            }
            else
            {
                lineData = string.Empty;
            }

            return lineData;
        }

        #endregion Private Methods
    }
}
