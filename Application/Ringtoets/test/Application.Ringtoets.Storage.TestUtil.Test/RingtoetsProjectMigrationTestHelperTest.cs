﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.IO;
using System.Linq;
using Application.Ringtoets.Migration.Core;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.Common.Util;

namespace Application.Ringtoets.Storage.TestUtil.Test
{
    [TestFixture]
    public class RingtoetsProjectMigrationTestHelperTest
    {
        private readonly string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

        [Test]
        public void GetLatestProjectFilePath_Always_ReturnsProjectFilePathToLatestProjectVersion()
        {
            // Call
            string latestProjectfilePath = RingtoetsProjectMigrationTestHelper.GetLatestProjectFilePath();

            // Assert
            AssertFilePath(latestProjectfilePath);

            var versionedFile = new RingtoetsVersionedFile(latestProjectfilePath);
            string actualTestProjectVersion = versionedFile.GetVersion();
            string assertionMessage = $"Database version {actualTestProjectVersion} of the testproject must match with the current database version {currentDatabaseVersion}.";
            Assert.AreEqual(currentDatabaseVersion, actualTestProjectVersion, assertionMessage);
        }

        [Test]
        public void GetLatestProjectFilePath_Always_ReturnsProjectThatCanBeLoaded()
        {
            // Setup
            string latestProjectfilePath = RingtoetsProjectMigrationTestHelper.GetLatestProjectFilePath();

            // Call
            IProject project = new StorageSqLite().LoadProject(latestProjectfilePath);

            // Assert
            Assert.IsNotNull(project);
        }

        [Test]
        public void GetOutdatedSupportedProjectFilePath_Always_ReturnsProjectFilePathToSupportedProjectVersion()
        {
            // Call
            string projectFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();

            // Assert
            AssertFilePath(projectFilePath);

            var versionedFile = new RingtoetsVersionedFile(projectFilePath);
            string actualTestProjectVersion = versionedFile.GetVersion();
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();
            Assert.IsTrue(migrator.IsVersionSupported(actualTestProjectVersion));
        }

        [Test]
        public void GetAllOutdatedSupportedProjectFileVersions_AlwaysReturnsAllProjectFilePathsToSupportedProjectVersions()
        {
            // Call
            string[] versions = RingtoetsProjectMigrationTestHelper.GetAllOutdatedSupportedProjectFileVersions().ToArray();

            // Assert
            const int expectedNrOfVersions = 2;
            Assert.AreEqual(expectedNrOfVersions, versions.Length);

            IEnumerable<string> expectedProjectVersions = new[]
            {
                "5",
                "17.1"
            };
            List<string> returnedProjectVersions = versions.Select(v => new RingtoetsVersionedFile(TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration.Core,
                                                                                                                              $"FullTestProject{v}.rtd")).GetVersion()).ToList();
            CollectionAssert.AreEqual(expectedProjectVersions, returnedProjectVersions);
        }

        [Test]
        public void GetOutdatedUnsupportedProjectFilePath_Always_ReturnsProjectFilePathToUnsupportedProjectVersion()
        {
            // Call
            string projectFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedUnSupportedProjectFilePath();

            // Assert
            AssertFilePath(projectFilePath);

            var versionedFile = new RingtoetsVersionedFile(projectFilePath);
            string actualTestProjectVersion = versionedFile.GetVersion();
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();
            Assert.IsFalse(migrator.IsVersionSupported(actualTestProjectVersion));
        }

        [Test]
        public void AssertMigrationScript_IsEqualToStorageScript()
        {
            // Setup
            string solutionRoot = TestHelper.SolutionRoot;
            const string baseFileName = "DatabaseStructure";
            string migrationFileName = $"{baseFileName}{currentDatabaseVersion}";

            string applicationScriptFilePath = Path.Combine(solutionRoot, "build", $"{baseFileName}.sql");
            string migrationScriptFilePath = Path.Combine(solutionRoot, "Application", "Ringtoets", "src", "Application.Ringtoets.Migration.Core", "EmbeddedResources", $"{migrationFileName}.sql");

            // Precondition
            AssertFilePath(applicationScriptFilePath);
            AssertFilePath(migrationScriptFilePath);

            // Call
            string[] applicationScriptContents = File.ReadAllLines(applicationScriptFilePath);
            string[] migrationScriptContents = File.ReadAllLines(migrationScriptFilePath);

            // Assert
            int expectedAmountOfLines = applicationScriptContents.Length;
            const string assertionMessage = "Application and migration SQL scripts do not have the same length.";
            Assert.AreEqual(expectedAmountOfLines, migrationScriptContents.Length, assertionMessage);

            for (var i = 0; i < expectedAmountOfLines; i++)
            {
                Assert.AreEqual(applicationScriptContents[i],
                                migrationScriptContents[i],
                                $"Mismatch between application and migration SQL scripts detected at line: {i + 1}.");
            }
        }

        private static void AssertFilePath(string filePath)
        {
            Assert.IsTrue(IOUtils.IsValidFilePath(filePath));
            Assert.IsTrue(File.Exists(filePath));
        }
    }
}