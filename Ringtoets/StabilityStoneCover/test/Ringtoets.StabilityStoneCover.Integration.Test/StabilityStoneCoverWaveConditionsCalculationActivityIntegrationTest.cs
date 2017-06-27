﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Service;

namespace Ringtoets.StabilityStoneCover.Integration.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationActivityIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private string validFilePath;

        [SetUp]
        public void SetUp()
        {
            validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        }

        [Test]
        public void Run_CalculationWithInvalidInput_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 5.3,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble) 5
                }
            };

            string testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                                                                                        stabilityStoneCoverFailureMechanism, mockRepository));
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual($"Validatie mislukt: Fout bij het lezen van bestand '{testFilePath}': het bestand bestaat niet.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_Always_SetProgressTexts()
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                                                                                        stabilityStoneCoverFailureMechanism, mockRepository));
            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count() * 2;

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var progessTexts = new List<string>();
                activity.ProgressChanged += (sender, args) => { progessTexts.Add(activity.ProgressText); };

                // Call
                activity.Run();

                // Assert
                RoundedDouble[] waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                int totalSteps = waterLevels.Length * 2;
                for (var i = 0; i < totalSteps; i++)
                {
                    string text = $"Stap {i + 1} van {totalSteps} | Waterstand '{waterLevels[i % waterLevels.Length]}' berekenen.";
                    Assert.AreEqual(text, progessTexts[i]);
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_Always_InputPropertiesCorrectlySendToService()
        {
            // Setup
            const int nrOfCalculators = 6;
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(stabilityStoneCoverFailureMechanism,
                                                                                                                                  mockRepository);
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    assessmentSectionStub);
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var calculator = new TestWaveConditionsCosineCalculator();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(nrOfCalculators, testWaveConditionsInputs.Length);

                GeneralStabilityStoneCoverWaveConditionsInput generalInput = stabilityStoneCoverFailureMechanism.GeneralInput;

                WaveConditionsInput input = calculation.InputParameters;

                var waterLevelIndex = 0;
                for (var i = 0; i < testWaveConditionsInputs.Length / 2; i++)
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
                for (int i = testWaveConditionsInputs.Length / 2; i < testWaveConditionsInputs.Length; i++)
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
        public void Cancel_WhenPerformingCalculationForBlocks_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                                                                                        stabilityStoneCoverFailureMechanism, mockRepository));
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var calculator = new TestWaveConditionsCosineCalculator();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(calculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.ProgressChanged += (sender, args) =>
                {
                    if (activity.State != ActivityState.Canceled && activity.ProgressText.Contains("Stap 1 van 6"))
                    {
                        // Call
                        activity.Cancel();
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(() => activity.Run(), messages =>
                {
                    string[] msgs = messages.ToArray();
                    RoundedDouble firstWaterLevel = calculation.InputParameters.WaterLevels.First();

                    Assert.AreEqual(9, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[2]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[3]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{firstWaterLevel}' is gestart.", msgs[4]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{firstWaterLevel}' is beëindigd.", msgs[6]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken beëindigd.", msgs[7]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(calculation.Name, msgs[8]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Cancel_WhenPerformingCalculationForColumns_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
        {
            // Setup
            var mockRepository = new MockRepository();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                                                                                        stabilityStoneCoverFailureMechanism, mockRepository));

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(4);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.ProgressChanged += (sender, args) =>
                {
                    if (activity.State != ActivityState.Canceled && activity.ProgressText.Contains("Stap 4 van 6"))
                    {
                        // Call
                        activity.Cancel();
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(() => activity.Run(), messages =>
                {
                    string[] msgs = messages.ToArray();
                    RoundedDouble[] waterLevels = calculation.InputParameters.WaterLevels.ToArray();

                    Assert.AreEqual(20, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[2]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[3]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevels[0]}' is gestart.", msgs[4]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevels[0]}' is beëindigd.", msgs[6]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevels[1]}' is gestart.", msgs[7]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[8]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevels[1]}' is beëindigd.", msgs[9]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevels[2]}' is gestart.", msgs[10]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[11]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevels[2]}' is beëindigd.", msgs[12]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken beëindigd.", msgs[13]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen gestart.", msgs[14]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevels[0]}' is gestart.", msgs[15]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[16]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevels[0]}' is beëindigd.", msgs[17]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen beëindigd.", msgs[18]);
                    Assert.AreEqual($"Berekening van '{calculation.Name}' is beëindigd.", msgs[19]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_WhenCanceled_OutputNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                                                                                        stabilityStoneCoverFailureMechanism, mockRepository));
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator());
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.ProgressChanged += (sender, args) =>
                {
                    if (activity.State != ActivityState.Canceled)
                    {
                        activity.Cancel();
                    }
                };

                activity.Run();

                // Call
                activity.Finish();

                // Assert
                Assert.AreEqual(ActivityState.Canceled, activity.State);
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationPerformed_SetsOutput()
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                                                                                        stabilityStoneCoverFailureMechanism, mockRepository));
            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count() * 2;

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.ColumnsOutput.Count());
                Assert.AreEqual(3, calculation.Output.BlocksOutput.Count());
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_ErrorInCalculation_ActivityStateFailed)
        })]
        public void Run_ErrorInCalculation_ActivityStateFailed(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            var mockRepository = new MockRepository();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            }).Repeat.Times(calculation.InputParameters.WaterLevels.Count());

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    failureMechanism,
                                                                                    AssessmentSectionHelper.CreateAssessmentSectionStub(
                                                                                        failureMechanism, mockRepository, validFilePath));
            mockRepository.ReplayAll();
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_CalculationFailed_OutputNull)
        })]
        public void Run_CalculationFailed_OutputNull(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                                                                                        stabilityStoneCoverFailureMechanism, mockRepository));
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            }).Repeat.Times(calculation.InputParameters.WaterLevels.Count());
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetValidCalculation()
        {
            return new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(9.3),
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
        }
    }
}