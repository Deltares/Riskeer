using System;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class SqLiteStorageConnectionTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void BuildSqLiteEntityConnectionString_InvalidPath_ThrowsArgumentNullException(string invalidPath)
        {
            // Call
            TestDelegate test = () => SqLiteStorageConnection.BuildSqLiteEntityConnectionString(invalidPath);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void BuildSqLiteEntityConnectionString_ValidFile_ValidConnectionString()
        {
            // Call
            var connectionString = SqLiteStorageConnection.BuildSqLiteEntityConnectionString(validFile);

            // Assert
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(validFile, connectionString);
            StringAssert.Contains(String.Format("metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;", "DbContext.RingtoetsEntities"), connectionString);
            StringAssert.Contains("provider=System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains(String.Format("data source={0}", validFile), connectionString);
            StringAssert.Contains("read only=False", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=True", connectionString);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void BuildSqLiteConnectionString_InvalidPath_ThrowsArgumentNullException(string invalidPath)
        {
            // Call
            TestDelegate test = () => SqLiteStorageConnection.BuildSqLiteConnectionString(invalidPath);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void BuildSqLiteConnectionString_ValidFile_ValidConnectionString()
        {
            // Call
            var connectionString = SqLiteStorageConnection.BuildSqLiteConnectionString(validFile);

            // Assert
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(validFile, connectionString);
            StringAssert.DoesNotContain("metadata=", connectionString);
            StringAssert.DoesNotContain("System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains(String.Format("data source={0}", validFile), connectionString);
            StringAssert.Contains("read only=False", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=True", connectionString);
        }

        private const string validFile = "validFile";
    }
}