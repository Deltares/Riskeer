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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base.IO;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// Provider of <see cref="DesignTablesSetting"/> by reading from the settings database or by resorting
    /// to defaults if no settings could be found.
    /// </summary>
    public class DesignTablesSettingsProvider : IDisposable
    {
        private readonly HydraRingSettingsDatabaseReader designTablesSettingsReader;
        private IDictionary<HydraRingFailureMechanismType, DesignTablesSetting> defaultDesignTablesSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="DesignTablesSettingsProvider"/> class.
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
        public DesignTablesSettingsProvider(string databaseFilePath)
        {
            InitializeDefaultDesignTablesSettings();

            designTablesSettingsReader = new HydraRingSettingsDatabaseReader(databaseFilePath);
        }

        /// <summary>
        /// Gets <see cref="DesignTablesSetting"/> based on the provided failure mechanism type and location id.
        /// </summary>
        /// <param name="locationId">The location id to obtain the <see cref="DesignTablesSetting"/> for.</param>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="DesignTablesSetting"/> for.</param>
        /// <returns>The <see cref="DesignTablesSetting"/> corresponding to the provided failure mechanism type and location id.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="failureMechanismType"/> is not a valid
        /// <see cref="HydraRingFailureMechanismType"/> value.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public DesignTablesSetting GetDesignTablesSetting(long locationId, HydraRingFailureMechanismType failureMechanismType)
        {
            return designTablesSettingsReader.ReadDesignTableSetting(locationId, failureMechanismType) ??
                   defaultDesignTablesSettings[failureMechanismType];
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
                designTablesSettingsReader.Dispose();
            }
        }

        private void InitializeDefaultDesignTablesSettings()
        {
            defaultDesignTablesSettings = new Dictionary<HydraRingFailureMechanismType, DesignTablesSetting>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new DesignTablesSetting(2, 4)
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new DesignTablesSetting(1, 4)
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new DesignTablesSetting(5, 15)
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new DesignTablesSetting(5, 15)
                },
                {
                    HydraRingFailureMechanismType.QVariant, new DesignTablesSetting(10, 50)
                },
                {
                    HydraRingFailureMechanismType.DikeHeight, new DesignTablesSetting(2, 4)
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new DesignTablesSetting(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new DesignTablesSetting(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new DesignTablesSetting(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new DesignTablesSetting(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.DunesBoundaryConditions, new DesignTablesSetting(3.5, 6)
                },
                {
                    HydraRingFailureMechanismType.OvertoppingRate, new DesignTablesSetting(0.001, 0.01)
                }
            };
        }
    }
}