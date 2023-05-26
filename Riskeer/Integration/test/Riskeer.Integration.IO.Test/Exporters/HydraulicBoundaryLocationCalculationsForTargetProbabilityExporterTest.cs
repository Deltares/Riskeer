﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Integration.IO.Exporters;

namespace Riskeer.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsForTargetProbabilityExporterTest
    {
        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter(null, string.Empty, HydraulicBoundaryLocationCalculationsType.WaterLevel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), null, HydraulicBoundaryLocationCalculationsType.WaterLevel);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void Constructor_InvalidHydraulicBoundaryLocationCalculationsType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));
            const HydraulicBoundaryLocationCalculationsType hydraulicBoundaryLocationCalculationsType = (HydraulicBoundaryLocationCalculationsType) 99;

            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), filePath, hydraulicBoundaryLocationCalculationsType);

            // Assert
            string expectedMessage = $"The value of argument 'calculationsType' ({hydraulicBoundaryLocationCalculationsType}) " +
                                     $"is invalid for Enum type '{nameof(HydraulicBoundaryLocationCalculationsType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("calculationsType", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), filePath, HydraulicBoundaryLocationCalculationsType.WaterLevel);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, "ExpectedWaterLevelExport")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, "ExpectedWaveHeightExport")]
        public void Export_ValidData_ReturnsTrueAndWritesCorrectData(HydraulicBoundaryLocationCalculationsType calculationsType,
                                                                     string expectedExportFileName)
        {
            // Setup
            const string fileName = "test";

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_ValidData_ReturnsTrueAndWritesCorrectData));
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
                                 nameof(HydraulicBoundaryLocationCalculationsForTargetProbabilityExporter)),
                    expectedExportFileName, 28, 8, 628);
                Assert.IsTrue(isExported);
            }
            finally
            {
                DirectoryHelper.TryDelete(directoryPath);
            }
        }

        [Test]
        public void Export_WriterThrowsCriticalFileWriteException_LogErrorAndReturnFalse()
        {
            // Setup
            const string fileName = "test";

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_WriterThrowsCriticalFileWriteException_LogErrorAndReturnFalse));
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
                DirectoryHelper.TryDelete(directoryPath);
            }
        }
    }
}