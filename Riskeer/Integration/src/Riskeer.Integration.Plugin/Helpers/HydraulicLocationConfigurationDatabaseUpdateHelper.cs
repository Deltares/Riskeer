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
        /// <param name="readHydraulicLocationConfigurationDatabase">The read hydraulic location configuration database.</param>
        /// <param name="usePreprocessorClosure">Indicator whether to use the preprocessor closure.</param>
        /// <param name="hlcdFilePath">The file path of the hydraulic location configuration database.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicLocationConfigurationDatabase"/> or
        /// <paramref name="hlcdFilePath"/> is <c>null</c>.</exception>
        public static void UpdateHydraulicLocationConfigurationDatabase(HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase,
                                                                     ReadHydraulicLocationConfigurationDatabaseSettings readHydraulicLocationConfigurationDatabase,
                                                                     bool usePreprocessorClosure,
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

            if (readHydraulicLocationConfigurationDatabase != null)
            {
                hydraulicLocationConfigurationDatabase.FilePath = hlcdFilePath;
                hydraulicLocationConfigurationDatabase.ScenarioName = readHydraulicLocationConfigurationDatabase.ScenarioName;
                hydraulicLocationConfigurationDatabase.Year = readHydraulicLocationConfigurationDatabase.Year;
                hydraulicLocationConfigurationDatabase.Scope = readHydraulicLocationConfigurationDatabase.Scope;
                hydraulicLocationConfigurationDatabase.SeaLevel = readHydraulicLocationConfigurationDatabase.SeaLevel;
                hydraulicLocationConfigurationDatabase.RiverDischarge = readHydraulicLocationConfigurationDatabase.RiverDischarge;
                hydraulicLocationConfigurationDatabase.LakeLevel = readHydraulicLocationConfigurationDatabase.LakeLevel;
                hydraulicLocationConfigurationDatabase.WindDirection = readHydraulicLocationConfigurationDatabase.WindDirection;
                hydraulicLocationConfigurationDatabase.WindSpeed = readHydraulicLocationConfigurationDatabase.WindSpeed;
                hydraulicLocationConfigurationDatabase.Comment = readHydraulicLocationConfigurationDatabase.Comment;
                hydraulicLocationConfigurationDatabase.UsePreprocessorClosure = usePreprocessorClosure;
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
                hydraulicLocationConfigurationDatabase.UsePreprocessorClosure = usePreprocessorClosure;
            }
        }
    }
}