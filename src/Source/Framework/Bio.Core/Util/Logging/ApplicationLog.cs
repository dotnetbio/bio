using System;
using System.Diagnostics;
using System.Globalization;

namespace Bio.Util.Logging
{
    // use this when you want a single, static logger for the whole
    // application (the usual case)

    /// <summary>
    /// log is a class that implements straightforward logging to a text file, 
    /// and tries to minimize clutter of the code that uses it.
    /// </summary>
    public static class ApplicationLog
    {
        /// <summary>
        /// Platform specific hook to get output from log.
        /// </summary>
        public static event Action<string> WriteHandler;

        /// <summary>
        /// Write a single string to the writer.
        /// </summary>
        /// <param name="output">the string</param>
        /// <returns>the string</returns>
        public static string Write(string output)
        {
            if (WriteHandler != null)
            {
                WriteHandler(output);
            }

            return output;
        }

        /// <summary>
        /// Write a formatted string to the output.
        /// Same syntax as Stream.Write().
        /// </summary>
        /// <param name="fmt">the format string</param>
        /// <param name="args">additional arguments</param>
        /// <returns>the formatted string that was written</returns>
        public static string Write(string fmt, params object[] args)
        {
            string ret = string.Empty;
            if (WriteHandler != null)
            {
                ret = string.Format(CultureInfo.InvariantCulture, fmt, args);
                WriteHandler(ret);
            }
            else
                Debug.WriteLine(fmt, args);

            return ret;
        }

        /// <summary>
        /// Write a plain string to the output, then a newline.
        /// </summary>
        /// <param name="output">the string</param>
        /// <returns>the string</returns>
        public static string WriteLine(string output)
        {
            return Write(output + Environment.NewLine);
        }

        /// <summary>
        /// Write a formatted string to the output, then a newline.
        /// Same syntax as Stream.WriteLine().
        /// </summary>
        /// <param name="fmt">the format string</param>
        /// <param name="args">additional arguments</param>
        /// <returns>the formatted string that was written</returns>
        public static string WriteLine(string fmt, params object[] args)
        {
            return Write(fmt + Environment.NewLine, args);
        }

        /// <summary>
        /// Write a formatted string to output, with the current date/time
        /// prepended, and a newline appended.
        /// </summary>
        /// <param name="fmt">the format string</param>
        /// <param name="args">additional arguments</param>
        /// <returns>the formatted string (including date/time) that was written</returns>
        public static string WriteTime(string fmt, params object[] args)
        {
            return WriteLine(DateTime.Now.ToString("u", CultureInfo.InvariantCulture) + ": " 
                + string.Format(CultureInfo.InvariantCulture, fmt, args));
        }

        /// <summary>
        /// Write an exception's message, its inner exception's message, and the
        /// stack trace to the log.
        /// </summary>
        /// <param name="exception">the Exception</param>
        /// <returns>the formatted string that was written</returns>
        public static string Exception(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            string ret;
            if (exception.InnerException == null)
            {
                ret = string.Format(CultureInfo.InvariantCulture, "Exception: {0}\r\n{1}\r\n",
                exception.Message, exception.StackTrace);
            }
            else
            {
                ret = string.Format(CultureInfo.InvariantCulture, "Exception: {0} (innerException {1})\r\n{2}\r\n",
                exception.Message, exception.InnerException.Message, exception.StackTrace);
            }
            
            return WriteLine(ret);
        }
    }
}