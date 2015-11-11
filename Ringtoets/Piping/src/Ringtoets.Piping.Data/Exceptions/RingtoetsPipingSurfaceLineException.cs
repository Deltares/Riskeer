using System;

namespace Ringtoets.Piping.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when operations on <see cref="RingtoetsPipingSurfaceLine"/> encounter 
    /// an error.
    /// </summary>
    public class RingtoetsPipingSurfaceLineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RingtoetsPipingSurfaceLineException"/> class.
        /// </summary>
        public RingtoetsPipingSurfaceLineException(){}

        /// <summary>
        /// Initializes a new instance of the <see cref="RingtoetsPipingSurfaceLineException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public RingtoetsPipingSurfaceLineException(string message) : base(message){}

        /// <summary>
        /// Initializes a new instance of the <see cref="RingtoetsPipingSurfaceLineException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public RingtoetsPipingSurfaceLineException(string message, Exception inner) : base(message, inner) { }
    }
}