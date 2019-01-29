// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.IO;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.IO.WaveConditions;

namespace Riskeer.Revetment.IO.Test.WaveConditions
{
    [TestFixture]
    public class WaveConditionsExporterBaseTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(Constructor_ValidParameters_ExpectedValues)}.csv");

            // Call
            var waveConditionsExporter = new TestWaveConditionsExporter(new ExportableWaveConditions[0], filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(waveConditionsExporter);
        }

        [Test]
        public void Constructor_ExportableWaveConditionsCollectionNull_ThrowArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(Constructor_ExportableWaveConditionsCollectionNull_ThrowArgumentNullException)}.csv");

            // Call
            TestDelegate call = () => new TestWaveConditionsExporter(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("exportableWaveConditionsCollection", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestWaveConditionsExporter(new ExportableWaveConditions[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void Export_InvalidData_LogErrorAndFalse()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test_.csv");
            string invalidFilePath = filePath.Replace("_", ">");
            var waveConditionsExporter = new TestWaveConditionsExporter(new ExportableWaveConditions[0], invalidFilePath);

            // Call
            var isExported = true;
            Action call = () => isExported = waveConditionsExporter.Export();

            // Assert
            string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{invalidFilePath}'. " +
                                     "Er zijn geen golfcondities geëxporteerd.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsFalse(isExported);
        }

        [Test]
        public void Export_ValidData_ReturnTrue()
        {
            // Setup
            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_ValidData_ReturnTrue));

            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_ValidData_ReturnTrue)))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var waveConditionsExporter = new TestWaveConditionsExporter(new ExportableWaveConditions[0], filePath);

                // Call
                bool isExported = waveConditionsExporter.Export();

                // Assert
                Assert.IsTrue(isExported);
            }
        }

        [Test]
        public void Export_ValidData_ValidFile()
        {
            // Setup
            ExportableWaveConditions[] exportableWaveConditions =
            {
                new ExportableWaveConditions("blocksName", new TestWaveConditionsInput
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0),
                    LowerBoundaryRevetment = (RoundedDouble) 5.68,
                    UpperBoundaryRevetment = (RoundedDouble) 7.214,
                    StepSize = WaveConditionsInputStepSize.One,
                    LowerBoundaryWaterLevels = (RoundedDouble) 2.689,
                    UpperBoundaryWaterLevels = (RoundedDouble) 77.8249863247
                }, CreateWaveConditionsOutputForExport(1.11111, 2.22222, 3.33333, 4.4, 5.5555555), CoverType.StoneCoverBlocks, "A->B"),
                new ExportableWaveConditions("columnsName", new TestWaveConditionsInput
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(8, "aLocation", 44, 123.456),
                    LowerBoundaryRevetment = (RoundedDouble) 1.384,
                    UpperBoundaryRevetment = (RoundedDouble) 11.54898963,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1.98699,
                    UpperBoundaryWaterLevels = (RoundedDouble) 84.26548
                }, CreateWaveConditionsOutputForExport(3.33333, 1.11111, 4.44444, 2.2, 6.66666), CoverType.StoneCoverColumns, "B->C")
            };

            string directoryPath = TestHelper.GetScratchPadPath(nameof(Export_ValidData_ValidFile));
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(Export_ValidData_ValidFile)))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                // Call
                WaveConditionsWriter.WriteWaveConditions(exportableWaveConditions, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Categoriegrens, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}" +
                                      $"blocksName, , 0.000, 0.000, , nee, nee, Steen (blokken), A->B, 1.11, 2.22, 3.33, 4.40, 5.56{Environment.NewLine}" +
                                      $"columnsName, aLocation, 44.000, 123.456, , nee, nee, Steen (zuilen), B->C, 3.33, 1.11, 4.44, 2.20, 6.67{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        private class TestWaveConditionsExporter : WaveConditionsExporterBase
        {
            public TestWaveConditionsExporter(IEnumerable<ExportableWaveConditions> exportableWaveConditionsCollection, string filePath) :
                base(exportableWaveConditionsCollection, filePath) {}
        }

        private static WaveConditionsOutput CreateWaveConditionsOutputForExport(double waterLevel, double waveHeight,
                                                                                double wavePeakPeriod, double waveAngle,
                                                                                double waveDirection)
        {
            return new WaveConditionsOutput(waterLevel, waveHeight, wavePeakPeriod, waveAngle,
                                            waveDirection, double.NaN, double.NaN, double.NaN,
                                            double.NaN, CalculationConvergence.CalculatedConverged);
        }
    }
}