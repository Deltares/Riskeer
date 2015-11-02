using System;
using System.IO;

using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO
{
    /// <summary>
    /// Class with reusable File related utility methods.
    /// </summary>
    static internal class FileUtils 
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
                throw new ArgumentException(Resources.Error_Path_must_be_specified);
            }

            string name;
            try
            {
                name = Path.GetFileName(path);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(String.Format(Resources.Error_Path_cannot_contain_characters_0_,
                                                          String.Join(", ", Path.GetInvalidFileNameChars())), e);
            }
            if (String.Empty == name)
            {
                throw new ArgumentException(Resources.Error_Path_must_not_point_to_folder);
            }
        }
    }
}