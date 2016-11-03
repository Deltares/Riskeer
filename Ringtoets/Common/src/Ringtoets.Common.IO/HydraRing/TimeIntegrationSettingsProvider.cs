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
using Core.Common.IO.Exceptions;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// Provider of <see cref="TimeIntegrationSetting"/>.
    /// </summary>
    public class TimeIntegrationSettingsProvider : IDisposable
    {
        private readonly HydraRingSettingsDatabaseReader timeIntegrationSettingsReader;

        /// <summary>
        /// Creates a new instance of <see cref="TimeIntegrationSettingsProvider"/>.
        /// </summary>
        /// <param name="databaseFilePath">The full path to the database file to use when reading settings.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// </list>
        /// </exception>
        public TimeIntegrationSettingsProvider(string databaseFilePath)
        {
            timeIntegrationSettingsReader = new HydraRingSettingsDatabaseReader(databaseFilePath);
        }

        /// <summary>
        /// Gets <see cref="TimeIntegrationSetting"/> based on the provided failure mechanism type and location id.
        /// </summary>
        /// <param name="locationId">The location id to obtain the <see cref="TimeIntegrationSetting"/> for.</param>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="TimeIntegrationSetting"/> for.</param>
        /// <returns>The <see cref="TimeIntegrationSetting"/> corresponding to the provided failure mechanism type and location id.</returns>
        public TimeIntegrationSetting GetTimeIntegrationSetting(long locationId, HydraRingFailureMechanismType failureMechanismType)
        {
            return timeIntegrationSettingsReader.ReadTimeIntegrationSetting(locationId, failureMechanismType) ??
                   new TimeIntegrationSetting(1);
        }

        public void Dispose()
        {
            timeIntegrationSettingsReader.Dispose();
        }
    }
}