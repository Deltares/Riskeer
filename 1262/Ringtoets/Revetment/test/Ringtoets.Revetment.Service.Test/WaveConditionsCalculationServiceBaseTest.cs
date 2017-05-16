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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Service.Test
{
    [TestFixture]
    public class WaveConditionsCalculationServiceBaseTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Validate_InputNull_ThrowArgumentNullException()
        {
            // Setup
            string dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            // Call
            TestDelegate action = () => new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(null,
                                                                                                                 "test",
                                                                                                                 dbFilePath,
                                                                                                                 string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(action);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void Validate_DesignWaterLevelNameNull_ThrowArgumentNullException()
        {
            // Setup 
            const string name = "test";

            string dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            };

            // Call
            TestDelegate action = () => new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                                                 name,
                                                                                                                 dbFilePath,
                                                                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(action);
            Assert.AreEqual("designWaterLevelName", exception.ParamName);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryDatabase_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            const string name = "test";
            var isValid = false;

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(new WaveConditionsInput(), name, string.Empty, string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand '': bestandspad mag niet leeg of ongedefinieerd zijn.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabaseFileLocation_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            const string name = "test";
            var isValid = false;
            string dbFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(new WaveConditionsInput(), name, dbFilePath, string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual($"Validatie mislukt: Fout bij het lezen van bestand '{dbFilePath}': het bestand bestaat niet.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsValidationMessageAndReturnFalse()
        {
            // Setup 
            const string name = "test";
            var isValid = false;
            string dbFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(new WaveConditionsInput(), name, dbFilePath, string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            const string name = "test";
            var isValid = false;

            var input = new WaveConditionsInput();
            string dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input, name, dbFilePath, string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocationDesignWaterLevel_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            const string name = "test";
            var isValid = false;

            string dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            };

            const string designWaterLevelName = "<de arbitraire naam voor designwaterlevel>";

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                                                     name,
                                                                                                                     dbFilePath,
                                                                                                                     designWaterLevelName);

            // Assert
            string expectedMessage = $"Validatie mislukt: Kan {designWaterLevelName} niet afleiden op basis van de invoer";

            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith(expectedMessage, msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN, 10.0, 12.0)]
        [TestCase(1.0, double.NaN, 12.0)]
        public void Validate_NoWaterLevels_ReturnsFalseAndLogsValidationError(double lowerBoundaryRevetments,
                                                                              double upperBoundaryRevetments,
                                                                              double designWaterLevel)
        {
            // Setup
            const string name = "test";
            var isValid = false;

            string dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(designWaterLevel),
                Orientation = (RoundedDouble) 0,
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetments,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetments,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                                                     name,
                                                                                                                     dbFilePath,
                                                                                                                     "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_ForeshoreProfileUseBreakWaterAndHasInvalidBreakWaterHeight_ReturnsFalseAndLogsValidationMessages(double breakWaterHeight)
        {
            // Setup
            const string name = "test";
            var isValid = false;

            string dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            WaveConditionsInput input = GetDefaultValidationInput();
            input.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight));
            input.UseBreakWater = true;

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                                                     name,
                                                                                                                     dbFilePath,
                                                                                                                     "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_ForeshoreProfileDoesNotUseBreakWaterAndHasInvalidBreakwaterHeight_ReturnsTrueAndLogsValidationStartAndEnd(double breakWaterHeight)
        {
            // Setup
            const string name = "test";
            var isValid = false;

            string dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            WaveConditionsInput input = GetDefaultValidationInput();
            input.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Wall, breakWaterHeight));
            input.UseBreakWater = false;

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                                                     name,
                                                                                                                     dbFilePath,
                                                                                                                     "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Validate_ValidInputValidateForeshoreProfile_ReturnsTrueAndLogsValidationStartAndEnd(CalculationType calculationType)
        {
            // Setup 
            const string name = "test";
            var isValid = false;

            string dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            WaveConditionsInput input = GetDefaultValidationInput();

            switch (calculationType)
            {
                case CalculationType.NoForeshore:
                    input.ForeshoreProfile = null;
                    input.UseBreakWater = false;
                    input.UseForeshore = false;
                    input.Orientation = (RoundedDouble) 0;
                    break;
                case CalculationType.ForeshoreWithoutBreakWater:
                    input.ForeshoreProfile = new TestForeshoreProfile();
                    input.UseBreakWater = false;
                    break;
                case CalculationType.ForeshoreWithValidBreakWater:
                    break;
            }

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                                                     name,
                                                                                                                     dbFilePath,
                                                                                                                     "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        public void Validate_StructureNormalOrientationInvalid_ReturnsFalse()
        {
            // Setup
            const string name = "test";
            var isValid = false;

            string dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            WaveConditionsInput input = GetDefaultValidationInput();
            input.Orientation = RoundedDouble.NaN;

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                                                     name,
                                                                                                                     dbFilePath,
                                                                                                                     "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith($"Validatie van '{name}' gestart om: ", msgs[0]);
                Assert.AreEqual("Validatie mislukt: De waarde voor 'oriëntatie' moet een concreet getal zijn.", msgs[1]);
                StringAssert.StartsWith($"Validatie van '{name}' beëindigd om: ", msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Calculate_InputNull_ThrowArgumentNullException()
        {
            // Setup
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
            const string calculationName = "test";

            // Call
            TestDelegate test = () => new WaveConditionsCalculationService().PublicCalculate(a, b, c, norm, null, hcldFilePath, calculationName);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        [Combinatorial]
        public void Calculate_Always_StartsCalculationWithRightParameters(
            [Values(true, false)] bool useForeshore,
            [Values(true, false)] bool useBreakWater)
        {
            // Setup
            var waterLevel = (RoundedDouble) 4.20;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(waterLevel),
                ForeshoreProfile = new TestForeshoreProfile(true),
                UpperBoundaryRevetment = (RoundedDouble) 4,
                LowerBoundaryRevetment = (RoundedDouble) 3,
                StepSize = WaveConditionsInputStepSize.Two,
                UseBreakWater = useBreakWater,
                UseForeshore = useForeshore,
                Orientation = (RoundedDouble) 0
            };

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
            const string calculationName = "test";

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator testCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;

                // Call
                new WaveConditionsCalculationService().PublicCalculate(a, b, c, norm, input, hcldFilePath, calculationName);

                // Assert
                Assert.AreEqual(Path.GetDirectoryName(hcldFilePath), testCalculator.HydraulicBoundaryDatabaseDirectory);
                for (var i = 0; i < input.WaterLevels.Count(); i++)
                {
                    WaveConditionsCosineCalculationInput expectedInput = CreateInput(input.WaterLevels.ElementAt(i), a, b, c, norm, input, useForeshore, useBreakWater);
                    HydraRingDataEqualityHelper.AreEqual(expectedInput, testCalculator.ReceivedInputs[i]);
                }
            }
        }

        [Test]
        public void Calculate_CalculationOutputNull_LogError()
        {
            // Setup
            var waterLevel = new RoundedDouble(2, 4.00);
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(4.2),
                ForeshoreProfile = new TestForeshoreProfile(),
                UpperBoundaryRevetment = waterLevel,
                LowerBoundaryRevetment = (RoundedDouble) 3
            };

            const string calculationName = "test";

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
            var exceptionThrown = false;

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator testCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                testCalculator.EndInFailure = true;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new WaveConditionsCalculationService().PublicCalculate(a, b, c, norm, input, hcldFilePath, calculationName);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    Assert.AreEqual($"Berekening '{calculationName}' voor waterstand '{waterLevel}' gestart.", msgs[0]);
                    Assert.AreEqual($"Berekening '{calculationName}' voor waterstand '{waterLevel}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    Assert.AreEqual($"Berekening '{calculationName}' voor waterstand '{waterLevel}' beëindigd.", msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            var waterLevel = new RoundedDouble(2, 4.00);
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double norm = 0.2;
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(4.2),
                ForeshoreProfile = new TestForeshoreProfile(),
                UpperBoundaryRevetment = waterLevel,
                LowerBoundaryRevetment = (RoundedDouble) 3
            };

            const string name = "test";

            string hcldFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator testCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                var service = new WaveConditionsCalculationService();
                testCalculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.PublicCalculate(a, b, c, norm, input, hcldFilePath, name);

                // Assert
                Assert.IsTrue(testCalculator.IsCanceled);
            }
        }

        private static WaveConditionsInput GetDefaultValidationInput()
        {
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(12),
                ForeshoreProfile = new TestForeshoreProfile(true),
                UseBreakWater = true,
                UseForeshore = true,
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0,
                Orientation = (RoundedDouble) 0
            };
            return input;
        }

        private static WaveConditionsCosineCalculationInput CreateInput(double waterLevel, double a, double b, double c, double norm, WaveConditionsInput input, bool useForeshore, bool useBreakWater)
        {
            return new WaveConditionsCosineCalculationInput(1,
                                                            input.Orientation,
                                                            input.HydraulicBoundaryLocation.Id,
                                                            norm,
                                                            useForeshore ?
                                                                input.ForeshoreGeometry.Select(coordinate => new HydraRingForelandPoint(coordinate.X, coordinate.Y))
                                                                : new HydraRingForelandPoint[0],
                                                            useBreakWater
                                                                ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height)
                                                                : null,
                                                            waterLevel,
                                                            a,
                                                            b,
                                                            c);
        }
    }

    public class WaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        public bool PublicValidateWaveConditionsInput(WaveConditionsInput waveConditionsInput, string calculationName, string dbFilePath, string valueName)
        {
            return ValidateWaveConditionsInput(waveConditionsInput, calculationName, dbFilePath, valueName);
        }

        public void PublicCalculate(RoundedDouble a, RoundedDouble b, RoundedDouble c, double norm, WaveConditionsInput input, string dbFilePath, string name)
        {
            CalculateWaveConditions(name, input, a, b, c, norm, dbFilePath);
        }
    }
}