using System;

namespace Core.Plugins.ProjectExplorer.Exceptions
{
    /// <summary>
    /// This class describes exceptions thrown when activating the plugin failed.
    /// </summary>
    public class PluginActivationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginActivationException"/> class.
        /// </summary>
        public PluginActivationException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginActivationException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public PluginActivationException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginActivationException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public PluginActivationException(string message, Exception inner) : base(message, inner) {}
    }
}