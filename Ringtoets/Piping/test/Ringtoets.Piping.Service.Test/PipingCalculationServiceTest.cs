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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingCalculationServiceTest
    {
        private const string averagingSoilLayerPropertiesMessage = "Meerdere aaneengesloten deklagen gevonden. De grondeigenschappen worden bepaald door het nemen van een gewogen gemiddelde, mits de standaardafwijkingen en verschuivingen voor alle lagen gelijk zijn.";
        private double testSurfaceLineTopLevel;
        private PipingCalculationScenario testCalculation;

        [SetUp]
        public void Setup()
        {
            testCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            testSurfaceLineTopLevel = testCalculation.InputParameters.SurfaceLine.Points.Max(p => p.Z);
        }

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationService.Validate(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.Name = name;

            // Call
            Action call = () => PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
        }

        [Test]
        public void Validate_InvalidPipingCalculationWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            var output = new TestPipingOutput();
            PipingCalculation invalidPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithInvalidInput();
            invalidPipingCalculation.Output = output;

            // Call
            bool isValid = PipingCalculationService.Validate(invalidPipingCalculation);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreSame(output, invalidPipingCalculation.Output);
        }

        [Test]
        public void Validate_InvalidCalculationInput_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Name = name
            };

            // Call
            var isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(7, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[2]);
                Assert.AreEqual("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[3]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'uittredepunt' moet een concreet getal zijn.", msgs[4]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'intredepunt' moet een concreet getal zijn.", msgs[5]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_HydraulicBoundaryLocationNotCalculated_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.Name = name;
            testCalculation.InputParameters.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            testCalculation.InputParameters.UseAssessmentLevelManualInput = false;

            // Call
            var isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Kan de stijghoogte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidManualAssessmentLevel_LogsErrorAndReturnsFalse(double assessmentLevel)
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.UseAssessmentLevelManualInput = true;
            testCalculation.InputParameters.AssessmentLevel = (RoundedDouble) assessmentLevel;
            testCalculation.Name = name;

            // Call
            var isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: De waarde voor 'toetspeil' moet een concreet getal zijn.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Kan de stijghoogte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutEntryPointL_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.EntryPointL = RoundedDouble.NaN;
            testCalculation.Name = name;

            // Call
            var isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: De waarde voor 'intredepunt' moet een concreet getal zijn.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutExitPointL_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.ExitPointL = RoundedDouble.NaN;
            testCalculation.Name = name;

            // Call
            var isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: De waarde voor 'uittredepunt' moet een concreet getal zijn.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutSurfaceLine_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.SurfaceLine = null;
            testCalculation.InputParameters.ExitPointL = (RoundedDouble) 0.9;
            testCalculation.InputParameters.EntryPointL = (RoundedDouble) 0.1;
            testCalculation.Name = name;

            // Call
            var isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithSurfaceLineOneOutOfFourDitchPoints_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            Point3D[] geometry = testCalculation.InputParameters.SurfaceLine.Points;
            const string surfaceLineName = "surfaceLineA";
            var surfaceLineMissingCharacteristicPoint = new RingtoetsPipingSurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLineMissingCharacteristicPoint.SetGeometry(geometry);
            surfaceLineMissingCharacteristicPoint.SetDitchDikeSideAt(geometry[2]);

            testCalculation.InputParameters.SurfaceLine = surfaceLineMissingCharacteristicPoint;
            testCalculation.InputParameters.ExitPointL = (RoundedDouble) 0.9;
            testCalculation.InputParameters.EntryPointL = (RoundedDouble) 0.1;
            testCalculation.Name = name;

            // Call
            var isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());

                string expected = $"Validatie mislukt: De sloot in de hoogtegeometrie {surfaceLineName} is niet correct. Niet alle 4 punten zijn gedefinieerd of de volgorde is incorrect.";
                Assert.AreEqual(expected, msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutStochasticSoilProfile_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.InputParameters.StochasticSoilProfile = null;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_StochasticSoilProfileBelowSurfaceLine_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var topLayer = new PipingSoilLayer(testSurfaceLineTopLevel - 1e-6)
            {
                IsAquifer = false,
                BelowPhreaticLevelMean = 15,
                BelowPhreaticLevelDeviation = 2,
                BelowPhreaticLevelShift = 0
            };
            var bottomLayer = new PipingSoilLayer(2.0)
            {
                IsAquifer = true,
                DiameterD70CoefficientOfVariation = 0,
                DiameterD70Mean = 1e-4,
                PermeabilityCoefficientOfVariation = 0.5,
                PermeabilityMean = 1
            };
            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = new PipingSoilProfile(
                string.Empty, 0.0,
                new[]
                {
                    topLayer,
                    bottomLayer
                },
                SoilProfileType.SoilProfile1D, -1);

            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: Hoogtegeometrie ligt (deels) boven de ondergrondschematisatie.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutAquiferLayer_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var aquitardLayer = new PipingSoilLayer(2.0)
            {
                IsAquifer = false
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    aquitardLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[1]);
                Assert.AreEqual("Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                Assert.AreEqual("Validatie mislukt: Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.", msgs[3]);
                Assert.AreEqual("Validatie mislukt: Geen watervoerende laag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[4]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutAquitardLayer_LogsWarningsAndReturnsTrue()
        {
            // Setup
            const string name = "<very nice name>";

            var aquiferLayer = new PipingSoilLayer(10.56)
            {
                IsAquifer = true,
                DiameterD70CoefficientOfVariation = 0,
                DiameterD70Mean = 1e-4,
                PermeabilityCoefficientOfVariation = 0.5,
                PermeabilityMean = 1
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    aquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[1]);
                Assert.AreEqual("Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        public void Validate_WithoutCoverageLayer_LogsWarningsAndReturnsTrue()
        {
            // Setup
            const string name = "<very nice name>";

            var coverageLayerAboveSurfaceLine = new PipingSoilLayer(13.0)
            {
                IsAquifer = false
            };
            var bottomAquiferLayer = new PipingSoilLayer(11.0)
            {
                IsAquifer = true,
                DiameterD70CoefficientOfVariation = 0,
                DiameterD70Mean = 1e-4,
                PermeabilityCoefficientOfVariation = 0.5,
                PermeabilityMean = 1
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    coverageLayerAboveSurfaceLine,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[1]);
                Assert.AreEqual("Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        public void Validate_MultipleCoverageLayer_LogsWarningAndReturnsTrue()
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            const double belowPhreaticLevelDeviation = 0.5;
            const int belowPhreaticLevelShift = 1;
            const double belowPhreaticLevelMeanBase = 15.0;

            var topCoverageLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                BelowPhreaticLevelMean = belowPhreaticLevelMeanBase + belowPhreaticLevelShift + random.NextDouble()
            };
            var middleCoverageLayer = new PipingSoilLayer(8.5)
            {
                IsAquifer = false,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                BelowPhreaticLevelMean = belowPhreaticLevelMeanBase + belowPhreaticLevelShift + random.NextDouble()
            };
            var bottomAquiferLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                PermeabilityCoefficientOfVariation = 0.5,
                PermeabilityMean = 1,
                DiameterD70CoefficientOfVariation = 0,
                DiameterD70Mean = 1e-4
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    topCoverageLayer,
                                                    middleCoverageLayer,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual(averagingSoilLayerPropertiesMessage, msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Validate_IncompleteDiameterD70Definition_LogsErrorAndReturnsFalse(bool meanSet, bool deviationSet)
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                PermeabilityCoefficientOfVariation = 0.5,
                PermeabilityMean = 1
            };
            if (meanSet)
            {
                incompletePipingSoilLayer.DiameterD70Mean = 0.1 + random.NextDouble();
            }
            if (deviationSet)
            {
                incompletePipingSoilLayer.DiameterD70CoefficientOfVariation = random.NextDouble();
            }

            var completeLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevelDeviation = random.GetFromRange(1e-6, 5.0),
                BelowPhreaticLevelMean = random.GetFromRange(15.0, 999.999),
                BelowPhreaticLevelShift = random.GetFromRange(1e-6, 10.0)
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    completeLayer,
                                                    incompletePipingSoilLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan de definitie voor het 70%-fraktiel van de korreldiameter van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(6.2e-5)]
        [TestCase(5.1e-3)]
        public void Validate_InvalidDiameterD70Value_LogsWarningAndReturnsTrue(double diameter70Value)
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var coverageLayerInvalidD70 = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                PermeabilityCoefficientOfVariation = 0.5,
                PermeabilityMean = 1,
                DiameterD70Mean = diameter70Value,
                DiameterD70CoefficientOfVariation = 0
            };
            var validLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevelDeviation = random.GetFromRange(1e-6, 5.0),
                BelowPhreaticLevelMean = random.GetFromRange(15.0, 999.999),
                BelowPhreaticLevelShift = random.GetFromRange(1e-6, 10.0)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    validLayer,
                                                    coverageLayerInvalidD70
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.Name = name;
            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual($"Rekenwaarde voor d70 ({new RoundedDouble(6, diameter70Value)} m) ligt buiten het geldigheidsbereik van dit model. Geldige waarden liggen tussen 0.000063 m en 0.0005 m.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Validate_IncompletePermeabilityDefinition_LogsErrorAndReturnsFalse(bool meanSet, bool deviationSet)
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                DiameterD70CoefficientOfVariation = 0,
                DiameterD70Mean = 1e-4
            };
            if (meanSet)
            {
                incompletePipingSoilLayer.PermeabilityMean = 0.1 + random.NextDouble();
            }
            if (deviationSet)
            {
                incompletePipingSoilLayer.PermeabilityCoefficientOfVariation = random.NextDouble();
            }

            var completeLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevelDeviation = random.GetFromRange(1e-6, 999.999),
                BelowPhreaticLevelMean = random.GetFromRange(10.0, 999.999),
                BelowPhreaticLevelShift = random.GetFromRange(1e-6, 10.0)
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    completeLayer,
                                                    incompletePipingSoilLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan de definitie voor de doorlatendheid van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        public void Validate_IncompleteSaturatedVolumicWeightDefinition_LogsErrorAndReturnsFalse(bool meanSet, bool deviationSet, bool shiftSet)
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false
            };
            if (deviationSet)
            {
                incompletePipingSoilLayer.BelowPhreaticLevelDeviation = random.NextDouble();
            }
            if (shiftSet)
            {
                incompletePipingSoilLayer.BelowPhreaticLevelShift = random.NextDouble();
            }
            if (meanSet)
            {
                incompletePipingSoilLayer.BelowPhreaticLevelMean = 0.1 + incompletePipingSoilLayer.BelowPhreaticLevelShift + random.NextDouble();
            }

            var completeLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                PermeabilityCoefficientOfVariation = 0.5,
                PermeabilityMean = 1,
                DiameterD70CoefficientOfVariation = 0,
                DiameterD70Mean = 1e-4
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    incompletePipingSoilLayer,
                                                    completeLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            testCalculation.Name = name;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_SaturatedCoverageLayerVolumicWeightLessThanWaterVolumicWeight_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var coverageLayerInvalidSaturatedVolumicWeight = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevelMean = 9.81,
                BelowPhreaticLevelDeviation = 2,
                BelowPhreaticLevelShift = 0
            };
            var validLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                PermeabilityCoefficientOfVariation = 0.5,
                PermeabilityMean = 1,
                DiameterD70Mean = 0.0002,
                DiameterD70CoefficientOfVariation = 0
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    coverageLayerInvalidSaturatedVolumicWeight,
                                                    validLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.Name = name;
            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;

            var isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual(
                    "Validatie mislukt: Het verzadigd volumetrisch gewicht van de deklaag moet groter zijn dan het volumetrisch gewicht van water.",
                    msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_SaturatedCoverageLayerLessThanWaterLayerAndMissingSaturatedParameter_LogsErrorOnlyForIncompleteDefinition()
        {
            // Setup
            const string name = "<very nice name>";

            var topCoverageLayer = new PipingSoilLayer(testSurfaceLineTopLevel)
            {
                IsAquifer = false,
                BelowPhreaticLevelMean = 5,
                BelowPhreaticLevelDeviation = 2,
                BelowPhreaticLevelShift = 0
            };
            var middleCoverageLayerMissingParameter = new PipingSoilLayer(8.5)
            {
                IsAquifer = false,
                BelowPhreaticLevelMean = 5,
                BelowPhreaticLevelDeviation = 2,
                BelowPhreaticLevelShift = double.NaN
            };
            var bottomAquiferLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                PermeabilityCoefficientOfVariation = 0.3,
                PermeabilityMean = 0.6,
                DiameterD70Mean = 0.0002,
                DiameterD70CoefficientOfVariation = 0
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    topCoverageLayer,
                                                    middleCoverageLayerMissingParameter,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            testCalculation.Name = name;
            testCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;

            // Call
            Action call = () => PipingCalculationService.Validate(testCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                Assert.AreEqual(averagingSoilLayerPropertiesMessage, msgs[1]);
                Assert.AreEqual(
                    "Validatie mislukt: Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.",
                    msgs[2]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs.Last());
            });
        }

        [Test]
        public void Validate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingInput input = testCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingCalculationService.Validate(testCalculation);

                // Assert
                AssertSubCalculatorInputs(input);
            }
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationService.Calculate(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Calculate_ValidPipingCalculation_LogStartAndEndOfValidatingInputsAndCalculation()
        {
            // Setup
            const string name = "<very nice name>";

            testCalculation.Name = name;

            Action call = () =>
            {
                // Precondition
                Assert.IsTrue(PipingCalculationService.Validate(testCalculation));

                // Call
                PipingCalculationService.Calculate(testCalculation);
            };

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs.First());
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[1]);

                StringAssert.StartsWith($"Berekening van '{name}' gestart om: ", msgs[2]);
                StringAssert.StartsWith($"Berekening van '{name}' beëindigd om: ", msgs.Last());
            });
        }

        [Test]
        public void Calculate_ValidPipingCalculationNoOutput_ShouldSetOutput()
        {
            // Precondition
            Assert.IsNull(testCalculation.Output);
            Assert.IsTrue(PipingCalculationService.Validate(testCalculation));

            // Call
            PipingCalculationService.Calculate(testCalculation);

            // Assert
            PipingOutput pipingOutput = testCalculation.Output;
            Assert.IsNotNull(pipingOutput);
            Assert.IsFalse(double.IsNaN(pipingOutput.UpliftEffectiveStress));
            Assert.IsFalse(double.IsNaN(pipingOutput.UpliftZValue));
            Assert.IsFalse(double.IsNaN(pipingOutput.UpliftFactorOfSafety));
            Assert.IsFalse(double.IsNaN(pipingOutput.HeaveZValue));
            Assert.IsFalse(double.IsNaN(pipingOutput.HeaveFactorOfSafety));
            Assert.IsFalse(double.IsNaN(pipingOutput.SellmeijerZValue));
            Assert.IsFalse(double.IsNaN(pipingOutput.SellmeijerFactorOfSafety));
        }

        [Test]
        public void Calculate_ValidPipingCalculationWithOutput_ShouldChangeOutput()
        {
            // Setup
            var output = new TestPipingOutput();

            testCalculation.Output = output;

            // Precondition
            Assert.IsTrue(PipingCalculationService.Validate(testCalculation));

            // Call
            PipingCalculationService.Calculate(testCalculation);

            // Assert
            Assert.AreNotSame(output, testCalculation.Output);
        }

        [Test]
        public void Calculate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingInput input = testCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingCalculationService.Calculate(testCalculation);

                // Assert
                AssertSubCalculatorInputs(input);
            }
        }

        private static void AssertSubCalculatorInputs(PipingInput input)
        {
            var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
            HeaveCalculatorStub heaveCalculator = testFactory.LastCreatedHeaveCalculator;
            UpliftCalculatorStub upliftCalculator = testFactory.LastCreatedUpliftCalculator;
            SellmeijerCalculatorStub sellmeijerCalculator = testFactory.LastCreatedSellmeijerCalculator;

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue(),
                            heaveCalculator.DTotal,
                            input.ThicknessCoverageLayer.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                            heaveCalculator.HExit,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(input.CriticalHeaveGradient, heaveCalculator.Ich);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                            heaveCalculator.PhiPolder,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(input.PiezometricHeadExit.Value, heaveCalculator.PhiExit);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(),
                            heaveCalculator.RExit,
                            input.DampingFactorExit.GetAccuracy());

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                            upliftCalculator.HExit,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(input.AssessmentLevel.Value, upliftCalculator.HRiver);
            Assert.AreEqual(input.UpliftModelFactor, upliftCalculator.ModelFactorUplift);
            Assert.AreEqual(input.PiezometricHeadExit.Value, upliftCalculator.PhiExit);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                            upliftCalculator.PhiPolder,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(),
                            upliftCalculator.RExit,
                            input.DampingFactorExit.GetAccuracy());
            Assert.AreEqual(input.WaterVolumetricWeight, upliftCalculator.VolumetricWeightOfWater);

            RoundedDouble effectiveThickness = PipingSemiProbabilisticDesignValueFactory.GetEffectiveThicknessCoverageLayer(input).GetDesignValue();
            RoundedDouble saturatedVolumicWeight = PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(input).GetDesignValue();
            RoundedDouble effectiveStress = effectiveThickness * (saturatedVolumicWeight - input.WaterVolumetricWeight);
            Assert.AreEqual(effectiveStress, upliftCalculator.EffectiveStress,
                            effectiveStress.GetAccuracy());

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(input).GetDesignValue(),
                            sellmeijerCalculator.SeepageLength,
                            input.SeepageLength.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(),
                            sellmeijerCalculator.HExit,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(input.AssessmentLevel.Value, sellmeijerCalculator.HRiver);
            Assert.AreEqual(input.WaterKinematicViscosity, sellmeijerCalculator.KinematicViscosityWater);
            Assert.AreEqual(input.SellmeijerModelFactor, sellmeijerCalculator.ModelFactorPiping);
            Assert.AreEqual(input.SellmeijerReductionFactor, sellmeijerCalculator.Rc);
            Assert.AreEqual(input.WaterVolumetricWeight, sellmeijerCalculator.VolumetricWeightOfWater);
            Assert.AreEqual(input.WhitesDragCoefficient, sellmeijerCalculator.WhitesDragCoefficient);
            Assert.AreEqual(input.BeddingAngle, sellmeijerCalculator.BeddingAngle);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue(),
                            sellmeijerCalculator.DTotal,
                            input.ThicknessCoverageLayer.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDiameter70(input).GetDesignValue(),
                            sellmeijerCalculator.D70,
                            input.Diameter70.GetAccuracy());
            Assert.AreEqual(input.MeanDiameter70, sellmeijerCalculator.D70Mean);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(input).GetDesignValue(),
                            sellmeijerCalculator.DAquifer,
                            input.ThicknessAquiferLayer.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(input).GetDesignValue(),
                            sellmeijerCalculator.DarcyPermeability,
                            input.DarcyPermeability.GetAccuracy());
            Assert.AreEqual(input.SandParticlesVolumicWeight, sellmeijerCalculator.GammaSubParticles);
            Assert.AreEqual(input.Gravity, sellmeijerCalculator.Gravity);
        }
    }
}