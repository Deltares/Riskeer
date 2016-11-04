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
using Core.Common.Utils.Builders;
using Core.Common.Utils.Properties;

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
        /// <exception cref="IOException">Thrown when an error occured while trying to search and delete files in <paramref name="path"/>.</exception>
        public static void DeleteOldFiles(string path, string searchPattern, int numberOfDaysToKeepFiles)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(@"No valid value for 'path'.", "path");
            }
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException(@"No valid value for 'searchPattern'.", "searchPattern");
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
                                                Resources.FileUtils_DeleteOldFiles_Error_occured_deleting_files_in_folder_0,
                                                path);
                    throw new IOException(message, e);
                }
                throw;
            }
        }
    }
}