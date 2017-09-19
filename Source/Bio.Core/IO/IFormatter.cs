using System.Collections.Generic;
using System.IO;

namespace Bio.IO
{
    /// <summary>
    /// Interface that defines the common properties for a formatter.
    /// All other formatters must extend this Interface.
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Gets the name of the formatter being implemented.
        /// This is intended to give the developer name of the formatter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the formatter being implemented.
        /// This is intended to give the developer some 
        /// information of the formatter.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the file extensions that the formatter will support.
        /// If multiple extensions are supported then this property 
        /// will return a string containing all extensions with a ',' delimited.
        /// </summary>
        string SupportedFileTypes { get; }
    }

    /// <summary>
    /// Generic formatter for specific types
    /// </summary>
    /// <typeparam name="T">Type to write</typeparam>
    public interface IFormatter<in T> : IFormatter
    {
        /// <summary>
        /// Writes a single data entry.
        /// </summary>
        /// <param name="writer">Stream to write to.</param>
        /// <param name="data">The data to write.</param>
        void Format(Stream writer, T data);

        /// <summary>
        /// Writes a set of entries.
        /// </summary>
        /// <param name="writer">Stream to write to.</param>
        /// <param name="data">The data to write.</param>
        void Format(Stream writer, IEnumerable<T> data);

    }
}
