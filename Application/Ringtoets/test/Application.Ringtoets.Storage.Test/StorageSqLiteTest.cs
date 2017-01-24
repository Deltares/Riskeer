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
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Utils;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class StorageSqLiteTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles");
        private readonly string tempRingtoetsFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles"), "tempProjectFile.rtd");

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var storage = new StorageSqLite();

            // Assert
            Assert.IsInstanceOf<IStoreProject>(storage);
            Assert.AreEqual("Ringtoetsproject (*.rtd)|*.rtd", storage.FileFilter);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void LoadProject_InvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Setup
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidPath}': {"bestandspad mag niet leeg of ongedefinieerd zijn."}";

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(invalidPath);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void LoadProject_NonExistingPath_ThrowsStorageExceptionAndCouldNotConnectException()
        {
            // Setup
            string nonExistingPath = "fileDoesNotExist";
            string expectedMessage = $@"Fout bij het lezen van bestand '{nonExistingPath}': {"het bestand bestaat niet."}";

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(nonExistingPath);

            // Assert
            StorageException exception = Assert.Throws<CouldNotConnectException>(test);

            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void LoadProject_RingtoetsFileWithoutSchema_ThrowsStorageExceptionAndStorageValidationException()
        {
            // Setup
            string validPath = "empty.rtd";
            var tempFile = Path.Combine(testDataPath, validPath);
            string expectedMessage = $@"Fout bij het lezen van bestand '{tempFile}': {"het bestand is geen geldig Ringtoets bestand."}";

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(tempFile);

            // Assert
            StorageException exception = Assert.Throws<StorageValidationException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void LoadProject_RingtoetsFileWithTwoProjects_ThrowsStorageExceptionAndStorageValidationException()
        {
            // Setup
            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                TestDelegate precondition = () => SqLiteDatabaseHelper.CreateCompleteDatabaseFileWithoutProjectData(tempRingtoetsFile);
                Assert.DoesNotThrow(precondition, "Precondition failed: creating corrupt database file failed");

                // Call
                TestDelegate test = () => new StorageSqLite().LoadProject(tempRingtoetsFile);

                // Assert
                var expectedMessage = $@"Fout bij het lezen van bestand '{tempRingtoetsFile}': {"het bestand is geen geldig Ringtoets bestand."}";

                StorageException exception = Assert.Throws<StorageException>(test);
                Assert.IsInstanceOf<Exception>(exception);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void LoadProject_CorruptRingtoetsFileThatPassesValidation_ThrowsStorageExceptionWithFullStackTrace()
        {
            // Setup
            string expectedMessage = $@"Fout bij het lezen van bestand '{tempRingtoetsFile}': {"het bestand is geen geldig Ringtoets bestand."}";
            var expectedInnerExceptionMessage = "An error occurred while executing the command definition. See the inner exception for details.";
            var expectedInnerExceptionInnerExceptionMessage = "SQL logic error or missing database" + Environment.NewLine +
                                                              "no such table: ProjectEntity";

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                TestDelegate precondition = () => SqLiteDatabaseHelper.CreateCorruptDatabaseFile(tempRingtoetsFile);
                Assert.DoesNotThrow(precondition, "Precondition failed: creating corrupt database file failed");

                // Call
                TestDelegate test = () => new StorageSqLite().LoadProject(tempRingtoetsFile);

                // Assert
                StorageException exception = Assert.Throws<StorageException>(test);
                Assert.IsInstanceOf<Exception>(exception);
                Assert.AreEqual(expectedMessage, exception.Message);

                Assert.IsInstanceOf<DataException>(exception.InnerException);
                Assert.AreEqual(expectedInnerExceptionMessage, exception.InnerException.Message);

                Assert.IsInstanceOf<SQLiteException>(exception.InnerException.InnerException);
                Assert.AreEqual(expectedInnerExceptionInnerExceptionMessage, exception.InnerException.InnerException.Message);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void LoadProject_DatabaseWithoutVersionEntities_ThrowStorageValidationException()
        {
            // Setup
            string expectedMessage = $@"Fout bij het lezen van bestand '{tempRingtoetsFile}': {"database moet één rij in de VersionEntity tabel hebben."}";

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                TestDelegate precondition = () => SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(tempRingtoetsFile);
                Assert.DoesNotThrow(precondition, "Precondition failed: creating corrupt database file failed");

                // Call
                TestDelegate test = () => new StorageSqLite().LoadProject(tempRingtoetsFile);

                // Assert
                StorageException exception = Assert.Throws<StorageValidationException>(test);
                Assert.IsInstanceOf<Exception>(exception);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void LoadProject_DatabaseWithMultipleVersionEntities_ThrowStorageValidationException()
        {
            // Setup
            string expectedMessage = $@"Fout bij het lezen van bestand '{tempRingtoetsFile}': {"database moet één rij in de VersionEntity tabel hebben."}";
            string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                TestDelegate precondition = () =>
                {
                    SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(tempRingtoetsFile);
                    SqLiteDatabaseHelper.AddVersionEntity(tempRingtoetsFile, currentDatabaseVersion);
                    SqLiteDatabaseHelper.AddVersionEntity(tempRingtoetsFile, currentDatabaseVersion);
                };
                Assert.DoesNotThrow(precondition, "Precondition failed: creating corrupt database file failed");

                // Call
                TestDelegate test = () => new StorageSqLite().LoadProject(tempRingtoetsFile);

                // Assert
                StorageException exception = Assert.Throws<StorageValidationException>(test);
                Assert.IsInstanceOf<Exception>(exception);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(500)]
        public void LoadProject_DatabaseFromFutureVersion_ThrowStorageValidationException(int additionalVersionNumber)
        {
            // Setup
            string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            var versionCode = additionalVersionNumber + currentDatabaseVersion;
            string subMessage = $"ringtoets bestand versie '{versionCode}' is hoger dan de huidig ondersteunde versie ('{currentDatabaseVersion}'). Update Ringtoets naar een nieuwere versie.";
            string expectedMessage = $@"Fout bij het lezen van bestand '{tempRingtoetsFile}': {subMessage}";

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                TestDelegate precondition = () =>
                {
                    SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(tempRingtoetsFile);
                    SqLiteDatabaseHelper.AddVersionEntity(tempRingtoetsFile, versionCode);
                };
                Assert.DoesNotThrow(precondition, "Precondition failed: creating future database file failed");

                // Call
                TestDelegate test = () => new StorageSqLite().LoadProject(tempRingtoetsFile);

                // Assert
                StorageException exception = Assert.Throws<StorageValidationException>(test);
                Assert.IsInstanceOf<Exception>(exception);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        [TestCase("0")]
        [TestCase("-567")]
        public void LoadProject_DatabaseWithInvalidVersionCode_ThrowStorageValidationException(string versionCode)
        {
            // Setup
            string subMessage = $"ringtoets bestand versie '{versionCode}' is niet valide. Ringtoets bestand versie dient '16.4' of hoger te zijn.";
            string expectedMessage = $@"Fout bij het lezen van bestand '{tempRingtoetsFile}': {subMessage}";

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                TestDelegate precondition = () =>
                {
                    SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(tempRingtoetsFile);
                    SqLiteDatabaseHelper.AddVersionEntity(tempRingtoetsFile, versionCode);
                };
                Assert.DoesNotThrow(precondition, "Precondition failed: creating future database file failed");

                // Call
                TestDelegate test = () => new StorageSqLite().LoadProject(tempRingtoetsFile);

                // Assert
                StorageException exception = Assert.Throws<StorageValidationException>(test);
                Assert.IsInstanceOf<Exception>(exception);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void LoadProject_ValidDatabase_ReturnsProject()
        {
            // Setup
            var projectName = Path.GetFileNameWithoutExtension(tempRingtoetsFile);
            var storage = new StorageSqLite();
            var mockRepository = new MockRepository();
            var projectMock = mockRepository.StrictMock<RingtoetsProject>();
            projectMock.Description = "<some description>";

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                // Precondition
                SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, projectMock);

                // Call
                IProject loadedProject = storage.LoadProject(tempRingtoetsFile);

                // Assert
                Assert.IsInstanceOf<RingtoetsProject>(loadedProject);
                Assert.AreEqual(projectName, loadedProject.Name);
                Assert.AreEqual(projectMock.Description, loadedProject.Description);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void StageProject_ProjectIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.StageProject(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void SaveProjectAs_InvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Setup
            RingtoetsProject project = new RingtoetsProject();
            var expectedMessage = $"Fout bij het lezen van bestand '{invalidPath}': {"bestandspad mag niet leeg of ongedefinieerd zijn."}";

            var storage = new StorageSqLite();
            storage.StageProject(project);

            // Call
            TestDelegate test = () => storage.SaveProjectAs(invalidPath);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void SaveProjectAs_ValidPathToNonExistingFile_DoesNotThrowException()
        {
            // Setup
            var project = new RingtoetsProject();
            var storage = new StorageSqLite();
            storage.StageProject(project);

            try
            {
                // Precondition
                Assert.IsFalse(File.Exists(tempRingtoetsFile));

                // Call
                TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile);

                // Assert
                Assert.DoesNotThrow(test);
            }
            finally
            {
                CallGarbageCollector();
                File.Delete(tempRingtoetsFile);
            }
        }

        [Test]
        public void SaveProjectAs_ValidPathToExistingFile_DoesNotThrowException()
        {
            // Setup
            var project = new RingtoetsProject();
            var storage = new StorageSqLite();
            storage.StageProject(project);

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                // Precondition
                Assert.IsTrue(File.Exists(tempRingtoetsFile));

                // Call
                TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile);

                // Assert
                Assert.DoesNotThrow(test);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void SaveProjectAs_ValidPathToLockedFile_ThrowsUpdateStorageException()
        {
            // Setup
            var expectedMessage = $@"Kan geen tijdelijk bestand maken van het originele bestand ({tempRingtoetsFile}).";
            var project = new RingtoetsProject();
            var storage = new StorageSqLite();
            storage.StageProject(project);

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                // Call
                TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile);

                StorageException exception;
                using (File.Create(tempRingtoetsFile)) // Locks file
                {
                    exception = Assert.Throws<StorageException>(test);
                }

                // Assert
                Assert.IsInstanceOf<Exception>(exception);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
                Assert.IsInstanceOf<Exception>(exception);
                Assert.AreEqual(expectedMessage, exception.Message);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void SaveProjectAs_NoStagedProject_ThrowInvalidOperationException()
        {
            // Setup
            var storage = new StorageSqLite();

            // Precondition
            Assert.IsFalse(storage.HasStagedProject);

            // Call
            TestDelegate call = () => storage.SaveProjectAs(tempRingtoetsFile);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            Assert.AreEqual("Call 'StageProject(IProject)' first before calling this method.", message);
        }

        [Test]
        public void HasStagedProjectChanges_InvalidPath_ThrowsArgumentException()
        {
            // Setup
            var storage = new StorageSqLite();
            storage.StageProject(new RingtoetsProject());

            string path = Path.Combine(testDataPath, "ValidCharacteristics.csv");
            char[] invalidCharacters = Path.GetInvalidPathChars();
            string corruptPath = path.Replace('V', invalidCharacters[0]);

            // Call
            TestDelegate call = () => storage.HasStagedProjectChanges(corruptPath);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void HasStagedProjectChanges_NoProjectStaged_ThrowInvalidOperationException()
        {
            // Setup
            var storage = new StorageSqLite();

            // Precondition
            Assert.IsFalse(storage.HasStagedProject);

            // Call
            TestDelegate call = () => storage.HasStagedProjectChanges(null);

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            Assert.AreEqual("Call 'StageProject(IProject)' first before calling this method.", message);
        }

        [Test]
        public void HasStagedProjectChanges_NoPathGiven_ReturnsTrue()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            storageSqLite.StageProject(new RingtoetsProject());

            // Call
            bool hasChanges = storageSqLite.HasStagedProjectChanges(null);

            // Assert
            Assert.IsTrue(hasChanges);
        }

        [Test]
        public void HasStagedProjectChanges_ValidProjectLoaded_ReturnsFalse()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            RingtoetsProject storedProject = new RingtoetsProject();

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
                IProject loadedProject = storageSqLite.LoadProject(tempRingtoetsFile);
                storageSqLite.StageProject(loadedProject);

                // Call
                bool hasChanges = storageSqLite.HasStagedProjectChanges(tempRingtoetsFile);

                // Assert
                Assert.IsFalse(hasChanges);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void HasStagedProjectChanges_ValidProjectLoadedWithUnaffectedChange_ReturnsFalse()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            RingtoetsProject storedProject = new RingtoetsProject();
            var changedName = "some name";

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
                IProject loadedProject = storageSqLite.LoadProject(tempRingtoetsFile);
                storageSqLite.StageProject(loadedProject);

                // Call
                loadedProject.Name = changedName;
                bool hasChanges = storageSqLite.HasStagedProjectChanges(tempRingtoetsFile);

                // Assert
                Assert.IsFalse(hasChanges);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void HasStagedProjectChanges_ValidProjectLoadedWithAffectedChange_ReturnsTrue()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            RingtoetsProject storedProject = RingtoetsProjectTestHelper.GetFullTestProject();
            var changedDescription = "some description";

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
                IProject loadedProject = storageSqLite.LoadProject(tempRingtoetsFile);

                loadedProject.Description = changedDescription;
                storageSqLite.StageProject(loadedProject);

                // Call
                bool hasChanges = storageSqLite.HasStagedProjectChanges(tempRingtoetsFile);

                // Assert
                Assert.IsTrue(hasChanges);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
        }

        [Test]
        public void HasStagedProjectChanges_SavedToEmptyDatabaseFile_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var projectMock = mockRepository.StrictMock<RingtoetsProject>();
            mockRepository.ReplayAll();
            var storage = new StorageSqLite();

            FileDisposeHelper fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile);
            try
            {
                // Precondition, required to set the connection string
                storage.StageProject(projectMock);
                TestDelegate precondition = () => storage.SaveProjectAs(tempRingtoetsFile);
                Assert.DoesNotThrow(precondition, "Precondition failed: creating database file failed");

                storage.StageProject(projectMock);

                // Call
                var hasChanges = storage.HasStagedProjectChanges(tempRingtoetsFile);

                // Assert
                Assert.IsFalse(hasChanges);
            }
            finally
            {
                CallGarbageCollector();
                fileDisposeHelper.Dispose();
            }
            mockRepository.VerifyAll();
        }

        private static void CallGarbageCollector()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}