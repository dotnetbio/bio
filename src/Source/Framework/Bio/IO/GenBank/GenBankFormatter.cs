using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bio.Util;
using System.Globalization;

namespace Bio.IO.GenBank
{
    /// <summary>
    /// Writes an ISequence to a particular location, usually a file. The output is formatted
    /// according to the GenBank file format. A method is also provided for quickly accessing
    /// the content in string form for applications that do not need to first write to file.
    /// </summary>
    public sealed class GenBankFormatter : ISequenceFormatter
    {
        #region Fields

        // the standard indent for data is different from the indent for headers or data in the
        // features section
        private static readonly string DataIndentString = Helper.StringMultiply(" ", 12);
        private static readonly string FeatureHeaderIndentString = Helper.StringMultiply(" ", 5);
        private static readonly string FeatureDataIndentString = Helper.StringMultiply(" ", 21);

        // the spec allows for up to 80 chars per line, but everyone else does 79
        private const int MaxLineLength = 79;

        // the sequence is output with each line containing 6 sets of 10 chars
        private const long SeqCharsPerChunk = 10;
        private const int SeqChunksPerLine = 6;

        private TextWriter writer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GenBankFormatter()
        {
            LocationBuilder = new LocationBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the GenBankFormatter class.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public GenBankFormatter(string filename)
        {
            Filename = filename;
            writer = new StreamWriter(filename);
            LocationBuilder = new LocationBuilder();
        }

        #endregion Constructors

        #region Properties
        /// <summary>
        /// Location builder is used to build location string from the location object present in the feature items.
        /// By default an instance of LocationBuilder class is used to get the location string.
        /// </summary>
        public ILocationBuilder LocationBuilder { get; set; }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gives the supported file types.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Properties.Resource.GENBANK_FILEEXTENSION;
            }
        }

        /// <summary>
        /// Gets the type of Formatter i.e GenBank.
        /// This is intended to give developers some information 
        /// of the formatter class.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.GENBANK_NAME;
            }
        }

        /// <summary>
        /// Gets the description of GenBank formatter.
        /// This is intended to give developers some information 
        /// of the formatter class. This property returns a simple description of what the
        /// GenBankFormatter class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.GENBANKFORMATTER_DESCRIPTION;
            }
        }

         #endregion Properties

        #region Public Methods

        /// <summary>
        /// Writes an ISequence to the specified file.
        /// </summary>
        /// <param name="sequence">The sequence to format.</param>
        public void Write(ISequence sequence)
        {
            if (string.IsNullOrEmpty(Filename) && this.writer == null)
            {
                throw new ArgumentNullException(Filename);
            }

            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            try
            {
                Format(sequence, writer);
            }
            catch
            {
                // If format failed, remove the created file.
                if (File.Exists(Filename))
                {
                    if (writer != null)
                    {
                        writer.Close();
                    }
                    File.Delete(Filename);
                }
                throw;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        /// <summary>
        /// Write a collection of ISequences to a file.
        /// </summary>
        /// <remarks>
        /// This method is overridden to format file-scope metadata that applies to all
        /// metadata that applies to all of the sequences in the file.
        /// </remarks>
        /// <param name="sequences">The sequences to write</param>
        public void Write(ICollection<ISequence> sequences)
        {
            if (string.IsNullOrEmpty(Filename) && this.writer == null)
            {
                throw new ArgumentNullException(Filename);
            }

            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            

            foreach (ISequence sequence in sequences)
            {
                WriteHeaders(sequence, writer);
                WriteFeatures(sequence, writer);
                WriteSequence(sequence, writer);
            }

            writer.Flush();
            writer.Close();
            writer.Dispose();
        }


        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public void Open(string filename)
        {
            Filename = filename;
            writer = new StreamWriter(Filename);
        }

        /// <summary>
        /// Opens the specified stream for writing sequences.
        /// </summary>
        /// <param name="outStream">StreamWriter to use.</param>
        public void Open(StreamWriter outStream)
        {
            if (this.writer != null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotClosed);
            }

            Filename = null;
            this.writer = outStream;
        }

        /// <summary>
        /// Closes the formatter.
        /// </summary>
        public void Close()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }

        
        /// <summary>
        /// Disposes the formatter.
        /// </summary>
        public void Dispose()
        {
            if (writer != null)
            {
                writer.Dispose();
            }
        }

       #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Writes an ISequence to a GenBank file in the location specified by the writer.
        /// </summary>
        /// <param name="sequence">The sequence to format.</param>
        /// <param name="txtWriter">The TextWriter used to write the formatted sequence text.</param>
        private void Format(ISequence sequence, TextWriter txtWriter)
        {
            WriteHeaders(sequence, txtWriter);
            WriteFeatures(sequence, txtWriter);
            WriteSequence(sequence, txtWriter);

            txtWriter.Flush();
        }

        // Write all the header sections that come before the features section.
        private void WriteHeaders(ISequence sequence, TextWriter txtWriter)
        {
            GenBankMetadata metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
            if (metadata != null)
            {
                WriteLocus(sequence, txtWriter);
                WriteHeaderSection("DEFINITION", metadata.Definition, txtWriter);

                if (metadata.Accession != null)
                {
                    WriteHeaderSection("ACCESSION", Helper.GetGenBankAccession(metadata.Accession), txtWriter);

                    string version;
                    if (metadata.Version != null)
                    {
                        version = metadata.Accession.Primary + "." + metadata.Version.Version;

                        if (!string.IsNullOrEmpty(metadata.Version.GiNumber))
                        {
                            version += "  GI:" + metadata.Version.GiNumber;
                        }
                        if (version.Length > 0)
                        {
                            WriteHeaderSection("VERSION", version, txtWriter);
                        }
                    }
                }

                if (metadata.Project != null)
                {
                    WriteHeaderSection("PROJECT", Helper.GetProjectIdentifier(metadata.Project), txtWriter);
                }

                if (metadata.DbLinks != null && metadata.DbLinks.Count>0)
                {
                    WriteHeaderSection("DBLINK", Helper.GetCrossReferenceLink(metadata.DbLinks), txtWriter);
                }

                WriteHeaderSection("DBSOURCE", metadata.DbSource, txtWriter);
                WriteHeaderSection("KEYWORDS", metadata.Keywords, txtWriter);

                if (metadata.Segment != null)
                {
                    WriteHeaderSection("SEGMENT", Helper.GetSequenceSegment(metadata.Segment), txtWriter);
                }

                WriteSource(metadata, txtWriter);
                WriteReferences(metadata, txtWriter);
                WriteComments(metadata, txtWriter);
                WriteHeaderSection("PRIMARY", metadata.Primary, txtWriter);

            }
        }

        private static void WriteLocus(ISequence sequence, TextWriter txtWriter)
        {
            // determine molecule and sequence type
            GenBankMetadata metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];

            GenBankLocusInfo locusInfo = null;
            string molType = sequence.Alphabet.Name;
            if (metadata != null)
            {
                locusInfo = metadata.Locus;
                molType = locusInfo.MoleculeType.ToString();
            }

            string seqType;
            if (sequence.Alphabet.Name != null)
            {
                if (molType == Alphabets.Protein.Name)
                {
                    seqType = "aa";
                    molType = string.Empty; // protein files don't use molecule type
                }
                else
                {
                    seqType = "bp";
                }
            }
            else
            {
                if (sequence.Alphabet == Alphabets.Protein)
                {
                    seqType = "aa";
                    molType = string.Empty; // protein files don't use molecule type
                }
                else
                {
                    seqType = "bp";

                    if (sequence.Alphabet == Alphabets.DNA)
                    {
                        molType = Alphabets.DNA.Name;
                    }
                    else
                    {
                        molType = Alphabets.RNA.Name;
                    }
                }
            }

            // retrieve metadata fields
            string strandType = string.Empty;
            string strandTopology = string.Empty;
            string division = string.Empty;
            DateTime date = DateTime.Now;

            if (locusInfo != null)
            {
                strandType = Helper.GetStrandType(locusInfo.Strand);

                strandTopology = Helper.GetStrandTopology(locusInfo.StrandTopology);
                if (locusInfo.DivisionCode != SequenceDivisionCode.None)
                {
                    division = locusInfo.DivisionCode.ToString();
                }

                date = locusInfo.Date;
            }

            txtWriter.WriteLine("{0,-12}{1,-16} {2,11} {3} {4,3}{5,-6}  {6,-8} {7,3} {8}",
                "LOCUS",
                sequence.ID,
                sequence.Count,
                seqType,
                strandType,
                molType,
                strandTopology,
                division,
                date.ToString("dd-MMM-yyyy",CultureInfo.InvariantCulture).ToUpper(CultureInfo.InvariantCulture));
        }

        private void WriteSource(GenBankMetadata metadata, TextWriter txtWriter)
        {
            if (metadata.Source != null)
            {
                string commonname = string.Empty;
                if (!string.IsNullOrEmpty(metadata.Source.CommonName))
                {
                    commonname = metadata.Source.CommonName;
                }

                WriteHeaderSection("SOURCE", commonname, txtWriter);

                string organism = string.Empty;
                if (!commonname.Equals(metadata.Source.Organism.Species))
                {
                    if (!string.IsNullOrEmpty(metadata.Source.Organism.Genus))
                    {
                        organism += metadata.Source.Organism.Genus;
                    }
                    organism += " ";
                }

                if (!string.IsNullOrEmpty(metadata.Source.Organism.Species))
                {
                    organism += metadata.Source.Organism.Species;
                }

                // Organism might be empty, trim the value to ensure that a string with one space is not written (writer fails on this)
                WriteHeaderSection("  ORGANISM", organism.Trim(), txtWriter);
                WriteHeaderSection(string.Empty, metadata.Source.Organism.ClassLevels, txtWriter);
            }
        }

        private void WriteReferences(GenBankMetadata metadata, TextWriter txtWriter)
        {
            if (metadata.References != null)
            {
                foreach (CitationReference reference in metadata.References)
                {
                    // format the data for the first line
                    string data = reference.Number.ToString(CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(reference.Location))
                    {
                        data = data.PadRight(3) + "(" + reference.Location + ")";
                    }

                    WriteHeaderSection("REFERENCE", data, txtWriter);
                    WriteHeaderSection("  AUTHORS", reference.Authors, txtWriter);
                    WriteHeaderSection("  CONSRTM", reference.Consortiums, txtWriter);
                    WriteHeaderSection("  TITLE", reference.Title, txtWriter);
                    WriteHeaderSection("  JOURNAL", reference.Journal, txtWriter);
                    WriteHeaderSection("  MEDLINE", reference.Medline, txtWriter);
                    WriteHeaderSection("  PUBMED", reference.PubMed, txtWriter);
                    WriteHeaderSection("  REMARK", reference.Remarks, txtWriter);
                }
            }
        }

        // Writes the comments, which are stored in a list of strings.
        private void WriteComments(GenBankMetadata metadata, TextWriter txtWriter)
        {
            foreach (string comment in metadata.Comments)
            {
                WriteHeaderSection("COMMENT", comment, txtWriter);
            }
        }

        private void WriteFeatures(ISequence sequence, TextWriter txtWriter)
        {
            ILocationBuilder locBuilder = LocationBuilder;
            if (locBuilder == null)
            {
                throw new InvalidOperationException(Properties.Resource.NullLocationBuild);
            }
            GenBankMetadata metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
            if (metadata != null && metadata.Features != null)
            {
                WriteFeatureSection("FEATURES", "Location/Qualifiers", txtWriter);

                // write the features in the order they were put in the list
                foreach (FeatureItem feature in metadata.Features.All)
                {
                    WriteFeatureSection(FeatureHeaderIndentString + feature.Key, locBuilder.GetLocationString(feature.Location), txtWriter);

                    // The sub-items of a feature are referred to as qualifiers.  These do not have
                    // unique keys, so they are stored as lists in the SubItems dictionary.
                    foreach (KeyValuePair<string, List<string>> qualifierList in feature.Qualifiers)
                    {
                        foreach (string qualifierValue in qualifierList.Value)
                        {
                            string data = "/" + qualifierList.Key;

                            if (qualifierValue != string.Empty)
                            {
                                data += "=" + qualifierValue;
                            }

                            // use a blank header; the qualifier key is part of the data
                            WriteFeatureSection(string.Empty, data, txtWriter);
                        }
                    }
                }
            }
        }

        // Writes a header and data string as a GenBank feature section, indenting the data of
        // each line to the standard feature indent.
        private void WriteFeatureSection(string header, string data, TextWriter txtWriter)
        {
            WriteGenBankSection(header, FeatureDataIndentString, data, txtWriter);
        }

        // Write the sequence and other post-features data.
        private void WriteSequence(ISequence sequence, TextWriter txtWriter)
        {
            // "BASE COUNT" is stored as "baseCount", not "base count"
            GenBankMetadata metadata = (GenBankMetadata)sequence.Metadata[Helper.GenBankMetadataKey];
            if (metadata != null && !string.IsNullOrEmpty(metadata.BaseCount))
            {
                txtWriter.WriteLine("BASE COUNT  " + metadata.BaseCount);
            }

            if (metadata != null && !string.IsNullOrEmpty(metadata.Contig))
            {
                WriteHeaderSection("CONTIG", metadata.Contig, txtWriter);
            }

            if (sequence.Count > 0)
            {
                if (metadata != null && !string.IsNullOrEmpty(metadata.Origin))
                {
                    WriteHeaderSection("ORIGIN", metadata.Origin, txtWriter);
                }
                else
                {
                    // always write at least a data-less origin line before the sequence, even
                    // if we don't have an origin stored in metadata
                    txtWriter.WriteLine("ORIGIN");
                }

                WriteGenBankSequence(sequence);
            }

            txtWriter.WriteLine("//");
        }

        // Output 6 groups of 10 symbols per line.
        private void WriteGenBankSequence(ISequence sequence)
        {
            bool done = false;
            long symbolIndex = 0;
            while (!done)
            {
                // start each line with the symbol number
                StringBuilder line = new StringBuilder(string.Format("{0,9}", symbolIndex + 1));

                // next add 6 groups of 10, with groups separated by spaces
                for (int chunkIndex = 0; chunkIndex < SeqChunksPerLine && !done; chunkIndex++)
                {
                    // set done = true if this is the last chunk
                    done = SeqCharsPerChunk >= sequence.Count - symbolIndex;
                    long chunkSize = done ? sequence.Count - symbolIndex : SeqCharsPerChunk;

                    // append the chunk
                    line.Append(" ");
                    for (long start = symbolIndex; symbolIndex < start + chunkSize; symbolIndex++)
                    {
                        line.Append((char)sequence[symbolIndex]); 
                    }
                }

                writer.WriteLine(line.ToString().ToLower(CultureInfo.InvariantCulture));
            }
        }


        // Writes a header and data string as a GenBank header section, indenting the data of
        // each line to the standard header indent.
        private void WriteHeaderSection(string header, string data, TextWriter txtWriter)
        {
            if (data != null)
            {
                WriteGenBankSection(header, DataIndentString, data, txtWriter);
            }
        }

        /// Writes a header and data string as a GenBank header section, indenting the data of
        /// each line to the length of the given indent string.
        private static void WriteGenBankSection(string header, string indentString, string data, TextWriter txtWriter)
        {
            int maxLineDataLength = MaxLineLength - indentString.Length;
            bool firstLine = true;

            // process the data by chunks using any line breaks it already contains
            foreach (string dataChunk in data.Split('\r', '\n'))
            {
                int lineDataLength;
                for (int lineStart = 0; lineStart < dataChunk.Length; lineStart += lineDataLength)
                {
                    // skip spaces at start of this line of data
                    while (dataChunk[lineStart] == ' ')
                    {
                        lineStart++;
                    }

                    // use the header for the first line, and the indent string for subsequent
                    // lines, appending the data
                    string beforeData;
                    if (firstLine)
                    {
                        beforeData = header.PadRight(indentString.Length);
                        firstLine = false;
                    }
                    else
                    {
                        beforeData = indentString;
                    }

                    // check if the rest of this chunk will fit on one line
                    if (lineStart + maxLineDataLength >= dataChunk.Length)
                    {
                        // the rest of the chunk will be written to this line
                        lineDataLength = dataChunk.Length - lineStart;
                    }
                    else
                    {
                        // use the last space in the first maxLineDataLength characters as the line
                        // break; the startIndex for LastIndexOf actually needs to equal the end of
                        // the substring being examined - not intuitive
                        int startIndex = lineStart + maxLineDataLength;
                        int lineBreak = dataChunk.LastIndexOf(' ', startIndex, maxLineDataLength);

                        // if we didn't find a space, look for assorted other punctuation
                        if (lineBreak == -1)
                        {
                            // move the start index back 1; we'll include any non-space break
                            // char on the same line
                            startIndex--;

                            // try commas and semi-colons first
                            lineBreak = dataChunk.LastIndexOfAny(
                                new char[] { ',', ';' }, startIndex, maxLineDataLength);

                            // next try periods and dashes
                            if (lineBreak == -1)
                            {
                                lineBreak = dataChunk.LastIndexOfAny(
                                    new char[] { '.', '-' }, startIndex, maxLineDataLength);
                            }

                            // include the break char if we found one
                            if (lineBreak != -1)
                            {
                                lineBreak++;
                            }
                        }

                        // use the line break to determine the length of the data to be written into
                        // this line; if no good place to break was found in the first
                        // maxLineDataLength characters, use maxLineDataLength
                        lineDataLength = (lineBreak == -1 ? maxLineDataLength : lineBreak - lineStart);
                    }

                    txtWriter.WriteLine(beforeData + dataChunk.Substring(lineStart, lineDataLength));
                }
            }
        }

        #endregion Private Methods
    }
}
