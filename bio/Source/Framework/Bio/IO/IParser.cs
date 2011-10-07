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
}
