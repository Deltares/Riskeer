// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
            return Create(locationIds, null, new Dictionary<long, bool>
            {
                {
                    trackId, false
                }
            });
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

            return CreateWithConfigurationSettings(settings, new Dictionary<long, bool>
            {
                {
                    trackId, false
                }
            });
        }

        /// <summary>
        /// Create a valid instance of <see cref="ReadHydraulicLocationConfigurationDatabase"/> with hydraulic location
        /// configuration settings.
        /// </summary>
        /// <param name="tracks">The tracks that should be part of the read tracks.</param>
        /// <returns>The created <see cref="ReadHydraulicLocationConfigurationDatabase"/> with hydraulic location configuration
        /// settings.</returns>
        public static ReadHydraulicLocationConfigurationDatabase CreateWithConfigurationSettings(IDictionary<long, bool> tracks)
        {
            ReadHydraulicLocationConfigurationSettings[] settings =
            {
                ReadHydraulicLocationConfigurationSettingsTestFactory.Create()
            };

            return CreateWithConfigurationSettings(settings, tracks);
        }

        private static ReadHydraulicLocationConfigurationDatabase CreateWithConfigurationSettings(IEnumerable<ReadHydraulicLocationConfigurationSettings> settings,
                                                                                                  IDictionary<long, bool> tracks)
        {
            return Create(new long[]
            {
                1,
                2
            }, settings, tracks);
        }

        private static ReadHydraulicLocationConfigurationDatabase Create(IEnumerable<long> locationIds,
                                                                         IEnumerable<ReadHydraulicLocationConfigurationSettings> settings,
                                                                         IDictionary<long, bool> tracks)
        {
            return new ReadHydraulicLocationConfigurationDatabase(locationIds.Select(locationId => new ReadHydraulicLocation(locationId + 100, locationId, tracks.First().Key)),
                                                                  settings,
                                                                  tracks.Select(t => new ReadTrack(t.Key, t.Value)));
        }
    }
}