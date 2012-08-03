using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Bio.IO.FastA
{
    /// <summary>
    /// A FastaParser reads from a source of text that is formatted according to the FASTA flat
    /// file specification and converts the data to in-memory ISequence objects.  For advanced
    /// users, the ability to select an encoding for the internal memory representation is
    /// provided. There is also a default encoding for each alphabet that may be encountered.
    /// Documentation for the latest FastA file format can be found at
    /// http://www.ncbi.nlm.nih.gov/blast/fasta.shtml .
    /// </summary>
    public sealed class FastAParser : ISequenceParser
    {
        #region Member variables
        /// <summary>
        /// The Size is 1 KB.
        /// </summary>
        public const int KBytes = 1024;

        /// <summary>
        /// The Size is 1 MB.
        /// </summary>
        public const int MBytes = 1024 * KBytes;

        /// <summary>
        /// The Size is 1 GB.
        /// </summary>
        public const int GBytes = 1024 * MBytes;

        /// <summary>
        /// Buffer size.
        /// </summary>
        private const int BufferSize = 256 * MBytes;

        /// <summary>
        /// Maximum sequence length.
        /// </summary>
        private const long MaximumSequenceLength = (long)2 * GBytes;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the FastAParser class.
        /// </summary>
        public FastAParser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the FastAParser class by 
        /// loading the specified filename.
        /// </summary>
        /// <param name="filename">Name of the File.</param>
        public FastAParser(string filename)
        {
            this.Open(filename);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the type of parser.
        /// This is intended to give developers name of the parser.
        /// </summary>
        public string Name
        {
            get
            {
                return Properties.Resource.FastAName;
            }
        }

        /// <summary>
        /// Gets the description of the parser.
        /// This is intended to give developers some information 
        /// of the parser class. This property returns a simple description of what this
        ///  class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Properties.Resource.FASTAPARSER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets a comma separated values of the possible FastA
        /// file extensions.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Properties.Resource.FASTA_FILEEXTENSION;
            }
        }

        /// <summary>
        /// Gets or sets the alphabet to use for parsed ISequence objects.  If this is not set, the alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Opens the stream for the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to open.</param>
        public void Open(string filename)
        {
            // if the file is alread open throw invalid 
            if (!string.IsNullOrEmpty(this.Filename))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.FileAlreadyOpen, this.Filename));
            }

            // Validate the file - by try to open.
            using (new StreamReader(filename))
            {
            }

            this.Filename = filename;
        }

        /// <summary>
        /// Returns an IEnumerable of sequences in the file being parsed.
        /// </summary>
        /// <returns>Returns ISequence arrays.</returns>
        public IEnumerable<ISequence> Parse()
        {
            byte[] buffer = new byte[BufferSize];
            using (StreamReader reader = new StreamReader(this.Filename))
            {
                do
                {
                    yield return this.ParseOne(reader, buffer);
                }
                while (!reader.EndOfStream);
            }
        }

        /// <summary>
        /// Returns an IEnumerable of sequences in the stream being parsed.
        /// </summary>
        /// <param name="reader">Stream to parse.</param>
        /// <returns>Returns ISequence arrays.</returns>
        public IEnumerable<ISequence> Parse(StreamReader reader)
        {
            byte[] buffer = new byte[BufferSize];
            do
            {
                yield return this.ParseOne(reader, buffer);
            }
            while (!reader.EndOfStream);
        }

        /// <summary>
        /// Returns an IEnumerable of sequences in the stream being parsed.
        /// </summary>
        /// <param name="reader">Stream to parse.</param>
        /// <param name="buffer">Buffer to use.</param>
        /// <returns>Returns a Sequence.</returns>
        public ISequence ParseOne(StreamReader reader, byte[] buffer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            IAlphabet alphabet = this.Alphabet;
            IAlphabet baseAlphabet = null;
            int currentBufferSize = BufferSize;

            bool skipBlankLine = true;
            string message = string.Empty;

            bool tryAutoDetectAlphabet;
            if (alphabet == null)
            {
                tryAutoDetectAlphabet = true;
            }
            else
            {
                tryAutoDetectAlphabet = false;
            }

            if (reader.EndOfStream)
            {
                message = string.Format(
                            CultureInfo.InvariantCulture,
                            Bio.Properties.Resource.INVALID_INPUT_FILE,
                            Bio.Properties.Resource.FASTA_NAME);

                throw new FileFormatException(message);
            }

            string line = reader.ReadLine();

            // Continue reading if blank line found.
            while (skipBlankLine && line != null && string.IsNullOrEmpty(line))
            {
                line = reader.ReadLine();
            }

            if (line == null || !line.StartsWith(">", StringComparison.OrdinalIgnoreCase))
            {
                message = string.Format(
                        CultureInfo.InvariantCulture,
                        Bio.Properties.Resource.INVALID_INPUT_FILE,
                        Bio.Properties.Resource.FASTA_NAME);

                throw new FileFormatException(message);
            }

            string name = line.Substring(1);
            int bufferPosition = 0;

            if (tryAutoDetectAlphabet)
            {
                alphabet = baseAlphabet;
            }

            // Read next line.
            line = reader.ReadLine();

            // Continue reading if blank line found.
            while (skipBlankLine && line != null && string.IsNullOrEmpty(line))
            {
                line = reader.ReadLine();
            }

            if (line == null)
            {
                message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resource.InvalidSymbolInString,
                    string.Empty);
                throw new FileFormatException(message);
            }

            do
            {
                // For large files copy the data in memory mapped file.
                if ((((long)bufferPosition + line.Length) >= MaximumSequenceLength))
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format(CultureInfo.CurrentUICulture, Properties.Resource.SequenceDataGreaterthan2GB, name));
                }

                if (((bufferPosition + line.Length) >= currentBufferSize))
                {
                    Array.Resize<byte>(ref buffer, buffer.Length + BufferSize);
                    currentBufferSize += BufferSize;
                }

                byte[] symbols = UTF8Encoding.UTF8.GetBytes(line);

                // Array.Copy -- for performance improvement.
                Array.Copy(symbols, 0, buffer, bufferPosition, symbols.Length);

                // Auto detect alphabet if alphabet is set to null, else validate with already set alphabet
                if (tryAutoDetectAlphabet)
                {
                    alphabet = Alphabets.AutoDetectAlphabet(buffer, bufferPosition, bufferPosition + line.Length, alphabet);
                    if (alphabet == null)
                    {
                        throw new FileFormatException(string.Format(CultureInfo.InvariantCulture, Properties.Resource.InvalidSymbolInString, line));
                    }
                }
                else if (this.Alphabet != null)
                {
                    if (!this.Alphabet.ValidateSequence(buffer, bufferPosition, bufferPosition + line.Length))
                    {
                        throw new FileFormatException(string.Format(CultureInfo.InvariantCulture, Properties.Resource.InvalidSymbolInString, line));
                    }
                }

                bufferPosition += line.Length;

                if (reader.Peek() == (byte)'>')
                {
                    break;
                }
                
                // Read next line.
                line = reader.ReadLine();

                // Continue reading if blank line found.
                while (skipBlankLine && line != null && string.IsNullOrEmpty(line) && reader.Peek() != (byte)'>')
                {
                    line = reader.ReadLine();
                }
            }
            while (line != null);

            // Truncate buffer to remove trailing 0's
            byte[] tmpBuffer = new byte[bufferPosition];
            Array.Copy(buffer, tmpBuffer, bufferPosition);

            Sequence sequence = null;

            if (tryAutoDetectAlphabet)
            {
                IAlphabet tmpalphabet = alphabet;
                IAlphabet tmpbaseAlphabet = null;
                while (Alphabets.AlphabetToBaseAlphabetMap.TryGetValue(tmpalphabet, out tmpbaseAlphabet))
                {
                    tmpalphabet = tmpbaseAlphabet;
                }

                if (tmpbaseAlphabet == null)
                {
                    tmpbaseAlphabet = tmpalphabet;
                }

                if (baseAlphabet == null)
                {
                    baseAlphabet = tmpbaseAlphabet;
                }

                if (tmpbaseAlphabet != baseAlphabet)
                {
                    throw new FileFormatException(Properties.Resource.FastAContainsMorethanOnebaseAlphabet);
                }
            }

            // In memory sequence
            sequence = new Sequence(alphabet, tmpBuffer, false);
            sequence.ID = name;
            return sequence;
        }

        /// <summary>
        /// Closes streams used.
        /// </summary>
        public void Close()
        {
            this.Filename = null;
        }

        /// <summary>
        /// Disposes the underlying stream.
        /// </summary>
        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
