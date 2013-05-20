using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Bio.Algorithms.Alignment;

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
        private string line = null;

        #region "Public Property(ies)"
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
            {
                throw new ArgumentNullException("reader");
            }

            // No empty files allowed
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

            // no empty files allowed
            if (line == null)
                ReadNextLine(reader);

            if (line == null)
            {
                throw new InvalidDataException(Properties.Resource.IONoTextToParse);
            }

            if (!line.StartsWith("CLUSTAL", StringComparison.OrdinalIgnoreCase))
            {
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, this.Name);
                throw new InvalidDataException(message);
            }

            ReadNextLine(reader);  // Skip blank lines until we get to the first block.

            // Now that we're at the first block, one or more blank lines are the block separators, which we'll need.
            skipBlankLines = false;

            Dictionary<string, Tuple<ISequence, List<byte>>> mapIdToSequence = new Dictionary<string, Tuple<ISequence, List<byte>>>();
            IAlphabet alignmentAlphabet = null;
            bool isFirstBlock = true;
            bool inBlock = false;
            HashSet<char> endOfBlockSymbols = new HashSet<char> { '*', ' ', '.', '+', ':' };

            while (reader.Peek() != -1)
            {
                // Blank line or consensus line signals end of block.
                if (String.IsNullOrEmpty(line) || line.All(a => endOfBlockSymbols.Contains(a)))
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
                    string data = tokens[1].ToUpper(CultureInfo.InvariantCulture);
                    byte[] byteData = ASCIIEncoding.ASCII.GetBytes(data);
                    Tuple<ISequence, List<byte>> sequenceTuple = null;
                    IAlphabet alphabet = Alphabet;

                    inBlock = true;
                    if (isFirstBlock)
                    {
                        if (null == alphabet)
                        {
                            alphabet = Alphabets.AutoDetectAlphabet(byteData, 0, byteData.Length, alphabet);

                            if (null == alphabet)
                            {
                                message = string.Format(
                                        CultureInfo.InvariantCulture,
                                        Properties.Resource.InvalidSymbolInString,
                                        data);
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
                                        message = string.Format(
                                                CultureInfo.CurrentCulture,
                                                Properties.Resource.SequenceAlphabetMismatch);
                                        throw new InvalidDataException(message);
                                    }
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
                            message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.ClustalUnknownSequence, id);
                            throw new InvalidDataException(message);
                        }

                        sequenceTuple = mapIdToSequence[id];
                        sequenceTuple.Item2.AddRange(byteData);
                    }
                }

                ReadNextLine(reader);
            }

            SequenceAlignment sequenceAlignment = new SequenceAlignment();
            sequenceAlignment.AlignedSequences.Add(new AlignedSequence());
            foreach (var alignmentSequenceTuple in mapIdToSequence.Values)
            {
                sequenceAlignment.AlignedSequences[0].Sequences.Add(
                    new Sequence(alignmentSequenceTuple.Item1.Alphabet, alignmentSequenceTuple.Item2.ToArray()) 
                    { 
                        ID = alignmentSequenceTuple.Item1.ID 
                    });
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
