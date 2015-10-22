using System;
using Wti.Data;

namespace Wti.IO.Builders
{
    /// <summary>
    /// Exception thrown when something went wrong while converting <see cref="SoilLayer2D"/> to <see cref="PipingSoilProfile"/>.
    /// </summary>
    public class SoilLayer2DConversionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoilLayer2DConversionException"/> class.
        /// </summary>
        public SoilLayer2DConversionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilLayer2DConversionException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SoilLayer2DConversionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilLayer2DConversionException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a 
        /// null reference if no inner exception is specified.</param>
        public SoilLayer2DConversionException(string message, Exception innerException) : base(message, innerException) { }
    }
}