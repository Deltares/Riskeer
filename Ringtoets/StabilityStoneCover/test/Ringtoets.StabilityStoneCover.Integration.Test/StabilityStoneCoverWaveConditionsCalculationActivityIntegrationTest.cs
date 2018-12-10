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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
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
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Run_CalculationWithInvalidHydraulicBoundaryDatabaseFilePath_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryDatabase.FilePath = invalidFilePath;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
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
                    Assert.AreEqual(4, msgs.Length);
                    Assert.AreEqual($"Golfcondities voor blokken en zuilen berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    Assert.AreEqual("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. " +
                                    $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.", msgs[2]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationWithInvalidPreprocessorDirectory_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
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
                    Assert.AreEqual(4, msgs.Length);
                    Assert.AreEqual($"Golfcondities voor blokken en zuilen berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", msgs[2]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidNorm_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            assessmentSection.FailureMechanismContribution.LowerLimitNorm = 0.1;

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
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
                    Assert.AreEqual(4, msgs.Length);
                    Assert.AreEqual($"Golfcondities voor blokken en zuilen berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    Assert.AreEqual("Doelkans is te groot om een berekening uit te kunnen voeren.", msgs[2]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_Always_SetProgressTexts()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(waterLevels.Length * 2);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var progessTexts = new List<string>();
                activity.ProgressChanged += (sender, args) => progessTexts.Add(activity.ProgressText);

                // Call
                activity.Run();

                // Assert
                int totalSteps = waterLevels.Length * 2;
                for (var i = 0; i < totalSteps; i++)
                {
                    string text = $"Stap {i + 1} van {totalSteps} | Berekenen voor een waterstand van '{waterLevels[i % waterLevels.Length]}' [m+NAP].";
                    Assert.AreEqual(text, progessTexts[i]);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Run_Always_InputPropertiesCorrectlySendToService(BreakWaterType breakWaterType)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            calculation.InputParameters.BreakWater.Type = breakWaterType;

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                                  stabilityStoneCoverFailureMechanism,
                                                                                                                                  assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var calculator = new TestWaveConditionsCosineCalculator();
            RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;

            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
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
                double norm = assessmentSection.FailureMechanismContribution.LowerLimitNorm * 30;

                var waterLevelIndex = 0;
                for (var i = 0; i < testWaveConditionsInputs.Length / 2; i++)
                {
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 norm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 waterLevels[waterLevelIndex++],
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
                                                                                 norm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 waterLevels[waterLevelIndex++],
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
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            var calculator = new TestWaveConditionsCosineCalculator();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(calculator);
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
                    RoundedDouble firstWaterLevel = GetWaterLevels(calculation, assessmentSection).First();

                    Assert.AreEqual(10, msgs.Length);
                    Assert.AreEqual($"Golfcondities voor blokken en zuilen berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual("Berekening voor blokken is gestart.", msgs[4]);
                    Assert.AreEqual($"Berekening voor waterstand '{firstWaterLevel}' is gestart.", msgs[5]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[6]);
                    Assert.AreEqual($"Berekening voor waterstand '{firstWaterLevel}' is beëindigd.", msgs[7]);
                    Assert.AreEqual("Berekening voor blokken is beëindigd.", msgs[8]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[9]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Cancel_WhenPerformingCalculationForColumns_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
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
                    RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();

                    Assert.AreEqual(21, msgs.Length);
                    Assert.AreEqual($"Golfcondities voor blokken en zuilen berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    Assert.AreEqual("Berekening voor blokken is gestart.", msgs[4]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevels[0]}' is gestart.", msgs[5]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[6]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevels[0]}' is beëindigd.", msgs[7]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevels[1]}' is gestart.", msgs[8]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[9]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevels[1]}' is beëindigd.", msgs[10]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevels[2]}' is gestart.", msgs[11]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[12]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevels[2]}' is beëindigd.", msgs[13]);
                    Assert.AreEqual("Berekening voor blokken is beëindigd.", msgs[14]);
                    Assert.AreEqual("Berekening voor zuilen is gestart.", msgs[15]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevels[0]}' is gestart.", msgs[16]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[17]);
                    Assert.AreEqual($"Berekening voor waterstand '{waterLevels[0]}' is beëindigd.", msgs[18]);
                    Assert.AreEqual("Berekening voor zuilen is beëindigd.", msgs[19]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[20]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_WhenCanceled_OutputNull()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestWaveConditionsCosineCalculator());
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
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(GetWaterLevels(calculation, assessmentSection).Count() * 2);
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
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestWaveConditionsCosineCalculator
                             {
                                 EndInFailure = endInFailure,
                                 LastErrorFileContent = lastErrorFileContent
                             })
                             .Repeat
                             .Times(GetWaterLevels(calculation, assessmentSection).Count());
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
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestWaveConditionsCosineCalculator
                             {
                                 EndInFailure = endInFailure,
                                 LastErrorFileContent = lastErrorFileContent
                             })
                             .Repeat
                             .Times(GetWaterLevels(calculation, assessmentSection).Count());
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

        [Test]
        public void Run_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_CreateWaveConditionsCosineCalculatorAsExpected()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(GetWaterLevels(calculation, assessmentSection).Count() * 2);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_CreateWaveConditionsCosineCalculatorAsExpected()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = validPreprocessorDirectory;

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.AreEqual(validPreprocessorDirectory, settings.PreprocessorDirectory);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(GetWaterLevels(calculation, assessmentSection).Count() * 2);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_CreateWaveConditionsCosineCalculatorAsExpected()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(GetWaterLevels(calculation, assessmentSection).Count() * 2);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Finish_VariousCalculationEndStates_NotifiesObserversOfCalculation(bool endInFailure)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
                             })
                             .Return(new TestWaveConditionsCosineCalculator
                             {
                                 EndInFailure = endInFailure
                             });
            mockRepository.ReplayAll();

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            calculation.Attach(observer);

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();
            }

            // Call
            activity.Finish();

            // Assert
            mockRepository.VerifyAll();
        }

        private static StabilityStoneCoverWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new StabilityStoneCoverWaveConditionsCalculation
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

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);

            return assessmentSection;
        }

        private static IEnumerable<RoundedDouble> GetWaterLevels(StabilityStoneCoverWaveConditionsCalculation calculation, IAssessmentSection assessmentSection)
        {
            return calculation.InputParameters.GetWaterLevels(assessmentSection.GetAssessmentLevel(
                                                                  calculation.InputParameters.HydraulicBoundaryLocation,
                                                                  calculation.InputParameters.CategoryType));
        }
    }
}