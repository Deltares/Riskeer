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
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Helpers;
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
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("locationCalculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("C:\\Not:Valid")]
        public void Constructor_InvalidFolderPath_ThrowsArgumentException(string folderPath)
        {
            // Call
            void Call() => new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                Enumerable.Empty<Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>>(),
                folderPath);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(
                Enumerable.Empty<Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>>(),
                "test");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Export_HydraulicBoundaryLocationCalculationsExporterReturnsFalse_ReturnsFalse()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporterTest)}.{nameof(Export_HydraulicBoundaryLocationCalculationsExporterReturnsFalse_ReturnsFalse)}");
            Directory.CreateDirectory(folderPath);

            var calculationsForTargetProbabilities = new[]
            {
                new Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1),
                    HydraulicBoundaryLocationCalculationsType.WaterLevel)
            };

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(calculationsForTargetProbabilities, folderPath);

            string expectedFilePath = Path.Combine(folderPath, "Waterstanden_10.shp");

            try
            {
                using (new DirectoryPermissionsRevoker(folderPath, FileSystemRights.Write))
                {
                    // Call
                    var isExported = true;
                    void Call() => isExported = exporter.Export();

                    // Assert
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{expectedFilePath}'. " +
                                             "Er zijn geen hydraulische belastingenlocaties geëxporteerd.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage);
                    Assert.IsFalse(isExported);
                }
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        [Test]
        public void Export_WithHydraulicBoundaryLocationCalculationsForTargetProbabilities_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporterTest)}.{nameof(Export_WithHydraulicBoundaryLocationCalculationsForTargetProbabilities_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);

            var random = new Random(21);

            var calculationsForTargetProbabilities = new[]
            {
                new Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                    HydraulicBoundaryLocationCalculationsType.WaterLevel),
                new Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                    HydraulicBoundaryLocationCalculationsType.WaveHeight)
            };

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(calculationsForTargetProbabilities, folderPath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);

                string[] expectedFilePaths =
                {
                    Path.Combine(folderPath, $"Waterstanden_{GetReturnPeriodText(calculationsForTargetProbabilities.First().Item1.TargetProbability)}.shp"),
                    Path.Combine(folderPath, $"Golfhoogten_{GetReturnPeriodText(calculationsForTargetProbabilities.Last().Item1.TargetProbability)}.shp")
                };
                Assert.IsTrue(expectedFilePaths.All(File.Exists));
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        [Test]
        public void Export_WithDoubleHydraulicBoundaryLocationCalculationsForTargetProbabilities_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporterTest)}.{nameof(Export_WithDoubleHydraulicBoundaryLocationCalculationsForTargetProbabilities_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);

            var calculationsForTargetProbabilities = new[]
            {
                new Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1),
                    HydraulicBoundaryLocationCalculationsType.WaterLevel),
                new Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1),
                    HydraulicBoundaryLocationCalculationsType.WaterLevel),
                new Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.001),
                    HydraulicBoundaryLocationCalculationsType.WaveHeight),
                new Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.001),
                    HydraulicBoundaryLocationCalculationsType.WaveHeight)
            };

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(calculationsForTargetProbabilities, folderPath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);

                string[] expectedFilePaths =
                {
                    Path.Combine(folderPath, "Waterstanden_10.shp"),
                    Path.Combine(folderPath, "Waterstanden_10 (1).shp"),
                    Path.Combine(folderPath, "Golfhoogten_1.000.shp"),
                    Path.Combine(folderPath, "Golfhoogten_1.000 (1).shp")
                };
                Assert.IsTrue(expectedFilePaths.All(File.Exists));
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        [Test]
        public void Export_WithHydraulicBoundaryLocationCalculationsForTargetProbabilities_LogsMessages()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporterTest)}.{nameof(Export_WithHydraulicBoundaryLocationCalculationsForTargetProbabilities_LogsMessages)}");
            Directory.CreateDirectory(folderPath);

            var random = new Random(21);

            var calculationsForTargetProbabilities = new[]
            {
                new Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                    HydraulicBoundaryLocationCalculationsType.WaterLevel),
                new Tuple<HydraulicBoundaryLocationCalculationsForTargetProbability, HydraulicBoundaryLocationCalculationsType>(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                    HydraulicBoundaryLocationCalculationsType.WaveHeight)
            };

            var exporter = new HydraulicBoundaryLocationCalculationsForTargetProbabilitiesExporter(calculationsForTargetProbabilities, folderPath);

            try
            {
                // Call
                var isExported = false;
                void Call() => isExported = exporter.Export();

                // Assert
                string[] expectedCalculationNames =
                {
                    $"Waterstanden {ProbabilityFormattingHelper.Format(calculationsForTargetProbabilities.First().Item1.TargetProbability)}",
                    $"Golfhoogten {ProbabilityFormattingHelper.Format(calculationsForTargetProbabilities.Last().Item1.TargetProbability)}"
                };

                string[] expectedFilePaths =
                {
                    Path.Combine(folderPath, $"Waterstanden_{GetReturnPeriodText(calculationsForTargetProbabilities.First().Item1.TargetProbability)}.shp"),
                    Path.Combine(folderPath, $"Golfhoogten_{GetReturnPeriodText(calculationsForTargetProbabilities.Last().Item1.TargetProbability)}.shp")
                };

                TestHelper.AssertLogMessagesAreGenerated(Call, new[]
                {
                    $"Exporteren van '{expectedCalculationNames[0]}' is gestart.",
                    $"Gegevens van '{expectedCalculationNames[0]}' zijn geëxporteerd naar bestand '{expectedFilePaths[0]}'.",
                    $"Exporteren van '{expectedCalculationNames[1]}' is gestart.",
                    $"Gegevens van '{expectedCalculationNames[1]}' zijn geëxporteerd naar bestand '{expectedFilePaths[1]}'."
                });
                Assert.IsTrue(isExported);
                Assert.IsTrue(expectedFilePaths.All(File.Exists));
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        private static string GetReturnPeriodText(double targetProbability)
        {
            return ReturnPeriodFormattingHelper.FormatFromProbability(targetProbability);
        }
    }
}