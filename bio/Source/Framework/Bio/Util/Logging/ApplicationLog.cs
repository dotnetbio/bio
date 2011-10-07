using System;
using System.Globalization;
using System.IO;
using System.Threading;

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
        /// File info field
        /// </summary>
        private static FileInfo finfo;

        /// <summary>
        /// Test writer
        /// </summary>
        private static TextWriter writer;

        /// <summary>
        /// Auto flush
        /// </summary>
        private static bool autoflush = true;

        /// <summary>
        /// Initializes static members of the ApplicationLog class
        /// </summary>
        static ApplicationLog()
        {
            if (!ApplicationLog.Ready)
            {
                ApplicationLog.Open(Properties.Resource.LogFileName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether flush occurs after each write or critical writes
        /// if true (default), a flush occurs after each write, to prevent loss of messages.
        /// if false, calling code should Flush() after critical writes.
        /// </summary>
        public static bool Autoflush
        {
            get
            {
                return autoflush;
            }

            set
            {
                autoflush = value;
            }           
        }

        /// <summary>
        /// Gets a value indicating whether there is a valid writer. 
        /// returns true if there is a valid writer, if not
        /// in this state, writes will be no-ops.
        /// </summary>
        public static bool Ready
        {
            get
            {
                return writer != null;
            }
        }

        /// <summary>
        /// Opens the specified filename for writing. If the file already
        /// exists, it is overwritten. If a different log file is open, it's 
        /// closed before the new one is opened.
        /// </summary>
        /// <param name="fileName">the filename</param>
        public static void Open(string fileName)
        {
            Close();
            finfo = new FileInfo(fileName);

            try
            {
                writer = new StreamWriter(finfo.FullName);
            }
            catch (UnauthorizedAccessException)
            {
                fileName = Path.GetTempPath() + Path.GetFileName(fileName);
                finfo = new FileInfo(fileName);
                writer = new StreamWriter(finfo.FullName);
            }
            catch (IOException)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = fileName.Replace(".log", DateTime.Now.Ticks + ".log");
                    finfo = new FileInfo(fileName);
                    writer = new StreamWriter(finfo.FullName);
                }
            }
        }

        /// <summary>
        /// If log is already open with the same filename, do nothing.
        /// Otherwise open (in append mode, if file exists).
        /// </summary>
        /// <remarks>
        /// Be aware that log files that are never truncated can become very large.
        /// </remarks>
        /// <param name="fileName">the filename</param>
        public static void Reopen(string fileName)
        {
            FileInfo temp = new FileInfo(fileName);
            if (Ready && finfo.FullName == temp.FullName)
            {
                return;
            }

            OpenAppend(fileName);
        }

        /// <summary>
        /// Open the specified filename (in append mode if it exists).
        /// </summary>
        /// <param name="fileName">the filename</param>
        public static void OpenAppend(string fileName)
        {
            Close();
            finfo = new FileInfo(fileName);
            writer = new StreamWriter(finfo.FullName, true);
        }

        /// <summary>
        /// If a writer is open, flush, close, and dispose it.
        /// </summary>
        public static void Close()
        {
            if (writer != null)
            {
                Flush();
                writer.Close();
                writer.Dispose();
            }
        }

        // the various write methods return what they wrote, as a convenience.

        /// <summary>
        /// Write a single string to the writer.
        /// </summary>
        /// <param name="output">string to write</param>
        /// <returns>the string</returns>
        public static string Write(string output)
        {
            if (Ready)
            {
                writer.Write(output);
                if (autoflush)
                {
                    Flush();
                }
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
            if (Ready)
            {
                ret = string.Format(CultureInfo.InvariantCulture, fmt, args);
                writer.Write(ret);
                if (autoflush)
                {
                    Flush();
                }
            }

            return ret;
        }

        /// <summary>
        /// Write a plain string to the output, then a newline.
        /// </summary>
        /// <param name="output">the string to write</param>
        /// <returns>the string</returns>
        public static string WriteLine(string output)
        {
            if (Ready)
            {
                writer.WriteLine(output);
                if (autoflush)
                {
                    Flush();
                }
            }

            return output + "\n";
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
            string ret = string.Empty;

            if (Ready)
            {
                ret = string.Format(CultureInfo.InvariantCulture, fmt, args) + "\n";
                writer.Write(ret);
                if (autoflush)
                {
                    Flush();
                }
            }

            return ret;
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
            string ret = string.Empty;

            if (Ready)
            {
                ret = DateTime.Now.ToString("u", CultureInfo.InvariantCulture) + ": " + string.Format(CultureInfo.InvariantCulture, fmt, args) + "\n";
                writer.Write(ret);
                if (autoflush)
                {
                    Flush();
                }
            }

            return ret;
        }

        /// <summary>
        /// Write an exception's message, its inner exception's message, and the
        /// stack trace to the log.
        /// </summary>
        /// <param name="ex">the Exception</param>
        /// <returns>the formatted string that was written</returns>
        public static string Exception(Exception ex)
        {
            string ret;
            if (ex != null)
            {
                if (ex.InnerException == null)
                {
                    ret = string.Format(CultureInfo.InvariantCulture, "Exception: {0}\n{1}\n", ex.Message, ex.StackTrace);
                }
                else
                {
                    ret = string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception: {0} (innerException {1})\n{2}\n",
                        ex.Message,
                        ex.InnerException.Message,
                        ex.StackTrace);
                }

                ApplicationLog.Write(ret);
                ApplicationLog.Flush();
                return ret;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Flush any pending writes
        /// </summary>
        public static void Flush()
        {
            if (Ready)
            {
                writer.Flush();
            }
        }
    }

#if UNDER_REVIEW
    // if instantiated, this is a null logger that does nothing.
    public class Logger
    {
        protected bool _autoflush = true;

        public virtual void Write(string output) { }
        public virtual void Write(string fmt, params object[] args) { }
        public virtual void WriteLine(string output) { }
        public virtual void WriteLine(string fmt, params object[] args) { }
        public virtual void WriteTime(string fmt, params object[] args) { }

        public virtual void Flush() { }
        public virtual void Close() { }

        public bool Autoflush
        {
            set
            {
                _autoflush = value;
            }
            get
            {
                return _autoflush;
            }
        }
    }

    public class ConsoleLogger : Logger
    {
        public ConsoleLogger() { }

        public override void Write(string output)
        {
            Thread.BeginCriticalRegion();
            Console.Write(output);
            Thread.EndCriticalRegion();
        }
        public override void Write(string fmt, params object[] args)
        {
            Thread.BeginCriticalRegion();
            Console.Write(fmt, args);
            Thread.EndCriticalRegion();
        }
        public override void WriteLine(string output)
        {
            Thread.BeginCriticalRegion();
            Console.WriteLine(output);
            Thread.EndCriticalRegion();
        }
        public override void WriteLine(string fmt, params object[] args)
        {
            Thread.BeginCriticalRegion();
            Console.WriteLine(fmt, args);
            Thread.EndCriticalRegion();
        }
        public override void WriteTime(string fmt, params object[] args)
        {
            Thread.BeginCriticalRegion();
            Console.Write(DateTime.Now.ToString("u") + ": ");
            Console.WriteLine(fmt, args);
            Thread.EndCriticalRegion();
        }
    }

    public class FileLogger : Logger
    {
        private FileInfo fInfo;
        private StreamWriter writer;

        public FileLogger(FileInfo info)
        {
            fInfo = info;
            writer = new StreamWriter(fInfo.FullName);
        }
        public FileLogger(string fname)
        {
            fInfo = new FileInfo(fname);
            writer = new StreamWriter(fInfo.FullName);
        }
        public override void Write(string output)
        {
            Thread.BeginCriticalRegion();
            writer.Write(output);
            if (_autoflush)
            {
                Flush();
            }
            Thread.EndCriticalRegion();
        }

        public override void Write(string fmt, params object[] args)
        {
            Thread.BeginCriticalRegion();
            writer.Write(fmt, args);
            if (_autoflush)
            {
                Flush();
            }
            Thread.EndCriticalRegion();
        }

        public override void Close()
        {
            Thread.BeginCriticalRegion();
            Flush();
            writer.Close();
            Thread.EndCriticalRegion();
        }

        public override void WriteLine(string output)
        {
            Thread.BeginCriticalRegion();
            writer.WriteLine(output);
            if (_autoflush)
            {
                Flush();
            }
            Thread.EndCriticalRegion();
        }
        public override void WriteLine(string fmt, params object[] args)
        {
            Thread.BeginCriticalRegion();
            writer.WriteLine(fmt, args);
            if (_autoflush)
            {
                Flush();
            }
            Thread.EndCriticalRegion();
        }
        public override void WriteTime(string fmt, params object[] args)
        {
            Thread.BeginCriticalRegion();
            writer.Write(DateTime.Now.ToString("u") + ": ");
            writer.WriteLine(fmt, args);
            if (_autoflush)
            {
                Flush();
            }
            Thread.EndCriticalRegion();
        }
        public override void Flush()
        {
            writer.Flush();
        }
    }
#endif
}
