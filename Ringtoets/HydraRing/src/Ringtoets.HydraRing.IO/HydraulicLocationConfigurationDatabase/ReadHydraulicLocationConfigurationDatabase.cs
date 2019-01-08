// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase
{
    /// <summary>
    /// Class for holding data that is read from a hydraulic location configuration database file.
    /// </summary>
    public class ReadHydraulicLocationConfigurationDatabase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadHydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <param name="locationIdMappings">The location id mappings of the read hydraulic location
        /// configuration database.</param>
        /// <param name="isScenarioInformationPresent">Indicator whether scenario information is present in the hydraulic location
        /// configuration database.</param>
        /// <param name="readHydraulicLocationConfigurationDatabaseSettings">The hydraulic location configuration settings
        /// of the read hydraulic location database.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="locationIdMappings"/>
        /// ic <c>null</c>.</exception>
        internal ReadHydraulicLocationConfigurationDatabase(IEnumerable<ReadHydraulicLocationMapping> locationIdMappings,
                                                            bool isScenarioInformationPresent,
                                                            IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> readHydraulicLocationConfigurationDatabaseSettings)
        {
            LocationIdMappings = locationIdMappings;
            IsScenarioInformationPresent = isScenarioInformationPresent;
            ReadHydraulicLocationConfigurationDatabaseSettings = readHydraulicLocationConfigurationDatabaseSettings;
        }

        /// <summary>
        /// Gets the location id mappings of the read hydraulic location configuration database.
        /// </summary>
        public IEnumerable<ReadHydraulicLocationMapping> LocationIdMappings { get; }

        /// <summary>
        /// Gets whether the scenario information is present in the hydraulic location configuration database.
        /// </summary>
        public bool IsScenarioInformationPresent { get; }

        /// <summary>
        /// Gets the settings of the read hydraulic location configuration database.
        /// </summary>
        public IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> ReadHydraulicLocationConfigurationDatabaseSettings { get; }
    }
}