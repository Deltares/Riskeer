﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

            PipingCalculation pipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            pipingCalculation.Name = name;

            // Call
            Action call = () => PipingCalculationService.Validate(pipingCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
        }

        [Test]
        public void Validate_InValidPipingCalculationWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            var output = new TestPipingOutput();
            PipingCalculation invalidPipingCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidPipingCalculation.Output = output;

            // Call
            bool isValid = PipingCalculationService.Validate(invalidPipingCalculation);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreSame(output, invalidPipingCalculation.Output);
        }

        [Test]
        public void Validate_InValidCalculationInput_LogsErrorAndReturnsFalse()
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
                var msgs = messages.ToArray();
                Assert.AreEqual(7, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[2]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[3]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen waarde voor het intredepunt opgegeven.", msgs[4]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen waarde voor het uittredepunt opgegeven.", msgs[5]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutHydraulicBoundaryLocation_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.HydraulicBoundaryLocation = null;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithHydraulicBoundaryLocationNotCalculated_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevel = double.NaN;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Kan de stijghoogte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutEntryPointL_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.EntryPointL = (RoundedDouble) double.NaN;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen waarde voor het intredepunt opgegeven.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutExitPointL_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.ExitPointL = (RoundedDouble) double.NaN;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen waarde voor het uittredepunt opgegeven.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutSurfaceLine_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
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
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutStochasticSoilProfile_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.StochasticSoilProfile = null;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutAquiferLayer_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                    new[]
                                                {
                                                    new PipingSoilLayer(2.0)
                                                    {
                                                        IsAquifer = false
                                                    }
                                                },
                                    SoilProfileType.SoilProfile1D, -1);

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith("Validatie mislukt: Geen watervoerende laag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[3]);
                StringAssert.StartsWith("Validatie mislukt: Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[4]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutAquitardLayer_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    new PipingSoilLayer(2.0)
                                                    {
                                                        IsAquifer = true,
                                                        DiameterD70Deviation = random.NextDouble(),
                                                        DiameterD70Mean = 0.1 + random.NextDouble(),
                                                        PermeabilityDeviation = random.NextDouble(),
                                                        PermeabilityMean = 0.1 + random.NextDouble()
                                                    }
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[2]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutCoverageLayer_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    new PipingSoilLayer(13.0)
                                                    {
                                                        IsAquifer = false
                                                    },
                                                    new PipingSoilLayer(11.0)
                                                    {
                                                        IsAquifer = true,
                                                        DiameterD70Deviation = random.NextDouble(),
                                                        DiameterD70Mean = 0.1 + random.NextDouble(),
                                                        PermeabilityDeviation = random.NextDouble(),
                                                        PermeabilityMean = 0.1 + random.NextDouble()
                                                    }
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Kan de totale deklaagdikte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Geen deklaag gevonden voor de ondergrondschematisatie onder de profielschematisatie bij het uittredepunt.", msgs[2]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Validate_CalculationWithIncompleteDiameterD70Definition_LogsErrorAndReturnsFalse(bool meanSet, bool deviationSet)
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                PermeabilityDeviation = random.NextDouble(),
                PermeabilityMean = 0.1 + random.NextDouble()
            };
            if (meanSet)
            {
                incompletePipingSoilLayer.DiameterD70Mean = 0.1 + random.NextDouble();
            }
            if (deviationSet)
            {
                incompletePipingSoilLayer.DiameterD70Deviation = random.NextDouble();
            }

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    new PipingSoilLayer(10.5)
                                                    {
                                                        IsAquifer = false,
                                                        BelowPhreaticLevelDeviation = GetRandomDoubleFromRange(random, 1e-6, 999.999),
                                                        BelowPhreaticLevelMean = GetRandomDoubleFromRange(random, 10.0, 999.999),
                                                        BelowPhreaticLevelShift = GetRandomDoubleFromRange(random, 1e-6, 10.0)
                                                    },
                                                    incompletePipingSoilLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Kan de definitie voor het 70%-fraktiel van de korreldiameter van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void Validate_CalculationWithIncompletePermeabilityDefinition_LogsErrorAndReturnsFalse(bool meanSet, bool deviationSet)
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(5.0)
            {
                IsAquifer = true,
                DiameterD70Deviation = random.NextDouble(),
                DiameterD70Mean = 0.1 + random.NextDouble()
            };
            if (meanSet)
            {
                incompletePipingSoilLayer.PermeabilityMean = 0.1 + random.NextDouble();
            }
            if (deviationSet)
            {
                incompletePipingSoilLayer.PermeabilityDeviation = random.NextDouble();
            }

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    new PipingSoilLayer(10.5)
                                                    {
                                                        IsAquifer = false,
                                                        BelowPhreaticLevelDeviation = GetRandomDoubleFromRange(random, 1e-6, 999.999),
                                                        BelowPhreaticLevelMean = GetRandomDoubleFromRange(random, 10.0, 999.999),
                                                        BelowPhreaticLevelShift = GetRandomDoubleFromRange(random, 1e-6, 10.0)
                                                    },
                                                    incompletePipingSoilLayer
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Kan de definitie voor de doorlatendheid van de watervoerende laag niet (volledig) afleiden.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        public void Validate_CalculationWithIncompletSaturatedVolumicWeightDefinition_LogsErrorAndReturnsFalse(bool meanSet, bool deviationSet, bool shiftSet)
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var incompletePipingSoilLayer = new PipingSoilLayer(10.5)
            {
                IsAquifer = false,
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

            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    incompletePipingSoilLayer,
                                                    new PipingSoilLayer(5.0)
                                                    {
                                                        IsAquifer = true,
                                                        PermeabilityDeviation = random.NextDouble(),
                                                        PermeabilityMean = 0.1 + random.NextDouble(),
                                                        DiameterD70Deviation = random.NextDouble(),
                                                        DiameterD70Mean = 0.1 + random.NextDouble()
                                                    }
                                                },
                                                SoilProfileType.SoilProfile1D, -1);
            
            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Kan de definitie voor het verzadigd gewicht van de deklaag niet (volledig) afleiden.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithMultipleCoverageLayer_LogsWarningAndReturnsTrue()
        {
            // Setup
            const string name = "<very nice name>";

            var random = new Random(21);
            var belowPhreaticLevelDeviation = random.NextDouble();
            var belowPhreaticLevelShift = random.NextDouble();
            var profile = new PipingSoilProfile(string.Empty, 0.0,
                                                new[]
                                                {
                                                    new PipingSoilLayer(10.5)
                                                    {
                                                        IsAquifer = false,
                                                        BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                                                        BelowPhreaticLevelShift = belowPhreaticLevelShift,
                                                        BelowPhreaticLevelMean = 0.1 + belowPhreaticLevelShift + random.NextDouble()
                                                    },
                                                    new PipingSoilLayer(8.5)
                                                    {
                                                        IsAquifer = false,
                                                        BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                                                        BelowPhreaticLevelShift = belowPhreaticLevelShift,
                                                        BelowPhreaticLevelMean = 0.1 + belowPhreaticLevelShift + random.NextDouble()
                                                    },
                                                    new PipingSoilLayer(5.0)
                                                    {
                                                        IsAquifer = true,
                                                        PermeabilityDeviation = random.NextDouble(),
                                                        PermeabilityMean = 0.1 + random.NextDouble(),
                                                        DiameterD70Deviation = random.NextDouble(),
                                                        DiameterD70Mean = 0.1 + random.NextDouble()
                                                    }
                                                },
                                                SoilProfileType.SoilProfile1D, -1);

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.StochasticSoilProfile.SoilProfile = profile;
            calculation.Name = name;

            bool isValid = false;

            // Call
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Meerdere aaneengesloten deklagen gevonden. De grondeigenschappen worden bepaald door het nemen van een gewogen gemiddelde, mits de standaard deviaties en verschuivingen voor alle lagen gelijk zijn.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsTrue(isValid);
        }

        [Test]
        public void Calculate_ValidPipingCalculation_LogStartAndEndOfValidatingInputsAndCalculation()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
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
                var msgs = messages.ToArray();
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);

                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", name), msgs[2]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", name), msgs.Last());
            });
        }

        [Test]
        public void Calculate_ValidPipingCalculationNoOutput_ShouldSetOutput()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();

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

            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Output = output;

            // Precondition
            Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));

            // Call
            PipingCalculationService.Calculate(validPipingCalculation);

            // Assert
            Assert.AreNotSame(output, validPipingCalculation.Output);
        }

        [Test]
        public void Validate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
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
        public void Calculate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingCalculationService.Calculate(validPipingCalculation);

                // Assert
                AssertSubCalculatorInputs(input);
            }
        }

        private void AssertSubCalculatorInputs(PipingInput input)
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
            RoundedDouble effectiveStress = PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue()*
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

        private double GetRandomDoubleFromRange(Random random, double lowerLimit, double upperLimit)
        {
            double difference = upperLimit - lowerLimit;
            return lowerLimit + random.NextDouble()*difference;
        }
    }
}