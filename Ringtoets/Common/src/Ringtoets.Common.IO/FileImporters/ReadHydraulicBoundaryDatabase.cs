// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.IO.FileImporters
{
    /// <summary>
    /// Class for holding data that is read from a hydraulic boundary database file.
    /// </summary>
    public class ReadHydraulicBoundaryDatabase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadHydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="version">The version of the read hydraulic boundary database.</param>
        /// <param name="locations">The read hydraulic boundary locations.</param>
        public ReadHydraulicBoundaryDatabase(string version, IEnumerable<HydraulicBoundaryLocation> locations)
        {
            Version = version;
            Locations = locations;
        }

        /// <summary>
        /// Gets the version of the read hydraulic boundary database.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the read hydraulic boundary locations.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocation> Locations { get; }
    }
}