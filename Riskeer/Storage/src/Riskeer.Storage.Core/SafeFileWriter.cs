// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Storage.Core.Properties;

namespace Riskeer.Storage.Core
{
    /// <summary>
    /// Class for providing a safe way of writing files by:
    /// <list type="bullet">
    /// <item>creating and locking a temporary file in order to prevent race conditions (caused by writing to the same target file from different threads/processes);</item> 
    /// <item>using the temporary file to store any former target file (this way rollback can take place in case of errors).</item>
    /// </list>
    /// </summary>
    public class SafeFileWriter
    {
        private readonly string targetFilePath;
        private readonly string temporaryFilePath;
        private FileStream temporaryFileStream;
        private bool isTemporaryFileEmpty;

        /// <summary>
        /// Creates an instance of <see cref="SafeFileWriter"/>.
        /// </summary>
        /// <param name="targetFilePath">The path of the file which will be (over)written.</param>
        /// <param name="temporaryFileExtension">The extension the temporary file should get.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="targetFilePath"/> is not a valid path.</exception>
        public SafeFileWriter(string targetFilePath, string temporaryFileExtension)
        {
            IOUtils.ValidateFilePath(targetFilePath);

            this.targetFilePath = targetFilePath;
            temporaryFilePath = Path.ChangeExtension(targetFilePath, temporaryFileExtension);
        }

        /// <summary>
        /// Performs the <paramref name="writeAction"/> in a safe way. Firstly by creating and locking a temporary file; when no
        /// temporary file can be created, the <paramref name="writeAction"/> will not be performed. Secondly by backing up any
        /// existing target file; it is expected that the <paramref name="writeAction"/> will throw an exception when the
        /// operation fails, so that the backed up target file can be restored.
        /// </summary>
        /// <param name="writeAction">The write action to perform.</param>
        /// <exception cref="IOException">Thrown when no temporary file can be created, indicating:
        /// <list type="bullet">
        /// <item>access rights are insufficient;</item>
        /// <item>the target file path is too long;</item>
        /// <item>the target file is already in use by another process/thread.</item>
        /// </list>
        /// </exception>
        /// <remarks>Any <see cref="Exception"/> thrown by <paramref name="writeAction"/> will be rethrown.</remarks>
        public void Perform(Action writeAction)
        {
            TryCreateTemporaryFile();

            try
            {
                writeAction();
            }
            catch
            {
                if (isTemporaryFileEmpty)
                {
                    RemoveTemporaryFile();
                }
                else
                {
                    RestoreTargetFile();
                }

                throw;
            }

            RemoveTemporaryFile();
        }

        /// <summary>
        /// Tries to create a temporary file and backs up any existing target file.
        /// </summary>
        /// <exception cref="IOException">Thrown when the temporary file cannot be created, indicating:
        /// <list type="bullet">
        /// <item>access rights are insufficient;</item>
        /// <item>the target file path is too long;</item>
        /// <item>the target file is already in use by another process/thread.</item>
        /// </list>
        /// </exception>
        private void TryCreateTemporaryFile()
        {
            try
            {
                if (File.Exists(targetFilePath))
                {
                    isTemporaryFileEmpty = false;

                    ThrowWhenTargetFileIsReadonly();

                    File.Move(targetFilePath, temporaryFilePath);

                    temporaryFileStream = File.Open(temporaryFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                }
                else
                {
                    isTemporaryFileEmpty = true;

                    temporaryFileStream = File.Open(temporaryFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new IOException(string.Format(Resources.SafeFileWriter_Insufficient_access_rights));
            }
            catch (PathTooLongException)
            {
                throw new IOException(string.Format(Resources.SafeFileWriter_Path_too_long));
            }
            catch (IOException)
            {
                throw new IOException(string.Format(Resources.SafeFileWriter_Target_file_currently_in_use));
            }
        }

        private void RestoreTargetFile()
        {
            File.Delete(targetFilePath);

            temporaryFileStream.Close();

            File.Move(temporaryFilePath, targetFilePath);
        }

        private void RemoveTemporaryFile()
        {
            temporaryFileStream.Close();

            File.Delete(temporaryFilePath);
        }

        private void ThrowWhenTargetFileIsReadonly()
        {
            using (File.Open(targetFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                // Do nothing
            }
        }
    }
}