// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Data.SQLite;
using System.IO;
using Core.Common.TestUtil;
using Migration.Core.Storage;
using Migration.Scripts.Data.Exceptions;
using NUnit.Framework;
using Ringtoets.Migration.Core.TestUtil;

namespace Ringtoets.Migration.Core.Test
{
    [TestFixture]
    public class RingtoetsSqLiteDatabaseFileMigratorTest
    {
        private static TestCaseData[] ValidFromVersions
        {
            get
            {
                return new[]
                {
                    new TestCaseData("5"),
                    new TestCaseData("17.1"),
                    new TestCaseData("17.2"),
                    new TestCaseData("17.3")
                };
            }
        }

        [Test]
        public void Constructor_ReturnsExpectedValues()
        {
            // Call
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Assert
            Assert.IsInstanceOf<VersionedFileMigrator>(migrator);
        }

        [Test]
        [TestCaseSource(nameof(ValidFromVersions))]
        public void IsVersionSupported_SupportedVersion_ReturnsTrue(string fromVersion)
        {
            // Setup
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            bool isSupported = migrator.IsVersionSupported(fromVersion);

            // Assert
            Assert.IsTrue(isSupported);
        }

        [Test]
        [TestCase("16.4")]
        [TestCase("18.2")]
        public void IsVersionSupported_UnsupportedVersion_ReturnsFalse(string fromVersion)
        {
            // Setup
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            bool isSupported = migrator.IsVersionSupported(fromVersion);

            // Assert
            Assert.IsFalse(isSupported);
        }

        [Test]
        public void NeedsMigrate_NeedsMigrate_ReturnsTrue()
        {
            // Setup
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var versionedFile = new RingtoetsVersionedFile(sourceFilePath);
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            bool needsMigrate = migrator.NeedsMigrate(versionedFile, "17.1");

            // Assert
            Assert.IsTrue(needsMigrate);
        }

        [Test]
        public void NeedsMigrate_DoesNotNeedMigration_ReturnsFalse()
        {
            // Setup
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var versionedFile = new RingtoetsVersionedFile(sourceFilePath);
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            bool needsMigrate = migrator.NeedsMigrate(versionedFile, "4");

            // Assert
            Assert.IsFalse(needsMigrate);
        }

        [Test]
        public void Migrate_ValidFilesWithoutLogFile_SavesFileAtNewLocation()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Migrate_ValidFilesWithoutLogFile_SavesFileAtNewLocation));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            using (new FileDisposeHelper(targetFilePath))
            {
                // Call
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Assert
                var toVersionedFile = new RingtoetsVersionedFile(targetFilePath);
                Assert.AreEqual(newVersion, toVersionedFile.GetVersion());
            }
        }

        [Test]
        public void Migrate_ValidFilesWithLogFile_SavesFileAtNewLocation()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Migrate_ValidFilesWithLogFile_SavesFileAtNewLocation));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Migrate_ValidFilesWithLogFile_SavesFileAtNewLocation), ".log"));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(logFilePath))
            using (new FileDisposeHelper(targetFilePath))
            {
                // Call
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Assert
                var toVersionedFile = new RingtoetsVersionedFile(targetFilePath);
                Assert.AreEqual(newVersion, toVersionedFile.GetVersion());
            }
        }

        [Test]
        public void Migrate_ValidFilesWithNonExistingLogFile_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Migrate_ValidFilesWithNonExistingLogFile_ThrowsCriticalDatabaseMigrationException));
            string logFilePath = TestHelper.GetScratchPadPath(string.Concat(nameof(Migrate_ValidFilesWithNonExistingLogFile_ThrowsCriticalDatabaseMigrationException), ".log"));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator
            {
                LogPath = logFilePath
            };

            using (new FileDisposeHelper(targetFilePath))
            {
                // Call
                TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Assert
                var exception = Assert.Throws<CriticalMigrationException>(call);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void Migrate_TargetFileInUse_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Migrate_TargetFileInUse_ThrowsCriticalDatabaseMigrationException));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            using (var fileDisposeHelper = new FileDisposeHelper(targetFilePath))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Assert
                var exception = Assert.Throws<CriticalMigrationException>(call);
                StringAssert.StartsWith("Het gemigreerde projectbestand is aangemaakt op '", exception.Message);
                StringAssert.EndsWith($"', maar er is een onverwachte fout opgetreden tijdens het verplaatsen naar '{targetFilePath}'.",
                                      exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void Migrate_TargetFilePathEqualsSourcePath_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, sourceFilePath);

            // Assert
            var exception = Assert.Throws<CriticalMigrationException>(call);
            Assert.AreEqual("Het doelprojectpad moet anders zijn dan het bronprojectpad.",
                            exception.Message);
        }

        [Test]
        public void Migrate_TargetFileNotWritable_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Migrate_TargetFileNotWritable_ThrowsCriticalDatabaseMigrationException));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            using (new FileDisposeHelper(targetFilePath))
            {
                FileAttributes attributes = File.GetAttributes(targetFilePath);
                File.SetAttributes(targetFilePath, attributes | FileAttributes.ReadOnly);

                try
                {
                    // Call
                    TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                    // Assert
                    var exception = Assert.Throws<CriticalMigrationException>(call);
                    StringAssert.StartsWith("Het gemigreerde projectbestand is aangemaakt op '",
                                            exception.Message);
                    StringAssert.EndsWith($"', maar er is een onverwachte fout opgetreden tijdens het verplaatsen naar '{targetFilePath}'.",
                                          exception.Message);
                    Assert.IsInstanceOf<UnauthorizedAccessException>(exception.InnerException);
                }
                finally
                {
                    File.SetAttributes(targetFilePath, attributes);
                }
            }
        }

        [Test]
        public void Migrate_InvalidToVersion_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            const string newVersion = "6";
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedSupportedProjectFilePath();
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Migrate_InvalidToVersion_ThrowsCriticalDatabaseMigrationException));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

            // Assert
            string message = Assert.Throws<CriticalMigrationException>(call).Message;
            Assert.AreEqual($"Het migreren van een projectbestand met versie '5' naar versie '{newVersion}' is niet ondersteund.", message);
        }

        [Test]
        public void Migrate_UnsupportedVersion_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            string sourceFilePath = RingtoetsProjectMigrationTestHelper.GetOutdatedUnSupportedProjectFilePath();
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetScratchPadPath(nameof(Migrate_UnsupportedVersion_ThrowsCriticalDatabaseMigrationException));
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            TestDelegate call = () => migrator.Migrate(fromVersionedFile, "17.1", targetFilePath);

            // Assert
            string message = Assert.Throws<CriticalMigrationException>(call).Message;
            Assert.AreEqual("Het migreren van een projectbestand met versie '8' naar versie '17.1' is niet ondersteund.", message);
        }
    }
}