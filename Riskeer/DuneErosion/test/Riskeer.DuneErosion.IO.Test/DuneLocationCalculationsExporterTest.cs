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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.IO.Test
{
    [TestFixture]
    public class DuneLocationCalculationsExporterTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine(nameof(DuneLocationCalculationsExporterTest), "test.bnd"));

            // Call
            var exporter = new DuneLocationCalculationsExporter(Enumerable.Empty<ExportableDuneLocationCalculation>(),
                                                                filePath,
                                                                new TestTypeConverter());

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_ExportableDuneLocationCalculationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneLocationCalculationsExporter(null, "IAmValid.bnd", new TestTypeConverter());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("exportableDuneLocationCalculations", exception.ParamName);
        }

        [Test]
        public void Constructor_ProbabilityConverterNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneLocationCalculationsExporter(Enumerable.Empty<ExportableDuneLocationCalculation>(),
                                                                           "IAmValid.bnd",
                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("probabilityConverter", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("c:\\>")]
        public void Constructor_FilePathInvalid_ThrowArgumentException(string filePath)
        {
            // Call
            TestDelegate test = () => new DuneLocationCalculationsExporter(Enumerable.Empty<ExportableDuneLocationCalculation>(),
                                                                           filePath,
                                                                           new TestTypeConverter());

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Export_ValidData_ReturnTrueAndWritesFile()
        {
            // Setup
            var calculationWithoutOutput = new ExportableDuneLocationCalculation(
                new DuneLocationCalculation(CreateDuneLocationForExport(9, 9740, 1.9583e-4)),
                0.5,
                "A");

            var calculationWithUncalculatedOutput = new ExportableDuneLocationCalculation(
                new DuneLocationCalculation(CreateDuneLocationForExport(10, 9770.1, 1.9583e-4))
                {
                    Output = CreateDuneLocationCalculationOutputForExport(double.NaN, double.NaN, double.NaN)
                },
                0.25,
                "B");

            var calculationWithOutput = new ExportableDuneLocationCalculation(
                new DuneLocationCalculation(CreateDuneLocationForExport(11, 9771.34, 1.337e-4))
                {
                    Output = CreateDuneLocationCalculationOutputForExport(5.89, 14.11, 8.53)
                },
                0.1,
                "C");

            ExportableDuneLocationCalculation[] exportableDuneLocationCalculations =
            {
                calculationWithoutOutput,
                calculationWithUncalculatedOutput,
                calculationWithOutput
            };

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_ValidData_ReturnTrueAndWritesFile));
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_ValidData_ReturnTrueAndWritesFile)))
            {
                string filePath = Path.Combine(directoryPath, "test.bnd");

                var exporter = new DuneLocationCalculationsExporter(exportableDuneLocationCalculations,
                                                                    filePath,
                                                                    new TestTypeConverter());

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Kv\tNr\tRp\tHs\tTp\tTm-1,0\tD50\t_WBI2017_ID\t_WBI2017_Categoriegrens\t_WBI2017_Waarde{Environment.NewLine}" +
                                      $"*Kustvaknummer\tMetrering\tRekenpeil\tSignificante golfhoogte\tPiekperiode\tSpectrale periode\tKorreldiameter\tScenario\tCategoriegrens\tPfdsn{Environment.NewLine}" +
                                      $"*[-]\t[dam]\t[m+NAP]\t[m]\t[s]\t[s]\t[m]\t[-]\t[-]\t[1/jaar]{Environment.NewLine}" +
                                      $"9\t9740\t*\t*\t*\t*\t0.000196\tA (Pfdsn = Converted 0.5 jaar)\tA\t0.5{Environment.NewLine}" +
                                      $"10\t9770.1\t*\t*\t*\t*\t0.000196\tB (Pfdsn = Converted 0.25 jaar)\tB\t0.25{Environment.NewLine}" +
                                      $"11\t9771.3\t5.89\t8.53\t14.11\t*\t0.000134\tC (Pfdsn = Converted 0.1 jaar)\tC\t0.1{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_InvalidDirectoryRights_LogErrorAndReturnFalse()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse));
            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_InvalidDirectoryRights_LogErrorAndReturnFalse)))
            {
                string filePath = Path.Combine(directoryPath, "test.bnd");
                var exporter = new DuneLocationCalculationsExporter(Enumerable.Empty<ExportableDuneLocationCalculation>(),
                                                                    filePath,
                                                                    new TestTypeConverter());

                disposeHelper.LockDirectory(FileSystemRights.Write);
                var isExported = true;

                // Call
                Action call = () => isExported = exporter.Export();

                // Assert
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. "
                                         + "Er zijn geen hydraulische belastingenlocaties geëxporteerd.";
                TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
                Assert.IsFalse(isExported);
            }
        }

        private static DuneLocationCalculationOutput CreateDuneLocationCalculationOutputForExport(double waterLevel, double wavePeriod, double waveHeight)
        {
            return new DuneLocationCalculationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationCalculationOutput.ConstructionProperties
            {
                WaterLevel = waterLevel,
                WavePeriod = wavePeriod,
                WaveHeight = waveHeight
            });
        }

        private static DuneLocation CreateDuneLocationForExport(int coastalAreaId, double offset, double d50)
        {
            return new DuneLocation(new TestHydraulicBoundaryLocation(),
                                    string.Empty,
                                    new Point2D(0.0, 0.0),
                                    new DuneLocation.ConstructionProperties
                                    {
                                        CoastalAreaId = coastalAreaId,
                                        Offset = offset,
                                        D50 = d50
                                    });
        }

        private class TestTypeConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                return "Converted " + ((double) value).ToString(culture);
            }
        }
    }
}