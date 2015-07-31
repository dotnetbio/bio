using System;

namespace Bio.Util.ArgumentParser
{
    /// <summary>
    /// Parse Exception class.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ParseException() : base() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The Message.</param>
        public ParseException(string message) : base(message) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ParseException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="args">The Arguments.</param>
        public ParseException(string message, params object[] args) : base(string.Format(message, args)) { }
    }
}
