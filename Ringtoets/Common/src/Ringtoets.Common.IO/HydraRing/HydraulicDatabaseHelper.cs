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

using System;
using System.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabaseContext;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabaseContext;
using Ringtoets.HydraRing.IO.Properties;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// This class defines helper methods for obtaining meta data from hyraulic boundary databases.
    /// </summary>
    public static class HydraulicDatabaseHelper
    {
        private const string hydraRingConfigurationDatabaseExtension = "config.sqlite";
        private const string hlcdFileName = "HLCD.sqlite";

        /// <summary>
        /// Attempts to connect to the <paramref name="filePath"/> as if it is a Hydraulic Boundary Locations 
        /// database with a Hydraulic Location Configurations database next to it.
        /// </summary>
        /// <param name="filePath">The path of the Hydraulic Boundary Locations database file.</param>
        /// <returns>A <see cref="string"/> describing the problem when trying to connect to the <paramref name="filePath"/> 
        /// or <c>null</c> if a connection could be correctly made.</returns>
        public static string ValidatePathForCalculation(string filePath)
        {
            try
            {
                FileUtils.ValidateFilePath(filePath);
            }
            catch (ArgumentException e)
            {
                return e.Message;
            }

            string directoryName;
            try
            {
                directoryName = Path.GetDirectoryName(filePath);
            }
            catch (PathTooLongException)
            {
                return String.Format(Resources.HydraulicDatabaseHelper_ValidatePathForCalculation_Invalid_path_0_, filePath);
            }

            string settingsDatabaseFileName = GetHydraulicBoundarySettingsDatabase(filePath);
            try
            {
                using (var db = new HydraulicBoundarySqLiteDatabaseReader(filePath))
                {
                    db.GetVersion();
                }
                string hlcdFilePath = Path.Combine(directoryName, hlcdFileName);
                new HydraulicLocationConfigurationSqLiteDatabaseReader(hlcdFilePath).Dispose();
                new DesignTablesSettingsProvider(settingsDatabaseFileName).Dispose();
                new TimeIntegrationSettingsProvider(settingsDatabaseFileName).Dispose();
                new NumericsSettingsProvider(settingsDatabaseFileName).Dispose();
            }
            catch (CriticalFileReadException e)
            {
                return e.Message;
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
                throw new ArgumentNullException("database");
            }
            if (pathToDatabase == null)
            {
                throw new ArgumentNullException("pathToDatabase");
            }
            return database.Version == GetVersion(pathToDatabase);
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
            using (var db = new HydraulicBoundarySqLiteDatabaseReader(filePath))
            {
                return db.GetVersion();
            }
        }

        public static string GetHydraulicBoundarySettingsDatabase(string hydraulicBoundaryDatabaseFilePath)
        {
            return Path.ChangeExtension(hydraulicBoundaryDatabaseFilePath, hydraRingConfigurationDatabaseExtension);
        }
    }
}