﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void DeleteOldFiles_InvalidEmptyPath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            TestDelegate call = () => FileUtils.DeleteOldFiles(invalidPath,"",0);

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
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils);

            // Call
            TestDelegate call = () => FileUtils.DeleteOldFiles(path, invalidSearchPattern, 0);

            // Assert
            string invalidParameterName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("searchPattern", invalidParameterName);
        }
        
        [Test]
        public void DeleteOldFiles_PathDoesNotExist_ThrowsIOException()
        {
            // Setup
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils, "doesNotExist");

            // Precondition
            Assert.IsFalse(Directory.Exists(path));

            // Call
            TestDelegate call = () => FileUtils.DeleteOldFiles(path, "*.log", 0);

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
            var path = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Utils);
            var filePath = Path.Combine(path, "fileToDelete.log");

            using (new FileDisposeHelper(filePath))
            {
                // Call
                FileUtils.DeleteOldFiles(path, "*.log", 0);

                // Assert
                Assert.IsFalse(File.Exists(filePath));
            }
        }
    }
}