using System;

namespace Bio.Hpc
{
    /// <summary>
    /// Custom unknown cluster exception
    /// </summary>
    public class UnknownClusterException : Exception
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        public UnknownClusterException() : base() { }

        /// <summary>
        /// Constructor with exception message
        /// </summary>
        /// <param name="message">message</param>
        public UnknownClusterException(string message) : base(message) { }
    }
}
