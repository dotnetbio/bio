using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Bio.Algorithms.Alignment;

namespace Bio.Util
{
    /// <summary>
    /// This parser reads from a source of text that contains DeltaAlignments
    /// and converts the data to in-memory DeltaAlignment objects.  
    /// </summary>
    public sealed class DeltaAlignmentParser : IDisposable
    {
        #region Member variables
        /// <summary>
        /// Stream reader for Delta file.
        /// </summary>
        private StreamReader _deltaFileReader;

        /// <summary>
        /// List holding all the open parsing readers.
        /// </summary>
        private List<StreamReader> _parsingReaders = new List<StreamReader>(); 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DeltaAlignmentParser class by 
        /// loading the specified filename.
        /// </summary>
        /// <param name="deltaFilename">Name of the File.</param>
        /// <param name="queryParser">FastASequencePositionParser instance.</param>
        public DeltaAlignmentParser(string deltaFilename, FastASequencePositionParser queryParser)
        {
            this.Open(deltaFilename, queryParser);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the delta filename.
        /// </summary>
        public string DeltaFilename { get; private set; }

        /// <summary>
        /// Gets the query parser.
        /// </summary>
        public FastASequencePositionParser QueryParser { get; private set; }

        #endregion

        #region Methods
        /// <summary>
        /// Opens the stream for the specified file.
        /// </summary>
        /// <param name="deltaFilename">Name of the file to open.</param>
        /// <param name="queryParser">Parser to parse the query file.</param>
        public void Open(string deltaFilename, FastASequencePositionParser queryParser)
        {
            // if the file is already open throw invalid 
            if (!string.IsNullOrEmpty(this.DeltaFilename))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.FileAlreadyOpen, this.DeltaFilename));
            }

            // Validate the file - by try to open.
            using (new StreamReader(deltaFilename))
            {
            }

            this.DeltaFilename = deltaFilename;
            if (queryParser == null)
            {
                throw new ArgumentNullException("queryParser");
            }

            this.QueryParser = queryParser;
        }

        /// <summary>
        /// Gets the position of DeltaAlignments in the specified file.
        /// </summary>
        public IEnumerable<long> GetPositions()
        {
            using (StreamReader streamReader = new StreamReader(this.DeltaFilename))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line.StartsWith("@"))
                    {
                        line = line.Substring(1);
                        yield return long.Parse(line);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the DeltaAlignment at specified position of the file.
        /// </summary>
        /// <param name="position">Position at which delta alignment is required.</param>
        /// <returns>Delta alignment.</returns>
        public DeltaAlignment GetDeltaAlignmentAt(long position)
        {
            if (this._deltaFileReader == null)
            {
                this._deltaFileReader = new StreamReader(new FileStream(this.DeltaFilename, FileMode.Open, FileAccess.Read));
            }

            this._deltaFileReader.BaseStream.Position = position;
            this._deltaFileReader.DiscardBufferedData();

            long deltaPosition = -1;
            string line = ReadNextLine(this._deltaFileReader);
            if (line == null || !line.StartsWith("@", StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.CorruptedDeltaAlignmentFile, position, this.DeltaFilename));
            }

            deltaPosition = long.Parse(line.Substring(1), CultureInfo.InvariantCulture);
            if (position != deltaPosition)
            {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.DeltaAlignmentIDDoesnotMatch, deltaPosition, position, this.DeltaFilename));
            }

            line = ReadNextLine(this._deltaFileReader);
            if (line == null || !line.StartsWith(">", StringComparison.OrdinalIgnoreCase))
            {
                string message = string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resource.INVALID_INPUT_FILE,
                        this.DeltaFilename);

                throw new FileFormatException(message);
            }

            string referenceId = line.Substring(1);

            // Read next line.
            line = ReadNextLine(this._deltaFileReader);

            // Second line - Query sequence id
            string queryId = line;

            // fetch the query sequence from the query file
            ISequence querySequence = null;
            Sequence refEmpty = null;

            if (!string.IsNullOrEmpty(queryId))
            {
                long sequencePosition = long.Parse(queryId.Substring(queryId.LastIndexOf("@", StringComparison.Ordinal) + 1), CultureInfo.InvariantCulture);
                querySequence = this.QueryParser.GetSequenceAt(sequencePosition);
                refEmpty = new Sequence(querySequence.Alphabet, "A", false);
                refEmpty.ID = referenceId;
            }

            DeltaAlignment deltaAlignment = new DeltaAlignment(refEmpty, querySequence);
            deltaAlignment.Id = deltaPosition;
            line = ReadNextLine(this._deltaFileReader);
            string[] deltaAlignmentProperties = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (deltaAlignmentProperties != null && deltaAlignmentProperties.Length == 7)
            {
                long temp;
                deltaAlignment.FirstSequenceStart = long.TryParse(deltaAlignmentProperties[0], out temp) ? temp : 0;
                deltaAlignment.FirstSequenceEnd = long.TryParse(deltaAlignmentProperties[1], out temp) ? temp : 0;
                deltaAlignment.SecondSequenceStart = long.TryParse(deltaAlignmentProperties[2], out temp) ? temp : 0;
                deltaAlignment.SecondSequenceEnd = long.TryParse(deltaAlignmentProperties[3], out temp) ? temp : 0;
                int error;
                deltaAlignment.Errors = int.TryParse(deltaAlignmentProperties[4], out error) ? error : 0;
                deltaAlignment.SimilarityErrors = int.TryParse(deltaAlignmentProperties[5], out error) ? error : 0;
                deltaAlignment.NonAlphas = int.TryParse(deltaAlignmentProperties[6], out error) ? error : 0;
            }

            // Fifth line - either a 0 - marks the end of the delta alignment or they are deltas
            while (line != null && !line.StartsWith("*", StringComparison.OrdinalIgnoreCase))
            {
                long temp;
                if (long.TryParse(line, out temp))
                {
                    deltaAlignment.Deltas.Add(temp);
                }

                // Read next line.
                line = this._deltaFileReader.ReadLine();

                // Continue reading if blank line found.
                while (line != null && string.IsNullOrEmpty(line))
                {
                    line = this._deltaFileReader.ReadLine();
                }
            }

            return deltaAlignment;
        }

        /// <summary>
        /// Gets the query sequence id in the DeltaAlignment at specified position.
        /// </summary>
        /// <param name="position">Position of the delta alignment.</param>
        public string GetQuerySeqIdAt(long position)
        {
            if (this._deltaFileReader == null)
            {
                this._deltaFileReader = new StreamReader(new FileStream(this.DeltaFilename, FileMode.Open, FileAccess.Read));
            }

            this._deltaFileReader.BaseStream.Position = position;
            this._deltaFileReader.DiscardBufferedData();

            long deltaPosition = -1;
            string line = ReadNextLine(this._deltaFileReader);
            if (line == null || !line.StartsWith("@", StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.CorruptedDeltaAlignmentFile, position, this.DeltaFilename));
            }

            deltaPosition = long.Parse(line.Substring(1), CultureInfo.InvariantCulture);
            if (position != deltaPosition)
            {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resource.DeltaAlignmentIDDoesnotMatch, deltaPosition, position, this.DeltaFilename));
            }

            line = ReadNextLine(this._deltaFileReader);
            if (line == null || !line.StartsWith(">", StringComparison.OrdinalIgnoreCase))
            {
                string message = string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resource.INVALID_INPUT_FILE,
                        this.DeltaFilename);

                throw new FileFormatException(message);
            }

            // Read next line.
            line = ReadNextLine(this._deltaFileReader);
            return line;
        }

        /// <summary>
        /// Gets Delta alignment id and query sequence ids pairs.
        /// </summary>
        public IEnumerable<Tuple<string, string>> GetQuerySeqIds()
        {
            using (StreamReader reader = new StreamReader(this.DeltaFilename))
            {
                while (!reader.EndOfStream)
                {
                    string line = ReadNextLine(reader);
                    if (line == null || !line.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                    {
                        string message = string.Format(
                           CultureInfo.InvariantCulture,
                           Properties.Resource.INVALID_INPUT_FILE,
                           this.DeltaFilename);
                        throw new FileFormatException(message);
                    }

                    string id = line;
                    line = ReadNextLine(reader);
                    if (line == null || !line.StartsWith(">", StringComparison.OrdinalIgnoreCase))
                    {
                        string message = string.Format(
                            CultureInfo.InvariantCulture,
                            Properties.Resource.INVALID_INPUT_FILE,
                            this.DeltaFilename);
                        throw new FileFormatException(message);
                    }

                    line = ReadNextLine(reader);
                    yield return new Tuple<string, string>(id, line);
                } 
            }
        }

        /// <summary>
        /// Returns an IEnumerable of DeltaAlignment in the file being parsed.
        /// </summary>
        /// <returns>Returns DeltaAlignment collection.</returns>
        public IEnumerable<DeltaAlignment> Parse()
        {
            StreamReader streamReader = new StreamReader(this.DeltaFilename);
            return this.ParseFrom(streamReader);
        }

        /// <summary>
        /// Starts parsing of delta alignments from the specified position of the file.
        /// </summary>
        /// <param name="position">Position from which to start parsing.</param>
        /// <returns>IEnumerable of DeltaAlignments.</returns>
        public IEnumerable<DeltaAlignment> ParseFrom(long position)
        {
            StreamReader reader = new StreamReader(new FileStream(this.DeltaFilename, FileMode.Open, FileAccess.Read));
            reader.BaseStream.Position = position;
            reader.DiscardBufferedData();
            return this.ParseFrom(reader);
        }

        /// <summary>
        /// Closes streams used.
        /// </summary>
        public void Close()
        {
            this.DeltaFilename = null;
            if (this._deltaFileReader != null)
            {
                this._deltaFileReader.Close();
                this._deltaFileReader = null;
            }

            foreach (var reader in _parsingReaders)
            {
                reader.Dispose();
            }

            _parsingReaders.Clear();
        }

        /// <summary>
        /// Disposes the underlying stream.
        /// </summary>
        public void Dispose()
        {
            this.Close();
            GC.SuppressFinalize(this);
        }

        #region Private Methods

        /// <summary>
        /// Gets the next line skipping the blank lines.
        /// </summary>
        /// <param name="streamReader">Stream reader.</param>
        private static string ReadNextLine(StreamReader streamReader)
        {
            // Read next line.
            string line = streamReader.ReadLine();
            string message = string.Empty;

            // Continue reading if blank line found.
            while (line != null && string.IsNullOrEmpty(line))
            {
                line = streamReader.ReadLine();
            }

            if (line == null)
            {
                message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resource.InvalidSymbolInString,
                    string.Empty);
                throw new FormatException(message);
            }

            return line;
        }

        /// <summary>
        /// Starts parsing from the specified StreamReader.
        /// </summary>
        /// <param name="streamReader">Stream reader to parse.</param>
        /// <returns>IEnumerable of DeltaAlignments.</returns>
        private IEnumerable<DeltaAlignment> ParseFrom(StreamReader streamReader)
        {
            _parsingReaders.Add(streamReader);

            string lastReadQuerySequenceId = string.Empty;
            ISequence sequence = null;
            string message;

            if (streamReader.EndOfStream)
            {
                message = string.Format(
                          CultureInfo.InvariantCulture,
                          Properties.Resource.INVALID_INPUT_FILE,
                          this.DeltaFilename);

                throw new FileFormatException(message);
            }

            string line = ReadNextLine(streamReader);
            do
            {
                if (line == null || !line.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                {
                    message = string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resource.INVALID_INPUT_FILE,
                        this.DeltaFilename);

                    throw new FileFormatException(message);
                }

                long deltaPosition = long.Parse(line.Substring(1));
                line = ReadNextLine(streamReader);
                if (line == null || !line.StartsWith(">", StringComparison.OrdinalIgnoreCase))
                {
                    message = string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resource.INVALID_INPUT_FILE,
                        this.DeltaFilename);

                    throw new FileFormatException(message);
                }

                DeltaAlignment deltaAlignment = null;

                // First line - reference id
                string referenceId = line.Substring(1);

                // Read next line.
                line = ReadNextLine(streamReader);

                // Second line - Query sequence id
                string queryId = line;

                // fetch the query sequence from the query file
                if (!string.IsNullOrEmpty(queryId))
                {
                    if (queryId != lastReadQuerySequenceId)
                    {
                        long seqPosition = long.Parse(queryId.Substring(queryId.LastIndexOf('@') + 1));
                        sequence = this.QueryParser.GetSequenceAt(seqPosition);
                        lastReadQuerySequenceId = queryId;
                    }

                    Sequence refEmpty = new Sequence(sequence.Alphabet, "A", false);
                    refEmpty.ID = referenceId;

                    deltaAlignment = new DeltaAlignment(refEmpty, sequence);
                }

                deltaAlignment.Id = deltaPosition;
                // Fourth line - properties of delta alignment
                // Read next line.
                line = ReadNextLine(streamReader);

                string[] deltaAlignmentProperties = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (deltaAlignmentProperties != null && deltaAlignmentProperties.Length == 7)
                {
                    long temp;
                    deltaAlignment.FirstSequenceStart = long.TryParse(deltaAlignmentProperties[0], out temp) ? temp : 0;
                    deltaAlignment.FirstSequenceEnd = long.TryParse(deltaAlignmentProperties[1], out temp) ? temp : 0;
                    deltaAlignment.SecondSequenceStart = long.TryParse(deltaAlignmentProperties[2], out temp) ? temp : 0;
                    deltaAlignment.SecondSequenceEnd = long.TryParse(deltaAlignmentProperties[3], out temp) ? temp : 0;
                    int error;
                    deltaAlignment.Errors = int.TryParse(deltaAlignmentProperties[4], out error) ? error : 0;
                    deltaAlignment.SimilarityErrors = int.TryParse(deltaAlignmentProperties[5], out error) ? error : 0;
                    deltaAlignment.NonAlphas = int.TryParse(deltaAlignmentProperties[6], out error) ? error : 0;
                }

                // Fifth line - either a 0 - marks the end of the delta alignment or they are deltas
                while (line != null && !line.StartsWith("*", StringComparison.OrdinalIgnoreCase))
                {
                    long temp;
                    if (long.TryParse(line, out temp))
                    {
                        deltaAlignment.Deltas.Add(temp);
                    }

                    // Read next line.
                    line = streamReader.ReadLine();

                    // Continue reading if blank line found.
                    while (line != null && string.IsNullOrEmpty(line))
                    {
                        line = streamReader.ReadLine();
                    }
                }

                yield return deltaAlignment;

                // Read the next line
                line = streamReader.ReadLine();
            }
            while (line != null);

            streamReader.Close();
        }
        #endregion
        #endregion
    }
}
