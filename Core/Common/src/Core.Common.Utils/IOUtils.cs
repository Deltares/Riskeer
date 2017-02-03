// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils.Builders;
using Core.Common.Utils.Properties;

namespace Core.Common.Utils
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
            if (string.IsNullOrWhiteSpace(path))
            {
                var message = new DirectoryWriterErrorMessageBuilder(path)
                    .Build(Resources.IOUtils_ValidateFolderPath_Path_cannot_be_empty);
                throw new ArgumentException(message);
            }
            try
            {
                string fullPath = Path.GetFullPath(path);
            }
            catch (SecurityException e)
            {
                var message = new DirectoryWriterErrorMessageBuilder(path)
                    .Build(Resources.IOUtils_ValidateFolderPath_No_access_rights_to_folder);
                throw new ArgumentException(message, e);
            }
            catch (PathTooLongException e)
            {
                var message = new DirectoryWriterErrorMessageBuilder(path)
                    .Build(Resources.IOUtils_ValidateFolderPath_Folder_path_too_long);
                throw new ArgumentException(message, e);
            }
            catch (NotSupportedException e)
            {
                var message = new DirectoryWriterErrorMessageBuilder(path)
                    .Build(Resources.IOUtils_ValidateFolderPath_Folder_path_contains_invalid_character);
                throw new ArgumentException(message, e);
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
        public static void ValidateFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
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
                    .Build(string.Format(CultureInfo.CurrentCulture,
                                         Resources.Error_Path_cannot_contain_Characters_0_,
                                         string.Join(", ", Path.GetInvalidFileNameChars())));
                throw new ArgumentException(message, e);
            }
            if (string.IsNullOrEmpty(name))
            {
                var message = new FileReaderErrorMessageBuilder(path).Build(Resources.Error_Path_must_not_point_to_empty_file_name);
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
                    var message = string.Format(CultureInfo.CurrentCulture,
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
    }
}