﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
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
using Riskeer.Revetment.Service;
using Riskeer.StabilityStoneCover.Data;

namespace Riskeer.StabilityStoneCover.Service.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationServiceTest
    {
        [Test]
        public void Constructor_ReturnsNewInstance()
        {
            // Call
            var service = new StabilityStoneCoverWaveConditionsCalculationService();

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationServiceBase>(service);
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            void Call() => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(null, assessmentSection,
                                                                                               failureMechanism.GeneralInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            void Call() => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation, null,
                                                                                               failureMechanism.GeneralInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Calculate_GeneralInputNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());

            // Call
            void Call() => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation, assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalWaveConditionsInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_InvalidCalculationType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.CalculationType = (StabilityStoneCoverWaveConditionsCalculationType) 99;

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            void Call() => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation, assessmentSection,
                                                                                               failureMechanism.GeneralInput);

            // Assert
            string expectedMessage = $"The value of argument 'calculationType' ({calculation.InputParameters.CalculationType}) " +
                                     $"is invalid for Enum type '{nameof(StabilityStoneCoverWaveConditionsCalculationType)}'.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage)
                                         .ParamName;
            Assert.AreEqual("calculationType", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(NaN)")]
        public void Calculate_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation, assessmentSection,
                                                                                                   stabilityStoneCoverFailureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(18, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    AssertCalculationLogs(msgs, waterLevels, "blokken", 1);
                    AssertCalculationLogs(msgs, waterLevels, "zuilen", 9);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[17]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(ForeshoreCalculationType.NoForeshore)]
        [TestCase(ForeshoreCalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(ForeshoreCalculationType.ForeshoreWithValidBreakWater)]
        public void Calculate_CalculationWithValidInputConditionsAndValidForeshore_LogStartAndEnd(ForeshoreCalculationType calculationType)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            switch (calculationType)
            {
                case ForeshoreCalculationType.NoForeshore:
                    calculation.InputParameters.ForeshoreProfile = null;
                    calculation.InputParameters.UseForeshore = false;
                    calculation.InputParameters.UseBreakWater = false;
                    break;
                case ForeshoreCalculationType.ForeshoreWithoutBreakWater:
                    calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
                    calculation.InputParameters.UseBreakWater = false;
                    break;
                case ForeshoreCalculationType.ForeshoreWithValidBreakWater:
                    break;
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation, assessmentSection,
                                                                                                   stabilityStoneCoverFailureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(18, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    AssertCalculationLogs(msgs, waterLevels, "blokken", 1);
                    AssertCalculationLogs(msgs, waterLevels, "zuilen", 9);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[17]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Both, 18)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Blocks, 10)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Columns, 10)]
        public void Calculate_CalculateWithValidInputAndDifferentCalculationTypes_LogStartAndEnd(
            StabilityStoneCoverWaveConditionsCalculationType calculationType, int expectedMessageCount)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.CalculationType = calculationType;

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
            int nrOfCalculators = calculationType == StabilityStoneCoverWaveConditionsCalculationType.Both
                                      ? waterLevels.Length * 2
                                      : waterLevels.Length;
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation, assessmentSection,
                                                                                                   stabilityStoneCoverFailureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(expectedMessageCount, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Blocks
                        || calculationType == StabilityStoneCoverWaveConditionsCalculationType.Both)
                    {
                        AssertCalculationLogs(msgs, waterLevels, "blokken", 1);
                    }

                    if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Columns
                        || calculationType == StabilityStoneCoverWaveConditionsCalculationType.Both)
                    {
                        int countStart = calculationType != StabilityStoneCoverWaveConditionsCalculationType.Columns
                                             ? expectedMessageCount / 2
                                             : 1;
                        AssertCalculationLogs(msgs, waterLevels, "zuilen", countStart);
                    }

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[expectedMessageCount - 1]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Both)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Blocks)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Columns)]
        public void Calculate_Always_SendsProgressNotifications(StabilityStoneCoverWaveConditionsCalculationType calculationType)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.CalculationType = calculationType;

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator());
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var currentStep = 1;
                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                stabilityStoneCoverWaveConditionsCalculationService.OnProgressChanged += (description, step, steps) =>
                {
                    // Assert
                    RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
                    int totalSteps = calculationType == StabilityStoneCoverWaveConditionsCalculationType.Both
                                         ? waterLevels.Length * 2
                                         : waterLevels.Length;

                    var revetmentType = "blokken";
                    if (step > waterLevels.Length
                        || calculationType == StabilityStoneCoverWaveConditionsCalculationType.Columns)
                    {
                        revetmentType = "zuilen";
                    }

                    var text = $"Waterstand '{waterLevels[(step - 1) % waterLevels.Length]}' [m+NAP] voor {revetmentType} berekenen.";
                    Assert.AreEqual(text, description);
                    Assert.AreEqual(currentStep++, step);
                    Assert.AreEqual(totalSteps, steps);
                };

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                              assessmentSection,
                                                                              stabilityStoneCoverFailureMechanism.GeneralInput);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_Always_InputPropertiesCorrectlySentToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            calculation.InputParameters.BreakWater.Type = breakWaterType;

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                    assessmentSection,
                                                                                    stabilityStoneCoverFailureMechanism.GeneralInput);

                // Assert
                WaveConditionsCosineCalculationInput[] waveConditionsInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(6, waveConditionsInputs.Length);

                GeneralStabilityStoneCoverWaveConditionsInput generalInput = stabilityStoneCoverFailureMechanism.GeneralInput;

                WaveConditionsInput input = calculation.InputParameters;

                double expectedTargetProbability = assessmentSection.FailureMechanismContribution.MaximumAllowableFloodingProbability;

                var waterLevelIndex = 0;
                for (var i = 0; i < waveConditionsInputs.Length / 2; i++)
                {
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 expectedTargetProbability,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 waterLevels.ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralBlocksWaveConditionsInput.A,
                                                                                 generalInput.GeneralBlocksWaveConditionsInput.B,
                                                                                 generalInput.GeneralBlocksWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, waveConditionsInputs[i]);
                }

                waterLevelIndex = 0;
                for (int i = waveConditionsInputs.Length / 2; i < waveConditionsInputs.Length; i++)
                {
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 expectedTargetProbability,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 waterLevels.ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralColumnsWaveConditionsInput.A,
                                                                                 generalInput.GeneralColumnsWaveConditionsInput.B,
                                                                                 generalInput.GeneralColumnsWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, waveConditionsInputs[i]);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Canceled_HasNoOutput()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                stabilityStoneCoverWaveConditionsCalculationService.Cancel();

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                              assessmentSection,
                                                                              stabilityStoneCoverFailureMechanism.GeneralInput);

                // Assert
                Assert.IsFalse(calculation.HasOutput);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => stabilityStoneCoverWaveConditionsCalculationService.Cancel();

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                              assessmentSection, stabilityStoneCoverFailureMechanism.GeneralInput);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Both, false, false)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Blocks, false, true)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Columns, true, false)]
        public void Calculate_WithValidInput_SetsOutput(StabilityStoneCoverWaveConditionsCalculationType calculationType, bool blocksNull, bool columnsNull)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.CalculationType = calculationType;

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator());
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                    assessmentSection,
                                                                                    stabilityStoneCoverFailureMechanism.GeneralInput);

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(blocksNull, calculation.Output.BlocksOutput == null);
                Assert.AreEqual(columnsNull, calculation.Output.ColumnsOutput == null);

                if (!columnsNull)
                {
                    Assert.AreEqual(3, calculation.Output.ColumnsOutput.Count());
                }

                if (!blocksNull)
                {
                    Assert.AreEqual(3, calculation.Output.BlocksOutput.Count());
                }
            }

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
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
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
                             .Times(3);
            mockRepository.ReplayAll();

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

                // Call
                void Call()
                {
                    try
                    {
                        new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);
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
                    Assert.AreEqual(16, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("Berekening voor blokken is gestart.", msgs[1]);

                    RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
                    RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                    RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                    RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is gestart.", msgs[2]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[3]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is beëindigd.", msgs[5]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is gestart.", msgs[6]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelMiddleRevetment}'. {detailedReport}", msgs[7]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[8]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is beëindigd.", msgs[9]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is gestart.", msgs[10]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelLowerBoundaryRevetment}'. {detailedReport}", msgs[11]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[12]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is beëindigd.", msgs[13]);

                    Assert.AreEqual("Berekening is mislukt voor alle waterstanden.", msgs[14]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[15]);
                });
                Assert.IsInstanceOf<HydraRingCalculationException>(exception);
                Assert.AreEqual("Berekening is mislukt voor alle waterstanden.", exception.Message);
                Assert.IsNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_OneOutOfThreeBlocksCalculationsFails_ReturnsOutputsAndLogError)
        })]
        public void Calculate_OneOutOfThreeBlocksCalculationsFails_ReturnsOutputsAndLogError(bool endInFailure,
                                                                                             string lastErrorFileContent,
                                                                                             string detailedReport)
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
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
                             .Times(5);
            mockRepository.ReplayAll();

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new StabilityStoneCoverWaveConditionsCalculationService();

                // Call
                void Call() => service.Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
                RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(25, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("Berekening voor blokken is gestart.", msgs[1]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is gestart.", msgs[2]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[3]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is beëindigd.", msgs[5]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is gestart.", msgs[6]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[7]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is beëindigd.", msgs[8]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is gestart.", msgs[9]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[10]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is beëindigd.", msgs[11]);

                    Assert.AreEqual("Berekening voor blokken is beëindigd.", msgs[12]);
                    Assert.AreEqual("Berekening voor zuilen is gestart.", msgs[13]);

                    var i = 14;
                    foreach (RoundedDouble waterLevel in waterLevels)
                    {
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", msgs[i++]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i++]);
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", msgs[i++]);
                    }

                    Assert.AreEqual("Berekening voor zuilen is beëindigd.", msgs[23]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[24]);
                });

                WaveConditionsOutput[] blocksWaveConditionsOutputs = calculation.Output.BlocksOutput.ToArray();
                Assert.AreEqual(3, blocksWaveConditionsOutputs.Length);

                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevelUpperBoundaryRevetment,
                                                                  assessmentSection.FailureMechanismContribution.MaximumAllowableFloodingProbability,
                                                                  blocksWaveConditionsOutputs[0]);

                WaveConditionsOutput[] columnsWaveConditionsOutputs = calculation.Output.ColumnsOutput.ToArray();
                Assert.AreEqual(3, columnsWaveConditionsOutputs.Length);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_OneOutOfThreeColumnsCalculationsFails_ReturnsOutputsAndLogError)
        })]
        public void Calculate_OneOutOfThreeColumnsCalculationsFails_ReturnsOutputsAndLogError(bool endInFailure,
                                                                                              string lastErrorFileContent,
                                                                                              string detailedReport)
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculatorThatFails = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(3);
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculatorThatFails);
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Twice();
            mockRepository.ReplayAll();

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new StabilityStoneCoverWaveConditionsCalculationService();

                // Call
                void Call() => service.Calculate(calculation, assessmentSection, failureMechanism.GeneralInput);

                // Assert
                RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
                RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(25, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("Berekening voor blokken is gestart.", msgs[1]);

                    var i = 2;
                    foreach (RoundedDouble waterLevel in waterLevels)
                    {
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", msgs[i++]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i++]);
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", msgs[i++]);
                    }

                    Assert.AreEqual("Berekening voor blokken is beëindigd.", msgs[11]);
                    Assert.AreEqual("Berekening voor zuilen is gestart.", msgs[12]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is gestart.", msgs[13]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[14]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[15]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is beëindigd.", msgs[16]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is gestart.", msgs[17]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[18]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is beëindigd.", msgs[19]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is gestart.", msgs[20]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[21]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is beëindigd.", msgs[22]);

                    Assert.AreEqual("Berekening voor zuilen is beëindigd.", msgs[23]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[24]);
                });

                WaveConditionsOutput[] blocksWaveConditionsOutputs = calculation.Output.BlocksOutput.ToArray();
                Assert.AreEqual(3, blocksWaveConditionsOutputs.Length);

                WaveConditionsOutput[] columnsWaveConditionsOutputs = calculation.Output.ColumnsOutput.ToArray();
                Assert.AreEqual(3, columnsWaveConditionsOutputs.Length);

                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevelUpperBoundaryRevetment,
                                                                  assessmentSection.FailureMechanismContribution.MaximumAllowableFloodingProbability,
                                                                  columnsWaveConditionsOutputs[0]);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculators()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            int nrOfCalculators = GetWaterLevels(calculation, assessmentSection).Count() * 2;
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                    assessmentSection,
                                                                                    stabilityStoneCoverFailureMechanism.GeneralInput);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_ExpectedPreprocessorDirectorySetToCalculators()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = TestHelper.GetScratchPadPath();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            int nrOfCalculators = GetWaterLevels(calculation, assessmentSection).Count() * 2;
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                    assessmentSection,
                                                                                    stabilityStoneCoverFailureMechanism.GeneralInput);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculators()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            int nrOfCalculators = GetWaterLevels(calculation, assessmentSection).Count() * 2;
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                    assessmentSection,
                                                                                    stabilityStoneCoverFailureMechanism.GeneralInput);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        private static void AssertCalculationLogs(string[] logMessages, RoundedDouble[] waterLevels, string calculationType, int index)
        {
            Assert.AreEqual($"Berekening voor {calculationType} is gestart.", logMessages[index++]);

            foreach (RoundedDouble waterLevel in waterLevels)
            {
                Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", logMessages[index++]);
                StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", logMessages[index++]);
                Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", logMessages[index++]);
            }

            Assert.AreEqual($"Berekening voor {calculationType} is beëindigd.", logMessages[index]);
        }

        private static IAssessmentSection CreateAssessmentSectionWithHydraulicBoundaryOutput()
        {
            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0, new HrdFile());

            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath,
                    Locations =
                    {
                        hydraulicBoundaryLocation
                    }
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);

            return assessmentSection;
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    WaterLevelType = WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability,
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

        private static StabilityStoneCoverWaveConditionsCalculation GetDefaultCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(hydraulicBoundaryLocation);
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 5.4;

            return calculation;
        }

        private static IEnumerable<RoundedDouble> GetWaterLevels(StabilityStoneCoverWaveConditionsCalculation calculation, IAssessmentSection assessmentSection)
        {
            return calculation.InputParameters.GetWaterLevels(WaveConditionsInputHelper.GetAssessmentLevel(calculation.InputParameters, assessmentSection));
        }
    }
}