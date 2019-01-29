// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using Core.Common.Base.IO;
using Core.Common.Base.Storage;

namespace Migration.Scripts.Data
{
    /// <summary>
    /// Interface that defines a file that has a version.
    /// </summary>
    public interface IVersionedFile
    {
        /// <summary>
        /// Gets the location of the versioned file.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Returns the version of the <see cref="IVersionedFile"/>.
        /// </summary>
        /// <returns>The version.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No file could be found at <see cref="Location"/>.</item>
        /// <item>Unable to open file at <see cref="Location"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="StorageValidationException">Thrown when is not a valid file.</exception>
        string GetVersion();
    }
}