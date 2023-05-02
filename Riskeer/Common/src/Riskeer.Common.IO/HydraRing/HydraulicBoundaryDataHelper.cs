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
using System.Globalization;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Riskeer.Common.IO.Properties;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.Common.IO.HydraRing
{
    /// <summary>
    /// This class defines helper methods for handling and using hydraulic boundary data.
    /// </summary>
    public static class HydraulicBoundaryDataHelper
    {
        private const string hydraulicBoundarySettingsDatabaseExtension = "config.sqlite";
        private const string preprocessorClosureFileName = "preprocClosure.sqlite";

        /// <summary>
        /// Attempts to connect to the <paramref name="hrdFilePath"/> as if it is a hydraulic boundary database with a hydraulic
        /// location configuration database and hydraulic boundary settings database next to it.
        /// </summary>
        /// <param name="hlcdFilePath">The file path of the hydraulic location configuration database.</param>
        /// <param name="hrdFilePath">The file path of the hydraulic boundary database.</param>
        /// <param name="usePreprocessorClosure">Indicator whether the preprocessor closure is used in a calculation.</param>
        /// <returns>A <see cref="string"/> describing the problem when trying to connect to the <paramref name="hrdFilePath"/> 
        /// or <c>null</c> if a connection could be correctly made.</returns>
        public static string ValidateFilesForCalculation(string hlcdFilePath, string hrdFilePath, bool usePreprocessorClosure)
        {
            try
            {
                IOUtils.ValidateFilePath(hrdFilePath);
            }
            catch (ArgumentException e)
            {
                return e.Message;
            }

            try
            {
                Path.GetDirectoryName(hrdFilePath);
            }
            catch (PathTooLongException)
            {
                return string.Format(CultureInfo.CurrentCulture, Resources.HydraulicBoundaryDataHelper_ValidatePathForCalculation_Invalid_path_0_,
                                     hrdFilePath);
            }

            try
            {
                using (new HydraulicBoundaryDatabaseReader(hrdFilePath))
                {
                    // Used on purpose to check the hydraulic boundary database file
                }

                using (new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
                {
                    // Used on purpose to check the hydraulic location configuration database file
                }

                using (var validator = new HydraRingSettingsDatabaseValidator(GetHydraulicBoundarySettingsDatabaseFilePath(hrdFilePath)))
                {
                    if (!validator.ValidateSchema())
                    {
                        return Resources.HydraRingSettingsDatabase_Hydraulic_calculation_settings_database_has_invalid_schema;
                    }
                }
            }
            catch (CriticalFileReadException e)
            {
                return e.Message;
            }

            if (usePreprocessorClosure)
            {
                string preprocessorClosureFilePath = GetPreprocessorClosureFilePath(hlcdFilePath);
                if (!File.Exists(preprocessorClosureFilePath))
                {
                    return new FileReaderErrorMessageBuilder(preprocessorClosureFilePath).Build(CoreCommonUtilResources.Error_File_does_not_exist);
                }
            }

            return null;
        }

        /// <summary>
        /// Checks whether the provided version string matches the version string in the provided hydraulic boundary database file.
        /// <param name="version">The version to check for.</param>
        /// <param name="hrdFilePath">The path of the hydraulic boundary database to check.</param>
        /// <returns><c>true</c> if the provided version string matches the version string in the provided hydraulic boundary
        /// database file, <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when no connection could be created with the hydraulic boundary
        /// database.</exception>
        /// </summary>
        public static bool IsCorrectVersion(string version, string hrdFilePath)
        {
            return version == GetVersion(hrdFilePath);
        }

        /// <summary>
        /// Gets the file path of the hydraulic boundary settings database.
        /// </summary>
        /// <param name="hrdFilePath">The file path of the corresponding hydraulic boundary database.</param>
        /// <returns>The file path of the hydraulic boundary settings database.</returns>
        public static string GetHydraulicBoundarySettingsDatabaseFilePath(string hrdFilePath)
        {
            return Path.ChangeExtension(hrdFilePath, hydraulicBoundarySettingsDatabaseExtension);
        }

        private static string GetPreprocessorClosureFilePath(string hlcdFilePath)
        {
            string directory = Path.GetDirectoryName(hlcdFilePath);
            string hlcdFileName = Path.GetFileNameWithoutExtension(hlcdFilePath);
            return Path.Combine(directory, $"{hlcdFileName}_{preprocessorClosureFileName}");
        }

        /// <summary>
        /// Returns the version of the hydraulic boundary database pointed at by the <paramref name="hrdFilePath"/>.
        /// </summary>
        /// <param name="hrdFilePath">The file path of the hydraulic boundary database.</param>
        /// <returns>The version of the hydraulic boundary database as a <see cref="string"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when no connection with the hydraulic boundary database could
        /// be created.</exception>
        private static string GetVersion(string hrdFilePath)
        {
            using (var db = new HydraulicBoundaryDatabaseReader(hrdFilePath))
            {
                return db.ReadVersion();
            }
        }
    }
}