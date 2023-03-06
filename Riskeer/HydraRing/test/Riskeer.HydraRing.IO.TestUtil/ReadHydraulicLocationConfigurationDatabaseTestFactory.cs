// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Riskeer.HydraRing.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ReadHydraulicLocationConfigurationDatabase"/> instances that can be used
    /// for testing.
    /// </summary>
    public static class ReadHydraulicLocationConfigurationDatabaseTestFactory
    {
        /// <summary>
        /// Creates a <see cref="ReadHydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <param name="trackId">The id of the track that should be part of the read tracks.</param>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationDatabase"/>.</returns>
        public static ReadHydraulicLocationConfigurationDatabase Create(long trackId)
        {
            return Create(new long[]
            {
                1,
                2
            }, trackId);
        }

        /// <summary>
        /// Creates a <see cref="ReadHydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <param name="locationIds">The location ids to add.</param>
        /// <param name="trackId">The id of the track that should be part of the read tracks.</param>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationDatabase"/>.</returns>
        public static ReadHydraulicLocationConfigurationDatabase Create(IEnumerable<long> locationIds, long trackId)
        {
            return Create(locationIds, null, trackId);
        }

        /// <summary>
        /// Create a valid instance of <see cref="ReadHydraulicLocationConfigurationDatabase"/> with hydraulic location
        /// configuration settings.
        /// </summary>
        /// <param name="trackId">The id of the track that should be part of the read tracks.</param>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationDatabase"/> with hydraulic location configuration
        /// settings.</returns>
        public static ReadHydraulicLocationConfigurationDatabase CreateWithConfigurationSettings(long trackId)
        {
            ReadHydraulicLocationConfigurationSettings[] settings =
            {
                ReadHydraulicLocationConfigurationSettingsTestFactory.Create()
            };

            return CreateWithConfigurationSettings(settings, trackId);
        }

        /// <summary>
        /// Create a valid instance of <see cref="ReadHydraulicLocationConfigurationDatabase"/> with hydraulic location
        /// configuration settings.
        /// </summary>
        /// <param name="settings">The <see cref="ReadHydraulicLocationConfigurationSettings"/> to set.</param>
        /// <param name="trackId">The id of the track that should be part of the read tracks.</param>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationDatabase"/> with hydraulic location configuration
        /// settings.</returns>
        public static ReadHydraulicLocationConfigurationDatabase CreateWithConfigurationSettings(IEnumerable<ReadHydraulicLocationConfigurationSettings> settings,
                                                                                                 long trackId)
        {
            return Create(new long[]
            {
                1,
                2
            }, settings, trackId);
        }

        private static ReadHydraulicLocationConfigurationDatabase Create(IEnumerable<long> locationIds,
                                                                         IEnumerable<ReadHydraulicLocationConfigurationSettings> settings,
                                                                         long trackId)
        {
            return new ReadHydraulicLocationConfigurationDatabase(locationIds.Select(locationId => new ReadHydraulicLocation(locationId + 100, locationId, trackId)),
                                                                  settings,
                                                                  new[]
                                                                  {
                                                                      new ReadTrack(trackId, false)
                                                                  });
        }
    }
}