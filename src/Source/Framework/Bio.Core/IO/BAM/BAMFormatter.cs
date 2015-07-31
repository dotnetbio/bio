using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

using Bio.Algorithms.Alignment;
using Bio.IO.SAM;
using Bio.Util;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Writes a SequenceAlignmentMap to a particular location, usually a file. 
    /// The output is formatted according to the BAM file format. 
    /// Documentation for the latest BAM file format can be found at
    /// http://samtools.sourceforge.net/SAM1.pdf
    /// </summary>
    public class BAMFormatter : ISequenceAlignmentFormatter
    {
        /// <summary>
        /// Maximum Block size used while compressing the BAM file.
        /// 64K = 65536 bytes.
        /// </summary>
        private const int MaxBlockSize = 65536; //64K

        /// <summary>
        /// Comma Delimiter.
        /// </summary>
        private static readonly char[] DelimComma = { ',' };

        // list of reference sequence ranges.
        private IList<SequenceRange> refSequences;

        /// <summary>
        /// Gets the name of the sequence alignment formatter being
        /// implemented. This is intended to give the developer some
        /// information of the formatter type.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.BAM_NAME;
            }
        }

        /// <summary>
        /// Gets the description of the sequence alignment formatter being
        /// implemented. This is intended to give the developer some 
        /// information of the formatter.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.BAMFORMATTER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets the file extensions that the formatter implementation
        /// will support.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Properties.Resource.BAM_FILEEXTENSION;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating that whether the output file to be sorted or not.
        /// </summary>
        public bool CreateSortedBAMFile { get; set; }

        /// <summary>
        /// Gets or sets the value indicating that whether to create index file or not.
        /// </summary>
        public bool CreateIndexFile { get; set; }

        /// <summary>
        /// Gets or sets type of sort needed.
        /// </summary>
        public BAMSortByFields SortType { get; set; }

        /// <summary>
        /// Writes specified alignment object to a stream.
        /// The output is formatted according to the BAM specification.
        /// </summary>
        /// <param name="writer">Stream to write BAM data.</param>
        /// <param name="indexWriter">BAMIndexFile to write index data.</param>
        /// <param name="sequenceAlignment">SequenceAlignmentMap object.</param>
        public void Format(Stream writer, BAMIndexStorage indexWriter, ISequenceAlignment sequenceAlignment)
        {
            if (sequenceAlignment == null)
            {
                throw new ArgumentNullException("sequenceAlignment");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (indexWriter == null)
            {
                throw new ArgumentNullException("indexWriter");
            }

            WriteSequenceAlignment(sequenceAlignment, writer, indexWriter);
        }

        /// <summary>
        /// Writes specified alignment object to a stream.
        /// The output is formatted according to the BAM specification.
        /// Note that this method does not create index file.
        /// </summary>
        /// <param name="sequenceAlignment">SequenceAlignmentMap object.</param>
        /// <param name="writer">Stream to write BAM data.</param>
        public void Format(Stream writer, ISequenceAlignment sequenceAlignment)
        {
            if (sequenceAlignment == null)
            {
                throw new ArgumentNullException("sequenceAlignment");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            WriteSequenceAlignment(sequenceAlignment, writer, null);
        }

        /// <summary>
        /// Write a collection of ISequenceAlignments to a file.
        /// </summary>
        /// <param name="stream">The name of the file to write the formatted sequence alignments.</param>
        /// <param name="sequenceAlignments">The sequenceAlignments to write.</param>
        public void Format(Stream stream, IEnumerable<ISequenceAlignment> sequenceAlignments)
        {
           throw new NotSupportedException(Properties.Resource.BAM_FormatMultipleAlignmentsNotSupported);
        }

        /// <summary>
        /// Compress the specified stream (reader) and writes to the specified stream (writer).
        /// </summary>
        /// <param name="reader">Stream to read from.</param>
        /// <param name="writer">Stream to write.</param>
        public void CompressBAMFile(Stream reader, Stream writer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            byte[] array = new byte[MaxBlockSize];

            int bytesRead = -1;
            //note that the reads can be split over a block per mailing list discussion
            bytesRead = reader.Read(array, 0, MaxBlockSize);
            while (bytesRead != 0)
            {
                byte[] bgzfArray;
                MemoryStream memStream = new MemoryStream();
                try
                {
                    using (GZipStream compress = new GZipStream(memStream, CompressionMode.Compress, true))
                    {
                        compress.Write(array, 0, bytesRead);
                        compress.Flush();
                    }

                    memStream.Seek(0, SeekOrigin.Begin);
                    bgzfArray = GetBGZFStructure(memStream);
                }
                finally
                {
                    memStream.Dispose();
                }

                writer.Write(bgzfArray, 0, bgzfArray.Length);
                writer.Flush();
                bytesRead = reader.Read(array, 0, MaxBlockSize);
            }

            writer.Write(GetEOFBlock(), 0, 28);
            writer.Flush();
        }

        /// <summary>
        /// Writes BAM header to the specified stream in BAM format.
        /// </summary>
        /// <param name="header">SAMAlignmentHeader object</param>
        /// <param name="writer">Stream to write.</param>
        public void WriteHeader(SAMAlignmentHeader header, Stream writer)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            string samHeader;

            if (this.refSequences == null)
            {
                this.refSequences = header.GetReferenceSequenceRanges();
            }

            using (StringWriter strwriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                SAMFormatter.WriteHeader(strwriter, header);
                samHeader = strwriter.ToString();
            }

            int samHeaderLen = samHeader.Length;
            byte[] bytes = Encoding.UTF8.GetBytes(samHeader);
            byte[] bamMagicNumber = { 66, 65, 77, 1 };

            // write BAM magic number
            writer.Write(bamMagicNumber, 0, 4);

            // Length of the header text
            writer.Write(Helper.GetLittleEndianByteArray(samHeaderLen), 0, 4);

            //Plain header text in SAM
            writer.Write(bytes, 0, bytes.Length);
            // number of reference sequences
            writer.Write(Helper.GetLittleEndianByteArray(this.refSequences.Count), 0, 4);

            foreach (SequenceRange range in this.refSequences)
            {
                int len = range.ID.Length;

                byte[] array = Encoding.UTF8.GetBytes(range.ID);
                writer.Write(Helper.GetLittleEndianByteArray(len + 1), 0, 4);
                writer.Write(array, 0, len);
                writer.WriteByte((byte)'\0');
                writer.Write(Helper.GetLittleEndianByteArray((int)range.End), 0, 4);
            }
        }

        /// <summary>
        /// Writes SAMAlignedSequence to specified stream.
        /// </summary>
        /// <param name="header">Header from SAM object.</param>
        /// <param name="alignedSeq">SAMAlignedSequence object.</param>
        /// <param name="writer">Stream to write.</param>
        public void WriteAlignedSequence(SAMAlignmentHeader header, SAMAlignedSequence alignedSeq, Stream writer)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }

            if (alignedSeq == null)
            {
                throw new ArgumentNullException("alignedSeq");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (this.refSequences == null)
            {
                this.refSequences = header.GetReferenceSequenceRanges();
            }

            WriteAlignedSequence(alignedSeq, writer);
        }

        /// <summary>
        /// Creates BAMIndex object from the specified BAM file and writes to specified BAMIndex file.
        /// </summary>
        /// <param name="compressedBAMStream"></param>
        /// <param name="indexStorage"></param>
        private static void CreateBAMIndexFile(Stream compressedBAMStream, BAMIndexStorage indexStorage)
        {
            var parser = new BAMParser();
            BAMIndex bamIndex = parser.GetIndexFromBAMStorage(compressedBAMStream);
            indexStorage.Write(bamIndex);
        }

        // Sorts SequenceRanges on ref sequence name.
        private static IList<SequenceRange> SortSequenceRanges(IEnumerable<SequenceRange> ranges)
        {
            return ranges.OrderBy(R => R.ID).ToList();
        }

        // Validates alignment header.
        private static void ValidateAlignmentHeader(SAMAlignmentHeader header)
        {
            string message = header.IsValid();
            if (!string.IsNullOrEmpty(message))
            {
                throw new ArgumentException(message);
            }
        }

        // Gets SQ fields from the list of fields
        private static IList<SAMRecordField> GetSQHeaders(IList<SAMRecordField> recordFields)
        {
            List<SAMRecordField> sqHeaders = new List<SAMRecordField>();
            for (int i = 0; i < recordFields.Count; i++)
            {
                SAMRecordField field = recordFields[i];
                if (field.Typecode.Equals("SQ"))
                {
                    sqHeaders.Add(field);
                }
            }

            return sqHeaders;
        }

        /// <summary>
        /// Writes sequence alignment to specified stream.
        /// </summary>
        /// <param name="sequenceAlignment">Sequence alignment object</param>
        /// <param name="writer">Stream to write.</param>
        /// <param name="indexStorage">BAMIndex file.</param>
        private void WriteSequenceAlignment(ISequenceAlignment sequenceAlignment, Stream writer, BAMIndexStorage indexStorage)
        {
            // validate sequenceAlignment.
            SequenceAlignmentMap sequenceAlignmentMap = ValidateAlignment(sequenceAlignment);

            // write bam struct to temp file.
            using (var fstemp = PlatformManager.Services.CreateTempStream())
            {
                WriteUncompressed(sequenceAlignmentMap, fstemp, CreateSortedBAMFile);

                fstemp.Seek(0, SeekOrigin.Begin);

                // Compress and write to the specified stream.
                CompressBAMFile(fstemp, writer);
            }

            // if index file need to be created.
            if (indexStorage != null)
            {
                writer.Seek(0, SeekOrigin.Begin);
                CreateBAMIndexFile(writer, indexStorage);
            }
        }

        /// <summary>
        /// Writes specified sequence alignment to stream.
        /// The output is formatted according to the BAM structure.
        /// </summary>
        /// <param name="sequenceAlignmentMap">SequenceAlignmentMap object.</param>
        /// <param name="writer">Stream to write.</param>
        /// <param name="createSortedFile">If this flag is true output file will be sorted.</param>
        private void WriteUncompressed(SequenceAlignmentMap sequenceAlignmentMap, Stream writer, bool createSortedFile)
        {
            SAMAlignmentHeader header = sequenceAlignmentMap.Header;
            if (createSortedFile && SortType == BAMSortByFields.ChromosomeNameAndCoordinates)
            {
                header = GetHeaderWithSortedSQFields(header, true);
                this.refSequences = header.GetReferenceSequenceRanges();
            }

            if (this.refSequences == null)
            {
                this.refSequences = header.GetReferenceSequenceRanges();
            }

            WriteHeader(header, writer);
            writer.Flush();
            if (createSortedFile)
            {
                WriteUncompressedSortedBAM(sequenceAlignmentMap, writer);
            }
            else
            {
                foreach (SAMAlignedSequence seq in sequenceAlignmentMap.QuerySequences)
                {
                    SAMAlignedSequence alignedSeq = seq;
                    this.ValidateSQHeader(alignedSeq.RName);
                    this.WriteAlignedSequence(alignedSeq, writer);
                    writer.Flush();
                }
            }

            writer.Flush();
        }

        /// <summary>
        /// Writes specified sequence alignment to stream according to the specified sorted order.
        /// The output is formatted according to the BAM structure.
        /// </summary>
        /// <param name="sequenceAlignmentMap">SequenceAlignmentMap object.</param>
        /// <param name="writer">Stream to write.</param>
        private void WriteUncompressedSortedBAM(SequenceAlignmentMap sequenceAlignmentMap, Stream writer)
        {
            if (SortType != BAMSortByFields.ReadNames)
            {
                List<IGrouping<string, SAMAlignedSequence>> groups =
                    sequenceAlignmentMap.QuerySequences.GroupBy(Q => Q.RName).OrderBy(G => G.Key).ToList();

                foreach (SequenceRange range in this.refSequences)
                {
                    IGrouping<string, SAMAlignedSequence> group = groups.FirstOrDefault(G => G.Key.Equals(range.ID));
                    if (group == null)
                    {
                        continue;
                    }

                    // sort aligned sequence on left co-ordinate.
                    List<SAMAlignedSequence> alignedSeqs = group.OrderBy(A => A.Pos).ToList();

                    foreach (SAMAlignedSequence alignedSeq in alignedSeqs)
                    {
                        ValidateSQHeader(alignedSeq.RName);
                        WriteAlignedSequence(alignedSeq, writer);
                        writer.Flush();
                    }
                }
            }
            else
            {
                List<SAMAlignedSequence> alignedSeqs =
                        sequenceAlignmentMap.QuerySequences.OrderBy(Q => Q.QName).ToList();

                foreach (SAMAlignedSequence alignedSeq in alignedSeqs)
                {
                    ValidateSQHeader(alignedSeq.RName);
                    WriteAlignedSequence(alignedSeq, writer);
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Gets BGZF structure from the GZipStream compression.
        /// </summary>
        /// <param name="compressedStream">BAM file which is compressed according to BGZF compression.</param>
        private static byte[] GetBGZFStructure(Stream compressedStream)
        {
            byte[] compressedArray = new byte[compressedStream.Length];
            compressedStream.Read(compressedArray, 0, (int)compressedStream.Length);
            UInt16 blockSize = (UInt16)(compressedStream.Length + 8);
            byte[] bgzfArray = new byte[blockSize];

            bgzfArray[0] = 31;  // gzip IDentifier1
            bgzfArray[1] = 139; // gzip IDentifier2
            bgzfArray[2] = 8;   // gzip Compression Method 
            bgzfArray[3] = 4;   // gzip FLaGs
            bgzfArray[9] = 255; // gzip Operating System "255 - unknown"
            bgzfArray[10] = 6;  // gzip eXtra LENgth 
            bgzfArray[11] = 0;
            bgzfArray[12] = 66; // Subfield Identifier1
            bgzfArray[13] = 67; // Subfield Identifier2
            bgzfArray[14] = 2;  // Subfield LENgth
            bgzfArray[15] = 0;

            byte[] intvalue = Helper.GetLittleEndianByteArray((UInt16)(blockSize - 1));

            bgzfArray[16] = intvalue[0];
            bgzfArray[17] = intvalue[1];
            //start at 10 to skip the gzip header which was already added above
            for (int i = 10; i < compressedStream.Length; i++)
            {
                bgzfArray[i + 8] = compressedArray[i];
            }

            return bgzfArray;
        }

        /// <summary>
        /// Gets empty block.
        /// </summary>
        private static byte[] GetEOFBlock()
        {
            // EOF block size will be 28 bytes.
            // Here it is set to 20 remaining bytes will 
            // be taken care by "GetBGZFStructure" method.
            const int BlockSize = 20;

            byte[] block = new byte[BlockSize];
            // represents no data.
            // got this value by reading Empty block from BAM file.
            block[10] = 3;
            using (MemoryStream memStream = new MemoryStream(BlockSize))
            {
                memStream.Write(block, 0, BlockSize);
                memStream.Seek(0, SeekOrigin.Begin);
                return GetBGZFStructure(memStream);
            }
        }

        /// <summary>
        /// Writes SAMAlignedSequence to specified stream.
        /// </summary>
        /// <param name="alignedSeq">SAMAlignedSequence object.</param>
        /// <param name="writer">Stream to write.</param>
        private void WriteAlignedSequence(SAMAlignedSequence alignedSeq, Stream writer)
        {
            // Get the total block size required.
            int blocksize = GetBlockSize(alignedSeq);

            // Get Reference sequence index.
            int rid = GetRefSeqID(alignedSeq.RName);

            // bin<<16|mapQual<<8|read_name_len (including NULL)
            uint bin_mq_nl = (uint)alignedSeq.Bin << 16;
            bin_mq_nl = bin_mq_nl | (uint)alignedSeq.MapQ << 8;
            bin_mq_nl = bin_mq_nl | (uint)(alignedSeq.QName.Length + 1);

            // flag<<16|cigar_len
            uint flag_nc = (uint)alignedSeq.Flag << 16;
            flag_nc = flag_nc | (uint)GetCIGARLength(alignedSeq.CIGAR);

            int readLen = (int)alignedSeq.QuerySequence.Count;

            int mateRefId = GetRefSeqID(alignedSeq.MRNM);

            byte[] readName = Encoding.UTF8.GetBytes(alignedSeq.QName);

            // Cigar: op_len<<4|op. Op: MIDNSHP=X => 012345678
            IList<uint> encodedCIGAR = GetEncodedCIGAR(alignedSeq.CIGAR);

            //block size
            writer.Write(Helper.GetLittleEndianByteArray(blocksize), 0, 4);

            // Reference sequence index.
            writer.Write(Helper.GetLittleEndianByteArray(rid), 0, 4);

            // Pos
            writer.Write(Helper.GetLittleEndianByteArray(alignedSeq.Pos > 0 ? alignedSeq.Pos - 1 : -1), 0, 4);

            // bin<<16|mapQual<<8|read_name_len (including NULL)
            writer.Write(Helper.GetLittleEndianByteArray(bin_mq_nl), 0, 4);

            // flag<<16|cigar_len
            writer.Write(Helper.GetLittleEndianByteArray(flag_nc), 0, 4);

            // Length of the read
            writer.Write(Helper.GetLittleEndianByteArray(readLen), 0, 4);

            // Mate reference sequence index
            writer.Write(Helper.GetLittleEndianByteArray(mateRefId), 0, 4);

            // mate_pos - Leftmost coordinate of the mate
            // As per SAM format Mpos will be 1 based and 0 indicates unpaired or pairing information is unavailabe.
            // In case of BAM format Mpos will be zero based and -1 indicates unpaired or pairing information is unavailabe.
            writer.Write(Helper.GetLittleEndianByteArray(alignedSeq.MPos - 1), 0, 4);

            // Insert size of the read pair (if paired)
            writer.Write(Helper.GetLittleEndianByteArray(alignedSeq.ISize), 0, 4);

            // Read name, null terminated
            writer.Write(readName, 0, readName.Length);
            writer.WriteByte((byte)'\0');

            // Cigar: op_len<<4|op. Op: MIDNSHP=>0123456
            foreach (uint data in encodedCIGAR)
            {
                writer.Write(Helper.GetLittleEndianByteArray(data), 0, 4);
            }

            // 4-bit encoded read: =ACGTN=>0,1,2,4,8,15; the earlier base is stored in the high-order 4 bits of the byte.
            byte[] encodedValues = GetEncodedSequence(alignedSeq);
            writer.Write(encodedValues, 0, encodedValues.Length);

            // Phred base quality (0xFF if absent)
            encodedValues = GetQualityValue(alignedSeq.QuerySequence);
            writer.Write(encodedValues, 0, encodedValues.Length);

            // Optional fields
            foreach (SAMOptionalField field in alignedSeq.OptionalFields)
            {
                byte[] optionalArray = GetOptioanField(field);
                writer.Write(optionalArray, 0, optionalArray.Length);
            }
        }

        /// <summary>
        /// Gets encoded sequence according to the BAM specification.
        /// </summary>
        /// <param name="alignedSeq"></param>
        /// <returns></returns>
        private static byte[] GetEncodedSequence(SAMAlignedSequence alignedSeq)
        {
            List<byte> byteList = new List<byte>();
            ISequence seq = alignedSeq.QuerySequence;
            if (seq != null)
            {
                if (!(seq.Alphabet is DnaAlphabet))
                {
                    throw new ArgumentException(Properties.Resource.BAMFormatterSupportsDNAOnly);
                }

                byte[] symbolMap = seq.Alphabet.GetSymbolValueMap();

                for (int i = 0; i < seq.Count; i++)
                {
                    char symbol = (char)symbolMap[seq[i]];
                    byte encodedvalue = 0;

                  
                    // 4-bit encoded read: =ACMGRSVTWYHKDBN -> 0-15; the earlier base is stored in the 
                    // high-order 4 bits of the byte.
                    //Note:
                    // All the other symbols which are not supported by BAM specification (other than "=ACMGRSVTWYHKDBN") are converted to 'N'
                    // for example a '.' symbol which is supported by SAM specification will be converted to symbol 'N'
                    switch (symbol)
                    {
                        case '=':
                            encodedvalue = 0;
                            break;
                        case 'A':
                            encodedvalue = 1;
                            break;
                        case 'C':
                            encodedvalue = 2;
                            break;
                        case 'M':
                            encodedvalue = 3;
                            break;
                        case 'G':
                            encodedvalue = 4;
                            break;
                        case 'R':
                            encodedvalue = 5;
                            break;
                        case 'S':
                            encodedvalue = 6;
                            break;
                        case 'V':
                            encodedvalue = 7;
                            break;
                        case 'T':
                            encodedvalue = 8;
                            break;
                        case 'W':
                            encodedvalue = 9;
                            break;
                        case 'Y':
                            encodedvalue = 10;
                            break;
                        case 'H':
                            encodedvalue = 11;
                            break;
                        case 'K':
                            encodedvalue = 12;
                            break;
                        case 'D':
                            encodedvalue = 13;
                            break;
                        case 'B':
                            encodedvalue = 14;
                            break;
                        default:
                            encodedvalue = 15;
                            break;
                    }

                    if ((i + 1) % 2 > 0)
                    {
                        byteList.Add((byte)(encodedvalue << 4));
                    }
                    else
                    {
                        byteList[byteList.Count - 1] = (byte)(byteList[byteList.Count - 1] | encodedvalue);
                    }
                }
            }

            return byteList.ToArray();
        }

        /// <summary>
        /// Gets quality values from specified sequence.
        /// </summary>
        /// <param name="sequence">Sequence object.</param>
        private static byte[] GetQualityValue(ISequence sequence)
        {
            byte[] qualityValues = new byte[sequence.Count];
            QualitativeSequence qualitativeSeq = sequence as QualitativeSequence;

            for (int i = 0; i < qualityValues.Length; i++)
            {
                if (qualitativeSeq == null)
                {
                    qualityValues[i] = 0xFF;
                }
                else
                {
                    qualityValues[i] =(byte)qualitativeSeq.GetPhredQualityScore(i);
                }
            }

            return qualityValues;
        }

        /// <summary>
        /// Gets encoded CIGAR value.
        /// </summary>
        /// <param name="cigar">CIGAR</param>
        private static IList<uint> GetEncodedCIGAR(string cigar)
        {
            List<uint> encodedValues = new List<uint>();
            if (cigar.Equals("*"))
            {
                return encodedValues;
            }

            uint value;
            cigar = cigar.ToUpperInvariant();
            string intvalue = string.Empty;
            foreach (char ch in cigar)
            {
                if (Char.IsDigit(ch))
                {
                    intvalue += ch;
                }
                else
                {
                    value = uint.Parse(intvalue, CultureInfo.InvariantCulture);
                    value = value << 4;
                    value = value | GetEncodedCIGAROperation(ch);
                    intvalue = string.Empty;
                    encodedValues.Add(value);
                }
            }

            return encodedValues;
        }

        // Gets encoded CIGAR operation.
        private static uint GetEncodedCIGAROperation(char operation)
        {
            //MIDNSHP=X  -> 012345678
            switch (operation)
            {
                case 'M':
                    return 0;
                case 'I':
                    return 1;
                case 'D':
                    return 2;
                case 'N':
                    return 3;
                case 'S':
                    return 4;
                case 'H':
                    return 5;
                case 'P':
                    return 6;
                case '=':
                    return 7;
                default:
                    return 8;
            }
        }

        // Gets block size required for the specified SAMAlignedSequence object.
        private int GetBlockSize(SAMAlignedSequence alignedSeq)
        {
            int readNameLen = alignedSeq.QName.Length + 1;
            int cigarLen = GetCIGARLength(alignedSeq.CIGAR);
            int readLen = (int)alignedSeq.QuerySequence.Count;

            return 32 + readNameLen + (cigarLen * 4) + ((readLen + 1) / 2) + readLen + GetAuxiliaryDataLength(alignedSeq);
        }

        // Gets the length of the optional fields in a SAMAlignedSequence object.
        private static int GetAuxiliaryDataLength(SAMAlignedSequence alignedSeq)
        {
            int size = 0;
            foreach (SAMOptionalField field in alignedSeq.OptionalFields)
            {
                size += 3;
                int valueSize = GetOptionalFieldValueSize(field);
                if (valueSize == 0)
                {
                    string message = string.Format(CultureInfo.InvariantCulture, Properties.Resource.BAM_InvalidIntValueInOptFieldOfAlignedSeq, field.Value, field.Tag, alignedSeq.QName);
                    throw new FormatException(message);
                }

                size += valueSize < 0 ? -valueSize : valueSize;
            }

            return size;
        }

        // Gets optional field value size.
        private static int GetOptionalFieldValueSize(SAMOptionalField optionalField)
        {
            switch (optionalField.VType)
            {
                case "A":  //  Printable character
                case "c": //signed 8-bit integer
                    return -1;
                case "C": //unsigned 8-bit integer
                    return 1;
                case "s": // signed 16 bit integer
                case "S"://unsinged 16 bit integer
                case "i": // signed 32 bit integer
                case "I": // unsigned 32 bit integer
                    return GetOptionalFieldIntValueSize(optionalField.Value);
                case "f": // float
                    return 4;
                case "Z": // printable string 
                case "H": // HexString
                    return optionalField.Value.Length + 1;
                case "B"://integer or numeric array
                    char type = optionalField.Value[0];
                    int arrayTypeSize = GetSizeOfArrayType(type);
                    int numberofelements = optionalField.Value.Split(DelimComma, StringSplitOptions.RemoveEmptyEntries).Length - 1;
                    int elementsSize = arrayTypeSize * numberofelements;
                    int arraylen = elementsSize + 1 + 4;  // 1 to store array type and 4 to store number of values in array.
                    return arraylen;
                default:
                    throw new Exception(Properties.Resource.BAM_InvalidOptValType);
            }
        }

        private static int GetSizeOfArrayType(char arrayType)
        {
            switch (arrayType)
            {
                case 'A':  //  Printable character
                case 'c': //signed 8-bit integer
                case 'C': //unsigned 8-bit integer
                    return 1;
                case 's': // signed 16 bit integer
                case 'S'://unsinged 16 bit integer
                    return 2;
                case 'i': // signed 32 bit integer
                case 'I': // unsigned 32 bit integer
                case 'f': // float
                    return 4;
                default:
                    throw new Exception(string.Format(Properties.Resource.BAM_InvalidOptValType, arrayType));
            }
        }



        /// <summary>
        /// Returns,
        ///  1 if the int value can be held in an unsinged byte.
        /// -1 if the int value can be held in a singed byte.
        ///  2 if the int value can be held in an unint16 (ushort).
        /// -2 if the int value can be held in an int16 (short).
        ///  4 if the int value can be held in an uint32.
        /// -4 if the int value can be held in an int32.
        ///  0 if the specified value can't parsed by an uint.
        /// </summary>
        /// <param name="value">String representing int value.</param>
        private static int GetOptionalFieldIntValueSize(string value)
        {
            uint positiveValue;
            int negativeValue;
            if (!uint.TryParse(value, out positiveValue))
            {
                if (!int.TryParse(value, out negativeValue))
                {
                    return 0;
                }

                if (sbyte.MinValue <= negativeValue)
                {
                    return -1;
                }

                if (short.MinValue <= negativeValue)
                {
                    return -2;
                }

                return -4;
            }

            if (byte.MaxValue >= positiveValue)
            {
                return 1;
            }

            if (ushort.MaxValue >= positiveValue)
            {
                return 2;
            }

            return 4;
        }

        // Gets optional field in a byte array.
        private static byte[] GetOptioanField(SAMOptionalField field)
        {
            int valueSize = GetOptionalFieldValueSize(field);
            if (valueSize == 0)
            {
                string message = string.Format(CultureInfo.InvariantCulture, Properties.Resource.BAM_InvalidIntValueInOptField, field.Value, field.Tag);
                throw new FormatException(message);
            }

            int arrayLen = valueSize < 0 ? -valueSize : valueSize;
            arrayLen += 3;
            byte[] array = new byte[arrayLen];
            array[0] = (byte)field.Tag[0];
            array[1] = (byte)field.Tag[1];
            array[2] = (byte)field.VType[0];
            byte[] temparray = new byte[4];

            switch (field.VType)
            {
                case "A":  //  Printable character
                    array[3] = (byte)field.Value[0];
                    break;
                case "c": //signed 8-bit integer
                case "C": //unsigned 8-bit integer
                case "s": // signed 16 bit integer
                case "S"://unsinged 16 bit integer
                case "i": // signed 32 bit integer
                case "I": // unsigned 32 bit integer
                    if (valueSize == 1)
                    {
                        array[2] = (byte)'C';
                        array[3] = byte.Parse(field.Value, CultureInfo.InvariantCulture);
                    }
                    else if (valueSize == -1)
                    {
                        sbyte sb = sbyte.Parse(field.Value, CultureInfo.InvariantCulture);
                        array[2] = (byte)'c';
                        array[3] = (byte)sb;
                    }
                    else if (valueSize == 2)
                    {
                        UInt16 uint16value = UInt16.Parse(field.Value, CultureInfo.InvariantCulture);
                        temparray = Helper.GetLittleEndianByteArray(uint16value);
                        array[2] = (byte)'S';
                        array[3] = temparray[0];
                        array[4] = temparray[1];
                    }
                    else if (valueSize == -2)
                    {
                        Int16 int16value = Int16.Parse(field.Value, CultureInfo.InvariantCulture);
                        temparray = Helper.GetLittleEndianByteArray(int16value);
                        array[2] = (byte)'s';
                        array[3] = temparray[0];
                        array[4] = temparray[1];
                    }
                    else if (valueSize == 4)
                    {
                        uint uint32value = uint.Parse(field.Value, CultureInfo.InvariantCulture);
                        temparray = Helper.GetLittleEndianByteArray(uint32value);
                        array[2] = (byte)'I';
                        array[3] = temparray[0];
                        array[4] = temparray[1];
                        array[5] = temparray[2];
                        array[6] = temparray[3];
                    }
                    else
                    {
                        int int32value = int.Parse(field.Value, CultureInfo.InvariantCulture);
                        temparray = Helper.GetLittleEndianByteArray(int32value);
                        array[2] = (byte)'i';
                        array[3] = temparray[0];
                        array[4] = temparray[1];
                        array[5] = temparray[2];
                        array[6] = temparray[3];
                    }

                    break;
                case "f": // float
                    float floatvalue = float.Parse(field.Value, CultureInfo.InvariantCulture);
                    temparray = Helper.GetLittleEndianByteArray(floatvalue);
                    array[3] = temparray[0];
                    array[4] = temparray[1];
                    array[5] = temparray[2];
                    array[6] = temparray[3];
                    break;

                case "Z": // printable string 
                    temparray = Encoding.UTF8.GetBytes(field.Value);
                    temparray.CopyTo(array, 3);
                    array[3 + temparray.Length] = (byte)'\0';
                    break;
                case "H": // HexString
                    temparray = Encoding.UTF8.GetBytes(field.Value);
                    temparray.CopyTo(array, 3);
                    array[3 + temparray.Length] = (byte)'\0';
                    break;
                case "B": // integer or numeric array.
                    UpdateArrayType(array, field);
                    break;
                default:
                    throw new Exception(Properties.Resource.BAM_InvalidOptValType);
            }

            return array;
        }

        private static void UpdateArrayType(byte[] array, SAMOptionalField field)
        {
            byte[] temparray = new byte[4];
            char arraytype = field.Value[0];
            int arrayTypeSize = GetSizeOfArrayType(arraytype);
            string[] elements = field.Value.Split(DelimComma, StringSplitOptions.RemoveEmptyEntries);
            array[3] = (byte)arraytype;
            int arrayIndex = 4;

            temparray = Helper.GetLittleEndianByteArray(elements.Length - 1);
            array[arrayIndex++] = temparray[0];
            array[arrayIndex++] = temparray[1];
            array[arrayIndex++] = temparray[2];
            array[arrayIndex++] = temparray[3];


            //elemetns[0] contains array type;
            for (int i = 1; i < elements.Length; i++)
            {
                switch (arraytype)
                {
                    case 'A':  //  Printable character
                        temparray[0] = (byte)elements[i][0];
                        break;
                    case 'c': //signed 8-bit integer
                        temparray[0] = (byte)sbyte.Parse(elements[i], CultureInfo.InvariantCulture);
                        break;
                    case 'C': //unsigned 8-bit integer
                        temparray[0] = byte.Parse(elements[i], CultureInfo.InvariantCulture);
                        break;
                    case 's': // signed 16 bit integer
                        Int16 int16value = Int16.Parse(elements[i], CultureInfo.InvariantCulture);
                        temparray = Helper.GetLittleEndianByteArray(int16value);
                        break;
                    case 'S'://unsinged 16 bit integer
                        UInt16 uint16value = UInt16.Parse(elements[i], CultureInfo.InvariantCulture);
                        temparray = Helper.GetLittleEndianByteArray(uint16value);
                        break;
                    case 'i': // signed 32 bit integer
                        int int32value = int.Parse(elements[i], CultureInfo.InvariantCulture);
                        temparray = Helper.GetLittleEndianByteArray(int32value);
                        break;
                    case 'I': // unsigned 32 bit integer
                        uint uint32value = uint.Parse(elements[i], CultureInfo.InvariantCulture);
                        temparray = Helper.GetLittleEndianByteArray(uint32value);
                        break;
                    case 'f': // float
                        float floatvalue = float.Parse(elements[i], CultureInfo.InvariantCulture);
                        temparray = Helper.GetLittleEndianByteArray(floatvalue);
                        break;
                    default:
                        throw new Exception(string.Format(Properties.Resource.BAM_InvalidOptValType, arraytype));

                }

                for (int tempIndex = 0; tempIndex < arrayTypeSize; tempIndex++)
                {
                    array[arrayIndex++] = temparray[tempIndex];
                }
            }
        }


        // Gets CIGAR length.
        private static int GetCIGARLength(string cigar)
        {
            return cigar.Equals("*") ? 0 : cigar.ToCharArray().Count(ch => !char.IsDigit(ch));
        }

        // Gets ref seq index.
        private int GetRefSeqID(string refSeqName)
        {
            SequenceRange range = this.refSequences.FirstOrDefault(SR => string.Compare(SR.ID, refSeqName, StringComparison.OrdinalIgnoreCase) == 0);
            if (range == null)
            {
                return -1;
            }

            return this.refSequences.IndexOf(range);
        }

        // Validates the alignment.
        private SequenceAlignmentMap ValidateAlignment(ISequenceAlignment sequenceAlignment)
        {
            SequenceAlignmentMap seqAlignmentMap = sequenceAlignment as SequenceAlignmentMap;
            if (seqAlignmentMap != null)
            {
                ValidateAlignmentHeader(seqAlignmentMap.Header);
                if (CreateSortedBAMFile && SortType == BAMSortByFields.ChromosomeNameAndCoordinates)
                {
                    this.refSequences = SortSequenceRanges(seqAlignmentMap.Header.GetReferenceSequenceRanges());
                }
                else
                {
                    this.refSequences = seqAlignmentMap.Header.GetReferenceSequenceRanges();
                }

                return seqAlignmentMap;
            }

            SAMAlignmentHeader header = sequenceAlignment.Metadata[Helper.SAMAlignmentHeaderKey] as SAMAlignmentHeader;
            if (header == null)
            {
                throw new ArgumentException(Properties.Resource.SAMAlignmentHeaderNotFound);
            }

            ValidateAlignmentHeader(header);

            seqAlignmentMap = new SequenceAlignmentMap(header);
            if (CreateSortedBAMFile && SortType == BAMSortByFields.ChromosomeNameAndCoordinates)
            {
                this.refSequences = SortSequenceRanges(seqAlignmentMap.Header.GetReferenceSequenceRanges());
            }
            else
            {
                this.refSequences = seqAlignmentMap.Header.GetReferenceSequenceRanges();
            }

            foreach (IAlignedSequence alignedSeq in sequenceAlignment.AlignedSequences)
            {
                SAMAlignedSequenceHeader alignedHeader = alignedSeq.Metadata[Helper.SAMAlignedSequenceHeaderKey] as SAMAlignedSequenceHeader;
                if (alignedHeader == null)
                {
                    throw new ArgumentException(Properties.Resource.SAMAlignedSequenceHeaderNotFound);
                }

                SAMAlignedSequence samAlignedSeq = new SAMAlignedSequence(alignedHeader);
                samAlignedSeq.QuerySequence = alignedSeq.Sequences[0];
                seqAlignmentMap.QuerySequences.Add(samAlignedSeq);
            }

            return seqAlignmentMap;
        }

        // Validates whether all ref seq names in aligned sequences are present in the SAMAlignmentHeader or not.
        private void ValidateSQHeader(string refSeqName)
        {
            if (refSeqName != Properties.Resource.SAM_NO_REFERENCE_DEFINED_INDICATOR)// the '*' to indicate the read is unmapped
            {
                string message;
                List<SequenceRange> rages = this.refSequences.Where(SR => string.Compare(SR.ID, refSeqName, StringComparison.OrdinalIgnoreCase) == 0).ToList();

                if (rages.Count == 0)
                {
                    message = string.Format(CultureInfo.InvariantCulture, Properties.Resource.SQHeaderMissing, refSeqName, CultureInfo.CurrentCulture);
                    throw new ArgumentException(message);
                }

                if (rages.Count > 1)
                {
                    message = string.Format(CultureInfo.InvariantCulture, Properties.Resource.DuplicateSQHeader, refSeqName, CultureInfo.CurrentCulture);

                    throw new ArgumentException(message);
                }
            }
        }

        // returns true if the list is sorted by chromosome name else returns false.
        private bool IsSortedByChromosomeNames(IList<SAMRecordField> sqHeaders)
        {
            for (int i = 1; i < sqHeaders.Count; i++)
            {
                if (CompareByChromosomeName(sqHeaders[i - 1], sqHeaders[i]) > 0)
                {
                    return false;
                }
            }

            return true;
        }

        // Gets new header with sorted SQ Fields.
        // If SQ fields are already sorted then returns the same header.
        private SAMAlignmentHeader GetHeaderWithSortedSQFields(SAMAlignmentHeader header, bool canChangeOtherTagPos)
        {
            if (IsSortedByChromosomeNames(GetSQHeaders(header.RecordFields)))
                return header;

            SAMAlignmentHeader newHeader = new SAMAlignmentHeader();
            int i = 0;
            if (canChangeOtherTagPos)
            {
                List<SAMRecordField> sqHeaders = new List<SAMRecordField>();
                for (; i < header.RecordFields.Count; i++)
                {
                    SAMRecordField field = header.RecordFields[i];
                    if (field.Typecode.Equals("SQ"))
                    {
                        sqHeaders.Add(field);
                    }
                    else
                    {
                        newHeader.RecordFields.Add(field);
                    }

                    sqHeaders.Sort(CompareByChromosomeName);

                    foreach (SAMRecordField sqfield in sqHeaders)
                    {
                        newHeader.RecordFields.Add(sqfield);
                    }

                    foreach (string str in header.Comments)
                    {
                        newHeader.Comments.Add(str);
                    }
                }
            }
            else
            {
                SortedList<SAMRecordField, int> map = new SortedList<SAMRecordField, int>(new ComparisonWrapper<SAMRecordField>(CompareByChromosomeName));

                for (; i < header.RecordFields.Count; i++)
                {
                    SAMRecordField field = header.RecordFields[i];
                    if (field.Typecode.Equals("SQ"))
                    {
                        map.Add(field, i);
                    }

                    newHeader.RecordFields.Add(field);
                }

                i = 0;
                foreach (int index in map.Values.OrderBy(I => I))
                {
                    newHeader.RecordFields[index] = map.Keys[i++];
                }

                foreach (string str in header.Comments)
                {
                    newHeader.Comments.Add(str);
                }
            }

            return newHeader;
        }

        // compares chromosome name in the specified fields.
        private int CompareByChromosomeName(SAMRecordField field1, SAMRecordField field2)
        {
            string chr1 = field1.Tags.FirstOrDefault(Tag => Tag.Tag.Equals("SN")).Value;
            string chr2 = field2.Tags.FirstOrDefault(Tag => Tag.Tag.Equals("SN")).Value;
            return string.Compare(chr1, chr2, StringComparison.Ordinal);
        }
    }
}
