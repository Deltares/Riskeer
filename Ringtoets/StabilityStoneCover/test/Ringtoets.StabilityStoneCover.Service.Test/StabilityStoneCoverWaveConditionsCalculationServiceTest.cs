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
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Service.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationServiceTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private string validFilePath;

        [SetUp]
        public void SetUp()
        {
            validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        }

        [Test]
        public void Validate_NoHydraulicBoundaryDatabase_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var calculation = GetDefaultValidationInput();

            var testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = new StabilityStoneCoverWaveConditionsCalculationService().Validate(calculation, testFilePath);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    Assert.AreEqual(string.Format("Validatie mislukt: Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.", testFilePath), msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var calculation = GetDefaultValidationInput();
            var testFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = new StabilityStoneCoverWaveConditionsCalculationService().Validate(calculation, testFilePath);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    Assert.AreEqual(string.Format("Validatie mislukt: Fout bij het lezen van bestand '{0}': Kon geen locaties verkrijgen van de database.", testFilePath), msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var calculation = GetDefaultValidationInput();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = new StabilityStoneCoverWaveConditionsCalculationService().Validate(calculation, validFilePath);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_NoDesignWaterLevel_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var calculation = GetDefaultValidationInput();
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevel = (RoundedDouble) double.NaN;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = new StabilityStoneCoverWaveConditionsCalculationService().Validate(calculation, validFilePath);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        [TestCase(double.NaN, 10.0)]
        [TestCase(1.0, double.NaN)]
        public void Validate_NoWaterLevels_LogsValidationMessageAndReturnFalse(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var calculation = GetDefaultValidationInput();
            calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment;
            calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble)upperBoundaryRevetment;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = new StabilityStoneCoverWaveConditionsCalculationService().Validate(calculation, validFilePath);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NaN)]
        public void Validate_CalculationWithForeshoreAndUsesBreakWaterAndHasInvalidBreakWaterHeight_LogsValidationMessageAndReturnFalse(double breakWaterHeight)
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultValidationInput();
            calculation.InputParameters.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                 breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = new StabilityStoneCoverWaveConditionsCalculationService().Validate(calculation, validFilePath);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);

                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Er is geen geldige damhoogte ingevoerd.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NaN)]
        public void Calculate_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultValidationInput();
            calculation.InputParameters.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                 breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                            assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(14, msgs.Length);

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);

                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken gestart.", calculation.Name), msgs[1]);

                    int i = 2;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i++]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i++]);
                    }

                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken beëindigd.", calculation.Name), msgs[6]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen gestart.", calculation.Name), msgs[7]);

                    i = 8;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i++]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i++]);
                    }

                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen beëindigd.", calculation.Name), msgs[12]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[13]);
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Calculate_CalculationWithValidInputConditionsAndValidForeshore_LogCalculationStartAndEnd(CalculationType calculationType)
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultValidationInput();
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            switch (calculationType)
            {
                case CalculationType.NoForeshore:
                    calculation.InputParameters.ForeshoreProfile = null;
                    calculation.InputParameters.UseForeshore = false;
                    calculation.InputParameters.UseBreakWater = false;
                    break;
                case CalculationType.ForeshoreWithoutBreakWater:
                    calculation.InputParameters.ForeshoreProfile = CreateForeshoreProfile(null);
                    calculation.InputParameters.UseBreakWater = false;
                    break;
                case CalculationType.ForeshoreWithValidBreakWater:
                    break;
            }

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                            assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(14, msgs.Length);

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);

                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken gestart.", calculation.Name), msgs[1]);

                    int i = 2;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i++]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i++]);
                    }

                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken beëindigd.", calculation.Name), msgs[6]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen gestart.", calculation.Name), msgs[7]);

                    i = 8;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i++]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i++]);
                    }

                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen beëindigd.", calculation.Name), msgs[12]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[13]);
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Always_SendsProgressNotifications()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                int currentStep = 1;
                var totalSteps = waterLevels.Length * 2;

                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                stabilityStoneCoverWaveConditionsCalculationService.OnProgress += (description, step, steps) =>
                {
                    // Assert
                    var text = string.Format("Waterstand '{0}' berekenen.", waterLevels[(step - 1) % waterLevels.Length]);
                    Assert.AreEqual(text, description);
                    Assert.AreEqual(currentStep++, step);
                    Assert.AreEqual(totalSteps, steps);
                };

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                            assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Always_InputPropertiesCorrectlySendToCalculator()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;

                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                            assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = testWaveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(6, testWaveConditionsInputs.Length);

                GeneralStabilityStoneCoverWaveConditionsInput generalInput = stabilityStoneCoverFailureMechanism.GeneralInput;

                var input = calculation.InputParameters;

                Assert.AreEqual(testDataPath, testWaveConditionsCosineCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, testWaveConditionsCosineCalculator.RingId);

                int waterLevelIndex = 0;
                for (int i = 0; i < testWaveConditionsInputs.Length/2; i++)
                {
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 assessmentSectionStub.FailureMechanismContribution.Norm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
                                                                                 input.WaterLevels.ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralBlocksWaveConditionsInput.A,
                                                                                 generalInput.GeneralBlocksWaveConditionsInput.B,
                                                                                 generalInput.GeneralBlocksWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, testWaveConditionsInputs[i]);
                }

                waterLevelIndex = 0;
                for (int i = testWaveConditionsInputs.Length/2; i < testWaveConditionsInputs.Length; i++)
                {
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 assessmentSectionStub.FailureMechanismContribution.Norm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
                                                                                 input.WaterLevels.ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralColumnsWaveConditionsInput.A,
                                                                                 generalInput.GeneralColumnsWaveConditionsInput.B,
                                                                                 generalInput.GeneralColumnsWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, testWaveConditionsInputs[i]);
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Canceled_HasNoOutput()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                stabilityStoneCoverWaveConditionsCalculationService.Cancel();

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                                        assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                Assert.IsFalse(calculation.HasOutput);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_WithValidInput_SetsOutput()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                        assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.ColumnsOutput.Count());
                Assert.AreEqual(3, calculation.Output.BlocksOutput.Count());
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_InnerCalculationFails_ThrowsException()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = true;

                // Call
                TestDelegate test = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                        assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                Assert.Throws<HydraRingFileParserException>(test);
            }
            mockRepository.VerifyAll();
        }

        private static IAssessmentSection CreateAssessmentSectionStub(IFailureMechanism failureMechanism, MockRepository mockRepository)
        {
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Stub(a => a.Id).Return("21");
            assessmentSectionStub.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, 2));
            return assessmentSectionStub;
        }

        public enum CalculationType
        {
            NoForeshore,
            ForeshoreWithValidBreakWater,
            ForeshoreWithoutBreakWater
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetValidCalculation()
        {
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "locationName", 0, 0)
                    {
                        DesignWaterLevel = (RoundedDouble) 9.3
                    },
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
            return calculation;
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetDefaultValidationInput()
        {
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 5.4;

            return calculation;
        }

        private static ForeshoreProfile CreateForeshoreProfile()
        {
            return CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam, 10.0));
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
    }
}