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
using System.Data.SQLite;
using System.IO;
using Core.Common.TestUtil;
using Migration.Scripts.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Migration.Test
{
    [TestFixture]
    public class RingtoetsUpgradeScriptTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_InvalidFromVersion_ThrowsArgumentException(string fromVersion)
        {
            // Setup
            const string query = "Valid query";
            const string toVersion = "toVersion";

            // Call
            TestDelegate call = () => new RingtoetsUpgradeScript(fromVersion, toVersion, query);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("fromVersion", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_InvalidToVersion_ThrowsArgumentException(string toVersion)
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string query = "Valid query";

            // Call
            TestDelegate call = () => new RingtoetsUpgradeScript(fromVersion, toVersion, query);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("toVersion", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_InvalidQuery_ThrowsArgumentException(string query)
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string toVersion = "toVersion";

            // Call
            TestDelegate call = () => new RingtoetsUpgradeScript(fromVersion, toVersion, query);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("query", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ReturnsExpectedValues()
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string toVersion = "toVersion";
            const string query = ";";

            // Call
            var upgradeScript = new RingtoetsUpgradeScript(fromVersion, toVersion, query);

            // Assert
            Assert.IsInstanceOf<UpgradeScript>(upgradeScript);
            Assert.AreEqual(fromVersion, upgradeScript.FromVersion());
            Assert.AreEqual(toVersion, upgradeScript.ToVersion());
        }

        [Test]
        public void Upgrade_SourceNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var targetVersionedFile = mockRepository.Stub<IVersionedFile>();
            mockRepository.ReplayAll();

            const string fromVersion = "fromVersion";
            const string toVersion = "toVersion";
            const string query = ";";
            var upgradeScript = new RingtoetsUpgradeScript(fromVersion, toVersion, query);

            // Call
            TestDelegate call = () => upgradeScript.Upgrade(null, targetVersionedFile);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("source", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Upgrade_TargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var sourceVersionedFile = mockRepository.Stub<IVersionedFile>();
            mockRepository.ReplayAll();

            const string fromVersion = "fromVersion";
            const string toVersion = "toVersion";
            const string query = ";";
            var upgradeScript = new RingtoetsUpgradeScript(fromVersion, toVersion, query);

            // Call
            TestDelegate call = () => upgradeScript.Upgrade(sourceVersionedFile, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("target", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Upgrade_UpgradeFails_ThrowsSQLiteException()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Migration, filename);

            var mockRepository = new MockRepository();
            var sourceVersionedFile = mockRepository.Stub<IVersionedFile>();
            sourceVersionedFile.Expect(tvf => tvf.Location).Return("c:\\file.ext");
            var targetVersionedFile = mockRepository.Stub<IVersionedFile>();
            targetVersionedFile.Expect(tvf => tvf.Location).Return(filePath);
            mockRepository.ReplayAll();

            var upgradeScript = new RingtoetsUpgradeScript("1", "2", "THIS WILL FAIL");

            // Call
            TestDelegate call = () => upgradeScript.Upgrade(sourceVersionedFile, targetVersionedFile);

            // Assert
            using (new FileDisposeHelper(filePath))
            {
                Assert.Throws<SQLiteException>(call);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Upgrade_ValidParameters_ExpectedProperties()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            var mockRepository = new MockRepository();
            var sourceVersionedFile = mockRepository.Stub<IVersionedFile>();
            sourceVersionedFile.Expect(tvf => tvf.Location).Return("c:\\file.ext");
            var targetVersionedFile = mockRepository.Stub<IVersionedFile>();
            targetVersionedFile.Expect(tvf => tvf.Location).Return(filePath);
            mockRepository.ReplayAll();

            var upgradeScript = new RingtoetsUpgradeScript("1", "2", ";");

            // Call
            upgradeScript.Upgrade(sourceVersionedFile, targetVersionedFile);

            // Assert
            Assert.IsNotNull(targetVersionedFile);
            using (new FileDisposeHelper(filePath))
            {
                Assert.IsTrue(File.Exists(filePath));
            }
            mockRepository.VerifyAll();
        }
    }
}