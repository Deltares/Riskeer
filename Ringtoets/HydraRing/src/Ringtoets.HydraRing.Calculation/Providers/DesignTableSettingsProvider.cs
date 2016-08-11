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
    /// Provider of <see cref="DesignTableSettings"/>.
    /// </summary>
    internal class DesignTableSettingsProvider
    {
        private readonly IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTableSettings>> fileDesignTableSettings;
        private IDictionary<HydraRingFailureMechanismType, DesignTableSettings> defaultDesignTableSettings;

        /// <summary>
        /// Creates a new instance of the <see cref="DesignTableSettingsProvider"/> class.
        /// </summary>
        public DesignTableSettingsProvider()
        {
            InitializeDefaultDesignTableSettings();

            fileDesignTableSettings = new DesignTableSettingsCsvReader(Resources.DesignTableSettings).ReadSettings();
        }

        /// <summary>
        /// Returns <see cref="DesignTableSettings"/> based on the provided <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="DesignTableSettings"/> for.</param>
        /// <param name="ringId"></param>
        /// <returns>The <see cref="DesignTableSettings"/> corresponding to the provided <see cref="HydraRingFailureMechanismType"/>.</returns>
        public DesignTableSettings GetDesignTableSettings(HydraRingFailureMechanismType failureMechanismType, string ringId)
        {
            if (fileDesignTableSettings.ContainsKey(failureMechanismType) &&
                ringId != null &&
                fileDesignTableSettings[failureMechanismType].ContainsKey(ringId))
            {
                return fileDesignTableSettings[failureMechanismType][ringId];
            }

            return defaultDesignTableSettings[failureMechanismType];
        }

        private void InitializeDefaultDesignTableSettings()
        {
            defaultDesignTableSettings = new Dictionary<HydraRingFailureMechanismType, DesignTableSettings>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel,
                    new DesignTableSettings(5, 15)
                },
                {
                    HydraRingFailureMechanismType.WaveHeight,
                    new DesignTableSettings(5, 15)
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod,
                    new DesignTableSettings(5, 15)
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod,
                    new DesignTableSettings(5, 15)
                },
                {
                    HydraRingFailureMechanismType.QVariant,
                    new DesignTableSettings(5, 15)
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping,
                    new DesignTableSettings(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.DikesHeight,
                    new DesignTableSettings(5, 15)
                },
                {
                    HydraRingFailureMechanismType.DikesPiping,
                    new DesignTableSettings(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping,
                    new DesignTableSettings(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure,
                    new DesignTableSettings(double.NaN, double.NaN)
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure,
                    new DesignTableSettings(double.NaN, double.NaN)
                }
            };
        }
    }
}