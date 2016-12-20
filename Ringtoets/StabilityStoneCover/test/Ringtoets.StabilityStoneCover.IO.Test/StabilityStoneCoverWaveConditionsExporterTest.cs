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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO;
using Ringtoets.Revetment.TestUtil;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.IO.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsExporterTest
    {
        private readonly string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.StabilityStoneCover.IO, "test.csv");

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Call
            var exporter = new StabilityStoneCoverWaveConditionsExporter(new StabilityStoneCoverWaveConditionsCalculation[0], testFilePath);

            // Assert
            Assert.IsInstanceOf<WaveConditionsExporterBase>(exporter);
        }

        [Test]
        public void Constructor_CalculationsNull_ExpectedValues()
        {
            // Call
            TestDelegate call = () => new StabilityStoneCoverWaveConditionsExporter(null, testFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Export_CalculationsWithoutOutput_FileWithOnlyHeader()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.StabilityStoneCover.IO,
                                                              "Export_CalculationsWithoutOutput_FileWithOnlyHeader");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.csv");

            var calculationsWithoutOutput = new[]
            {
                new StabilityStoneCoverWaveConditionsCalculation()
            };

            var exporter = new StabilityStoneCoverWaveConditionsExporter(calculationsWithoutOutput, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                Assert.AreEqual("Naam berekening, Naam HR locatie, X HR locatie (RD) [m], Y HR locatie (RD) [m], Naam voorlandprofiel, Dam aanwezig, Voorlandgeometrie aanwezig, Type bekleding, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]\r\n", fileContent);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }

        [Test]
        public void Export_CalculationsWithoutHydraulicBoundaryLocation_FileWithOnlyHeader()
        {
            // Setup
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.StabilityStoneCover.IO,
                                                              "Export_CalculationsWithoutHydraulicBoundaryLocation_FileWithOnlyHeader");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.csv");

            var calculations = new[]
            {
                new StabilityStoneCoverWaveConditionsCalculation
                {
                    Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>()),
                }
            };

            var exporter = new StabilityStoneCoverWaveConditionsExporter(calculations, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                Assert.AreEqual("Naam berekening, Naam HR locatie, X HR locatie (RD) [m], Y HR locatie (RD) [m], Naam voorlandprofiel, Dam aanwezig, Voorlandgeometrie aanwezig, Type bekleding, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]\r\n", fileContent);
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
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.StabilityStoneCover.IO,
                                                              "Export_ValidData_ValidFile");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.csv");

            var calculations = new[]
            {
                new StabilityStoneCoverWaveConditionsCalculation
                {
                    Name = "aCalculation",
                    InputParameters =
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
                    },
                    Output = new StabilityStoneCoverWaveConditionsOutput(new[]
                    {
                        new TestWaveConditionsOutput()
                    }, new[]
                    {
                        new TestWaveConditionsOutput()
                    })
                }
            };

            var exporter = new StabilityStoneCoverWaveConditionsExporter(calculations, filePath);

            try
            {
                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                Assert.AreEqual("Naam berekening, Naam HR locatie, X HR locatie (RD) [m], Y HR locatie (RD) [m], Naam voorlandprofiel, Dam aanwezig, Voorlandgeometrie aanwezig, Type bekleding, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]\r\n" +
                                "aCalculation, aLocation, 44.000, 123.456, , nee, nee, Steen (zuilen), 1.10, 2.20, 3.30, 4.40, 5.50\r\n" +
                                "aCalculation, aLocation, 44.000, 123.456, , nee, nee, Steen (blokken), 1.10, 2.20, 3.30, 4.40, 5.50\r\n",
                                fileContent);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }
        }
    }
}