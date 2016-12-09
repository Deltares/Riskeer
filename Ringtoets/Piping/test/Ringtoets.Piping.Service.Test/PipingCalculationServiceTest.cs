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
using System.Linq;
using Core.Common.Base.Data;
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
        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation pipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            pipingCalculation.Name = name;

            // Call
            Action call = () => PipingCalculationService.Validate(pipingCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
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
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(7, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[2]);
                Assert.AreEqual("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[3]);
                Assert.AreEqual("Validatie mislukt: Er is geen concreet getal ingevoerd voor 'Intredepunt'.", msgs[4]);
                Assert.AreEqual("Validatie mislukt: Er is geen concreet getal ingevoerd voor 'Uittredepunt'.", msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_HydraulicBoundaryLocationNotCalculated_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.Name = name;
            calculation.InputParameters.HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            calculation.InputParameters.UseAssessmentLevelManualInput = false;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Kan de stijghoogte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
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

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.UseAssessmentLevelManualInput = true;
            calculation.InputParameters.AssessmentLevel = (RoundedDouble) assessmentLevel;
            calculation.Name = name;
            
            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen concreet getal ingevoerd voor 'toetspeil'.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Kan de stijghoogte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutEntryPointL_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.EntryPointL = RoundedDouble.NaN;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen concreet getal ingevoerd voor 'Intredepunt'.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutExitPointL_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.ExitPointL = RoundedDouble.NaN;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen concreet getal ingevoerd voor 'Uittredepunt'.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutSurfaceLine_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.SurfaceLine = null;
            calculation.InputParameters.ExitPointL = (RoundedDouble) 0.9;
            calculation.InputParameters.EntryPointL = (RoundedDouble) 0.1;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutStochasticSoilProfile_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile = null;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_StochasticSoilProfileBelowSurfaceLine_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            var topLayer = new PipingSoilLayer(10.56 - 1e-6)
            {
                IsAquifer = false,
                BelowPhreaticLevelMean = 15,
                BelowPhreaticLevelDeviation = 2,
                BelowPhreaticLevelShift = 0
            };
            var bottomLayer = new PipingSoilLayer(2.0)
            {
                IsAquifer = true,
                DiameterD70Deviation = 0,
                DiameterD70Mean = 1e-4,
                PermeabilityDeviation = 0.5,
                PermeabilityMean = 1
            };
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = new PipingSoilProfile(
                string.Empty, 0.0,
                new[]
                {
                    topLayer,
                    bottomLayer
                },
                SoilProfileType.SoilProfile1D, -1);

            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Profielschematisch ligt (deels) boven de ondergrondschematisatie.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
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

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                Assert.AreEqual("Validatie mislukt: Geen watervoerende laag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[3]);
                Assert.AreEqual("Validatie mislukt: Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[4]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutAquitardLayer_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var aquiferLayer = new PipingSoilLayer(2.0)
            {
                IsAquifer = true,
                DiameterD70Deviation = 0,
                DiameterD70Mean = 1e-4,
                PermeabilityDeviation = 0.5,
                PermeabilityMean = 1
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    aquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_WithoutCoverageLayer_LogsErrorAndReturnsFalse()
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
                DiameterD70Deviation = 0,
                DiameterD70Mean = 1e-4,
                PermeabilityDeviation = 0.5,
                PermeabilityMean = 1
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    coverageLayerAboveSurfaceLine,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[1]);
                Assert.AreEqual("Validatie mislukt: Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_MultipleCoverageLayer_LogsWarningAndReturnsTrue()
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var belowPhreaticLevelDeviation = 0.5;
            var belowPhreaticLevelShift = 1;
            var belowPhreaticLevelMeanBase = 15.0;

            var topCoverageLayer = new PipingSoilLayer(10.56)
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
                PermeabilityDeviation = 0.5,
                PermeabilityMean = 1,
                DiameterD70Deviation = 0,
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

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Meerdere aaneengesloten deklagen gevonden. De grondeigenschappen worden bepaald door het nemen van een gewogen gemiddelde, mits de standaardafwijkingen en verschuivingen voor alle lagen gelijk zijn.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
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
                PermeabilityDeviation = 0.5,
                PermeabilityMean = 1
            };
            if (meanSet)
            {
                incompletePipingSoilLayer.DiameterD70Mean = 0.1 + random.NextDouble();
            }
            if (deviationSet)
            {
                incompletePipingSoilLayer.DiameterD70Deviation = random.NextDouble();
            }

            var completeLayer = new PipingSoilLayer(10.56)
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

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan de definitie voor het 70%-fraktiel van de korreldiameter van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
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
                PermeabilityDeviation = 0.5,
                PermeabilityMean = 1,
                DiameterD70Mean = diameter70Value,
                DiameterD70Deviation = 0
            };
            var validLayer = new PipingSoilLayer(10.56)
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

            PipingCalculation pipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            pipingCalculation.Name = name;
            pipingCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(pipingCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual(string.Format("Rekenwaarde voor d70 ({0} m) ligt buiten het geldigheidsbereik van dit model. Geldige waarden liggen tussen 0.000063 m en 0.0005 m.",
                                              new RoundedDouble(6, diameter70Value)), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
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
                DiameterD70Deviation = 0,
                DiameterD70Mean = 1e-4
            };
            if (meanSet)
            {
                incompletePipingSoilLayer.PermeabilityMean = 0.1 + random.NextDouble();
            }
            if (deviationSet)
            {
                incompletePipingSoilLayer.PermeabilityDeviation = random.NextDouble();
            }

            var completeLayer = new PipingSoilLayer(10.56)
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

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan de definitie voor de doorlatendheid van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
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
            var incompletePipingSoilLayer = new PipingSoilLayer(10.56)
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
                PermeabilityDeviation = 0.5,
                PermeabilityMean = 1,
                DiameterD70Deviation = 0,
                DiameterD70Mean = 1e-4
            };

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    incompletePipingSoilLayer,
                                                    completeLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual("Validatie mislukt: Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_SaturatedCoverageLayerVolumicWeightLessThanWaterVolumicWeight_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var coverageLayerInvalidSaturatedVolumicWeight = new PipingSoilLayer(10.56)
            {
                IsAquifer = false,
                BelowPhreaticLevelMean = 9.81,
                BelowPhreaticLevelDeviation = 2,
                BelowPhreaticLevelShift = 0
            };
            var validLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                PermeabilityDeviation = 0.5,
                PermeabilityMean = 1,
                DiameterD70Mean = 0.0002,
                DiameterD70Deviation = 0
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    coverageLayerInvalidSaturatedVolumicWeight,
                                                    validLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation pipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            pipingCalculation.Name = name;
            pipingCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(pipingCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual(
                    "Validatie mislukt: Het verzadigd volumetrisch gewicht van de deklaag moet groter zijn dan het volumetrisch gewicht van water.",
                    msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_SaturatedCoverageLayerLessThanWaterLayerAndMissingSaturatedParameter_LogsErrorOnlyForIncompleteDefinition()
        {
            // Setup
            const string name = "<very nice name>";

            var topCoverageLayer = new PipingSoilLayer(10.56)
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
                PermeabilityDeviation = 0.3,
                PermeabilityMean = 0.6,
                DiameterD70Mean = 0.0002,
                DiameterD70Deviation = 0
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    topCoverageLayer,
                                                    middleCoverageLayerMissingParameter,
                                                    bottomAquiferLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation pipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            pipingCalculation.Name = name;
            pipingCalculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;

            // Call
            Action call = () => PipingCalculationService.Validate(pipingCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                Assert.AreEqual(
                    "Meerdere aaneengesloten deklagen gevonden. De grondeigenschappen worden bepaald door het nemen van een gewogen gemiddelde, mits de standaardafwijkingen en verschuivingen voor alle lagen gelijk zijn.", 
                    msgs[1]);
                Assert.AreEqual(
                    "Validatie mislukt: Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.",
                    msgs[2]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
        }

        [Test]
        public void Validate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingCalculationService.Validate(validPipingCalculation);

                // Assert
                AssertSubCalculatorInputs(input);
            }
        }

        [Test]
        public void Calculate_ValidPipingCalculation_LogStartAndEndOfValidatingInputsAndCalculation()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation validPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            validPipingCalculation.Name = name;

            Action call = () =>
            {
                // Precondition
                Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));

                // Call
                PipingCalculationService.Calculate(validPipingCalculation);
            };

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);

                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", name), msgs[2]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", name), msgs.Last());
            });
        }

        [Test]
        public void Calculate_ValidPipingCalculationNoOutput_ShouldSetOutput()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            // Precondition
            Assert.IsNull(validPipingCalculation.Output);
            Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));

            // Call
            PipingCalculationService.Calculate(validPipingCalculation);

            // Assert
            Assert.IsNotNull(validPipingCalculation.Output);
        }

        [Test]
        public void Calculate_ValidPipingCalculationWithOutput_ShouldChangeOutput()
        {
            // Setup
            var output = new TestPipingOutput();

            PipingCalculation validPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            validPipingCalculation.Output = output;

            // Precondition
            Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));

            // Call
            PipingCalculationService.Calculate(validPipingCalculation);

            // Assert
            Assert.AreNotSame(output, validPipingCalculation.Output);
        }

        [Test]
        public void Calculate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingCalculationService.Calculate(validPipingCalculation);

                // Assert
                AssertSubCalculatorInputs(input);
            }
        }

        private static void AssertSubCalculatorInputs(PipingInput input)
        {
            var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
            var heaveCalculator = testFactory.LastCreatedHeaveCalculator;
            var upliftCalculator = testFactory.LastCreatedUpliftCalculator;
            var sellmeijerCalculator = testFactory.LastCreatedSellmeijerCalculator;

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
            RoundedDouble effectiveStress = PipingSemiProbabilisticDesignValueFactory.GetEffectiveThicknessCoverageLayer(input).GetDesignValue()*
                                            (PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(input).GetDesignValue() - input.WaterVolumetricWeight);
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