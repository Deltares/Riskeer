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
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Util.Helpers;
using Riskeer.Integration.IO.Exporters;

namespace Riskeer.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporterTest
    {
        [Test]
        public void Constructor_LocationCalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                null, HydraulicBoundaryLocationCalculationsType.WaterLevel, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("locationCalculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void Constructor_InvalidHydraulicBoundaryLocationCalculationsType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const HydraulicBoundaryLocationCalculationsType calculationsType = (HydraulicBoundaryLocationCalculationsType) 99;

            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                Enumerable.Empty<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>>(), calculationsType, string.Empty);

            // Assert
            var expectedMessage = $"The value of argument 'calculationsType' ({calculationsType}) is invalid for Enum type '{nameof(HydraulicBoundaryLocationCalculationsType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("calculationsType", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentException()
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                Enumerable.Empty<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>>(),
                HydraulicBoundaryLocationCalculationsType.WaterLevel, null);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                Enumerable.Empty<Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>>(),
                HydraulicBoundaryLocationCalculationsType.WaterLevel, filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Export_HydraulicBoundaryLocationCalculationsExporterReturnsFalse_LogsErrorAndReturnsFalse()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_HydraulicBoundaryLocationCalculationsExporterReturnsFalse_LogsErrorAndReturnsFalse));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "export.zip");

            var calculationsForTargetProbabilities = new[]
            {
                new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                    Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), 0.1)
            };

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                calculationsForTargetProbabilities, HydraulicBoundaryLocationCalculationsType.WaterLevel, filePath);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.CreateDirectories))
                {
                    // Call
                    var isExported = true;
                    void Call() => isExported = exporter.Export();

                    // Assert
                    string expectedFilePath = Path.Combine(directoryPath, "~temp", "Waterstanden_10.shp");
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{expectedFilePath}'. " +
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

        [Test]
        public void Export_CreatingZipFileThrowsCriticalFileWriteException_LogsErrorAndReturnsFalse()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_CreatingZipFileThrowsCriticalFileWriteException_LogsErrorAndReturnsFalse));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "export.zip");

            var calculationsForTargetProbabilities = new[]
            {
                new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                    Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), 0.1)
            };

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                calculationsForTargetProbabilities, HydraulicBoundaryLocationCalculationsType.WaterLevel, filePath);

            try
            {
                using (var helper = new FileDisposeHelper(filePath))
                {
                    helper.LockFiles();

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

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, "Waterstanden")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, "Golfhoogten")]
        public void Export_WithHydraulicBoundaryLocationCalculationsForTargetProbabilities_WritesFilesAndReturnsTrue(
            HydraulicBoundaryLocationCalculationsType calculationsType, string expectedCalculationsTypeName)
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_WithHydraulicBoundaryLocationCalculationsForTargetProbabilities_WritesFilesAndReturnsTrue));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "export.zip");

            var random = new Random(21);

            var calculationsForTargetProbabilities = new[]
            {
                new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                    new List<HydraulicBoundaryLocationCalculation>(), random.NextDouble(0, 0.1)),
                new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(
                    new List<HydraulicBoundaryLocationCalculation>(), random.NextDouble(0, 0.01))
            };

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                calculationsForTargetProbabilities, calculationsType, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);

                string[] expectedFiles =
                {
                    $"{expectedCalculationsTypeName}_{GetReturnPeriodText(calculationsForTargetProbabilities.First().Item2)}.shp",
                    $"{expectedCalculationsTypeName}_{GetReturnPeriodText(calculationsForTargetProbabilities.Last().Item2)}.shp"
                };

                using (ZipArchive zipArchive = ZipFile.OpenRead(filePath))
                {
                    CollectionAssert.IsSubsetOf(expectedFiles, zipArchive.Entries.Select(e => e.FullName));
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaterLevel, "Waterstanden")]
        [TestCase(HydraulicBoundaryLocationCalculationsType.WaveHeight, "Golfhoogten")]
        public void Export_WithDuplicateHydraulicBoundaryLocationCalculationsForTargetProbabilities_WritesFilesAndReturnsTrue(
            HydraulicBoundaryLocationCalculationsType calculationsType, string expectedCalculationsTypeName)
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_WithDuplicateHydraulicBoundaryLocationCalculationsForTargetProbabilities_WritesFilesAndReturnsTrue));
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "export.zip");

            var calculationsForTargetProbabilities = new[]
            {
                new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(new List<HydraulicBoundaryLocationCalculation>(), 0.1),
                new Tuple<IEnumerable<HydraulicBoundaryLocationCalculation>, double>(new List<HydraulicBoundaryLocationCalculation>(), 0.1)
            };

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                calculationsForTargetProbabilities, calculationsType, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);

                string[] expectedFiles =
                {
                    $"{expectedCalculationsTypeName}_10.shp",
                    $"{expectedCalculationsTypeName}_10 (1).shp"
                };

                using (ZipArchive zipArchive = ZipFile.OpenRead(filePath))
                {
                    CollectionAssert.IsSubsetOf(expectedFiles, zipArchive.Entries.Select(e => e.FullName));
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        private static string GetReturnPeriodText(double targetProbability)
        {
            return ReturnPeriodFormattingHelper.FormatFromProbability(targetProbability);
        }
    }
}