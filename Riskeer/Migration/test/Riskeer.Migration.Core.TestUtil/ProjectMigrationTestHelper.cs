﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using Core.Common.TestUtil;
using Riskeer.Common.Util;

namespace Riskeer.Migration.Core.TestUtil
{
    /// <summary>
    /// Class which provides file paths of the project files that can 
    /// be used for testing migration functionality.
    /// </summary>
    public static class ProjectMigrationTestHelper
    {
        private static readonly TestDataPath testDataPath = TestDataPath.Riskeer.Migration.Core;

        /// <summary>
        /// Retrieves the file path of a project with the latest database version
        /// format that can be opened in the application.
        /// </summary>
        /// <returns>A file path to a project file with the latest database version.</returns>
        public static string GetLatestProjectFilePath()
        {
            string currentDatabaseVersion = ProjectVersionHelper.GetCurrentDatabaseVersion();
            string versionSuffix = currentDatabaseVersion.Replace(".", string.Empty);
            string projectFileName = $"MigrationTestProject{versionSuffix}.risk";

            return TestHelper.GetTestDataPath(testDataPath, projectFileName);
        }

        /// <summary>
        /// Retrieves the file path of a project with an older database version,
        /// which is supported for migration and can be opened in the application.
        /// </summary>
        /// <returns>A file path to an outdated project which is supported 
        /// for migration.</returns>
        public static string GetOutdatedSupportedProjectFilePath()
        {
            const string projectFileName = "MigrationTestProject164.rtd";
            return TestHelper.GetTestDataPath(testDataPath, projectFileName);
        }

        /// <summary>
        /// Retrieves the file path of a project with an unsupported database version
        /// which is unsupported for migration.
        /// </summary>
        /// <returns>A file path to an outdated project which is unsupported for 
        /// migration.</returns>
        public static string GetOutdatedUnSupportedProjectFilePath()
        {
            const string projectFileName = "UnsupportedVersion8.rtd";
            return TestHelper.GetTestDataPath(testDataPath, projectFileName);
        }
    }
}