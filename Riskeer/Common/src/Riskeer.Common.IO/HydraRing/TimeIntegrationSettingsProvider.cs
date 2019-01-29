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

using System;
using System.ComponentModel;
using Core.Common.Base.IO;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Settings;

namespace Riskeer.Common.IO.HydraRing
{
    /// <summary>
    /// Provider of <see cref="TimeIntegrationSetting"/> by reading from the settings database or by resorting
    /// to defaults if no settings could be found.
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
        /// <item>The opened database doesn't have the expected schema.</item>
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
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="failureMechanismType"/> is not a valid
        /// <see cref="HydraRingFailureMechanismType"/> value.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public TimeIntegrationSetting GetTimeIntegrationSetting(long locationId, HydraRingFailureMechanismType failureMechanismType)
        {
            return timeIntegrationSettingsReader.ReadTimeIntegrationSetting(locationId, failureMechanismType) ??
                   new TimeIntegrationSetting(1);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                timeIntegrationSettingsReader.Dispose();
            }
        }
    }
}