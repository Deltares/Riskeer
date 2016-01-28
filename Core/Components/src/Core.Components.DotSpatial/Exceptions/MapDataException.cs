﻿using System;
using Core.Components.DotSpatial.Data;

namespace Core.Components.DotSpatial.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the <see cref="MapData"/> is not valid.
    /// </summary>
    public class MapDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapDataException"/> class.
        /// </summary>
        public MapDataException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MapDataException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MapDataException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="MapDataException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public MapDataException(string message, Exception inner) : base(message, inner) {}
    }
}