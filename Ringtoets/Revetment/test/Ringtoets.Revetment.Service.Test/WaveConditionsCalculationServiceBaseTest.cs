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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Service.Test
{
    [TestFixture]
    public class WaveConditionsCalculationServiceBaseTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Validate_DesignWaterLevelNameNull_ThrowArgumentNullException()
        {
            // Setup 
            string name = "test";

            var dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var input = new WaveConditionsInput()
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
            string name = "test";
            bool isValid = false;

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(new WaveConditionsInput(), name, string.Empty, string.Empty);

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand '': Bestandspad mag niet leeg of ongedefinieerd zijn.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabaseFileLocation_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            string name = "test";
            bool isValid = false;
            var dbFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            // Call
            Action action = () =>
            {
                isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(null, name, dbFilePath, string.Empty);
            };

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                Assert.AreEqual(string.Format("Validatie mislukt: Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.", dbFilePath), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            string name = "test";
            bool isValid = false;

            var input = new WaveConditionsInput();
            var dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            // Call
            Action action = () =>
            {
                isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input, name, dbFilePath, string.Empty);
            };

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocationDesignWaterLevel_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            string name = "test";
            bool isValid = false;

            var dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            };

            string designWaterLevelName = "<de arbitraire naam voor designwaterlevel>";

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                               name,
                                                                                               dbFilePath,
                                                                                               designWaterLevelName);

            // Assert
            string expectedMessage = string.Format("Validatie mislukt: Kan {0} niet afleiden op basis van de invoer",
                                                   designWaterLevelName);

            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(expectedMessage, msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
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
            string name = "test";
            bool isValid = false;

            var dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var input = new WaveConditionsInput()
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) designWaterLevel
                },
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
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
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
            string name = "test";
            bool isValid = false;

            var dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            WaveConditionsInput input = GetDefaultValidationInput();
            input.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight));
            input.UseBreakWater = true;

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                               name,
                                                                                               dbFilePath,
                                                                                               "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen geldige damhoogte ingevoerd.", msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
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
            string name = "test";
            bool isValid = false;

            var dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            WaveConditionsInput input = GetDefaultValidationInput();
            input.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Wall, breakWaterHeight));
            input.UseBreakWater = false;

            // Call
            Action action = () => isValid = new WaveConditionsCalculationService().PublicValidateWaveConditionsInput(input,
                                                                                               name,
                                                                                               dbFilePath,
                                                                                               "DesignWaterLevelName");

            // Assert
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
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
            string name = "test";
            bool isValid = false;

            var dbFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var input = GetDefaultValidationInput();

            switch (calculationType)
            {
                case CalculationType.NoForeshore:
                    input.ForeshoreProfile = null;
                    input.UseBreakWater = false;
                    input.UseForeshore = false;
                    break;
                case CalculationType.ForeshoreWithoutBreakWater:
                    input.ForeshoreProfile = CreateForeshoreProfile(null);
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
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        [Combinatorial]
        public void Calculate_Always_StartsCalculationWithRightParameters(
            [Values(true, false)] bool useForeshore,
            [Values(true, false)] bool useBreakWater)
        {
            // Setup
            RoundedDouble waterLevel = (RoundedDouble) 4.20;
            RoundedDouble a = (RoundedDouble)1.0;
            RoundedDouble b = (RoundedDouble)0.8;
            RoundedDouble c = (RoundedDouble)0.4;
            int norm = 5;
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = waterLevel
                },
                ForeshoreProfile = CreateForeshoreProfile(),
                UpperBoundaryRevetment = (RoundedDouble) 4,
                LowerBoundaryRevetment = (RoundedDouble) 3,
                StepSize = WaveConditionsInputStepSize.Two,
                UseBreakWater = useBreakWater,
                UseForeshore = useForeshore
            };

            string hcldFilePath = "C:\\temp\\someFile";
            string ringId = "11-1";
            string name = "test";

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;

                // Call
                new WaveConditionsCalculationService().PublicCalculate(a, b, c, norm, input, hcldFilePath, ringId, name);

                // Assert
                Assert.AreEqual(Path.GetDirectoryName(hcldFilePath), testCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(ringId, testCalculator.RingId);
                for (int i = 0; i < input.WaterLevels.Count(); i++)
                {
                    var expectedInput = CreateInput(input.WaterLevels.ElementAt(i), a, b, c, norm, input, useForeshore, useBreakWater);
                    HydraRingDataEqualityHelper.AreEqual(expectedInput, testCalculator.ReceivedInputs[i]);
                }
            }
        }

        [Test]
        public void Calculate_CalculationOutputNull_LogError()
        {
            // Setup
            RoundedDouble waterLevel = new RoundedDouble(2, 4.00);
            RoundedDouble a = (RoundedDouble)1.0;
            RoundedDouble b = (RoundedDouble)0.8;
            RoundedDouble c = (RoundedDouble)0.4;
            int norm = 5;
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 4.2
                },
                ForeshoreProfile = CreateForeshoreProfile(),
                UpperBoundaryRevetment = waterLevel,
                LowerBoundaryRevetment = (RoundedDouble) 3
            };

            string hlcdDirectory = "C:/temp";
            string ringId = "11-1";
            string name = "test";
            bool exceptionThrown = false;

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testCalculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                testCalculator.EndInFailure = true;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new WaveConditionsCalculationService().PublicCalculate(a, b, c, norm, input, hlcdDirectory, ringId, name);
                    }
                    catch (HydraRingFileParserException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(2, msgs.Length);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", name, waterLevel), msgs[0]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' is niet gelukt.", name, waterLevel), msgs[1]);
                });
                Assert.IsTrue(exceptionThrown);
            }
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            RoundedDouble waterLevel = new RoundedDouble(2, 4.00);
            RoundedDouble a = (RoundedDouble)1.0;
            RoundedDouble b = (RoundedDouble)0.8;
            RoundedDouble c = (RoundedDouble)0.4;
            int norm = 5;
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble)4.2
                },
                ForeshoreProfile = CreateForeshoreProfile(),
                UpperBoundaryRevetment = waterLevel,
                LowerBoundaryRevetment = (RoundedDouble)3
            };

            string hlcdDirectory = "C:/temp";
            string ringId = "11-1";
            string name = "test";

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testCalculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                var service = new WaveConditionsCalculationService();

                // Call
                service.PublicCalculate(a, b, c, norm, input, hlcdDirectory, ringId, name);
                service.Cancel();

                // Assert
                Assert.IsTrue(testCalculator.IsCanceled);
            }
        }

        public enum CalculationType
        {
            NoForeshore,
            ForeshoreWithValidBreakWater,
            ForeshoreWithoutBreakWater
        }

        private static WaveConditionsInput GetDefaultValidationInput()
        {
            var input = new WaveConditionsInput()
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 12.0
                },
                ForeshoreProfile = CreateForeshoreProfile(),
                UseBreakWater = true,
                UseForeshore = true,
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };
            return input;
        }

        private static ForeshoreProfile CreateForeshoreProfile()
        {
            return CreateForeshoreProfile(new BreakWater(BreakWaterType.Wall, 5.5));
        }

        private static ForeshoreProfile CreateForeshoreProfile(BreakWater breakWater)
        {
            return new ForeshoreProfile(new Point2D(0, 0),
                                        new[]
                                        {
                                            new Point2D(3.3, 4.4),
                                            new Point2D(5.5, 6.6)
                                        },
                                        breakWater,
                                        new ForeshoreProfile.ConstructionProperties());
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

        public void PublicCalculate(RoundedDouble a, RoundedDouble b, RoundedDouble c, int norm, WaveConditionsInput input, string dbFilePath, string ringId, string name)
        {
            CalculateWaveConditions(name, input, a, b, c, norm, ringId, dbFilePath);
        }
    }
}