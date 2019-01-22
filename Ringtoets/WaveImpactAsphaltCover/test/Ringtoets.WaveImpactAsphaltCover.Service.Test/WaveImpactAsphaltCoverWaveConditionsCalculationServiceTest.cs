// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Service;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.Service.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Constructor_ReturnsNewInstance()
        {
            // Call
            var service = new WaveImpactAsphaltCoverWaveConditionsCalculationService();

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

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            TestDelegate test = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                null,
                assessmentSection,
                failureMechanism.GeneralInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            TestDelegate test = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                calculation,
                null,
                failureMechanism.GeneralInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Calculate_GeneralInputNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation(new TestHydraulicBoundaryLocation());

            // Call
            TestDelegate test = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                calculation,
                assessmentSection,
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalWaveConditionsInput", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(NaN)")]
        public void Calculate_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;
            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Twice();
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                    calculation,
                    assessmentSection,
                    waveImpactAsphaltCoverFailureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    var i = 0;
                    foreach (RoundedDouble waterLevel in GetWaterLevels(calculation, assessmentSection))
                    {
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i + 2]);
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", msgs[i + 3]);

                        i = i + 3;
                    }

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
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
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Twice();
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
                Action call = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                    calculation,
                    assessmentSection,
                    waveImpactAsphaltCoverFailureMechanism.GeneralInput);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    var i = 0;
                    foreach (RoundedDouble waterLevel in GetWaterLevels(calculation, assessmentSection))
                    {
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i + 2]);
                        Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", msgs[i + 3]);

                        i = i + 3;
                    }

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[7]);
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
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            calculation.InputParameters.BreakWater.Type = breakWaterType;

            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var calculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator)
                             .Repeat
                             .Times(3);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                    calculation,
                    assessmentSection,
                    waveImpactAsphaltCoverFailureMechanism.GeneralInput);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(3, testWaveConditionsInputs.Length);

                var waterLevelIndex = 0;
                foreach (WaveConditionsCosineCalculationInput actualInput in testWaveConditionsInputs)
                {
                    GeneralWaveConditionsInput generalInput = waveImpactAsphaltCoverFailureMechanism.GeneralInput;

                    WaveConditionsInput input = calculation.InputParameters;
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 assessmentSection.FailureMechanismContribution.LowerLimitNorm * 30,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 GetWaterLevels(calculation, assessmentSection).ElementAt(waterLevelIndex++),
                                                                                 generalInput.A,
                                                                                 generalInput.B,
                                                                                 generalInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Canceled_HasNoOutput()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var waveImpactAsphaltCoverWaveConditionsCalculationService = new WaveImpactAsphaltCoverWaveConditionsCalculationService();
                waveImpactAsphaltCoverWaveConditionsCalculationService.Cancel();

                // Call
                waveImpactAsphaltCoverWaveConditionsCalculationService.Calculate(
                    calculation,
                    assessmentSection,
                    waveImpactAsphaltCoverFailureMechanism.GeneralInput);

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
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var calculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var waveImpactAsphaltCoverWaveConditionsCalculationService = new WaveImpactAsphaltCoverWaveConditionsCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => waveImpactAsphaltCoverWaveConditionsCalculationService.Cancel();

                // Call
                waveImpactAsphaltCoverWaveConditionsCalculationService.Calculate(
                    calculation,
                    assessmentSection,
                    waveImpactAsphaltCoverFailureMechanism.GeneralInput);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_WithValidInput_SetsOutput()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(3);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                    calculation,
                    assessmentSection,
                    waveImpactAsphaltCoverFailureMechanism.GeneralInput);

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.Items.Count());
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_OneOutOfThreeCalculationsFails_ReturnsOutputsAndLogError)
        })]
        public void Calculate_ThreeCalculationsFail_ThrowsHydraRingCalculationExceptionAndLogError(bool endInFailure,
                                                                                                   string lastErrorFileContent,
                                                                                                   string detailedReport)
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                Contribution = 20
            };
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
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                            calculation,
                            assessmentSection,
                            failureMechanism.GeneralInput);
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
                    Assert.AreEqual(15, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
                    RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                    RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                    RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is gestart.", msgs[1]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[2]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is beëindigd.", msgs[4]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is gestart.", msgs[5]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelMiddleRevetment}'. {detailedReport}", msgs[6]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[7]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is beëindigd.", msgs[8]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is gestart.", msgs[9]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelLowerBoundaryRevetment}'. {detailedReport}", msgs[10]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[11]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is beëindigd.", msgs[12]);

                    Assert.AreEqual("Berekening is mislukt voor alle waterstanden.", msgs[13]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[14]);
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
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                Contribution = 20
            };
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

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new WaveImpactAsphaltCoverWaveConditionsCalculationService();

                // Call
                Action call = () => service.Calculate(calculation,
                                                      assessmentSection,
                                                      failureMechanism.GeneralInput);

                // Assert
                RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
                RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(12, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is gestart.", msgs[1]);
                    Assert.AreEqual($"Berekening is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[2]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelUpperBoundaryRevetment}' is beëindigd.", msgs[4]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is gestart.", msgs[5]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[6]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelMiddleRevetment}' is beëindigd.", msgs[7]);

                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is gestart.", msgs[8]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[9]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevelLowerBoundaryRevetment}' is beëindigd.", msgs[10]);

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[11]);
                });

                WaveConditionsOutput[] waveConditionsOutputs = calculation.Output.Items.ToArray();
                Assert.AreEqual(3, waveConditionsOutputs.Length);

                double targetNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm * 30;
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevelUpperBoundaryRevetment,
                                                                  targetNorm,
                                                                  waveConditionsOutputs[0]);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var calculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator)
                             .Repeat
                             .Times(3);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                    calculation,
                    assessmentSection,
                    waveImpactAsphaltCoverFailureMechanism.GeneralInput);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = validPreprocessorDirectory;

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var calculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator)
                             .Repeat
                             .Times(3);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                    calculation,
                    assessmentSection,
                    waveImpactAsphaltCoverFailureMechanism.GeneralInput);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_ExpectedPreprocessorDirectorySetToCalculator()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var calculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator)
                             .Repeat
                             .Times(3);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(
                    calculation,
                    assessmentSection,
                    waveImpactAsphaltCoverFailureMechanism.GeneralInput);
            }

            // Assert
            mockRepository.VerifyAll();
        }

        private static IAssessmentSection CreateAssessmentSectionWithHydraulicBoundaryOutput()
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

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

            assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);

            return assessmentSection;
        }

        private static WaveImpactAsphaltCoverWaveConditionsCalculation GetValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = AssessmentSectionCategoryType.FactorizedLowerLimitNorm,
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

        private static WaveImpactAsphaltCoverWaveConditionsCalculation GetDefaultCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(hydraulicBoundaryLocation);
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 5.4;

            return calculation;
        }

        private static IEnumerable<RoundedDouble> GetWaterLevels(WaveImpactAsphaltCoverWaveConditionsCalculation calculation, IAssessmentSection assessmentSection)
        {
            return calculation.InputParameters.GetWaterLevels(assessmentSection.GetAssessmentLevel(
                                                                  calculation.InputParameters.HydraulicBoundaryLocation,
                                                                  calculation.InputParameters.CategoryType));
        }
    }
}