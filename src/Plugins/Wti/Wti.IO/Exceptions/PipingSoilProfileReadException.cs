﻿using System;
using Wti.Data;

namespace Wti.IO.Exceptions
{
    /// <summary>
    /// Exception thrown when something went wrong while reading <see cref="PipingSoilProfile"/> in <see cref="PipingSoilProfileReader"/>.
    /// </summary>
    public class PipingSoilProfileReadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingSoilProfileReadException"/> class.
        /// </summary>
        public PipingSoilProfileReadException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingSoilProfileReadException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PipingSoilProfileReadException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Exception class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a 
        /// null reference if no inner exception is specified.</param>
        public PipingSoilProfileReadException(string message, Exception innerException) : base(message, innerException) { }
    }
}