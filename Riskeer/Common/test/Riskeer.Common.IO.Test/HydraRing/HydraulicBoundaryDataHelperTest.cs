// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
        private const string validHlcdFileName = "HLCD.sqlite";
        private const string validHrdFileName = "HRD dutch coast south.sqlite";

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));

        private readonly string validHlcdFilePath = Path.Combine(testDataPath, validHlcdFileName);
        private readonly string validHrdFilePath = Path.Combine(testDataPath, validHrdFileName);

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ValidateFilesForCalculation_ValidFilesWithPreprocessorClosure_ReturnsNull(bool usePreprocessorClosure)
        {
            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(validHlcdFilePath, validHrdFilePath, usePreprocessorClosure);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ValidateFilesForCalculation_NonExistingHrdFile_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "nonexisting.sqlite");

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(validHlcdFilePath, hrdFilePath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hrdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_HrdFileWithInvalidPathChars_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = validHrdFilePath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(validHlcdFilePath, hrdFilePath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hrdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_HrdFileIsDirectory_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "/");

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(validHlcdFilePath, hrdFilePath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hrdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_NonExistingHlcdFile_ReturnsMessageWithError()
        {
            // Setup
            string hlcdFilePath = Path.Combine(testDataPath, "nonexisting.sqlite");

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hlcdFilePath, validHrdFilePath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hlcdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_MissingSettingsDatabase_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "withoutSettings", "empty.sqlite");
            string hlcdFilePath = Path.Combine(testDataPath, "withoutSettings", validHlcdFileName);

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hlcdFilePath, hrdFilePath, false);

            // Assert
            StringAssert.StartsWith($"Fout bij het lezen van bestand '{hrdFilePath}':", result);
        }

        [Test]
        public void ValidateFilesForCalculation_InvalidSettingsDatabase_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", validHrdFileName);
            string hlcdFilePath = Path.Combine(testDataPath, "invalidSettingsSchema", validHlcdFileName);

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hlcdFilePath, hrdFilePath, false);

            // Assert
            Assert.AreEqual("De rekeninstellingen database heeft niet het juiste schema.", result);
        }

        [Test]
        public void ValidateFilesForCalculation_PathToHrdFileTooLong_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = GetFolderPathThatIsTooLong() + validHrdFileName;

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(validHlcdFilePath, hrdFilePath, false);

            // Assert
            Assert.AreEqual($"Het opgegeven bestandspad ({hrdFilePath}) is niet geldig.", result);
        }

        [Test]
        public void ValidateFileForCalculation_UsePreprocessorClosureTrueWithoutPreprocessorClosureDatabase_ReturnsMessageWithError()
        {
            // Setup
            string hrdFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", validHrdFileName);
            string hlcdFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", validHlcdFileName);

            // Call
            string result = HydraulicBoundaryDataHelper.ValidateFilesForCalculation(hlcdFilePath, hrdFilePath, true);

            // Assert
            string preprocessorClosureFilePath = Path.Combine(testDataPath, "withoutPreprocessorClosure", "HLCD_preprocClosure.sqlite");
            Assert.AreEqual($"Fout bij het lezen van bestand '{preprocessorClosureFilePath}': het bestand bestaat niet.", result);
        }

        [Test]
        public void IsCorrectVersion_HrdFileWithInvalidPathChars_ThrowsCriticalFileReadException()
        {
            // Setup
            string hrdFilePath = validHrdFilePath.Replace('c', Path.GetInvalidPathChars()[0]);

            // Call
            void Test() => HydraulicBoundaryDataHelper.IsCorrectVersion(string.Empty, hrdFilePath);

            // Assert
            Assert.Throws<CriticalFileReadException>(Test);
        }

        [Test]
        public void IsCorrectVersion_EqualVersions_ReturnsTrue()
        {
            // Call
            bool isEqual = HydraulicBoundaryDataHelper.IsCorrectVersion("Dutch coast South19-11-2015 12:0013", validHrdFilePath);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void IsCorrectVersion_DifferentVersions_ReturnsFalse()
        {
            // Call
            bool isEqual = HydraulicBoundaryDataHelper.IsCorrectVersion("Dutch coast South19-11-2015 12:0113", validHrdFilePath);

            // Assert
            Assert.IsFalse(isEqual);
        }

        private static string GetFolderPathThatIsTooLong()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(@"C:\");

            for (var i = 0; i < 300; i++)
            {
                stringBuilder.Append("A");
            }

            stringBuilder.Append(Path.DirectorySeparatorChar);

            return stringBuilder.ToString();
        }
    }
}