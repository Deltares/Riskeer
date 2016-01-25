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
        public void BuildSqLiteEntityConnectionString_InvalidPathToSqLiteFile_ThrowsArgumentNullException(string invalidPathToSqLiteFile)
        {
            // Call
            TestDelegate test = () => SqLiteStorageConnection.BuildSqLiteEntityConnectionString(invalidPathToSqLiteFile);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void BuildSqLiteEntityConnectionString_ValidPathToSqLiteFile_ValidConnectionString()
        {
            // Call
            var connectionString = SqLiteStorageConnection.BuildSqLiteEntityConnectionString(pathToSqLiteFile);

            // Assert
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(pathToSqLiteFile, connectionString);
            StringAssert.Contains(String.Format("metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;", "DbContext.RingtoetsEntities"), connectionString);
            StringAssert.Contains("provider=System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains(String.Format("data source={0}", pathToSqLiteFile), connectionString);
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
        public void BuildSqLiteConnectionString_ValidPathToSqLiteFile_ValidConnectionString()
        {
            // Call
            var connectionString = SqLiteStorageConnection.BuildSqLiteConnectionString(pathToSqLiteFile);

            // Assert
            Assert.IsNotNullOrEmpty(connectionString);
            StringAssert.Contains(pathToSqLiteFile, connectionString);
            StringAssert.DoesNotContain("metadata=", connectionString);
            StringAssert.DoesNotContain("System.Data.SQLite.EF6", connectionString);
            StringAssert.Contains("failifmissing=True", connectionString);
            StringAssert.Contains(String.Format("data source={0}", pathToSqLiteFile), connectionString);
            StringAssert.Contains("read only=False", connectionString);
            StringAssert.Contains("foreign keys=True", connectionString);
            StringAssert.Contains("version=3", connectionString);
            StringAssert.Contains("pooling=True", connectionString);
        }

        private const string pathToSqLiteFile = @"C:\SqLiteFile.sqlite";
    }
}