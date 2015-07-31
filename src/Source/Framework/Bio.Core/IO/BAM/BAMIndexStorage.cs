using System;
using System.IO;
using Bio.Util;

namespace Bio.IO.BAM
{
    /// <summary>
    /// Class to read or write BAMIndex data from a file or a stream.
    /// </summary>
    public class BAMIndexStorage : IDisposable
    {
        /// <summary>
        /// The highest number of bins allowed, meta-data can be stored in the chunks position for a bin
        /// this large
        /// </summary>
        internal const int MaxBins = 37450;   // =(8^6-1)/7+1

        /// <summary>
        /// The number of 16kb (2^14) bins in the indexing scheme
        /// </summary>
        internal const int MaxLinerindexArraySize = MaxBins + 1 - 4681;

        /// <summary>
        /// Not all sequences can get all possible bins, so this returns the largest sequence length possible
        /// </summary>
        /// <param name="sequenceLength"></param>
        /// <returns></returns>
        internal static int LargestBinPossibleForSequenceLength(int sequenceLength)
        {
            return 4681 + (sequenceLength >> 14);
        }

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        public Stream Source { get; private set; }

        /// <summary>
        /// Creates new instance of the class with specified stream.
        /// </summary>
        /// <param name="stream">Stream to use while reading or writing BAMIndex data.</param>
        public BAMIndexStorage(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            Source = stream;
        }

        /// <summary>
        /// Writes specified BAMIndex data.
        /// </summary>
        /// <param name="bamIndex">BAMIndex instance to write.</param>
        public void Write(BAMIndex bamIndex)
        {
            if (bamIndex == null)
            {
                throw new ArgumentNullException("bamIndex");
            }

            if (Source == null)
            {
                throw new InvalidOperationException(Properties.Resource.BAM_CantUseBAMIndexStreamDisposed);
            }

            byte[] magic = { 66, 65, 73, 1 };
            Write(magic, 0, 4);

            byte[] arrays = Helper.GetLittleEndianByteArray(bamIndex.RefIndexes.Count);
            Write(arrays, 0, 4);

            foreach (BAMReferenceIndexes index in bamIndex.RefIndexes)
            {
                int binCount = index.Bins.Count;
                bool addingMetaData = index.HasMetaData && BitConverter.IsLittleEndian;
                if (addingMetaData)
                {
                    binCount++;
                }                
                arrays = Helper.GetLittleEndianByteArray(binCount);
                this.Write(arrays, 0, 4);
                
                //Write each bin
                foreach (Bin bin in index.Bins)
                {
                    arrays = Helper.GetLittleEndianByteArray(bin.BinNumber);
                    this.Write(arrays, 0, 4);
                    int chunkCount = bin.Chunks.Count;
                   
                    arrays = Helper.GetLittleEndianByteArray(chunkCount);
                    this.Write(arrays, 0, 4);
                    foreach (Chunk chunk in bin.Chunks)
                    {
                        arrays = GetBAMOffsetArray(chunk.ChunkStart);
                        this.Write(arrays, 0, 8);
                        arrays = GetBAMOffsetArray(chunk.ChunkEnd);
                        this.Write(arrays, 0, 8);
                    }
                }

                //Add Meta Data - this varies by implementation, .NET Bio will do start and
                //end of reads found in file and then mapped/unmapped
                //TODO: Assumes little endian, only adds if so
                if (addingMetaData)
                {
                    //Dummy bin to indicate meta-data
                    arrays = Helper.GetLittleEndianByteArray(BAMIndexStorage.MaxBins);
                    this.Write(arrays, 0, 4);
                    //2 chunks worth of meta data
                    //first the file offsets
                    arrays = Helper.GetLittleEndianByteArray((int)2);
                    this.Write(arrays, 0, 4);
                    arrays = GetBAMOffsetArray(index.FirstOffSetSeen);
                    this.Write(arrays, 0, 8);
                    arrays = GetBAMOffsetArray(index.LastOffSetSeen);
                    this.Write(arrays, 0, 8);
                    arrays = BitConverter.GetBytes(index.MappedReadsCount);
                    this.Write(arrays, 0, 8);
                    arrays = BitConverter.GetBytes(index.UnMappedReadsCount);
                    this.Write(arrays, 0, 8);
                }
                
                arrays = Helper.GetLittleEndianByteArray(index.LinearIndex.Count);
                this.Write(arrays, 0, 4);
                
                foreach (FileOffset value in index.LinearIndex)
                {
                    arrays = GetBAMOffsetArray(value);
                    this.Write(arrays, 0, 8);
                }
                Source.Flush();
            }
        }

        /// <summary>
        /// Returns BAMIndex instance by parsing BAM index source.
        /// </summary>
        public BAMIndex Read()
        {
            if (Source == null)
            {
                throw new InvalidOperationException(Properties.Resource.BAM_CantUseBAMIndexStreamDisposed);
            }

            BAMIndex bamIndex = new BAMIndex();
            byte[] arrays = new byte[20];

            Read(arrays, 0, 4);

            if (arrays[0] != 66 || arrays[1] != 65 || arrays[2] != 73 || arrays[3] != 1)
            {
                throw new FormatException(Properties.Resource.BAM_InvalidIndexFile);
            }
            Read(arrays, 0, 4);
            int n_ref = Helper.GetInt32(arrays, 0);
            for (Int32 refindex = 0; refindex < n_ref; refindex++)
            {
                BAMReferenceIndexes bamindices = new BAMReferenceIndexes();
                bamIndex.RefIndexes.Add(bamindices);
                Read(arrays, 0, 4);
                int n_bin = Helper.GetInt32(arrays, 0);
                for (Int32 binIndex = 0; binIndex < n_bin; binIndex++)
                {
                    Bin bin = new Bin();
                    Read(arrays, 0, 4);
                    bin.BinNumber = Helper.GetUInt32(arrays, 0);
                    Read(arrays, 0, 4);
                    int n_chunk = Helper.GetInt32(arrays, 0);
                    if (bin.BinNumber == MaxBins)//some groups use this to place meta-data, such as the picard toolkit and now SAMTools
                    {
                        //Meta data was later added in to the SAMTools specification
                        for (Int32 chunkIndex = 0; chunkIndex < n_chunk; chunkIndex++)
                        {
                            bamindices.HasMetaData = true;
                            Read(arrays, 0, 8);
                            bamindices.MappedReadsCount = Helper.GetUInt64(arrays, 0);
                            Read(arrays, 0, 8);
                            bamindices.UnMappedReadsCount = Helper.GetUInt64(arrays, 0);
                        }

                    }
                    else if (bin.BinNumber > MaxBins)
                    {
                        throw new Exception("BAM Index is incorrectly formatted.  Bin number specified is higher than the maximum allowed.");
                    }
                    else
                    {
                         bamindices.Bins.Add(bin);
                        for (Int32 chunkIndex = 0; chunkIndex < n_chunk; chunkIndex++)
                        {
                            Chunk chunk = new Chunk();
                            bin.Chunks.Add(chunk);
                            Read(arrays, 0, 8);
                            chunk.ChunkStart = GetBAMOffset(arrays, 0);
                            Read(arrays, 0, 8);
                            chunk.ChunkEnd = GetBAMOffset(arrays, 0);
                        }
                    }
                }
                //Get number of linear bins
                Read(arrays, 0, 4);
                int n_intv = Helper.GetInt32(arrays, 0);

                for (Int32 offsetIndex = 0; offsetIndex < n_intv; offsetIndex++)
                {
                    FileOffset value;
                    Read(arrays, 0, 8);
                    value = GetBAMOffset(arrays, 0);
                    bamindices.LinearIndex.Add(value);
                }
            }
            

            return bamIndex;
        }

        /// <summary>
        /// Disposes resources held by this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the underlying stream.
        /// </summary>
        /// <param name="disposing">If disposing equals true, Requests that the system not call the finalizer for this instance.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Source != null)
            {
                Source.Dispose();
                Source = null;
            }
        }

        // Converts bytes array to FileOffset object.
        private static FileOffset GetBAMOffset(byte[] bytes, int startIndex)
        {
            UInt64 value = bytes[startIndex + 7];
            value = (value << 8) + bytes[startIndex + 6];
            value = (value << 8) + bytes[startIndex + 5];
            value = (value << 8) + bytes[startIndex + 4];
            value = (value << 8) + bytes[startIndex + 3];
            value = (value << 8) + bytes[startIndex + 2];
            UInt16 uvalue = bytes[startIndex + 1];
            uvalue = (UInt16)((UInt16)(uvalue << 8) + (UInt16)bytes[startIndex]);
            
            return new FileOffset(value,uvalue);
        }

        // Converts FileOffset object to byte array.
        private static byte[] GetBAMOffsetArray(FileOffset offset)
        {
            byte[] bytes = new byte[8];

            bytes[0] = (byte)(offset.UncompressedBlockOffset & 0x00FF);
            bytes[1] = (byte)((offset.UncompressedBlockOffset & 0xFF00) >> 8);

            bytes[2] = (byte)(offset.CompressedBlockOffset & 0x0000000000FF);
            bytes[3] = (byte)((offset.CompressedBlockOffset & 0x00000000FF00) >> 8);
            bytes[4] = (byte)((offset.CompressedBlockOffset & 0x000000FF0000) >> 16);
            bytes[5] = (byte)((offset.CompressedBlockOffset & 0x0000FF000000) >> 24);
            bytes[6] = (byte)((offset.CompressedBlockOffset & 0x00FF00000000) >> 32);
            bytes[7] = (byte)((offset.CompressedBlockOffset & 0xFF0000000000) >> 40);

            return bytes;
        }

        // Writes byte array to underlying stream of this instance.
        private void Write(byte[] array, int offset, int count)
        {
            if (Source == null)
            {
                throw new ObjectDisposedException("BAMIndex has been disposed");
            }

            Source.Write(array, offset, count);
        }

        // reads specified number of bytes from the underlying stream to specified array starting from specified offset.
        private void Read(byte[] array, int offset, int count)
        {
            if (Source == null)
            {
                throw new ObjectDisposedException("BAMIndex has been disposed");
            }

            if (IsEOF() || Source.Read(array, offset, count) != count)
            {
                throw new Exception(Properties.Resource.BAM_InvalidIndexFile+"\nCould not read the correct number of bytes from file");
            }
        }

        // Gets a boolean which indicates whether underlying stream reached EOF or not.
        private bool IsEOF()
        {
            return Source.Position == Source.Length;
        }

    }
}
