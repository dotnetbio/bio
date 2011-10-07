using System;

namespace Bio.Util.Logging
{
    /// <summary>
    /// Writes messages to the console every so many increments.
    /// </summary>
    public class CounterWithMessages
    {
        /// <summary>
        /// Format string
        /// </summary>
        private string formatString;

        /// <summary>
        /// Message interval                
        /// </summary>
        private int messageInterval;

        /// <summary>
        /// Count or null
        /// </summary>
        private int? countOrNull;

        /// <summary>
        /// Quiet field
        /// </summary>
        private bool quiet = false;                        

        /// <summary>
        /// Initializes a new instance of the CounterWithMessages class that will will output messages to the console every so many increments. 
        /// Incrementing is thread-safe.
        /// </summary>
        /// <param name="formatValueWithOneOrTwoPlaceholders">A format string with containing at least {0} and, optionally, {1}.</param>
        /// <param name="messageInterval">How often messages should be output, in increments.</param>
        /// <param name="totalCountOrNull">The total number of increments, or null if not known.</param>
        /// <returns>A counter</returns>
        public CounterWithMessages(string formatValueWithOneOrTwoPlaceholders, int messageInterval, int? totalCountOrNull)
            : this(formatValueWithOneOrTwoPlaceholders, messageInterval, totalCountOrNull, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CounterWithMessages class that will will output messages to the console every so many increments. Incrementing is thread-safe.
        /// </summary>
        /// <param name="formatValueWithOneOrTwoPlaceholders">A format string with containing at least {0} and, optionally, {1}.</param>
        /// <param name="messageInterval">How often messages should be output, in increments.</param>
        /// <param name="totalCountOrNull">The total number of increments, or null if not known.</param>
        /// <param name="quiet">if true, doesn't output to the console.</param>
        /// <returns>A counter</returns>
        public CounterWithMessages(string formatValueWithOneOrTwoPlaceholders, int messageInterval, int? totalCountOrNull, bool quiet)
        {
            this.formatString = formatValueWithOneOrTwoPlaceholders;
            this.messageInterval = messageInterval;
            this.Index = -1;
            this.countOrNull = totalCountOrNull;
            this.quiet = quiet;
        }

        /// <summary>
        /// Prevents a default instance of the CounterWithMessages class from being created
        /// </summary>
        private CounterWithMessages()
        {
        }

        /// <summary>
        /// Gets the number of increments so far.
        /// </summary>
        public int Index { get; private set; }    

        /// <summary>
        /// Increment the counter by one. Incrementing is thread-safe.
        /// </summary>
        /// <returns>the Index value</returns>
        public int Increment()
        {
            lock (this)
            {
                ++this.Index;
                if (this.Index % this.messageInterval == 0 && !this.quiet)
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
