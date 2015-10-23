using System;

namespace Wti.IO.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a file reader class encounters a critical error
    /// during the read.
    /// </summary>
    public class CriticalFileReadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalFileReadException"/> class.
        /// </summary>
        public CriticalFileReadException(){}

        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalFileReadException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public CriticalFileReadException(string message) : base(message){}

        /// <summary>
        /// Initializes a new instance of the <see cref="CriticalFileReadException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CriticalFileReadException(string message, Exception inner) : base(message, inner) { }
    }
}