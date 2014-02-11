using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bio.Algorithms.Alignment;
using Bio.IO.SAM;
using Bio.Util;

namespace Bio.IO.BAM
{
    /// <summary>
    /// A BAMParser reads from a source of binary data that is formatted according to the BAM
    /// file specification, and converts the data to in-memory SequenceAlignmentMap object.
    /// Documentation for the latest BAM file format can be found at
    /// http://samtools.sourceforge.net/SAM1.pdf
    /// </summary>
public partial class BAMParser : IDisposable, ISequenceAlignmentParser
    {
        #region Private Fields
  
        /// <summary>
        /// Symbols supported by BAM.
        /// </summary>
        private const string BAMAlphabet = "=ACMGRSVTWYHKDBN";

        private static byte[] BAMAlphabetAsBytes = BAMAlphabet.Select(x => (byte)x).ToArray();
   
        /// <summary>
        /// Holds the BAM file stream.
        /// </summary>
        private Stream readStream;

        /// <summary>
        /// Flag to indicate whether the BAM file is compressed or not.
        /// </summary>
        private bool isCompressed;

        /// <summary>
        /// Holds the names of the reference sequence.
        /// </summary>
        private RegexValidatedStringList refSeqNames;

        /// <summary>
        /// Holds the length of the reference sequences.
        /// </summary>
        private List<int> refSeqLengths;

        /// <summary>
        /// A temporary file stream to hold uncompressed blocks.
        /// </summary>
        private Stream deCompressedStream;

        /// <summary>
        /// Holds the current position of the compressed BAM file stream.
        /// Used while creating BAMIndex objects from a BAM file and while parsing a BAM file using a BAM index file.
        /// </summary>
        private long currentCompressedBlockStartPos;

        /// <summary>
        /// Holds the bam index object created from a BAM file.
        /// </summary>
        private BAMIndex bamIndex;

        /// <summary>
        /// Flag to indicate to whether to create BAMIndex while parsing BAM file or not.
        /// </summary>
        private bool createBamIndex = false;

        /// <summary>
        /// Name of the file being parsed.
        /// </summary>
        private string bamFilename;

        #endregion

        #region Constructors
        /// <summary>
        /// The default constructor which chooses the default encoding based on the alphabet.
        /// </summary>
        public BAMParser()
        {
            RefSequences = new List<ISequence>();
            refSeqNames = new RegexValidatedStringList(SAMAlignedSequenceHeader.RNameRegxExprPattern);
            refSeqLengths = new List<int>();
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the sequence alignment parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser type.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.BAM_NAME; }
        }

        /// <summary>
        /// Gets the description of the sequence alignment parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.BAMPARSER_DESCRIPTION; }
        }

        /// <summary>
        /// The alphabet to use for sequences in parsed SequenceAlignmentMap objects.
        /// Always returns singleton instance of SAMDnaAlphabet.
        /// </summary>
        public IAlphabet Alphabet
        {
            get
            {
                return SAMDnaAlphabet.Instance;
            }
            set
            {
                throw new NotSupportedException(Properties.Resource.BAMParserAlphabetCantBeSet);
            }
        }

        /// <summary>
        /// Gets the file extensions that the parser implementation
        /// will support.
        /// </summary>
        public string SupportedFileTypes
        {
            get { return Properties.Resource.BAM_FILEEXTENSION; }
        }

        /// <summary>
        /// Reference sequences, used to resolve "=" symbol in the sequence data.
        /// </summary>
        public IList<ISequence> RefSequences { get; private set; }
        #endregion

        #region Private Static Methods

        /// <summary>
        /// Returns a boolean value indicating whether a BAM file is compressed or uncompressed.
        /// </summary>
        /// <param name="array">Byte array containing first 4 bytes of a BAM file</param>
        /// <returns>Returns true if the specified byte array indicates that the BAM file is compressed else returns false.</returns>
        private static bool IsCompressedBAMFile(byte[] array)
        {
            bool result = false;
            if (array[0] == 31 && array[1] == 139 && array[2] == 8) //  && array[3] == 4
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Returns a boolean value indicating whether a BAM file is valid uncompressed BAM file or not.
        /// </summary>
        /// <param name="array">Byte array containing first 4 bytes of a BAM file</param>
        /// <returns>Returns true if the specified byte array indicates a valid uncompressed BAM file else returns false.</returns>
        private static bool IsUnCompressedBAMFile(byte[] array)
        {
            bool result = false;
            if (array[0] == 66 && array[1] == 65 && array[2] == 77 && array[3] == 1)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Gets optional value depending on the valuetype.
        /// </summary>
        /// <param name="valueType">Value Type.</param>
        /// <param name="array">Byte array to read from.</param>
        /// <param name="startIndex">Start index of value in the array.</param>
        private static object GetOptionalValue(char valueType, byte[] array, ref int startIndex)
        {
            object obj;
            int intValue;
            int len;
            switch (valueType)
            {
                case 'A':  //  Printable character
                    obj = (char)array[startIndex];
                    startIndex++;
                    break;
                case 'c': //signed 8-bit integer
                    intValue = (array[startIndex] & 0x7F);
                    if ((array[startIndex] & 0x80) == 0x80)
                    {

                        intValue = intValue + sbyte.MinValue;
                    }

                    obj = intValue;
                    startIndex++;
                    break;
                case 'C':
                    obj = (uint)array[startIndex];
                    startIndex++;
                    break;
                case 's':
                    obj = Helper.GetInt16(array, startIndex);
                    startIndex += 2;
                    break;
                case 'S':
                    obj = Helper.GetUInt16(array, startIndex);
                    startIndex += 2;
                    break;
                case 'i':
                    obj = Helper.GetInt32(array, startIndex);
                    startIndex += 4;
                    break;
                case 'I':
                    obj = Helper.GetUInt32(array, startIndex);
                    startIndex += 4;
                    break;
                case 'f':
                    obj = Helper.GetSingle(array, startIndex);
                    startIndex += 4;
                    break;
                case 'Z':
                    len = GetStringLength(array, startIndex);
                    obj = System.Text.ASCIIEncoding.ASCII.GetString(array, startIndex, len - 1);
                    startIndex += len;
                    break;
                case 'H':
                    len = GetStringLength(array, startIndex);
                    obj = Helper.GetHexString(array, startIndex, len - 1);
                    startIndex += len;
                    break;
                case 'B':
                    char arrayType = (char)array[startIndex];
                    startIndex++;
                    int arrayLen = Helper.GetInt32(array, startIndex);
                    startIndex += 4;
                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.Append(arrayType);
                    for (int i = 0; i < arrayLen; i++)
                    {
                        strBuilder.Append(',');
                        string value = GetOptionalValue(arrayType, array, ref startIndex).ToString();
                        strBuilder.Append(value);
                    }

                    obj = strBuilder.ToString();
                    break;
                default:
                    throw new FileFormatException(Properties.Resource.BAM_InvalidOptValType);
            }

            return obj;
        }

        /// <summary>
        /// Gets the length of the string in byte array.
        /// </summary>
        /// <param name="array">Byte array which contains string.</param>
        /// <param name="startIndex">Start index of array from which string is stored.</param>
        private static int GetStringLength(byte[] array, int startIndex)
        {
            int i = startIndex;
            while (i < array.Length && array[i] != '\x0')
            {
                i++;
            }

            return i + 1 - startIndex;
        }
   
#if FALSE  //Old code that is avoided now by going straight to bytes, kept for future debugging purposes if necessary 
        /// <summary>
        /// Gets equivalent sequence char for the specified encoded value.
        /// </summary>
        /// <param name="encodedValue">Encoded value.</param>
        private static char GetSeqChar(int encodedValue)
        {
            if (encodedValue >= 0 && encodedValue <= BAMAlphabet.Length)
            {
                return BAMAlphabet[encodedValue];
            }

            throw new FileFormatException(Properties.Resource.BAM_InvalidEncodedSequenceValue);
        }
#endif

        /// <summary>
        /// Gets equivalent sequence char for the specified encoded value.
        /// </summary>
        /// <param name="encodedValue">Encoded value.</param>
        private static byte GetSeqCharAsByte(int encodedValue)
        {
            if (encodedValue >= 0 && encodedValue <= BAMAlphabetAsBytes.Length)
            {
                return BAMAlphabetAsBytes[encodedValue];
            }
            throw new FileFormatException(Properties.Resource.BAM_InvalidEncodedSequenceValue);
        }

        /// <summary>
        /// Decompresses specified compressed stream to out stream.
        /// </summary>
        /// <param name="compressedStream">Compressed stream to decompress.</param>
        /// <param name="outStream">Out stream.</param>
        private static void Decompress(Stream compressedStream, Stream outStream)
        {
            using (GZipStream Decompress = new GZipStream(compressedStream, CompressionMode.Decompress, true))
            {
                Decompress.CopyTo(outStream);
            }
        }

        // Gets list of possible bins for a given start and end reference sequence co-ordinates.
        private static IList<uint> Reg2Bins(uint start, uint end)
        {
            List<uint> bins = new List<uint>();
            uint k;
            --end;
            bins.Add(0);
            for (k = 1 + (start >> 26); k <= 1 + (end >> 26); ++k) bins.Add(k);
            for (k = 9 + (start >> 23); k <= 9 + (end >> 23); ++k) bins.Add(k);
            for (k = 73 + (start >> 20); k <= 73 + (end >> 20); ++k) bins.Add(k);
            for (k = 585 + (start >> 17); k <= 585 + (end >> 17); ++k) bins.Add(k);
            for (k = 4681 + (start >> 14); k <= 4681 + (end >> 14); ++k) bins.Add(k);
            return bins;
        }

        // Gets all chunks for the specified ref sequence index.
        private static IList<Chunk> GetChunks(BAMReferenceIndexes refIndex)
        {
            List<Chunk> chunks = new List<Chunk>();
            foreach (Bin bin in refIndex.Bins)
            {
                chunks.InsertRange(chunks.Count, bin.Chunks);
            }

            return SortAndMergeChunks(chunks);
        }

        // Gets chunks for specified ref seq index, start and end co-ordinate this method considers linear index also.
        private static IList<Chunk> GetChunks(BAMReferenceIndexes refIndex, int start, int end)
        {
            //get all bins that overlap
            IList<uint> binnumbers = Reg2Bins((uint)start, (uint)end);
            //now only get those that match
            List<Chunk> chunks = refIndex.Bins.Where(B => binnumbers.Contains(B.BinNumber)).SelectMany(x=>x.Chunks).ToList();
            //now use linear indexing to filter any chunks that end before the first start
            if (refIndex.LinearIndex.Count > 0)
            {
                var binStart = start >> 14;
                FileOffset minStart;
                if (refIndex.Bins.Count <= binStart)
                {
                    minStart = refIndex.LinearIndex[binStart];
                }
                else
                {
                    minStart = refIndex.LinearIndex.Last();
                }
                chunks = chunks.Where(x => x.ChunkEnd >= minStart).ToList();
            }
                return SortAndMergeChunks(chunks);
        }

        /// <summary>
        /// Sorts and merges the overlapping chunks.
        /// </summary>
        /// <param name="chunks">Chunks to sort and merge.</param>
        private static List<Chunk> SortAndMergeChunks(List<Chunk> chunks)
        {
            List<Chunk> sortedChunks = chunks.OrderBy(C => C, ChunkSorterForMerging.GetInstance()).ToList();


            for (int i = 0; i < sortedChunks.Count - 1; i++)
            {
                Chunk currentChunk = sortedChunks[i];
                Chunk nextChunk = sortedChunks[i + 1];

                if (nextChunk.ChunkStart.CompareTo(currentChunk.ChunkStart) >= 0 && nextChunk.ChunkStart.CompareTo(currentChunk.ChunkEnd) <= 0)
                {
                    // merge chunks.
                    currentChunk.ChunkEnd = currentChunk.ChunkEnd.CompareTo(nextChunk.ChunkEnd) >= 0 ? currentChunk.ChunkEnd : nextChunk.ChunkEnd;
                    sortedChunks.RemoveAt(i + 1);
                    i--;
                }
            }

            return sortedChunks;
        }
        #endregion

        #region Public Methods

        #region Parse methods
        /// <summary>
        /// Returns a SequenceAlignmentMap object by parsing a BAM file.
        /// </summary>
        /// <param name="reader">Stream to read.</param>
        /// <returns>SequenceAlignmentMap object.</returns>
        public SequenceAlignmentMap Parse(Stream reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return GetAlignment(reader);
        }
        private SequenceAlignmentMap GetAlignmentMap(Stream reader, BAMIndexFile bamIndexFile = null,
              string refSeqName = null, int? refSeqIndex = null, int start = 0, int end = int.MaxValue)
        {
            SAMAlignmentHeader header;
            SequenceAlignmentMap seqMap;

            if (reader == null || reader.Length == 0)
            {
                throw new FileFormatException(Properties.Resource.BAM_InvalidBAMFile);
            }
            readStream = reader;
            ValidateReader();
            header = GetHeader();
            seqMap = null;
            if (refSeqIndex.HasValue && refSeqName == null)
            {
                // verify whether the chromosome index is there in the header or not.
                if (refSeqIndex < 0 || refSeqIndex >= header.ReferenceSequences.Count)
                {
                    throw new ArgumentOutOfRangeException("refSeqIndex");
                }
            }
            else if (refSeqName != null && !refSeqIndex.HasValue)
            {
                refSeqIndex = refSeqNames.IndexOf(refSeqName);
                if (refSeqIndex < 0 || !refSeqIndex.HasValue)
                {
                    string message = string.Format(CultureInfo.InvariantCulture, Properties.Resource.BAM_RefSeqNotFound, refSeqName);
                    throw new ArgumentException(message, "refSeqName");
                }
            }
            else if (refSeqIndex.HasValue && refSeqName != null)
            {
                throw new ArgumentException("Received values for params reSeqIndex and refSeqName. Only one parameter can have a value, not both.");
            }
            if (refSeqIndex.HasValue)
            {
                if (bamIndexFile != null)
                {
                    GetAlignmentWithIndex(bamIndexFile, (int)refSeqIndex, start, end, header, ref seqMap);
                }
                else
                {
                    throw new ArgumentNullException("refSeqIndex");
                }
            }
            else
            {
                GetAlignmentWithoutIndex(header, ref seqMap);
            }
            return seqMap;
        }

        /// <summary>
        /// Returns an iterator over a set of SAMAlignedSequences retrieved from a parsed BAM file.
        /// </summary>
        /// <param name="fileName">File name to read.</param>
        /// <returns>IEnumerable SAMAlignedSequence object.</returns>
        public IEnumerable<SAMAlignedSequence> ParseSequenceAsEnumerable(string fileName)
        {
            bamFilename = fileName;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            using (readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                foreach (SAMAlignedSequence seq in ParseSequenceAsEnumerable(readStream))
                {
                    yield return seq;
                }
            }
        }
        /// <summary>
        /// Returns an iterator over a set of SAMAlignedSequences retrieved from a parsed BAM file.
        /// </summary>
        /// <param name="reader">Stream to read</param>
        /// <returns>IEnumerable SAMAlignedSequence object.</returns>
        public IEnumerable<SAMAlignedSequence> ParseSequenceAsEnumerable(Stream reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            foreach (SAMAlignedSequence seq in GetAlignmentMapIterator(reader))
            {
                yield return seq;
            }
        }
        /// <summary>
        /// Returns SequenceAlignmentMap object by parsing specified BAM stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private SequenceAlignmentMap GetAlignment(Stream reader)
        {
            return GetAlignmentMap(reader);
        }
       
        private IEnumerable<SAMAlignedSequence> GetAlignmentMapIterator(Stream reader, BAMIndexFile bamIndexFile = null,
                string refSeqName = null, int? refSeq = null, int start = 0, int end = int.MaxValue)
        {
            SAMAlignmentHeader header;

            if (reader == null || reader.Length == 0)
            {
                throw new FileFormatException(Properties.Resource.BAM_InvalidBAMFile);
            }
            readStream = reader;
            ValidateReader();
            header = GetHeader();

            if (refSeq.HasValue && refSeqName == null)
            {
                // verify whether the chromosome index is there in the header or not.
                if (refSeq < 0 || refSeq >= header.ReferenceSequences.Count)
                {
                    throw new ArgumentOutOfRangeException("refSeq");
                }
            }
            else if (refSeqName != null && !refSeq.HasValue)
            {
                refSeq = refSeqNames.IndexOf(refSeqName);
                if (refSeq < 0 || !refSeq.HasValue)
                {
                    string message = string.Format(CultureInfo.InvariantCulture, Properties.Resource.BAM_RefSeqNotFound, refSeqName);
                    throw new ArgumentException(message, "refSeqName");
                }
            }
            else if (refSeq.HasValue && refSeqName != null)
            {
                throw new ArgumentException("Received values for params reSeqIndex and refSeqName. Only one parameter can have a value, not both.");
            }

            if (refSeq.HasValue)
            {
                if (bamIndexFile != null)
                {
                    foreach (SAMAlignedSequence seq in GetAlignmentWithIndexYield(bamIndexFile, (int)refSeq, start, end))
                    {
                        yield return seq;
                    }
                }
                else
                {
                    throw new ArgumentNullException("refSeqIndex");
                }
            }
            else
            {
                foreach (SAMAlignedSequence seq in GetAlignmentWithoutIndexYield())
                {
                    yield return seq;
                }
            }
        }
        private IEnumerable<SAMAlignedSequence> GetAlignmentWithIndexYield(BAMIndexFile bamIndexFile, int refSeqIndex, int start, int end)
        {
            BAMIndex bamIndexInfo;
            BAMReferenceIndexes refIndex;
            IList<Chunk> chunks;

            bamIndexInfo = bamIndexFile.Read();

            if (refSeqIndex != -1 && bamIndexInfo.RefIndexes.Count <= refSeqIndex)
            {
                throw new ArgumentOutOfRangeException("refSeqIndex");
            }

            refIndex = bamIndexInfo.RefIndexes[refSeqIndex];

            if (start == 0 && end == int.MaxValue)
            {
                chunks = GetChunks(refIndex);
            }
            else
            {
                chunks = GetChunks(refIndex, start, end);
            }

            IList<SAMAlignedSequence> alignedSeqs = GetAlignedSequences(chunks, start, end);
            foreach (SAMAlignedSequence alignedSeq in alignedSeqs)
            {
                yield return alignedSeqs[0];
            }

            readStream = null;
        }

        private void GetAlignmentWithIndex(BAMIndexFile bamIndexFile, int refSeqIndex, int start, int end,
               SAMAlignmentHeader header, ref SequenceAlignmentMap seqMap)
        {
            BAMIndex bamIndexInfo;
            BAMReferenceIndexes refIndex;
            IList<Chunk> chunks;
            seqMap = new SequenceAlignmentMap(header);

            bamIndexInfo = bamIndexFile.Read();

            if (refSeqIndex != -1 && bamIndexInfo.RefIndexes.Count <= refSeqIndex)
            {
                throw new ArgumentOutOfRangeException("refSeqIndex");
            }

            refIndex = bamIndexInfo.RefIndexes[refSeqIndex];

            if (start == 0 && end == int.MaxValue)
            {
                chunks = GetChunks(refIndex);
            }
            else
            {
                chunks = GetChunks(refIndex, start, end);
            }

            IList<SAMAlignedSequence> alignedSeqs = GetAlignedSequences(chunks, start, end);
            foreach (SAMAlignedSequence alignedSeq in alignedSeqs)
            {
                seqMap.QuerySequences.Add(alignedSeq);
            }

            readStream = null;
        }

        private void GetAlignmentWithoutIndex(SAMAlignmentHeader header, ref SequenceAlignmentMap seqMap)
        {
            Chunk lastChunk = null;
            FileOffset lastOffSet=new FileOffset(0,0);
            BAMReferenceIndexes refIndices = null;
            int lastBin = int.MaxValue;
            Bin bin;
            Chunk chunk;
            int lastRefSeqIndex = 0;
            int lastRefPos = Int32.MinValue;
            int curRefSeqIndex;
            if (createBamIndex)
            {
                bamIndex = new BAMIndex();
                for (int i = 0; i < refSeqLengths.Count; i++)
                {
                    bamIndex.RefIndexes.Add(new BAMReferenceIndexes(refSeqLengths[i]));
                }
                refIndices = bamIndex.RefIndexes[0];
            }
            if (!createBamIndex && seqMap == null)
            {
                seqMap = new SequenceAlignmentMap(header);
            }
            while (!IsEOF())
            {
                if (createBamIndex)
                {
                    lastOffSet=new FileOffset((ulong)currentCompressedBlockStartPos,(ushort)deCompressedStream.Position);
                }
                SAMAlignedSequence alignedSeq = GetAlignedSequence(0, int.MaxValue);
                #region BAM indexing
                if (createBamIndex)
                {
                    //TODO: This linear lookup is probably performance murder if many names
                    curRefSeqIndex = refSeqNames.IndexOf(alignedSeq.RName);
                    if (lastRefSeqIndex != curRefSeqIndex)
                    {
                        //switch to a new reference sequence and force the last bins to be unequal
                        if (lastRefSeqIndex > curRefSeqIndex)
                        {
                            throw new InvalidDataException("The BAM file is not sorted.  " + alignedSeq.QName + " appears after a later sequence");
                        }
                        refIndices = bamIndex.RefIndexes[curRefSeqIndex];
                        lastBin = int.MaxValue;
                        lastRefSeqIndex = curRefSeqIndex;
                        lastRefPos = Int32.MinValue;
                    }
                    if (lastRefPos > alignedSeq.Pos)
                    {
                        throw new InvalidDataException("The BAM file is not sorted.  " + alignedSeq.QName + " appears after a later sequence");                      
                    }
                 
                    lastRefPos = alignedSeq.Pos;
                    //update Bins when we switch over
                    if (lastBin != alignedSeq.Bin)
                    {
                        //do we need to add a new bin here or have we already seen it?
                        bin = refIndices.Bins.FirstOrDefault(B => B.BinNumber == alignedSeq.Bin);
                        if (bin == null)
                        {
                            bin = new Bin();
                            bin.BinNumber = (uint)alignedSeq.Bin;
                            refIndices.Bins.Add(bin);
                        }
                        //update the chunk we have just finished with, this code also appears outside the loop 
                        if (lastChunk != null)
                        {
                            lastChunk.ChunkEnd = lastOffSet;
                        }
                        //make a new chunk for the new bin
                        chunk = new Chunk();
                        chunk.ChunkStart = lastOffSet;
                        bin.Chunks.Add(chunk);
                        //update variables
                        lastChunk = chunk;
                        lastBin = alignedSeq.Bin;
                    }
                    //UPDATE LINEAR INDEX AND PROCESS READ FOR META-DATA
                    refIndices.AddReadToIndexInformation(alignedSeq,lastOffSet);                    
                }
                #endregion
                if (!createBamIndex && alignedSeq != null)
                {
                    seqMap.QuerySequences.Add(alignedSeq);
                }
                alignedSeq = null;
            }

            #region BAM Indexing
            if (createBamIndex)
            {
                ulong compressedOff=(ulong)readStream.Position;
                ushort uncompressedEnd=0;
                //TODO: Shouldn't this always be true?  Or go to max value?
                if (deCompressedStream != null) {
                    uncompressedEnd = (ushort)deCompressedStream.Position;
                }
                FileOffset veryLast=new FileOffset(compressedOff,uncompressedEnd);
                lastChunk.ChunkEnd=veryLast;
                foreach (var ri in bamIndex.RefIndexes)
                {
                    ri.Freeze();
                }
            }
            #endregion

        }



        private IEnumerable<SAMAlignedSequence> GetAlignmentWithoutIndexYield()
        {
            if (createBamIndex) { throw new InvalidProgramException("It was assumed that the index would not be created during enumeration, please check the logic"); }
            while (!IsEOF())
            {
                SAMAlignedSequence alignedSeq = GetAlignedSequence(0, int.MaxValue);
                yield return alignedSeq;
                alignedSeq = null;
            }
        }

       


        #endregion

        /// <summary>
        /// Returns a SequenceAlignmentMap object by parsing a BAM file.
        /// </summary>
        /// <param name="fileName">BAM filename to parse.</param>
        /// <returns>SequenceAlignmentMap object.</returns>
        public SequenceAlignmentMap Parse(string fileName)
        {
            bamFilename = fileName;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            using (readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Parse(readStream);
            }
        }
        #endregion Parse methods

        #region Methods to get BAMIndex from BAMFile
        /// <summary>
        /// Returns BAMIndex by parsing specified BAM stream.
        /// </summary>
        /// <param name="bamfilename">BAM file to read.</param>
        public BAMIndex GetIndexFromBAMFile(string bamfilename)
        {
            if (string.IsNullOrWhiteSpace(bamfilename))
            {
                throw new ArgumentNullException("bamfilename");
            }

            using (Stream bamStream = new FileStream(bamfilename, FileMode.Open, FileAccess.Read))
            {
                return GetIndexFromBAMFile(bamStream);
            }
        }

        /// <summary>
        /// Returns BAMIndex by parsing specified BAM stream.
        /// </summary>
        /// <param name="bamStream">Stream to read.</param>
        public BAMIndex GetIndexFromBAMFile(Stream bamStream)
        {
            if (bamStream == null)
            {
                throw new ArgumentNullException("bamStream");
            }

            try
            {
                createBamIndex = true;
                GetAlignment(bamStream);
                ReduceChunks();
                return bamIndex;
            }
            finally
            {
                createBamIndex = false;
            }
        }
        #endregion Methods to get BAMIndex from BAMFile

        #region ParseRange Methods (Uses Index file)

        /// <summary>
        /// Attempts to find the name of an index file for the given BAM file name, throws an error if none is found.
        /// </summary>
        /// <param name="BamFileToFindIndexOf">The name of the BAM file.</param>
        /// <returns>The name of the index file for the given BAM file.</returns>
        private string getBAMIndexFileName(string BamFileToFindIndexOf)
        {
            //Try Name+".bai"
            string possibleName = BamFileToFindIndexOf + Properties.Resource.BAM_INDEXFILEEXTENSION;
            if (File.Exists(possibleName))
            {
                return possibleName;
            }
            //Try to remove .bam and replace it with .bai
            possibleName = BamFileToFindIndexOf.Replace(Properties.Resource.BAM_FILEEXTENSION, Properties.Resource.BAM_INDEXFILEEXTENSION);
            if (File.Exists(possibleName))
            {
                return possibleName;
            }
            throw new FileNotFoundException("Could not find BAM Index file for: " + bamFilename + " you may need to create an index file before parsing it.");
        }
        #region ParseRange with Chromosome name

        /// <summary>
        /// Parses specified BAM file using index file.
        /// Index file is assumed to be in the same location as that of the specified bam file with the name "filename".bai
        /// For example, if the specified bam file name is D:\BAMdata\sample.bam then index file name will be taken as D:\BAMdata\sample.bam.bai
        /// If index file is not available then this method throw an exception.
        /// </summary>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="refSeqName">Name of reference sequence.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments for specified reference sequence.</returns>
        public SequenceAlignmentMap ParseRange(string fileName, string refSeqName)
        {

            if (refSeqName == null)
            {
                throw new ArgumentNullException("refSeqName");
            }

            using (FileStream bamStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string bamIndexFileName = getBAMIndexFileName(fileName);
                using (BAMIndexFile bamIndexFile = new BAMIndexFile(bamIndexFileName, FileMode.Open, FileAccess.Read))
                {
                    return GetAlignment(bamStream, bamIndexFile, refSeqName);
                }
            }
        }

        /// <summary>
        /// Parses specified BAM file using index file.
        /// Index file is assumed to be in the same location as that of the specified bam file with the name "filename".bai
        /// For example, if the specified bam file name is D:\BAMdata\sample.bam then index file name will be taken as D:\BAMdata\sample.bam.bai
        /// If index file is not available then this method throw an exception.
        /// </summary>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="refSeqName">Name of reference sequence.</param>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments overlaps with the specified start 
        /// and end co-ordinate of the specified reference sequence.</returns>
        public SequenceAlignmentMap ParseRange(string fileName, string refSeqName, int start, int end)
        {
            if (refSeqName == null)
            {
                throw new ArgumentNullException("refSeqName");
            }
            using (FileStream bamStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string bamIndexFileName = getBAMIndexFileName(fileName);
                using (BAMIndexFile bamIndexFile = new BAMIndexFile(bamIndexFileName, FileMode.Open, FileAccess.Read))
                {
                    return GetAlignment(bamStream, bamIndexFile, refSeqName, start, end);
                }
            }
        }

        #endregion ParseRange with Chromosome name

        #region ParseRange With Referece Index (Reference sequence name in the Header)

        /// <summary>
        /// Parses specified BAM file using index file.
        /// Index file is assumed to be in the same location as that of the specified bam file with the name "filename".bai
        /// For example, if the specified bam file name is D:\BAMdata\sample.bam then index file name will be taken as D:\BAMdata\sample.bam.bai
        /// If index file is not available then this method throw an exception.
        /// </summary>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="refSeqIndex">Index of reference sequence.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments for specified reference sequence.</returns>
        public SequenceAlignmentMap ParseRange(string fileName, int refSeqIndex)
        {
            using (FileStream bamStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string bamIndexFileName = getBAMIndexFileName(fileName);
                using (BAMIndexFile bamIndexFile = new BAMIndexFile(bamIndexFileName, FileMode.Open, FileAccess.Read))
                {
                    return GetAlignment(bamStream, bamIndexFile, refSeqIndex);
                }
            }
        }

        /// <summary>
        /// Parses specified BAM file using index file.
        /// Index file is assumed to be in the same location as that of the specified bam file with the name "filename".bai
        /// For example, if the specified bam file name is D:\BAMdata\sample.bam then index file name will be taken as D:\BAMdata\sample.bam.bai
        /// If index file is not available then this method throw an exception.
        /// </summary>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="refSeqIndex">Index of reference sequence.</param>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments overlaps with the specified start 
        /// and end co-ordinate of the specified reference sequence.</returns>
        public SequenceAlignmentMap ParseRange(string fileName, int refSeqIndex, int start, int end)
        {
            using (FileStream bamStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string bamIndexFileName = getBAMIndexFileName(fileName);
                using (BAMIndexFile bamIndexFile = new BAMIndexFile(bamIndexFileName, FileMode.Open, FileAccess.Read))
                {
                    return GetAlignment(bamStream, bamIndexFile, refSeqIndex, start, end);
                }
            }
        }
        #endregion ParseRange With Referece Index

        #region ParseRange with SequenceRange

        /// <summary>
        /// Parses specified BAM file using index file.
        /// </summary>
        /// <param name="fileName">BAM file name.</param>
        /// <param name="range">SequenceRange object which contains reference sequence name and start and end co-ordinates.</param>
        /// <returns>SequenceAlignmentMap object which contains alignments for specified reference sequence and for specified range.</returns>
        public SequenceAlignmentMap ParseRange(string fileName, SequenceRange range)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            if (string.IsNullOrEmpty(range.ID))
            {
                throw new ArgumentException("Reference sequence name (range.ID) can't empty or null.");
            }

            int start = range.Start >= int.MaxValue ? int.MaxValue : (int)range.Start;
            int end = range.End >= int.MaxValue ? int.MaxValue : (int)range.End;

            if (start == 0 && end == int.MaxValue)
            {
                using (FileStream bamStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string bamIndexFileName = getBAMIndexFileName(fileName);
                    using (BAMIndexFile bamIndexFile = new BAMIndexFile(bamIndexFileName, FileMode.Open, FileAccess.Read))
                    {
                        return GetAlignment(bamStream, bamIndexFile, range.ID);
                    }
                }
            }
            else
            {
                using (FileStream bamStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string bamIndexFileName = getBAMIndexFileName(fileName);
                    using (BAMIndexFile bamIndexFile = new BAMIndexFile(bamIndexFileName, FileMode.Open, FileAccess.Read))
                    {
                        return GetAlignment(bamStream, bamIndexFile, range.ID, start, end);
                    }
                }
            }

        }

        #endregion ParseRange with SequenceRange

        #endregion ParseRange Methods

        #region ISequenceAlignmentParser Method
        /// <summary>
        /// Always throws NotSupportedException as BAM parser does not supports reading from a text reader.
        /// </summary>
        /// <param name="reader">Text reader.</param>
        IList<ISequenceAlignment> ISequenceAlignmentParser.Parse(TextReader reader)
        {
            throw new NotSupportedException(Properties.Resource.BAM_TextreaderNotSupportedMessage);
        }

        ///// <summary>
        ///// Parses a list of sequence alignment from a BAM file.
        ///// </summary>
        ///// <param name="fileName">The name of a BAM file to parse.</param>
        ///// <returns>The list of parsed ISequenceAlignment objects.</returns>
        IList<ISequenceAlignment> ISequenceAlignmentParser.Parse(string fileName)
        {
            ISequenceAlignment alignment = Parse(fileName);
            return new List<ISequenceAlignment>() { alignment };
        }


        /// <summary>
        /// Always throws NotSupportedException as BAM parser does not supports reading from a text reader.
        /// </summary>
        /// <param name="reader">Text reader.</param>
        public ISequenceAlignment ParseOne(TextReader reader)
        {
            throw new NotSupportedException(Properties.Resource.BAM_TextreaderNotSupportedMessage);
        }


        /// <summary>
        /// Parses a SequenceAlignmentMap from a BAM file.
        /// </summary>
        /// <param name="fileName">The name of a BAM file.</param>
        /// <returns>ISequenceAlignment object.</returns>
        public ISequenceAlignment ParseOne(string fileName)
        {
            return Parse(fileName);
        }

        #endregion

        /// <summary>
        /// Disposes resources if any.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the SAMAlignmentHeader from the specified stream.
        /// Note that this method resets the specified stream to BOF before reading.
        /// </summary>
        /// <param name="bamStream">BAM file stream.</param>
        public SAMAlignmentHeader GetHeader(Stream bamStream)
        {
            if (bamStream == null)
            {
                throw new ArgumentNullException("bamStream");
            }

            readStream = bamStream;
            ValidateReader();

            return GetHeader();
        }

        /// <summary>
        /// Returns an aligned sequence by parses the BAM file.
        /// </summary>
        /// <param name="isReadOnly">
        /// Flag to indicate whether the resulting sequence in the SAMAlignedSequence should be in 
        /// readonly mode or not. If this flag is set to true then the resulting sequence's 
        /// isReadOnly property will be set to true, otherwise it will be set to false.
        /// </param>
        public SAMAlignedSequence GetAlignedSequence(bool isReadOnly)
        {
            return GetAlignedSequence(0, int.MaxValue);
        }


        #region Protected Methods

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">If true disposes resourses held by this instance.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (readStream != null)
                {
                    if (!string.IsNullOrWhiteSpace(bamFilename))
                    {
                        readStream.Dispose();
                    }

                    readStream = null;
                }

                if (deCompressedStream != null)
                {
                    deCompressedStream.Dispose();
                    deCompressedStream = null;
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Validates the BAM stream.
        /// </summary>
        private void ValidateReader()
        {
            isCompressed = true;
            byte[] array = new byte[4];

            if (readStream.Read(array, 0, 4) != 4)
            {
                // cannot read file properly.
                throw new FileFormatException(Properties.Resource.BAM_InvalidBAMFile);
            }

            isCompressed = IsCompressedBAMFile(array);

            if (!isCompressed)
            {
                if (!IsUnCompressedBAMFile(array))
                {
                    // Neither compressed BAM file nor uncompressed BAM file header.
                    throw new FileFormatException(Properties.Resource.BAM_InvalidBAMFile);
                }
            }

            readStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Parses the BAM file and returns the Header.
        /// </summary>
        private SAMAlignmentHeader GetHeader()
        {
            SAMAlignmentHeader header = new SAMAlignmentHeader();
            refSeqNames = new RegexValidatedStringList(Bio.IO.SAM.SAMAlignedSequenceHeader.RNameRegxExprPattern);
            refSeqLengths = new List<int>();

            readStream.Seek(0, SeekOrigin.Begin);
            this.deCompressedStream = null;
            byte[] array = new byte[8];
            ReadUnCompressedData(array, 0, 8);
            int l_text = Helper.GetInt32(array, 4);
            byte[] samHeaderData = new byte[l_text];
            if (l_text != 0)
            {
                ReadUnCompressedData(samHeaderData, 0, l_text);
            }

            ReadUnCompressedData(array, 0, 4);
            int noofRefSeqs = Helper.GetInt32(array, 0);

            for (int i = 0; i < noofRefSeqs; i++)
            {
                ReadUnCompressedData(array, 0, 4);
                int len = Helper.GetInt32(array, 0);
                byte[] refName = new byte[len];
                ReadUnCompressedData(refName, 0, len);
                ReadUnCompressedData(array, 0, 4);
                int refLen = Helper.GetInt32(array, 0);
                refSeqNames.Add(System.Text.ASCIIEncoding.ASCII.GetString(refName, 0, refName.Length - 1));
                refSeqLengths.Add(refLen);
            }

            if (samHeaderData.Length != 0)
            {
                string str = System.Text.ASCIIEncoding.ASCII.GetString(samHeaderData);
                using (StringReader reader = new StringReader(str))
                {
                    header = SAMParser.ParseSAMHeader(reader);
                }
            }

            header.ReferenceSequences.Clear();

            for (int i = 0; i < refSeqNames.Count; i++)
            {
                string refname = refSeqNames[i];
                int length = refSeqLengths[i];
                header.ReferenceSequences.Add(new ReferenceSequenceInfo(refname, length));
            }

            return header;
        }

        /// <summary>
        /// Merges small chunks belongs to a bin which are in the same compressed block.
        /// This will reduce number of seek calls required.
        /// </summary>
        private void ReduceChunks()
        {
            if (bamIndex == null)
                return;

            for (int i = 0; i < bamIndex.RefIndexes.Count; i++)
            {
                BAMReferenceIndexes bamRefIndex = bamIndex.RefIndexes[i];

                for (int j = 0; j < bamRefIndex.Bins.Count; j++)
                {
                    Bin bin = bamRefIndex.Bins[j];
                    int lastIndex = 0;
                    int noofchunksToRemove = 0;
                    for (int k = 1; k < bin.Chunks.Count; k++)
                    {
                        // check for the chunks which are in the same compressed blocks.
                        //note picard merges the same or adjacent blocks, though I think that may be a coding error oon their part
                        if (bin.Chunks[lastIndex].ChunkEnd.CompressedBlockOffset == bin.Chunks[k].ChunkStart.CompressedBlockOffset)
                        {
                            bin.Chunks[lastIndex].ChunkEnd = bin.Chunks[k].ChunkEnd;
                            noofchunksToRemove++;
                        }
                        else
                        {
                            bin.Chunks[++lastIndex] = bin.Chunks[k];
                        }
                    }
                    if (noofchunksToRemove > 0)
                    {
                        for (int index = 0; index < noofchunksToRemove; index++)
                        {
                            bin.Chunks.RemoveAt(bin.Chunks.Count - 1);
                        }
                    }
                }
            }
        }

     
        /// <summary>
        /// Returns an aligned sequence by parses the BAM file.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        private SAMAlignedSequence GetAlignedSequence(int start, int end)
        {
            byte[] array = new byte[4];

            ReadUnCompressedData(array, 0, 4);
            int blockLen = Helper.GetInt32(array, 0);
            byte[] alignmentBlock = new byte[blockLen];
            ReadUnCompressedData(alignmentBlock, 0, blockLen);
            SAMAlignedSequence alignedSeq = new SAMAlignedSequence();
            int value;
            UInt32 UnsignedValue;
            // 0-4 bytes
            int refSeqIndex = Helper.GetInt32(alignmentBlock, 0);

            if (refSeqIndex == -1)
                alignedSeq.SetPreValidatedRName("*");
            else
                alignedSeq.SetPreValidatedRName(refSeqNames[refSeqIndex]);

            // 4-8 bytes
            alignedSeq.Pos = Helper.GetInt32(alignmentBlock, 4) + 1;

            // if there is no overlap no need to parse further.
            //     BAMPos > closedEnd
            // => (alignedSeq.Pos - 1) > end -1
            if (alignedSeq.Pos > end)
            {
                return null;
            }

            // 8 - 12 bytes "bin<<16|mapQual<<8|read_name_len"
            UnsignedValue = Helper.GetUInt32(alignmentBlock, 8);

            // 10 -12 bytes
            alignedSeq.Bin = (int)(UnsignedValue & 0xFFFF0000) >> 16;
            // 9th bytes
            alignedSeq.MapQ = (int)(UnsignedValue & 0x0000FF00) >> 8;
            // 8th bytes
            int queryNameLen = (int)(UnsignedValue & 0x000000FF);

            // 12 - 16 bytes
            UnsignedValue = Helper.GetUInt32(alignmentBlock, 12);
            // 14-16 bytes
            int flagValue = (int)(UnsignedValue & 0xFFFF0000) >> 16;
            alignedSeq.Flag = (SAMFlags)flagValue;
            // 12-14 bytes
            int cigarLen = (int)(UnsignedValue & 0x0000FFFF);

            // 16-20 bytes
            int readLen = Helper.GetInt32(alignmentBlock, 16);

            // 20-24 bytes
            int mateRefSeqIndex = Helper.GetInt32(alignmentBlock, 20);
            if (mateRefSeqIndex != -1)
            {
                alignedSeq.SetPreValidatedMRNM(refSeqNames[mateRefSeqIndex]);
            }
            else
            {
                alignedSeq.SetPreValidatedMRNM("*");
            }

            // 24-28 bytes
            alignedSeq.MPos = Helper.GetInt32(alignmentBlock, 24) + 1;

            // 28-32 bytes
            alignedSeq.ISize = Helper.GetInt32(alignmentBlock, 28);

            // 32-(32+readLen) bytes
            alignedSeq.QName = System.Text.ASCIIEncoding.ASCII.GetString(alignmentBlock, 32, queryNameLen - 1);
            StringBuilder strbuilder = new StringBuilder();
            int startIndex = 32 + queryNameLen;

            for (int i = startIndex; i < (startIndex + cigarLen * 4); i += 4)
            {
                // Get the CIGAR operation length stored in first 28 bits.
                UInt32 cigarValue = Helper.GetUInt32(alignmentBlock, i);
                strbuilder.Append(((cigarValue & 0xFFFFFFF0) >> 4).ToString(CultureInfo.InvariantCulture));

                // Get the CIGAR operation stored in last 4 bits.
                value = (int)cigarValue & 0x0000000F;

                // MIDNSHP=>0123456
                switch (value)
                {
                    case 0:
                        strbuilder.Append("M");
                        break;
                    case 1:
                        strbuilder.Append("I");
                        break;
                    case 2:
                        strbuilder.Append("D");
                        break;
                    case 3:
                        strbuilder.Append("N");
                        break;
                    case 4:
                        strbuilder.Append("S");
                        break;
                    case 5:
                        strbuilder.Append("H");
                        break;
                    case 6:
                        strbuilder.Append("P");
                        break;
                    case 7:
                        strbuilder.Append("=");
                        break;
                    case 8:
                        strbuilder.Append("X");
                        break;
                    default:
                        throw new FileFormatException(Properties.Resource.BAM_InvalidCIGAR);
                }
            }

            string cigar = strbuilder.ToString();
            if (string.IsNullOrWhiteSpace(cigar))
            {
                alignedSeq.SetPreValidatedCIGAR("*");
            }
            else
            {
                alignedSeq.SetPreValidatedCIGAR(cigar);
            }
            // if there is no overlap no need to parse further.
            // ZeroBasedRefEnd < start
            // => (alignedSeq.RefEndPos -1) < start
            if (alignedSeq.RefEndPos - 1 < start && alignedSeq.RName!="*")
            {
                return null;
            }

            startIndex += cigarLen * 4;
            var sequence = new byte[readLen];
            int sequenceIndex = 0;
            int index = startIndex;
            for (; index < (startIndex + (readLen + 1) / 2) - 1; index++)
            {
                // Get first 4 bit value
                value = (alignmentBlock[index] & 0xF0) >> 4;
                sequence[sequenceIndex++] = GetSeqCharAsByte(value);
                // Get last 4 bit value
                value = alignmentBlock[index] & 0x0F;
                sequence[sequenceIndex++] = GetSeqCharAsByte(value);
                
            }

            value = (alignmentBlock[index] & 0xF0) >> 4;
            sequence[sequenceIndex++] = GetSeqCharAsByte(value);
                
            if (readLen % 2 == 0)
            {
                value = alignmentBlock[index] & 0x0F;
                sequence[sequenceIndex++] = GetSeqCharAsByte(value);
            }

            startIndex = index + 1;
            byte[] qualValues = new byte[readLen];
            
            if (alignmentBlock[startIndex] != 0xFF)
            {
                for (int i = startIndex; i < (startIndex + readLen); i++)
                {
                    qualValues[i - startIndex] = (byte)(alignmentBlock[i] + 33);
                }
                //validate quality scores here
                byte badVal;
                bool ok = QualitativeSequence.ValidateQualScores(qualValues, SAMParser.QualityFormatType, out badVal);
                if (!ok)
                {
                    string message = string.Format(CultureInfo.CurrentUICulture,
                                         Properties.Resource.InvalidEncodedQualityScoreFound,
                                         (char)badVal,
                                         SAMParser.QualityFormatType);
                    throw new ArgumentOutOfRangeException("encodedQualityScores", message);
                }
            }
            else
            {
                qualValues = new byte[] { SAMParser.AsteriskAsByte };
            }
            //Values have already been validated when first parsed at this point so no need to again
            SAMParser.ParseQualityNSequence(alignedSeq, Alphabet, sequence, qualValues,false);

            startIndex += readLen;
            if (alignmentBlock.Length > startIndex + 4 && alignmentBlock[startIndex] != 0x0 && alignmentBlock[startIndex + 1] != 0x0)
            {
                for (index = startIndex; index < alignmentBlock.Length; )
                {
                    SAMOptionalField optionalField = new SAMOptionalField();
                    optionalField.Tag = System.Text.ASCIIEncoding.ASCII.GetString(alignmentBlock, index, 2);
                    index += 2;
                    char vType = (char)alignmentBlock[index++];
                    
                    // SAM format supports [AifZH] for value type.
                    // In BAM, an integer may be stored as a signed 8-bit integer (c), unsigned 8-bit integer (C), signed short (s), unsigned
                    // short (S), signed 32-bit (i) or unsigned 32-bit integer (I), depending on the signed magnitude of the integer. However,
                    // in SAM, all types of integers are presented as type ʻiʼ. 

                    //NOTE: Code previously here checked for valid value and threw an exception here, but this exception/validation is checked for in this method below, as while as when the value is set.

                    optionalField.Value = GetOptionalValue(vType, alignmentBlock, ref index).ToString();

                    // Convert to SAM format, where all integers are represented the same way
                    if ("cCsSI".IndexOf(vType) >= 0)
                    {
                        vType = 'i';
                    }
                    optionalField.VType = vType.ToString();

                    alignedSeq.OptionalFields.Add(optionalField);
                }
            }

            return alignedSeq;
        }
            


        /// <summary>
        /// Reads specified number of uncompressed bytes from BAM file to byte array
        /// </summary>
        /// <param name="array">Byte array to copy.</param>
        /// <param name="offset">Offset of Byte array from which the data has to be copied.</param>
        /// <param name="count">Number of bytes to copy.</param>
        private void ReadUnCompressedData(byte[] array, int offset, int count)
        {
            if (!isCompressed)
            {
                readStream.Read(array, offset, count);
                return;
            }

            if (deCompressedStream == null || deCompressedStream.Length - deCompressedStream.Position == 0)
            {
                GetNextBlock();
            }

            long remainingBlockSize = deCompressedStream.Length - deCompressedStream.Position;
            if (remainingBlockSize == 0)
            {
                return;
            }

            int bytesToRead = remainingBlockSize >= (long)count ? count : (int)remainingBlockSize;
            deCompressedStream.Read(array, offset, bytesToRead);

            if (bytesToRead < count)
            {
                GetNextBlock();
                ReadUnCompressedData(array, bytesToRead, count - bytesToRead);
            }
        }

        /// <summary>
        /// Gets next block by reading and decompressing the compressed block from compressed BAM file.
        /// </summary>
        private void GetNextBlock()
        {
            int ELEN = 0;
            int BSIZE = 0;
            int size = 0;
            byte[] arrays = new byte[18];
            deCompressedStream = null;
            if (readStream.Length <= readStream.Position)
            {
                return;
            }

            currentCompressedBlockStartPos = readStream.Position;
            //read the bgzf header array
            readStream.Read(arrays, 0, 18);
            //called xlen in the spec
            ELEN = Helper.GetUInt16(arrays, 10);
            //verify there is an extra field, get the block size
            if (ELEN != 0)
            {
                BSIZE = Helper.GetUInt16(arrays, 12 + ELEN - 2);
            }

            size = BSIZE + 1;

            byte[] block = new byte[size];
            using (MemoryStream memStream = new MemoryStream(size))
            {
                arrays.CopyTo(block, 0);

                if (readStream.Read(block, 18, size - 18) != size - 18)
                {
                    throw new FileFormatException(Properties.Resource.BAM_UnableToReadCompressedBlock);
                }

                uint unCompressedBlockSize = Helper.GetUInt32(block, size - 4);

                deCompressedStream = GetTempStream(unCompressedBlockSize);

                memStream.Write(block, 0, size);
                memStream.Seek(0, SeekOrigin.Begin);
                Decompress(memStream, deCompressedStream);
            }

            deCompressedStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Gets the temp stream to store Decompressed blocks.
        /// If the specified capacity is with in the Maximum integer (32 bit int) limit then 
        /// a MemoryStream is created else a temp file is created to store Decompressed data.
        /// </summary>
        /// <param name="capacity">Required capacity.</param>
        private Stream GetTempStream(uint capacity)
        {
            if (deCompressedStream != null)
            {
                deCompressedStream.Close();
                deCompressedStream = null;
            }

            if (capacity <= int.MaxValue)
            {
                deCompressedStream = new MemoryStream((int)capacity);
            }
            else
            {
                string fileName = Path.GetTempFileName();
                deCompressedStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }

            return deCompressedStream;
        }

        /// <summary>
        /// Returns a boolean indicating whether the reader is reached end of file or not.
        /// </summary>
        /// <returns>Returns true if the end of the file has been reached.</returns>
        public bool IsEOF()
        {
            // if the BAM file is uncompressed then check the EOF by verifying the BAM file EOF.
            if (!isCompressed || deCompressedStream == null)
            {
                return readStream.Length <= readStream.Position;
            }

            // if the BAM file is compressed then verify uncompressed block.
            if (deCompressedStream.Length <= deCompressedStream.Position)
            {
                GetNextBlock();
            }

            return deCompressedStream == null || deCompressedStream.Length == 0;
        }

        // Returns SequenceAlignmentMap by prasing specified BAM stream and BAMIndexFile for the specified reference sequence index.
        private SequenceAlignmentMap GetAlignment(Stream bamStream, BAMIndexFile bamIndexFile, int refSeqIndex)
        {
            return GetAlignmentMap(bamStream, bamIndexFile, null, refSeqIndex);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private IEnumerable<SAMAlignedSequence> GetAlignmentYield(Stream bamStream, BAMIndexFile bamIndexFile, int refSeqIndex)
        {
            foreach (SAMAlignedSequence seq in GetAlignmentMapIterator(bamStream, bamIndexFile, null, refSeqIndex))
            {
                yield return seq;
            }
        }
        // Returns SequenceAlignmentMap by prasing specified BAM stream and BAMIndexFile for the specified reference sequence name.
        private SequenceAlignmentMap GetAlignment(Stream bamStream, BAMIndexFile bamIndexFile, string refSeqName)
        {
            return GetAlignmentMap(bamStream, bamIndexFile, refSeqName);
    
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private IEnumerable<SAMAlignedSequence> GetAlignmentYield(Stream bamStream, BAMIndexFile bamIndexFile, string refSeqName)
        {
            foreach (SAMAlignedSequence seq in GetAlignmentMapIterator(bamStream, bamIndexFile, refSeqName))
            {
                yield return seq;
            }
        }

        // Returns SequenceAlignmentMap by prasing specified BAM stream and BAMIndexFile for the specified reference sequence index.
        // this method uses linear index information also.
        private SequenceAlignmentMap GetAlignment(Stream bamStream, BAMIndexFile bamIndexFile, string refSeqName, int start, int end)
        {
            return GetAlignmentMap(bamStream, bamIndexFile, refSeqName, null, start, end);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private IEnumerable<SAMAlignedSequence> GetAlignmentYield(Stream bamStream, BAMIndexFile bamIndexFile, string refSeqName, int start, int end)
        {
            foreach (SAMAlignedSequence seq in GetAlignmentMapIterator(bamStream, bamIndexFile, refSeqName, -1, start, end))
            {
                yield return seq;
            }
        }
        // Returns SequenceAlignmentMap by prasing specified BAM stream and BAMIndexFile for the specified reference sequence index.
        // this method uses linear index information also.
        private SequenceAlignmentMap GetAlignment(Stream bamStream, BAMIndexFile bamIndexFile, int refSeqIndex, int start, int end)
        {
            return GetAlignmentMap(bamStream, bamIndexFile, null, refSeqIndex, start, end);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private IEnumerable<SAMAlignedSequence> GetAlignmentYield(Stream bamStream, BAMIndexFile bamIndexFile, int refSeqIndex, int start, int end)
        {
            foreach (SAMAlignedSequence seq in GetAlignmentMapIterator(bamStream, bamIndexFile, null, refSeqIndex, start, end))
            {
                yield return seq;
            }
        }

        // Gets aligned sequence from the specified chunks of the BAM file which overlaps with the specified start and end co-ordinates.
        private IList<SAMAlignedSequence> GetAlignedSequences(IList<Chunk> chunks, int start, int end)
        {
            List<SAMAlignedSequence> alignedSeqs = new List<SAMAlignedSequence>();
            foreach (Chunk chunk in chunks)
            {
                readStream.Seek((long)chunk.ChunkStart.CompressedBlockOffset, SeekOrigin.Begin);
                GetNextBlock();
                if (deCompressedStream != null)
                {
                    deCompressedStream.Seek(chunk.ChunkStart.UncompressedBlockOffset, SeekOrigin.Begin);

                    // read until eof or end of the chunck is reached.
                    while (!IsEOF() && (currentCompressedBlockStartPos < (long)chunk.ChunkEnd.CompressedBlockOffset || deCompressedStream.Position < chunk.ChunkEnd.UncompressedBlockOffset))
                    {
                        SAMAlignedSequence alignedSeq = GetAlignedSequence(start, end);
                        if (alignedSeq != null)
                        {
                            alignedSeqs.Add(alignedSeq);
                        }
                    }
                }
            }

            return alignedSeqs;
        }

        #endregion
    }
}
