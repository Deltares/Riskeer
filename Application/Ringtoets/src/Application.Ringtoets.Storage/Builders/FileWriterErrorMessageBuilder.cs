namespace Application.Ringtoets.Storage.Builders
{
    /// <summary>
    /// Class to help create consistent file writer error messages.
    /// </summary>
    public class FileWriterErrorMessageBuilder
    {
        private readonly string filePath;
        private string subject;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWriterErrorMessageBuilder"/> class.
        /// </summary>
        /// <param name="filePath">The file path to the file where the error occurred.</param>
        public FileWriterErrorMessageBuilder(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Builds the specified error message.
        /// </summary>
        /// <param name="errorMessage">The message about the error that has occurred.</param>
        /// <returns>The full error message.</returns>
        public string Build(string errorMessage)
        {
            return string.Format("Fout bij het schrijven van bestand '{0}'{1}: {2}",
                                 filePath,
                                 subject ?? string.Empty,
                                 errorMessage);
        }

        /// <summary>
        /// Adds the subject where the error occurred to the error message.
        /// </summary>
        /// <param name="subjectDescription">The subject description.</param>
        /// <returns>The builder being configured.</returns>
        /// <example>soil profile 'Lorem Ipsum'</example>
        public FileWriterErrorMessageBuilder WithSubject(string subjectDescription)
        {
            subject = string.Format(" ({0})", subjectDescription);
            return this;
        }
    }
}