using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bio.IO
{
    /// <summary>
    /// Parser decorator to convert IParser to ISequenceParser, this can be used to take
    /// specializations of IParser(T) and turn them into ISequenceParser (e.g. FastQ).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    internal class SequenceParserDecorator<T, TItem> : ISequenceParser
        where T : class, IParserWithAlphabet<TItem>
        where TItem : ISequence
    {
        private readonly T innerParser;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="innerParser"></param>
        public SequenceParserDecorator(T innerParser)
        {
            this.innerParser = innerParser;
        }

        /// <summary>
        /// Parses a list of biological sequence texts from a given stream.
        /// </summary>
        /// <param name="stream">The stream to pull the data from</param>
        /// <returns>The collection of parsed ISequence objects.</returns>
        public IEnumerable<ISequence> Parse(Stream stream)
        {
            return innerParser.Parse(stream).Cast<ISequence>();
        }

        /// <summary>
        /// Parse a single sequence from the stream.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <returns>Sequence</returns>
        public ISequence ParseOne(Stream stream)
        {
            return innerParser.ParseOne(stream);
        }

        /// <summary>
        /// Gets the name of the parser being implemented. 
        /// This is intended to give the developer name of the parser.
        /// </summary>
        public string Name { get { return innerParser.Name; } }

        /// <summary>
        /// Gets the description of the parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser.
        /// </summary>
        public string Description { get { return innerParser.Description; } }

        /// <summary>
        /// Gets the file extensions that the parser supports.
        /// If multiple extensions are supported then this property 
        /// will return a string containing all extensions with a ',' delimited.
        /// </summary>
        public string SupportedFileTypes { get { return innerParser.SupportedFileTypes; } }

        /// <summary>
        /// Gets or sets the alphabet to use for parsed ISequence objects.  If this is not set, the alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        public IAlphabet Alphabet
        {
            get { return innerParser.Alphabet;  }
            set { innerParser.Alphabet = value;  }
        }
    }
}
