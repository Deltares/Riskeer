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
using Core.Common.Utils;

namespace Ringtoets.HeightStructures.IO
{
    /// <summary>
    /// File reader for a plain text file in comma-separated values format (*.csv) containing
    /// data specifying characteristics of height structures.
    /// </summary>
    public class HeightStructuresCharacteristicsCsvReader
    {
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCharacteristicsCsvReader"/>
        /// and opens a given file path.
        /// </summary>
        /// <param name="path">The path to the file to be read.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        public HeightStructuresCharacteristicsCsvReader(string path)
        {
            FileUtils.ValidateFilePath(path);

            filePath = path;
        }
    }
}