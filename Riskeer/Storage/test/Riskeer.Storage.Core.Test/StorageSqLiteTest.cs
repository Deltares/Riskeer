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
using System.Data;
using System.Data.SQLite;
using System.IO;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Util;
using Riskeer.Integration.Data;
using Riskeer.Storage.Core.TestUtil;

namespace Riskeer.Storage.Core.Test
{
    [TestFixture]
    public class StorageSqLiteTest
    {
        private readonly string testPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Storage.Core, "DatabaseFiles");
        private readonly string workingDirectory = TestHelper.GetScratchPadPath(nameof(StorageSqLiteTest));
        private DirectoryDisposeHelper directoryDisposeHelper;

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
            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(invalidPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual($"Fout bij het lezen van bestand '{invalidPath}': bestandspad mag niet leeg of ongedefinieerd zijn.",
                            exception.Message);
        }

        [Test]
        public void LoadProject_NonExistingPath_ThrowsStorageExceptionAndCouldNotConnectException()
        {
            // Setup
            const string nonExistingPath = "fileDoesNotExist";

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(nonExistingPath);

            // Assert
            StorageException exception = Assert.Throws<CouldNotConnectException>(test);

            Assert.AreEqual($@"Fout bij het lezen van bestand '{nonExistingPath}': het bestand bestaat niet.",
                            exception.Message);
        }

        [Test]
        public void LoadProject_RingtoetsFileWithoutSchema_ThrowsStorageExceptionAndStorageValidationException()
        {
            // Setup
            const string validPath = "empty.rtd";
            string tempFile = Path.Combine(testPath, validPath);

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(tempFile);

            // Assert
            StorageException exception = Assert.Throws<StorageValidationException>(test);
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempFile}': het bestand is geen geldig Ringtoets bestand.",
                            exception.Message);
        }

        [Test]
        public void LoadProject_RingtoetsFileWithTwoProjects_ThrowsStorageExceptionAndStorageValidationException()
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(LoadProject_RingtoetsFileWithTwoProjects_ThrowsStorageExceptionAndStorageValidationException));

            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateCompleteDatabaseFileWithoutProjectData(tempRingtoetsFile);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating corrupt database file failed");

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(tempRingtoetsFile);

            // Assert
            string expectedMessage = $@"Fout bij het lezen van bestand '{tempRingtoetsFile}': het bestand is geen geldig Ringtoets bestand.";

            var exception = Assert.Throws<StorageException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void LoadProject_CorruptRingtoetsFileThatPassesValidation_ThrowsStorageExceptionWithFullStackTrace()
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(LoadProject_CorruptRingtoetsFileThatPassesValidation_ThrowsStorageExceptionWithFullStackTrace));
            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateCorruptDatabaseFile(tempRingtoetsFile);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating corrupt database file failed");

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(tempRingtoetsFile);

            // Assert
            var exception = Assert.Throws<StorageException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempRingtoetsFile}': het bestand is geen geldig Ringtoets bestand.",
                            exception.Message);

            Assert.IsInstanceOf<DataException>(exception.InnerException);
            Assert.AreEqual("An error occurred while executing the command definition. See the inner exception for details.",
                            exception.InnerException.Message);

            Assert.IsInstanceOf<SQLiteException>(exception.InnerException.InnerException);
            Assert.AreEqual($"SQL logic error{Environment.NewLine}"
                            + "no such table: AssessmentSectionEntity", exception.InnerException.InnerException.Message);
        }

        [Test]
        public void LoadProject_DatabaseWithoutVersionEntities_ThrowStorageValidationException()
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(LoadProject_DatabaseWithoutVersionEntities_ThrowStorageValidationException));

            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateCompleteDatabaseFileEmpty(tempRingtoetsFile);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating corrupt database file failed");

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(tempRingtoetsFile);

            // Assert
            StorageException exception = Assert.Throws<StorageValidationException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempRingtoetsFile}': database moet één rij in de VersionEntity tabel hebben.",
                            exception.Message);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        [Test]
        public void LoadProject_DatabaseWithMultipleVersionEntities_ThrowStorageValidationException()
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(LoadProject_DatabaseWithMultipleVersionEntities_ThrowStorageValidationException));
            string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

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
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempRingtoetsFile}': database moet één rij in de VersionEntity tabel hebben.",
                            exception.Message);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        [Test]
        [TestCase(1)]
        [TestCase(500)]
        public void LoadProject_DatabaseFromFutureVersion_ThrowStorageValidationException(int additionalVersionNumber)
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, $"{nameof(LoadProject_DatabaseFromFutureVersion_ThrowStorageValidationException)}_{Path.GetRandomFileName()}");
            string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();
            string versionCode = additionalVersionNumber + currentDatabaseVersion;

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
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempRingtoetsFile}': ringtoets "
                            + $"bestand versie '{versionCode}' is hoger dan de huidig ondersteunde versie "
                            + $"('{currentDatabaseVersion}'). Update Ringtoets naar een nieuwere versie.",
                            exception.Message);
        }

        [Test]
        [TestCase("0")]
        [TestCase("-567")]
        public void LoadProject_DatabaseWithInvalidVersionCode_ThrowStorageValidationException(string versionCode)
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, $"{nameof(LoadProject_DatabaseWithInvalidVersionCode_ThrowStorageValidationException)}_{Path.GetRandomFileName()}");

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
            Assert.AreEqual($@"Fout bij het lezen van bestand '{tempRingtoetsFile}': ringtoets "
                            + $"bestand versie '{versionCode}' is niet valide. De versie van het Ringtoets projectbestand "
                            + "dient '16.4' of hoger te zijn.", exception.Message);
        }

        [Test]
        public void LoadProject_ValidDatabase_ReturnsProject()
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(LoadProject_ValidDatabase_ReturnsProject));
            string projectName = Path.GetFileNameWithoutExtension(tempRingtoetsFile);
            var storage = new StorageSqLite();
            var mockRepository = new MockRepository();
            var project = mockRepository.StrictMock<RingtoetsProject>();
            project.Description = "<some description>";

            // Precondition
            SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, project);

            // Call
            IProject loadedProject = storage.LoadProject(tempRingtoetsFile);

            // Assert
            Assert.IsInstanceOf<RingtoetsProject>(loadedProject);
            Assert.AreEqual(projectName, loadedProject.Name);
            Assert.AreEqual(project.Description, loadedProject.Description);
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
            var project = new RingtoetsProject();

            var storage = new StorageSqLite();
            storage.StageProject(project);

            // Call
            TestDelegate test = () => storage.SaveProjectAs(invalidPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual($"Fout bij het lezen van bestand '{invalidPath}': bestandspad mag niet "
                            + "leeg of ongedefinieerd zijn.", exception.Message);
        }

        [Test]
        public void SaveProjectAs_ValidPathToNonExistingFile_DoesNotThrowException()
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(SaveProjectAs_ValidPathToNonExistingFile_DoesNotThrowException));
            var project = new RingtoetsProject();
            var storage = new StorageSqLite();
            storage.StageProject(project);

            // Precondition
            Assert.IsFalse(File.Exists(tempRingtoetsFile));

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void SaveProjectAs_ValidPathToExistingFile_DoesNotThrowException()
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(SaveProjectAs_ValidPathToExistingFile_DoesNotThrowException));
            var project = new RingtoetsProject();
            var storage = new StorageSqLite();
            storage.StageProject(project);

            using (File.Create(tempRingtoetsFile)) {}

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void SaveProjectAs_ValidPathToLockedFile_ThrowsUpdateStorageException()
        {
            // Setup
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(SaveProjectAs_ValidPathToLockedFile_ThrowsUpdateStorageException));
            var project = new RingtoetsProject();
            var storage = new StorageSqLite();
            storage.StageProject(project);

            using (var fileDisposeHelper = new FileDisposeHelper(tempRingtoetsFile))
            {
                try
                {
                    fileDisposeHelper.LockFiles();

                    // Call
                    TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile);

                    // Assert
                    var exception = Assert.Throws<StorageException>(test);

                    Assert.IsInstanceOf<Exception>(exception);
                    Assert.IsInstanceOf<IOException>(exception.InnerException);
                    Assert.IsInstanceOf<Exception>(exception);
                    Assert.AreEqual($@"Kan geen tijdelijk bestand maken van het originele bestand ({tempRingtoetsFile}).",
                                    exception.Message);
                }
                finally
                {
                    CallGarbageCollector();
                }
            }
        }

        [Test]
        public void SaveProjectAs_NoStagedProject_ThrowInvalidOperationException()
        {
            // Setup
            string tempRingtoetsFile = TestHelper.GetScratchPadPath(nameof(SaveProjectAs_NoStagedProject_ThrowInvalidOperationException));
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

            string path = Path.Combine(testPath, "ValidCharacteristics.csv");
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
            var storageSqLite = new StorageSqLite();
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
            var storageSqLite = new StorageSqLite();
            var storedProject = new RingtoetsProject();
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(HasStagedProjectChanges_ValidProjectLoaded_ReturnsFalse));

            SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
            IProject loadedProject = storageSqLite.LoadProject(tempRingtoetsFile);
            storageSqLite.StageProject(loadedProject);

            // Call
            bool hasChanges = storageSqLite.HasStagedProjectChanges(tempRingtoetsFile);

            // Assert
            Assert.IsFalse(hasChanges);
        }

        [Test]
        public void HasStagedProjectChanges_ValidProjectLoadedWithUnaffectedChange_ReturnsFalse()
        {
            // Setup
            var storageSqLite = new StorageSqLite();
            var storedProject = new RingtoetsProject();
            const string changedName = "some name";
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(HasStagedProjectChanges_ValidProjectLoadedWithUnaffectedChange_ReturnsFalse));

            SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
            IProject loadedProject = storageSqLite.LoadProject(tempRingtoetsFile);
            storageSqLite.StageProject(loadedProject);

            // Call
            loadedProject.Name = changedName;
            bool hasChanges = storageSqLite.HasStagedProjectChanges(tempRingtoetsFile);

            // Assert
            Assert.IsFalse(hasChanges);
        }

        [Test]
        public void HasStagedProjectChanges_ValidProjectLoadedWithAffectedChange_ReturnsTrue()
        {
            // Setup
            var storageSqLite = new StorageSqLite();
            RingtoetsProject storedProject = RiskeerProjectTestHelper.GetFullTestProject();
            const string changedDescription = "some description";
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(HasStagedProjectChanges_ValidProjectLoadedWithAffectedChange_ReturnsTrue));

            SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
            IProject loadedProject = storageSqLite.LoadProject(tempRingtoetsFile);

            loadedProject.Description = changedDescription;
            storageSqLite.StageProject(loadedProject);

            // Call
            bool hasChanges = storageSqLite.HasStagedProjectChanges(tempRingtoetsFile);

            // Assert
            Assert.IsTrue(hasChanges);
        }

        [Test]
        public void HasStagedProjectChanges_SavedToEmptyDatabaseFile_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var project = mockRepository.StrictMock<RingtoetsProject>();
            mockRepository.ReplayAll();
            var storage = new StorageSqLite();
            string tempRingtoetsFile = Path.Combine(workingDirectory, nameof(HasStagedProjectChanges_SavedToEmptyDatabaseFile_ReturnsFalse));

            // Precondition, required to set the connection string
            storage.StageProject(project);
            TestDelegate precondition = () => storage.SaveProjectAs(tempRingtoetsFile);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating database file failed");

            storage.StageProject(project);

            // Call
            bool hasChanges = storage.HasStagedProjectChanges(tempRingtoetsFile);

            // Assert
            Assert.IsFalse(hasChanges);

            mockRepository.VerifyAll();
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(StorageSqLiteTest));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            CallGarbageCollector();
            directoryDisposeHelper.Dispose();
        }

        private static void CallGarbageCollector()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}