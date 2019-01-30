// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using NUnit.Framework;

namespace Riskeer.Migration.Core.Test
{
    [TestFixture]
    public class ProjectDatabaseFileTest
    {
        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void Constructor_FilePathNullOrWhiteSpace_ThrowsArgumentException(string filePath)
        {
            // Call
            TestDelegate call = () =>
            {
                using (new ProjectDatabaseFile(filePath)) {}
            };

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual($"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet leeg of ongedefinieerd zijn.", exception.Message);
        }

        [Test]
        public void Constructor_FileNotWritable_ThrowsArgumentException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(ProjectDatabaseFileTest)}.{nameof(Constructor_FileNotWritable_ThrowsArgumentException)}");

            using (var helper = new FileDisposeHelper(filePath))
            {
                helper.LockFiles();

                // Call
                TestDelegate call = () =>
                {
                    using (new ProjectDatabaseFile(filePath)) {}
                };

                // Assert
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void ExecuteQuery_QueryIsNullOrWhiteSpace_ThrowsArgumentException(string query)
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetScratchPadPath(nameof(ExecuteQuery_QueryIsNullOrWhiteSpace_ThrowsArgumentException) + filename);

            using (new FileDisposeHelper(filePath))
            using (var databaseFile = new ProjectDatabaseFile(filePath))
            {
                databaseFile.OpenDatabaseConnection();

                // Call
                TestDelegate call = () => databaseFile.ExecuteQuery(query);

                // Assert
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Parameter must be a valid database script.");
            }
        }

        [Test]
        public void ExecuteQuery_InvalidQuery_ThrowsSQLiteException()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetScratchPadPath(nameof(ExecuteQuery_InvalidQuery_ThrowsSQLiteException) + filename);

            using (new FileDisposeHelper(filePath))
            using (var databaseFile = new ProjectDatabaseFile(filePath))
            {
                databaseFile.OpenDatabaseConnection();

                // Call
                TestDelegate call = () => databaseFile.ExecuteQuery("THIS WILL FAIL");

                // Assert
                Assert.Throws<SQLiteException>(call);
            }
        }

        [Test]
        public void ExecuteQuery_ValidQuery_CreatesFile()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetScratchPadPath(filename);

            using (var databaseFile = new ProjectDatabaseFile(filePath))
            {
                databaseFile.OpenDatabaseConnection();

                // Call
                databaseFile.ExecuteQuery(";");

                // Assert
                Assert.IsTrue(File.Exists(filePath));
            }

            using (new FileDisposeHelper(filePath)) {}
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetScratchPadPath(filename);

            // Call
            TestDelegate call = () =>
            {
                using (var databaseFile = new ProjectDatabaseFile(filePath))
                {
                    databaseFile.Dispose();
                }
            };

            using (new FileDisposeHelper(filePath))
            {
                // Assert
                Assert.DoesNotThrow(call);
            }
        }
    }
}