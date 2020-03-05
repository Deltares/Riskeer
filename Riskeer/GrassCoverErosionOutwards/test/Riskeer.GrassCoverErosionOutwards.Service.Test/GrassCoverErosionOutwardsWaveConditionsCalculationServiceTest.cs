// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
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

namespace Riskeer.GrassCoverErosionOutwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationServiceTest
    {
        [Test]
        public void Constructor_ReturnsNewInstance()
        {
            // Call
            var service = new GrassCoverErosionOutwardsWaveConditionsCalculationService();

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationServiceBase>(service);
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(null,
                                                                                                                failureMechanism,
                                                                                                                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                null,
                                                                                                                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                failureMechanism,
                                                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Calculate_InvalidCalculationType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    CalculationType = (GrassCoverErosionOutwardsWaveConditionsCalculationType) 99
                }
            };

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                failureMechanism,
                                                                                                                assessmentSection);

            // Assert
            string expectedMessage = $"The value of argument 'calculationType' ({(int) calculation.InputParameters.CalculationType}) " +
                                     $"is invalid for Enum type '{nameof(GrassCoverErosionOutwardsWaveConditionsCalculationType)}'.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage)
                                         .ParamName;
            Assert.AreEqual("calculationType", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterNoInvalidBreakWaterHeight_PerformAndLog(NaN)")]
        public void Calculate_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;

            RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                              failureMechanism,
                                                                                                              assessmentSection);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    const int expectedNrOfMessages = 18;
                    Assert.AreEqual(expectedNrOfMessages, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    AssertCalculationLogs(msgs, GetWaterLevels(calculation, failureMechanism, assessmentSection), "golfoploop", 1);
                    AssertCalculationLogs(msgs, GetWaterLevels(calculation, failureMechanism, assessmentSection), "golfklap", 9);

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[expectedNrOfMessages - 1]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Run_CalculationWithValidInputAndValidForeshore_LogStartAndEnd(CalculationType calculationType)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            switch (calculationType)
            {
                case CalculationType.NoForeshore:
                    calculation.InputParameters.ForeshoreProfile = null;
                    calculation.InputParameters.UseForeshore = false;
                    calculation.InputParameters.UseBreakWater = false;
                    break;
                case CalculationType.ForeshoreWithoutBreakWater:
                    calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
                    calculation.InputParameters.UseBreakWater = false;
                    break;
                case CalculationType.ForeshoreWithValidBreakWater:
                    break;
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                              failureMechanism,
                                                                                                              assessmentSection);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    const int expectedNrOfMessages = 18;
                    Assert.AreEqual(expectedNrOfMessages, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    AssertCalculationLogs(msgs, GetWaterLevels(calculation, failureMechanism, assessmentSection), "golfoploop", 1);
                    AssertCalculationLogs(msgs, GetWaterLevels(calculation, failureMechanism, assessmentSection), "golfklap", 9);

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[expectedNrOfMessages - 1]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact, 18)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp, 10)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact, 10)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact, 10)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact, 18)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.All, 26)]
        public void Calculate_CalculateWithValidInputAndDifferentCalculationTypes_LogStartAndEnd(
            GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType, int expectedMessageCount)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.CalculationType = calculationType;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
            int nrOfCalculators = DetermineAmountOfCalculators(calculationType, waterLevels);

            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                              failureMechanism,
                                                                                                              assessmentSection);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(expectedMessageCount, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    const int messageBlockSize = 8;

                    if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp ||
                        calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact ||
                        calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All)
                    {
                        AssertCalculationLogs(msgs, waterLevels, "golfoploop", 1);
                    }

                    if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact ||
                        calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact ||
                        calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All)
                    {
                        int countStart = 1;
                        if (calculationType != GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact)
                        {
                            countStart += messageBlockSize;
                        }

                        AssertCalculationLogs(msgs, waterLevels, "golfklap", countStart);
                    }

                    if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact ||
                        calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact ||
                        calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All)
                    {
                        int countStart = 1;
                        if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact)
                        {
                            countStart += messageBlockSize;
                        }

                        if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All)
                        {
                            countStart += messageBlockSize * 2;
                        }

                        AssertCalculationLogs(msgs, waterLevels, "golfklap voor toets op maat", countStart);
                    }

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[expectedMessageCount - 1]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_Always_InputPropertiesCorrectlySendToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.BreakWater.Type = breakWaterType;

            RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;

            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(waveConditionsCosineCalculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            // Precondition
            Assert.AreEqual(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact,
                            calculation.InputParameters.CalculationType);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = waveConditionsCosineCalculator.ReceivedInputs.ToArray();
                int nrOfReceivedInputs = testWaveConditionsInputs.Length;
                Assert.AreEqual(nrOfCalculators, nrOfReceivedInputs);

                var waterLevelIndex = 0;
                GeneralGrassCoverErosionOutwardsInput generalInput = failureMechanism.GeneralInput;
                double expectedNorm = RiskeerCommonDataCalculationService.ProfileSpecificRequiredProbability(
                    assessmentSection.FailureMechanismContribution.SignalingNorm / 30,
                    failureMechanism.Contribution,
                    failureMechanism.GeneralInput.N);
                for (var i = 0; i < nrOfReceivedInputs / 2; i++)
                {
                    WaveConditionsInput input = calculation.InputParameters;
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 expectedNorm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 GetWaterLevels(calculation, failureMechanism, assessmentSection).ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralWaveRunUpWaveConditionsInput.A,
                                                                                 generalInput.GeneralWaveRunUpWaveConditionsInput.B,
                                                                                 generalInput.GeneralWaveRunUpWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, testWaveConditionsInputs[i]);
                }

                waterLevelIndex = 0;
                for (int i = nrOfReceivedInputs / 2; i < nrOfReceivedInputs; i++)
                {
                    WaveConditionsInput input = calculation.InputParameters;
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 expectedNorm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 GetWaterLevels(calculation, failureMechanism, assessmentSection).ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralWaveImpactWaveConditionsInput.A,
                                                                                 generalInput.GeneralWaveImpactWaveConditionsInput.B,
                                                                                 generalInput.GeneralWaveImpactWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, testWaveConditionsInputs[i]);
                }

                waterLevelIndex = 0;
                for (int i = nrOfReceivedInputs / 2; i < nrOfReceivedInputs; i++)
                {
                    WaveConditionsInput input = calculation.InputParameters;
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 expectedNorm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 GetWaterLevels(calculation, failureMechanism, assessmentSection).ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralWaveImpactWaveConditionsInput.A,
                                                                                 generalInput.GeneralWaveImpactWaveConditionsInput.B,
                                                                                 generalInput.GeneralWaveImpactWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, testWaveConditionsInputs[i]);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Canceled_HasNoOutput()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var grassCoverErosionOutwardsWaveConditionsCalculationService = new GrassCoverErosionOutwardsWaveConditionsCalculationService();
                grassCoverErosionOutwardsWaveConditionsCalculationService.Cancel();

                // Call
                grassCoverErosionOutwardsWaveConditionsCalculationService.Calculate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

                // Assert
                Assert.IsFalse(calculation.HasOutput);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(waveConditionsCosineCalculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var grassCoverErosionOutwardsWaveConditionsCalculationService = new GrassCoverErosionOutwardsWaveConditionsCalculationService();
                waveConditionsCosineCalculator.CalculationFinishedHandler += (s, e) => grassCoverErosionOutwardsWaveConditionsCalculationService.Cancel();

                // Call
                grassCoverErosionOutwardsWaveConditionsCalculationService.Calculate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(waveConditionsCosineCalculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact, false, false)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp, false, true)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact, true, false)]
        public void Calculate_WithValidInput_SetsOutput(
            GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType, bool waveRunUpNull, bool waveImpactNull)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.CalculationType = calculationType;

            RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
            int nrOfCalculators = DetermineAmountOfCalculators(calculationType, waterLevels);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection);

                // Assert
                GrassCoverErosionOutwardsWaveConditionsOutput calculationOutput = calculation.Output;
                Assert.IsNotNull(calculationOutput);
                Assert.AreEqual(waveRunUpNull, calculationOutput.WaveRunUpOutput == null);
                Assert.AreEqual(waveImpactNull, calculationOutput.WaveImpactOutput == null);

                if (!waveRunUpNull)
                {
                    Assert.AreEqual(3, calculationOutput.WaveRunUpOutput.Count());
                }

                if (!waveImpactNull)
                {
                    Assert.AreEqual(3, calculationOutput.WaveImpactOutput.Count());
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

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

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exception = e;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(16, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("Berekening voor golfoploop is gestart.", msgs[1]);

                    RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
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
            nameof(Calculate_OneOutOfThreeCalculationsFails_ReturnsOutputsAndLogError)
        })]
        public void Calculate_OneOutOfThreeCalculationsFails_ReturnsOutputsAndLogError(bool endInFailure,
                                                                                       string lastErrorFileContent,
                                                                                       string detailedReport)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
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

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
                };

                // Assert
                RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
                RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(25, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    AssertCalculationLogs(msgs, waterLevels, "golfoploop", 1);

                    Assert.AreEqual("Berekening voor golfklap is gestart.", msgs[12]);
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

                    Assert.AreEqual("Berekening voor golfklap is beëindigd.", msgs[23]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[24]);
                });

                WaveConditionsOutput[] waveRunUpOutputs = calculation.Output.WaveRunUpOutput.ToArray();
                Assert.AreEqual(3, waveRunUpOutputs.Length);

                WaveConditionsOutput[] waveImpactOutputs = calculation.Output.WaveImpactOutput.ToArray();
                Assert.AreEqual(3, waveImpactOutputs.Length);

                double targetNorm = CalculateTargetNorm(assessmentSection.FailureMechanismContribution.SignalingNorm / 30,
                                                        failureMechanism.Contribution,
                                                        failureMechanism.GeneralInput.N);
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevelUpperBoundaryRevetment,
                                                                  targetNorm,
                                                                  waveImpactOutputs[0]);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
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
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = TestHelper.GetScratchPadPath();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             }).Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
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
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          failureMechanism,
                                                                                          assessmentSection);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.All)]
        public void Calculate_Always_SendsProgressNotifications(GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureFailureMechanismWithHydraulicBoundaryOutput(failureMechanism);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.InputParameters.CalculationType = calculationType;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator());
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var currentStep = 1;
                var grassCoverErosionOutwardsWaveConditionsCalculationService = new GrassCoverErosionOutwardsWaveConditionsCalculationService();
                grassCoverErosionOutwardsWaveConditionsCalculationService.OnProgressChanged += (description, step, steps) =>
                {
                    // Assert
                    RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
                    int totalSteps = DetermineAmountOfCalculators(calculationType, waterLevels);

                    var revetmentType = "golfoploop";
                    if (step > waterLevels.Length
                        || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact)
                    {
                        revetmentType = "golfklap";
                    }

                    if (step > waterLevels.Length * 2 ||
                        calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.TailorMadeWaveImpact ||
                        step > waterLevels.Length && calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact)
                    {
                        revetmentType = "golfklap voor toets op maat";
                    }

                    string text = $"Waterstand '{waterLevels[(step - 1) % waterLevels.Length]}' [m+NAP] voor {revetmentType} berekenen.";
                    Assert.AreEqual(text, description);
                    Assert.AreEqual(currentStep++, step);
                    Assert.AreEqual(totalSteps, steps);
                };

                // Call
                grassCoverErosionOutwardsWaveConditionsCalculationService.Calculate(calculation,
                                                                                    failureMechanism,
                                                                                    assessmentSection);
            }

            mockRepository.VerifyAll();
        }

        private static int DetermineAmountOfCalculators(GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType, RoundedDouble[] waterLevels)
        {
            if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.All)
            {
                return waterLevels.Length * 3;
            }

            if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact
                || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndTailorMadeWaveImpact)
            {
                return waterLevels.Length * 2;
            }

            return waterLevels.Length;
        }

        private static void AssertCalculationLogs(string[] logMessages, IEnumerable<RoundedDouble> waterLevels, string calculationType, int index)
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

        private static AssessmentSectionStub CreateAssessmentSection()
        {
            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            return assessmentSection;
        }

        private static void ConfigureFailureMechanismWithHydraulicBoundaryOutput(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            failureMechanism.Contribution = 20;
            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);
        }

        private static double CalculateTargetNorm(double norm, double contribution, RoundedDouble N)
        {
            return norm * (contribution / 100) / N;
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation(HydraulicBoundaryLocation location)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = location,
                    CategoryType = FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
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

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetDefaultCalculation(HydraulicBoundaryLocation location)
        {
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(location);
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 5.4;

            return calculation;
        }

        private static IEnumerable<RoundedDouble> GetWaterLevels(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                 GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                 IAssessmentSection assessmentSection)
        {
            return calculation.InputParameters.GetWaterLevels(failureMechanism.GetAssessmentLevel(
                                                                  assessmentSection,
                                                                  calculation.InputParameters.HydraulicBoundaryLocation,
                                                                  calculation.InputParameters.CategoryType));
        }
    }
}