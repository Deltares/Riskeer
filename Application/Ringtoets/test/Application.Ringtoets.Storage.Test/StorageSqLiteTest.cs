using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class StorageSqLiteTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles");
        private readonly string tempRingtoetsFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Application.Ringtoets.Storage, "DatabaseFiles"), "tempProjectFile.rtd");

        [TestFixtureTearDown]
        public void TearDownTempRingtoetsFile()
        {
            TearDownTempRingtoetsFile(tempRingtoetsFile);
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup

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
            string expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   invalidPath, UtilsResources.Error_Path_must_be_specified);

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(invalidPath);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("fileDoesNotExist")]
        public void LoadProject_NonExistingPath_ThrowsStorageExceptionAndCouldNotConnectException(string nonExistingPath)
        {
            // SetUp
            string expectedMessage = String.Format(@"Fout bij het lezen van bestand '{0}': ", nonExistingPath);
            string expectedInnerMessage = "Het bestand bestaat niet.";

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(nonExistingPath);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);

            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<CouldNotConnectException>(exception.InnerException);
            Assert.AreEqual(expectedInnerMessage, exception.InnerException.Message);
        }

        [Test]
        [TestCase("empty.rtd")]
        public void LoadProject_InvalidRingtoetsFile_ThrowsStorageExceptionAndStorageValidationException(string validPath)
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, validPath);
            var expectedInnerMessage = String.Format(@"Het bestand '{0}' is geen geldig Ringtoets bestand.", tempFile);

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(tempFile);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
            Assert.IsInstanceOf<StorageValidationException>(exception.InnerException);
            Assert.AreEqual(expectedInnerMessage, exception.InnerException.Message);
        }

        [Test]
        [TestCase("corruptRingtoetsDatabase.rtd")]
        public void LoadProject_CorruptRingtoetsFileThatPassesValidation_ThrowsStorageExceptionWithFullStackTrace(string validPath)
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, validPath);
            var expectedMessage = String.Format(@"Fout bij het lezen van bestand '{0}': ", tempFile);
            var expectedInnerExceptionMessage = "An error occurred while executing the command definition. See the inner exception for details.";
            var expectedInnerExceptionInnerExceptionMessage = "SQL logic error or missing database\r\nno such table: ProjectEntity";

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(tempFile);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);

            Assert.IsInstanceOf<DataException>(exception.InnerException);
            Assert.AreEqual(expectedInnerExceptionMessage, exception.InnerException.Message);

            Assert.IsInstanceOf<SQLiteException>(exception.InnerException.InnerException);
            Assert.AreEqual(expectedInnerExceptionInnerExceptionMessage, exception.InnerException.InnerException.Message);
        }

        [Test]
        [TestCase("ValidRingtoetsDatabase.rtd")]
        public void LoadProject_ValidDatabase_ReturnsProject(string validPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, validPath);
            var projectName = Path.GetFileNameWithoutExtension(validPath);
            var storage = new StorageSqLite();

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");

            // Call
            Project loadedProject = storage.LoadProject(dbFile);

            // Assert
            Assert.IsInstanceOf<Project>(loadedProject);
            Assert.AreEqual(1, loadedProject.StorageId);
            Assert.AreEqual(projectName, loadedProject.Name);
            Assert.AreEqual("Test description", loadedProject.Description);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void SaveProjectAs_InvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Setup
            Project project = new Project();
            var expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                invalidPath, UtilsResources.Error_Path_must_be_specified);

            // Call
            TestDelegate test = () => new StorageSqLite().SaveProjectAs(invalidPath, project);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void SaveProjectAs_InvalidProject_ThrowsArgumentNullException()
        {
            // Setup
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            // Tear Down
            TearDownTempRingtoetsFile(tempRingtoetsFile);
        }

        [Test]
        public void SaveProjectAs_ValidPathToNonExistingFile_DoesNotThrowException()
        {
            // Setup
            var project = new Project();
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile, project);

            // Assert
            Assert.DoesNotThrow(test);

            // Tear Down
            TearDownTempRingtoetsFile(tempRingtoetsFile);
        }

        [Test]
        public void SaveProjectAs_ValidPathToExistingFile_DoesNotThrowException()
        {
            // Setup
            SetUpTempRingtoetsFile(tempRingtoetsFile);
            var project = new Project();
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile, project);

            // Assert
            Assert.DoesNotThrow(test);

            // Tear Down
            TearDownTempRingtoetsFile(tempRingtoetsFile);
        }

        [Test]
        public void SaveProjectAs_ValidPathToLockedFile_ThrowsUpdateStorageException()
        {
            // Setup
            var expectedMessage = String.Format(@"Fout bij het schrijven naar bestand '{0}': Een fout is opgetreden met schrijven naar het nieuwe Ringtoets bestand.", tempRingtoetsFile);
            var project = new Project();
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempRingtoetsFile, project);

            StorageException exception;
            using (File.Create(tempRingtoetsFile)) // Locks file
            {
                exception = Assert.Throws<StorageException>(test);
            }

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsInstanceOf<UpdateStorageException>(exception.InnerException);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);

            // Tear Down
            TearDownTempRingtoetsFile(tempRingtoetsFile);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void SaveProject_InvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Setup
            Project project = new Project();
            var expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                invalidPath, UtilsResources.Error_Path_must_be_specified);

            // Call
            TestDelegate test = () => new StorageSqLite().SaveProject(invalidPath, project);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void SaveProject_InvalidProject_ThrowsArgumentNullException()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "ValidRingtoetsDatabase.rtd");
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProject(tempFile, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void SaveProject_ValidProjectNonExistingPath_ThrowsStorageExceptionAndCouldNotConnectException()
        {
            // Setup
            var project = new Project
            {
                StorageId = 1234L
            };
            var tempFile = Path.Combine(testDataPath, "DoesNotExist.rtd");
            var expectedMessage = String.Format(@"Fout bij het lezen van bestand '{0}': ", tempFile);
            var expectedInnerMessage = "Het bestand bestaat niet.";
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProject(tempFile, project);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);

            Assert.IsInstanceOf<CouldNotConnectException>(exception.InnerException);
            Assert.AreEqual(expectedInnerMessage, exception.InnerException.Message);

            // Tear Down
            TearDownTempRingtoetsFile(tempFile);
        }

        [Test]
        public void SaveProject_EmptyDatabaseFile_ThrowsStorageException()
        {
            // Setup
            var project = new Project
            {
                StorageId = 1234L
            };
            var tempFile = Path.Combine(testDataPath, "ValidRingtoetsDatabase.rtd");
            var storage = new StorageSqLite();
            var expectedMessage = String.Format(@"Fout bij het schrijven naar bestand '{0}'{1}: {2}", tempFile, "",
                                                String.Format("Het object '{0}' met id '{1}' is niet gevonden.", "project", project.StorageId));

            // Call
            TestDelegate test = () => storage.SaveProject(tempFile, project);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("corruptRingtoetsDatabase.rtd")]
        public void SaveProject_CorruptRingtoetsFileThatPassesValidation_ThrowsStorageExceptionWithFullStackTrace(string validPath)
        {
            // Setup
            var project = new Project
            {
                StorageId = 1234L
            };
            var tempFile = Path.Combine(testDataPath, validPath);
            var storage = new StorageSqLite();
            var expectedMessage = String.Format(@"Fout bij het schrijven naar bestand '{0}'{1}: {2}", tempFile, "", "Een fout is opgetreden met het updaten van het Ringtoets bestand.");
            var expectedInnerExceptionMessage = "An error occurred while executing the command definition. See the inner exception for details.";
            var expectedInnerExceptionInnerExceptionMessage = "SQL logic error or missing database\r\nno such table: ProjectEntity";

            // Call
            TestDelegate test = () => storage.SaveProject(tempFile, project);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);

            Assert.IsInstanceOf<DataException>(exception.InnerException);
            Assert.AreEqual(expectedInnerExceptionMessage, exception.InnerException.Message);

            Assert.IsInstanceOf<SQLiteException>(exception.InnerException.InnerException);
            Assert.AreEqual(expectedInnerExceptionInnerExceptionMessage, exception.InnerException.InnerException.Message);
        }

        [Test]
        public void SaveProject_ValidPathToSetFilePath_DoesNotThrowException()
        {
            // Setup
            long projectId = 1234L;
            var projectName = Path.GetFileNameWithoutExtension(tempRingtoetsFile);

            var project = new Project()
            {
                StorageId = projectId
            };
            var storage = new StorageSqLite();
            TestDelegate precondition = () => storage.SaveProjectAs(tempRingtoetsFile, project);
            Assert.DoesNotThrow(precondition, String.Format("Precondition: file '{0}' must be a valid Ringtoets database file.", tempRingtoetsFile));

            // Call
            TestDelegate test = () => storage.SaveProject(tempRingtoetsFile, project);

            // Assert
            Assert.DoesNotThrow(test);
            Assert.AreEqual(projectName, project.Name);

            // TearDown
            TearDownTempRingtoetsFile(tempRingtoetsFile);
        }

        [Test]
        public void HasChanges_NoConnectionSet_ReturnsTrue()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            var mockRepository = new MockRepository();
            var projectMock = mockRepository.StrictMock<Project>();
            mockRepository.ReplayAll();

            // Call
            bool hasChanges = storageSqLite.HasChanges(projectMock);

            // Assert
            Assert.IsTrue(hasChanges);
            mockRepository.VerifyAll();
        }

        [Test]
        public void HasChanges_ValidProjectLoaded_ReturnsFalse()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            var dbFile = Path.Combine(testDataPath, "ValidRingtoetsDatabase.rtd");

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");
            Project loadedProject = storageSqLite.LoadProject(dbFile);

            // Call
            bool hasChanges = storageSqLite.HasChanges(loadedProject);

            // Assert
            Assert.IsFalse(hasChanges);
        }

        [Test]
        public void HasChanges_ValidProjectLoadedWithUnaffectedChange_ReturnsFalse()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            var dbFile = Path.Combine(testDataPath, "ValidRingtoetsDatabase.rtd");
            var changedName = "some name";

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");
            Project loadedProject = storageSqLite.LoadProject(dbFile);

            // Call
            loadedProject.Name = changedName;
            bool hasChanges = storageSqLite.HasChanges(loadedProject);

            // Assert
            Assert.IsFalse(hasChanges);
        }

        [Test]
        public void HasChanges_ValidProjectLoadedWithAffectedChange_ReturnsTrue()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            var dbFile = Path.Combine(testDataPath, "ValidRingtoetsDatabase.rtd");
            var changedDescription = "some description";

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");
            Project loadedProject = storageSqLite.LoadProject(dbFile);

            // Call
            loadedProject.Description = changedDescription;
            bool hasChanges = storageSqLite.HasChanges(loadedProject);

            // Assert
            Assert.IsTrue(hasChanges);
        }

        [Test]
        public void HasChanges_EmptyDatabaseFile_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var projectMock = mockRepository.StrictMock<Project>();
            projectMock.StorageId = 1234L;
            mockRepository.ReplayAll();

            var tempFile = Path.Combine(testDataPath, "ValidRingtoetsDatabase.rtd");
            var storage = new StorageSqLite();
            bool hasChanges = true;

            // Precondition, ignore return value
            TestDelegate loadProjectToSetFilePath = () => storage.LoadProject(tempFile);
            Assert.DoesNotThrow(loadProjectToSetFilePath, "Precondition failed: LoadProject failed");

            // Call
            hasChanges = storage.HasChanges(projectMock);

            // Assert
            Assert.IsFalse(hasChanges);
            mockRepository.VerifyAll();
        }

        private static void SetUpTempRingtoetsFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                TearDownTempRingtoetsFile(filePath);
            }
            using (File.Create(filePath)) {}
        }

        private static void TearDownTempRingtoetsFile(string filePath)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}