using System;

namespace Application.Ringtoets.Storage.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a file is not valid.
    /// </summary>
    public class InvalidFileException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFileException"/> class.
        /// </summary>
        public InvalidFileException(){}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFileException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidFileException(string message) : base(message){}

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFileException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public InvalidFileException(string message, Exception inner) : base(message, inner) { }
    }
}