// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.IO;
using Core.Common.Util;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Helper class for providing a convenient method for obtaining and updating Hydra-Ring settings per location from the settings database 
    /// based on <see cref="HydraRingCalculationInput"/>.
    /// </summary>
    public static class HydraRingSettingsDatabaseHelper
    {
        /// <summary>
        /// Obtains the Hydra-Ring settings based on the location and the failure mechanism obtained from the <paramref name="calculationInput"/>
        /// and sets these value on the <paramref name="calculationInput"/>.
        /// </summary>
        /// <param name="calculationInput">The calculation input for which the settings are updated.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        public static void AssignSettingsFromDatabase(HydraRingCalculationInput calculationInput, string hydraulicBoundaryDatabaseFilePath, bool usePreprocessor)
        {
            IOUtils.ValidateFilePath(hydraulicBoundaryDatabaseFilePath);

            long locationId = calculationInput.HydraulicBoundaryLocationId;
            string settingsDatabaseFileName = HydraulicBoundaryDatabaseHelper.GetHydraulicBoundarySettingsDatabase(hydraulicBoundaryDatabaseFilePath);

            using (var preprocessorSettingsProvider = new PreprocessorSettingsProvider(settingsDatabaseFileName))
            {
                calculationInput.PreprocessorSetting = preprocessorSettingsProvider.GetPreprocessorSetting(locationId, usePreprocessor);
            }

            using (var designTablesSettingsProviders = new DesignTablesSettingsProvider(settingsDatabaseFileName))
            {
                calculationInput.DesignTablesSetting = designTablesSettingsProviders.GetDesignTablesSetting(
                    locationId,
                    calculationInput.FailureMechanismType);
            }

            using (var numericsSettingsProvider = new NumericsSettingsProvider(settingsDatabaseFileName))
            {
                calculationInput.NumericsSettings = numericsSettingsProvider.GetNumericsSettings(
                    locationId,
                    calculationInput.FailureMechanismType);
            }

            using (var timeIntegrationSettingsProvider = new TimeIntegrationSettingsProvider(settingsDatabaseFileName))
            {
                calculationInput.TimeIntegrationSetting = timeIntegrationSettingsProvider.GetTimeIntegrationSetting(
                    locationId,
                    calculationInput.FailureMechanismType);
            }
        }
    }
}