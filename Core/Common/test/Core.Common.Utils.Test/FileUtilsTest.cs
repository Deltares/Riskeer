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
        public void ValidateFilePath_PathEndsWithEmptyFileName_ThrowsArgumentException()
        {
            // Setup
            var folderPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils) + Path.DirectorySeparatorChar;

            // Call
            TestDelegate call = () => FileUtils.ValidateFilePath(folderPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = String.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet verwijzen naar een lege bestandsnaam.", folderPath);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void IsValidFilePath_ValidPath_ReturnsTrue()
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "validFile.txt");

            // Call
            var valid = FileUtils.IsValidFilePath(path);

            // Assert
            Assert.IsTrue(valid);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void IsValidFilePath_InvalidEmptyPath_ReturnsFalse(string invalidPath)
        {
            // Call
            var valid = FileUtils.IsValidFilePath(invalidPath);

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidFilePath_PathContainingInvalidFileCharacters_ReturnsFalse()
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "validFile.txt");
            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            var invalidPath = path.Replace('d', invalidFileNameChars[0]);

            // Call
            var valid = FileUtils.IsValidFilePath(invalidPath);

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidFilePath_PathIsActuallyFolder_ReturnsFalse()
        {
            // Setup
            var folderPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils) + Path.DirectorySeparatorChar;

            // Call
            var valid = FileUtils.IsValidFilePath(folderPath);

            // Assert
            Assert.IsFalse(valid);
        }
    }
}