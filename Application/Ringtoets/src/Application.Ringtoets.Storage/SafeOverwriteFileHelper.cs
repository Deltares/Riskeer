// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Utils;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// Class for providing a safe way of overwriting files through the use of a temporary backup file.
    /// </summary>
    public class SafeOverwriteFileHelper
    {
        private const string temporarySuffix = "~";

        private readonly string temporaryFilePath;
        private readonly string targetFilePath;
        private bool createTemporaryFile;

        /// <summary>
        /// Creates an instance of <see cref="SafeOverwriteFileHelper"/>.
        /// </summary>
        /// <param name="targetFilePath">The file which will be overwritten.</param>
        /// <exception cref="ArgumentException"><paramref name="targetFilePath"/> is not a valid path.</exception>
        /// <exception cref="IOException">Thrown when:
        /// <list type="bullet">
        /// <item>A temporary file already exists and cannot be deleted.</item>
        /// <item>A temporary file cannot be created from <paramref name="targetFilePath"/>.</item>
        /// <item>A new file cannot be created in the same directory as <paramref name="targetFilePath"/>.</item>
        /// </list>
        /// </exception>
        public SafeOverwriteFileHelper(string targetFilePath)
        {
            FileUtils.ValidateFilePath(targetFilePath);

            this.targetFilePath = targetFilePath;
            temporaryFilePath = targetFilePath + temporarySuffix;

            Initialize();
        }

        /// <summary>
        /// Removes the temporary file. If a revert action needs to be performed, the original file is restored
        /// from the temporary file first.
        /// </summary>
        /// <param name="revert"><c>true</c> if the original file needs to be restored, <c>false</c> otherwise.</param>
        /// <exception cref="IOException">Thrown when:
        /// <list type="bullet">
        /// <item>While reverting, the original file cannot be restored.</item>
        /// <item>The temporary file cannot be removed.</item>
        /// </list>
        /// </exception>
        public void Finish(bool revert)
        {
            if (createTemporaryFile)
            {
                CleanUp(revert);
            }
        }

        private void Initialize()
        {
            createTemporaryFile = File.Exists(targetFilePath);

            if (createTemporaryFile)
            {
                CreateTemporaryFile();
            }

            CreateNewTargetFile();
        }

        private void CreateNewTargetFile()
        {
            try
            {
                using (File.Create(targetFilePath)) {}
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is SystemException)
                {
                    var message = string.Format(Resources.SafeOverwriteFileHelper_CreateNewTargetFile_Cannot_create_target_file_0_Try_change_save_location, targetFilePath);
                    throw new IOException(message, e);
                }
                throw;
            }
        }

        private void CreateTemporaryFile()
        {
            try
            {
                if (File.Exists(temporaryFilePath))
                {
                    File.Delete(temporaryFilePath);
                }

                File.Move(targetFilePath, temporaryFilePath);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is SystemException)
                {
                    var message = string.Format(Resources.SafeOverwriteFileHelper_CreateTemporaryFile_Cannot_create_temporary_file_0_Try_change_save_location, targetFilePath);
                    throw new IOException(message, e);
                }
                throw;
            }
        }

        private void CleanUp(bool revert)
        {
            if (revert)
            {
                RestoreOriginalFile();
            }
            else
            {
                DeleteTemporaryFile();
            }
        }

        private void RestoreOriginalFile()
        {
            try
            {
                File.Delete(targetFilePath);
                File.Move(temporaryFilePath, targetFilePath);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is SystemException)
                {
                    var message = string.Format(Resources.SafeOverwriteFileHelper_CreateTemporaryFile_Cannot_rever_to_original_file_0_Try_removing_manually, targetFilePath);
                    throw new IOException(message, e);
                }
                throw;
            }
        }

        private void DeleteTemporaryFile()
        {
            try
            {
                File.Delete(temporaryFilePath);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is SystemException)
                {
                    var message = string.Format(Resources.SafeOverwriteFileHelper_CreateTemporaryFile_Cannot_remove_temporary_file_0_Try_removing_manually, temporaryFilePath);
                    throw new IOException(message, e);
                }
                throw;
            }
        }
    }
}