using System;

namespace LayoutRefinementUtil
{
    /// <summary>
    /// Internal logging class used by utility
    /// </summary>
    public static class Output
    {
        private static OutputLevel _traceLevel;

        /// <summary>
        /// The current output level
        /// </summary>
        public static OutputLevel TraceLevel
        {
            get { return _traceLevel | OutputLevel.Required | OutputLevel.Results | OutputLevel.Error; }
            set { _traceLevel = value; }
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
        Required = 1,
        Error = 2,
        Information = 4,
        Verbose = 8,
        Results = 16,
    }
}
