using System;
using System.IO;
using Core.Common.Base.Data;
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
        [TestCase("empty.rt")]
        public void Constructor_ValidPath_NewInstance(string validPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, validPath);

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");

            // Call
            TestDelegate test = () => new StorageSqLite(dbFile);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_ThrowsFileNotFoundException(string invalidPath)
        {
            // Call
            TestDelegate test = () => new StorageSqLite(invalidPath);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            string expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   invalidPath, UtilsResources.Error_Path_must_be_specified);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("NonExistingFile")]
        public void Constructor_InvalidPath_ThrowsFileNotFoundException(string invalidPath)
        {
            // Call
            TestDelegate test = () => new StorageSqLite(invalidPath);

            // Assert
            FileNotFoundException exception = Assert.Throws<FileNotFoundException>(test);
            string expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                   invalidPath, UtilsResources.Error_File_does_not_exist);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("ValidRingtoetsDatabase.rt")]
        public void TestConnection_ValidConnection_ReturnsTrue(string validPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, validPath);

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");

            // Call
            StorageSqLite storage = new StorageSqLite(dbFile);

            // Assert
            Assert.True(storage.TestConnection());
        }

        [Test]
        [TestCase("empty.rt")]
        public void TestConnection_InvalidConnection_ReturnsFalse(string invalidPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, invalidPath);

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");

            // Call
            StorageSqLite storage = new StorageSqLite(dbFile);

            // Assert
            Assert.False(storage.TestConnection());
        }

        [Test]
        [TestCase("ValidRingtoetsDatabase.rt")]
        public void LoadProject_ValidDatabase_ReturnsProject(string validPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, validPath);

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");
            var storage = new StorageSqLite(dbFile);
            Assert.True(storage.TestConnection(), "Precondition: file must be a valid Ringtoets database.");

            // Call
            Project loadedProject = storage.LoadProject();

            // Assert
            Assert.IsInstanceOf<Project>(loadedProject);
        }
    }
}