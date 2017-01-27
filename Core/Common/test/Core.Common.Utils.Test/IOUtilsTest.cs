// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Text;
using System.Threading;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Utils.Test
{
    [TestFixture]
    public class IOUtilsTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void IsValidFolderPath_InvalidEmptyPath_ReturnFalse(string invalidPath)
        {
            // Call
            bool isFolderPathValid = IOUtils.IsValidFolderPath(invalidPath);

            // Assert
            Assert.IsFalse(isFolderPathValid);
        }

        [Test]
        public void IsValidFolderPath_PathTooLong_ReturnFalse()
        {
            // Setup
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"C:\");
            for (int i = 0; i < 300; i++)
            {
                stringBuilder.Append("A");
            }
            stringBuilder.Append(Path.DirectorySeparatorChar);
            string tooLongFolderPath = stringBuilder.ToString();

            // Call
            bool isFolderPathValid = IOUtils.IsValidFolderPath(tooLongFolderPath);

            // Assert
            Assert.IsFalse(isFolderPathValid);
        }

        [Test]
        public void IsValidFolderPath_InvalidColonCharacterInPath_ReturnFalse()
        {
            // Setup
            string pathWithInvalidColonCharacter = @"C:\Left:Right";

            // Call
            bool isFolderPathValid = IOUtils.IsValidFolderPath(pathWithInvalidColonCharacter);

            // Assert
            Assert.IsFalse(isFolderPathValid);
        }

        [Test]
        public void IsValidFolderPath_ValidPath_ReturnTrue()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils);

            // Call
            bool isFolderPathValid = IOUtils.IsValidFilePath(path);

            // Assert
            Assert.IsTrue(isFolderPathValid);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ValidateFolderPath_InvalidEmptyPath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            TestDelegate call = () => IOUtils.ValidateFolderPath(invalidPath);

            // Assert
            string message = $"Fout bij het schrijven naar bestandsmap '{invalidPath}': pad naar bestandsmap mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        [Test]
        public void ValidateFolderPath_PathTooLong_ThrowsArgumentException()
        {
            // Setup
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"C:\");
            for (int i = 0; i < 300; i++)
            {
                stringBuilder.Append("A");
            }
            stringBuilder.Append(Path.DirectorySeparatorChar);
            string tooLongFolderPath = stringBuilder.ToString();

            // Call
            TestDelegate call = () => IOUtils.ValidateFolderPath(tooLongFolderPath);

            // Assert
            string message = $"Fout bij het schrijven naar bestandsmap '{tooLongFolderPath}': het pad naar bestandsmap is te lang.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        [Test]
        public void ValidateFolderPath_InvalidColonCharacterInPath_ThrowsArgumentException()
        {
            // Setup
            string folderWithInvalidColonCharacter = @"C:\Left:Right";

            // Call
            TestDelegate call = () => IOUtils.ValidateFolderPath(folderWithInvalidColonCharacter);

            // Assert
            string message = $"Fout bij het schrijven naar bestandsmap '{folderWithInvalidColonCharacter}': het pad naar bestandsmap bevat een ':' op een ongeldige plek.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        [Test]
        public void ValidateFolderPath_ValidPath_DoesNotThrow()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils);

            // Call
            TestDelegate call = () => IOUtils.ValidateFolderPath(path);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ValidateFilePath_ValidPath_DoesNotThrowAnyExceptions()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "validFile.txt");

            // Call
            TestDelegate call = () => IOUtils.ValidateFilePath(path);

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
            TestDelegate call = () => IOUtils.ValidateFilePath(invalidPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': bestandspad mag niet leeg of ongedefinieerd zijn.", invalidPath);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ValidateFilePath_PathContainingInvalidFileCharacters_ThrowsArgumentException()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "validFile.txt");
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            string invalidPath = path.Replace('d', invalidFileNameChars[0]);

            // Call
            TestDelegate call = () => IOUtils.ValidateFilePath(invalidPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidPath, string.Join(", ", invalidFileNameChars));
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ValidateFilePath_PathEndsWithEmptyFileName_ThrowsArgumentException()
        {
            // Setup
            string folderPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils) + Path.DirectorySeparatorChar;

            // Call
            TestDelegate call = () => IOUtils.ValidateFilePath(folderPath);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': bestandspad mag niet verwijzen naar een lege bestandsnaam.", folderPath);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void IsValidFilePath_ValidPath_ReturnsTrue()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "validFile.txt");

            // Call
            var valid = IOUtils.IsValidFilePath(path);

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
            bool valid = IOUtils.IsValidFilePath(invalidPath);

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidFilePath_PathContainingInvalidFileCharacters_ReturnsFalse()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "validFile.txt");
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            string invalidPath = path.Replace('d', invalidFileNameChars[0]);

            // Call
            bool valid = IOUtils.IsValidFilePath(invalidPath);

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidFilePath_PathIsActuallyFolder_ReturnsFalse()
        {
            // Setup
            string folderPath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils) + Path.DirectorySeparatorChar;

            // Call
            bool valid = IOUtils.IsValidFilePath(folderPath);

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void DeleteOldFiles_InvalidEmptyPath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            TestDelegate call = () => IOUtils.DeleteOldFiles(invalidPath, "", 0);

            // Assert
            string invalidParameterName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("path", invalidParameterName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void DeleteOldFiles_InvalidSearchPattern_ThrowsArgumentException(string invalidSearchPattern)
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils);

            // Call
            TestDelegate call = () => IOUtils.DeleteOldFiles(path, invalidSearchPattern, 0);

            // Assert
            string invalidParameterName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("searchPattern", invalidParameterName);
        }

        [Test]
        public void DeleteOldFiles_PathDoesNotExist_ThrowsIOException()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "doesNotExist");

            // Precondition
            Assert.IsFalse(Directory.Exists(path));

            // Call
            TestDelegate call = () => IOUtils.DeleteOldFiles(path, "*.log", 0);

            // Assert
            IOException exception = Assert.Throws<IOException>(call);
            var message = string.Format("Er is een fout opgetreden bij het verwijderen van bestanden in de map '{0}'.", path);
            Assert.AreEqual(message, exception.Message);
            Assert.IsInstanceOf<IOException>(exception.InnerException);
        }

        [Test]
        public void DeleteOldFiles_ValidPathWithFile_DeletesFile()
        {
            // Setup
            string path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils);
            string filePath = Path.Combine(path, "fileToDelete.log");

            using (new FileDisposeHelper(filePath))
            {
                Thread.Sleep(1); // Sleep 1 ms to make sure File. Create has had enough time to create the file.

                // Call
                IOUtils.DeleteOldFiles(path, "*.log", 0);

                // Assert
                Thread.Sleep(1); // Sleep 1 ms to make sure File. Delete has had enough time to remove the file.
                Assert.IsFalse(File.Exists(filePath));
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void CreateFileIfNotExists_FilePathNullOrWhiteSpace_ThrowsArgumentException(string filePath)
        {
            // Call
            TestDelegate call = () => IOUtils.CreateFileIfNotExists(filePath);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual($"Fout bij het lezen van bestand '{filePath}': bestandspad mag niet leeg of ongedefinieerd zijn.", exception.Message);
        }

        [Test]
        public void CreateFileIfNotExists_FileNotWritable_ThrowsArgumentException()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, filename);

            using (new FileDisposeHelper(filePath))
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                File.SetAttributes(filePath, attributes | FileAttributes.ReadOnly);

                // Call
                TestDelegate call = () => IOUtils.CreateFileIfNotExists(filePath);

                // Assert
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

                File.SetAttributes(filePath, attributes);
            }
        }

        [Test]
        public void CreateFileIfNotExists_FileWritable_DoesNotThrowException()
        {
            // Setup
            string filename = Path.GetRandomFileName();
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, filename);

            using (new FileDisposeHelper(filePath))
            {
                // Call
                TestDelegate call = () => IOUtils.CreateFileIfNotExists(filePath);

                // Assert
                Assert.DoesNotThrow(call);
            }
        }
    }
}