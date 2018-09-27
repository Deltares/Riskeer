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
using Core.Common.TestUtil;
using Migration.Scripts.Data.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Migration.Scripts.Data.Test
{
    [TestFixture]
    public class FileMigrationScriptTest
    {
        [Test]
        public void Constructor_CreateScriptNull_ThrowsArgumentNullException()
        {
            // Setup
            var upgradeScript = new TestUpgradeScript("1", "2");

            // Call
            TestDelegate call = () => new FileMigrationScript(null, upgradeScript);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("createScript", paramName);
        }

        [Test]
        public void Constructor_UpgradeScriptNull_ThrowsArgumentNullException()
        {
            // Setup
            var createScript = new TestCreateScript("1");

            // Call
            TestDelegate call = () => new FileMigrationScript(createScript, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("upgradeScript", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedProperties()
        {
            // Setup
            var createScript = new TestCreateScript("2");
            var upgradeScript = new TestUpgradeScript("1", "2");

            // Call
            var migrationScript = new FileMigrationScript(createScript, upgradeScript);

            // Assert
            Assert.AreEqual(upgradeScript.FromVersion(), migrationScript.SupportedVersion());
            Assert.AreEqual(upgradeScript.ToVersion(), migrationScript.TargetVersion());
        }

        [Test]
        public void Upgrade_VersionedFileNull_ThrowsArgumentNullException()
        {
            // Setup
            var createScript = new TestCreateScript("2");
            var upgradeScript = new TestUpgradeScript("1", "2");
            var migrationScript = new FileMigrationScript(createScript, upgradeScript);

            // Call
            TestDelegate call = () => migrationScript.Upgrade(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceVersionedFile", paramName);
        }

        [Test]
        public void Upgrade_ValidParameters_ExpectedProperties()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Upgrade_ValidParameters_ExpectedProperties));

            var mockRepository = new MockRepository();
            var versionedFile = mockRepository.Stub<IVersionedFile>();
            versionedFile.Expect(vf => vf.Location).Return(filePath);
            mockRepository.ReplayAll();

            var createScript = new TestCreateScript("2");
            var upgradeScript = new TestUpgradeScript("1", "2");
            var migrationScript = new FileMigrationScript(createScript, upgradeScript);

            using (new FileDisposeHelper(filePath))
            {
                // Call
                IVersionedFile upgradedFile = migrationScript.Upgrade(versionedFile);

                // Assert
                Assert.IsNotNull(upgradedFile);
            }

            mockRepository.VerifyAll();
        }
    }
}