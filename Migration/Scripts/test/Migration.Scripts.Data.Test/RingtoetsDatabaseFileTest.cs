﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;

namespace Migration.Scripts.Data.Test
{
    [TestFixture]
    public class RingtoetsDatabaseFileTest
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
                using (new RingtoetsDatabaseFile(filePath)) {}
            };

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual($"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet leeg of ongedefinieerd zijn.", exception.Message);
        }

        [Test]
        public void Constructor_FileNotWritable_ThrowsArgumentException()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            using (new FileDisposeHelper(filePath))
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                File.SetAttributes(filePath, attributes | FileAttributes.ReadOnly);
                try
                {
                    // Call
                    TestDelegate call = () =>
                    {
                        using (new RingtoetsDatabaseFile(filePath)) {}
                    };

                    // Assert
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.";
                    TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
                }
                finally
                {
                    File.SetAttributes(filePath, attributes);
                }
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void CreateStructure_QueryIsNullOrWhiteSpace_ThrowsArgumentException(string query)
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            using (new FileDisposeHelper(filePath))
            using (var databaseFile = new RingtoetsDatabaseFile(filePath))
            {
                databaseFile.OpenDatabaseConnection();

                // Call
                TestDelegate call = () => databaseFile.CreateStructure(query);

                // Assert
                string paramName = Assert.Throws<ArgumentException>(call).ParamName;
                Assert.AreEqual("query", paramName);
            }
        }

        [Test]
        public void CreateStructure_InvalidQuery_ThrowsSQLiteException()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            using (new FileDisposeHelper(filePath))
            using (var databaseFile = new RingtoetsDatabaseFile(filePath))
            {
                databaseFile.OpenDatabaseConnection();

                // Call
                TestDelegate call = () => databaseFile.CreateStructure("THIS WILL FAIL");

                // Assert
                Assert.Throws<SQLiteException>(call);
            }
        }

        [Test]
        public void CreateStructure_ValidQuery_CreatesFile()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            using (var databaseFile = new RingtoetsDatabaseFile(filePath))
            {
                databaseFile.OpenDatabaseConnection();

                // Call
                databaseFile.CreateStructure(";");

                // Assert
                Assert.IsTrue(File.Exists(filePath));
            }

            using (new FileDisposeHelper(filePath)) {}
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void ExecuteMigration_QueryIsNullOrWhiteSpace_ThrowsArgumentException(string query)
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            using (new FileDisposeHelper(filePath))
            using (var databaseFile = new RingtoetsDatabaseFile(filePath))
            {
                databaseFile.OpenDatabaseConnection();

                // Call
                TestDelegate call = () => databaseFile.ExecuteMigration(query);

                // Assert
                string paramName = Assert.Throws<ArgumentException>(call).ParamName;
                Assert.AreEqual("query", paramName);
            }
        }

        [Test]
        public void ExecuteMigration_InvalidQuery_ThrowsSQLiteException()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            using (new FileDisposeHelper(filePath))
            using (var databaseFile = new RingtoetsDatabaseFile(filePath))
            {
                databaseFile.OpenDatabaseConnection();

                // Call
                TestDelegate call = () => databaseFile.ExecuteMigration("THIS WILL FAIL");

                // Assert
                Assert.Throws<SQLiteException>(call);
            }
        }

        [Test]
        public void ExecuteMigration_ValidQuery_CreatesFile()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            using (var databaseFile = new RingtoetsDatabaseFile(filePath))
            {
                databaseFile.OpenDatabaseConnection();

                // Call
                databaseFile.ExecuteMigration(";");

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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Migration.Core.Storage, filename);

            // Call
            TestDelegate call = () =>
            {
                using (var databaseFile = new RingtoetsDatabaseFile(filePath))
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