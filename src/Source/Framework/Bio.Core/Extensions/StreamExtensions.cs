using System;
using System.IO;
using System.Text;

namespace Bio.Extensions
{
    /// <summary>
    /// Extensions for the Stream
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Opens the given stream for reading with a StreamReader.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="encoding">Encoding, defaults to UTF8</param>
        /// <param name="detectEncodingFromByteOrderMarks"></param>
        /// <param name="bufferSize">Buffer size, defaults to 1k</param>
        /// <param name="leaveOpen">True to keep underlying stream open on disposal.</param>
        /// <returns>StreamReader</returns>
        public static StreamReader OpenRead(this Stream stream, Encoding encoding = null, bool detectEncodingFromByteOrderMarks = true, int bufferSize = 1024, bool leaveOpen=true)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (encoding == null)
                encoding = AsciiEncoding.Default;

            return new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen);
        }

        /// <summary>
        /// Opens the given stream for writing with a StreamWriter.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="encoding">Encoding, defaults to UTF8</param>
        /// <param name="bufferSize">Buffer size, defaults to 1k</param>
        /// <param name="leaveOpen">True to keep underlying stream open on disposal.</param>
        /// <returns>StreamWriter</returns>
        public static StreamWriter OpenWrite(this Stream stream, Encoding encoding = null, int bufferSize = 1024, bool leaveOpen = true)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (encoding == null)
                encoding = AsciiEncoding.Default;

            return new StreamWriter(stream, encoding, bufferSize, leaveOpen);
        }

    }
}
