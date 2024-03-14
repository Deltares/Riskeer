// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;

namespace Riskeer.Revetment.Service.Test
{
    [TestFixture]
    public class WaveConditionsCalculationServiceBaseTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
        private static readonly string validHrdFileVersion = "IJssel lake2016-07-04 16:187";

        [Test]
        public void Validate_InputNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsCalculationServiceBase.Validate(null, GetValidAssessmentLevel(),
                                                                         GetValidHydraulicBoundaryData(new TestHydraulicBoundaryLocation()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void Validate_HydraulicBoundaryDataNull_ThrowArgumentNullException()
        {
            // Setup 
            var input = new WaveConditionsInput();

            // Call
            void Call() => WaveConditionsCalculationServiceBase.Validate(input, GetValidAssessmentLevel(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_ReturnsFalseAndLogsValidationError()
        {
            // Setup
            var isValid = false;

            var input = new WaveConditionsInput();

            // Call
            void Call() => isValid = WaveConditionsCalculationServiceBase.Validate(input, GetValidAssessmentLevel(),
                                                                                   new HydraulicBoundaryData());

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische belastingenlocatie geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_ReturnsFalseAndLogsValidationError()
        {
            // Setup 
            var isValid = false;
            string invalidFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "hlcd.sqlite")
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = invalidFilePath,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var waveConditionsInput = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation
            };

            // Call
            void Call() => isValid = WaveConditionsCalculationServiceBase.Validate(waveConditionsInput,
                                                                                   GetValidAssessmentLevel(),
                                                                                   hydraulicBoundaryData);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. " +
                                $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabaseWithoutSettings_LogsValidationMessageAndReturnFalse()
        {
            // Setup 
            var isValid = false;
            string hrdFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "hlcd.sqlite")
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = hrdFilePath,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var waveConditionsInput = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation
            };

            // Call
            void Call() => isValid = WaveConditionsCalculationServiceBase.Validate(waveConditionsInput,
                                                                                   GetValidAssessmentLevel(),
                                                                                   hydraulicBoundaryData);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. " +
                                        "Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_AssessmentLevelNaN_ReturnsFalseAndLogsValidationError()
        {
            // Setup
            var isValid = false;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation
            };

            // Call
            void Call() => isValid = WaveConditionsCalculationServiceBase.Validate(input, RoundedDouble.NaN,
                                                                                   GetValidHydraulicBoundaryData(hydraulicBoundaryLocation));

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Kan waterstand bij doelkans niet afleiden op basis van de invoer", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });

            Assert.IsFalse(isValid);
        }

        [Test]
        [TestCase(double.NaN, 10.0, 12.0)]
        [TestCase(1.0, double.NaN, 12.0)]
        public void Validate_NoWaterLevels_ReturnsFalseAndLogsValidationError(double lowerBoundaryRevetments,
                                                                              double upperBoundaryRevetments,
                                                                              double assessmentLevel)
        {
            // Setup
            var isValid = false;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                Orientation = (RoundedDouble) 0,
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetments,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetments,
                StepSize = (RoundedDouble) 1,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            void Call() => isValid = WaveConditionsCalculationServiceBase.Validate(input, (RoundedDouble) assessmentLevel,
                                                                                   GetValidHydraulicBoundaryData(hydraulicBoundaryLocation));

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
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
            var isValid = false;

            WaveConditionsInput input = GetDefaultValidationInput();
            input.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight));
            input.UseBreakWater = true;

            // Call
            void Call() => isValid = WaveConditionsCalculationServiceBase.Validate(input, GetValidAssessmentLevel(),
                                                                                   GetValidHydraulicBoundaryData(input.HydraulicBoundaryLocation));

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
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
            var isValid = false;

            WaveConditionsInput input = GetDefaultValidationInput();
            input.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Wall, breakWaterHeight));
            input.UseBreakWater = false;

            // Call
            void Call() => isValid = WaveConditionsCalculationServiceBase.Validate(input, GetValidAssessmentLevel(),
                                                                                   GetValidHydraulicBoundaryData(input.HydraulicBoundaryLocation));

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(ForeshoreCalculationType.NoForeshore)]
        [TestCase(ForeshoreCalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(ForeshoreCalculationType.ForeshoreWithValidBreakWater)]
        public void Validate_ValidInputValidateForeshoreProfile_ReturnsTrueAndLogsValidationStartAndEnd(ForeshoreCalculationType calculationType)
        {
            // Setup 
            var isValid = false;

            WaveConditionsInput input = GetDefaultValidationInput();

            switch (calculationType)
            {
                case ForeshoreCalculationType.NoForeshore:
                    input.ForeshoreProfile = null;
                    input.UseBreakWater = false;
                    input.UseForeshore = false;
                    input.Orientation = (RoundedDouble) 0;
                    break;
                case ForeshoreCalculationType.ForeshoreWithoutBreakWater:
                    input.ForeshoreProfile = new TestForeshoreProfile();
                    input.UseBreakWater = false;
                    break;
                case ForeshoreCalculationType.ForeshoreWithValidBreakWater:
                    break;
            }

            // Call
            void Call() => isValid = WaveConditionsCalculationServiceBase.Validate(input, GetValidAssessmentLevel(),
                                                                                   GetValidHydraulicBoundaryData(input.HydraulicBoundaryLocation));

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });

            Assert.IsTrue(isValid);
        }

        [Test]
        public void Validate_StructureNormalOrientationInvalid_ReturnsFalse()
        {
            // Setup
            var isValid = false;

            WaveConditionsInput input = GetDefaultValidationInput();
            input.Orientation = RoundedDouble.NaN;

            // Call
            void Call() => isValid = WaveConditionsCalculationServiceBase.Validate(input, GetValidAssessmentLevel(),
                                                                                   GetValidHydraulicBoundaryData(input.HydraulicBoundaryLocation));

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De waarde voor 'Oriëntatie' moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
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
            const double targetProbability = 0.2;

            // Call
            void Call() => new TestWaveConditionsCalculationService().PublicCalculate(a, b, c, targetProbability, null,
                                                                                      GetValidAssessmentLevel(),
                                                                                      GetValidHydraulicBoundaryData(new TestHydraulicBoundaryLocation()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void Calculate_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double targetProbability = 0.2;

            // Call
            void Call() => new TestWaveConditionsCalculationService().PublicCalculate(a, b, c, targetProbability, GetDefaultValidationInput(),
                                                                                      GetValidAssessmentLevel(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_WithoutBreakWater_StartsCalculationWithRightParameters(bool useForeshore)
        {
            // Setup
            var waterLevel = (RoundedDouble) 4.20;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double targetProbability = 0.2;
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(true),
                UpperBoundaryRevetment = (RoundedDouble) 4,
                LowerBoundaryRevetment = (RoundedDouble) 3,
                StepSize = (RoundedDouble) 2,
                UseBreakWater = false,
                UseForeshore = useForeshore,
                Orientation = (RoundedDouble) 0
            };

            var calculator = new TestWaveConditionsCosineCalculator();
            RoundedDouble[] waterLevels = input.GetWaterLevels(waterLevel).ToArray();
            int nrOfCalculators = waterLevels.Length;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new TestWaveConditionsCalculationService().PublicCalculate(a,
                                                                           b,
                                                                           c,
                                                                           targetProbability,
                                                                           input,
                                                                           waterLevel,
                                                                           GetValidHydraulicBoundaryData(hydraulicBoundaryLocation));

                // Assert
                for (var i = 0; i < nrOfCalculators; i++)
                {
                    WaveConditionsCosineCalculationInput expectedInput = CreateInput(waterLevels[i], a, b, c, targetProbability, input, useForeshore, false);
                    HydraRingDataEqualityHelper.AreEqual(expectedInput, calculator.ReceivedInputs.ElementAt(i));
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_WithBreakWater_StartsCalculationWithRightParameters(BreakWaterType breakWaterType)
        {
            // Setup
            var waterLevel = (RoundedDouble) 4.20;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double targetProbability = 0.2;
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(true),
                UpperBoundaryRevetment = (RoundedDouble) 4,
                LowerBoundaryRevetment = (RoundedDouble) 3,
                StepSize = (RoundedDouble) 2,
                UseBreakWater = true,
                UseForeshore = true,
                Orientation = (RoundedDouble) 0,
                BreakWater =
                {
                    Type = breakWaterType
                }
            };

            var calculator = new TestWaveConditionsCosineCalculator();
            RoundedDouble[] waterLevels = input.GetWaterLevels(waterLevel).ToArray();

            int nrOfCalculators = waterLevels.Length;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new TestWaveConditionsCalculationService().PublicCalculate(a,
                                                                           b,
                                                                           c,
                                                                           targetProbability,
                                                                           input,
                                                                           waterLevel,
                                                                           GetValidHydraulicBoundaryData(hydraulicBoundaryLocation));

                // Assert
                for (var i = 0; i < nrOfCalculators; i++)
                {
                    WaveConditionsCosineCalculationInput expectedInput = CreateInput(waterLevels[i], a, b, c, targetProbability, input, true, true);
                    HydraRingDataEqualityHelper.AreEqual(expectedInput, calculator.ReceivedInputs.ElementAt(i));
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryDataConfigurations))]
        public void Calculate_VariousHydraulicBoundaryDatabaseConfigurations_StartsCalculationWithRightParameters(
            HydraulicBoundaryData hydraulicBoundaryData)
        {
            // Setup
            HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryData.GetLocations().First();

            var waterLevel = (RoundedDouble) 4.20;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double targetProbability = 0.2;
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(true),
                UpperBoundaryRevetment = (RoundedDouble) 4,
                LowerBoundaryRevetment = (RoundedDouble) 3,
                StepSize = (RoundedDouble) 2,
                Orientation = (RoundedDouble) 0
            };

            var calculator = new TestWaveConditionsCosineCalculator();
            int nrOfCalculators = input.GetWaterLevels(waterLevel).Count();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData,
                                                                                                hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new TestWaveConditionsCalculationService().PublicCalculate(a,
                                                                           b,
                                                                           c,
                                                                           targetProbability,
                                                                           input,
                                                                           waterLevel,
                                                                           hydraulicBoundaryData);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_ThreeCalculationsFail_ThrowsHydraRingCalculationExceptionAndLogError)
        })]
        public void Calculate_ThreeCalculationsFail_ThrowsHydraRingCalculationExceptionAndLogError(bool endInFailure,
                                                                                                   string lastErrorFileContent,
                                                                                                   string detailedReport)
        {
            // Setup
            var waterLevel = (RoundedDouble) 4.20;
            var waterLevelUpperBoundaryRevetment = new RoundedDouble(2, 4.00);
            var waterLevelLowerBoundaryRevetment = new RoundedDouble(2, 3.00);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(),
                UpperBoundaryRevetment = waterLevelUpperBoundaryRevetment,
                LowerBoundaryRevetment = waterLevelLowerBoundaryRevetment
            };

            int nrOfCalculators = input.GetWaterLevels(waterLevel).Count();

            var calculatorThatFails = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculatorThatFails)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double targetProbability = 0.2;

            HydraRingCalculationException exception = null;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call()
                {
                    try
                    {
                        new TestWaveConditionsCalculationService().PublicCalculate(a, b, c, targetProbability, input, waterLevel,
                                                                                   GetValidHydraulicBoundaryData(hydraulicBoundaryLocation));
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exception = e;
                    }
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(13, msgs.Length);

                    var waterLevelMiddleRevetment = new RoundedDouble(2, 3.50);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is gestart.", msgs[0]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[1]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is beëindigd.", msgs[3]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is gestart.", msgs[4]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelMiddleRevetment}'. {detailedReport}", msgs[5]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[6]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is beëindigd.", msgs[7]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is gestart.", msgs[8]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelLowerBoundaryRevetment}'. {detailedReport}", msgs[9]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[10]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is beëindigd.", msgs[11]);

                    Assert.AreEqual("Berekening is mislukt voor alle waterstanden.", msgs[12]);
                });
                Assert.IsInstanceOf<HydraRingCalculationException>(exception);
                Assert.AreEqual("Berekening is mislukt voor alle waterstanden.", exception.Message);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_OneOutOfThreeCalculationsFails_ReturnsOutputsAndLogError)
        })]
        public void Calculate_OneOutOfThreeCalculationsFails_ReturnsOutputsAndLogError(bool endInFailure,
                                                                                       string lastErrorFileContent,
                                                                                       string detailedReport)
        {
            // Setup
            var calculatorThatFails = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculatorThatFails);
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Twice();
            mockRepository.ReplayAll();

            var waterLevelUpperBoundary = new RoundedDouble(2, 4.00);
            var waterLevelLowerBoundary = new RoundedDouble(2, 3.00);
            var waterLevel = (RoundedDouble) 4.2;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double targetProbability = 0.2;
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(),
                UpperBoundaryRevetment = waterLevelUpperBoundary,
                LowerBoundaryRevetment = waterLevelLowerBoundary
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestWaveConditionsCalculationService();

                // Call
                void Call() => service.PublicCalculate(a, b, c, targetProbability, input, waterLevel,
                                                       GetValidHydraulicBoundaryData(hydraulicBoundaryLocation));

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(10, msgs.Length);

                    var waterLevelMiddle = new RoundedDouble(2, 3.50);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundary}' is gestart.", msgs[0]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundary}'. {detailedReport}", msgs[1]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundary}' is beëindigd.", msgs[3]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddle}' is gestart.", msgs[4]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddle}' is beëindigd.", msgs[6]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundary}' is gestart.", msgs[7]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[8]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundary}' is beëindigd.", msgs[9]);
                });

                WaveConditionsOutput[] waveConditionsOutputs = service.Outputs.ToArray();
                Assert.AreEqual(3, waveConditionsOutputs.Length);

                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevelUpperBoundary,
                                                                  targetProbability,
                                                                  waveConditionsOutputs[0]);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            var waterLevel = (RoundedDouble) 4.2;
            var a = (RoundedDouble) 1.0;
            var b = (RoundedDouble) 0.8;
            var c = (RoundedDouble) 0.4;
            const double targetProbability = 0.2;
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                ForeshoreProfile = new TestForeshoreProfile(),
                UpperBoundaryRevetment = waterLevel,
                LowerBoundaryRevetment = (RoundedDouble) 3
            };
            var calculator = new TestWaveConditionsCosineCalculator();
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestWaveConditionsCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.PublicCalculate(a,
                                        b,
                                        c,
                                        targetProbability,
                                        input,
                                        waterLevel,
                                        GetValidHydraulicBoundaryData(hydraulicBoundaryLocation));

                // Assert
                Assert.IsTrue(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        private static WaveConditionsInput GetDefaultValidationInput()
        {
            var input = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                ForeshoreProfile = new TestForeshoreProfile(true),
                UseBreakWater = true,
                UseForeshore = true,
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = (RoundedDouble) 1,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0,
                Orientation = (RoundedDouble) 0
            };
            return input;
        }

        private static HydraulicBoundaryData GetValidHydraulicBoundaryData(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        Version = validHrdFileVersion,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };
        }

        private static WaveConditionsCosineCalculationInput CreateInput(double waterLevel, double a, double b, double c, double targetProbability,
                                                                        WaveConditionsInput input, bool useForeshore, bool useBreakWater)
        {
            return new WaveConditionsCosineCalculationInput(1,
                                                            input.Orientation,
                                                            input.HydraulicBoundaryLocation.Id,
                                                            targetProbability,
                                                            useForeshore
                                                                ? input.ForeshoreGeometry.Select(coordinate => new HydraRingForelandPoint(coordinate.X, coordinate.Y))
                                                                : new HydraRingForelandPoint[0],
                                                            useBreakWater
                                                                ? new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(input.BreakWater.Type), input.BreakWater.Height)
                                                                : null,
                                                            waterLevel,
                                                            a,
                                                            b,
                                                            c);
        }

        private static RoundedDouble GetValidAssessmentLevel()
        {
            return (RoundedDouble) 12;
        }

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryDataConfigurations()
        {
            var hydraulicBoundaryDataWithUsePreprocessorClosureTrue = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        UsePreprocessorClosure = true,
                        Locations =
                        {
                            new TestHydraulicBoundaryLocation()
                        }
                    }
                }
            };
            yield return new TestCaseData(hydraulicBoundaryDataWithUsePreprocessorClosureTrue).SetName("UsePreprocessorClosureTrue");

            var hydraulicBoundaryDataWithUsePreprocessorClosureFalse = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        Locations =
                        {
                            new TestHydraulicBoundaryLocation()
                        }
                    }
                }
            };
            yield return new TestCaseData(hydraulicBoundaryDataWithUsePreprocessorClosureFalse).SetName("UsePreprocessorClosureFalse");
        }

        private class TestWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
        {
            public IEnumerable<WaveConditionsOutput> Outputs;

            public void PublicCalculate(RoundedDouble a,
                                        RoundedDouble b,
                                        RoundedDouble c,
                                        double targetProbability,
                                        WaveConditionsInput input,
                                        RoundedDouble assessmentLevel,
                                        HydraulicBoundaryData hydraulicBoundaryData)
            {
                Outputs = CalculateWaveConditions(input,
                                                  assessmentLevel,
                                                  a,
                                                  b,
                                                  c,
                                                  targetProbability,
                                                  hydraulicBoundaryData);
            }
        }
    }
}