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
using Riskeer.Integration.IO.Exporters;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsExportHelperTest
    {
        [Test]
        public void ExportLocationCalculationsForTargetProbabilityConstructor_CalculationsForTargetProbabilityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbability(
                null, new Dictionary<IEnumerable<HydraulicBoundaryLocationCalculation>, string>(),
                HydraulicBoundaryLocationCalculationsType.WaterLevel, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbability", exception.ParamName);
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbability_ExportedCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbability(
                new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), double.NaN),
                null, HydraulicBoundaryLocationCalculationsType.WaterLevel, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("exportedCalculations", exception.ParamName);
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbability_InvalidCalculationsType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const HydraulicBoundaryLocationCalculationsType calculationsType = (HydraulicBoundaryLocationCalculationsType) 99;

            // Call
            void Call() => HydraulicBoundaryLocationCalculationsExportHelper.ExportLocationCalculationsForTargetProbability(
                new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), double.NaN),
                new Dictionary<IEnumerable<HydraulicBoundaryLocationCalculation>, string>(), calculationsType, string.Empty);

            // Assert
            string expectedMessage = $"The value of argument 'calculationsType' ({calculationsType}) " +
                                     $"is invalid for Enum type '{nameof(HydraulicBoundaryLocationCalculationsType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("calculationsType", exception.ParamName);
        }

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, "ExpectedWaterLevelExport")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, "ExpectedWaveHeightExport")]
        public void ExportLocationCalculationsForTargetProbability_ValidData_ReturnsTrueAndWritesCorrectData(HydraulicBoundaryLocationCalculationsType calculationsType,
                                                                                                             string expectedExportFileName)
        {
            // Setup
            const string fileName = "test";

            string directoryPath = TestHelper.GetScratchPadPath(nameof(ExportLocationCalculationsForTargetProbability_ValidData_ReturnsTrueAndWritesCorrectData));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, $"{fileName}.shp");

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter(new[]
            {
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2))
            }, filePath, calculationsType);

            // Precondition
            FileTestHelper.AssertEssentialShapefilesExist(directoryPath, fileName, false);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                FileTestHelper.AssertEssentialShapefilesExist(directoryPath, fileName, true);
                FileTestHelper.AssertEssentialShapefileMd5Hashes(
                    directoryPath, fileName,
                    Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO),
                                 nameof(HydraulicBoundaryLocationCalculationsExportHelper)),
                    expectedExportFileName, 28, 8, 628);
                Assert.IsTrue(isExported);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void ExportLocationCalculationsForTargetProbability_InvalidDirectoryRights_LogErrorAndReturnFalse()
        {
            // Setup
            const string fileName = "test";

            string directoryPath = TestHelper.GetScratchPadPath(nameof(ExportLocationCalculationsForTargetProbability_InvalidDirectoryRights_LogErrorAndReturnFalse));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, $"{fileName}.shp");

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter(new[]
            {
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2))
            }, filePath, HydraulicBoundaryLocationCalculationsType.WaterLevel);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    var isExported = true;
                    void Call() => isExported = exporter.Export();

                    // Assert
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
    }
}