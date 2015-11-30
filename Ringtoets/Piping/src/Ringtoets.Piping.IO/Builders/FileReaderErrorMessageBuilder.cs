namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// Class to help create consistent file reader error messages.
    /// </summary>
    public class FileReaderErrorMessageBuilder
    {
        private readonly string filePath;
        private string location;
        private string subject;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileReaderErrorMessageBuilder"/> class.
        /// </summary>
        /// <param name="filePath">The file path to the file where the error occurred.</param>
        public FileReaderErrorMessageBuilder(string filePath)
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
            return string.Format("Fout bij het lezen van bestand '{0}'{1}{2}: {3}",
                                 filePath, 
                                 location ?? string.Empty, 
                                 subject ?? string.Empty,
                                 errorMessage);
        }

        /// <summary>
        /// Adds file location information to the error message.
        /// </summary>
        /// <param name="locationDescription">The location description.</param>
        /// <returns>The builder being configured.</returns>
        /// <example>line 7</example>
        public FileReaderErrorMessageBuilder WithLocation(string locationDescription)
        {
            location = " " + locationDescription;
            return this;
        }

        /// <summary>
        /// Adds the subject where the error occurred to the error message.
        /// </summary>
        /// <param name="subjectDescription">The subject description.</param>
        /// <returns>The builder being configured.</returns>
        /// <example>soil profile 'blabla'</example>
        public FileReaderErrorMessageBuilder WithSubject(string subjectDescription)
        {
            subject = string.Format(" ({0})", subjectDescription);
            return this;
        }
    }
}