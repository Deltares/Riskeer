using System;

namespace Core.Plugins.ProjectExplorer.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a file reader class encounters a critical error
    /// during the read.
    /// </summary>
    public class ProjectExplorerGuiPluginException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectExplorerGuiPluginException"/> class.
        /// </summary>
        public ProjectExplorerGuiPluginException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectExplorerGuiPluginException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ProjectExplorerGuiPluginException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectExplorerGuiPluginException"/> class
        /// with a specified error message and a reference to the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.</param>
        public ProjectExplorerGuiPluginException(string message, Exception inner) : base(message, inner) {}
    }
}