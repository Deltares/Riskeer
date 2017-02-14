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
using Core.Common.TestUtil;
using Migration.Core.Storage;
using Migration.Scripts.Data.Exceptions;
using NUnit.Framework;

namespace Application.Ringtoets.Migration.Test
{
    [TestFixture]
    public class RingtoetsSqLiteDatabaseFileMigratorTest
    {
        [Test]
        public void Constructor_ReturnsExpectedValues()
        {
            // Call
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Assert
            Assert.IsInstanceOf<VersionedFileMigrator>(migrator);
        }

        [Test]
        public void IsVersionSupported_SupportedVersion_ReturnsTrue()
        {
            // Setup
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            bool isSupported = migrator.IsVersionSupported("5");

            // Assert
            Assert.IsTrue(isSupported);
        }

        [Test]
        [TestCase("16.4")]
        [TestCase("17.1")]
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
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
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
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            var versionedFile = new RingtoetsVersionedFile(sourceFilePath);
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            bool needsMigrate = migrator.NeedsMigrate(versionedFile, "4");

            // Assert
            Assert.IsFalse(needsMigrate);
        }

        [Test]
        public void Migrate_ValidFiles_SavesFileAtNewLocation()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, Path.GetRandomFileName());
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
        public void Migrate_TargetFileInUse_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, Path.GetRandomFileName());
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            using (var fileDisposeHelper = new FileDisposeHelper(targetFilePath))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Assert
                CriticalMigrationException exception = Assert.Throws<CriticalMigrationException>(call);
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
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, sourceFilePath);

            // Assert
            CriticalMigrationException exception = Assert.Throws<CriticalMigrationException>(call);
            Assert.AreEqual("Het doelprojectpad moet anders zijn dan het bronprojectpad.",
                            exception.Message);
        }

        [Test]
        public void Migrate_TargetFileNotWritable_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, Path.GetRandomFileName());
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
                    CriticalMigrationException exception = Assert.Throws<CriticalMigrationException>(call);
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
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "FullTestProject164.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, Path.GetRandomFileName());
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
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, "UnsupportedVersion8.rtd");
            var fromVersionedFile = new RingtoetsVersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, Path.GetRandomFileName());
            var migrator = new RingtoetsSqLiteDatabaseFileMigrator();

            // Call
            TestDelegate call = () => migrator.Migrate(fromVersionedFile, "17.1", targetFilePath);

            // Assert
            string message = Assert.Throws<CriticalMigrationException>(call).Message;
            Assert.AreEqual("Het migreren van een projectbestand met versie '8' naar versie '17.1' is niet ondersteund.", message);
        }
    }
}