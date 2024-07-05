// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.Collections.Generic;

namespace Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase
{
    /// <summary>
    /// Class for holding data that is read from a hydraulic location configuration database.
    /// </summary>
    public class ReadHydraulicLocationConfigurationDatabase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadHydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <param name="readHydraulicLocations">The read hydraulic locations.</param>
        /// <param name="readHydraulicLocationConfigurationSettings">The read hydraulic location configuration settings.</param>
        /// <param name="readTracks">The read tracks.</param>
        internal ReadHydraulicLocationConfigurationDatabase(IEnumerable<ReadHydraulicLocation> readHydraulicLocations,
                                                            IEnumerable<ReadHydraulicLocationConfigurationSettings> readHydraulicLocationConfigurationSettings,
                                                            IEnumerable<ReadTrack> readTracks)
        {
            ReadHydraulicLocations = readHydraulicLocations;
            ReadHydraulicLocationConfigurationSettings = readHydraulicLocationConfigurationSettings;
            ReadTracks = readTracks;
        }

        /// <summary>
        /// Gets the read hydraulic locations.
        /// </summary>
        public IEnumerable<ReadHydraulicLocation> ReadHydraulicLocations { get; }

        /// <summary>
        /// Gets the read hydraulic location configuration settings.
        /// </summary>
        public IEnumerable<ReadHydraulicLocationConfigurationSettings> ReadHydraulicLocationConfigurationSettings { get; }

        /// <summary>
        /// Gets the read tracks.
        /// </summary>
        public IEnumerable<ReadTrack> ReadTracks { get; }
    }
}