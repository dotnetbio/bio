using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Bio.IO
{
    /// <summary>
    /// Implementations of this interface are designed to parse a file from a particular file
    /// format to produce an ISequence or collection of ISequence.
    /// </summary>
    [InheritedExport(".NetBioSequenceParsersExport", typeof(ISequenceParser))]
    public interface ISequenceParser : IParser, IDisposable
    {
        /// <summary>
        /// Opens the specified file to parse.
        /// </summary>
        /// <param name="dataSource">Data source to open. If its a file, this parameter value will be the filename.</param>
        void Open(string dataSource);

        /// <summary>
        /// Parses a list of biological sequence texts.
        /// </summary>
        /// <returns>The collection of parsed ISequence objects.</returns>
        IEnumerable<ISequence> Parse();

        /// <summary>
        /// Parses a list of biological sequences.
        /// </summary>
        /// <returns>The collection of parsed ISequence objects.</returns>
        IEnumerable<ISequence> Parse(StreamReader reader);

        /// <summary>
        /// Closes underlying stream if any.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets or sets the alphabet to use for parsed ISequence objects.  If this is not set, the alphabet will
        /// be determined based on the file being parsed.
        /// </summary>
        IAlphabet Alphabet { get; set; }
    }
}
