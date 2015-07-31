using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Bio.Util;

namespace Bio.IO
{
    /// <summary>
    /// Extensions related to INamedStreamCreator
    /// </summary>
    public static class INamedStreamCreatorExtensions
    {
        /// <summary>
        /// Turn a file name (a string) into a INamedStreamCreator
        /// </summary>
        /// <param name="fileName">The name of the file to turn into a INamedStreamCreator.</param>
        /// <param name="name">(Optional) The name of the INamedStreamCreator. If null, fileName is used.</param>
        /// <returns>a INamedStreamCreator</returns>
        public static INamedStreamCreator ToNamedStreamCreatorFromFileName(this string fileName, string name = null)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (string.IsNullOrEmpty(name))
            {
                name = fileName;
            }
            return new NamedStreamCreator(name, () => new FileStream(fileName, FileMode.Open, FileAccess.Read));
        }

        /// <summary>
        /// Turn a FileInfo into a INamedStreamCreator
        /// </summary>
        /// <param name="fileInfo">The FileInfo to turn into a INamedStreamCreator.</param>
        /// <param name="name">(Optional) The name of the INamedStreamCreator. If null, the FileInfo's name is used.</param>
        /// <returns>a INamedStreamCreator</returns>
        public static INamedStreamCreator ToNamedStreamCreator(this FileInfo fileInfo, string name = null)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            if (null == name)
            {
                name = fileInfo.Name;
            }
            return new NamedStreamCreator(name, fileInfo.OpenRead);
        }


        /// <summary>
        /// Turn a resource into a INamedStreamCreator
        /// </summary>
        /// <param name="assembly">The assembly containing the resource</param>
        /// <param name="resourceName">The name of the resource</param>
        /// <param name="name">(Optional) The name of the INamedStreamCreator. If null, resourceName is used.</param>
        /// <returns>a INamedStreamCreator</returns>
        public static INamedStreamCreator ToNamedStreamCreator(this Assembly assembly, string resourceName, string name = null)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException("resourceName");

            if (string.IsNullOrEmpty(name))
            {
                name = resourceName;
            }

            return new NamedStreamCreator(name, () => assembly.GetManifestResourceStream(resourceName));
        }


        /// <summary>
        /// Turn a string into a INamedStreamCreator. The string will be used as the data for the stream.
        /// </summary>
        /// <param name="text">The text to turn into a INamedStreamCreator.</param>
        /// <param name="name">(Optional) The name of the INamedStreamCreator. If null, string is used for both data and for the name.</param>
        /// <returns>a INamedStreamCreator</returns>
        public static INamedStreamCreator ToNamedStreamCreatorFromString(this string text, string name = null)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (string.IsNullOrEmpty(name))
            {
                name = text;
            }
            byte[] byteBuffer = System.Text.Encoding.UTF8.GetBytes(text);
            return new NamedStreamCreator(name, () => new MemoryStream(byteBuffer));
        }


        /// <summary>
        /// Enumerates the lines of a INamedStreamCreator without comments.
        /// </summary>
        /// <param name="namedStreamCreator">The INamedStreamCreator to read from</param>
        /// <returns>an enumerator of lines</returns>
        public static IEnumerable<string> ReadEachUncommentedLine(this INamedStreamCreator namedStreamCreator)
        {
            if (namedStreamCreator == null)
                throw new ArgumentNullException("namedStreamCreator");

            using (TextReader textReader = namedStreamCreator.OpenUncommentedText())
            {
                string line;
                while (null != (line = textReader.ReadLine()))
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Enumerates the lines of a INamedStreamCreator.
        /// </summary>
        /// <param name="namedStreamCreator">The INamedStreamCreator to read from</param>
        /// <returns>an enumerator of lines</returns>
        public static IEnumerable<string> ReadEachLine(this INamedStreamCreator namedStreamCreator)
        {
            if (namedStreamCreator == null)
                throw new ArgumentNullException("namedStreamCreator");

            using (TextReader textReader = namedStreamCreator.OpenText())
            {
                string line;
                while (null != (line = textReader.ReadLine()))
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Return the first line of namedStreamCreator after any comments.
        /// </summary>
        /// <param name="namedStreamCreator">The namedStreamCreator from which to read.</param>
        /// <returns>The first line of a file after skipping any comments.</returns>
        public static string ReadUncommentedLine(this INamedStreamCreator namedStreamCreator)
        {
            return FileUtils.ReadLine(namedStreamCreator);
        }

        /// <summary>
        /// Open a INamedStreamCreator for reading as text. This should be used is a Using statement. (According to http://msdn.microsoft.com/en-us/library/hh40558e.aspx, the stream will be disposed of, too.)
        /// </summary>
        /// <param name="namedStreamCreator">The INamedStreamCreator to read from</param>
        /// <returns>a StreamReader</returns>
        public static StreamReader OpenText(this INamedStreamCreator namedStreamCreator)
        {
            if (namedStreamCreator == null)
                throw new ArgumentNullException("namedStreamCreator");

            Stream stream = namedStreamCreator.Creator();
            return new StreamReader(stream);
        }

        /// <summary>
        /// Open a INamedStreamCreator for reading as uncommented text. This should be used is a Using statement. (According to http://msdn.microsoft.com/en-us/library/hh40558e.aspx, the stream will be disposed of, too.)
        /// </summary>
        /// <param name="namedStreamCreator">The INamedStreamCreator to read from</param>
        /// <returns>a StreamReader</returns>
        public static CommentedStreamReader OpenUncommentedText(this INamedStreamCreator namedStreamCreator)
        {
            if (namedStreamCreator == null)
                throw new ArgumentNullException("namedStreamCreator");

            Stream stream = namedStreamCreator.Creator();
            return new CommentedStreamReader(stream);
        }


        /// <summary>
        /// Returns all the data of a INamedStreamCreator as a string.
        /// </summary>
        /// <param name="namedStreamCreator">The INamedStreamCreator to read from</param>
        /// <returns>The data as text</returns>
        public static string ReadToEnd(this INamedStreamCreator namedStreamCreator)
        {
            if (namedStreamCreator == null)
                throw new ArgumentNullException("namedStreamCreator");

            using (TextReader textReader = namedStreamCreator.OpenText())
            {
                return textReader.ReadToEnd();
            }
        }


        /// <summary>
        /// Write the context of a INamedStreamCreator to a stream.
        /// </summary>
        /// <param name="namedStreamCreator">The INamedStreamCreator to read from</param>
        /// <param name="stream">The stream to write to</param>
        public static void WriteToStream(this INamedStreamCreator namedStreamCreator, Stream stream)
        {
            if (namedStreamCreator == null)
                throw new ArgumentNullException("namedStreamCreator");

            if (stream == null)
                throw new ArgumentNullException("stream");

            using (TextWriter textWriter = new StreamWriter(stream))
            {
                foreach (string line in namedStreamCreator.ReadEachLine())
                {
                    textWriter.WriteLine(line);
                }
            }
        }
    }
}
