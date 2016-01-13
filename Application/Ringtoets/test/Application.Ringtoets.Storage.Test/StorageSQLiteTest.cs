using System.IO;
using Application.Ringtoets.Storage.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;

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
        [TestCase("NonExistingFile")]
        public void Constructor_invalidPath_throwsFileNotFoundException(string invalidPath)
        {
            // Call
            TestDelegate test = () => new StorageSqLite(invalidPath);

            // Assert
            var exception = Assert.Throws<FileNotFoundException>(test);
            Assert.AreEqual(Resources.Error_File_does_not_exist, exception.Message);
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