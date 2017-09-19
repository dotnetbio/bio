using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Bio.Extensions;
using Bio.Properties;

namespace Bio.IO.Text
{
    /// <summary>
    ///     Field parser reads from a source of text (default tab-delimited)
    ///     and converts the data to in-memory ISequence objects.
    ///     Example, tab-delimited sequence file contains two columns:
    ///     First column contain sequence id and second column contains the sequence.
    /// </summary>
    public sealed class FieldTextFileParser : ISequenceParser
    {
        /// <summary>
        ///     Initialize instance of for Tab (default) parser class.
        /// </summary>
        public FieldTextFileParser()
        {
            this.ContainsHeader = true;
            this.Delimiter = '\t';
        }

        /// <summary>
        ///     Gets or sets value whether file contains header.
        ///     By default first line is considered as header line.
        /// </summary>
        public bool ContainsHeader { get; set; }

        /// <summary>
        ///     Gets or sets value of delimiter. The delimiter defines how columns in file are separated.
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        ///     Gets the type of Parser i.e field parser.
        ///     This is intended to give developers some information
        ///     about parser class.
        /// </summary>
        public string Name
        {
            get
            {
                return Resource.FIELDPARSER_NAME;
            }
        }

        /// <summary>
        ///     Gets the description of field parser.
        ///     This is intended to give developers some information
        ///     of the parser class. This property returns a simple description of what the
        ///     FieldParser class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Resource.FIELDPARSER_DESCRIPTION;
            }
        }

        /// <summary>
        ///     Gets a comma separated values of the possible
        ///     file extensions for a TAB file.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Resource.FIELDPARSER_FILEEXTENSION;
            }
        }

        /// <summary>
        ///     The alphabet to use for parsed ISequence objects.  If this is not set, the alphabet will
        ///     be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        /// Parse a single sequence from the stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <returns>Sequence</returns>
        public ISequence ParseOne(Stream stream)
        {
            return Parse(stream).FirstOrDefault();
        }

        /// <summary>
        ///     Returns an IEnumerable of sequences in the stream being parsed.
        /// </summary>
        /// <param name="stream">Stream to parse.</param>
        /// <returns>Returns ISequence arrays.</returns>
        public IEnumerable<ISequence> Parse(Stream stream)
        {
            using (var sr = stream.OpenRead())
            {
                string fileLine = sr.ReadLine();

                if (this.ContainsHeader)
                {
                    fileLine = sr.ReadLine();
                }

                while (!string.IsNullOrEmpty(fileLine))
                {
                    yield return this.ParseLine(fileLine);
                    fileLine = sr.ReadLine();
                }
            }
        }

        /// <summary>
        ///     Parses one line from the text file.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private ISequence ParseLine(string line)
        {
            string[] splitLine = line.Split(this.Delimiter);
            if (splitLine.Length != 2)
                throw new Exception(string.Format(CultureInfo.InvariantCulture, Resource.INVALID_INPUT_FILE, line));

            IAlphabet alphabet = this.Alphabet;
            if (alphabet == null)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(splitLine[1]);
                alphabet = Alphabets.AutoDetectAlphabet(byteArray, 0, byteArray.Length, null);
                if (alphabet == null)
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, Resource.InvalidSymbolInString, splitLine[1]));
            }

            return new Sequence(alphabet, splitLine[1]) { ID = splitLine[0] };
        }
    }
}