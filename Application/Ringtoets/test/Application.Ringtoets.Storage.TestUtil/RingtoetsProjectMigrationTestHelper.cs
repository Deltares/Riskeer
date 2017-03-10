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
using Core.Common.TestUtil;
using Ringtoets.Common.Utils;

namespace Application.Ringtoets.Storage.TestUtil
{
    /// <summary>
    /// Class which provides file paths of the Ringtoets project files that can 
    /// be used unit testing migration functionality.
    /// </summary>
    public static class RingtoetsProjectMigrationTestHelper
    {
        private static readonly TestDataPath testDataPath = TestDataPath.Application.Ringtoets.Migration;

        /// <summary>
        /// Retrieves the file path of a Ringtoets project with the latest database version
        /// format.
        /// </summary>
        /// <returns>A file path to a Ringtoets project file with the latest database version.</returns>
        public static string GetLatestProjectFilePath()
        {
            string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            string versionSuffix = currentDatabaseVersion.Replace(".", string.Empty);
            string projectFileName = $"FullTestProject{versionSuffix}.rtd";

            return TestHelper.GetTestDataPath(testDataPath, projectFileName);
        }

        /// <summary>
        /// Retrieves the file path of a Ringtoets project with an older database version,
        /// which is supported for migration.
        /// </summary>
        /// <returns>A file path to an outdated Ringtoets project which is supported 
        /// for migration.</returns>
        public static string GetOutdatedSupportedProjectFilePath()
        {
            const string projectFileName = "FullTestProject164.rtd";
            return TestHelper.GetTestDataPath(testDataPath, projectFileName);
        }

        /// <summary>
        /// Retrieves the file path of all Ringtoets projects with an older database version,
        /// which is supported for the migration.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of all the file paths to supported
        /// outdated Ringtoets projects.</returns>
        public static IEnumerable<string> GetAllOutdatedSupportedProjectFilePaths()
        {
            const string projectFileName = "FullTestProject164.rtd";
            return new[]
            {
                TestHelper.GetTestDataPath(testDataPath, projectFileName)
            };
        }

        /// <summary>
        /// Retrieves the file path of a Ringtoets project with an unsupported database version
        /// which is unsupported for migration.
        /// </summary>
        /// <returns>A file path to an outdated Ringtoets project which is unsupported for 
        /// migration.</returns>
        public static string GetOutdatedUnSupportedProjectFilePath()
        {
            const string projectFileName = "UnsupportedVersion8.rtd";
            return TestHelper.GetTestDataPath(testDataPath, projectFileName);
        }
    }
}