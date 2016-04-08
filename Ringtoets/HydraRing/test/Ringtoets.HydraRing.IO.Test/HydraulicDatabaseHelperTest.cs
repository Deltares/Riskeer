using System;
using System.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HydraRing.IO.Test
{
    [TestFixture]
    public class HydraulicDatabaseHelperTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        [Test]
        public void ValidatePathForCalculation_ExistingFileWithHlcd_ReturnsNull()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            var result = HydraulicDatabaseHelper.ValidatePathForCalculation(validFilePath);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ValidatePathForCalculation_NonExistingFile_ReturnsMessageWithError()
        {
            // Setup
            var nonexistingSqlite = "nonexisting.sqlite";
            string filePath = Path.Combine(testDataPath, nonexistingSqlite);

            // Call
            var result = HydraulicDatabaseHelper.ValidatePathForCalculation(filePath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", filePath), result);
        }

        [Test]
        public void ValidatePathForCalculation_InvalidFile_ReturnsMessageWithError()
        {
            // Setup
            var invalidPath = Path.Combine(testDataPath, "complete.sqlite");
            invalidPath = invalidPath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            var result = HydraulicDatabaseHelper.ValidatePathForCalculation(invalidPath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", invalidPath), result);
        }

        [Test]
        public void ValidatePathForCalculation_FileIsDirectory_ReturnsMessageWithError()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "/");

            // Call
            var result = HydraulicDatabaseHelper.ValidatePathForCalculation(filePath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", filePath), result);
        }

        [Test]
        public void ValidatePathForCalculation_ExistingFileWithoutHlcd_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutHLCD", "empty.sqlite");

            // Call
            var result = HydraulicDatabaseHelper.ValidatePathForCalculation(validFilePath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", validFilePath), result);
        }

        [Test]
        public void HaveEqualVersion_InvalidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            var invalidPath = Path.Combine(testDataPath, "complete.sqlite");
            invalidPath = invalidPath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            TestDelegate test = () => HydraulicDatabaseHelper.HaveEqualVersion(new HydraulicBoundaryDatabase(), invalidPath);

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void HaveEqualVersion_PathNotSet_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraulicDatabaseHelper.HaveEqualVersion(new HydraulicBoundaryDatabase(), null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("pathToDatabase", parameter);
        }

        [Test]
        public void HaveEqualVersion_DatabaseNotSet_ThrowsArgumentNullException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            TestDelegate test = () => HydraulicDatabaseHelper.HaveEqualVersion(null, validFilePath);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("database", parameter);
        }

        [Test]
        public void HaveEqualVersion_ValidFileEqualVersion_ReturnsTrue()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var database = new HydraulicBoundaryDatabase()
            {
                Version = "Dutch coast South19-11-2015 12:00"
            };

            // Call
            bool isEqual = HydraulicDatabaseHelper.HaveEqualVersion(database, validFilePath);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void HaveEqualVersion_ValidFileDifferentVersion_ReturnsFalse()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var database = new HydraulicBoundaryDatabase()
            {
                Version = "Dutch coast South19-11-2015 12:01"
            };

            // Call
            bool isEqual = HydraulicDatabaseHelper.HaveEqualVersion(database, validFilePath);

            // Assert
            Assert.IsFalse(isEqual);
        }
    }
}