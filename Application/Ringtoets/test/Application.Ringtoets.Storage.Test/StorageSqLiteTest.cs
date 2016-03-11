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
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
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
        public void LoadProject_CorruptRingtoetsFileThatPassesValidation_ThrowsStorageExceptionWithFullStackTrace()
        {
            // Setup
            SetUpTempRingtoetsFile(tempRingtoetsFile);

            var expectedMessage = String.Format(@"Fout bij het lezen van bestand '{0}': ", tempRingtoetsFile);
            var expectedInnerExceptionMessage = "An error occurred while executing the command definition. See the inner exception for details.";
            var expectedInnerExceptionInnerExceptionMessage = "SQL logic error or missing database\r\nno such table: ProjectEntity";

            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateDatabaseFile(tempRingtoetsFile, SqLiteDatabaseHelper.GetCorruptSchema());
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

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
        }

        [Test]
        public void LoadProject_ValidDatabase_ReturnsProject()
        {
            // Setup
            var projectName = Path.GetFileNameWithoutExtension(tempRingtoetsFile);
            var storage = new StorageSqLite();
            var mockRepository = new MockRepository();
            var projectMock = mockRepository.StrictMock<Project>();
            projectMock.Description = "<some description>";
            projectMock.StorageId = 1L;
            SetUpTempRingtoetsFile(tempRingtoetsFile);

            // Precondition
            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, projectMock);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating database file failed");

            // Call
            Project loadedProject = storage.LoadProject(tempRingtoetsFile);

            // Assert
            Assert.IsInstanceOf<Project>(loadedProject);
            Assert.AreEqual(1L, loadedProject.StorageId);
            Assert.AreEqual(projectName, loadedProject.Name);
            Assert.AreEqual(projectMock.Description, loadedProject.Description);

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
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
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
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
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
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
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
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
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
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
            var storage = new StorageSqLite();
            Project storedProject = new Project();

            // Precondition
            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating database file failed");

            // Call
            TestDelegate test = () => storage.SaveProject(tempRingtoetsFile, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
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

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempFile);
        }

        [Test]
        public void SaveProject_EmptyDatabaseFile_ThrowsStorageException()
        {
            // Setup
            var savedProject = new Project
            {
                StorageId = 1L
            };
            var projectWithIncorrectStorageId = new Project
            {
                StorageId = 1234L
            };
            var storage = new StorageSqLite();
            var expectedMessage = String.Format(@"Fout bij het schrijven naar bestand '{0}'{1}: {2}", tempRingtoetsFile, "",
                                                String.Format("Het object '{0}' met id '{1}' is niet gevonden.", "project", projectWithIncorrectStorageId.StorageId));

            // Precondition
            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, savedProject);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating database file failed");

            // Call
            TestDelegate test = () => storage.SaveProject(tempRingtoetsFile, projectWithIncorrectStorageId);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
        }

        [Test]
        public void SaveProject_CorruptRingtoetsFileThatPassesValidation_ThrowsStorageExceptionWithFullStackTrace()
        {
            // Setup
            SetUpTempRingtoetsFile(tempRingtoetsFile);

            var project = new Project
            {
                StorageId = 1234L
            };

            var storage = new StorageSqLite();
            var expectedMessage = String.Format(@"Fout bij het schrijven naar bestand '{0}'{1}: {2}", tempRingtoetsFile, "", "Een fout is opgetreden met het updaten van het Ringtoets bestand.");
            var expectedInnerExceptionMessage = "An error occurred while executing the command definition. See the inner exception for details.";
            var expectedInnerExceptionInnerExceptionMessage = "SQL logic error or missing database\r\nno such table: ProjectEntity";

            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateDatabaseFile(tempRingtoetsFile, SqLiteDatabaseHelper.GetCorruptSchema());
            Assert.DoesNotThrow(precondition, "Precondition failed: creating corrupt database file failed");

            // Call
            TestDelegate test = () => storage.SaveProject(tempRingtoetsFile, project);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreEqual(expectedMessage, exception.Message);

            Assert.IsInstanceOf<DataException>(exception.InnerException);
            Assert.AreEqual(expectedInnerExceptionMessage, exception.InnerException.Message);

            Assert.IsInstanceOf<SQLiteException>(exception.InnerException.InnerException);
            Assert.AreEqual(expectedInnerExceptionInnerExceptionMessage, exception.InnerException.InnerException.Message);

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
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
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
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
            Project storedProject = new Project();

            // Precondition
            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating database file failed");

            // Precondition
            Project loadedProject = storageSqLite.LoadProject(tempRingtoetsFile);

            // Call
            bool hasChanges = storageSqLite.HasChanges(loadedProject);

            // Assert
            Assert.IsFalse(hasChanges);

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
        }

        [Test]
        public void HasChanges_ValidProjectLoadedWithUnaffectedChange_ReturnsFalse()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            Project storedProject = new Project();
            var changedName = "some name";

            // Precondition
            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating database file failed");

            // Precondition
            Project loadedProject = storageSqLite.LoadProject(tempRingtoetsFile);

            // Call
            loadedProject.Name = changedName;
            bool hasChanges = storageSqLite.HasChanges(loadedProject);

            // Assert
            Assert.IsFalse(hasChanges);

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
        }

        [Test]
        public void HasChanges_ValidProjectLoadedWithAffectedChange_ReturnsTrue()
        {
            // Setup
            StorageSqLite storageSqLite = new StorageSqLite();
            Project storedProject = new Project();
            var changedDescription = "some description";

            // Precondition
            TestDelegate precondition = () => SqLiteDatabaseHelper.CreateValidRingtoetsDatabase(tempRingtoetsFile, storedProject);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating database file failed");

            // Precondition
            Project loadedProject = storageSqLite.LoadProject(tempRingtoetsFile);

            // Call
            loadedProject.Description = changedDescription;
            bool hasChanges = storageSqLite.HasChanges(loadedProject);

            // Assert
            Assert.IsTrue(hasChanges);

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
        }

        [Test]
        public void HasChanges_EmptyDatabaseFile_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var projectMock = mockRepository.StrictMock<Project>();
            projectMock.StorageId = 1234L;
            mockRepository.ReplayAll();

            SetUpTempRingtoetsFile(tempRingtoetsFile);

            var storage = new StorageSqLite();
            bool hasChanges = true;

            // Precondition, required to set the connection string
            TestDelegate precondition = () => storage.SaveProjectAs(tempRingtoetsFile, projectMock);
            Assert.DoesNotThrow(precondition, "Precondition failed: creating database file failed");

            // Call
            hasChanges = storage.HasChanges(projectMock);

            // Assert
            Assert.IsFalse(hasChanges);
            mockRepository.VerifyAll();

            // TearDown
            SqLiteDatabaseHelper.TearDownTempFile(tempRingtoetsFile);
        }

        private static void SetUpTempRingtoetsFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                SqLiteDatabaseHelper.TearDownTempFile(filePath);
            }
            using (File.Create(filePath)) {}
        }
    }
}