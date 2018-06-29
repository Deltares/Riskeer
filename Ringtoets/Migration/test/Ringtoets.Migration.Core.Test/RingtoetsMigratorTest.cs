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
using Migration.Core.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Migration.Test
{
    [TestFixture]
    public class RingtoetsMigratorTest
    {
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new RingtoetsMigrator(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("versionedFileMigrator", paramName);
        }

        [Test]
        public void ShouldMigrate_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var versionedFileMigrator = mocks.Stub<VersionedFileMigrator>();
            var migrator = new RingtoetsMigrator(versionedFileMigrator);

            // Call
            TestDelegate call = () => migrator.ShouldMigrate(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourcePath", paramName);
        }

        [Test]
        public void ShouldMigrateInvalidFilePath_ThrowsArgumentException()
        {
            // Setup
            var versionedFileMigrator = mocks.Stub<VersionedFileMigrator>();
            var migrator = new RingtoetsMigrator(versionedFileMigrator);

            // Call
            TestDelegate call = () => migrator.ShouldMigrate(null);
        }

        [Test]
        public void Migrate_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var versionedFileMigrator = mocks.Stub<VersionedFileMigrator>();
            var migrator = new RingtoetsMigrator(versionedFileMigrator);

            // Call
            TestDelegate call = () => migrator.Migrate(null, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourcePath", paramName);
        }

        [Test]
        public void Migrate_TargetPathNull_ThrowsArgumentNullException()
        {
            // Setup
            var versionedFileMigrator = mocks.Stub<VersionedFileMigrator>();
            var migrator = new RingtoetsMigrator(versionedFileMigrator);

            // Call
            TestDelegate call = () => migrator.Migrate(string.Empty, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("targetPath", paramName);
        }
    }
}
