using System.Collections.Generic;
using System.IO;
using Bio.Algorithms.Alignment;

namespace Bio.IO
{
    /// <summary>
    /// Interface that defines a contract for parser to parse sequence alignment files. 
    /// For advanced users, the ability to select an encoding for the internal memory 
    /// representation is provided. Implementations also have a default encoding for 
    /// each alphabet that may be encountered.
    /// </summary>
    public interface ISequenceAlignmentParser : IParser
    {
        /// <summary>
        /// Parses a list of biological sequence alignment texts from a reader.
        /// </summary>
        /// <param name="reader">A reader for a biological sequence alignment text.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        IList<ISequenceAlignment> Parse(TextReader reader);

        /// <summary>
        /// Parses a list of biological sequence alignment texts from a reader.
        /// </summary>
        /// <param name="reader">A reader for a biological sequence alignment text.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        ISequenceAlignment ParseOne(TextReader reader);

        /// <summary>
        /// Parses a list of biological sequence alignment texts from a file.
        /// </summary>
        /// <param name="dataSource">The source of a biological sequence alignment. If its a file, this parameter value will be the filename.</param>
        /// <returns>The list of parsed ISequenceAlignment objects.</returns>
        IList<ISequenceAlignment> Parse(string dataSource);

        /// <summary>
        /// Parses a single biological sequence alignment text from a file.
        /// </summary>
        /// <param name="dataSource">The source of a biological sequence alignment file. If its a file, this parameter value will be the filename.</param>
        /// <returns>The parsed ISequenceAlignment object.</returns>
        ISequenceAlignment ParseOne(string dataSource);
    }
}
