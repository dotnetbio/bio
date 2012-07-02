using System;
using System.Collections.Generic;
using System.IO;
using Bio.IO.AppliedBiosystems.DataParsers;
using Bio.IO.AppliedBiosystems.Model;
using Bio.Properties;

namespace Bio.IO.AppliedBiosystems
{
    /// <summary>
    /// Parses an applied biosystems data file format as defined in:
    /// 
    /// http://www6.appliedbiosystems.com/support/software_community/ABIF_File_Format.pdf
    /// </summary>
    public sealed class Ab1Parser : ISequenceParser
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Ab1Parser()
        {
            Alphabet = Alphabets.AmbiguousDNA;
        }

        /// <summary>
        /// Parsers the files binary content into a abi parser context using the DNA alphabet.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IParserContext Parse(BinaryReader reader)
        {
            return Parse(reader, null);
        }

        /// <summary>
        /// Parsers the files binary content into a abi parser context using
        /// the specified alphabet.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="alphabet"></param>
        /// <returns></returns>
        public static IParserContext Parse(BinaryReader reader, IAlphabet alphabet)
        {
            // Default to the DNA alphabet
            if (alphabet == null)
                alphabet = Alphabets.DNA;

            var rawData = new Ab1Header(reader);
            IVersionedDataParser dataParser = DataParserFactory.GetParser(rawData.MajorVersion);
            var context = new ParserContext
            {
                Header = rawData,
                Reader = reader,
                Alphabet = alphabet,
            };
            dataParser.ParseData(context);

            return context;
        }

        #region Implementation of IParser

        /// <summary>
        /// Name.
        /// </summary>
        public string Name
        {
            get { return Resource.APPLIEDBIOSYSTEMS_NAME; }
        }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description
        {
            get { return Resource.APPLIEDBIOSYSTEMS_DESCRIPTION; }
        }

        /// <summary>
        /// Supported file types.
        /// </summary>
        public string SupportedFileTypes
        {
            get { return Resource.APPLIEDBIOSYSTEMS_FILETYPES; }
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                Close();
        }

        #endregion

        #region Implementation of ISequenceParser

        /// <summary>
        /// File to be parsed.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Open the file.
        /// </summary>
        /// <param name="filename"></param>
        public void Open(string filename)
        {
            // if the file is already open throw invalid 
            if (string.IsNullOrEmpty(filename))
            {
                throw new InvalidOperationException();
            }

            // Validate the file - by try to open.
            using (new StreamReader(filename))
            {
            }

            Filename = filename;
        }

        /// <summary>
        /// Parser the file.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISequence> Parse()
        {
            using (FileStream stream = new FileInfo(Filename).OpenRead())
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    yield return Ab1ContextToSequenceConverter.Convert(Parse(binaryReader, Alphabet));
                }
            }
        }

        /// <summary>
        /// Parses the file using stream reader - not implemented
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IEnumerable<ISequence> Parse(StreamReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Close the parser.
        /// </summary>
        public void Close()
        {
            Filename = null;
        }

        /// <summary>
        /// Alphabet
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        #endregion
    }
}
