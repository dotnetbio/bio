using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Bio.Algorithms.Alignment;

namespace Bio.Util
{
    /// <summary>
    /// This class provides indexer access to the DeltaAlignments stored in the specified delta alignment file. 
    /// This class uses a file to hold DeltaAlignment id's from the deltaAlignment file.
    /// As the id of delta alignment itself is the location in the file, using DeltaAlignmentParser 
    /// and FastASequencePositionParser this class gets the delta alignment on demand.
    /// </summary>
    public class DeltaAlignmentCollection : IDisposable
    {
        /// <summary>
        /// Bytes required per delta alignment record.
        /// </summary>
        private const int BytesPerRecord = 8;

        /// <summary>
        /// FastASequencePositionParser instance.
        /// </summary>
        private FastASequencePositionParser fastASequencePositionParser;

        /// <summary>
        /// FileStream to read collectionFile.
        /// </summary>
        private Stream collectionFileReader;

        /// <summary>
        /// Buffer to use while reading collectionFile.
        /// </summary>
        private byte[] readBuffer = new byte[BytesPerRecord];

        /// <summary>
        /// Flag to indicate whether disposing this instance should dispose FastASequencePositionParser or not.
        /// </summary>
        private readonly bool disposeFastASequencePositionParser = true;

        /// <summary>
        /// Initializes a new instance of the DeltaAlignmentCollection class.
        /// </summary>
        /// <param name="deltaAlignmentStream">Delta alignment stream.</param>
        /// <param name="readsStream">Query/Reads stream.</param>
        public DeltaAlignmentCollection(Stream deltaAlignmentStream, Stream readsStream)
        {
            this.DeltaAlignmentStream = deltaAlignmentStream;
            this.QueryStream = readsStream;
            this.fastASequencePositionParser = new FastASequencePositionParser(this.QueryStream, true);
            this.DeltaAlignmentParser = new DeltaAlignmentParser(this.DeltaAlignmentStream, this.fastASequencePositionParser);
            this.collectionFileReader = PlatformManager.Services.CreateTempStream();
            this.LoadAllFromFile();
        }

        /// <summary>
        /// Initializes a new instance of the DeltaAlignmentCollection class.
        /// </summary>
        /// <param name="deltaAlignmentStream">Delta alignment stream.</param>
        /// <param name="fastASequencePositionParser">Query/Reads filename.</param>
        public DeltaAlignmentCollection(Stream deltaAlignmentStream, FastASequencePositionParser fastASequencePositionParser)
        {
            if (fastASequencePositionParser == null)
                throw new ArgumentNullException("fastASequencePositionParser");
            
            this.disposeFastASequencePositionParser = false;
            this.DeltaAlignmentStream = deltaAlignmentStream;
            this.QueryStream = fastASequencePositionParser.Stream;
            this.fastASequencePositionParser = fastASequencePositionParser;
            this.DeltaAlignmentParser = new DeltaAlignmentParser(this.DeltaAlignmentStream, this.fastASequencePositionParser);
            this.collectionFileReader = PlatformManager.Services.CreateTempStream();
            this.LoadAllFromFile();
        }

        /// <summary>
        /// Gets the no of delta alignment present in this collection.
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// Gets Query or read file name.
        /// </summary>
        public Stream QueryStream { get; private set; }

        /// <summary>
        /// Gets Delta alignments file name.
        /// </summary>
        public Stream DeltaAlignmentStream { get; private set; }

        /// <summary>
        /// Gets the DeltaAlignment parser.
        /// </summary>
        public DeltaAlignmentParser DeltaAlignmentParser { get; private set; }

        /// <summary>
        /// Gets the Delta alignment present at the specified index.
        /// </summary>
        /// <param name="index">Index at which delta alignment is required.</param>
        /// <returns>Delta alignment.</returns>
        public DeltaAlignment this[long index]
        {
            get
            {
                if (index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                long positionToSeek = index * BytesPerRecord;
                this.collectionFileReader.Position = positionToSeek;

                if (this.collectionFileReader.Read(this.readBuffer, 0, BytesPerRecord) != BytesPerRecord)
                    throw new ArgumentException(Properties.Resource.DeltaCollectionFileCorrupted);

                long position = BitConverter.ToInt64(this.readBuffer, 0);
                return this.DeltaAlignmentParser.GetDeltaAlignmentAt(position);
            }
        }

        /// <summary>
        /// Gets Delta alignments grouped by read sequence id.
        /// </summary>
        public IEnumerable<List<DeltaAlignment>> GetDeltaAlignmentsByReads()
        {
            List<DeltaAlignment> list = null;
            string previousQueryId = string.Empty;
            foreach (DeltaAlignment delta in this.DeltaAlignmentParser.Parse())
            {
                if (previousQueryId != delta.QuerySequence.ID)
                {
                    if (list != null)
                        yield return list;
                    list = new List<DeltaAlignment>();
                }

                if (list == null)
                    list = new List<DeltaAlignment>();
                
                list.Add(delta);
            }
        }

        /// <summary>
        /// Gets the list of delta alignments for the specified sequence id.
        /// </summary>
        /// <param name="sequenceId">Sequence id.</param>
        public List<DeltaAlignment> GetDeltaAlignmentFor(string sequenceId)
        {
            string fullSequenceId;
            var list = new List<DeltaAlignment>();

            long deltaId = this.GetDeltaAlignmentIdFor(sequenceId, out fullSequenceId);
            if (deltaId == -1)
                return list;

            foreach (DeltaAlignment delta in this.DeltaAlignmentParser.ParseFrom(deltaId))
            {
                if (fullSequenceId != delta.QuerySequence.ID)
                    return list;

                list.Add(delta);
            }
            return list;
        }

        /// <summary>
        /// Disposes the underlying streams used.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the underlying streams used.
        /// </summary>
        /// <param name="disposing">Flag to indicate whether it is called from dispose method or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.DeltaAlignmentParser != null)
                {
                    this.DeltaAlignmentParser.Dispose();
                    this.DeltaAlignmentParser = null;
                }

                if (this.fastASequencePositionParser != null)
                {
                    if (this.disposeFastASequencePositionParser)
                    {
                        this.fastASequencePositionParser.Dispose();
                    }
                    this.fastASequencePositionParser = null;
                }

                if (this.collectionFileReader != null)
                {
                    this.collectionFileReader.Dispose();
                    this.collectionFileReader = null;
                }

                this.readBuffer = null;
            }
        }

        /// <summary>
        /// Loads this collection with delta alignment from file.
        /// </summary>
        private void LoadAllFromFile()
        {
            this.collectionFileReader.Seek(0, SeekOrigin.Begin);
            this.Count = 0;

            foreach (long position in this.DeltaAlignmentParser.GetPositions())
            {
                byte[] bytes = BitConverter.GetBytes(position);
                this.collectionFileReader.Write(bytes, 0, BytesPerRecord);
                this.Count++;
            }

            this.collectionFileReader.Flush();
            this.collectionFileReader.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Gets the first delta alignment's id for the specified sequence id.
        /// </summary>
        /// <param name="sequenceId">Sequence id.</param>
        /// <param name="fullSequenceId">Full id of the sequence id.</param>
        /// <returns>Delta alignment id.</returns>
        private long GetDeltaAlignmentIdFor(string sequenceId, out string fullSequenceId)
        {
            foreach (var data in this.DeltaAlignmentParser.GetQuerySeqIds())
            {
                string id = data.Item2;
                int index = id.LastIndexOf(Helper.PairedReadDelimiter);
                if (index > 0)
                    id = id.Substring(0, index);

                if (id == sequenceId)
                {
                    fullSequenceId = data.Item2;
                    return long.Parse(data.Item1, CultureInfo.InvariantCulture);
                }
            }

            fullSequenceId = string.Empty;
            return -1;
        }
    }
}
