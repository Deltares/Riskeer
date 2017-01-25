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

using System.IO;
using Core.Common.TestUtil;
using Migration.Scripts.Data;
using Migration.Scripts.Data.Exceptions;
using NUnit.Framework;

namespace Migration.Core.Storage.Test
{
    [TestFixture]
    public class VersionedFileMigratorTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void IsVersionSupported_ToVersionIsNullOrWhiteSpace_ReturnsFalse(string toVersion)
        {
            // Setup
            var migrator = new VersionedFileMigrator();

            // Call
            bool isSupported = migrator.IsVersionSupported(toVersion);

            // Assert
            Assert.IsFalse(isSupported);
        }

        [Test]
        public void IsVersionSupported_SupportedVersion_ReturnsTrue()
        {
            // Setup
            var migrator = new VersionedFileMigrator();

            // Call
            bool isSupported = migrator.IsVersionSupported("4");

            // Assert
            Assert.IsTrue(isSupported);
        }

        [Test]
        [TestCase("16.4")]
        [TestCase("17.1")]
        public void IsVersionSupported_UnsupportedVersion_ReturnsFalse(string fromVersion)
        {
            // Setup
            var migrator = new VersionedFileMigrator();

            // Call
            bool isSupported = migrator.IsVersionSupported(fromVersion);

            // Assert
            Assert.IsFalse(isSupported);
        }

        [Test]
        public void NeedsMigrade_NeedsMigrade_ReturnsTrue()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, "Demo164.rtd");
            VersionedFile versionedFile = new VersionedFile(sourceFilePath);
            var migrator = new VersionedFileMigrator();

            // Call
            bool needsMigrade = migrator.NeedsMigrade(versionedFile, "17.1");

            // Assert
            Assert.IsTrue(needsMigrade);
        }

        [Test]
        public void NeedsMigrade_DoesNotNeedMigration_ReturnsFalse()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, "Demo164.rtd");
            VersionedFile versionedFile = new VersionedFile(sourceFilePath);
            var migrator = new VersionedFileMigrator();

            // Call
            bool needsMigrade = migrator.NeedsMigrade(versionedFile, "4");

            // Assert
            Assert.IsFalse(needsMigrade);
        }

        [Test]
        public void Migrate_ValidFiles_SavesFileAtNewLocation()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, "Demo164.rtd");
            var fromVersionedFile = new VersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, Path.GetRandomFileName());
            var migrator = new VersionedFileMigrator();

            using (new FileDisposeHelper(targetFilePath))
            {
                // Call
                migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Assert
                var toVersionedFile = new VersionedFile(targetFilePath);
                Assert.AreEqual(newVersion, toVersionedFile.GetVersion());
            }
        }

        [Test]
        public void Migrate_TargetFileInUse_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            const string newVersion = "17.1";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, "Demo164.rtd");
            var fromVersionedFile = new VersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, Path.GetRandomFileName());
            var migrator = new VersionedFileMigrator();

            using (new FileDisposeHelper(targetFilePath))
            using (File.Create(targetFilePath))
            {
                // Call
                TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

                // Assert
                string message = Assert.Throws<CriticalDatabaseMigrationException>(call).Message;
                Assert.That(message.StartsWith("Er is een onverwachte fout opgetreden tijdens het verplaatsen van het gemigreerde bestand '"));
                Assert.That(message.EndsWith($"' naar '{targetFilePath}'."));
            }
        }

        [Test]
        public void Migrate_InvalidToVersion_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            const string newVersion = "6";
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, "Demo164.rtd");
            var fromVersionedFile = new VersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, Path.GetRandomFileName());
            var migrator = new VersionedFileMigrator();

            // Call
            TestDelegate call = () => migrator.Migrate(fromVersionedFile, newVersion, targetFilePath);

            // Assert
            string message = Assert.Throws<CriticalDatabaseMigrationException>(call).Message;
            Assert.AreEqual($"Het is niet mogelijk om versie 4 te migreren naar versie {newVersion}", message);
        }

        [Test]
        public void Migrate_UnsupportedVersion_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            string sourceFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, "UnsupportedVersion8.rtd");
            var fromVersionedFile = new VersionedFile(sourceFilePath);

            string targetFilePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, Path.GetRandomFileName());
            var migrator = new VersionedFileMigrator();

            // Call
            TestDelegate call = () => migrator.Migrate(fromVersionedFile, "17.1", targetFilePath);

            // Assert
            string message = Assert.Throws<CriticalDatabaseMigrationException>(call).Message;
            Assert.AreEqual("Het upgraden van versie 8 is niet ondersteund.", message);
        }
    }
}