// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.IO.WaveConditions;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.IO.Exporters;

namespace Ringtoets.WaveImpactAsphaltCover.IO.Test.Exporters
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsExporterTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.csv");

            // Call
            var waveConditionsExporter = new WaveImpactAsphaltCoverWaveConditionsExporter(new WaveImpactAsphaltCoverWaveConditionsCalculation[0], filePath);

            // Assert
            Assert.IsInstanceOf<WaveConditionsExporterBase>(waveConditionsExporter);
        }

        [Test]
        public void Constructor_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath("test.csv");

            // Call
            TestDelegate call = () => new WaveImpactAsphaltCoverWaveConditionsExporter(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WaveImpactAsphaltCoverWaveConditionsExporter(new WaveImpactAsphaltCoverWaveConditionsCalculation[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void Export_CalculationsWithoutOutput_FileWithOnlyHeader()
        {
            // Setup
            const string folderName = nameof(Export_CalculationsWithoutOutput_FileWithOnlyHeader);
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var calculationsWithoutOutput = new[]
                {
                    new WaveImpactAsphaltCoverWaveConditionsCalculation()
                };

                var exporter = new WaveImpactAsphaltCoverWaveConditionsExporter(calculationsWithoutOutput, filePath);

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Categoriegrens, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_CalculationsWithoutHydraulicBoundaryLocation_FileWithOnlyHeader()
        {
            // Setup
            const string folderName = nameof(Export_CalculationsWithoutHydraulicBoundaryLocation_FileWithOnlyHeader);
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var calculations = new[]
                {
                    new WaveImpactAsphaltCoverWaveConditionsCalculation
                    {
                        Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
                    }
                };

                var exporter = new WaveImpactAsphaltCoverWaveConditionsExporter(calculations, filePath);

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Categoriegrens, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_ValidData_ValidFile()
        {
            // Setup
            string folderName = $"{nameof(WaveImpactAsphaltCoverWaveConditionsExporterTest)}.{nameof(Export_ValidData_ValidFile)}";
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var calculations = new[]
                {
                    new WaveImpactAsphaltCoverWaveConditionsCalculation
                    {
                        Name = "aCalculation",
                        InputParameters =
                        {
                            HydraulicBoundaryLocation = new HydraulicBoundaryLocation(8, "aLocation", 44, 123.456),
                            ForeshoreProfile = new TestForeshoreProfile("foreshoreA"),
                            LowerBoundaryRevetment = (RoundedDouble) 1.384,
                            UpperBoundaryRevetment = (RoundedDouble) 11.54898963,
                            StepSize = WaveConditionsInputStepSize.Half,
                            LowerBoundaryWaterLevels = (RoundedDouble) 1.98699,
                            UpperBoundaryWaterLevels = (RoundedDouble) 84.26548,
                            CategoryType = AssessmentSectionCategoryType.FactorizedLowerLimitNorm
                        },
                        Output = new WaveImpactAsphaltCoverWaveConditionsOutput(new[]
                        {
                            new TestWaveConditionsOutput()
                        })
                    }
                };

                var exporter = new WaveImpactAsphaltCoverWaveConditionsExporter(calculations, filePath);

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Categoriegrens, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Asfalt, C->D, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }
    }
}