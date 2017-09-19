using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Bio.Algorithms.Alignment;
using Bio.Extensions;

namespace Bio.IO.Nexus
{
    /// <summary>
    /// A NexusParser reads from a source of text that is formatted according 
    /// to the NexusParser flat file specification, and converts the data to 
    /// in-memory ISequenceAlignment objects.  For advanced users, the ability 
    /// to select an encoding for the internal memory representation is provided. 
    /// There is also a default encoding for each alphabet that may be encountered.
    /// </summary>
    public class NexusParser : ISequenceAlignmentParser
    {
        /// <summary>
        /// Indicates that the parser should skip any blank line while reading the stream.
        /// </summary>
        private bool skipBlankLines = true;

        /// <summary>
        /// Stores the last line read by the reader
        /// </summary>
        private string line = string.Empty;

        /// <summary>
        /// Gets the name of the sequence alignment parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser type.
        /// </summary>
        public string Name
        {
            get { return Properties.Resource.NEXUS_NAME; }
        }

        /// <summary>
        /// Gets the description of the sequence alignment parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser.
        /// </summary>
        public string Description
        {
            get { return Properties.Resource.NEXUSPARSER_DESCRIPTION; }
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
            get { return Properties.Resource.NEXUS_FILEEXTENSION; }
        }

        /// <summary>
        /// Parses a list of biological sequence alignment texts from a reader.
        /// </summary>
        /// <param name="stream">A stream for a biological sequence alignment text.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        public IEnumerable<ISequenceAlignment> Parse(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var reader = stream.OpenRead())
            {
                // no empty files allowed
                if (reader.EndOfStream)
                {
                    throw new InvalidDataException(Properties.Resource.IONoTextToParse);
                }

                // Parse Header, Loop through the blocks and parse
                while (line != null)
                {
                    yield return ParseOne(reader);
                }
            }
        }

        /// <summary>
        /// Parse a single sequence alignment from the stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Sequence alignment</returns>
        public ISequenceAlignment ParseOne(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var reader = stream.OpenRead())
            {
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
            ReadNextLine(reader);
            if (line == null)
            {
                throw new Exception(Properties.Resource.INVALID_INPUT_FILE);
            }

            this.ParseHeader(reader);

            var alignedSequence = new AlignedSequence();
            IList<string> ids = null;
            bool isInBlock = true;

            if (this.line.StartsWith("begin", StringComparison.OrdinalIgnoreCase))
            {
                while (this.line != null && isInBlock)
                {
                    if (string.IsNullOrEmpty(this.line.Trim()))
                    {
                        this.ReadNextLine(reader);
                        continue;
                    }

                    string blockName = GetTokens(this.line)[1];

                    switch (blockName.ToUpperInvariant())
                    {
                        case "TAXA":
                        case "TAXA;":
                            // This block contains the count of sequence & title of each sequence
                            ids = this.ParseTaxaBlock(reader);
                            break;

                        case "CHARACTERS":
                        case "CHARACTERS;":
                            // Block contains sequences
                            Dictionary<string, string> dataSet = this.ParseCharacterBlock(reader, ids);
                            IAlphabet alignmentAlphabet = null;

                            foreach (string id in ids)
                            {
                                IAlphabet alphabet = this.Alphabet;
                                string data = dataSet[id];

                                if (null == alphabet)
                                {
                                    byte[] dataArray = data.ToByteArray();
                                    alphabet = Alphabets.AutoDetectAlphabet(dataArray, 0, dataArray.Length, null);

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
                                                CultureInfo.InvariantCulture,
                                                Properties.Resource.SequenceAlphabetMismatch));
                                        }
                                    }
                                }

                                alignedSequence.Sequences.Add(new Sequence(alphabet, data) { ID = id });
                            }

                            break;

                        case "END":
                        case "END;":
                            // Have reached the end of block
                            isInBlock = false;
                            break;

                        default:
                            // skip this block
                            while (this.line != null)
                            {
                                this.ReadNextLine(reader);
                                if (0 == string.Compare(this.line, "end;", StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                            break;
                    }

                    this.ReadNextLine(reader);
                }
            }

            ISequenceAlignment sequenceAlignment = new SequenceAlignment();
            sequenceAlignment.AlignedSequences.Add(alignedSequence);
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

        /// <summary>
        /// Split the line and return the tokens in the line
        /// </summary>
        /// <param name="line">Line to be split</param>
        /// <returns>Tokens in line</returns>
        private static IList<string> GetTokens(string line)
        {
            return line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Parse Nexus Header
        /// </summary>
        /// <param name="reader">A reader for a biological sequence text.</param>
        private void ParseHeader(TextReader reader)
        {
            string message = string.Empty;

            if (!line.StartsWith("#NEXUS", StringComparison.OrdinalIgnoreCase))
            {
                message = string.Format(CultureInfo.CurrentCulture, Properties.Resource.INVALID_INPUT_FILE, this.Name);
                throw new InvalidDataException(message);
            }

            ReadNextLine(reader);

            // Title of Alignment
            if (line.Trim().StartsWith("[", StringComparison.OrdinalIgnoreCase))
            {
                while(line != null)
                {
                    ReadNextLine(reader);
                    if (line.Trim().EndsWith("]", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }

            ReadNextLine(reader);

            // Now that we're at the first block, one or more blank lines are the block separators, which we'll need.
            this.skipBlankLines = false;
        }

        /// <summary>
        /// Gets the list of sequence titles
        /// </summary>
        /// <param name="reader">A reader for a biological sequence text.</param>
        /// <returns>List of sequence IDs</returns>
        private IList<string> ParseTaxaBlock(TextReader reader)
        {
            bool isInTaxaBlock = true;
            string data = string.Empty;
            int sequenceCount = 0;
            IList<string> IDs = new List<string>();

            while (line != null && isInTaxaBlock)
            {
                ReadNextLine(reader);
                IList<string> tokens = GetTokens(line);
                switch (tokens[0].ToUpperInvariant())
                {
                    case "DIMENSIONS":
                        tokens[0] = string.Empty;

                        // Parse dimensions
                        // 1. Read count of sequence
                        do
                        {
                            foreach (string token in tokens)
                            {
                                data = token.Trim(new char[] { ';' });

                                if (string.IsNullOrEmpty(data))
                                {
                                    continue;
                                }

                                if (data.StartsWith("ntax=", StringComparison.OrdinalIgnoreCase))
                                {
                                    sequenceCount = Int32.Parse(data.Substring(5), CultureInfo.InvariantCulture);
                                }
                            }

                            if (line.Trim().EndsWith(";", StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                            this.ReadNextLine(reader);
                            tokens = GetTokens(this.line);
                        }
                        while (line != null);

                        break;

                    case "TAXLABELS":
                    case "TAXLABELS;":
                        tokens[0] = string.Empty;

                        // Parse taxlabels
                        // 1. Read IDs of sequence
                        do
                        {
                            foreach (string token in tokens)
                            {
                                data = token.Trim(new char[] { ';' });

                                if (string.IsNullOrEmpty(data))
                                {
                                    continue;
                                }

                                IDs.Add(data);
                            }

                            if (line.Trim().EndsWith(";", StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                            this.ReadNextLine(reader);
                            tokens = GetTokens(this.line);
                        }
                        while (line != null);

                        break;

                    case "END":
                    case "END;":
                        // Have reached the end of taxa block
                        isInTaxaBlock = false;
                        break;

                    default:
                        break;
                }
            }

            // Read the end line "end;"
            ReadNextLine(reader);

            // Validate the count
            if (sequenceCount != IDs.Count)
            {
                throw new InvalidDataException(Properties.Resource.NtaxMismatch);
            }

            return IDs;
        }

        /// <summary>
        /// Parse the Sequence data in the block
        /// </summary>
        /// <param name="reader">A reader for a biological sequence text.</param>
        /// <param name="IDs">List of sequence IDs</param>
        /// <returns>parse sequence in alignment</returns>
        private Dictionary<string, string> ParseCharacterBlock(TextReader reader, IList<string> IDs)
        {
            bool isInCharactersBlock = true;
            string data = string.Empty;
            int sequenceLength = 0;
            Dictionary<string, string> dataSet = new Dictionary<string, string>();

            while (line != null && isInCharactersBlock)
            {
                ReadNextLine(reader);
                IList<string> tokens = GetTokens(line);

                if (0 == string.Compare("DIMENSIONS", tokens[0], StringComparison.OrdinalIgnoreCase))
                {
                    tokens[0] = string.Empty;

                    // Parse dimensions
                    // 1. Length of sequence
                    do
                    {
                        foreach (string token in tokens)
                        {
                            data = token.Trim(new char[] { ';' });

                            if (string.IsNullOrEmpty(data))
                            {
                                continue;
                            }

                            if (data.StartsWith("nchar=", StringComparison.OrdinalIgnoreCase))
                            {
                                sequenceLength = Int32.Parse(data.Substring(6), CultureInfo.InvariantCulture);
                            }
                        }

                        if (line.Trim().EndsWith(";", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                        else
                        {
                            ReadNextLine(reader);
                            tokens = GetTokens(line);
                        }
                    }
                    while (line != null);
                }
                else if (0 == string.Compare("FORMAT", tokens[0], StringComparison.OrdinalIgnoreCase))
                {
                    tokens[0] = string.Empty;

                    // Parse format
                    // 1. Notation for "missing"
                    // 2. Notation for "gap"
                    // 3. Notation for "matchchar"
                    // 4. data type
                    do
                    {
                        if (line.Trim().EndsWith(";", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                        else
                        {
                            ReadNextLine(reader);
                            tokens = GetTokens(line);
                        }
                    }
                    while (line != null);
                }
                if (0 == string.Compare("MATRIX", tokens[0], StringComparison.OrdinalIgnoreCase))
                {
                    tokens[0] = string.Empty;

                    // "If available" ignore the data in square brackets []
                    while (line != null)
                    {
                        if (line.StartsWith("[", StringComparison.OrdinalIgnoreCase))
                        {
                            ReadNextLine(reader);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Here are the alignment sequences
                    while (line != null)
                    {
                        ReadNextLine(reader);

                        if (string.IsNullOrEmpty(line.Trim()))
                        {
                            continue;
                        }

                        tokens = GetTokens(line);
                        if (tokens[0].StartsWith(";", StringComparison.OrdinalIgnoreCase))
                        {
                            isInCharactersBlock = false;
                            break;
                        }

                        if (IDs.Contains(tokens[0]))
                        {
                            data = tokens[1];

                            if (dataSet.ContainsKey(tokens[0]))
                            {
                                data = string.Concat(dataSet[tokens[0]], data);
                            }

                            dataSet[tokens[0]] = data;
                        }
                    }
                }
                else if (tokens[0].StartsWith(";", StringComparison.OrdinalIgnoreCase))
                {
                    isInCharactersBlock = false;
                }
            }

            // Read the end line "end;"
            ReadNextLine(reader);

            // Validate the length of sequence
            if (dataSet.Values.Any(dataSequence => dataSequence.Length != sequenceLength))
            {
                throw new FormatException(Properties.Resource.SequenceLengthMismatch);
            }

            return dataSet;
        }
    }
}
