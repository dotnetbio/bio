using System;

namespace Bio.Util
{
    /// <summary>
    /// Writes messages to the console every so many increments.
    /// </summary>
    public class CounterWithMessages
    {
        private readonly string formatString;
        private readonly int messageInterval;
        private int? countOrNull;

        /// <summary>
        /// The number of increments so far.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// If true, doesn't send any messages to the console; just counts.
        /// </summary>
        public bool Quiet { get; set; }

        /// <summary>
        /// Create a counter that will output messages to the console every so many increments. Incrementing is thread-safe.
        /// </summary>
        /// <param name="formatStringWithOneOrTwoPlaceholders">A format string with containing at least {0} and, optionally, {1}.</param>
        /// <param name="messageInterval">How often messages should be output, in increments.</param>
        /// <param name="totalCountOrNull">The total number of increments, or null if not known.</param>
        /// <returns>A counter</returns>
        public CounterWithMessages(string formatStringWithOneOrTwoPlaceholders, int messageInterval, int? totalCountOrNull)
            : this(formatStringWithOneOrTwoPlaceholders, messageInterval, totalCountOrNull, false)
        {
        }

        /// <summary>
        /// Create a counter that will output messages to the console every so many increments. Incrementing is thread-safe.
        /// </summary>
        /// <param name="formatStringWithOneOrTwoPlaceholders">A format string with containing at least {0} and, optionally, {1}.</param>
        /// <param name="messageInterval">How often messages should be output, in increments.</param>
        /// <param name="totalCountOrNull">The total number of increments, or null if not known.</param>
        /// <param name="quiet">if true, doesn't output to the console.</param>
        /// <returns>A counter</returns>
        public CounterWithMessages(string formatStringWithOneOrTwoPlaceholders, int messageInterval, int? totalCountOrNull, bool quiet)
        {
            this.formatString = formatStringWithOneOrTwoPlaceholders;
            this.messageInterval = messageInterval;
            this.countOrNull = totalCountOrNull;
            this.Index = -1;
            this.Quiet = quiet;
        }

        /// <summary>
        /// Increment the counter by one. Incrementing is thread-safe.
        /// </summary>
        public int Increment()
        {
            lock (this)
            {
                ++this.Index;
                if (this.Index % this.messageInterval == 0 && !this.Quiet)
                {
                    if (null == this.countOrNull)
                    {
                        Console.WriteLine(this.formatString, this.Index);
                    }
                    else
                    {
                        Console.WriteLine(this.formatString, this.Index, this.countOrNull.Value);
                    }
                }
                return this.Index;
            }
        }
    }
}
