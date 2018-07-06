using System;
using System.Runtime.Serialization;

namespace Ringtoets.Integration.Service.Exceptions
{
    /// <summary>
    /// Exception thrown when something went wrong when providing assessment sections.
    /// </summary>
    [Serializable]
    public class AssessmentSectionProviderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionProviderException"/> class.
        /// </summary>
        public AssessmentSectionProviderException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionProviderException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AssessmentSectionProviderException(string message)
            : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionProviderException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or <c>null</c> if no inner exception is specified.</param>
        public AssessmentSectionProviderException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Initializes a new instance of <see cref="AssessmentSectionProviderException"/> with
        /// serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info"/> parameter is
        /// <c>null</c>.</exception>
        /// <exception cref="SerializationException">The class name is <c>null</c> or
        /// <see cref="Exception.HResult" /> is zero (0).</exception>
        protected AssessmentSectionProviderException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}