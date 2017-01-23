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
using Core.Common.Base.Storage;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;

namespace Migration.Scripts.Data
{
    /// <summary>
    /// Class that defines a file that has a version.
    /// </summary>
    public class VersionedFile
    {
        /// <summary>
        /// Creates a new instance of the <see cref="VersionedFile"/> class.
        /// </summary>
        /// <param name="path">Path to the versioned file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public VersionedFile(string path)
        {
            IOUtils.ValidateFilePath(path);
            Location = path;
        }

        /// <summary>
        /// Gets the location of the versioned file.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// Gets the version of the <see cref="VersionedFile"/>.
        /// </summary>
        /// <returns>The version.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No file could be found at <see cref="Location"/>.</item>
        /// <item>Unable to open file at <see cref="Location"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="StorageValidationException">Thrown when is not a valid file.</exception>
        public string GetVersion()
        {
            using (var sourceFile = new RingtoetsDatabaseSourceFile(Location))
            {
                return sourceFile.GetVersion();
            }
        }
    }
}