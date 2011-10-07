using System;
using System.IO;

namespace Bio.IO
{
    /// <summary>
    /// Implementations of this interface write an ISequence to a particular location, usually a
    /// file. The output is formatted according to the particular file format.
    /// </summary>
    public interface ISequenceFormatter : IFormatter, IDisposable
    {
        /// <summary>
        /// Opens specified file to write.
        /// </summary>
        /// <param name="filename">Filename to open.</param>
        void Open(string filename);

        /// <summary>
        /// Opens the specified stream for writing sequences.
        /// </summary>
        /// <param name="outStream">StreamWriter to use.</param>
        void Open(StreamWriter outStream);

        /// <summary>
        /// Writes an ISequence.
        /// </summary>
        /// <param name="sequence">The sequence to write.</param>
        void Write(ISequence sequence);

        /// <summary>
        /// Closes underlying stream if any.
        /// </summary>
        void Close();
    }
}
