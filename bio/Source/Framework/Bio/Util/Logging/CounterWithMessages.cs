using System;

namespace Bio.Util.Logging
{
    /// <summary>
    /// Writes messages to the console every so many increments.
    /// </summary>
    public class CounterWithMessages
    {
        private CounterWithMessages()
        {
        }

        private string FormatString;
        private int MessageInterval;

        /// <summary>
        /// The number of increments so far.
        /// </summary>
        public int Index { get; private set; }
        private int? CountOrNull;

        /// <summary>
        /// If true, doesn't send any messages to the console; just counts.
        /// </summary>
        public bool Quiet
        {
            get { return _quiet; }
            set { _quiet = value; }
        }
        private bool _quiet = false;

        /// <summary>
        /// Create a counter that will will output messages to the console every so many increments. Incrementing is thread-safe.
        /// </summary>
        /// <param name="formatStringWithOneOrTwoPlaceholders">A format string with containing at least {0} and, optionally, {1}.</param>
        /// <param name="messageInterval">How often messages should be output, in increments.</param>
        /// <param name="totalCountOrNull">The total number of increments, or null if not known.</param>
        /// <returns>A counter</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
        public CounterWithMessages(string formatStringWithOneOrTwoPlaceholders, int messageInterval, int? totalCountOrNull)
            : this(formatStringWithOneOrTwoPlaceholders, messageInterval, totalCountOrNull, false)
        {
        }

        /// <summary>
        /// Create a counter that will will output messages to the console every so many increments. Incrementing is thread-safe.
        /// </summary>
        /// <param name="formatStringWithOneOrTwoPlaceholders">A format string with containing at least {0} and, optionally, {1}.</param>
        /// <param name="messageInterval">How often messages should be output, in increments.</param>
        /// <param name="totalCountOrNull">The total number of increments, or null if not known.</param>
        /// <param name="quiet">if true, doesn't output to the console.</param>
        /// <returns>A counter</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
        public CounterWithMessages(string formatStringWithOneOrTwoPlaceholders, int messageInterval, int? totalCountOrNull, bool quiet)
        {
            FormatString = formatStringWithOneOrTwoPlaceholders;
            MessageInterval = messageInterval;
            Index = -1;
            CountOrNull = totalCountOrNull;
            Quiet = quiet;
        }

        /// <summary>
        /// Increment the counter by one. Incrementing is thread-safe.
        /// </summary>
        public int Increment()
        {
            lock (this)
            {
                ++Index;
                if (Index % MessageInterval == 0 && !Quiet)
                {
                    if (null == CountOrNull)
                    {
                        Console.WriteLine(FormatString, Index);
                    }
                    else
                    {
                        Console.WriteLine(FormatString, Index, CountOrNull.Value);
                    }
                }
                return Index;
            }
        }
    }
}
