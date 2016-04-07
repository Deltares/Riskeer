using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.HydraRing.IO.Test
{
    [TestFixture]
    public class HydraulicDatabaseHelperTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        [Test]
        public void ValidateAndConnectTo_ExistingFileWithHlcd_ReturnsTrue()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            var result = HydraulicDatabaseHelper.ValidatePathForCalculation(validFilePath);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ValidateAndConnectTo_NonExistingFile_ReturnsFalse()
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
        public void ValidateAndConnectTo_InvalidFile_ReturnsFalse()
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
        public void ValidateAndConnectTo_FileIsDirectory_ReturnsFalse()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "/");

            // Call
            var result = HydraulicDatabaseHelper.ValidatePathForCalculation(filePath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", filePath), result);
        }

        [Test]
        public void ValidateAndConnectTo_ExistingFileWithoutHlcd_ReturnsFalse()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutHLCD", "empty.sqlite");

            // Call
            var result = HydraulicDatabaseHelper.ValidatePathForCalculation(validFilePath);

            // Assert
            StringAssert.StartsWith(string.Format("Fout bij het lezen van bestand '{0}':", validFilePath), result);
        }
 
    }
}