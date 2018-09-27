// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using Core.Common.Util.Builders;
using Core.Common.Util.Properties;

namespace Core.Common.Util
{
    /// <summary>
    /// Class with reusable file and folder related utility methods.
    /// </summary>
    public static class IOUtils
    {
        /// <summary>
        /// Validates the folder path.
        /// </summary>
        /// <param name="path">The folder path to be validated.</param>
        /// <returns><c>true</c> if the folder path is valid; <c>false</c> otherwise.</returns>
        /// <remarks>A valid folder path:
        /// <list type="bullet">
        /// <item>is not empty nor contains only whitespaces.</item>
        /// <item>has no access rights to that location.</item>
        /// <item>isn't too long.</item>
        /// <item>does not contain an invalid ':' character.</item>
        /// </list></remarks>
        public static bool IsValidFolderPath(string path)
        {
            try
            {
                ValidateFolderPath(path);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the folder path.
        /// </summary>
        /// <param name="path">The folder path to be validated.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>The folder path is empty or contains only whitespaces.</item>
        /// <item>Caller has no access rights to the folder path.</item>
        /// <item>The folder path is too long.</item>
        /// <item>The folder path contains an invalid ':' character.</item>
        /// </list></exception>
        public static void ValidateFolderPath(string path)
        {
            try
            {
                GetFullPath(path);
            }
            catch (ArgumentException exception)
            {
                string message = new DirectoryWriterErrorMessageBuilder(path)
                    .Build(exception.Message);
                throw new ArgumentException(message, exception.InnerException);
            }
        }

        /// <summary>
        /// Validates the file path.
        /// </summary>
        /// <param name="path">The file path to be validated.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        /// <seealso cref="Path.GetInvalidPathChars()"/>
        public static void ValidateFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                string message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_be_specified);
                throw new ArgumentException(message);
            }

            string name;
            try
            {
                name = Path.GetFileName(path);
            }
            catch (ArgumentException exception)
            {
                string message = new FileReaderErrorMessageBuilder(path)
                    .Build(Resources.Error_Path_cannot_contain_invalid_characters);
                throw new ArgumentException(message, exception);
            }

            if (string.IsNullOrEmpty(name))
            {
                string message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_not_point_to_empty_file_name);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Validates the file path.
        /// </summary>
        /// <param name="path">The file path to be validated.</param>
        /// <returns><c>true</c> if the file path is valid, <c>false</c> otherwise.</returns>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>contains not only whitespace,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public static bool IsValidFilePath(string path)
        {
            try
            {
                ValidateFilePath(path);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Searches the files in <paramref name="path"/> that match <paramref name="searchPattern"/> and 
        /// deletes the files older than <paramref name="numberOfDaysToKeepFiles"/> days.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path.</param>
        /// <param name="numberOfDaysToKeepFiles">The maximum number days since the file was created.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> or <paramref name="searchPattern"/> is <c>null</c>, is a zero-length string, 
        /// contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="IOException">Thrown when an error occurred while trying to search and delete files in <paramref name="path"/>.</exception>
        public static void DeleteOldFiles(string path, string searchPattern, int numberOfDaysToKeepFiles)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(@"No valid value for 'path'.", nameof(path));
            }

            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException(@"No valid value for 'searchPattern'.", nameof(searchPattern));
            }

            try
            {
                foreach (string logFile in Directory.GetFiles(path, searchPattern).Where(
                    l => (DateTime.Now - File.GetCreationTime(l)).TotalDays > numberOfDaysToKeepFiles))
                {
                    File.Delete(logFile);
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
                {
                    string message = string.Format(CultureInfo.CurrentCulture,
                                                   Resources.IOUtils_DeleteOldFiles_Error_occurred_deleting_files_in_folder_0,
                                                   path);
                    throw new IOException(message, e);
                }

                throw;
            }
        }

        /// <summary>
        /// Creates a file at <paramref name="path"/> if it does not exist already.
        /// </summary>
        /// <param name="path">The file path to be created.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public static void CreateFileIfNotExists(string path)
        {
            ValidateFilePath(path);

            var canWrite = false;
            try
            {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    canWrite = fs.CanWrite;
                }
            }
            finally
            {
                if (!canWrite)
                {
                    throw new ArgumentException(string.Format(Resources.Error_General_output_error_0, path), nameof(path));
                }
            }
        }

        /// <summary>
        /// Returns the absolute path for the specified path string.
        /// </summary>
        /// <param name="path">The file or directory for which to obtain absolute path information.</param>
        /// <returns>The fully qualified location of path, such as "C:\MyFile.txt".</returns>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>The path is <c>null</c>, empty or contains only whitespaces.</item>
        /// <item>The caller has no access rights to the path.</item>
        /// <item>The path is too long.</item>
        /// <item>The path contains a ':' that is not part of a volume identifier.</item>
        /// </list></exception>
        public static string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(Resources.IOUtils_Path_cannot_be_empty);
            }

            try
            {
                return Path.GetFullPath(path);
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(Resources.Error_Path_cannot_contain_invalid_characters,
                                            exception);
            }
            catch (SecurityException exception)
            {
                throw new ArgumentException(Resources.IOUtils_No_access_rights_to_path,
                                            exception);
            }
            catch (PathTooLongException exception)
            {
                throw new ArgumentException(Resources.IOUtils_Path_too_long,
                                            exception);
            }
            catch (NotSupportedException exception)
            {
                throw new ArgumentException(Resources.IOUtils_Path_contains_invalid_character,
                                            exception);
            }
        }
    }
}