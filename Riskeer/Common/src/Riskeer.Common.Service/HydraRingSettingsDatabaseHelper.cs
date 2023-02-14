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

using System;
using Core.Common.Base.IO;
using Core.Common.Util;
using Riskeer.Common.IO.HydraRing;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.Common.Service
{
    /// <summary>
    /// Helper class for obtaining and updating Hydra-Ring settings.
    /// </summary>
    public static class HydraRingSettingsDatabaseHelper
    {
        /// <summary>
        /// Assigns Hydra-Ring settings to the provided <paramref name="calculationInput"/>.
        /// </summary>
        /// <param name="calculationInput">The calculation input for which the settings are updated.</param>
        /// <param name="hrdFilePath">The path to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hrdFilePath"/> contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>no hydraulic boundary settings database could be found at the location of <paramref name="hrdFilePath"/>;</item>
        /// <item>the hydraulic boundary settings database cannot be opened;</item>
        /// <item>the required data cannot be read from the hydraulic boundary settings database.</item>
        /// </list>
        /// </exception>
        public static void AssignSettingsFromDatabase(HydraRingCalculationInput calculationInput, string hrdFilePath, bool usePreprocessor)
        {
            IOUtils.ValidateFilePath(hrdFilePath);

            long locationId = calculationInput.HydraulicBoundaryLocationId;
            string hbsdFilePath = HydraulicBoundaryDataHelper.GetHydraulicBoundarySettingsDatabase(hrdFilePath);

            using (var preprocessorSettingsProvider = new PreprocessorSettingsProvider(hbsdFilePath))
            {
                calculationInput.PreprocessorSetting = preprocessorSettingsProvider.GetPreprocessorSetting(locationId, usePreprocessor);
            }

            using (var designTablesSettingsProviders = new DesignTablesSettingsProvider(hbsdFilePath))
            {
                calculationInput.DesignTablesSetting = designTablesSettingsProviders.GetDesignTablesSetting(
                    locationId,
                    calculationInput.FailureMechanismType);
            }

            using (var numericsSettingsProvider = new NumericsSettingsProvider(hbsdFilePath))
            {
                calculationInput.NumericsSettings = numericsSettingsProvider.GetNumericsSettings(
                    locationId,
                    calculationInput.FailureMechanismType);
            }

            using (var timeIntegrationSettingsProvider = new TimeIntegrationSettingsProvider(hbsdFilePath))
            {
                calculationInput.TimeIntegrationSetting = timeIntegrationSettingsProvider.GetTimeIntegrationSetting(
                    locationId,
                    calculationInput.FailureMechanismType);
            }
        }
    }
}