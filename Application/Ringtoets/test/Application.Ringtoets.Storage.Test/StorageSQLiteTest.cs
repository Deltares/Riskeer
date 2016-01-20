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
    }
}