using System;
using System.IO;
using Core.Common.Utils.Properties;
using Core.Common.Utils.Builders;

namespace Core.Common.Utils
{
    /// <summary>
    /// Class with reusable File related utility methods.
    /// </summary>
    public static class FileUtils 
    {
        /// <summary>
        /// Validates the file path.
        /// </summary>
        /// <param name="path">The file path to be validated.</param>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is invalid.</exception>
        public  static void ValidateFilePath(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                var message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_be_specified);
                throw new ArgumentException(message);
            }

            string name;
            try
            {
                name = Path.GetFileName(path);
            }
            catch (ArgumentException e)
            {
                var message = new FileReaderErrorMessageBuilder(path)
                    .Build(String.Format(Resources.Error_Path_cannot_contain_Characters_0_,
                                         String.Join(", ", Path.GetInvalidFileNameChars())));
                throw new ArgumentException(message, e);
            }
            if (String.Empty == name)
            {
                var message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_not_point_to_folder);
                throw new ArgumentException(message);
            }
        }
    }
}