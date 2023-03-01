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
using log4net;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.Plugin.Properties;

namespace Riskeer.Integration.Plugin.Helpers
{
    /// <summary>
    /// Helper class for updating <see cref="HydraulicLocationConfigurationSettings"/>.
    /// </summary>
    public static class HydraulicLocationConfigurationSettingsUpdateHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicLocationConfigurationSettingsUpdateHelper));

        /// <summary>
        /// Sets the hydraulic location configuration settings.
        /// </summary>
        /// <param name="hydraulicLocationConfigurationSettings">The hydraulic location configuration settings to update.</param>
        /// <param name="readHydraulicLocationConfigurationDatabaseSettings">The read hydraulic location configuration settings.</param>
        /// <param name="usePreprocessorClosure">Indicator whether to use the preprocessor closure.</param>
        /// <param name="hlcdFilePath">The file path of the hydraulic location configuration database.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicLocationConfigurationSettings"/>
        /// or <paramref name="hlcdFilePath"/> is <c>null</c>.</exception>
        public static void SetHydraulicLocationConfigurationSettings(HydraulicLocationConfigurationSettings hydraulicLocationConfigurationSettings,
                                                                     ReadHydraulicLocationConfigurationDatabaseSettings readHydraulicLocationConfigurationDatabaseSettings,
                                                                     bool usePreprocessorClosure,
                                                                     string hlcdFilePath)
        {
            if (hydraulicLocationConfigurationSettings == null)
            {
                throw new ArgumentNullException(nameof(hydraulicLocationConfigurationSettings));
            }

            if (hlcdFilePath == null)
            {
                throw new ArgumentNullException(nameof(hlcdFilePath));
            }

            if (readHydraulicLocationConfigurationDatabaseSettings != null)
            {
                hydraulicLocationConfigurationSettings.FilePath = hlcdFilePath;
                hydraulicLocationConfigurationSettings.ScenarioName = readHydraulicLocationConfigurationDatabaseSettings.ScenarioName;
                hydraulicLocationConfigurationSettings.Year = readHydraulicLocationConfigurationDatabaseSettings.Year;
                hydraulicLocationConfigurationSettings.Scope = readHydraulicLocationConfigurationDatabaseSettings.Scope;
                hydraulicLocationConfigurationSettings.SeaLevel = readHydraulicLocationConfigurationDatabaseSettings.SeaLevel;
                hydraulicLocationConfigurationSettings.RiverDischarge = readHydraulicLocationConfigurationDatabaseSettings.RiverDischarge;
                hydraulicLocationConfigurationSettings.LakeLevel = readHydraulicLocationConfigurationDatabaseSettings.LakeLevel;
                hydraulicLocationConfigurationSettings.WindDirection = readHydraulicLocationConfigurationDatabaseSettings.WindDirection;
                hydraulicLocationConfigurationSettings.WindSpeed = readHydraulicLocationConfigurationDatabaseSettings.WindSpeed;
                hydraulicLocationConfigurationSettings.Comment = readHydraulicLocationConfigurationDatabaseSettings.Comment;
                hydraulicLocationConfigurationSettings.UsePreprocessorClosure = usePreprocessorClosure;
            }
            else
            {
                log.Warn(Resources.HydraulicLocationConfigurationSettingsUpdateHelper_ReadHydraulicLocationConfigurationDatabaseSettings_No_ScenarioInformation_entries_present);

                hydraulicLocationConfigurationSettings.FilePath = hlcdFilePath;
                hydraulicLocationConfigurationSettings.ScenarioName = HydraulicLocationConfigurationSettingsConstants.MandatoryConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationSettings.Year = HydraulicLocationConfigurationSettingsConstants.YearDefaultValue;
                hydraulicLocationConfigurationSettings.Scope = HydraulicLocationConfigurationSettingsConstants.MandatoryConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationSettings.SeaLevel = HydraulicLocationConfigurationSettingsConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationSettings.RiverDischarge = HydraulicLocationConfigurationSettingsConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationSettings.LakeLevel = HydraulicLocationConfigurationSettingsConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationSettings.WindDirection = HydraulicLocationConfigurationSettingsConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationSettings.WindSpeed = HydraulicLocationConfigurationSettingsConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationSettings.Comment = HydraulicLocationConfigurationSettingsConstants.AdditionalInformationConfigurationPropertyValue;
                hydraulicLocationConfigurationSettings.UsePreprocessorClosure = usePreprocessorClosure;
            }
        }
    }
}