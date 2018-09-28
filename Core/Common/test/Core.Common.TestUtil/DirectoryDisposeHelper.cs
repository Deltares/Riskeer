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
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// This class can be used to set temporary folders while testing. 
    /// Disposing an instance of this class will delete the subfolder(s).
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new DirectoryDisposeHelper("root path", "sub folder to create" 
    ///     [, "sub sub folder to create"])) {
    ///     // Perform tests with folders
    /// }
    /// </code>
    /// </example>
    public class DirectoryDisposeHelper : IDisposable
    {
        private const int numberOfAdditionalDeleteAttempts = 3;
        private readonly string rootPathToTemp;
        private readonly string fullPath;
        private bool disposed;
        private DirectoryPermissionsRevoker directoryPermissionsRevoker;

        /// <summary>
        /// Creates a new instance of <see cref="DirectoryDisposeHelper"/>.
        /// </summary>
        /// <param name="rootFolder">Root folder to create the temporary folders in.</param>
        /// <param name="subFolders">Path that will temporarily be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rootFolder"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="subFolders"/> is <c>null</c>, empty, or could 
        /// not be created by the system.</exception>
        public DirectoryDisposeHelper(string rootFolder, params string[] subFolders)
        {
            if (rootFolder == null)
            {
                throw new ArgumentNullException(nameof(rootFolder));
            }

            if (subFolders == null || !subFolders.Any())
            {
                throw new ArgumentException(@"Must have at least one sub folder.", nameof(subFolders));
            }

            rootPathToTemp = Path.Combine(rootFolder, subFolders[0]);
            fullPath = Path.Combine(rootFolder, Path.Combine(subFolders));
            CreatePath();
        }

        /// <summary>
        /// Adds a <paramref name="rights"/> of type <see cref="AccessControlType.Deny"/> to the access
        /// rule set for the folder at the set directory path.
        /// </summary>
        /// <param name="rights">The right to deny.</param>
        /// <exception cref="InvalidOperationException">Thrown when the directory could not be locked.</exception>
        /// <remarks>The lock is removed when disposing the instance.</remarks>
        public void LockDirectory(FileSystemRights rights)
        {
            try
            {
                directoryPermissionsRevoker = new DirectoryPermissionsRevoker(rootPathToTemp, rights);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Unable to lock '{rootPathToTemp}'.", exception);
            }
        }

        /// <summary>
        /// Unlocks the directory that was locked by <see cref="LockDirectory"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the directory was not locked.</exception>
        public void UnlockDirectory()
        {
            if (directoryPermissionsRevoker == null)
            {
                throw new InvalidOperationException($"Directory '{rootPathToTemp}' is not locked.");
            }

            directoryPermissionsRevoker.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                directoryPermissionsRevoker?.Dispose();
            }

            var attempts = 0;
            while (!TryDeleteRootFolder() && attempts < numberOfAdditionalDeleteAttempts)
            {
                attempts++;

                GC.WaitForPendingFinalizers();
                Thread.Sleep(10);
            }

            disposed = true;
        }

        private bool TryDeleteRootFolder()
        {
            try
            {
                Directory.Delete(rootPathToTemp, true);
            }
            catch (Exception e)
            {
                if (e is IOException)
                {
                    return false;
                }

                // Ignore other exceptions
            }

            return true;
        }

        /// <summary>
        /// Creates the temporary folder path.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the file could not be created by the system.</exception>
        private void CreatePath()
        {
            try
            {
                Directory.CreateDirectory(fullPath);
            }
            catch (Exception e) when (e is DirectoryNotFoundException
                                      || e is IOException
                                      || e is NotSupportedException
                                      || e is UnauthorizedAccessException)
            {
                throw new ArgumentException(e.Message, e);
            }
        }

        ~DirectoryDisposeHelper()
        {
            Dispose(false);
        }
    }
}