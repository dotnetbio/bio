namespace System.IO
{
    /// <summary>
    /// Dummy class to enable FileFormatException in Silverlight
    /// This is internally just a FormatException
    /// </summary>
    public sealed class FileFormatException : FormatException
    {
        // Summary:
        //     Creates a new instance of the System.IO.FileFormatException class.
        public FileFormatException() { }

        //
        // Summary:
        //     Creates a new instance of the System.IO.FileFormatException class with a
        //     specified error message.
        //
        // Parameters:
        //   message:
        //     A System.String value that represents the error message.
        public FileFormatException(string message) : base(message) { }

        //
        // Summary:
        //     Creates a new instance of the System.IO.FileFormatException class with a
        //     specified error message and exception type.
        //
        // Parameters:
        //   message:
        //     A System.String value that represents the error message.
        //
        //   innerException:
        //     The value of the System.Exception.InnerException property, which represents
        //     the cause of the current exception.
        public FileFormatException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Dummy class to enable InvalidDataException in Silverlight
    /// This is internally just a SystemException
    /// </summary>
    public sealed class InvalidDataException : SystemException
    {
        // Summary:
        //     Initializes a new instance of the System.IO.InvalidDataException class.
        public InvalidDataException() { }

        //
        // Summary:
        //     Initializes a new instance of the System.IO.InvalidDataException class with
        //     a specified error message.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        public InvalidDataException(string message) : base(message) { }

        //
        // Summary:
        //     Initializes a new instance of the System.IO.InvalidDataException class with
        //     a reference to the inner exception that is the cause of this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception. If the innerException
        //     parameter is not null, the current exception is raised in a catch block that
        //     handles the inner exception.
        public InvalidDataException(string message, Exception innerException) : base(message, innerException) { }
    }
}
