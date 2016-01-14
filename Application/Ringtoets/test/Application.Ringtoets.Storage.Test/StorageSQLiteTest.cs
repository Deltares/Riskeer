using System;
using System.IO;
using Application.Ringtoets.Storage.Exceptions;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
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
        public void Constructor_validPath_NewInstance(string validPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, validPath);

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");

            // Call
            TestDelegate test = () => new StorageSqLite(dbFile);
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_FileNullOrEmpty_throwsFileNotFoundException(string invalidPath)
        {
            // Call
            TestDelegate test = () => new StorageSqLite(invalidPath);

            // Assert
            var exception = Assert.Throws<InvalidFileException>(test);
            var expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                invalidPath, UtilsResources.Error_Path_must_be_specified);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
        
        [Test]
        [TestCase("NonExistingFile")]
        public void Constructor_invalidPath_throwsFileNotFoundException(string invalidPath)
        {
            // Call
            TestDelegate test = () => new StorageSqLite(invalidPath);

            // Assert
            var exception = Assert.Throws<FileNotFoundException>(test);
            var expectedMessage = new FileReaderErrorMessageBuilder(invalidPath).Build(UtilsResources.Error_File_does_not_exist);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("ValidRingtoetsDatabase.rt")]
        public void TestConnection_validConnection_ReturnsTrue(string validPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, validPath);

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");

            // Call
            var storage = new StorageSqLite(dbFile);
            Assert.True(storage.TestConnection());
        }

        [Test]
        [TestCase("ValidRingtoetsDatabase.rt")]
        public void LoadProject_validDatabase_ReturnsProject(string validPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, validPath);

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");

            // Call
            var storage = new StorageSqLite(dbFile);
            Assert.True(storage.TestConnection(), "Precondition: file must be a valid Ringtoets database.");

            var loadedProject = storage.LoadProject();
            Assert.IsInstanceOf<Project>(loadedProject);
        }

        [Test]
        [TestCase("empty.rt")]
        public void TestConnection_invalidConnection_ReturnsFalse(string invalidPath)
        {
            // Setup
            var dbFile = Path.Combine(testDataPath, invalidPath);

            // Precondition
            Assert.IsTrue(File.Exists(dbFile), "Precondition: file must exist.");

            // Call
            var storage = new StorageSqLite(dbFile);
            Assert.False(storage.TestConnection());
        }
    }
}