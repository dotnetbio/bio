using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Bio.Extensions;
using Bio.Properties;

namespace Bio.IO.Gff
{
    /// <summary>
    ///     Writes an ISequence to a particular location, usually a file. The output is formatted
    ///     according to the GFF file format. A method is also provided for quickly accessing
    ///     the content in string form for applications that do not need to first write to file.
    /// </summary>
    public class GffFormatter : ISequenceFormatter
    {
        #region Constants
        const string HeaderMark = "##";
        const string SourceVersionKey = "SOURCE-VERSION";
        const string SourceVersionLowercaseKey = "source-version";
        const string SourceKey = "source";
        const string VersionKey = "version";
        const string TypeKey = "TYPE";
        const string TypeLowercaseKey = "type";
        const string MultiTypeKey = "TYPE_";
        const string MultiSeqDataKey = "SEQDATA_";
        const string MultiSeqRegKey = "SEQUENCE-REGION_";
        const string CommentSectionKey = "COMMENTSECTION_";
        const string GffVersionLowercaseKey = "gff-version";
        const string GffVersionKey = "GFF-VERSION";
        const string DateKey = "DATE";
        const string DateLowercaseKey = "date";
        const string SeqRegKey = "sequence-region";
        const string StartKey = "start";
        const string EndKey = "end";
        const string ScoreKey = "score";
        const string StrandKey = "strand";
        const string FrameKey = "frame";
        const string FeaturesKey = "features";

        // the spec 32k chars per line, but it shows only 37 sequence symbols per line
        // in the examples
        const long MaxSequenceSymbolsPerLine = 37;
        #endregion

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public GffFormatter()
        {
            this.ShouldWriteSequenceData = true;
        }

        /// <summary>
        ///     Whether or not sequence data will be written as part of the GFF header information;
        ///     This property is required as GFF files normally do not contain sequence data.
        ///     Defaults value is true.
        /// </summary>
        public bool ShouldWriteSequenceData { get; set; }

        /// <summary>
        ///     Gets the type of Formatter i.e GFF.
        ///     This is intended to give developers some information
        ///     of the formatter class.
        /// </summary>
        public string Name
        {
            get
            {
                return Resource.GFF_NAME;
            }
        }

        /// <summary>
        ///     Gets the description of GFF formatter.
        ///     This is intended to give developers some information
        ///     of the formatter class. This property returns a simple description of what the
        ///     GffFormatter class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Resource.GFFFORMATTER_DESCRIPTION;
            }
        }

        /// <summary>
        ///     Writes an ISequence to the specified file.
        /// </summary>
        /// <param name="stream">Writer</param>
        /// <param name="data">The sequence to format.</param>
        public void Format(Stream stream, ISequence data)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            using (StreamWriter writer = stream.OpenWrite())
            {
                this.Format(data, writer);
            }
        }

        /// <summary>
        ///     Write a collection of ISequences to a file.
        /// </summary>
        /// <remarks>
        ///     This method is overridden to format file-scope metadata that applies to all
        ///     metadata that applies to all of the sequences in the file.
        /// </remarks>
        /// <param name="stream">Writer</param>
        /// <param name="sequences">The sequences to write</param>
        public void Format(Stream stream, IEnumerable<ISequence> sequences)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (sequences == null)
            {
                throw new ArgumentNullException("sequences");
            }

            // Try to cast for performance, otherwise create a new list; we enumerate
            // this many times so we need to make sure it's all available in memory.
            IList<ISequence> data = sequences as IList<ISequence> ?? sequences.ToList();

            using (StreamWriter writer = stream.OpenWrite())
            {
                this.WriteHeaders(data, writer);
                foreach (ISequence sequence in data)
                {
                    this.WriteFeatures(sequence, writer);
                }

                writer.Flush();
            }
        }

        /// <summary>
        ///     Gives the Supported types.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Resource.GFF_FILEEXTENSION;
            }
        }

        /// <summary>
        ///     Writes an ISequence to a GenBank file in the location specified by the writer.
        /// </summary>
        /// <param name="sequence">The sequence to format.</param>
        /// <param name="writer">The TextWriter used to write the formatted sequence text.</param>
        private void Format(ISequence sequence, TextWriter writer)
        {
            this.WriteHeaders(new List<ISequence> { sequence }, writer);
            this.WriteFeatures(sequence, writer);

            writer.Flush();
        }

        /// <summary>
        ///     The headers for all sequences go at the top of the file before any features.
        /// </summary>
        /// <param name="sequenceList"></param>
        /// <param name="writer"></param>
        private void WriteHeaders(ICollection<ISequence> sequenceList, TextWriter writer)
        {
            // look for file-scope data that is common to all sequences; null signifies no match
            string source = null;
            string version = null;
            string type = null;
            bool firstSeq = true;
            ISequence commonSeq = null;
            var typeExceptionList = new List<string>();
            var seqDataExceptionList = new List<string>();
            var seqRegExceptionList = new List<string>();

            foreach (ISequence sequence in sequenceList)
            {
                MetadataListItem<string> sourceVersion;
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
                    type = this.GetGenericTypeString(sequence.Alphabet);

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
                                sourceAndVersionMatchOthers = source == sourceVersion.SubItems[SourceKey]
                                                              && version == sourceVersion.SubItems[VersionKey];
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
                    if (type != null && type != this.GetGenericTypeString(sequence.Alphabet))
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

            this.WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 1);

            int totalTypeCount = commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(MultiTypeKey));
            int currentTypeCount = 0;
            int totalSeqData = commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(MultiSeqDataKey));
            int totalSeqRegs = commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(MultiSeqRegKey));

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

                ISequence seq = null;
                switch (keyToCompare)
                {
                    case CommentSectionKey:
                        writer.WriteLine(value);
                        break;

                    case GffVersionKey:
                        // formatting using gff version 2
                        this.WriteHeaderLine(writer, GffVersionLowercaseKey, "2");
                        this.WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 2);
                        break;

                    case SourceVersionKey:

                        // only output source if they all match
                        if (source != null)
                        {
                            this.WriteHeaderLine(writer, SourceVersionLowercaseKey, source, version);
                        }

                        this.WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 3);
                        break;

                    case DateKey:
                        // today's date
                        this.WriteHeaderLine(writer, DateLowercaseKey, DateTime.Today.ToString("yyyy-MM-dd"));
                        this.WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 4);
                        break;
                    case TypeKey:
                        // type header
                        if (type != null)
                        {
                            // output that the types all match; don't need to output if DNA, as DNA is default
                            if (type != "DNA")
                            {
                                this.WriteHeaderLine(writer, TypeLowercaseKey, type);
                            }
                        }
                        else if (totalTypeCount == 0)
                        {
                            foreach (ISequence sequence in sequenceList)
                            {
                                type = this.GetGenericTypeString(sequence.Alphabet);

                                // only output seq-specific type header if this seq won't have its type
                                // output as part of a sequence data header; don't need to output if DNA,
                                // as DNA is default
                                if (type != "DNA" && (!this.ShouldWriteSequenceData || sequence.Count == 0))
                                {
                                    this.WriteHeaderLine(writer, TypeLowercaseKey, type, sequence.ID);
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
                                    this.WriteHeaderLine(writer, TypeLowercaseKey, seq.Alphabet.Name.ToUpper(), seq.ID);
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

                                        type = this.GetGenericTypeString(sequence.Alphabet);

                                        // only output seq-specific type header if this seq won't have its type
                                        // output as part of a sequence data header; don't need to output if DNA,
                                        // as DNA is default
                                        if (type != "DNA" && (!this.ShouldWriteSequenceData || sequence.Count == 0))
                                        {
                                            this.WriteHeaderLine(writer, TypeLowercaseKey, type, sequence.ID);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // output that the types all match; don't need to output if DNA, as DNA is default
                                if (type != "DNA")
                                {
                                    this.WriteHeaderLine(writer, TypeLowercaseKey, type);
                                }

                                totalTypeCount = 0;
                            }
                        }
                        break;

                    case MultiSeqDataKey:
                        // sequence data
                        if (this.ShouldWriteSequenceData)
                        {
                            seq = sequenceList.FirstOrDefault(S => S.ID.Equals(value));
                            if (seq != null)
                            {
                                this.WriteSeqData(seq, type, writer);
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

                                    this.WriteSeqData(sequence, type, writer);
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
                                this.WriteHeaderLine(
                                    writer,
                                    SeqRegKey,
                                    seq.ID,
                                    seq.Metadata[StartKey] as string,
                                    seq.Metadata[EndKey] as string);
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
                                    this.WriteHeaderLine(
                                        writer,
                                        SeqRegKey,
                                        sequence.ID,
                                        sequence.Metadata[StartKey] as string,
                                        sequence.Metadata[EndKey] as string);
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        ///     Writes the sequence to the specified writer.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="type"></param>
        /// <param name="writer"></param>
        private void WriteSeqData(ISequence sequence, string type, TextWriter writer)
        {
            if (sequence.Count > 0)
            {
                byte[] TempSeqData = null;
                type = this.GetGenericTypeString(sequence.Alphabet);

                this.WriteHeaderLine(writer, type, sequence.ID);

                for (long lineStart = 0; lineStart < sequence.Count; lineStart += MaxSequenceSymbolsPerLine)
                {
                    long length = Math.Min(MaxSequenceSymbolsPerLine, sequence.Count - lineStart);
                    ISequence subSequence = sequence.GetSubSequence(lineStart, length);
                    TempSeqData = new byte[length];
                    for (int i = 0; i < length; i++)
                    {
                        TempSeqData[i] = subSequence[i];
                    }
                    string key = Encoding.UTF8.GetString(TempSeqData, 0, TempSeqData.Length);

                    this.WriteHeaderLine(writer, key);
                }

                this.WriteHeaderLine(writer, "end-" + type);
            }
        }

        /// <summary>
        ///     Writes common metadata.
        /// </summary>
        /// <param name="commonSeq"></param>
        /// <param name="sequenceList"></param>
        /// <param name="writer"></param>
        /// <param name="source"></param>
        /// <param name="version"></param>
        /// <param name="type"></param>
        /// <param name="startFrom"></param>
        private void WriteCommonMetadata(
            ISequence commonSeq,
            IEnumerable<ISequence> sequenceList,
            TextWriter writer,
            string source,
            string version,
            string type,
            int startFrom)
        {
            int totalTypeCount = commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(MultiTypeKey));

            if (startFrom == 1)
            {
                if (commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(GffVersionKey)) == 0)
                {
                    // formatting using gff version 2
                    this.WriteHeaderLine(writer, GffVersionLowercaseKey, "2");

                    this.WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 2);
                }
            }

            if (startFrom == 2)
            {
                if (source != null
                    && commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(SourceVersionKey)) == 0)
                {
                    // only output source if they all match
                    this.WriteHeaderLine(writer, SourceVersionLowercaseKey, source, version);
                }

                this.WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 3);
            }

            if (startFrom == 3)
            {
                if (commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(DateKey)) == 0)
                {
                    // today's date
                    this.WriteHeaderLine(
                        writer,
                        DateLowercaseKey,
                        DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

                    this.WriteCommonMetadata(commonSeq, sequenceList, writer, source, version, type, 4);
                }
            }

            if (startFrom == 4)
            {
                if (totalTypeCount == 0
                    && commonSeq.Metadata.Keys.Count(K => K.ToUpperInvariant().Contains(TypeKey)) == 0)
                {
                    if (type == null)
                    {
                        foreach (ISequence sequence in sequenceList)
                        {
                            type = this.GetGenericTypeString(sequence.Alphabet);

                            // only output seq-specific type header if this seq won't have its type
                            // output as part of a sequence data header; don't need to output if DNA,
                            // as DNA is default
                            if (type != "DNA" && (!this.ShouldWriteSequenceData || sequence.Count == 0))
                            {
                                this.WriteHeaderLine(writer, TypeLowercaseKey, type, sequence.ID);
                            }
                        }
                    }
                    else
                    {
                        // output that the types all match; don't need to output if DNA, as DNA is default
                        if (type != "DNA")
                        {
                            this.WriteHeaderLine(writer, TypeLowercaseKey, type);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Returns "DNA", "RNA", "Protein", or null.
        /// </summary>
        /// <param name="alphabet"></param>
        /// <returns></returns>
        private string GetGenericTypeString(IAlphabet alphabet)
        {
            if (alphabet == Alphabets.DNA)
            {
                return "DNA";
            }
            if (alphabet == Alphabets.RNA)
            {
                return "RNA";
            }
            if (alphabet == Alphabets.Protein)
            {
                return "Protein";
            }
            return null;
        }

        private void WriteHeaderLine(TextWriter writer, string key, params string[] dataFields)
        {
            string headerLine = dataFields.Aggregate(HeaderMark + key, (current, field) => current + (" " + field));
            writer.WriteLine(headerLine);
        }

        /// <summary>
        ///     Skips the sequence if it has no features, and skips any features that don't
        ///     have all the mandatory fields.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="writer"></param>
        private void WriteFeatures(ISequence sequence, TextWriter writer)
        {
            if (sequence.Metadata.ContainsKey(FeaturesKey))
            {
                foreach (var feature in
                    sequence.Metadata[FeaturesKey] as List<MetadataListItem<List<string>>>)
                {
                    // only write the line if we have all the mandatory fields
                    if (feature.SubItems.ContainsKey(SourceKey) && feature.SubItems.ContainsKey(StartKey)
                        && feature.SubItems.ContainsKey(EndKey))
                    {
                        var featureLine = new StringBuilder();
                        featureLine.Append(sequence.ID);
                        featureLine.Append("\t");
                        featureLine.Append(this.GetSubItemString(feature, SourceKey));
                        featureLine.Append("\t");
                        featureLine.Append(feature.Key);
                        featureLine.Append("\t");
                        featureLine.Append(this.GetSubItemString(feature, StartKey));
                        featureLine.Append("\t");
                        featureLine.Append(this.GetSubItemString(feature, EndKey));
                        featureLine.Append("\t");
                        featureLine.Append(this.GetSubItemString(feature, ScoreKey));
                        featureLine.Append("\t");
                        featureLine.Append(this.GetSubItemString(feature, StrandKey));
                        featureLine.Append("\t");
                        featureLine.Append(this.GetSubItemString(feature, FrameKey));

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

        /// <summary>
        ///     Returns a tab plus the sub-item text or a "." if the sub-item is absent.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="subItemName"></param>
        /// <returns></returns>
        private string GetSubItemString(MetadataListItem<List<string>> feature, string subItemName)
        {
            List<string> list;
            if (feature.SubItems.TryGetValue(subItemName, out list))
            {
                if (list.Count >= 1)
                {
                    return list[0];
                }
            }

            return ".";
        }
    }
}