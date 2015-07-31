using System;
using System.Globalization;
using System.Runtime.Serialization;
using Bio.Properties;

namespace Bio.IO.AppliedBiosystems.Exceptions
{
    /// <summary>
    /// Item size is invalid.
    /// </summary>
    public class InvalidItemSizeException : Exception
    {
        /// <summary>
        /// Create a new exception.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="name"></param>
        public InvalidItemSizeException(int expected, int actual, string name) :
            base(string.Format(CultureInfo.InvariantCulture, Resource.InvalidItemSizeExceptionFormat, expected, actual, name))
        {
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public InvalidItemSizeException()
        {
        }

        /// <summary>
        /// Constructor with message.
        /// </summary>
        /// <param name="message"></param>
        public InvalidItemSizeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor with message and inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidItemSizeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
