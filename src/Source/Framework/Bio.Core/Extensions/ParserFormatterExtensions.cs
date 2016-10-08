using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Bio
{
    /// <summary>
    /// Extensions to the parser/formatter framework to support Open/Close/Dispose semantics.
    /// </summary>
    internal static class ParserFormatterExtensions<T> where T : class
    {
        /// <summary>
        /// This associates some data to the given parser to hold the filename and filestream.
        /// </summary>
        private static readonly ConditionalWeakTable<object, Tuple<string, FileStream>> fileData
            = new ConditionalWeakTable<object, Tuple<string, FileStream>>();

        class DisposableParser : IDisposable
        {
            private readonly T parser;

            public DisposableParser(T parser)
            {
                this.parser = parser;
            }

            public void Dispose()
            {
                Close(this.parser);
            }
        }

        /// <summary>
        /// Opens a sequence file using the parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="filename">File to open</param>
        /// <returns>Disposable object to close the stream.</returns>
        public static IDisposable Open(T parser, string filename)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }

            lock (fileData)
            {
                fileData.Add(parser, new Tuple<string, FileStream>(filename, null));
            }

            return new DisposableParser(parser);
        }

        /// <summary>
        /// Parses the sequences from the open file.
        /// </summary>
        /// <param name="parser">Sequence Parser</param>
        /// <param name="writable">True for a writable stream</param>
        /// <returns>Set of parsed sequences.</returns>
        public static Stream GetOpenStream(T parser, bool writable)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            FileStream fs = null;
            lock (fileData)
            {
                Tuple<string, FileStream> data;
                if (fileData.TryGetValue(parser, out data))
                {
                    if (data.Item2 != null)
                    {
                        fs = data.Item2;
                    }
                    else
                    {
                        fs = new FileStream(data.Item1, (writable) ? FileMode.Create : FileMode.Open);
                        fileData.Remove(parser);
                        fileData.Add(parser, new Tuple<string, FileStream>(data.Item1, fs));
                    }
                }
            }
            return fs;
        }

        /// <summary>
        /// Closes the parser.
        /// </summary>
        /// <param name="parser">Parser</param>
        public static void Close(T parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("parser");
            }

            lock (fileData)
            {
                Tuple<string, FileStream> data;
                if (fileData.TryGetValue(parser, out data))
                {
                    if (data.Item2 != null)
                    {
                        data.Item2.Dispose();
                    }
                    fileData.Remove(parser);
                }
            }
        }
    }
}
