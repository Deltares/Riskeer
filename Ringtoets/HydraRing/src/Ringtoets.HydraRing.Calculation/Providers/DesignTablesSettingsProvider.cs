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

using System.Collections.Generic;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.IO;
using Ringtoets.HydraRing.Calculation.Properties;

namespace Ringtoets.HydraRing.Calculation.Providers
{
    /// <summary>
    /// Provider of <see cref="DesignTablesSetting"/>.
    /// </summary>
    internal class DesignTablesSettingsProvider
    {
        private readonly IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>> fileDesignTablesSettings;
        private IDictionary<HydraRingFailureMechanismType, DesignTablesSetting> defaultDesignTablesSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="DesignTablesSettingsProvider"/> class.
        /// </summary>
        public DesignTablesSettingsProvider()
        {
            InitializeDefaultDesignTablesSettings();

            fileDesignTablesSettings = new DesignTablesSettingsCsvReader(Resources.DesignTablesSettings).ReadSettings();
        }

        /// <summary>
        /// Gets <see cref="DesignTablesSetting"/> based on the provided failure mechanism type and ring id.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="DesignTablesSetting"/> for.</param>
        /// <param name="ringId">The ring id to obtain the <see cref="DesignTablesSetting"/> for.</param>
        /// <returns>The <see cref="DesignTablesSetting"/> corresponding to the provided failure mechanism type and ring id.</returns>
        public DesignTablesSetting GetDesignTablesSetting(HydraRingFailureMechanismType failureMechanismType, string ringId)
        {
            if (fileDesignTablesSettings.ContainsKey(failureMechanismType) &&
                ringId != null &&
                fileDesignTablesSettings[failureMechanismType].ContainsKey(ringId))
            {
                return fileDesignTablesSettings[failureMechanismType][ringId];
            }

            return defaultDesignTablesSettings[failureMechanismType];
        }

        private void InitializeDefaultDesignTablesSettings()
        {
            defaultDesignTablesSettings = new Dictionary<HydraRingFailureMechanismType, DesignTablesSetting>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel,
                    new DesignTablesSetting(2, 4)
                },
                {
                    HydraRingFailureMechanismType.WaveHeight,
                    new DesignTablesSetting(1, 4)
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod,
                    new DesignTablesSetting(5, 15)
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod,
                    new DesignTablesSetting(5, 15)
                },
                {
                    HydraRingFailureMechanismType.QVariant,
                    new DesignTablesSetting(10, 50)
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping,
                    new DesignTablesSetting(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.DikesHeight,
                    new DesignTablesSetting(2, 4)
                },
                {
                    HydraRingFailureMechanismType.DikesPiping,
                    new DesignTablesSetting(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping,
                    new DesignTablesSetting(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure,
                    new DesignTablesSetting(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure,
                    new DesignTablesSetting(double.NaN, double.NaN)
                }
            };
        }
    }
}