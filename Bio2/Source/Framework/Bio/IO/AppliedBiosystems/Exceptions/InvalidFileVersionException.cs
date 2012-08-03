using System;
using System.Globalization;
using System.Runtime.Serialization;
using Bio.Properties;

namespace Bio.IO.AppliedBiosystems.Exceptions
{
    /// <summary>
    /// File version is invalid.
    /// </summary>
    [Serializable]
    public class InvalidFileVersionException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public InvalidFileVersionException(int expected, int actual) :
            base(string.Format(CultureInfo.InvariantCulture, Resource.Ab1InvalidFileVersionExceptionFormat, expected, actual))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidFileVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public InvalidFileVersionException()
        {
        }

        /// <summary>
        /// Constructor with message.
        /// </summary>
        /// <param name="message"></param>
        public InvalidFileVersionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor with message and inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidFileVersionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
