// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util;
using Riskeer.Storage.Core.Exceptions;
using Riskeer.Storage.Core.Properties;

namespace Riskeer.Storage.Core
{
    /// <summary>
    /// Class for providing a safe way of writing files by creating a temporary backup file of targeted files.
    /// </summary>
    public class BackedUpFileWriter
    {
        private const string temporarySuffix = "~";

        private readonly string temporaryFilePath;
        private readonly string targetFilePath;
        private bool isTemporaryFileCreated;

        /// <summary>
        /// Creates an instance of <see cref="BackedUpFileWriter"/>.
        /// </summary>
        /// <param name="targetFilePath">The path of the file which will be overwritten.</param>
        /// <exception cref="ArgumentException"><paramref name="targetFilePath"/> is not a valid path.</exception>
        public BackedUpFileWriter(string targetFilePath)
        {
            IOUtils.ValidateFilePath(targetFilePath);

            this.targetFilePath = targetFilePath;
            temporaryFilePath = targetFilePath + temporarySuffix;
        }

        /// <summary>
        /// Performs the <paramref name="writeAction"/> in a safe way by backing up the targeted file provided when
        /// constructing the <see cref="BackedUpFileWriter"/>. It is expected that the <paramref name="writeAction"/>
        /// will throw an exception when the operation fails, so that the backed up target file can be restored.
        /// </summary>
        /// <param name="writeAction">The action to perform after backing up the targeted file. </param>
        /// <exception cref="IOException">Thrown when:
        /// <list type="bullet">
        /// <item>The temporary file already exists and cannot be deleted.</item>
        /// <item>The temporary file cannot be created from the target file.</item>
        /// <item>When reverting, the original file cannot be restored.</item>
        /// </list>
        /// </exception>
        /// <exception cref="CannotDeleteBackupFileException">Thrown when cleaning up, the temporary file cannot be removed.</exception>
        /// <remarks>Any <see cref="Exception"/> thrown by <paramref name="writeAction"/> will be rethrown.</remarks>
        public void Perform(Action writeAction)
        {
            CreateTemporaryFile();

            try
            {
                writeAction();
            }
            catch
            {
                Revert();
                throw;
            }

            Finish();
        }

        /// <summary>
        /// Removes the temporary file if it was created.
        /// </summary>
        /// <exception cref="CannotDeleteBackupFileException">The temporary file cannot be removed.</exception>
        private void Finish()
        {
            if (isTemporaryFileCreated)
            {
                DeleteTemporaryFile();
            }
        }

        /// <summary>
        /// Reverts the target file to the temporary file if it was created. If the operation fails, 
        /// the temporary file will remain in the directory of the target file.
        /// </summary>
        /// <exception cref="IOException">The original file cannot be restored.</exception>
        private void Revert()
        {
            if (isTemporaryFileCreated)
            {
                RestoreOriginalFile();
            }
        }

        /// <summary>
        /// Creates a temporary file from the target file, if there is any. Creates a new file at the target
        /// file path.
        /// </summary>
        /// <exception cref="IOException">Thrown when either:
        /// <list type="bullet">
        /// <item>The temporary file already exists and cannot be deleted.</item>
        /// <item>The temporary file cannot be created from the target file.</item>
        /// </list>
        /// </exception>
        private void CreateTemporaryFile()
        {
            isTemporaryFileCreated = false;

            if (File.Exists(targetFilePath))
            {
                RemoveAlreadyExistingTemporaryFile();
                CreateNewTemporaryFile();
                isTemporaryFileCreated = true;
            }
        }

        /// <summary>
        /// Removes the temporary file for the target file if it already exists.
        /// </summary>
        /// <exception cref="IOException">The temporary file already exists and cannot be deleted.</exception>
        private void RemoveAlreadyExistingTemporaryFile()
        {
            if (File.Exists(temporaryFilePath))
            {
                try
                {
                    File.Delete(temporaryFilePath);
                }
                catch (Exception e)
                {
                    if (e is ArgumentException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
                    {
                        string message = string.Format(
                            Resources.SafeOverwriteFileHelper_RemoveAlreadyExistingTemporaryFile_Already_existing_temporary_file_at_FilePath_0_could_not_be_removed,
                            temporaryFilePath);
                        throw new IOException(message, e);
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Creates a temporary file from the target file.
        /// </summary>
        /// <exception cref="IOException">The temporary file cannot be created from the target file.</exception>
        private void CreateNewTemporaryFile()
        {
            try
            {
                File.Move(targetFilePath, temporaryFilePath);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is UnauthorizedAccessException || e is NotSupportedException)
                {
                    string message = string.Format(
                        Resources.SafeOverwriteFileHelper_CreateNewTemporaryFile_Cannot_create_temporary_FilePath_0_Try_change_save_location,
                        targetFilePath);
                    throw new IOException(message, e);
                }

                throw;
            }
        }

        /// <summary>
        /// Moves the temporary file back to the original path. If the operation fails, the temporary file
        /// will remain.
        /// </summary>
        /// <exception cref="IOException">Thrown when either:
        /// <list type="bullet">
        /// <item>The new file could not be deleted.</item>
        /// <item>The temporary file could not be moved to its original place.</item>
        /// </list></exception>
        private void RestoreOriginalFile()
        {
            try
            {
                File.Delete(targetFilePath);
                File.Move(temporaryFilePath, targetFilePath);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
                {
                    string message = string.Format(
                        Resources.SafeOverwriteFileHelper_RestoreOriginalFile_Cannot_revert_to_original_FilePath_0_Try_reverting_manually,
                        targetFilePath);
                    throw new IOException(message, e);
                }

                throw;
            }
        }

        /// <summary>
        /// Deletes the created temporary file.
        /// </summary>
        /// <exception cref="CannotDeleteBackupFileException">The temporary file cannot be removed.</exception>
        private void DeleteTemporaryFile()
        {
            try
            {
                File.Delete(temporaryFilePath);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
                {
                    string message = string.Format(
                        Resources.SafeOverwriteFileHelper_DeleteTemporaryFile_Cannot_remove_temporary_FilePath_0_Try_removing_manually,
                        temporaryFilePath);
                    throw new CannotDeleteBackupFileException(message, e);
                }

                throw;
            }
        }
    }
}