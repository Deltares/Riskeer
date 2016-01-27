using System;
using System.IO;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using NUnit.Framework;
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
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void LoadProject_InvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(invalidPath);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            string expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   invalidPath, UtilsResources.Error_Path_must_be_specified);
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
            Assert.IsInstanceOf<CouldNotConnectException>(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(expectedInnerMessage, exception.InnerException.Message);
        }

        [Test]
        [TestCase("empty.rtd")]
        public void LoadProject_InvalidRingtoetsFile_ThrowsStorageExceptionAndStorageValidationException(string validPath)
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, validPath);
            string expectedInnerMessage = String.Format(@"Het bestand '{0}' is geen geldig Ringtoets bestand.", tempFile);

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(tempFile);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
            Assert.IsInstanceOf<StorageValidationException>(exception.InnerException);
            Assert.AreEqual(expectedInnerMessage, exception.InnerException.Message);
        }

        [Test]
        [TestCase("ValidRingtoetsDatabase.rtd")]
        public void LoadProject_ValidDatabase_ReturnsProject(string validPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, validPath);
            var storage = new StorageSqLite();

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");

            // Call
            Project loadedProject = storage.LoadProject(dbFile);

            // Assert
            Assert.IsInstanceOf<Project>(loadedProject);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void SaveProjectAs_InvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Setup
            Project project = new Project();
            // Call
            TestDelegate test = () => new StorageSqLite().SaveProjectAs(invalidPath, project);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            string expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   invalidPath, UtilsResources.Error_Path_must_be_specified);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void SaveProjectAs_InvalidProject_ThrowsArgumentNullException()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "tempProjectFile.rtd");
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempFile, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            // Tear Down
            TearDownTempRingtoetsFile(tempFile);
        }

        [Test]
        public void SaveProjectAs_ValidPathToNonExistingFile_DoesNotThrowException()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "tempProjectFile.rtd");
            var project = new Project();
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempFile, project);

            // Assert
            Assert.DoesNotThrow(test);

            // Tear Down
            TearDownTempRingtoetsFile(tempFile);
        }

        [Test]
        public void SaveProjectAs_ValidPathToExistingFile_DoesNotThrowException()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "tempProjectFile.rtd");
            SetUpTempRingtoetsFile(tempFile);
            var project = new Project();
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempFile, project);

            // Assert
            Assert.DoesNotThrow(test);

            // Tear Down
            TearDownTempRingtoetsFile(tempFile);
        }

        [Test]
        public void SaveProjectAs_ValidPathToLockedFile_ThrowsException()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "tempProjectFile.rtd");
            string expectedMessage = String.Format(@"Fout bij het schrijven naar bestand '{0}': Een fout is opgetreden met schrijven naar het nieuwe Ringtoets bestand.", tempFile);
            var project = new Project();
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempFile, project);

            StorageException exception;
            using (File.Create(tempFile)) // Locks file
            {
                exception = Assert.Throws<StorageException>(test);
            }

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsInstanceOf<UpdateStorageException>(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);

            // Tear Down
            TearDownTempRingtoetsFile(tempFile);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void SaveProject_InvalidPath_ThrowsArgumentException(string invalidPath)
        {
            // Setup
            Project project = new Project();
            // Call
            TestDelegate test = () => new StorageSqLite().SaveProject(invalidPath, project);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            string expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   invalidPath, UtilsResources.Error_Path_must_be_specified);
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
        public void SaveProject_InvalidProject_ThrowsCouldNotConnectException()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "tempProjectFile.rtd");
            string expectedMessage = String.Format(@"Fout bij het lezen van bestand '{0}': ", tempFile);
            string expectedInnerMessage = "Het bestand bestaat niet.";
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProject(tempFile, null);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
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

            // Call
            TestDelegate test = () => storage.SaveProject(tempFile, project);

            // Assert
            StorageException exception = Assert.Throws<StorageException>(test);
            string expectedMessage = String.Format(@"Fout bij het schrijven naar bestand '{0}'{1}: {2}", tempFile, "",
                                                   String.Format("Het object '{0}' met id '{1}' is niet gevonden.", "project", project.StorageId));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void SaveProject_ValidPathToSetFilePath_DoesNotThrowException()
        {
            // Setup
            long projectId = 1234L;
            var tempFile = Path.Combine(testDataPath, "tempProjectFile.rtd");
            var project = new Project()
            {
                StorageId = projectId
            };
            var storage = new StorageSqLite();
            TestDelegate precondition = () => storage.SaveProjectAs(tempFile, project);
            Assert.DoesNotThrow(precondition, String.Format("Precondition: file '{0}' must be a valid Ringtoets database file.", tempFile));

            // Call
            TestDelegate test = () => storage.SaveProject(tempFile, project);

            // Assert
            Assert.DoesNotThrow(test);

            // TearDown
            TearDownTempRingtoetsFile(tempFile);
        }

        [Test]
        public void SaveProjectLoadProject_ValidPathToSetFilePath_ValidContentsFromDatabase()
        {
            // Setup
            long projectId = 1234L;
            var tempFile = Path.Combine(testDataPath, "tempProjectFile.rtd");
            var project = new Project()
            {
                StorageId = projectId,
                Name = "test",
                Description = "description"
            };
            var storage = new StorageSqLite();
            TestDelegate precondition = () => storage.SaveProjectAs(tempFile, project);
            Assert.DoesNotThrow(precondition, String.Format("Precondition: file '{0}' must be a valid Ringtoets database file.", tempFile));

            // Call
            TestDelegate test = () => storage.SaveProject(tempFile, project);

            // Assert
            Assert.DoesNotThrow(test, String.Format("Precondition: failed to save project to file '{0}'.", tempFile));

            // Call
            Project loadedProject = storage.LoadProject(tempFile);

            // Assert
            Assert.IsInstanceOf<Project>(loadedProject);
            Assert.AreNotEqual(project, loadedProject);
            Assert.AreEqual(project.StorageId, loadedProject.StorageId);
            Assert.AreEqual(project.Name, loadedProject.Name);
            Assert.AreEqual(project.Description, loadedProject.Description);

            // TearDown
            TearDownTempRingtoetsFile(tempFile);
        }

        private void SetUpTempRingtoetsFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                TearDownTempRingtoetsFile(filePath);
            }
            using (File.Create(filePath)) {}
        }

        private void TearDownTempRingtoetsFile(string filePath)
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