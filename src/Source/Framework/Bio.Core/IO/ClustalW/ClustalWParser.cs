using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Bio.Algorithms.Alignment;
using Bio.Extensions;

namespace Bio.IO.ClustalW
{
    /// <summary>
    /// A ClustalWParser reads from a source of text that is formatted according to the ClustalW flat
    /// file specification, and converts the data to in-memory ISequenceAlignment objects.
    /// </summary>
    public class ClustalWParser : ISequenceAlignmentParser
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
            get { return Properties.Resource.CLUSTALW_NAME; }
        }

        /// <summary>
        /// Gets the description of the sequence alignment parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.CLUSTALWPARSER_DESCRIPTION; }
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
            get { return Properties.Resource.CLUSTALW_FILEEXTENSION; }
        }

        /// <summary>
        /// Parses a list of biological sequence alignment texts from a reader.
        /// </summary>
        /// <param name="stream">A stream for a biological sequence alignment text, it will be closed at the end.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        public IEnumerable<ISequenceAlignment> Parse(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            // No empty files allowed
            if (!stream.CanRead || stream.Length == 0)
            {
                throw new InvalidDataException(Properties.Resource.IONoTextToParse);
            }

            using (var reader = stream.OpenRead())
            {
                while (!reader.EndOfStream)
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
        /// Parses a single biological sequence alignment text from a stream.
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
                return this.ParseOne(reader);
            }
        }

        /// <summary>
        /// Parses a single biological sequence alignment text from a stream.
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Sequence</returns>
        private ISequenceAlignment ParseOne(StreamReader reader)
        {
            // no empty files allowed
            if (line == null)
                ReadNextLine(reader);

            if (line == null)
                throw new InvalidDataException(Properties.Resource.IONoTextToParse);

            if (!line.StartsWith("CLUSTAL", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, this.Name));
            }

            ReadNextLine(reader);  // Skip blank lines until we get to the first block.

            // Now that we're at the first block, one or more blank lines are the block separators, which we'll need.
            skipBlankLines = false;

            var mapIdToSequence = new Dictionary<string, Tuple<ISequence, List<byte>>>();
            IAlphabet alignmentAlphabet = null;
            bool isFirstBlock = true;
            bool inBlock = false;
            var endOfBlockSymbols = new HashSet<char> { '*', ' ', '.', '+', ':' };

            while (reader.Peek() != -1)
            {
                // Blank line or consensus line signals end of block.
                if (String.IsNullOrEmpty(line) || line.ToCharArray().All(endOfBlockSymbols.Contains))
                {
                    if (inBlock)
                    {
                        // Blank line signifies end of block
                        inBlock = false;
                        isFirstBlock = false;
                    }
                }
                else // It's not a blank or consensus line.
                {
                    // It's a data line in a block.
                    // Lines begin with sequence id, then the sequence segment, and optionally a number, which we will ignore
                    string[] tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries); // (char[])null uses whitespace delimiters
                    string id = tokens[0];
                    string data = tokens[1].ToUpperInvariant();
                    byte[] byteData = Encoding.UTF8.GetBytes(data);
                    Tuple<ISequence, List<byte>> sequenceTuple;
                    IAlphabet alphabet = Alphabet;

                    inBlock = true;
                    if (isFirstBlock)
                    {
                        if (null == alphabet)
                        {
                            alphabet = Alphabets.AutoDetectAlphabet(byteData, 0, byteData.Length, alphabet);

                            if (null == alphabet)
                            {
                                throw new InvalidDataException(string.Format(
                                        CultureInfo.InvariantCulture,
                                        Properties.Resource.InvalidSymbolInString,
                                        data));
                            }
                            
                            if (null == alignmentAlphabet)
                            {
                                alignmentAlphabet = alphabet;
                            }
                            else
                            {
                                if (alignmentAlphabet != alphabet)
                                {
                                    throw new InvalidDataException(string.Format(
                                        CultureInfo.CurrentCulture,
                                        Properties.Resource.SequenceAlphabetMismatch));
                                }
                            }
                        }

                        sequenceTuple = new Tuple<ISequence, List<byte>>(
                            new Sequence(alphabet, "") { ID = id }, 
                            new List<byte>());
                        sequenceTuple.Item2.AddRange(byteData);

                        mapIdToSequence.Add(id, sequenceTuple);
                    }
                    else
                    {
                        if (!mapIdToSequence.ContainsKey(id))
                        {
                            throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.ClustalUnknownSequence, id));
                        }

                        sequenceTuple = mapIdToSequence[id];
                        sequenceTuple.Item2.AddRange(byteData);
                    }
                }

                ReadNextLine(reader);
            }

            var sequenceAlignment = new SequenceAlignment();
            var alignedSequence = new AlignedSequence();
            sequenceAlignment.AlignedSequences.Add(alignedSequence);
            foreach (var alignmentSequenceTuple in mapIdToSequence.Values)
            {
                alignedSequence.Sequences.Add(
                    new Sequence(alignmentSequenceTuple.Item1.Alphabet, alignmentSequenceTuple.Item2.ToArray()) 
                    { 
                        ID = alignmentSequenceTuple.Item1.ID 
                    });
            }

            return sequenceAlignment;
        }


        /// <summary>
        /// Reads next line considering
        /// </summary>
        /// <returns></returns>
        private void ReadNextLine(TextReader reader)
        {
            if (reader.Peek() == -1) {
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
