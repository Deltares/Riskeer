using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    class SqLiteStorageConnectionTest
    {
        [Test]
        public void BuildConnectionString_ValidFile_ValidConnectionString()
        {
            const string validFile = "validFile";
            var connectionString = SqLiteStorageConnection.BuildSqLiteEntityConnectionString(validFile);
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(validFile, connectionString);
        }
    }
}
