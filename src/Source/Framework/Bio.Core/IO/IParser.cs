using System.Collections.Generic;
using System.IO;

namespace Bio.IO
{
    /// <summary>
    /// Common interface for all parsers.
    /// Used in Framework abstraction layer and auto registration mechanism.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Gets the name of the parser being implemented. 
        /// This is intended to give the developer name of the parser.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the parser being
        /// implemented. This is intended to give the
        /// developer some information of the parser.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the file extensions that the parser supports.
        /// If multiple extensions are supported then this property 
        /// will return a string containing all extensions with a ',' delimited.
        /// </summary>
        string SupportedFileTypes { get; }
    }

    /// <summary>
    /// Typed interface for parsers which return some data structure.
    /// </summary>
    /// <typeparam name="T">Returning type</typeparam>
    public interface IParser<out T> : IParser
    {
        /// <summary>
        /// Parses a list of biological sequence texts from a given stream.
        /// </summary>
        /// <param name="stream">The stream to pull the data from</param>
        /// <returns>The collection of parsed objects.</returns>
        IEnumerable<T> Parse(Stream stream);

        /// <summary>
        /// Parse a single entity from the given stream.
        /// </summary>
        /// <param name="stream">The stream to pull data from</param>
        /// <returns>Parse entity object</returns>
        T ParseOne(Stream stream);
    }

    /// <summary>
    /// Extends IParser to include an alphabet
    /// </summary>
    public interface IParserWithAlphabet<out T> : IParser<T>
    {
        /// <summary>
        /// Gets or sets the alphabet to use for parsed data objects.  If this is not set, the alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        IAlphabet Alphabet { get; set; }
    }
}
