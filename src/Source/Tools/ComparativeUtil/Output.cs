using System;

namespace ComparativeUtil
{
    /// <summary>
    /// Internal logging class used by utility
    /// </summary>
    public static class Output
    {
        private static OutputLevel traceLevel;

        /// <summary>
        /// The current output level
        /// </summary>
        public static OutputLevel TraceLevel
        {
            get { return traceLevel | OutputLevel.Required | OutputLevel.Results | OutputLevel.Error; }
            set { traceLevel = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        static Output()
        {
            TraceLevel = 0;
        }

        /// <summary>
        /// Writes the message out to the console based on the current output level
        /// </summary>
        public static void Write(OutputLevel level, string message = "", params object[] args)
        {
            if ((level & TraceLevel) == 0)
                return;

            if (level == OutputLevel.Error)
                Console.Error.Write(message, args);
            else
                Console.Write(message, args);
        }

        /// <summary>
        /// Writes the message out to the console based on the current output level
        /// </summary>
        public static void WriteLine(OutputLevel level, string message = "", params object[] args)
        {
            if ((level & TraceLevel) == 0)
                return;

            if (level == OutputLevel.Error)
                Console.Error.WriteLine(message, args);
            else
                Console.WriteLine(message, args);
        }
    }

    /// <summary>
    /// The output level requested
    /// </summary>
    [Flags]
    public enum OutputLevel
    {
        /// <summary>
        /// Required
        /// </summary>
        Required = 1,
        /// <summary>
        /// Error
        /// </summary>
        Error = 2,
        /// <summary>
        /// Info
        /// </summary>
        Information = 4,
        /// <summary>
        /// Verbose (all)
        /// </summary>
        Verbose = 8,
        /// <summary>
        /// Output results
        /// </summary>
        Results = 16,
    }
}
