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
using System.Data;
using System.Data.SQLite;
using System.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Migration.Scripts.Data.Exceptions;
using NUnit.Framework;

namespace Migration.Scripts.Data.Test
{
    [TestFixture]
    public class MigrationScriptTest
    {
        [Test]
        public void Constructor_CreateScriptNull_ThrowsArgumentNullException()
        {
            // Setup
            var upgradeScript = new UpgradeScript("1", "2", ";");

            // Call
            TestDelegate call = () => new MigrationScript(null, upgradeScript);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("createScript", paramName);
        }

        [Test]
        public void Constructor_UpgradeScriptNull_ThrowsArgumentNullException()
        {
            // Setup
            var createScript = new CreateScript("1", ";");

            // Call
            TestDelegate call = () => new MigrationScript(createScript, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("upgradeScript", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedProperties()
        {
            // Setup
            var createScript = new CreateScript("2", ";");
            var upgradeScript = new UpgradeScript("1", "2", ";");

            // Call
            var migrationScript = new MigrationScript(createScript, upgradeScript);

            // Assert
            Assert.AreEqual(upgradeScript.FromVersion(), migrationScript.SupportedVersion());
            Assert.AreEqual(upgradeScript.ToVersion(), migrationScript.TargetVersion());
        }

        [Test]
        public void Upgrade_VersionedFileNull_ThrowsArgumentNullException()
        {
            // Setup
            var createScript = new CreateScript("2", ";");
            var upgradeScript = new UpgradeScript("1", "2", ";");
            var migrationScript = new MigrationScript(createScript, upgradeScript);

            // Call
            TestDelegate call = () => migrationScript.Upgrade(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceVersionedFile", paramName);
        }

        [Test]
        public void Upgrade_UpgradeFails_ThrowsCriticalDatabaseMigrationException()
        {
            // Setup
            var createScript = new CreateScript("2", "THIS WILL FAIL");
            var upgradeScript = new UpgradeScript("1", "2", ";");
            var migrationScript = new MigrationScript(createScript, upgradeScript);
            var versionedFile = new VersionedFile("c:\\file.ext");

            // Call
            TestDelegate call = () => migrationScript.Upgrade(versionedFile);

            // Assert
            CriticalDatabaseMigrationException exception = Assert.Throws<CriticalDatabaseMigrationException>(call);
            Assert.AreEqual("Het migreren van de data is mislukt.", exception.Message);
            Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
        }

        [Test]
        public void Upgrade_ValidParameters_ExpectedProperties()
        {
            // Setup
            var createScript = new CreateScript("2", ";");
            var upgradeScript = new UpgradeScript("1", "2", ";");
            var migrationScript = new MigrationScript(createScript, upgradeScript);
            var versionedFile = new VersionedFile("c:\\file.ext");

            // Call
            VersionedFile upgradedFile = migrationScript.Upgrade(versionedFile);

            // Assert
            Assert.IsNotNull(upgradedFile);
            Assert.IsTrue(File.Exists(upgradedFile.Location));
            using (new FileDisposeHelper(upgradedFile.Location)) {}
        }

        [Test]
        public void Upgrade_ActualQueries_ExpectedProperties()
        {
            // Setup
            var createScript = new CreateScript("2", "CREATE TABLE 'MigrationScript' ('Field' TEXT NOT NULL);");
            var upgradeScript = new UpgradeScript("1", "2", "INSERT INTO 'MigrationScript' SELECT '{0}';");
            var migrationScript = new MigrationScript(createScript, upgradeScript);
            var versionedFile = new VersionedFile("c:\\file.ext");

            // Call
            VersionedFile upgradedFile = migrationScript.Upgrade(versionedFile);

            // Assert
            Assert.IsNotNull(upgradedFile);

            Assert.IsTrue(File.Exists(upgradedFile.Location));
            using (var msdr = new MigrationScriptDatabaseReader(upgradedFile.Location))
            {
                Assert.IsTrue(msdr.IsValueInserted(versionedFile.Location));
            }
            using (new FileDisposeHelper(upgradedFile.Location)) {}
        }

        private class MigrationScriptDatabaseReader : SqLiteDatabaseReaderBase
        {
            public MigrationScriptDatabaseReader(string filePath) : base(filePath) {}

            public bool IsValueInserted(string value)
            {
                const string query = "SELECT FIELD FROM 'MigrationScript';";
                try
                {
                    using (IDataReader dataReader = CreateDataReader(query, null))
                    {
                        if (!dataReader.Read())
                        {
                            return false;
                        }

                        return dataReader.Read<string>("Field").Equals(value);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}