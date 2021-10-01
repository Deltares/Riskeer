// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
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
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                null,
                HydraulicBoundaryLocationCalculationsType.WaterLevel, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbabilities_FolderPathNull_ThrowsArgumentException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                Enumerable.Empty<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>>(), HydraulicBoundaryLocationCalculationsType.WaterLevel, null);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbabilities_InvalidCalculationsType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const HydraulicBoundaryLocationCalculationsType calculationsType = (HydraulicBoundaryLocationCalculationsType) 99;

            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbabilities(
                Enumerable.Empty<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>>(), calculationsType, string.Empty);

            // Assert
            string expectedMessage = $"The value of argument 'calculationsType' ({calculationsType}) " +
                                     $"is invalid for Enum type '{nameof(HydraulicBoundaryLocationCalculationsType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("calculationsType", exception.ParamName);
        }

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, "ExpectedWaterLevelExport")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, "ExpectedWaveHeightExport")]
        public void ExportLocationCalculationsForTargetProbabilities_ValidData_ReturnsTrueAndWritesCorrectData(HydraulicBoundaryLocationCalculationsType calculationsType,
                                                                                                               string expectedExportFileName)
        {
            // Setup
            const double targetProbability = 0.05;

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
                            new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2))
                        }, targetProbability)
                    }, calculationsType, directoryPath);

                // Assert
                Assert.IsTrue(isExported);

                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, shapeFileName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(
                    directoryPath, shapeFileName,
                    Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO),
                                 nameof(HydraulicBoundaryLocationCalculationsExportHelper)),
                    expectedExportFileName, 28, 8, 628);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel)]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight)]
        public void ExportLocationCalculationsForTargetProbabilities_DuplicateTargetProbability_ReturnsTrueAndWritesExpectedFiles(HydraulicBoundaryLocationCalculationsType calculationsType)
        {
            // Setup
            const double targetProbability = 0.00005;

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
                    }, calculationsType, directoryPath);

                // Assert
                Assert.IsTrue(isExported);

                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, shapeFileName, true);
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, $"{shapeFileName} (1)", true);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbabilities_HydraulicBoundaryLocationCalculationsExporterReturnsFalse_LogErrorAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            double targetProbability = random.NextDouble(0, 0.1);
            var calculationsType = random.NextEnumValue<HydraulicBoundaryLocationCalculationsType>();

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
                                               new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2))
                                           }, targetProbability)
                                       }, calculationsType, directoryPath);

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
                Directory.Delete(directoryPath, true);
            }
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