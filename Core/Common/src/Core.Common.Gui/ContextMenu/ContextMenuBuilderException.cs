using System;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// Exception thrown when something went wrong while initializing a <see cref="ContextMenuBuilder"/>.
    /// </summary>
    public class ContextMenuBuilderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuBuilderException"/> class.
        /// </summary>
        public ContextMenuBuilderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuBuilderException"/> class 
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ContextMenuBuilderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenuBuilderException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a 
        /// null reference if no inner exception is specified.</param>
        public ContextMenuBuilderException(string message, Exception innerException) : base(message, innerException) { }
    }
}