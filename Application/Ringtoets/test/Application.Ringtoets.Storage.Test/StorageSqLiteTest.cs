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
        public void LoadProject_NonExistingPath_ThrowsCouldNotConnectException(string nonExistingPath)
        {
            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(nonExistingPath);

            // Assert
            CouldNotConnectException exception = Assert.Throws<CouldNotConnectException>(test);
            string expectedMessage = String.Format(@"Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.", nonExistingPath);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("empty.rtd")]
        public void LoadProject_InvalidRingtoetsFile_ThrowsStorageValidationException(string validPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, validPath);

            // Call
            TestDelegate test = () => new StorageSqLite().LoadProject(dbFile);

            // Assert
            StorageValidationException exception = Assert.Throws<StorageValidationException>(test);
            string expectedMessage = String.Format(@"Het bestand '{0}' is geen geldig Ringtoets bestand.", dbFile);
            Assert.AreEqual(expectedMessage, exception.Message);
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
            TearDownRingtoetsFile(tempFile);
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
            TearDownRingtoetsFile(tempFile);
        }

        [Test]
        public void SaveProjectAs_ValidPathToExistingFile_DoesNotThrowException()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "tempProjectFile.rtd");
            SetUpRingtoetsFile(tempFile);
            var project = new Project();
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempFile, project);

            // Assert
            Assert.DoesNotThrow(test);

            // Tear Down
            TearDownRingtoetsFile(tempFile);
        }

        [Test]
        public void SaveProjectAs_ValidPathToLockedFile_ThrowsException()
        {
            // Setup
            var tempFile = Path.Combine(testDataPath, "tempProjectFile.rtd");
            var project = new Project();
            var storage = new StorageSqLite();

            // Call
            TestDelegate test = () => storage.SaveProjectAs(tempFile, project);

            // Assert
            UpdateStorageException updateStorageException;
            using (File.Create(tempFile)) // Locks file
            {
                updateStorageException = Assert.Throws<UpdateStorageException>(test);
            }
            string expectedMessage = String.Format("Fout bij het schrijven van bestand '{0}': {1}",
                                                   tempFile,
                                                   "Een fout is opgetreden met schrijven naar het nieuwe Ringtoets bestand.");
            Assert.AreEqual(expectedMessage, updateStorageException.Message);

            // Tear Down
            TearDownRingtoetsFile(tempFile);
        }

        private void SetUpRingtoetsFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                TearDownRingtoetsFile(filePath);
            }
            using (File.Create(filePath)) {}
        }

        private void TearDownRingtoetsFile(string filePath)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            File.Delete(filePath);
        }
    }
}