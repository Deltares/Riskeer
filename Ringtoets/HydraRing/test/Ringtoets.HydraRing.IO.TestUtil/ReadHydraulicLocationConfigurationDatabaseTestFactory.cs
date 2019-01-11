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
using System.Linq;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Ringtoets.HydraRing.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ReadHydraulicLocationConfigurationDatabase"/> instances
    /// that can be used for testing.
    /// </summary>
    public static class ReadHydraulicLocationConfigurationDatabaseTestFactory
    {
        /// <summary>
        /// Creates a <see cref="ReadHydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationDatabase"/>.</returns>
        public static ReadHydraulicLocationConfigurationDatabase Create()
        {
            return Create(new long[]
            {
                1,
                2
            });
        }

        /// <summary>
        /// Creates a <see cref="ReadHydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <param name="locationIds">The location ids to add.</param>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationDatabase"/>.</returns>
        public static ReadHydraulicLocationConfigurationDatabase Create(IEnumerable<long> locationIds)
        {
            return Create(locationIds, null);
        }

        /// <summary>
        /// Create a valid instance of <see cref="ReadHydraulicLocationConfigurationDatabase"/>
        /// with hydraulic location configuration database settings.
        /// </summary>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationDatabase"/> with hydraulic location configuration database settings.</returns>
        public static ReadHydraulicLocationConfigurationDatabase CreateWithConfigurationSettings()
        {
            ReadHydraulicLocationConfigurationDatabaseSettings[] settings =
            {
                ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create()
            };

            return Create(new long[]
            {
                1,
                2
            }, settings);
        }

        /// <summary>
        /// Create a valid instance of <see cref="ReadHydraulicLocationConfigurationDatabase"/>
        /// with hydraulic location configuration database settings.
        /// </summary>
        /// <param name="settings">The <see cref="ReadHydraulicLocationConfigurationDatabaseSettings"/> to set.</param>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationDatabase"/> with hydraulic location configuration database settings.</returns>
        public static ReadHydraulicLocationConfigurationDatabase CreateWithConfigurationSettings(IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> settings)
        {
            return Create(new long[]
            {
                1,
                2
            }, settings);
        }

        private static ReadHydraulicLocationConfigurationDatabase Create(IEnumerable<long> locationIds,
                                                                         IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> settings)
        {
            return new ReadHydraulicLocationConfigurationDatabase(locationIds.Select(locationId => new ReadHydraulicLocationMapping(locationId, locationId + 100))
                                                                             .ToList(),
                                                                  settings);
        }
    }
}