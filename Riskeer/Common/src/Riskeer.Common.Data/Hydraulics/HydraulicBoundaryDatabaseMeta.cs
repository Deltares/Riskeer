// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Wrapper class representing the meta data used as a basis for importing a hydraulic boundary database.
    /// </summary>
    public class HydraulicBoundaryDatabaseMeta
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseMeta"/>.
        /// </summary>
        /// <param name="hlcdFilePath">The path to the HLCD file.</param>
        /// <param name="hrdDirectoryPath">The path to the HRD directory.</param>
        /// <param name="locationsFilePath">The path to the locations file.</param>
        public HydraulicBoundaryDatabaseMeta(string hlcdFilePath, string hrdDirectoryPath, string locationsFilePath)
        {
            HlcdFilePath = hlcdFilePath;
            HrdDirectoryPath = hrdDirectoryPath;
            LocationsFilePath = locationsFilePath;
        }

        /// <summary>
        /// Gets the path to the HLCD file.
        /// </summary>
        public string HlcdFilePath { get; }

        /// <summary>
        /// Gets the path to the HRD directory.
        /// </summary>
        public string HrdDirectoryPath { get; }

        /// <summary>
        /// Gets the path to the locations file.
        /// </summary>
        public string LocationsFilePath { get; }
    }
}
