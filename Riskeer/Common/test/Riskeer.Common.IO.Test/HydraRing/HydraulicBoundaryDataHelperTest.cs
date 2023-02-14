// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
    public class HydraulicBoundaryDataHelperTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));

        private readonly string validHrdFilePath = Path.Combine(testDataPath, "complete.sqlite");
        private readonly string validHlcdFilePath = Path.Combine(testDataPath, "HLCD.sqlite");

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateFilesForCalculation_ValidFilesWithPreprocessorClosure_ReturnsNull(bool usePreprocessorClosure)
        {
            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(validHrdFilePath, validHlcdFilePath, testDataPath, usePreprocessorClosure);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ValidateFilesForCalculation_NonExistingHrdFile_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "nonexisting.sqlite");

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hrdFilePath, validHlcdFilePath, testDataPath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hrdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_HrdFileWithInvalidPathChars_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "complete.sqlite").Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hrdFilePath, validHlcdFilePath, testDataPath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hrdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_HrdFileIsDirectory_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "/");

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hrdFilePath, validHlcdFilePath, testDataPath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hrdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_NonExistingHlcdFile_ReturnsMessageWithError()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "nonexisting.sqlite");

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(validHrdFilePath, hlcdFilePath, testDataPath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hlcdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_MissingSettingsDatabase_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "withoutSettings", "empty.sqlite");
            string hlcdFilePath = Path.Combine(testDataPath, "withoutSettings", "HLCD.sqlite");

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hrdFilePath, hlcdFilePath, testDataPath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hrdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_InvalidSettingsDatabase_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", "complete.sqlite");
            string hlcdFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", "HLCD.sqlite");

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hrdFilePath, hlcdFilePath, testDataPath, false);

            // Assert
            Assert.AreEqual("De rekeninstellingen database heeft niet het juiste schema.", result);
        }

        [Test]
        public void ValidateFilesForCalculation_PathToHrdFileTooLong_ReturnsMessageWithError()
        {
            // Setup
            const string hrdFilePath = "C:\\Thisissomeverylongpath\\toadirectorywhich\\doesntevenexist\\Nowlets\\finishwithsomelongname\\" +
                                       "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong" +
                                       "naaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaame" +
                                       "\\followedbythefile";

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hrdFilePath, validHlcdFilePath, testDataPath, false);

            // Assert
            Assert.AreEqual($"Het opgegeven bestandspad ({hrdFilePath}) is niet geldig.", result);
        }

        [Test]
        public void ValidateFileForCalculation_UsePreprocessorClosureTrueWithoutPreprocessorClosureFile_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "complete.sqlite");
            string hlcdFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "HLCD.sqlite");

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hrdFilePath, hlcdFilePath, testDataPath, true);

            // Assert
            string preprocessorClosureFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "HLCD_preprocClosure.sqlite");
            Assert.AreEqual($"Fout bij het lezen van bestand '{preprocessorClosureFilePath}': het bestand bestaat niet.", result);
        }

        [Test]
        public void HaveEqualVersion_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "complete.sqlite");

            // Call
            void Call() => HydraulicBoundaryDataHelper.HaveEqualVersion(null, hrdFilePath);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hydraulicBoundaryData", parameter);
        }

        [Test]
        public void HaveEqualVersion_HrdFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            void Call() => HydraulicBoundaryDataHelper.HaveEqualVersion(hydraulicBoundaryData, null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("hrdFilePath", parameter);
        }

        [Test]
        public void HaveEqualVersion_HrdFileWithInvalidPathChars_ThrowsCriticalFileReadException()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "complete.sqlite").Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            void Test() => HydraulicBoundaryDataHelper.HaveEqualVersion(new HydraulicBoundaryData(), hrdFilePath);

            // Assert
            Assert.Throws<CriticalFileReadException>(Test);
        }

        [Test]
        public void HaveEqualVersion_EqualVersion_ReturnsTrue()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                Version = "Dutch coast South19-11-2015 12:0013"
            };

            // Call
            bool isEqual = HydraulicBoundaryDataHelper.HaveEqualVersion(hydraulicBoundaryData, hrdFilePath);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void HaveEqualVersion_DifferentVersions_ReturnsFalse()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                Version = "Dutch coast South19-11-2015 12:0113"
            };

            // Call
            bool isEqual = HydraulicBoundaryDataHelper.HaveEqualVersion(hydraulicBoundaryData, hrdFilePath);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        [TestCase(null)]
        [TestCase("   ")]
        public void ValidatePreprocessorDirectory_InvalidEmptyPath_ReturnsExpectedMessage(string invalidEmptyPath)
        {
            // Call
            string message = HydraulicBoundaryDataHelper.ValidatePreprocessorDirectory(invalidEmptyPath);

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
            string message = HydraulicBoundaryDataHelper.ValidatePreprocessorDirectory(tooLongFolderPath);

            // Assert
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. Het bestandspad is te lang.", message);
        }

        [Test]
        public void ValidatePreprocessorDirectory_InvalidColonCharacterInPath_ReturnsExpectedMessage()
        {
            // Setup
            const string folderWithInvalidColonCharacter = @"C:\Left:Right";

            // Call
            string message = HydraulicBoundaryDataHelper.ValidatePreprocessorDirectory(folderWithInvalidColonCharacter);

            // Assert
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. Het bestandspad bevat een ':' op een ongeldige plek.", message);
        }

        [Test]
        public void ValidatePreprocessorDirectory_ValidEmptyPath_ReturnsNull()
        {
            // Call
            string message = HydraulicBoundaryDataHelper.ValidatePreprocessorDirectory("");

            // Assert
            Assert.IsNull(message);
        }

        [Test]
        public void ValidatePreprocessorDirectory_ValidPath_ReturnsNull()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath();

            // Call
            string message = HydraulicBoundaryDataHelper.ValidatePreprocessorDirectory(path);

            // Assert
            Assert.IsNull(message);
        }

        [Test]
        public void ValidatePreprocessorDirectory_PathDoesNotExist_ReturnsExpectedMessage()
        {
            // Setup
            const string nonExistingFolder = "Preprocessor";

            // Call
            string message = HydraulicBoundaryDataHelper.ValidatePreprocessorDirectory(nonExistingFolder);

            // Assert
            Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", message);
        }

        [Test]
        public void GetPreprocessorClosureFilePath_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryDataHelper.GetPreprocessorClosureFilePath(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
        }

        [Test]
        public void GetPreprocessorClosureFilePath_WithHlcdFilePath_ReturnsPreprocessorClosureFilePath()
        {
            // Call
            string preprocessorClosureFilePath = HydraulicBoundaryDataHelper.GetPreprocessorClosureFilePath(validHlcdFilePath);

            // Assert
            Assert.AreEqual(Path.Combine(testDataPath, "HLCD_preprocClosure.sqlite"), preprocessorClosureFilePath);
        }
    }
}