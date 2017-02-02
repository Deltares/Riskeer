// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Migration.Scripts.Data;
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

            string toVersion = "1";
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