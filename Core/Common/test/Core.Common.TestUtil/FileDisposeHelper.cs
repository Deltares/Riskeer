﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        private readonly IEnumerable<string> files;

        /// <summary>
        /// Creates a new instance of <see cref="FileDisposeHelper"/>.
        /// </summary>
        /// <param name="filePaths">Path of the files that will be used.</param>
        public FileDisposeHelper(IEnumerable<string> filePaths)
        {
            files = filePaths;
            Create();
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileDisposeHelper"/>.
        /// </summary>
        /// <param name="filePath">Path of the single file that will be used.</param>
        public FileDisposeHelper(string filePath) : this(new [] { filePath })
        {
        }

        /// <summary>
        /// Creates the temporary files.
        /// </summary>
        private void Create()
        {
            foreach (var file in files)
            {
                CreateFile(file);
            }
        }

        /// <summary>
        /// Disposes the <see cref="FileDisposeHelper"/> instance.
        /// </summary>
        public void Dispose()
        {
            foreach (var file in files)
            {
                Dispose(file);
            }
        }

        private static void CreateFile(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                using (File.Create(filename)) {}
            }
        }

        private static void Dispose(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                File.Delete(filename);
            }
        }
    }
}