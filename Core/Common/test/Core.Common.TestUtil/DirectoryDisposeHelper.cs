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
using System.IO;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// This class can be used to set temporary folders while testing. 
    /// Disposing an instance of this class will delete the folder(s).
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new DirectoryDisposeHelper("path")) {
    ///     // Perform tests with folders
    /// }
    /// </code>
    /// </example>
    public class DirectoryDisposeHelper : IDisposable
    {
        private readonly string folderPath;
        private bool disposed;

        /// <summary>
        /// Creates a new instance of <see cref="DirectoryDisposeHelper"/>.
        /// </summary>
        /// <param name="folderPath">Paths that will be created, if the path is valid.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="folderPath"/> could 
        /// not be created by the system.</exception>
        public DirectoryDisposeHelper(string folderPath)
        {
            this.folderPath = folderPath;
            CreatePath();
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

            try
            {
                Directory.Delete(folderPath, true);
            }
            catch
            {
                // ignored
            }

            disposed = true;
        }

        /// <summary>
        /// Creates the temporary folder path.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the file could not be created by the system.</exception>
        private void CreatePath()
        {
            try
            {
                Directory.CreateDirectory(folderPath);
            }
            catch (Exception e) when (e is DirectoryNotFoundException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
            {
                throw new ArgumentException(e.Message, e);
            }
        }
    }
}