using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bio.IO.FastA;

namespace Bio.Util
{
    /// <summary>
    /// This class is similar to FastAParser except that this class appends the
    /// position of the sequence parsed to its id.
    /// </summary>
    public class FastASequencePositionParser : IDisposable
    {
        /// <summary>
        /// Buffer size.
        /// </summary>
        private const int BufferSize = 256 * 1024 * 1024;

        /// <summary>
        /// An instance of FastAParser.
        /// </summary>
        private FastAParser fastaParser;

        /// <summary>
        /// Buffer to use in GetSequenceAt method.
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// StreamReader to use in GetSequenceAt method.
        /// </summary>
        private StreamReader reader;

        /// <summary>
        /// Sequence cache to hold sequences.
        /// </summary>
        private SequenceCache sequenceCache;

        /// <summary>
        /// Flag to indicate to get the forward strand sequence of a reverse paired read.
        /// </summary>
        private bool reverseReversePairedRead = false;
        /// <summary>
        /// Initializes a new instance of the FastASequencePositionParser class.
        /// </summary>
        public FastASequencePositionParser()
        {
            this.fastaParser = new FastAParser();
        }

        /// <summary>
        /// Initializes a new instance of the FastASequencePositionParser class by 
        /// loading the specified filename.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public FastASequencePositionParser(string filename)
        {
            this.fastaParser = new FastAParser(filename);
            this.Filename = filename;
        }

        /// <summary>
        /// Initializes a new instance of the FastASequencePositionParser class by 
        /// loading the specified filename.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        /// <param name="reverseReversePairedRead">Flag to indicate to reversecomplement reversePairedRead.</param>
        public FastASequencePositionParser(string filename, bool reverseReversePairedRead)
        {
            this.fastaParser = new FastAParser(filename);
            this.Filename = filename;
            this.reverseReversePairedRead = reverseReversePairedRead;
        }

        /// <summary>
        /// Finalizes an instance of the FastASequencePositionParser class.
        /// </summary>
        ~FastASequencePositionParser()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the sequences are cached or not.
        /// </summary>
        public bool SequencesCached { get; private set; }

        /// <summary>
        /// Gets or sets the alphabet to use for parsed ISequence objects.  If this is not set, the alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet
        {
            get
            {
                return this.fastaParser.Alphabet;
            }

            set
            {
                this.fastaParser.Alphabet = value;
            }
        }

        /// <summary>
        /// Returns an IEnumerable of sequences in the file being parsed.
        /// </summary>
        /// <returns>Returns ISequence arrays.</returns>
        public IEnumerable<ISequence> Parse()
        {
            if (this.SequencesCached)
            {
                return this.sequenceCache.GetAllSequences();
            }
            else
            {
                return this.ParseFromFile();
            }
        }

        /// <summary>
        /// Loads sequences to cache.
        /// This method will ignore the call if sequences are already cached.
        /// </summary>
        public void CacheSequencesForRandomAccess()
        {
            if (this.SequencesCached)
            {
                return;
            }

            this.sequenceCache = new SequenceCache();

            foreach (ISequence sequence in this.Parse())
            {
                long position = long.Parse(sequence.ID.Substring(sequence.ID.LastIndexOf('@') + 1), CultureInfo.InvariantCulture);
                this.sequenceCache.Add(position, sequence);
            }

            if (this.sequenceCache.Count > 0)
            {
                this.SequencesCached = true;
            }
        }

        /// <summary>
        /// Gets the sequence at specified position.
        /// </summary>
        /// <param name="position">Start postion of the sequence required in the file.</param>
        /// <returns>Sequence present at the specified position.</returns>
        public ISequence GetSequenceAt(long position)
        {
            if (this.SequencesCached)
            {
                return this.sequenceCache.GetSequenceAt(position);
            }

            if (this.buffer == null)
            {
                this.buffer = new byte[BufferSize];
            }

            if (this.reader == null)
            {
                this.reader = new StreamReader(this.Filename);
            }

            this.reader.BaseStream.Position = position;
            this.reader.DiscardBufferedData();

            ISequence sequence = this.fastaParser.ParseOne(this.reader, this.buffer);
            string delim = "@";
            if (sequence.ID.LastIndexOf(Helper.PairedReadDelimiter) == -1)
            {
                delim = "!@";
            }
            
            if (this.reverseReversePairedRead)
            {
                string originalSequenceId;
                bool forwardRead;
                string pairedReadType;
                string libraryName;
                bool pairedRead = Helper.ValidatePairedSequenceId(sequence.ID, out originalSequenceId, out forwardRead, out pairedReadType, out libraryName);
                if (pairedRead && !forwardRead)
                {
                    sequence = sequence.GetReverseComplementedSequence();
                }
            }

            sequence.ID += delim + position.ToString(CultureInfo.InvariantCulture);

            return sequence;
        }

        /// <summary>
        /// Closes streams used.
        /// </summary>
        public void Close()
        {
            this.Filename = null;

            if (this.fastaParser != null)
            {
                this.fastaParser.Close();
                this.fastaParser = null;
            }

            if (this.reader != null)
            {
                this.reader.Close();
                this.reader = null;
            }
        }

        /// <summary>
        /// Disposes the underlying stream.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the underlying stream.
        /// </summary>
        /// <param name="disposing">Flag to indicate whether disposing or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.Close();

            if (disposing)
            {
                this.buffer = null;
                this.sequenceCache = null;
                this.SequencesCached = false;
            }
        }

        /// <summary>
        /// Parses sequences from the file.
        /// </summary>
        private IEnumerable<ISequence> ParseFromFile()
        {
            using (FileStream stream = new FileStream(this.Filename, FileMode.Open, FileAccess.Read))
            {
                IEnumerable<ISequence> sequences = this.fastaParser.Parse();
                IEnumerator<long> positions = GetNextSequenceStartPosition(stream).GetEnumerator();

                foreach (ISequence sequence in sequences)
                {
                    ISequence seq = sequence;
                    long position = -1;
                    if (positions.MoveNext())
                    {
                        position = positions.Current;
                    }

                    string delim = "@";
                    if (seq.ID.LastIndexOf(Helper.PairedReadDelimiter) == -1)
                    {
                        delim = "!@";
                    }
                    
                    if (this.reverseReversePairedRead)
                    {
                        string originalSequenceId;
                        bool forwardRead;
                        string pairedReadType;
                        string libraryName;
                        bool pairedRead = Helper.ValidatePairedSequenceId(seq.ID, out originalSequenceId, out forwardRead, out pairedReadType, out libraryName);
                        if (pairedRead && !forwardRead)
                        {
                            seq = seq.GetReverseComplementedSequence();
                        }
                    }

                    seq.ID = seq.ID + delim + position.ToString();

                    yield return seq;
                }
            }
        }

        /// <summary>
        /// Gets the next sequence start position in the file.
        /// </summary>
        /// <param name="stream">FastA file stream.</param>
        /// <returns>Position of the next sequence in the stream.</returns>
        private static IEnumerable<long> GetNextSequenceStartPosition(FileStream stream)
        {
            // 4k at a time.
            byte[] readBuffer = new byte[4 * 1024];
            int lastReadLength = 0;
            int startIndex = 0;
            byte startChar = (byte)'>';
            long position = 0;
            while (stream.Position != stream.Length)
            {
                if (startIndex >= lastReadLength)
                {
                    startIndex = 0;
                    position += lastReadLength;
                    lastReadLength = stream.Read(readBuffer, 0, 4 * 1024);
                }

                while (startIndex < lastReadLength)
                {
                    if (readBuffer[startIndex++] == startChar)
                    {
                        yield return position + startIndex - 1;
                    }
                }
            }
        }

        /// <summary>
        /// Class to hold sequences.
        /// </summary>
        private class SequenceCache
        {
            private int sequenceLength = 1;
            private int bucketSize = 1000;
            private List<SequenceHolder> buckets = new List<SequenceHolder>();

            /// <summary>
            /// Gets total sequences present in this instance.
            /// </summary>
            public long Count { get; private set; }

            /// <summary>
            /// Adds the specified sequence with position.
            /// </summary>
            /// <param name="position">Position of the sequence in file.</param>
            /// <param name="sequence">Sequence to cache.</param>
            public void Add(long position, ISequence sequence)
            {
                if (this.Count == 0)
                {
                    this.sequenceLength = (int)sequence.Count;
                }

                int index = (int)(position / (sequenceLength * bucketSize));
                SequenceHolder newHolder = new SequenceHolder() { Position = position, Sequence = sequence };

                if (index >= buckets.Count)
                {
                    while (index >= buckets.Count)
                    {
                        buckets.Add(null);
                    }

                    buckets[index] = newHolder;
                    this.Count++;
                    return;
                }

                SequenceHolder holder = buckets[index];
                if (holder == null)
                {
                    buckets[index] = newHolder;
                    this.Count++;
                    return;
                }

                if (holder.Position > position)
                {
                    buckets[index] = newHolder;
                    buckets[index].Next = holder;
                }
                else
                {
                    while (holder.Next != null)
                    {
                        if (holder.Next.Position > position)
                        {
                            break;
                        }

                        holder = holder.Next;
                    }

                    newHolder.Next = holder.Next;
                    holder.Next = newHolder;
                }

                this.Count++;
            }

            /// <summary>
            /// Gets the sequence for the specified position.
            /// </summary>
            /// <param name="position">Position.</param>
            public ISequence GetSequenceAt(long position)
            {
                int index = (int)(position / (sequenceLength * bucketSize));
                if (index >= buckets.Count)
                {
                    return null;
                }

                SequenceHolder holder = buckets[index];
                while (holder != null)
                {
                    if (holder.Position == position)
                    {
                        return holder.Sequence;
                    }

                    holder = holder.Next;
                }

                return null;
            }

            /// <summary>
            /// Gets all sequences present in this instance.
            /// </summary>
            public IEnumerable<ISequence> GetAllSequences()
            {
                for (int i = 0; i < buckets.Count; i++)
                {
                    SequenceHolder holder = buckets[i];
                    while (holder != null)
                    {
                        yield return holder.Sequence;
                        holder = holder.Next;
                    }
                }
            }

            /// <summary>
            /// Class to hold sequence along with its position.
            /// </summary>
            private class SequenceHolder
            {
                /// <summary>
                /// Position of the sequence in file.
                /// </summary>
                public long Position { get; set; }

                /// <summary>
                /// Reference to next SequenceHolder
                /// </summary>
                public SequenceHolder Next { get; set; }

                /// <summary>
                /// Sequence.
                /// </summary>
                public ISequence Sequence { get; set; }
            }
        }
    }
}
