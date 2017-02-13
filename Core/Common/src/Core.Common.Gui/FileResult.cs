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

namespace Core.Common.Gui
{
    /// <summary>
    /// The result of a file inquiry operation.
    /// </summary>
    public class FileResult
    {
        /// <summary>
        /// Creates a new <see cref="FileResult"/>.
        /// </summary>
        /// <param name="filePath">The path to a file.</param>
        public FileResult(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Gets the path to the file.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Returns <c>true</c> if the <see cref="FileResult"/> contains a
        /// <see cref="FilePath"/>, <c>false</c> othwerise.
        /// </summary>
        public bool HasFilePath
        {
            get
            {
                return FilePath != null;
            }
        }
    }
}