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
using Riskeer.Integration.Data;

namespace Riskeer.Storage.Core.TestUtil.Test
{
    [TestFixture]
    public class SqLiteDatabaseHelperTest
    {
        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void CreateCorruptDatabaseFile_InvalidPath_ThrowArgumentNullException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => SqLiteDatabaseHelper.CreateCorruptDatabaseFile(invalidFilePath);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void CreateCorruptDatabaseFile_ValidArguments_CreatedDatabaseFile()
        {
            // Setup
            string validPath = TestHelper.GetScratchPadPath(nameof(CreateCorruptDatabaseFile_ValidArguments_CreatedDatabaseFile));

            using (new FileDisposeHelper(validPath))
            {
                try
                {
                    // Call
                    TestDelegate test = () => SqLiteDatabaseHelper.CreateCorruptDatabaseFile(validPath);

                    // Assert
                    Assert.DoesNotThrow(test);
                    Assert.IsTrue(File.Exists(validPath));
                }
                finally
                {
                    CallGarbageCollector();
                }
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void CreateCompleteDatabaseFileWithoutProjectData_InvalidPath_ThrowsArgumentNullException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => SqLiteDatabaseHelper.CreateCompleteDatabaseFileWithoutProjectData(invalidFilePath);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void CreateCompleteDatabaseFileWithoutProjectData_ValidArguments_CreatedDatabaseFile()
        {
            // Setup
            string validPath = TestHelper.GetScratchPadPath("tempFile.rtd");

            using (new FileDisposeHelper(validPath))
            {
                try
                {
                    // Call
                    TestDelegate test = () => SqLiteDatabaseHelper.CreateCompleteDatabaseFileWithoutProjectData(validPath);

                    // Assert
                    Assert.DoesNotThrow(test);
                    Assert.IsTrue(File.Exists(validPath));
                }
                finally
                {
                    CallGarbageCollector();
                }
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void CreateCompleteDatabaseFileEmpty_InvalidPath_ThrowsArgumentNullException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(invalidFilePath);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void CreateCompleteDatabaseFileEmpty_ValidArguments_CreatedDatabaseFile()
        {
            // Setup
            string validPath = TestHelper.GetScratchPadPath(nameof(CreateCompleteDatabaseFileEmpty_ValidArguments_CreatedDatabaseFile));

            using (new FileDisposeHelper(validPath))
            {
                try
                {
                    // Call
                    TestDelegate test = () => SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(validPath);

                    // Assert
                    Assert.DoesNotThrow(test);
                    Assert.IsTrue(File.Exists(validPath));
                }
                finally
                {
                    CallGarbageCollector();
                }
            }
        }

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
            string validPath = TestHelper.GetScratchPadPath(nameof(CreateDatabaseFile_ValidPathInvalidScript_ThrowsSQLiteException));
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
            string validPath = TestHelper.GetScratchPadPath(nameof(CreateDatabaseFile_ValidPathValidScript_CreatesValidFile));
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
        public void CreateDatabaseFile_FileAlreadyExists_OverwriteFile()
        {
            // Setup
            string validPath = TestHelper.GetScratchPadPath(nameof(CreateDatabaseFile_FileAlreadyExists_OverwriteFile));
            const string validScript = "select * from sqlite_master;";

            using (new FileDisposeHelper(validPath))
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
        public void CreateDatabaseFile_FileAlreadyExistsAndLocked_Fail()
        {
            // Setup
            string validPath = TestHelper.GetScratchPadPath(nameof(CreateDatabaseFile_FileAlreadyExistsAndLocked_Fail));
            const string validScript = ";";

            using (var fileDisposeHelper = new FileDisposeHelper(validPath))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate test = () => SqLiteDatabaseHelper.CreateDatabaseFile(validPath, validScript);

                // Assert
                Assert.Throws<IOException>(test);
            }
        }

        [Test]
        public void CreateValidRingtoetsDatabase_ValidArguments_SavesProjectToFile()
        {
            // Setup
            string validPath = TestHelper.GetScratchPadPath(nameof(CreateValidRingtoetsDatabase_ValidArguments_SavesProjectToFile));
            var project = new RingtoetsProject();

            var fileDisposeHelper = new FileDisposeHelper(validPath);
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
            const string expectedCreateVersionTable = "CREATE TABLE 'VersionEntity' ";
            const string notExpectedCreateProjectEntityTable = "CREATE TABLE 'ProjectEntity' ";

            // Call
            string query = SqLiteDatabaseHelper.GetCorruptSchema();

            // Assert
            Assert.That(!string.IsNullOrEmpty(query));
            Assert.IsTrue(query.Contains(expectedCreateVersionTable));
            Assert.IsFalse(query.Contains(notExpectedCreateProjectEntityTable));
        }

        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void AddVersionEntity_InvalidPath_ThrowsArgumentNullException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => SqLiteDatabaseHelper.AddVersionEntity(invalidFilePath, "4");

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        private static void CallGarbageCollector()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}