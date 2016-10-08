using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Bio.Properties;

namespace Bio.Util
{
    /// <summary>
    ///     A stream reader that can skip over comments in the input.
    /// </summary>
    public class CommentedStreamReader : StreamReader
    {
        /// <summary>
        /// The string that tells the parser how comments are marked.
        /// </summary>
        public const string CommentHeader = "Comment token:";

        private bool haveReadFirstLine;
        private bool isCommented;

        /// <summary>
        ///     Create a CommentedStreamReader from a FileInfo
        /// </summary>
        /// <param name="fileInfo">The fileinfo to read.</param>
        public CommentedStreamReader(FileInfo fileInfo)
            : base(fileInfo.OpenRead())
        {
        }

        /// <summary>
        ///     Create a CommentedStreamReader from a file
        /// </summary>
        /// <param name="filename">The file to read</param>
        public CommentedStreamReader(string filename)
            : base(filename)
        {
        }

        /// <summary>
        ///     Create a CommentedStreamReader from a stream
        /// </summary>
        /// <param name="stream">The stream to create a CommentedStreamReader from</param>
        public CommentedStreamReader(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        ///     Create a CommentedStreamReader from a TextReader. Loads the entire contents of the text reader into memory.
        /// </summary>
        /// <param name="reader">The TextReader to create a CommentedStreamReader from</param>
        public CommentedStreamReader(TextReader reader)
            : base(new MemoryStream(new UTF8Encoding().GetBytes(reader.ReadToEnd())))
        {
        }

        /// <summary>
        ///     The string used to mark a line as a comment line.
        /// </summary>
        public string CommentToken { get; private set; }

        /// <summary>
        ///     Returns the next noncomment line
        /// </summary>
        /// <returns>The next noncomment line</returns>
        public override string ReadLine()
        {
            return this.ReadCommentOrNonCommentLine(false);
        }

        /// <summary>
        ///     Returns the next comment line
        /// </summary>
        /// <returns>A comment line</returns>
        public string ReadCommentLine()
        {
            return this.ReadCommentOrNonCommentLine(true);
        }

        /// <summary>
        ///     Read the next line
        /// </summary>
        /// <param name="returnComment">if true, returns the next comment line; otherwise, returns the next noncomment line.</param>
        /// <returns>the next line</returns>
        protected string ReadCommentOrNonCommentLine(bool returnComment)
        {
            string line = base.ReadLine();

            if (line == null)
            {
                return null;
            }
            if (!this.haveReadFirstLine)
            {
                this.haveReadFirstLine = true;
                if (line.StartsWith(CommentHeader))
                {
                    this.CommentToken = line.Substring(CommentHeader.Length);
                    this.isCommented = true;
                    Helper.CheckCondition(
                        this.CommentToken.Length > 0,
                        () => "Expected non-zero length comment");
                    if (returnComment)
                    {
                        return line;
                    }
                    return this.ReadCommentOrNonCommentLine(returnComment);
                }
                if (returnComment)
                {
                    return this.ReadCommentOrNonCommentLine(returnComment);
                }
                return line;
            }
            if (this.isCommented && line.StartsWith(this.CommentToken))
            {
                if (returnComment)
                {
                    return line;
                }
                return this.ReadCommentOrNonCommentLine(returnComment);
            }
            if (returnComment)
            {
                return this.ReadCommentOrNonCommentLine(returnComment);
            }
            return line;
        }

        /// <summary>
        /// Reads the next character from the input stream and advances the character position by one character.
        /// </summary>
        /// <returns>
        /// The next character from the input stream represented as an <see cref="T:System.Int32"/> object, or -1 if no more characters are available.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception><filterpriority>1</filterpriority>
        public override int Read()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads a specified maximum of characters from the current stream into a buffer, beginning at the specified index.
        /// </summary>
        /// <returns>
        /// The number of characters that have been read, or 0 if at the end of the stream and no data was read. The number will be less than or equal to the <paramref name="count"/> parameter, depending on whether the data is available within the stream.
        /// </returns>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index"/> and (<paramref name="count"/>) replaced by the characters read from the current source. </param><param name="index">The index of <paramref name="buffer"/> at which to begin writing. </param><param name="count">The maximum number of characters to read. </param><exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>. </exception><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is negative. </exception><exception cref="T:System.IO.IOException">An I/O error occurs, such as the stream is closed. </exception><filterpriority>1</filterpriority>
        public override int Read(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the next available character but does not consume it.
        /// </summary>
        /// <returns>
        /// An integer representing the next character to be read, or -1 if there are no characters to be read or if the stream does not support seeking.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception><filterpriority>1</filterpriority>
        public override int Peek()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads a specified maximum number of characters from the current stream and writes the data to a buffer, beginning at the specified index.
        /// </summary>
        /// <returns>
        /// The number of characters that have been read. The number will be less than or equal to <paramref name="count"/>, depending on whether all input characters have been read.
        /// </returns>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index"/> and (<paramref name="count"/>) replaced by the characters read from the current source.</param><param name="index">The position in <paramref name="buffer"/> at which to begin writing.</param><param name="count">The maximum number of characters to read.</param><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception><exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is negative. </exception><exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.StreamReader"/> is closed. </exception><exception cref="T:System.IO.IOException">An I/O error occurred. </exception>
        public override int ReadBlock(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads all characters from the current position to the end of the stream.
        /// </summary>
        /// <returns>
        /// The rest of the stream as a string, from the current position to the end. If the current position is at the end of the stream, returns an empty string ("").
        /// </returns>
        /// <exception cref="T:System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string. </exception><exception cref="T:System.IO.IOException">An I/O error occurs. </exception><filterpriority>1</filterpriority>
        public override string ReadToEnd()
        {
            var sb = new StringBuilder();
            string line;
            while (null != (line = this.ReadLine()))
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Read all the comments from the stream
        /// </summary>
        /// <returns>A sequence of comment lines.</returns>
        public IEnumerable<string> ReadAllComments()
        {
            string line;
            while (null != (line = this.ReadCommentLine()))
            {
                yield return line;
            }
        }
    }
}