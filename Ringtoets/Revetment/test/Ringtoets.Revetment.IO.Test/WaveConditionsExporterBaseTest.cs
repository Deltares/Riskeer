// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO.Test
{
    [TestFixture]
    public class WaveConditionsExporterBaseTest
    {
        private readonly string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO, "test.csv");

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Call
            var waveConditionsExporter = new TestWaveConditionsExporter(new ExportableWaveConditions[0], testFilePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(waveConditionsExporter);
        }

        [Test]
        public void Constructor_ExportableWaveConditionsCollectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestWaveConditionsExporter(null, testFilePath);

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
            string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO, "test_.csv");
            string invalidFilePath = filePath.Replace("_", ">");
            var waveConditionsExporter = new TestWaveConditionsExporter(new ExportableWaveConditions[0], invalidFilePath);

            // Call
            bool isExported = true;
            Action call = () => isExported = waveConditionsExporter.Export();

            // Assert
            string expectedMessage = string.Format("Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{0}'. " +
                                                   "Er zijn geen golfrandvoorwaarden geëxporteerd.", invalidFilePath);
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
            Assert.IsFalse(isExported);
        }

        [Test]
        public void Export_ValidData_ReturnTrue()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO,
                                                              "Export_ValidData_ReturnTrue");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.csv");

            try
            {
                var waveConditionsExporter = new TestWaveConditionsExporter(new ExportableWaveConditions[0], filePath);

                // Call
                bool isExported = waveConditionsExporter.Export();

                // Assert
                Assert.IsTrue(isExported);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void Export_ValidData_ValidFile()
        {
            // Setup
            ExportableWaveConditions[] exportableWaveConditions =
            {
                new ExportableWaveConditions("blocksName", new WaveConditionsInput
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                    {
                        DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(12.34567)
                    },
                    LowerBoundaryRevetment = (RoundedDouble) 5.68,
                    UpperBoundaryRevetment = (RoundedDouble) 7.214,
                    StepSize = WaveConditionsInputStepSize.One,
                    LowerBoundaryWaterLevels = (RoundedDouble) 2.689,
                    UpperBoundaryWaterLevels = (RoundedDouble) 77.8249863247
                }, CreateWaveConditionsOutputForExport(1.11111, 2.22222, 3.33333, 4.4, 5.5555555), CoverType.StoneCoverBlocks),
                new ExportableWaveConditions("columnsName", new WaveConditionsInput
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(8, "aLocation", 44, 123.456)
                    {
                        DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(28.36844)
                    },
                    LowerBoundaryRevetment = (RoundedDouble) 1.384,
                    UpperBoundaryRevetment = (RoundedDouble) 11.54898963,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1.98699,
                    UpperBoundaryWaterLevels = (RoundedDouble) 84.26548
                }, CreateWaveConditionsOutputForExport(3.33333, 1.11111, 4.44444, 2.2, 6.66666), CoverType.StoneCoverColumns)
            };

            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Revetment.IO,
                                                              "Export_ValidData_ValidFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.csv");

            try
            {
                // Call
                WaveConditionsWriter.WriteWaveConditions(exportableWaveConditions, filePath);

                // Assert
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                Assert.AreEqual("Naam berekening, Naam HR locatie, X HR locatie (RD) [m], Y HR locatie (RD) [m], Naam voorlandprofiel, Dam aanwezig, Voorlandgeometrie aanwezig, Type bekleding, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]\r\n" +
                                "blocksName, , 0.000, 0.000, , nee, nee, Steen (blokken), 1.11, 2.22, 3.33, 4.40, 5.56\r\n" +
                                "columnsName, aLocation, 44.000, 123.456, , nee, nee, Steen (zuilen), 3.33, 1.11, 4.44, 2.20, 6.67\r\n",
                                fileContent);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        private class TestWaveConditionsExporter : WaveConditionsExporterBase
        {
            public TestWaveConditionsExporter(IEnumerable<ExportableWaveConditions> exportableWaveConditionsCollection, string filePath) : base(exportableWaveConditionsCollection, filePath) {}
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