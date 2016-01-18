namespace Core.Common.Utils.Validation
{
    /// <summary>
    /// Denotes the severity of a validation message.
    /// </summary>
    public enum ValidationSeverity
    {
        /// <summary>
        /// The message has no particular severity.
        /// </summary>
        None,
        /// <summary>
        /// The message is intended to provide information.
        /// </summary>
        Info,
        /// <summary>
        /// The message is intended to provide a warning.
        /// </summary>
        Warning,
        /// <summary>
        /// The message is intended to notify about an error-condition.
        /// </summary>
        Error
    }
}