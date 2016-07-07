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
using Core.Common.Utils;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// This class can be used to create a temporary directory while testing. 
    /// Disposing an instance of this class will delete the directory and its contents.
    /// </summary>
    /// <example>
    /// The following is an example for how to use this class:
    /// <code>
    /// using(new DirectoryDisposeHelper("pathToDirectory")) {
    ///     // Perform tests with directory
    /// }
    /// </code>
    /// </example>
    public class DirectoryDisposeHelper : IDisposable
    {
        private string directory;

        /// <summary>
        /// Creates a new instance of <see cref="DirectoryDisposeHelper"/>.
        /// </summary>
        /// <param name="directory">Path of the files that will be used.</param>
        public DirectoryDisposeHelper(string directory)
        {
            this.directory = directory;
            Create();
        }

        public void Dispose()
        {
            if (directory != null)
            {
                Directory.Delete(directory, true);
            }
        }

        private void Create()
        {
            try
            {
                FileUtils.ValidateFilePath(directory);
                Directory.CreateDirectory(directory);
            }
            catch (ArgumentException)
            {
                directory = null;
            }
        }
    }
}