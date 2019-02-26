// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.Properties;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using HydraRingResources = Riskeer.HydraRing.IO.Properties.Resources;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.Common.IO.HydraRing
{
    /// <summary>
    /// This class defines helper methods for obtaining meta data from hydraulic boundary databases.
    /// </summary>
    public static class HydraulicBoundaryDatabaseHelper
    {
        private const string hydraRingConfigurationDatabaseExtension = "config.sqlite";
        private const string preprocessorClosureFileName = "preprocClosure.sqlite";

        /// <summary>
        /// Attempts to connect to the <paramref name="filePath"/> as if it is a Hydraulic Boundary Locations 
        /// database with a Hydraulic Location Configurations database and settings next to it.
        /// </summary>
        /// <param name="filePath">The path of the Hydraulic Boundary Locations database file.</param>
        /// <param name="hlcdFilePath">The path of the Hydraulic Location Configuration database file.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="usePreprocessorClosure">Indicator whether the preprocessor closure is used in a calculation.</param>
        /// <returns>A <see cref="string"/> describing the problem when trying to connect to the <paramref name="filePath"/> 
        /// or <c>null</c> if a connection could be correctly made.</returns>
        public static string ValidateFilesForCalculation(string filePath, string hlcdFilePath, string preprocessorDirectory, bool usePreprocessorClosure)
        {
            try
            {
                IOUtils.ValidateFilePath(filePath);
            }
            catch (ArgumentException e)
            {
                return e.Message;
            }

            try
            {
                Path.GetDirectoryName(filePath);
            }
            catch (PathTooLongException)
            {
                return string.Format(CultureInfo.CurrentCulture, HydraRingResources.HydraulicBoundaryDatabaseHelper_ValidatePathForCalculation_Invalid_path_0_,
                                     filePath);
            }

            string settingsDatabaseFileName = GetHydraulicBoundarySettingsDatabase(filePath);
            try
            {
                using (new HydraulicBoundaryDatabaseReader(filePath)) {}

                using (new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath)) {}

                using (var validator = new HydraRingSettingsDatabaseValidator(settingsDatabaseFileName, preprocessorDirectory))
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
                string directory = Path.GetDirectoryName(hlcdFilePath);
                string hlcdFileName = Path.GetFileNameWithoutExtension(hlcdFilePath);
                string preprocessorClosureFilePath = Path.Combine(directory, $"{hlcdFileName}_{preprocessorClosureFileName}");

                if (!File.Exists(preprocessorClosureFilePath))
                {
                    return new FileReaderErrorMessageBuilder(preprocessorClosureFilePath).Build(CoreCommonUtilResources.Error_File_does_not_exist);
                }
            }

            return null;
        }

        /// <summary>
        /// Checks whether the version of a <see cref="HydraulicBoundaryDatabase"/> matches the version
        /// of a database at the given <see cref="pathToDatabase"/>.
        /// </summary>
        /// <param name="database">The database to compare the version of.</param>
        /// <param name="pathToDatabase">The path to the database to compare the version of.</param>
        /// <returns><c>true</c> if <paramref name="database"/> equals the version of the database at
        /// <paramref name="pathToDatabase"/>, <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when no connection with the hydraulic 
        /// boundary database could be created using <paramref name="pathToDatabase"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="database"/> is <c>null</c></item>
        /// <item><paramref name="pathToDatabase"/> is <c>null</c></item>
        /// </list></exception>
        public static bool HaveEqualVersion(HydraulicBoundaryDatabase database, string pathToDatabase)
        {
            if (database == null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (pathToDatabase == null)
            {
                throw new ArgumentNullException(nameof(pathToDatabase));
            }

            return database.Version == GetVersion(pathToDatabase);
        }

        /// <summary>
        /// Gets the file path of the hydraulic boundary database settings file.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the corresponding hydraulic boundary database file.</param>
        /// <returns>The file path of the hydraulic boundary settings database file.</returns>
        public static string GetHydraulicBoundarySettingsDatabase(string hydraulicBoundaryDatabaseFilePath)
        {
            return Path.ChangeExtension(hydraulicBoundaryDatabaseFilePath, hydraRingConfigurationDatabaseExtension);
        }

        /// <summary>
        /// Checks <paramref name="preprocessorDirectory"/> for being a valid folder path.
        /// </summary>
        /// <param name="preprocessorDirectory">The preprocessor directory to validate.</param>
        /// <returns>A <see cref="string"/> describing the problem with <paramref name="preprocessorDirectory"/> 
        /// or <c>null</c> when <paramref name="preprocessorDirectory"/> is a valid directory path.</returns>
        public static string ValidatePreprocessorDirectory(string preprocessorDirectory)
        {
            if (preprocessorDirectory != string.Empty)
            {
                try
                {
                    IOUtils.GetFullPath(preprocessorDirectory);
                }
                catch (ArgumentException exception)
                {
                    return $"{Resources.HydraulicBoundaryDatabaseHelper_ValidatePreprocessorDirectory_Invalid_path} {exception.Message}";
                }

                if (!Directory.Exists(preprocessorDirectory))
                {
                    return $"{Resources.HydraulicBoundaryDatabaseHelper_ValidatePreprocessorDirectory_Invalid_path} {Resources.HydraulicBoundaryDatabaseHelper_ValidatePreprocessorDirectory_Path_does_not_exist}";
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the version from the database pointed at by the <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The location of the database.</param>
        /// <returns>The version from the database as a <see cref="string"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when no connection with the hydraulic 
        /// boundary database could be created.</exception>
        private static string GetVersion(string filePath)
        {
            using (var db = new HydraulicBoundaryDatabaseReader(filePath))
            {
                return db.ReadVersion();
            }
        }
    }
}