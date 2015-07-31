using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Bio.Extensions;

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
    public class FastAParser : ISequenceParser
    {
        private IAlphabet baseAlphabet;

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

        /// <summary>
        /// Returns an IEnumerable of sequences in the stream being parsed.
        /// </summary>
        /// <param name="stream">Stream to parse.</param>
        /// <returns>Returns ISequence arrays.</returns>
        public IEnumerable<ISequence> Parse(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            byte[] buffer = new byte[PlatformManager.Services.DefaultBufferSize];
            using (var reader = stream.OpenRead())
            {
                while (!reader.EndOfStream)
                {
                    var seq = this.ParseOne(reader, buffer);
                    if (seq != null)
                        yield return seq;
                }
            }
        }

        /// <summary>
        /// Returns an IEnumerable of sequences in the stream being parsed.
        /// </summary>
        /// <param name="stream">Stream to parse.</param>
        /// <returns>Returns a Sequence.</returns>
        public ISequence ParseOne(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            byte[] buffer = new byte[PlatformManager.Services.DefaultBufferSize];
            using (var reader = stream.OpenRead())
            {
                return this.ParseOne(reader, buffer);
            }
        }

        /// <summary>
        /// Returns an IEnumerable of sequences in the stream being parsed.
        /// </summary>
        /// <param name="reader">Stream to parse.</param>
        /// <param name="buffer">Buffer to use.</param>
        /// <returns>Returns a Sequence.</returns>
        ISequence ParseOne(TextReader reader, byte[] buffer)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.Peek() == -1)
                return null;

            int currentBufferSize = PlatformManager.Services.DefaultBufferSize;

            string message;
            string line = reader.ReadLine();

            // Continue reading if blank line found.
            while (line != null && string.IsNullOrEmpty(line))
            {
                line = reader.ReadLine();
            }

            if (line == null || !line.StartsWith(">", StringComparison.OrdinalIgnoreCase))
            {
                message = string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resource.INVALID_INPUT_FILE,
                        Properties.Resource.FASTA_NAME);

                throw new Exception(message);
            }

            string name = line.Substring(1);
            int bufferPosition = 0;

            // Read next line.
            line = reader.ReadLine();

            // Continue reading if blank line found.
            while (line != null && string.IsNullOrEmpty(line))
            {
                line = reader.ReadLine();
            }

            if (line == null)
            {
                message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resource.InvalidSymbolInString,
                    string.Empty);
                throw new Exception(message);
            }

            IAlphabet alphabet = Alphabet;
            bool tryAutoDetectAlphabet = alphabet == null;

            do
            {
                // Files > 2G are not supported in this release.
                if ((((long)bufferPosition + line.Length) >= PlatformManager.Services.MaxSequenceSize))
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format(CultureInfo.CurrentUICulture, Properties.Resource.SequenceDataGreaterthan2GB, name));
                }
                int neededSize = bufferPosition + line.Length;
                if (neededSize >= currentBufferSize)
                {
                    //Grow file dynamically, by buffer size, or if too small to fit the new sequence by the size of the sequence
                    int suggestedSize = buffer.Length + PlatformManager.Services.DefaultBufferSize;
                    int newSize = neededSize < suggestedSize ? suggestedSize : neededSize;
                    Array.Resize(ref buffer, newSize);
                    currentBufferSize =newSize;
                }

                byte[] symbols = Encoding.UTF8.GetBytes(line);

                // Array.Copy -- for performance improvement.
                Array.Copy(symbols, 0, buffer, bufferPosition, symbols.Length);

                // Auto detect alphabet if alphabet is set to null, else validate with already set alphabet
                if (tryAutoDetectAlphabet)
                {
                    // If we have a base alphabet we detected earlier, 
                    // then try that first.
                    if (this.baseAlphabet != null &&
                        this.baseAlphabet.ValidateSequence(buffer, bufferPosition, bufferPosition + line.Length))
                    {
                        alphabet = this.baseAlphabet;
                    }
                    // Otherwise attempt to identify alphabet
                    else
                    {
                        // Different alphabet - try to auto detect.
                        this.baseAlphabet = null;
                        alphabet = Alphabets.AutoDetectAlphabet(buffer, bufferPosition, bufferPosition + line.Length, alphabet);
                        if (alphabet == null)
                        {
                            throw new Exception(string.Format(CultureInfo.InvariantCulture,
                                            Properties.Resource.InvalidSymbolInString, line));
                        }
                    }

                    // Determine the base alphabet used.
                    if (this.baseAlphabet == null)
                    {
                        this.baseAlphabet = alphabet;
                    }
                    else
                    {
                        // If they are not the same, then this might be an error.
                        if (this.baseAlphabet != alphabet)
                        {
                            // If the new alphabet includes all the base alphabet then use it instead.
                            // This happens when we hit an ambiguous form of the alphabet later in the file.
                            if (!this.baseAlphabet.HasAmbiguity && Alphabets.GetAmbiguousAlphabet(this.baseAlphabet) == alphabet)
                            {
                                this.baseAlphabet = alphabet;
                            }
                            else if (alphabet.HasAmbiguity || Alphabets.GetAmbiguousAlphabet(alphabet) != this.baseAlphabet)
                            {
                                throw new Exception(Properties.Resource.FastAContainsMorethanOnebaseAlphabet);
                            }
                        }
                    }
                }
                else
                {
                    // Validate against supplied alphabet.
                    if (!alphabet.ValidateSequence(buffer, bufferPosition, bufferPosition + line.Length))
                    {
                        throw new Exception(string.Format(CultureInfo.InvariantCulture, Properties.Resource.InvalidSymbolInString, line));
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
                while (line != null && string.IsNullOrEmpty(line) && reader.Peek() != (byte)'>')
                {
                    line = reader.ReadLine();
                }
            }
            while (line != null);

            // Truncate buffer to remove trailing 0's
            byte[] tmpBuffer = new byte[bufferPosition];
            Array.Copy(buffer, tmpBuffer, bufferPosition);

            if (tryAutoDetectAlphabet)
            {
                alphabet = this.baseAlphabet;
            }

            // In memory sequence
            return new Sequence(alphabet, tmpBuffer, false) {ID = name};
        }
    }
}
