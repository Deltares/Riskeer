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
using NUnit.Framework;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.TestUtil.Test
{
    [TestFixture]
    public class SqLiteDatabaseHelperTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles");

        [Test]
        public void CreateDatabaseFile_NullPath_ThrowsArgumentNullException()
        {
            // Setup
            const string validScript = ";";

            // Call
            TestDelegate test = () => SqLiteDatabaseHelper.CreateDatabaseFile(null, validScript);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void CreateDatabaseFile_ValidPathNullScript_ThrowsArgumentNullException()
        {
            // Setup
            const string validPath = "tempFile";

            // Call
            TestDelegate test = () => SqLiteDatabaseHelper.CreateDatabaseFile(validPath, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void CreateDatabaseFile_ValidPathInvalidScript_ThrowsSQLiteException()
        {
            // Setup
            string validPath = Path.Combine(testDataPath, "tempFile.rtd");
            const string invalidScript = "SELECT '' FROM *;";

            using (new FileDisposeHelper(validPath))
            {
                // Call
                TestDelegate test = () => SqLiteDatabaseHelper.CreateDatabaseFile(validPath, invalidScript);

                // Assert
                Assert.Throws<SQLiteException>(test);
            }
        }

        [Test]
        public void CreateDatabaseFile_ValidPathValidScript_CreatesValidFile()
        {
            // Setup
            string validPath = Path.Combine(testDataPath, "tempFile.rtd");
            const string validScript = "select * from sqlite_master;";

            using (new FileDisposeHelper(validPath))
            {
                // Call
                TestDelegate test = () => SqLiteDatabaseHelper.CreateDatabaseFile(validPath, validScript);

                // Assert
                Assert.DoesNotThrow(test);
                Assert.IsTrue(File.Exists(validPath));
            }
        }

        [Test]
        public void CreateDatabaseFile_FileAreadyExists_OverwriteFile()
        {
            // Setup
            string validPath = Path.Combine(testDataPath, "tempFile.rtd");
            const string validScript = "select * from sqlite_master;";

            using (var fileDisposeHelper = new FileDisposeHelper(validPath))
            {
                // Call
                TestDelegate test = () => SqLiteDatabaseHelper.CreateDatabaseFile(validPath, validScript);

                // Assert
                Assert.DoesNotThrow(test);
                Assert.IsTrue(File.Exists(validPath));

                // Assert
                File.Exists(validPath);
            }
        }

        [Test]
        public void CreateDatabaseFile_FileAreadyExistsAndLocked_Fail()
        {
            // Setup
            string validPath = Path.Combine(testDataPath, "tempFile.rtd");
            const string validScript = ";";

            using (new FileDisposeHelper(validPath))
            {
                using (File.Create(validPath))
                {
                    // Call
                    TestDelegate test = () => SqLiteDatabaseHelper.CreateDatabaseFile(validPath, validScript);

                    // Assert
                    Assert.Throws<IOException>(test);
                }
            }
        }

        [Test]
        public void CreateValidRingtoetsDatabase_ValidArguments_SavesProjectToFile()
        {
            // Setup
            string validPath = Path.Combine(testDataPath, "tempFile.rtd");
            var project = new RingtoetsProject();

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(validPath);
            try
            {
                // Call
                TestDelegate test = () => SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(validPath, project);

                // Assert
                Assert.DoesNotThrow(test);
                Assert.IsTrue(File.Exists(validPath));
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void GetCorruptSchema_Always_ScriptThatOnlyGeneratesEnoughToPassValidation()
        {
            // Setup
            const string expectedCreateVersionTable = "CREATE TABLE VersionEntity ";
            const string notExpectedCreateProjectEntityTable = "CREATE TABLE ProjectEntity ";

            // Call
            string query = SqLiteDatabaseHelper.GetCorruptSchema();

            // Assert
            Assert.IsNotNullOrEmpty(query);
            Assert.IsTrue(query.Contains(expectedCreateVersionTable));
            Assert.IsFalse(query.Contains(notExpectedCreateProjectEntityTable));
        }

        private static void CallGarbageCollector()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}