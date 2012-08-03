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
    /// As the id of delta alignment itself is the location in the file, 
    /// using DeltaAlignmentParser and FastASequencePositionParser this class gets the deltaalignment on demand.
    /// </summary>
    public class DeltaAlignmentCollection : IDisposable
    {
        /// <summary>
        /// Bytes required per delta alignment record.
        /// </summary>
        private const int BytesPerRecord = 8;

        /// <summary>
        /// DeltaAlignment parser.
        /// </summary>
        private DeltaAlignmentParser deltaAlignmentParser;

        /// <summary>
        /// FastASequencePositionParser instance.
        /// </summary>
        private FastASequencePositionParser fastASequencePositionParser;

        /// <summary>
        /// File required to store the content of this collection.
        /// </summary>
        private string collectionFile;

        /// <summary>
        /// FileStream to read collectionFile.
        /// </summary>
        private FileStream collectionFileReader;

        /// <summary>
        /// Buffer to use while reading collectionFile.
        /// </summary>
        private byte[] readBuffer = new byte[BytesPerRecord];

        /// <summary>
        /// Flag to indicate whether disposing this instance should dispose FastASequencePositionParser or not.
        /// </summary>
        private bool disposeFastASequencePositionParser = true;

        /// <summary>
        /// Initializes a new instance of the DeltaAlignmentCollection class.
        /// </summary>
        /// <param name="deltaAlignmentFilename">Delta alignment file name.</param>
        /// <param name="readsFilename">Query/Reads filename.</param>
        public DeltaAlignmentCollection(string deltaAlignmentFilename, string readsFilename)
        {
            this.DeltaAlignmentFilename = deltaAlignmentFilename;
            this.QueryFilename = readsFilename;
            this.fastASequencePositionParser = new FastASequencePositionParser(this.QueryFilename, true);
            this.deltaAlignmentParser = new DeltaAlignmentParser(this.DeltaAlignmentFilename, this.fastASequencePositionParser);
            this.collectionFile = Guid.NewGuid().ToString();
            this.collectionFileReader = new FileStream(this.collectionFile, FileMode.Create, FileAccess.ReadWrite);
            this.LoadAllFromFile();
        }

        /// <summary>
        /// Initializes a new instance of the DeltaAlignmentCollection class.
        /// </summary>
        /// <param name="deltaAlignmentFilename">Delta alignment file name.</param>
        /// <param name="fastASequencePositionParser">Query/Reads filename.</param>
        public DeltaAlignmentCollection(string deltaAlignmentFilename, FastASequencePositionParser fastASequencePositionParser)
        {
            if (fastASequencePositionParser == null)
            {
                throw new ArgumentNullException("fastASequencePositionParser");
            }
            this.disposeFastASequencePositionParser = false;
            this.DeltaAlignmentFilename = deltaAlignmentFilename;
            this.QueryFilename = fastASequencePositionParser.Filename;
            this.fastASequencePositionParser = fastASequencePositionParser;
            this.deltaAlignmentParser = new DeltaAlignmentParser(this.DeltaAlignmentFilename, this.fastASequencePositionParser);
            this.collectionFile = Guid.NewGuid().ToString();
            this.collectionFileReader = new FileStream(this.collectionFile, FileMode.Create, FileAccess.ReadWrite);
            this.LoadAllFromFile();
        }

        /// <summary>
        /// Finalizes an instance of the DeltaAlignmentCollection class.
        /// </summary>
        ~DeltaAlignmentCollection()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the no of delta alignment present in this collection.
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// Gets Query or read file name.
        /// </summary>
        public string QueryFilename { get; private set; }

        /// <summary>
        /// Gets Delta alignments file name.
        /// </summary>
        public string DeltaAlignmentFilename { get; private set; }

        /// <summary>
        /// Gets the DeltaAlignment parser.
        /// </summary>
        public DeltaAlignmentParser DeltaAlignmentParser
        {
            get
            {
                return this.deltaAlignmentParser;
            }
        }

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

                if (this.collectionFileReader.Position != positionToSeek)
                {
                    this.collectionFileReader.Position = positionToSeek;
                }

                if (this.collectionFileReader.Read(this.readBuffer, 0, BytesPerRecord) != BytesPerRecord)
                {
                    throw new ArgumentException(Properties.Resource.DeltaCollectionFileCorrupted);
                }

                long position = BitConverter.ToInt64(this.readBuffer, 0);
                return this.deltaAlignmentParser.GetDeltaAlignmentAt(position);
            }
        }

        /// <summary>
        /// Gets Delta alignments grouped by read sequence id.
        /// </summary>
        public IEnumerable<List<DeltaAlignment>> GetDeltaAlignmentsByReads()
        {
            List<DeltaAlignment> list = null;
            string previousQueryID = string.Empty;
            foreach (DeltaAlignment delta in this.deltaAlignmentParser.Parse())
            {
                if (previousQueryID != delta.QuerySequence.ID)
                {
                    if (list != null)
                    {
                        yield return list;
                    }

                    list = new List<DeltaAlignment>();
                }

                list.Add(delta);
            }
        }

        /// <summary>
        /// Gets the list of delta alignments for the specified sequence id.
        /// </summary>
        /// <param name="sequenceId">Sequence id.</param>
        public List<DeltaAlignment> GetDeltaAlignmentFor(string sequenceId)
        {
            string fullSequenceId = string.Empty;
            List<DeltaAlignment> list = new List<DeltaAlignment>();
            long deltaId = this.GetDeltaAlignmentIDFor(sequenceId, out fullSequenceId);
            if (deltaId == -1)
            {
                return list;
            }

            foreach (DeltaAlignment delta in this.deltaAlignmentParser.ParseFrom(deltaId))
            {
                if (fullSequenceId != delta.QuerySequence.ID)
                {
                    return list;
                }

                list.Add(delta);
            }

            return list;
        }

        /// <summary>
        /// Disposes the underlaying streams used.
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
                if (this.deltaAlignmentParser != null)
                {
                    this.deltaAlignmentParser.Dispose();
                    this.deltaAlignmentParser = null;
                }

                if (this.fastASequencePositionParser != null)
                {
                    if (disposeFastASequencePositionParser)
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

            // Delete the collection file on Dispose or GC.
            if (File.Exists(this.collectionFile))
            {
                File.Delete(this.collectionFile);
            }
        }

        /// <summary>
        /// Loads this collection with delta alignment from file.
        /// </summary>
        private void LoadAllFromFile()
        {
            this.collectionFileReader.Seek(0, SeekOrigin.Begin);
            this.Count = 0;
            byte[] bytes;

            foreach (long position in this.deltaAlignmentParser.GetPositions())
            {
                bytes = BitConverter.GetBytes(position);
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
        private long GetDeltaAlignmentIDFor(string sequenceId, out string fullSequenceId)
        {
            foreach (var data in this.deltaAlignmentParser.GetQuerySeqIds())
            {
                string id = data.Item2;
                int index = id.LastIndexOf(Helper.PairedReadDelimiter);
                if (index > 0)
                {
                    id = id.Substring(0, index);
                }

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
