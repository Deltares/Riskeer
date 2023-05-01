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
    /// Helper class for updating <see cref="HydraulicLocationConfigurationDatabase"/> instances.
    /// </summary>
    public static class HydraulicLocationConfigurationDatabaseUpdateHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicLocationConfigurationDatabaseUpdateHelper));

        /// <summary>
        /// Updates the hydraulic location configuration database.
        /// </summary>
        /// <param name="hydraulicLocationConfigurationDatabase">The hydraulic location configuration database to update.</param>
        /// <param name="readHydraulicLocationConfigurationSettings">The read hydraulic location configuration settings.</param>
        /// <param name="hlcdFilePath">The file path of the hydraulic location configuration database.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicLocationConfigurationDatabase"/> or
        /// <paramref name="hlcdFilePath"/> is <c>null</c>.</exception>
        public static void UpdateHydraulicLocationConfigurationDatabase(HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase,
                                                                        ReadHydraulicLocationConfigurationSettings readHydraulicLocationConfigurationSettings,
                                                                        string hlcdFilePath)
        {
            if (hydraulicLocationConfigurationDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicLocationConfigurationDatabase));
            }

            if (hlcdFilePath == null)
            {
                throw new ArgumentNullException(nameof(hlcdFilePath));
            }

            if (readHydraulicLocationConfigurationSettings != null)
            {
                hydraulicLocationConfigurationDatabase.FilePath = hlcdFilePath;
                hydraulicLocationConfigurationDatabase.ScenarioName = readHydraulicLocationConfigurationSettings.ScenarioName;
                hydraulicLocationConfigurationDatabase.Year = readHydraulicLocationConfigurationSettings.Year;
                hydraulicLocationConfigurationDatabase.Scope = readHydraulicLocationConfigurationSettings.Scope;
                hydraulicLocationConfigurationDatabase.SeaLevel = readHydraulicLocationConfigurationSettings.SeaLevel;
                hydraulicLocationConfigurationDatabase.RiverDischarge = readHydraulicLocationConfigurationSettings.RiverDischarge;
                hydraulicLocationConfigurationDatabase.LakeLevel = readHydraulicLocationConfigurationSettings.LakeLevel;
                hydraulicLocationConfigurationDatabase.WindDirection = readHydraulicLocationConfigurationSettings.WindDirection;
                hydraulicLocationConfigurationDatabase.WindSpeed = readHydraulicLocationConfigurationSettings.WindSpeed;
                hydraulicLocationConfigurationDatabase.Comment = readHydraulicLocationConfigurationSettings.Comment;
            }
            else
            {
                log.Warn(Resources.HydraulicLocationConfigurationDatabaseUpdateHelper_ReadHydraulicLocationConfigurationDatabase_No_ScenarioInformation_entries_present);

                hydraulicLocationConfigurationDatabase.FilePath = hlcdFilePath;
                hydraulicLocationConfigurationDatabase.ScenarioName = HydraulicLocationConfigurationDatabaseConstants.MandatoryConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationDatabase.Year = HydraulicLocationConfigurationDatabaseConstants.YearDefaultValue;
                hydraulicLocationConfigurationDatabase.Scope = HydraulicLocationConfigurationDatabaseConstants.MandatoryConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationDatabase.SeaLevel = HydraulicLocationConfigurationDatabaseConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationDatabase.RiverDischarge = HydraulicLocationConfigurationDatabaseConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationDatabase.LakeLevel = HydraulicLocationConfigurationDatabaseConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationDatabase.WindDirection = HydraulicLocationConfigurationDatabaseConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationDatabase.WindSpeed = HydraulicLocationConfigurationDatabaseConstants.OptionalConfigurationPropertyDefaultValue;
                hydraulicLocationConfigurationDatabase.Comment = HydraulicLocationConfigurationDatabaseConstants.AdditionalInformationConfigurationPropertyValue;
            }
        }
    }
}