using System;
using System.Globalization;

namespace Bio.Util.Logging
{
    /// <summary>
    /// A TraceMessage is a simple message holding class.
    /// </summary>
    public class TraceMessage
    {
        /// <summary>
        /// The context where the event occurred, such as a method name, or
        /// a particular point in a complex operation.
        /// </summary>
        private readonly string context;

        /// <summary>
        /// A description of the event.
        /// </summary>
        private readonly string message;
        
        /// <summary>
        /// Data associated with the event, such as argument values.
        /// </summary>
        private readonly string data;
        
        /// <summary>
        /// When the event occurred.
        /// </summary>
        private DateTime when;

        /// <summary>
        /// Initializes a new instance of the TraceMessage class to construct a message.
        /// </summary>
        /// <param name="c">The context parameter.</param>
        /// <param name="m">The message parameter.</param>
        /// <param name="d">The data parameter.</param>
        /// <param name="w">When the event occurred.</param>
        public TraceMessage(string c, string m, string d, DateTime w)
        {
            this.context = c;
            this.message = m;
            this.data = d;
            this.when = w;
        }

        /// <summary>
        /// Initializes a new instance of the TraceMessage class to construct a message, using the current date/time.
        /// </summary>
        /// <param name="c">The context parameter.</param>
        /// <param name="m">The message parameter.</param>
        /// <param name="d">The data parameter.</param>
        public TraceMessage(string c, string m, string d)
            : this(c, m, d, DateTime.Now)
        {
        }

        /// <summary>
        /// Convert a Trace.Message into a user-friendly string.
        /// </summary>
        /// <returns>the string.</returns>
        public string Format()
        {
            return string.Format(CultureInfo.InvariantCulture, this.when.ToString("u", CultureInfo.InvariantCulture) + ": {0} ({1}, data {2})", this.message, this.context, this.data);
        }
    }
}
