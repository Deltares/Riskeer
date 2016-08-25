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
    /// Provider of <see cref="HydraulicModelsSetting"/>.
    /// </summary>
    internal class HydraulicModelsSettingsProvider
    {
        private readonly IDictionary<HydraRingFailureMechanismType, IDictionary<string, HydraulicModelsSetting>> fileHydraulicModelsSettings;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicModelsSettingsProvider"/>.
        /// </summary>
        public HydraulicModelsSettingsProvider()
        {
            fileHydraulicModelsSettings = new HydraulicModelsSettingsCsvReader(Resources.HydraulicModelsSettings).ReadSettings();
        }

        /// <summary>
        /// Gets <see cref="HydraulicModelsSetting"/> based on the provided failure mechanism type and ring id.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="HydraRingFailureMechanismType"/> to obtain the <see cref="HydraulicModelsSetting"/> for.</param>
        /// <param name="ringId">The ring id to obtain the <see cref="HydraulicModelsSetting"/> for.</param>
        /// <returns>The <see cref="HydraulicModelsSetting"/> corresponding to the provided failure mechanism type and ring id.</returns>
        public HydraulicModelsSetting GetHydraulicModelsSetting(HydraRingFailureMechanismType failureMechanismType, string ringId)
        {
            if (fileHydraulicModelsSettings.ContainsKey(failureMechanismType) &&
                ringId != null &&
                fileHydraulicModelsSettings[failureMechanismType].ContainsKey(ringId))
            {
                return fileHydraulicModelsSettings[failureMechanismType][ringId];
            }

            return new HydraulicModelsSetting(1);
        }
    }
}