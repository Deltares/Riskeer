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
using NUnit.Framework;
using Rhino.Mocks;

namespace Migration.Scripts.Data.Test
{
    [TestFixture]
    public class UpgradeScriptTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_InvalidFromVersion_ThrowsArgumentException(string fromVersion)
        {
            // Setup
            const string toVersion = "toVersion";

            // Call
            TestDelegate call = () => new TestUpgradeScript(fromVersion, toVersion);

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

            // Call
            TestDelegate call = () => new TestUpgradeScript(fromVersion, toVersion);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("toVersion", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ReturnsExpectedValues()
        {
            // Setup
            const string fromVersion = "fromVersion";
            const string toVersion = "toVersion";

            // Call
            var upgradeScript = new TestUpgradeScript(fromVersion, toVersion);

            // Assert
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
            var upgradeScript = new TestUpgradeScript(fromVersion, toVersion);

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
            var upgradeScript = new TestUpgradeScript(fromVersion, toVersion);

            // Call
            TestDelegate call = () => upgradeScript.Upgrade(sourceVersionedFile, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("target", paramName);
            mockRepository.VerifyAll();
        }
    }
}