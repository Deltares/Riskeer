// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Text;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseHelperTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        [Test]
        public void ValidateFilesForCalculation_ExistingFileWithHlcd_ReturnsNull()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(validFilePath, "");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ValidateFilesForCalculation_NonExistingFile_ReturnsMessageWithError()
        {
            // Setup
            const string nonexistingSqlite = "nonexisting.sqlite";
            string filePath = Path.Combine(testDataPath, nonexistingSqlite);

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(filePath, "");

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{filePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_InvalidFile_ReturnsMessageWithError()
        {
            // Setup
            string invalidPath = Path.Combine(testDataPath, "complete.sqlite");
            invalidPath = invalidPath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(invalidPath, testDataPath);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{invalidPath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_FileIsDirectory_ReturnsMessageWithError()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "/");

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(filePath, testDataPath);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{filePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_ExistingFileWithoutHlcd_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutHLCD", "empty.sqlite");

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(validFilePath, testDataPath);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{validFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_ExistingFileWithoutSettings_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutSettings", "empty.sqlite");

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(validFilePath, testDataPath);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{validFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_ExistingFileWithInvalidSettingsDatabase_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", "complete.sqlite");

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(validFilePath, testDataPath);

            // Assert
            Assert.AreEqual("De rekeninstellingen database heeft niet het juiste schema.", result);
        }

        [Test]
        public void ValidateFilesForCalculation_InvalidFilePath_ReturnsMessageWithError()
        {
            // Setup
            const string invalidFilePath = "C:\\Thisissomeverylongpath\\toadirectorywhich\\doesntevenexist\\Nowlets\\finishwithsomelongname\\" +
                                           "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong" +
                                           "naaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaame" +
                                           "\\followedbythefile";

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(invalidFilePath, testDataPath);

            // Assert
            Assert.AreEqual($"Het opgegeven bestandspad ({invalidFilePath}) is niet geldig.", result);
        }

        [Test]
        public void HaveEqualVersion_InvalidFile_ThrowsCriticalFileReadException()
        {
            // Setup
            string invalidPath = Path.Combine(testDataPath, "complete.sqlite");
            invalidPath = invalidPath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            TestDelegate test = () => HydraulicBoundaryDatabaseHelper.HaveEqualVersion(new HydraulicBoundaryDatabase(), invalidPath);

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void HaveEqualVersion_PathNotSet_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HydraulicBoundaryDatabaseHelper.HaveEqualVersion(new HydraulicBoundaryDatabase(), null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("pathToDatabase", parameter);
        }

        [Test]
        public void HaveEqualVersion_DatabaseNotSet_ThrowsArgumentNullException()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            TestDelegate test = () => HydraulicBoundaryDatabaseHelper.HaveEqualVersion(null, validFilePath);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("database", parameter);
        }

        [Test]
        public void HaveEqualVersion_ValidFileEqualVersion_ReturnsTrue()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var database = new HydraulicBoundaryDatabase
            {
                Version = "Dutch coast South19-11-2015 12:0013"
            };

            // Call
            bool isEqual = HydraulicBoundaryDatabaseHelper.HaveEqualVersion(database, validFilePath);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void HaveEqualVersion_ValidFileDifferentVersion_ReturnsFalse()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var database = new HydraulicBoundaryDatabase
            {
                Version = "Dutch coast South19-11-2015 12:0113"
            };

            // Call
            bool isEqual = HydraulicBoundaryDatabaseHelper.HaveEqualVersion(database, validFilePath);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        [TestCase(null)]
        [TestCase("   ")]
        public void ValidatePreprocessorDirectory_InvalidEmptyPath_ReturnsExpectedMessage(string invalidEmptyPath)
        {
            // Call
            string message = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(invalidEmptyPath);

            // Assert
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. Het bestandspad moet opgegeven zijn.", message);
        }

        [Test]
        public void ValidatePreprocessorDirectory_PathTooLong_ReturnsExpectedMessage()
        {
            // Setup
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"C:\");
            for (var i = 0; i < 300; i++)
            {
                stringBuilder.Append("A");
            }

            stringBuilder.Append(Path.DirectorySeparatorChar);
            string tooLongFolderPath = stringBuilder.ToString();

            // Call
            string message = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(tooLongFolderPath);

            // Assert
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. Het bestandspad is te lang.", message);
        }

        [Test]
        public void ValidatePreprocessorDirectory_InvalidColonCharacterInPath_ReturnsExpectedMessage()
        {
            // Setup
            const string folderWithInvalidColonCharacter = @"C:\Left:Right";

            // Call
            string message = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(folderWithInvalidColonCharacter);

            // Assert
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. Het bestandspad bevat een ':' op een ongeldige plek.", message);
        }

        [Test]
        public void ValidatePreprocessorDirectory_ValidEmptyPath_ReturnsNull()
        {
            // Call
            string message = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory("");

            // Assert
            Assert.IsNull(message);
        }

        [Test]
        public void ValidatePreprocessorDirectory_ValidPath_ReturnsNull()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath();

            // Call
            string message = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(path);

            // Assert
            Assert.IsNull(message);
        }

        [Test]
        public void ValidatePreprocessorDirectory_PathDoesNotExist_ReturnsExpectedMessage()
        {
            // Setup
            const string nonExistingFolder = "Preprocessor";

            // Call
            string message = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(nonExistingFolder);

            // Assert
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", message);
        }
    }
}