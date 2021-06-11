using System;
using System.Runtime.Serialization;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// Exception thrown when something went wrong while creating an <see cref="IProject"/>.
    /// </summary>
    [Serializable]
    public class ProjectFactoryException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProjectFactoryException"/>.
        /// </summary>
        public ProjectFactoryException() {}

        /// <summary>
        /// Creates a new instance of <see cref="ProjectFactoryException"/> 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ProjectFactoryException(string message)
            : base(message) {}

        /// <summary>
        /// Creates a new instance of <see cref="ProjectFactoryException"/> with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified.</param>
        public ProjectFactoryException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Creates a new instance of <see cref="ProjectFactoryException"/> with
        /// serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is
        /// <c>null</c>.</exception>
        /// <exception cref="SerializationException">The class name is <c>null</c> or
        /// <see cref="Exception.HResult" /> is zero (0).</exception>
        protected ProjectFactoryException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}