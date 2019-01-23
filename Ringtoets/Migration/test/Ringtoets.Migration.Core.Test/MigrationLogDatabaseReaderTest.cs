// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.ObjectModel;
using System.Data.SQLite;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Migration.Core.TestUtil;
using Riskeer.Migration.Core;

namespace Ringtoets.Migration.Core.Test
{
    [TestFixture]
    public class MigrationLogDatabaseReaderTest
    {
        private static readonly TestDataPath testPath = TestDataPath.Ringtoets.Migration.Core;

        [Test]
        public void Constructor_ValidFilePath_ExpectedValues()
        {
            // Setup
            string fileName = TestHelper.GetScratchPadPath(nameof(Constructor_ValidFilePath_ExpectedValues));

            using (new FileDisposeHelper(fileName))
            {
                // Call
                using (var reader = new MigrationLogDatabaseReader(fileName))
                {
                    // Assert
                    Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
                }
            }
        }

        [Test]
        public void GetMigrationLogMessages_InvalidDatabaseFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string fileName = TestHelper.GetTestDataPath(testPath, "invalidMigrationLog.sqlite");

            using (var reader = new MigrationLogDatabaseReader(fileName))
            {
                // Call
                TestDelegate test = () => reader.GetMigrationLogMessages();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual("Kritieke fout opgetreden bij het uitlezen van het Ringtoets logbestand van de migratie.",
                                exception.Message);
                Assert.IsInstanceOf<SQLiteException>(exception.InnerException);
            }
        }

        [Test]
        public void GetMigrationLogMessages_ValidMigrationLogFile_ReturnsExpectedLogMessages()
        {
            // Setup
            string fileName = TestHelper.GetTestDataPath(testPath, "validMigrationLog.sqlite");

            using (var reader = new MigrationLogDatabaseReader(fileName))
            {
                // Call
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                // Assert
                Assert.AreEqual(2, messages.Count);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(new MigrationLogMessage("version 1", "version 2", "some message"),
                                                                      messages[0]);
                MigrationLogTestHelper.AssertMigrationLogMessageEqual(new MigrationLogMessage("version 2", "version 3", "different message"),
                                                                      messages[1]);
            }
        }

        [Test]
        public void GetMigrationLogMessages_IncompleteMigrationLogFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string fileName = TestHelper.GetTestDataPath(testPath, "incompleteMigrationLog.sqlite");

            using (var reader = new MigrationLogDatabaseReader(fileName))
            {
                // Call
                TestDelegate test = () => reader.GetMigrationLogMessages();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);
                Assert.AreEqual("Kritieke fout opgetreden bij het uitlezen van het Ringtoets logbestand van de migratie.",
                                exception.Message);
                Assert.IsInstanceOf<ArgumentException>(exception.InnerException);
            }
        }

        [Test]
        public void GetMigrationLogMessages_EmptyMigrationLogFile_ReturnsEmptyLogMessages()
        {
            // Setup
            string fileName = TestHelper.GetTestDataPath(testPath, "emptyMigrationLog.sqlite");

            using (var reader = new MigrationLogDatabaseReader(fileName))
            {
                // Call
                ReadOnlyCollection<MigrationLogMessage> messages = reader.GetMigrationLogMessages();

                // Assert
                Assert.AreEqual(0, messages.Count);
            }
        }
    }
}