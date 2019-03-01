// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.HydraRing;

namespace Riskeer.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseHelperTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase));
        private readonly string hlcdFilePath = Path.Combine(testDataPath, "HLCD.sqlite");

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateFilesForCalculation_ExistingFileWithHlcdAndPreprocessorClosure_ReturnsNull(bool usePreprocessorClosure)
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            
            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(validFilePath, hlcdFilePath, "", usePreprocessorClosure);

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
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(filePath, "", "", false);

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
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(invalidPath, hlcdFilePath, testDataPath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{invalidPath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_FileIsDirectory_ReturnsMessageWithError()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "/");

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(filePath, hlcdFilePath, testDataPath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{filePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_ExistingFileWithoutHlcd_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutHLCD", "empty.sqlite");
            string customHlcdFilePath = Path.Combine(testDataPath, "withoutHLCD", "HLCD.sqlite");

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(validFilePath, customHlcdFilePath, testDataPath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{customHlcdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_ExistingFileWithoutSettings_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutSettings", "empty.sqlite");

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(validFilePath, hlcdFilePath, testDataPath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{validFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_ExistingFileWithInvalidSettingsDatabase_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", "complete.sqlite");

            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(validFilePath, hlcdFilePath, testDataPath, false);

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
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(invalidFilePath, hlcdFilePath, testDataPath, false);

            // Assert
            Assert.AreEqual($"Het opgegeven bestandspad ({invalidFilePath}) is niet geldig.", result);
        }

        [Test]
        public void ValidateFileForCalculation_UsePreprocessorClosureTrueWithoutPreprocessorClosureFile_ReturnsMessageWithError()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "complete.sqlite");
            string customHlcdFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "HLCD.sqlite");
            
            // Call
            string result = HydraulicBoundaryDatabaseHelper.ValidateFilesForCalculation(validFilePath, customHlcdFilePath, testDataPath, true);

            // Assert
            string preprocessorClosureFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "HLCD_preprocClosure.sqlite");
            Assert.AreEqual($"Fout bij het lezen van bestand '{preprocessorClosureFilePath}': het bestand bestaat niet.", result);
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

        [Test]
        public void GetPreprocessorClosureFilePath_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicBoundaryDatabaseHelper.GetPreprocessorClosureFilePath(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
        }

        [Test]
        public void GetPreprocessorClosureFilePath_WithHlcdFilePath_ReturnsPreprocessorClosureFilePath()
        {
            // Call
            string preprocessorClosureFilePath = HydraulicBoundaryDatabaseHelper.GetPreprocessorClosureFilePath(hlcdFilePath);

            // Assert
            Assert.AreEqual(Path.Combine(testDataPath, "HLCD_preprocClosure.sqlite"), preprocessorClosureFilePath);
        }
    }
}