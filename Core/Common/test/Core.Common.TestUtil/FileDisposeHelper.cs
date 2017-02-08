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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Utils;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// This class can be used to set temporary files while testing. 
    /// Disposing an instance of this class will delete the files.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new FileDisposeHelper(new[]{"pathToFile"})) {
    ///     // Perform tests with files
    /// }
    /// </code>
    /// </example>
    public class FileDisposeHelper : IDisposable
    {
        private readonly Dictionary<string, FileStream> filePathStreams;
        private bool disposed;

        /// <summary>
        /// Creates a new instance of <see cref="FileDisposeHelper"/>.
        /// </summary>
        /// <param name="filePaths">Paths of the files that will be created, if the path is valid.</param>
        /// <exception cref="ArgumentException">Thrown when one of the files in <paramref name="filePaths"/> could 
        /// not be created by the system.</exception>
        public FileDisposeHelper(IEnumerable<string> filePaths)
        {
            filePathStreams = new Dictionary<string, FileStream>();
            foreach (string filePath in filePaths)
            {
                filePathStreams.Add(filePath, null);
            }
            Create();
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileDisposeHelper"/>.
        /// </summary>
        /// <param name="filePath">Path of the file that will be created, if valid.</param>
        /// <exception cref="ArgumentException">Thrown when the file could not be created by the system.</exception>
        public FileDisposeHelper(string filePath) : this(new[]
        {
            filePath
        }) {}

        /// <summary>
        /// Declines sharing of the files specified in the constructor.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when one of the files could not be locked.</exception>
        /// <seealso cref="FileShare.None"/>
        public void LockFiles()
        {
            IEnumerable<KeyValuePair<string, FileStream>> notLockedFiles = filePathStreams.Where(f => f.Value == null).ToArray();
            foreach (KeyValuePair<string, FileStream> filePathStream in notLockedFiles)
            {
                LockFile(filePathStream.Key);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void LockFile(string filePath)
        {
            try
            {
                filePathStreams[filePath] = File.OpenWrite(filePath);
            }
            catch (IOException exception)
            {
                throw new InvalidOperationException($"Unable to lock '{filePath}'.", exception);
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                KeyValuePair<string, FileStream>[] dictionary = filePathStreams.ToArray();
                foreach (KeyValuePair<string, FileStream> filePathStream in dictionary)
                {
                    filePathStream.Value?.Dispose();
                    filePathStreams[filePathStream.Key] = null;

                    try
                    {
                        DeleteFile(filePathStream.Key);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Creates the temporary files.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the file could not be created by the system.</exception>
        private void Create()
        {
            foreach (string filePath in filePathStreams.Keys)
            {
                CreateFile(filePath);
            }
        }

        /// <summary>
        /// Creates a file at at the given file path. If the <see cref="filePath"/> is
        /// invalid, no file is created.
        /// </summary>
        /// <param name="filePath">Path of the new file.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item>The caller does not have the required permission.-or- path specified a file that is read-only.</item>
        /// <item>The specified path, file name, or both exceed the system-defined maximum length. For example, on 
        /// Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260
        /// characters.</item>
        /// <item>The specified path is invalid (for example, it is on an unmapped drive).</item>
        /// <item> An I/O error occurred while creating the file.</item>
        /// </list>
        /// </exception>
        private static void CreateFile(string filePath)
        {
            if (IOUtils.IsValidFilePath(filePath))
            {
                try
                {
                    using (File.Create(filePath)) {}
                }
                catch (Exception e)
                {
                    if (e is DirectoryNotFoundException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
                    {
                        throw new ArgumentException(e.Message);
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a file at the given file path. If the <see cref="filePath"/> is
        /// invalid, no file is deleted (obviously).
        /// </summary>
        /// <param name="filePath">Path of the file to delete.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item>The specified <paramref name="filePath"/> is invalid (for example, it is on an unmapped drive).</item>
        /// <item>The specified <paramref name="filePath"/> is in use. -or-There is an open handle on the file, 
        /// and the operating system is Windows XP or earlier. This open handle can result from enumerating 
        /// directories and files. For more information, see How to: Enumerate Directories and Files.</item>
        /// <item><paramref name="filePath"/> is in an invalid format.</item>
        /// <item>The specified <paramref name="filePath"/> exceed the system-defined 
        /// maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and 
        /// file names must be less than 260 characters.</item>
        /// <item>The caller does not have the required permission.-or- 
        /// <paramref name="filePath"/> is a directory.-or- <paramref name="filePath"/> specified a read-only file.</item>
        /// </list>
        /// </exception>
        private static void DeleteFile(string filePath)
        {
            if (IOUtils.IsValidFilePath(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception e)
                {
                    if (e is DirectoryNotFoundException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
                    {
                        throw new ArgumentException(e.Message);
                    }
                    throw;
                }
            }
        }
    }
}