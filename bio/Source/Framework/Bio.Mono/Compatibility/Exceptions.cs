namespace System.IO
{
    /// <summary>
    /// Dummy class to enable FileFormatException in Silverlight
    /// This is internally just a FormatException
    /// </summary>
    public sealed class FileFormatException : FormatException
    {
        /// <summary>
        /// Creates a new instance of the System.IO.FileFormatException class.
        /// </summary>
        public FileFormatException() { }

        /// <summary>
        /// Creates a new instance of the System.IO.FileFormatException class with a
        /// specified error message.
        /// </summary>
        /// <param name="message">A System.String value that represents the error message.</param>
        public FileFormatException(string message) : base(message) { }

        /// <summary>
        /// Creates a new instance of the System.IO.FileFormatException class with a
        /// specified error message and exception type.
        /// </summary>
        /// <param name="message">A System.String value that represents the error message.</param>
        /// <param name="innerException">The value of the System.Exception.InnerException property, which represents the cause of the current exception. </param>
        public FileFormatException(string message, Exception innerException) : base(message, innerException) { }
    }

#if FALSE
    /// <summary>
    /// Dummy class to enable InvalidDataException in Silverlight
    /// This is internally just a SystemException
    /// </summary>
    public sealed class InvalidDataException : SystemException
    {
        /// <summary>
        /// Initializes a new instance of the System.IO.InvalidDataException class.
        /// </summary>
        public InvalidDataException() { }

        /// <summary>
        /// Initializes a new instance of the System.IO.InvalidDataException class with
        /// a specified error message
        /// </summary>
        /// <param name="message">Error message</param>
        public InvalidDataException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the System.IO.InvalidDataException class.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Existing exception</param>
        public InvalidDataException(string message, Exception innerException) : base(message, innerException) { }
    }
#endif
}
