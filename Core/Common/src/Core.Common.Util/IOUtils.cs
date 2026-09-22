// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
        private const int maxPath = 32_767;

        /// <summary>
        /// Validates the folder path.
        /// </summary>
        /// <param name="path">The folder path to be validated.</param>
        /// <returns><c>true</c> if the folder path is valid; <c>false</c> otherwise.</returns>
        /// <remarks>See <see cref="GetFullPath"/> for the conditions that make a folder path valid.</remarks>
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
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        /// <remarks>See <see cref="GetFullPath"/> for the conditions that make a folder path valid.</remarks>
        public static void ValidateFolderPath(string path)
        {
            try
            {
                GetFullPath(path);
            }
            catch (ArgumentException exception)
            {
                string message = new DirectoryWriterErrorMessageBuilder(path).Build(exception.Message);
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
        /// <item>is not too long,</item>
        /// <item>does not contain a colon outside the volume identifier,</item>
        /// <item>does not contain an invalid path character (<seealso cref="Path.GetInvalidPathChars()"/>),</item>
        /// <item>does not end with a directory or path separator (empty file name),</item>
        /// <item>does not have a file name that contains an invalid file name character (<seealso cref="Path.GetInvalidFileNameChars()"/>).</item>
        /// </list>
        /// </remarks>
        public static void ValidateFilePath(string path)
        {
            ValidatePath(path, message => new FileReaderErrorMessageBuilder(path).Build(message));

            string name = Path.GetFileName(path);

            if (string.IsNullOrEmpty(name))
            {
                string message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_not_point_to_empty_file_name);
                throw new ArgumentException(message);
            }

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                string message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_cannot_contain_invalid_characters);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Validates the file path.
        /// </summary>
        /// <param name="path">The file path to be validated.</param>
        /// <returns><c>true</c> if the file path is valid, <c>false</c> otherwise.</returns>
        /// <remarks>See <see cref="ValidateFilePath"/> for the conditions that make a file path valid.</remarks>
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
                foreach (string logFile in Directory.GetFiles(path, searchPattern).Where(l => (DateTime.Now - File.GetCreationTime(l)).TotalDays > numberOfDaysToKeepFiles))
                {
                    File.Delete(logFile);
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
                {
                    string message = string.Format(CultureInfo.CurrentCulture, Resources.IOUtils_DeleteOldFiles_Error_occurred_deleting_files_in_folder_0, path);
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
        /// <remarks>See <see cref="ValidateFilePath"/> for the conditions that make a file path valid.</remarks>
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
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>is not too long,</item>
        /// <item>does not contain a colon outside the volume identifier,</item>
        /// <item>does not contain an invalid path character (<seealso cref="Path.GetInvalidPathChars()"/>),</item>
        /// <item>provides the caller with sufficient access rights.</item>
        /// </list>
        /// </remarks>
        public static string GetFullPath(string path)
        {
            ValidatePath(path);

            try
            {
                return Path.GetFullPath(path);
            }
            catch (SecurityException exception)
            {
                throw new ArgumentException(Resources.IOUtils_No_access_rights_to_path, exception);
            }
        }

        private static void ValidatePath(string path, Func<string, string> decorateMessageFunc = null)
        {
            string message = path switch
            {
                _ when string.IsNullOrWhiteSpace(path) => Resources.Error_Path_must_be_specified,
                _ when path.Length > maxPath => Resources.IOUtils_Path_too_long,
                _ when ContainsInvalidColonOutsideVolumeIdentifier(path) => Resources.IOUtils_Path_contains_invalid_colon,
                _ when path.IndexOfAny(Path.GetInvalidPathChars()) >= 0 => Resources.Error_Path_cannot_contain_invalid_characters,
                _ => null
            };

            if (message == null)
            {
                return;
            }

            if (decorateMessageFunc != null)
            {
                message = decorateMessageFunc(message);
            }

            throw new ArgumentException(message);
        }

        private static bool ContainsInvalidColonOutsideVolumeIdentifier(string path)
        {
            int colonIndex = path.IndexOf(':');
            if (colonIndex < 0)
            {
                return false;
            }

            bool hasSingleDriveSeparator = colonIndex == 1 && char.IsLetter(path[0]) && path.IndexOf(':', colonIndex + 1) < 0;
            return !hasSingleDriveSeparator;
        }
    }
}