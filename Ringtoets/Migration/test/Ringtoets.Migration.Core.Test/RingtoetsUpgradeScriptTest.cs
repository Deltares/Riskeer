// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Migration.Scripts.Data.Exceptions;
using NUnit.Framework;
using Ringtoets.Common.Util;

namespace Application.Ringtoets.Migration.Core.Test
{
    [TestFixture]
    public class RingtoetsUpgradeScriptTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_FromVersionNullOrEmpty_ThrowsArgumentException(string fromVersion)
        {
            // Setup
            const string query = "Valid query";
            string toVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            // Call
            TestDelegate call = () => new RingtoetsUpgradeScript(fromVersion, toVersion, query, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("fromVersion", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_ToVersionNullOrEmpty_ThrowsArgumentException(string toVersion)
        {
            // Setup
            string fromVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            const string query = "Valid query";

            // Call
            TestDelegate call = () => new RingtoetsUpgradeScript(fromVersion, toVersion, query, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("toVersion", paramName);
        }

        [Test]
        [TestCase("4")]
        public void Constructor_InvalidFromVersion_ThrowsArgumentException(string fromVersion)
        {
            // Setup
            const string query = "Valid query";
            string toVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            // Call
            TestDelegate call = () => new RingtoetsUpgradeScript(fromVersion, toVersion, query, string.Empty);

            // Assert
            string expectedMessage = $@"'{fromVersion}' is geen geldige Ringtoets versie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase("4")]
        public void Constructor_InvalidToVersion_ThrowsArgumentException(string toVersion)
        {
            // Setup
            string fromVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            const string query = "Valid query";

            // Call
            TestDelegate call = () => new RingtoetsUpgradeScript(fromVersion, toVersion, query, string.Empty);

            // Assert
            string expectedMessage = $@"'{toVersion}' is geen geldige Ringtoets versie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_InvalidQuery_ThrowsArgumentException(string query)
        {
            // Setup
            string fromVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            string toVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            // Call
            TestDelegate call = () => new RingtoetsUpgradeScript(fromVersion, toVersion, query, string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("query", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ReturnsExpectedValues()
        {
            // Setup
            string fromVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            string toVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            const string query = ";";

            // Call
            var upgradeScript = new RingtoetsUpgradeScript(fromVersion, toVersion, query, string.Empty);

            // Assert
            Assert.IsInstanceOf<UpgradeScript>(upgradeScript);
            Assert.AreEqual(fromVersion, upgradeScript.FromVersion());
            Assert.AreEqual(toVersion, upgradeScript.ToVersion());
        }

        [Test]
        public void Upgrade_UpgradeFails_ThrowsCriticalMigrationException()
        {
            // Setup
            string fromVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            string toVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            string fileLocation = TestHelper.GetScratchPadPath(nameof(Upgrade_UpgradeFails_ThrowsCriticalMigrationException));
            var upgradeScript = new RingtoetsUpgradeScript(fromVersion, toVersion, "THIS WILL FAIL", string.Empty);

            using (new FileDisposeHelper(fileLocation))
            {
                // Call
                TestDelegate call = () => upgradeScript.Upgrade(fileLocation, fileLocation);

                // Assert
                var exception = Assert.Throws<CriticalMigrationException>(call);
                Assert.AreEqual($"Het migreren van het Ringtoets projectbestand van versie '{fromVersion}' naar '{fromVersion}' is mislukt.",
                                exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void Upgrade_ValidParameters_ExpectedProperties()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Upgrade_ValidParameters_ExpectedProperties));
            string fromVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            string toVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

            var upgradeScript = new RingtoetsUpgradeScript(fromVersion, toVersion, ";", "");

            // Call
            upgradeScript.Upgrade("c:\\file.ext", filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
            File.Delete(filePath);
        }
    }
}