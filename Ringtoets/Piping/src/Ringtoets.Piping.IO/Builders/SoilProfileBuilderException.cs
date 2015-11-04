using System;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// Exception thrown when something went wrong while building a <see cref="PipingSoilProfile"/>.
    /// </summary>
    public class SoilProfileBuilderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoilProfileBuilderException"/> class.
        /// </summary>
        public SoilProfileBuilderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilProfileBuilderException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SoilProfileBuilderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilProfileBuilderException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a 
        /// null reference if no inner exception is specified.</param>
        public SoilProfileBuilderException(string message, Exception innerException) : base(message, innerException) { }
    }
}