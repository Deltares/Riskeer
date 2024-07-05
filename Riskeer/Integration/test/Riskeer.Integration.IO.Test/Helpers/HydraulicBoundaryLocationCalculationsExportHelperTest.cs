// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Common.Util.Helpers;
using Riskeer.Integration.IO.Exporters;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsExportHelperTest
    {
        [Test]
        public void ExportLocationCalculationsForTargetProbabilities_CalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                null, assessmentSection, HydraulicBoundaryLocationCalculationsType.WaterLevel, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbabilities", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbabilities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                Enumerable.Empty<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>>(), null, HydraulicBoundaryLocationCalculationsType.WaterLevel, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbabilities_FolderPathNull_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                Enumerable.Empty<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>>(), assessmentSection, HydraulicBoundaryLocationCalculationsType.WaterLevel, null);

            // Assert
            Assert.Throws<ArgumentException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbabilities_InvalidCalculationsType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const HydraulicBoundaryLocationCalculationsType calculationsType = (HydraulicBoundaryLocationCalculationsType) 99;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                Enumerable.Empty<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>>(), assessmentSection, calculationsType, string.Empty);

            // Assert
            string expectedMessage = $"The value of argument 'calculationsType' ({calculationsType}) " +
                                     $"is invalid for Enum type '{nameof(HydraulicBoundaryLocationCalculationsType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("calculationsType", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, "ExpectedWaterLevelExport")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, "ExpectedWaveHeightExport")]
        public void ExportLocationCalculationsForTargetProbabilities_ValidData_ReturnsTrueAndWritesCorrectData(HydraulicBoundaryLocationCalculationsType calculationsType,
                                                                                                               string expectedExportFileName)
        {
            // Setup
            const double targetProbability = 0.05;

            var location = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(new HydraulicBoundaryData
            {
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = "Just/A/HRD/File.sqlite",
                        Locations =
                        {
                            location
                        }
                    }
                }
            });
            mocks.ReplayAll();

            string directoryPath = TestHelper.GetScratchPadPath(nameof(ExportLocationCalculationsForTargetProbabilities_ValidData_ReturnsTrueAndWritesCorrectData));
            Directory.CreateDirectory(directoryPath);

            string shapeFileName = GetExpectedShapeFileName(calculationsType, targetProbability);

            // Precondition
            FileTestHelper.AssertEssentialShapefilesExist(directoryPath, $"{shapeFileName}.shp", false);

            try
            {
                // Call
                bool isExported = HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                    new[]
                    {
                        new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(new[]
                        {
                            new HydraulicBoundaryLocationCalculation(location)
                        }, targetProbability)
                    }, assessmentSection, calculationsType, directoryPath);

                // Assert
                Assert.IsTrue(isExported);

                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, shapeFileName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(
                    directoryPath, shapeFileName,
                    Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO),
                                 nameof(HydraulicBoundaryLocationCalculationsExportHelper)),
                    expectedExportFileName, 28, 8, 915);
            }
            finally
            {
                DirectoryHelper.TryDelete(directoryPath);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel)]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight)]
        public void ExportLocationCalculationsForTargetProbabilities_DuplicateTargetProbability_ReturnsTrueAndWritesExpectedFiles(HydraulicBoundaryLocationCalculationsType calculationsType)
        {
            // Setup
            const double targetProbability = 0.00005;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(new HydraulicBoundaryData());
            mocks.ReplayAll();

            string directoryPath = TestHelper.GetScratchPadPath(nameof(ExportLocationCalculationsForTargetProbabilities_DuplicateTargetProbability_ReturnsTrueAndWritesExpectedFiles));
            Directory.CreateDirectory(directoryPath);

            string shapeFileName = GetExpectedShapeFileName(calculationsType, targetProbability);

            // Precondition
            FileTestHelper.AssertEssentialShapefilesExist(directoryPath, $"{shapeFileName}.shp", false);

            try
            {
                // Call
                bool isExported = HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                    new[]
                    {
                        new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                            Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), targetProbability),
                        new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                            Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), targetProbability)
                    }, assessmentSection, calculationsType, directoryPath);

                // Assert
                Assert.IsTrue(isExported);

                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, shapeFileName, true);
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, $"{shapeFileName} (1)", true);
            }
            finally
            {
                DirectoryHelper.TryDelete(directoryPath);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbabilities_HydraulicBoundaryLocationCalculationsExporterReturnsFalse_LogErrorAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            double targetProbability = random.NextDouble(0, 0.1);
            var calculationsType = random.NextEnumValue<HydraulicBoundaryLocationCalculationsType>();

            var location = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(new HydraulicBoundaryData
            {
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = "Just/A/HRD/File.sqlite",
                        Locations =
                        {
                            location
                        }
                    }
                }
            });
            mocks.ReplayAll();

            string directoryPath = TestHelper.GetScratchPadPath(nameof(ExportLocationCalculationsForTargetProbabilities_HydraulicBoundaryLocationCalculationsExporterReturnsFalse_LogErrorAndReturnFalse));
            Directory.CreateDirectory(directoryPath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    var isExported = true;

                    void Call() => isExported = HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                                       new[]
                                       {
                                           new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(new[]
                                           {
                                               new HydraulicBoundaryLocationCalculation(location)
                                           }, targetProbability)
                                       }, assessmentSection, calculationsType, directoryPath);

                    // Assert
                    string fileName = GetExpectedShapeFileName(calculationsType, targetProbability);
                    string filePath = Path.Combine(directoryPath, $"{fileName}.shp");
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. " +
                                             "Er zijn geen hydraulische belastingenlocaties geëxporteerd.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage);
                    Assert.IsFalse(isExported);
                }
            }
            finally
            {
                DirectoryHelper.TryDelete(directoryPath);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, 0.011, "Waterstanden_91")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, 0.0021, "Waterstanden_476")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, 0.031, "Golfhoogten_32")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, 0.0041, "Golfhoogten_244")]
        public void GetUniqueFileName_WithoutExistingFileNames_ReturnsExpectedFileName(HydraulicBoundaryLocationCalculationsType calculationsType,
                                                                                       double targetProbability, string expectedFileName)
        {
            // Call
            string uniqueFileName = HydraulicBoundaryLocationCalculationsExportHelper.GetUniqueFileName(calculationsType, targetProbability);

            // Assert
            Assert.AreEqual(expectedFileName, uniqueFileName);
        }

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, 0.123, "Waterstanden_8")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, 0.0456, "Waterstanden_22 (1)")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, 0.789, "Golfhoogten_1 (2)")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, 0.01011, "Golfhoogten_99")]
        public void GetUniqueFileName_WithExistingFileNames_ReturnsExpectedFileName(HydraulicBoundaryLocationCalculationsType calculationsType,
                                                                                    double targetProbability, string expectedFileName)
        {
            // Setup
            var existingFileNames = new[]
            {
                "Waterstanden_8 (1)",
                "Waterstanden_22",
                "Golfhoogten_1",
                "Golfhoogten_1 (1)",
                "Golfhoogten_1 (3)",
                "Golfhoogten_99 (1)"
            };

            // Call
            string uniqueFileName = HydraulicBoundaryLocationCalculationsExportHelper.GetUniqueFileName(calculationsType, targetProbability, existingFileNames);

            // Assert
            Assert.AreEqual(expectedFileName, uniqueFileName);
        }

        private static string GetExpectedShapeFileName(HydraulicBoundaryLocationCalculationsType calculationsType,
                                                       double targetProbability)
        {
            string exportType = calculationsType == HydraulicBoundaryLocationCalculationsType.WaterLevel
                                    ? "Waterstanden"
                                    : "Golfhoogten";

            return $"{exportType}_{ReturnPeriodFormattingHelper.FormatFromProbability(targetProbability)}";
        }
    }
}