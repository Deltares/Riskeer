﻿using System;

namespace Ringtoets.Piping.Calculation.Piping
{
    /// <summary>
    /// Exception thrown when something went wrong in the <see cref="PipingCalculation"/>
    /// </summary>
    public class PipingProfileCreatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingProfileCreatorException"/> class.
        /// </summary>
        public PipingProfileCreatorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingProfileCreatorException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PipingProfileCreatorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingProfileCreatorException"/> class  with 
        /// a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a 
        /// null reference if no inner exception is specified.</param>
        public PipingProfileCreatorException(string message, Exception innerException) : base(message, innerException) { }
    }
}