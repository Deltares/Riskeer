using System;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Utils.Test
{
    [TestFixture]
    public class FileUtilsTest
    {
        [Test]
        public void ValidateFilePath_ValidPath_DoesNotThrowAnyExceptions()
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "validFile.txt");

            // Call
            TestDelegate call = () => FileUtils.ValidateFilePath(path);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ValidateFilePath_InvalidEmptyPath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            TestDelegate call = () => FileUtils.ValidateFilePath(invalidPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet leeg of ongedefinieerd zijn.", invalidPath);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ValidateFilePath_PathContainingInvalidFileCharacters_ThrowsArgumentException()
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "validFile.txt");
            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            var invalidPath = path.Replace('d', invalidFileNameChars[0]);

            // Call
            TestDelegate call = () => FileUtils.ValidateFilePath(invalidPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidPath, string.Join(", ", invalidFileNameChars));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ValidateFilePath_PathIsActuallyFolder_ThrowsArgumentException()
        {
            // Setup
            var folderPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils) + Path.DirectorySeparatorChar;

            // Call
            TestDelegate call = () => FileUtils.ValidateFilePath(folderPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.", folderPath);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase(null, "FileToCompare_Original.txt")]
        [TestCase("", "FileToCompare_Original.txt")]
        [TestCase("   ", "FileToCompare_Original.txt")]
        [TestCase("FileToCompare_Original.txt", null)]
        [TestCase("FileToCompare_Original.txt", "")]
        [TestCase("FileToCompare_Original.txt", "   ")]
        public void CompareFiles_InvalidPaths_ThrowsArgumentxception(string pathA, string pathB)
        {
            // Setup
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                ("FileToCompare_Original.txt" == pathB) ? pathA : pathB);

            // Call
            TestDelegate test = () => FileUtils.CompareFiles(pathA, pathB);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCase("DoesNotExist", "FileToCompare_Original.txt")]
        [TestCase("FileToCompare_Original.txt", "DoesNotExist")]
        public void CompareFiles_NonExistingPaths_ThrowsArgumentException(string pathA, string pathB)
        {
            // Setup
            const string expectedMessage = "Er is een onverwachte fout opgetreden tijdens het inlezen van het bestand.";

            // Call
            TestDelegate test = () => FileUtils.CompareFiles(pathA, pathB);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void CompareFiles_SameFiles_ReturnsTrue()
        {
            // Setup
            var pathA = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "FileToCompare_Original.txt");
            var pathB = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "FileToCompare_Equal.txt");

            // Call
            bool areEqual = FileUtils.CompareFiles(pathA, pathB);

            // Assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void CompareFiles_EqualFiles_ReturnsTrue()
        {
            // Setup
            var pathA = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "FileToCompare_Original.txt");
            var pathB = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "FileToCompare_Equal.txt");

            // Call
            bool areEqual = FileUtils.CompareFiles(pathA, pathB);

            // Assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void CompareFiles_DifferentFiles_ReturnsFalse()
        {
            // Setup
            var pathA = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "FileToCompare_Original.txt");
            var pathB = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "FileToCompare_Different.txt");

            // Call
            bool areEqual = FileUtils.CompareFiles(pathA, pathB);

            // Assert
            Assert.IsFalse(areEqual);
        }
    }
}