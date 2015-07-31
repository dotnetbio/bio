using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bio.IO.AppliedBiosystems.DataParsers;
using Bio.IO.AppliedBiosystems.Model;
using Bio.Properties;

namespace Bio.IO.AppliedBiosystems
{
    /// <summary>
    ///     Parses an applied biosystems data file format as defined in:
    ///     http://www6.appliedbiosystems.com/support/software_community/ABIF_File_Format.pdf
    /// </summary>
    public sealed class Ab1Parser : ISequenceParser
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public Ab1Parser()
        {
            this.Alphabet = Alphabets.AmbiguousDNA;
        }

        /// <summary>
        ///     Name.
        /// </summary>
        public string Name
        {
            get
            {
                return Resource.APPLIEDBIOSYSTEMS_NAME;
            }
        }

        /// <summary>
        ///     Description.
        /// </summary>
        public string Description
        {
            get
            {
                return Resource.APPLIEDBIOSYSTEMS_DESCRIPTION;
            }
        }

        /// <summary>
        ///     Supported file types.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Resource.APPLIEDBIOSYSTEMS_FILETYPES;
            }
        }

        /// <summary>
        ///     Parser the file.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISequence> Parse(Stream stream)
        {
            using (var binaryReader = new BinaryReader(stream))
            {
                yield return Ab1ContextToSequenceConverter.Convert(Parse(binaryReader, this.Alphabet));
            }
        }

        /// <summary>
        /// Parse a single sequence from the stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Sequence</returns>
        public ISequence ParseOne(Stream stream)
        {
            return Parse(stream).FirstOrDefault();
        }

        /// <summary>
        ///     Alphabet
        /// </summary>
        public IAlphabet Alphabet { get; set; }

        /// <summary>
        ///     Parsers the files binary content into a abi parser context using the DNA alphabet.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IParserContext Parse(BinaryReader reader)
        {
            return Parse(reader, null);
        }

        /// <summary>
        ///     Parsers the files binary content into a abi parser context using
        ///     the specified alphabet.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="alphabet"></param>
        /// <returns></returns>
        public static IParserContext Parse(BinaryReader reader, IAlphabet alphabet)
        {
            // Default to the DNA alphabet
            if (alphabet == null)
            {
                alphabet = Alphabets.DNA;
            }

            var rawData = new Ab1Header(reader);
            IVersionedDataParser dataParser = DataParserFactory.GetParser(rawData.MajorVersion);
            var context = new ParserContext { Header = rawData, Reader = reader, Alphabet = alphabet, };
            dataParser.ParseData(context);

            return context;
        }
    }
}