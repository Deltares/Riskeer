using System;
using System.Runtime.Serialization;

namespace Core.Components.DotSpatial.Layer.BruTile.Configurations
{
    /// <summary>
    /// The exception that is thrown when the creation of a tile cache failed.
    /// </summary>
    public class CannotCreateTileCacheException : SystemException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotCreateTileCacheException"/> class.
        /// </summary>
        public CannotCreateTileCacheException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotCreateTileCacheException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public CannotCreateTileCacheException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotCreateTileCacheException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public CannotCreateTileCacheException(string message, Exception inner) : base(message, inner) { }

        protected CannotCreateTileCacheException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}