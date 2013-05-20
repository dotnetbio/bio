using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Bio.IO.Gff
{
    /// <summary>
    /// Writes an ISequence to a particular location, usually a file. The output is formatted
    /// according to the GFF file format. A method is also provided for quickly accessing
    /// the content in string form for applications that do not need to first write to file.
    /// </summary>
    public class GffFormatter : ISequenceFormatter
    {
        #region Fields

        private const string HeaderMark = "##";
        private const string SourceVersionKey = "SOURCE-VERSION";
        private const string SourceVersionLowercaseKey = "source-version";
        private const string SourceKey = "source";
        private const string VersionKey = "version";
        private const string TypeKey = "TYPE";
        private const string TypeLowercaseKey = "type";
        private const string MultiTypeKey = "TYPE_";
        private const string MultiSeqDataKey = "SEQDATA_";
        private const string MultiSeqRegKey = "SEQUENCE-REGION_";
        private const string CommentSectionKey = "COMMENTSECTION_";
        private const string GffVersionLowercaseKey = "gff-version";
        private const string GffVersionKey = "GFF-VERSION";
        private const string DateKey = "DATE";
        private const string DateLowercaseKey = "date";
        private const string SeqRegKey = "sequence-region";
        private const string StartKey = "start";
        private const string EndKey = "end";
        private const string ScoreKey = "score";
        private const string StrandKey = "strand";
        private const string FrameKey = "frame";
        private const string FeaturesKey = "features";

        // the spec 32k chars per line, but it shows only 37 sequence symbols per line
        // in the examples
        private const long MaxSequenceSymbolsPerLine = 37;

        private TextWriter writer;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GffFormatter()
            : base()
        {
            ShouldWriteSequenceData = true;
        }

         /// <summary>
        /// Initializes a new instance of the FastAParser class.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public GffFormatter(string filename)
        {
            this.Filename = filename;
            this.writer = new StreamWriter(filename);
            ShouldWriteSequenceData = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Whether or not sequence data will be written as part of the GFF header information;
        /// This property is required as GFF files normally do not contain sequence data.
        /// Defaults value is true.
        /// </summary>
        public bool ShouldWriteSequenceData { get; set; }

       

        /// <summary>
        /// Gets the type of Formatter i.e GFF.
        /// This is intended to give developers some information 
        /// of the formatter class.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.GFF_NAME;
            }
        }

        /// <summary>
        /// Gets the description of GFF formatter.
        /// This is intended to give developers some information 
        /// of the formatter class. This property returns a simple description of what the
        /// GffFormatter class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.GFFFORMATTER_DESCRIPTION;
            }
        }

        #endregion Properties

        #region Public Methods
        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="outStream">Name of the file to open.</param>
        public void Open(StreamWriter outStream)
        {
            if (this.writer != null)
            {
                throw new InvalidOperationException(Properties.Resource.FileNotClosed);
            }

            this.writer = outStream;
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public void Open(string filename)
        {
            this.Filename = filename;
            if (this.writer != null)
            {
                this.writer.Close();
            }

            this.writer = new StreamWriter(this.Filename);
        }

        /// <summary>
        /// Writes an ISequence to the specified file.
        /// </summary>
        /// <param name="sequence">The sequence to format.</param>
        public void Write(ISequence sequence)
        {
            if (string.IsNullOrEmpty(this.Filename) && this.writer == null)
            {
                throw new ArgumentNullException(this.Filename);
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
                if (File.Exists(this.Filename))
                {
                    if (writer != null)
                    {
                        writer.Close();
                    }
                    File.Delete(this.Filename);
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
            if (string.IsNullOrEmpty(this.Filename) && this.writer == null)
            {
                throw new ArgumentNullException(this.Filename);
            }

            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            WriteHeaders(sequences, writer);

            foreach (ISequence sequence in sequences)
            {
                WriteFeatures(sequence, writer);
            }

            writer.Flush();
            writer.Close();
            writer.Dispose();
        }

        /// <summary>
        /// Converts an ISequence to a formatted text.
        /// </summary>
        /// <param name="sequence">The sequence to format.</param> 
        /// <returns>A string of the formatted text.</returns>
        public string FormatString(ISequence sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            using (TextWriter writer = new StringWriter())
            {
                Format(sequence, writer);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Close the Writer. 
        /// </summary>
        public void Close()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Gives the Supported types.
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
        /// <param name="writer">The TextWriter used to write the formatted sequence text.</param>
        private void Format(ISequence sequence, TextWriter writer)
        {
            WriteHeaders(new List<ISequence> { sequence }, writer);
            WriteFeatures(sequence, writer);

            writer.Flush();
        }

        

        // The headers for all sequences go at the top of the file before any features.
        private void WriteHeaders(ICollection<ISequence> sequenceList, TextWriter writer)
        {
            // look for file-scope data that is common to all sequences; null signifies no match
            MetadataListItem<string> sourceVersion = null;
            string source = null;
            string version = null;
            string type = null;
            bool firstSeq = true;
            ISequence commonSeq = null;
            List<string> typeExceptionList = new List<string>();
            List<string> seqDataExceptionList = new List<string>();
            List<string> seqRegExceptionList = new List<string>();

            foreach (ISequence sequence in sequenceList)
            {
                if (firstSeq)
                {
                    // consider first seq for common metadata.
                    commonSeq = sequence;

                    object tmpobj;
                    // source and version go together; can't output one without the other
                    if (sequence.Metadata.TryGetValue(SourceVersionKey, out tmpobj))
                    {
                        sourceVersion = tmpobj as MetadataListItem<string>;
                        if (sourceVersion != null && sourceVersion.SubItems.Count > 1)
                        {
                            source = sourceVersion.SubItems[SourceKey];
                            version = sourceVersion.SubItems[VersionKey];
                        }
                    }

                    // map to generic string; e.g. mRNA, tRNA -> RNA
                    type = GetGenericTypeString(sequence.Alphabet);

                    firstSeq = false;
                }
                else
                {
                    // source and version go together; can't output one without the other
                    if (source != null)
                    {
                        bool sourceAndVersionMatchOthers = false;

                        object tmpobj;
                        // source and version go together; can't output one without the other
                        if (sequence.Metadata.TryGetValue(SourceVersionKey, out tmpobj))
                        {
                            sourceVersion = tmpobj as MetadataListItem<string>;
                            if (sourceVersion != null && sourceVersion.SubItems.Count > 1)
                            {
                                sourceAndVersionMatchOthers = source == sourceVersion.SubItems[SourceKey] &&
                                version == sourceVersion.SubItems[VersionKey];
                            }
                        }

                        // set both to null if this seq source and version don't match previous ones
                        if (!sourceAndVersionMatchOthers)
                        {
                            source = null;
                            version = null;
                        }
                    }

                    // set type to null if this seq type doesn't match previous types
                    if (type != null && type != GetGenericTypeString(sequence.Alphabet))
                    {
                        type = null;
                    }
                }
            }

            if (commonSeq == null)
            {
                byte[] sequenceData = null;
                commonSeq = new Sequence(Alphabets.DNA, sequenceData);
            }

            WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 1);

            int totalTypeCount = commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(MultiTypeKey));
            int currentTypeCount = 0;
            int totalSeqData = commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(MultiSeqDataKey));
            int totalSeqRegs = commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(MultiSeqRegKey));

            ISequence seq = null;
            foreach (string key in commonSeq.Metadata.Keys)
            {
                string keyToCompare = key.ToUpperInvariant();
                string value = string.Empty;

                if (keyToCompare.Contains(CommentSectionKey))
                {
                    keyToCompare = CommentSectionKey;
                    value = commonSeq.Metadata[key] as string;
                }

                if (keyToCompare.Contains(MultiTypeKey))
                {
                    keyToCompare = MultiTypeKey;
                    value = commonSeq.Metadata[key] as string;
                }

                if (keyToCompare.Contains(MultiSeqDataKey))
                {
                    keyToCompare = MultiSeqDataKey;
                    value = commonSeq.Metadata[key] as string;
                }

                if (keyToCompare.Contains(MultiSeqRegKey))
                {
                    keyToCompare = MultiSeqRegKey;
                    value = commonSeq.Metadata[key] as string;
                }

                switch (keyToCompare)
                {
                    case CommentSectionKey:
                        writer.WriteLine(value);
                        break;

                    case GffVersionKey:
                        // formatting using gff version 2
                        WriteHeaderLine(writer, GffVersionLowercaseKey, "2");
                        WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 2);
                        break;

                    case SourceVersionKey:

                        // only output source if they all match
                        if (source != null)
                        {
                            WriteHeaderLine(writer, SourceVersionLowercaseKey, source, version);
                        }

                        WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 3);
                        break;

                    case DateKey:
                        // today's date
                        WriteHeaderLine(writer, DateLowercaseKey, DateTime.Today.ToString("yyyy-MM-dd"));
                        WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 4);
                        break;
                    case TypeKey:
                        // type header
                        if (type != null)
                        {
                            // output that the types all match; don't need to output if DNA, as DNA is default
                            if (type != "DNA")
                            {
                                WriteHeaderLine(writer, TypeLowercaseKey, type);
                            }

                        }
                        else if (totalTypeCount == 0)
                        {
                            foreach (ISequence sequence in sequenceList)
                            {
                                type = GetGenericTypeString(sequence.Alphabet);

                                // only output seq-specific type header if this seq won't have its type
                                // output as part of a sequence data header; don't need to output if DNA,
                                // as DNA is default
                                if (type != "DNA" &&
                                    (!ShouldWriteSequenceData || sequence.Count == 0))
                                {
                                    WriteHeaderLine(writer, TypeLowercaseKey, type, sequence.ID);
                                }
                            }
                        }
                        break;

                    case MultiTypeKey:

                        if (totalTypeCount > 0)
                        {
                            if (type == null)
                            {
                                seq = sequenceList.FirstOrDefault(S => S.ID.Equals(value));
                                if (seq != null)
                                {
                                    WriteHeaderLine(writer, TypeLowercaseKey, seq.Alphabet.Name.ToUpper(), seq.ID);
                                    typeExceptionList.Add(seq.ID);
                                }

                                currentTypeCount++;

                                if (currentTypeCount == totalTypeCount)
                                {
                                    foreach (ISequence sequence in sequenceList)
                                    {
                                        if (typeExceptionList.Contains(sequence.ID))
                                        {
                                            continue;
                                        }

                                        type = GetGenericTypeString(sequence.Alphabet);

                                        // only output seq-specific type header if this seq won't have its type
                                        // output as part of a sequence data header; don't need to output if DNA,
                                        // as DNA is default
                                        if (type != "DNA" &&
                                            (!ShouldWriteSequenceData || sequence.Count == 0))
                                        {
                                            WriteHeaderLine(writer, TypeLowercaseKey, type, sequence.ID);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // output that the types all match; don't need to output if DNA, as DNA is default
                                if (type != "DNA")
                                {
                                    WriteHeaderLine(writer, TypeLowercaseKey, type);
                                }

                                totalTypeCount = 0;
                            }
                        }
                        break;

                    case MultiSeqDataKey:
                        // sequence data
                        if (ShouldWriteSequenceData)
                        {

                            seq = sequenceList.FirstOrDefault(S => S.ID.Equals(value));
                            if (seq != null)
                            {
                                WriteSeqData(seq, type, writer);
                                seqDataExceptionList.Add(seq.ID);
                            }

                            totalSeqData--;

                            if (totalSeqData == 0)
                            {
                                foreach (ISequence sequence in sequenceList)
                                {
                                    if (seqDataExceptionList.Contains(sequence.ID))
                                    {
                                        continue;
                                    }

                                    WriteSeqData(sequence, type, writer);
                                }
                            }
                        }

                        break;

                    case MultiSeqRegKey:
                        seq = sequenceList.FirstOrDefault(S => S.ID.Equals(value));
                        if (seq != null)
                        {
                            if (seq.Metadata.ContainsKey(StartKey) && seq.Metadata.ContainsKey(EndKey))
                            {
                                WriteHeaderLine(writer, SeqRegKey, seq.ID,
                                    seq.Metadata[StartKey] as string, seq.Metadata[EndKey] as string);

                            }

                            seqRegExceptionList.Add(value);
                        }


                        totalSeqRegs--;
                        if (totalSeqRegs == 0)
                        {
                            // sequence-region header
                            foreach (ISequence sequence in sequenceList)
                            {
                                if (seqRegExceptionList.Contains(sequence.ID))
                                {
                                    continue;
                                }

                                if (sequence.Metadata.ContainsKey(StartKey) && sequence.Metadata.ContainsKey(EndKey))
                                {
                                    WriteHeaderLine(writer, SeqRegKey, sequence.ID,
                                        sequence.Metadata[StartKey] as string, sequence.Metadata[EndKey] as string);

                                }
                            }
                        }
                        break;
                }
            }
        }

        // writes the sequence to the specified writer.
        private void WriteSeqData(ISequence sequence, string type, TextWriter writer)
        {
            if (sequence.Count > 0)
            {
                byte[] TempSeqData = null;
                type = GetGenericTypeString(sequence.Alphabet);

                WriteHeaderLine(writer, type, sequence.ID);

                for (long lineStart = 0; lineStart < sequence.Count; lineStart += MaxSequenceSymbolsPerLine)
                {
                   long length = Math.Min(MaxSequenceSymbolsPerLine, sequence.Count - lineStart);
                    ISequence subSequence = sequence.GetSubSequence(lineStart, length);
                   TempSeqData = new byte[length];
                   for (int i = 0; i < length; i++)
                   {
                       TempSeqData[i] = subSequence[i];
                   }
                   string key = UTF8Encoding.UTF8.GetString(TempSeqData, 0, TempSeqData.Length);

                   WriteHeaderLine(writer, key);
                }

                WriteHeaderLine(writer, "end-" + type);
            }
        }

        // writes common metadata.
        private void WriteCommonMetadata(ISequence commonSeq, ICollection<ISequence> sequenceList, TextWriter writer, string source, string version, string type, int startFrom)
        {
            int totalTypeCount = commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(MultiTypeKey));

            if (startFrom == 1)
            {
                if (commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(GffVersionKey)) == 0)
                {
                    // formatting using gff version 2
                    WriteHeaderLine(writer, GffVersionLowercaseKey, "2");

                    WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 2);
                }
            }

            if (startFrom == 2)
            {

                if (source != null && commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(SourceVersionKey)) == 0)
                {
                    // only output source if they all match
                    WriteHeaderLine(writer, SourceVersionLowercaseKey, source, version);
                }

                WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 3);
            }

            if (startFrom == 3)
            {
                if (commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(DateKey)) == 0)
                {
                    // today's date
                    WriteHeaderLine(writer, DateLowercaseKey, DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

                    WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 4);
                }
            }

            if (startFrom == 4)
            {
                if (totalTypeCount == 0 && commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(TypeKey)) == 0)
                {
                    if (type == null)
                    {
                        foreach (ISequence sequence in sequenceList)
                        {
                            type = GetGenericTypeString(sequence.Alphabet);

                            // only output seq-specific type header if this seq won't have its type
                            // output as part of a sequence data header; don't need to output if DNA,
                            // as DNA is default
                            if (type != "DNA" &&
                                (!ShouldWriteSequenceData || sequence.Count == 0))
                            {
                                WriteHeaderLine(writer, TypeLowercaseKey, type, sequence.ID);
                            }
                        }
                    }
                    else
                    {
                        // output that the types all match; don't need to output if DNA, as DNA is default
                        if (type != "DNA")
                        {
                            WriteHeaderLine(writer, TypeLowercaseKey, type);
                        }
                    }
                }
            }
        }

        // Returns "DNA", "RNA", "Protein", or null.
        private string GetGenericTypeString(IAlphabet alphabet)
        {
            if (alphabet == Alphabets.DNA)
            {
                return "DNA";
            }
            else if (alphabet == Alphabets.RNA)
            {
                return "RNA";
            }
            else if (alphabet == Alphabets.Protein)
            {
                return "Protein";
            }
            else
            {
                return null;
            }
        }

        private void WriteHeaderLine(TextWriter writer, string key, params string[] dataFields)
        {
            string headerLine = HeaderMark + key;

            foreach (string field in dataFields)
            {
                headerLine += " " + field;
            }

            writer.WriteLine(headerLine);
        }

        // Skips the sequence if it has no features, and skips any features that don't
        // have all the mandatory fields.
        private void WriteFeatures(ISequence sequence, TextWriter writer)
        {
            if (sequence.Metadata.ContainsKey(FeaturesKey))
            {
                foreach (MetadataListItem<List<string>> feature in
                    sequence.Metadata[FeaturesKey] as List<MetadataListItem<List<string>>>)
                {
                    // only write the line if we have all the mandatory fields
                    if (feature.SubItems.ContainsKey(SourceKey) &&
                        feature.SubItems.ContainsKey(StartKey) &&
                        feature.SubItems.ContainsKey(EndKey))
                    {
                        StringBuilder featureLine = new StringBuilder();
                        featureLine.Append(sequence.ID);
                        featureLine.Append("\t");
                        featureLine.Append(GetSubItemString(feature, SourceKey));
                        featureLine.Append("\t");
                        featureLine.Append(feature.Key);
                        featureLine.Append("\t");
                        featureLine.Append(GetSubItemString(feature, StartKey));
                        featureLine.Append("\t");
                        featureLine.Append(GetSubItemString(feature, EndKey));
                        featureLine.Append("\t");
                        featureLine.Append(GetSubItemString(feature, ScoreKey));
                        featureLine.Append("\t");
                        featureLine.Append(GetSubItemString(feature, StrandKey));
                        featureLine.Append("\t");
                        featureLine.Append(GetSubItemString(feature, FrameKey));

                        // optional attributes field is stored as free text
                        if (feature.FreeText != string.Empty)
                        {
                            featureLine.Append("\t");
                            featureLine.Append(feature.FreeText);
                        }

                        writer.WriteLine(featureLine.ToString());
                    }
                }
            }
        }

        // Returns a tab plus the sub-item text or a "." if the sub-item is absent.
        private string GetSubItemString(MetadataListItem<List<string>> feature, string subItemName)
        {
            List<string> list = null;

            if (feature.SubItems.TryGetValue(subItemName, out list))
            {
                return list[0];
            }

            return ".";
        }

        #endregion
    }
}
