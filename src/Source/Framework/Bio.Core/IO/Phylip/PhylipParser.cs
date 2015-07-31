using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Bio.Algorithms.Alignment;
using Bio.Extensions;

namespace Bio.IO.Phylip
{
    /// <summary>
    /// A PhylipParser reads from a source of text that is formatted according 
    /// to the PhylipParser flat file specification, and converts the data to 
    /// in-memory ISequenceAlignment objects.
    /// </summary>
    public class PhylipParser : ISequenceAlignmentParser
    {
        /// <summary>
        /// Indicates that the parser should skip any blank line while reading the stream.
        /// </summary>
        private bool skipBlankLines = true;

        /// <summary>
        /// Stores the last line read by the reader
        /// </summary>
        private string line;

        /// <summary>
        /// Gets the name of the sequence alignment parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser type.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.PHYLIP_NAME; }
        }

        /// <summary>
        /// Gets the description of the sequence alignment parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.PHYLIPPARSER_DESCRIPTION; }
        }

        /// <summary>
        /// Gets or sets alphabet to use for sequences in parsed ISequenceAlignment objects.
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        /// Gets the file extensions that the parser implementation
        /// will support.
        /// </summary>
        public string SupportedFileTypes
        {
            get { return Properties.Resource.PHYLIP_FILEEXTENSION; }
        }

        /// <summary>
        /// Parses a list of biological sequence alignment texts from a reader.
        /// </summary>
        /// <param name="stream">A stream for a biological sequence alignment text.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        public IEnumerable<ISequenceAlignment> Parse(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var reader = stream.OpenRead()) 
            {
                // no empty files allowed
                if (reader.EndOfStream)
                    throw new InvalidDataException(Properties.Resource.IONoTextToParse);

                while (reader.Peek() != -1)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        ReadNextLine(reader);
                        continue;
                    }

                    yield return ParseOne(reader);
                }
            }
        }

        /// <summary>
        /// Parses a single biological sequence alignment text from a reader.
        /// </summary>
        /// <param name="stream">A stream for a biological sequence alignment text.</param>
        /// <returns>The parsed ISequenceAlignment object.</returns>
        public ISequenceAlignment ParseOne(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var reader = stream.OpenRead())
            {
                // no empty files allowed
                if (reader.EndOfStream)
                    throw new InvalidDataException(Properties.Resource.IONoTextToParse);

                return ParseOne(reader);
            }
        }

        /// <summary>
        /// Parses a single biological sequence alignment text from a reader.
        /// </summary>
        /// <param name="reader">A reader for a biological sequence alignment text.</param>
        /// <returns>The parsed ISequenceAlignment object.</returns>
        ISequenceAlignment ParseOne(TextReader reader)
        {
            if (line == null)
                ReadNextLine(reader);

            // no empty files allowed
            if (line == null)
            {
                throw new InvalidDataException(Properties.Resource.IONoTextToParse);
            }

            // Parse first line
            IList<string> tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            if (2 != tokens.Count)
            {
                throw new InvalidDataException(
                    string.Format(CultureInfo.CurrentCulture, 
                        Properties.Resource.INVALID_INPUT_FILE, this.Name));
            }

            bool isFirstBlock = true;
            int sequenceCount;
            int sequenceLength;
            IList<Tuple<Sequence, List<byte>>> data = new List<Tuple<Sequence, List<byte>>>();
            IAlphabet alignmentAlphabet = null;

            sequenceCount = Int32.Parse(tokens[0], CultureInfo.InvariantCulture);
            sequenceLength = Int32.Parse(tokens[1], CultureInfo.InvariantCulture);

            ReadNextLine(reader);  // Skip blank lines until we get to the first block.

            // Now that we're at the first block, one or more blank lines are the block separators, which we'll need.
            skipBlankLines = false;

            while (reader.Peek() != -1)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    ReadNextLine(reader);
                    continue;
                }

                for (int index = 0; index < sequenceCount; index++)
                {
                    if (isFirstBlock)
                    {
                        // First 10 characters are sequence ID, remaining is the first block of sequence
                        // Note that both may contain whitespace, and there may be no whitespace between them.
                        if (line.Length <= 10)
                        {
                            throw new Exception(string.Format(
                                CultureInfo.CurrentCulture, 
                                Properties.Resource.INVALID_INPUT_FILE, this.Name));
                        }
                        string id = line.Substring(0, 10).Trim();
                        string sequenceString = line.Substring(10).Replace(" ","");
                        byte[] sequenceBytes = Encoding.UTF8.GetBytes(sequenceString);

                        IAlphabet alphabet = Alphabet;
                        if (null == alphabet)
                        {
                            alphabet = Alphabets.AutoDetectAlphabet(sequenceBytes, 0, sequenceBytes.Length, alphabet);

                            if (null == alphabet)
                            {
                                throw new InvalidDataException(string.Format(
                                        CultureInfo.InvariantCulture,
                                        Properties.Resource.InvalidSymbolInString,
                                        sequenceString));
                            }

                            if (null == alignmentAlphabet)
                            {
                                alignmentAlphabet = alphabet;
                            }
                            else
                            {
                                if (alignmentAlphabet != alphabet)
                                {
                                    throw new InvalidDataException(Properties.Resource.SequenceAlphabetMismatch);
                                }
                            }
                        }

                        var sequenceStore = new Tuple<Sequence, List<byte>>(
                            new Sequence(alphabet, string.Empty){ ID = id }, 
                            new List<byte>());

                        sequenceStore.Item2.AddRange(sequenceBytes);
                        data.Add(sequenceStore);
                    }
                    else
                    {
                        Tuple<Sequence, List<byte>> sequence = data[index];
                        byte[] sequenceBytes = Encoding.UTF8.GetBytes(line.Replace(" ",""));
                        sequence.Item2.AddRange(sequenceBytes);
                    }

                    ReadNextLine(reader);
                }

                // Reset the first block flag
                isFirstBlock = false;
            }

            // Validate for the count of sequence
            if (sequenceCount != data.Count)
            {
                throw new InvalidDataException(Properties.Resource.SequenceCountMismatch);
            }

            SequenceAlignment sequenceAlignment = new SequenceAlignment();
            sequenceAlignment.AlignedSequences.Add(new AlignedSequence());

            foreach (var dataSequence in data)
            {
                // Validate for the count of sequence
                if (sequenceLength != dataSequence.Item2.Count)
                {
                    throw new InvalidDataException(Properties.Resource.SequenceLengthMismatch);
                }

                sequenceAlignment.AlignedSequences[0].Sequences.Add(
                    new Sequence(dataSequence.Item1.Alphabet, dataSequence.Item2.ToArray()) { ID = dataSequence.Item1.ID });
            }

            return sequenceAlignment;
        }

        /// <summary>
        /// Reads next line considering
        /// </summary>
        /// <returns></returns>
        private void ReadNextLine(TextReader reader)
        {
            if (reader.Peek() == -1)
            {
                line = null;
                return;
            }

            line = reader.ReadLine();
            while (skipBlankLines && string.IsNullOrWhiteSpace(line) && reader.Peek() != -1)
            {
                line = reader.ReadLine();
            }
        }
    }
}
