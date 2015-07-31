using System;
using System.Globalization;
using System.Runtime.Serialization;
using Bio.Properties;

namespace Bio.IO.AppliedBiosystems.Exceptions
{
    /// <summary>
    /// File signature is invalid.
    /// </summary>
    public class InvalidFileSignatureException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="expected"></param>
        public InvalidFileSignatureException(string signature, string expected) :
            base(string.Format(CultureInfo.InvariantCulture, Resource.Ab1InvalidFileSignatureExceptionFormat, signature, expected))
        {
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public InvalidFileSignatureException()
        {
        }

        /// <summary>
        /// Constructor with message.
        /// </summary>
        /// <param name="message"></param>
        public InvalidFileSignatureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor with message and inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public InvalidFileSignatureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
