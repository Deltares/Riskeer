﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.IO.Exporters;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.IO.WaveConditions;

namespace Riskeer.GrassCoverErosionOutwards.IO.Test.Exporters
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsExporterTest
    {
        private readonly string testFilePath = TestHelper.GetScratchPadPath($"{nameof(GrassCoverErosionOutwardsWaveConditionsExporterTest)}.csv");

        [Test]
        public void Constructor_CalculationsNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionOutwardsWaveConditionsExporter(null, testFilePath, i => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_GetTargetProbabilityFuncNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionOutwardsWaveConditionsExporter(Array.Empty<GrassCoverErosionOutwardsWaveConditionsCalculation>(), testFilePath, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getTargetProbabilityFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Call
            var waveConditionsExporter = new GrassCoverErosionOutwardsWaveConditionsExporter(Array.Empty<GrassCoverErosionOutwardsWaveConditionsCalculation>(), testFilePath, i => "1/100");

            // Assert
            Assert.IsInstanceOf<WaveConditionsExporterBase>(waveConditionsExporter);
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
                    new GrassCoverErosionOutwardsWaveConditionsCalculation()
                };

                var exporter = new GrassCoverErosionOutwardsWaveConditionsExporter(calculationsWithoutOutput, filePath, i => "1/100");

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Doelkans, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}";
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
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
                    {
                        Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create()
                    }
                };

                var exporter = new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath, i => "1/100");

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Doelkans, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_ValidDataWithCalculationTypeWaveRunUpAndWaveImpact_ValidFile()
        {
            // Setup
            string folderName = $"{nameof(GrassCoverErosionOutwardsWaveConditionsExporterTest)}.{nameof(Export_ValidDataWithCalculationTypeWaveRunUpAndWaveImpact_ValidFile)}";
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var calculations = new[]
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
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
                            WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit
                        },
                        Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(
                            new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            })
                    }
                };

                var exporter = new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath, i => "1/100");

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Doelkans, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfoploop), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfklap), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_ValidDataWithCalculationTypeWaveRunUp_ValidFile()
        {
            // Setup
            string folderName = $"{nameof(GrassCoverErosionOutwardsWaveConditionsExporterTest)}.{nameof(Export_ValidDataWithCalculationTypeWaveRunUp_ValidFile)}";
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var calculations = new[]
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
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
                            WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit,
                            CalculationType = GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp
                        },
                        Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(
                            new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            })
                    }
                };

                var exporter = new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath, i => "1/100");

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Doelkans, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfoploop), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_ValidDataWithCalculationTypeWaveImpact_ValidFile()
        {
            // Setup
            string folderName = $"{nameof(GrassCoverErosionOutwardsWaveConditionsExporterTest)}.{nameof(Export_ValidDataWithCalculationTypeWaveImpact_ValidFile)}";
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var calculations = new[]
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
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
                            WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit,
                            CalculationType = GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact
                        },
                        Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(
                            new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            })
                    }
                };

                var exporter = new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath, i => "1/100");

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Doelkans, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfklap), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_ValidDataWithCalculationTypeTailorMadeWaveImpact_ValidFile()
        {
            // Setup
            string folderName = $"{nameof(GrassCoverErosionOutwardsWaveConditionsExporterTest)}.{nameof(Export_ValidDataWithCalculationTypeWaveImpact_ValidFile)}";
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var calculations = new[]
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
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
                            WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit,
                            CalculationType = GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact
                        },
                        Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(
                            new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            })
                    }
                };

                var exporter = new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath, i => "1/100");

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Doelkans, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfklap voor toets op maat), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_ValidDataWithCalculationTypeWaveRunUpAndTailorMadeWaveImpact_ValidFile()
        {
            // Setup
            string folderName = $"{nameof(GrassCoverErosionOutwardsWaveConditionsExporterTest)}.{nameof(Export_ValidDataWithCalculationTypeWaveImpact_ValidFile)}";
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var calculations = new[]
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
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
                            WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit,
                            CalculationType = GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact
                        },
                        Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(
                            new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            })
                    }
                };

                var exporter = new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath, i => "1/100");

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Doelkans, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfoploop), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfklap voor toets op maat), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }

        [Test]
        public void Export_ValidDataWithCalculationTypeAll_ValidFile()
        {
            // Setup
            string folderName = $"{nameof(GrassCoverErosionOutwardsWaveConditionsExporterTest)}.{nameof(Export_ValidDataWithCalculationTypeWaveImpact_ValidFile)}";
            string directoryPath = TestHelper.GetScratchPadPath(folderName);
            using (new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), folderName))
            {
                string filePath = Path.Combine(directoryPath, "test.csv");

                var calculations = new[]
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculation
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
                            WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit,
                            CalculationType = GrassCoverErosionOutwardsWaveConditionsCalculationType.All
                        },
                        Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create(
                            new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            }, new[]
                            {
                                new TestWaveConditionsOutput()
                            })
                    }
                };

                var exporter = new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath, i => "1/100");

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(isExported);
                Assert.IsTrue(File.Exists(filePath));
                string fileContent = File.ReadAllText(filePath);
                string expectedText = $"Naam berekening, Naam HB locatie, X HB locatie (RD) [m], Y HB locatie (RD) [m], Naam voorlandprofiel, Dam gebruikt, Voorlandgeometrie gebruikt, Type bekleding, Doelkans, Waterstand [m+NAP], Golfhoogte (Hs) [m], Golfperiode (Tp) [s], Golfrichting t.o.v. dijknormaal [°], Golfrichting t.o.v. Noord [°]{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfoploop), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfklap), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}" +
                                      $"aCalculation, aLocation, 44.000, 123.456, foreshoreA, nee, nee, Gras (golfklap voor toets op maat), 1/100, 1.10, 2.20, 3.30, 4.40, 5.50{Environment.NewLine}";
                Assert.AreEqual(expectedText, fileContent);
            }
        }
    }
}