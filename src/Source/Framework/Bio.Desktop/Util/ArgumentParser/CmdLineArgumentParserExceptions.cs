namespace Bio.Util.ArgumentParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Base class for Command line Argument Exceptions.
    /// </summary>
    public class ArgumentParserException : Exception
    {
        /// <summary>
        /// Argument Parser Exception.
        /// </summary>
        public ArgumentParserException() { }

        /// <summary>
        /// Argument Parser Exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public ArgumentParserException(string message) : base(message) { }

        /// <summary>
        /// Argument Parser Exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public ArgumentParserException(string message, Exception innerException) : base(message, innerException) { }

    }

    /// <summary>
    /// The exception that is thrown when there is syntax error.
    /// </summary>
    public class ArgumentSyntaxException : ArgumentParserException
    {

        /// <summary>
        /// Argument Syntax Exception.
        /// </summary>
        public ArgumentSyntaxException() { }

        /// <summary>
        /// Argument Syntax Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        public ArgumentSyntaxException(string message) : base(message) { }

        /// <summary>
        /// Argument Syntax Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public ArgumentSyntaxException(string message, Exception innerException) : base(message, innerException) { }

    }

    /// <summary>
    /// The exception that is thrown when Required parameter is not passed in the command line
    /// </summary>
    public class RequiredArgumentMissingException : ArgumentParserException
    {

        /// <summary>
        /// Required Argument Missing Exception.
        /// </summary>
        public RequiredArgumentMissingException() { }

        /// <summary>
        /// Required Argument Missing Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        public RequiredArgumentMissingException(string message) : base(message) { }

        /// <summary>
        /// Required Argument Missing Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public RequiredArgumentMissingException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// This exception is thrown when an invalid value is passed to the parameter.
    /// </summary>
    public class InvalidArgumentValueException : ArgumentParserException
    {

        /// <summary>
        /// Invalid Argument Value Exception.
        /// </summary>
        public InvalidArgumentValueException() { }

        /// <summary>
        /// Invalid Argument Value Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        public InvalidArgumentValueException(string message) : base(message) { }

        /// <summary>
        /// Invalid Argument Value Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="innerException"></param>
        public InvalidArgumentValueException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// This exception is thrown when an duplicate value is passed to the array type parameter.
    /// </summary>
    public class DuplicateArgumentValueException : ArgumentParserException
    {

        /// <summary>
        /// Duplicate Argument Value Exception.
        /// </summary>
        public DuplicateArgumentValueException() { }

        /// <summary>
        /// Duplicate Argument Value Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        public DuplicateArgumentValueException(string message) : base(message) { }

        /// <summary>
        /// Duplicate Argument Value Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public DuplicateArgumentValueException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// This exception is thrown when the same parameter is passed more than once.
    /// </summary>
    public class ArgumentRepeatedException : ArgumentParserException
    {

        /// <summary>
        /// Argument Repeated Exception.
        /// </summary>
        public ArgumentRepeatedException() { }

        /// <summary>
        /// Argument Repeated Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        public ArgumentRepeatedException(string message) : base(message) { }

        /// <summary>
        /// Argument Repeated Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public ArgumentRepeatedException(string message, Exception innerException) : base(message, innerException) { }
    }
    /// <summary>
    /// This exception is thrown when passed arguments (other than boolean type arguments) does not hold values.
    /// </summary>
    public class ArgumentNullValueException : ArgumentParserException
    {

        /// <summary>
        /// Argument Null Value Exception.
        /// </summary>
        public ArgumentNullValueException() { }

        /// <summary>
        /// Argument Null Value Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        public ArgumentNullValueException(string message) : base(message) { }

        /// <summary>
        /// Argument Null Value Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public ArgumentNullValueException(string message, Exception innerException) : base(message, innerException) { }

    }

    /// <summary>
    /// This exception is raised when a parameter is not defined but passed in cmdline.
    /// </summary>
    public class ArgumentNotFoundException : ArgumentParserException
    {

        /// <summary>
        /// Argument Not Found Exception.
        /// </summary>
        public ArgumentNotFoundException() { }

        /// <summary>
        /// Argument Not Found Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        public ArgumentNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Argument Not Found Exception.
        /// </summary>
        /// <param name="message">The Message.</param>
        /// <param name="innerException">Inner Exception.</param>
        public ArgumentNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
