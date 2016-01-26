using Core.Common.Utils.Properties;

namespace Core.Common.Utils.Builders
{
    /// <summary>
    /// Class to help create consistent file writer error messages.
    /// </summary>
    public class FileWriterErrorMessageBuilder
    {
        private readonly string filePath;

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
            return string.Format(Resources.Error_Writing_To_File_0_1,
                                 filePath,
                                 errorMessage);
        }
    }
}