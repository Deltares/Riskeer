using System;

namespace Core.Common.Base.Storage
{
    /// <summary>
    /// The exception that is thrown when a storage execution failed.
    /// </summary>
    public class StorageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageException"/> class.
        /// </summary>
        public StorageException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public StorageException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public StorageException(string message, Exception inner) : base(message, inner) {}
    }
}