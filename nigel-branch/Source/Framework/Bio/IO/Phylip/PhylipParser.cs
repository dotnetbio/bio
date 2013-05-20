using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bio.Algorithms.Alignment;

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
        private string line = null;

        #region "Public Property(ies)"
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
        #endregion

        #region "Public Method(s)"
        /// <summary>
        /// Parses a list of biological sequence alignment texts from a reader.
        /// </summary>
        /// <param name="reader">A reader for a biological sequence alignment text.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        public IList<ISequenceAlignment> Parse(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            // no empty files allowed
            if (reader.Peek() == -1)
            {
                throw new InvalidDataException(Properties.Resource.IONoTextToParse);
            }

            List<ISequenceAlignment> alignments = new List<ISequenceAlignment>();

            while (reader.Peek() != -1)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    ReadNextLine(reader);
                    continue;
                }

                alignments.Add(ParseOne(reader));
            }

            return alignments;
        }

        /// <summary>
        /// Parses a list of biological sequence alignment texts from a file.
        /// </summary>
        /// <param name="fileName">The name of a biological sequence alignment file.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        public IList<ISequenceAlignment> Parse(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses a single biological sequence alignment text from a reader.
        /// </summary>
        /// <param name="reader">A reader for a biological sequence alignment text.</param>
        /// <returns>The parsed ISequenceAlignment object.</returns>
        public ISequenceAlignment ParseOne(TextReader reader)
        {
            string message = string.Empty;

            if (reader == null)
                throw new ArgumentNullException("reader");

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
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, this.Name);
                throw new InvalidDataException(message);
            }

            bool isFirstBlock = true;
            int sequenceCount = 0;
            int sequenceLength = 0;
            IList<Tuple<Sequence, List<byte>>> data = new List<Tuple<Sequence, List<byte>>>();
            string id = string.Empty;
            string sequenceString = string.Empty;
            Tuple<Sequence, List<byte>> sequence = null;
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
                            message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, this.Name);
                            throw new Exception(message);
                        }
                        id = line.Substring(0, 10).Trim();
                        sequenceString = line.Substring(10).Replace(" ","");
                        byte[] sequenceBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(sequenceString);

                        IAlphabet alphabet = Alphabet;
                        if (null == alphabet)
                        {
                            alphabet = Alphabets.AutoDetectAlphabet(sequenceBytes, 0, sequenceBytes.Length, alphabet);

                            if (null == alphabet)
                            {
                                message = string.Format(
                                        CultureInfo.InvariantCulture,
                                        Properties.Resource.InvalidSymbolInString,
                                        sequenceString);
                                throw new InvalidDataException(message);
                            }
                            else
                            {
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
                        }

                        Tuple<Sequence, List<byte>> sequenceStore = new Tuple<Sequence, List<byte>>(
                            new Sequence(alphabet, string.Empty){ ID = id }, 
                            new List<byte>());

                        sequenceStore.Item2.AddRange(sequenceBytes);
                        data.Add(sequenceStore);
                    }
                    else
                    {
                        sequence = data[index];
                        byte[] sequenceBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(line.Replace(" ",""));
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
        /// Parses a single biological sequence alignment text from a file.
        /// </summary>
        /// <param name="fileName">The name of a biological sequence alignment file.</param>
        /// <returns>The parsed ISequenceAlignment object.</returns>
        public ISequenceAlignment ParseOne(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                return ParseOne(reader);
            }
        }
        #endregion

        #region "Private Method(s)"
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
        #endregion
    }
}
