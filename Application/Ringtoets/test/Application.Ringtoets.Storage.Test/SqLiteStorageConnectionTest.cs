using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    class SqLiteStorageConnectionTest
    {
        [Test]
        public void BuildSqLiteEntityConnectionString_ValidFile_ValidConnectionString()
        {
            const string validFile = "validFile";
            var connectionString = SqLiteStorageConnection.BuildSqLiteEntityConnectionString(validFile);
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(validFile, connectionString);
            StringAssert.Contains("System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("DbContext.RingtoetsEntities", connectionString);
        }
        
        [Test]
        public void BuildSqLiteConnectionString_ValidFile_ValidConnectionString()
        {
            const string validFile = "validFile";
            var connectionString = SqLiteStorageConnection.BuildSqLiteConnectionString(validFile);
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(validFile, connectionString);
            StringAssert.DoesNotContain("System.Data.SQLite.EF6", connectionString);
        }
    }
}
