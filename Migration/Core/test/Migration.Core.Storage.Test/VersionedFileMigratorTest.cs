// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using Migration.Scripts.Data;
using Migration.Scripts.Data.Exceptions;
using Migration.Scripts.Data.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Migration.Core.Storage.Test
{
    [TestFixture]
    public class VersionedFileMigratorTest
    {
        [Test]
        public void Constructor_ComparerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new SimpleVersionedFileMigrator(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("comparer", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void IsVersionSupported_FromVersionIsNullOrWhiteSpace_ReturnsFalse(string fromVersion)
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            mockRepository.ReplayAll();
            var migrator = new SimpleVersionedFileMigrator(comparer);

            // Call
            bool isSupported = migrator.IsVersionSupported(fromVersion);

            // Assert
            Assert.IsFalse(isSupported);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("true", true)]
        [TestCase("false", false)]
        public void IsVersionSupported_ValidFromVersion_ReturnsIfSupported(string fromVersion, bool shouldSupport)
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            mockRepository.ReplayAll();

            const string toVersion = "1";
            var migrator = new SimpleVersionedFileMigrator(comparer)
            {
                CreateScripts =
                {
                    new TestCreateScript(toVersion)
                },
                UpgradeScripts =
                {
                    new TestUpgradeScript("true", toVersion)
                }
            };

            // Call
            bool isSupported = migrator.IsVersionSupported(fromVersion);

            // Assert
            Assert.AreEqual(shouldSupport, isSupported);
            mockRepository.VerifyAll();
        }

        [Test]
        public void NeedsMigrate_VersionedFileNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            mockRepository.ReplayAll();
            var migrator = new SimpleVersionedFileMigrator(comparer);

            // Call
            TestDelegate call = () => migrator.NeedsMigrate(null, "");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("versionedFile", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void NeedsMigrate_ToVersionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            mockRepository.ReplayAll();
            var migrator = new SimpleVersionedFileMigrator(comparer);

            // Call
            TestDelegate call = () => migrator.NeedsMigrate(versionedFile, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("toVersion", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("0", "0", false)]
        [TestCase("1", "0", false)]
        [TestCase("0", "1", true)]
        public void NeedsMigrate_ValidVersionedFile_ReturnsIfNeedsMigrate(string fromVersion, string toVersion, bool shouldMigrate)
        {
            // Setup
            var mockRepository = new MockRepository();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            versionedFile.Expect(vf => vf.GetVersion()).Return(fromVersion);
            mockRepository.ReplayAll();

            var migrator = new SimpleVersionedFileMigrator(new SimpleVersionComparer())
            {
                CreateScripts =
                {
                    new TestCreateScript(toVersion)
                },
                UpgradeScripts =
                {
                    new TestUpgradeScript(fromVersion, toVersion)
                }
            };

            // Call
            bool needsMigrate = migrator.NeedsMigrate(versionedFile, toVersion);

            // Assert
            Assert.AreEqual(shouldMigrate, needsMigrate);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Migrate_VersionedFileNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            mockRepository.ReplayAll();

            const string toVersion = "toVersion";
            const string toLocation = "location";

            var migrator = new SimpleVersionedFileMigrator(comparer);

            // Call
            TestDelegate call = () => migrator.Migrate(null, toVersion, toLocation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("versionedFile", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Migrate_ToVersionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            mockRepository.ReplayAll();

            const string toLocation = "location";

            var migrator = new SimpleVersionedFileMigrator(comparer);

            // Call
            TestDelegate call = () => migrator.Migrate(versionedFile, null, toLocation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("toVersion", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Migrate_NewFileLocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            mockRepository.ReplayAll();

            const string toVersion = "toVersion";

            var migrator = new SimpleVersionedFileMigrator(comparer);

            // Call
            TestDelegate call = () => migrator.Migrate(versionedFile, toVersion, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("newFileLocation", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Migrate_NewFileLocationEqualToVersionedFileLocation_ThrowsCriticalMigrationException()
        {
            // Setup
            const string toVersion = "toVersion";
            const string toLocation = "location";

            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            versionedFile.Expect(vf => vf.Location).Return(toLocation);
            mockRepository.ReplayAll();

            var migrator = new SimpleVersionedFileMigrator(comparer);

            // Call
            TestDelegate call = () => migrator.Migrate(versionedFile, toVersion, toLocation);

            // Assert
            var exception = Assert.Throws<CriticalMigrationException>(call);
            Assert.AreEqual("Het doelprojectpad moet anders zijn dan het bronprojectpad.", exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Migrate_VersionedFileVersionNotSupported_ThrowsCriticalMigrationException()
        {
            // Setup
            const string fromLocation = "fromLocation";
            const string toVersion = "toVersion";
            const string toLocation = "location";
            const string incorrectVersion = "not supported";

            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            versionedFile.Expect(vf => vf.Location).Return(fromLocation);
            versionedFile.Expect(vf => vf.GetVersion()).Return(incorrectVersion);
            mockRepository.ReplayAll();

            var migrator = new SimpleVersionedFileMigrator(comparer);

            // Call
            TestDelegate call = () => migrator.Migrate(versionedFile, toVersion, toLocation);

            // Assert
            var exception = Assert.Throws<CriticalMigrationException>(call);
            Assert.AreEqual($"Het migreren van een projectbestand met versie '{incorrectVersion}' naar versie '{toVersion}' is niet ondersteund.", exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Migrate_NoMigrationScriptToMigrate_ThrowsCriticalMigrationException()
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string fromLocation = "fromLocation";
            const string toVersion = "toVersion";
            const string toLocation = "location";
            const string incorrectVersion = "not supported";

            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            versionedFile.Expect(vf => vf.Location).Return(fromLocation);
            versionedFile.Expect(vf => vf.GetVersion()).Return(fromVersion);
            mockRepository.ReplayAll();

            var migrator = new SimpleVersionedFileMigrator(comparer)
            {
                CreateScripts =
                {
                    new TestCreateScript(toVersion)
                },
                UpgradeScripts =
                {
                    new TestUpgradeScript(fromVersion, toVersion)
                }
            };

            // Call
            TestDelegate call = () => migrator.Migrate(versionedFile, incorrectVersion, toLocation);

            // Assert
            var exception = Assert.Throws<CriticalMigrationException>(call);
            Assert.AreEqual($"Het migreren van een projectbestand met versie '{fromVersion}' naar versie '{incorrectVersion}' is niet ondersteund.", exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Migrate_ValidMigration_CreatesNewVersion()
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string fromLocation = "fromLocation";
            const string toVersion = "toVersion";

            string toLocation = TestHelper.GetScratchPadPath(nameof(Migrate_ValidMigration_CreatesNewVersion));

            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            versionedFile.Stub(vf => vf.Location).Return(fromLocation);
            versionedFile.Expect(vf => vf.GetVersion()).Return(fromVersion);
            mockRepository.ReplayAll();

            var migrator = new SimpleVersionedFileMigrator(comparer)
            {
                CreateScripts =
                {
                    new TestCreateScript(toVersion)
                },
                UpgradeScripts =
                {
                    new TestUpgradeScript(fromVersion, toVersion)
                }
            };

            // Call
            migrator.Migrate(versionedFile, toVersion, toLocation);

            // Assert
            Assert.IsTrue(File.Exists(toLocation), $"File at location {toLocation} has not been created");
            using (new FileDisposeHelper(toLocation)) {}

            mockRepository.VerifyAll();
        }

        [Test]
        public void Migrate_ValidChainingMigration_CreatesNewVersion()
        {
            // Setup
            const string fromLocation = "fromLocation";
            const string fromVersion = "0";
            const string toVersion = "2";

            string toLocation = TestHelper.GetScratchPadPath(nameof(Migrate_ValidChainingMigration_CreatesNewVersion));

            var mockRepository = new MockRepository();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            versionedFile.Stub(vf => vf.Location).Return(fromLocation);
            versionedFile.Expect(vf => vf.GetVersion()).Return(fromVersion);
            mockRepository.ReplayAll();

            var migrator = new SimpleVersionedFileMigrator(new SimpleVersionComparer())
            {
                CreateScripts =
                {
                    new TestCreateScript("1"),
                    new TestCreateScript(toVersion)
                },
                UpgradeScripts =
                {
                    new TestUpgradeScript(fromVersion, "1"),
                    new TestUpgradeScript("1", toVersion)
                }
            };

            // Call
            migrator.Migrate(versionedFile, toVersion, toLocation);

            // Assert
            Assert.IsTrue(File.Exists(toLocation), $"File at location {toLocation} has not been created");
            File.Delete(toLocation);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Migrate_ValidMigrationFileInUse_ThrowsCriticalMigrationException()
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string fromLocation = "fromLocation";
            const string toVersion = "toVersion";

            string toLocation = TestHelper.GetScratchPadPath(nameof(Migrate_ValidMigrationFileInUse_ThrowsCriticalMigrationException));

            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            versionedFile.Stub(vf => vf.Location).Return(fromLocation);
            versionedFile.Expect(vf => vf.GetVersion()).Return(fromVersion);
            mockRepository.ReplayAll();

            using (var fileDisposeHelper = new FileDisposeHelper(toLocation))
            {
                fileDisposeHelper.LockFiles();
                var migrator = new SimpleVersionedFileMigrator(comparer)
                {
                    CreateScripts =
                    {
                        new TestCreateScript(toVersion)
                    },
                    UpgradeScripts =
                    {
                        new TestUpgradeScript(fromVersion, toVersion)
                    }
                };

                // Call
                TestDelegate call = () => migrator.Migrate(versionedFile, toVersion, toLocation);

                // Assert
                var exception = Assert.Throws<CriticalMigrationException>(call);
                StringAssert.StartsWith("Het gemigreerde projectbestand is aangemaakt op '", exception.Message);
                StringAssert.EndsWith($"', maar er is een onverwachte fout opgetreden tijdens het verplaatsen naar '{toLocation}'.",
                                      exception.Message);
            }

            mockRepository.VerifyAll();
        }

        private class SimpleVersionComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return Compare((string) x, (string) y);
            }

            private static int Compare(string x, string y)
            {
                return string.Compare(x, y, StringComparison.InvariantCulture);
            }
        }

        private class SimpleVersionedFileMigrator : VersionedFileMigrator
        {
            public SimpleVersionedFileMigrator(IComparer comparer) : base(comparer)
            {
                UpgradeScripts = new List<UpgradeScript>();
                CreateScripts = new List<CreateScript>();
            }

            public List<UpgradeScript> UpgradeScripts { get; }
            public List<CreateScript> CreateScripts { get; }

            protected override IEnumerable<UpgradeScript> GetAvailableUpgradeScripts()
            {
                return UpgradeScripts;
            }

            protected override IEnumerable<CreateScript> GetAvailableCreateScripts()
            {
                return CreateScripts;
            }
        }
    }
}