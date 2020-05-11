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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Service;
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
using Riskeer.GrassCoverErosionOutwards.Service;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Run_CalculationWithInvalidHydraulicBoundaryDatabaseFilePath_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            assessmentSection.HydraulicBoundaryDatabase.FilePath = invalidFilePath;

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    hydraulicBoundaryLocation
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    Assert.AreEqual("Herstellen van de verbinding met één van de benodigde hydraulische belastingendatabases is mislukt. " +
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = "NonExistingPreprocessorDirectory";

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            assessmentSection.FailureMechanismContribution.LowerLimitNorm = 0.1;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);

            int nrOfCalculators = GetNrOfCalculators(calculation, failureMechanism, assessmentSection);

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
                var progessTexts = new List<string>();
                activity.ProgressChanged += (sender, args) => progessTexts.Add(activity.ProgressText);

                // Call
                activity.Run();

                // Assert
                RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
                int totalSteps = waterLevels.Length * 2;
                for (var i = 0; i < totalSteps; i++)
                {
                    string calculationType = i < waterLevels.Length ? "golfoploop" : "golfklap";
                    string text = $"Stap {i + 1} van {totalSteps} | Waterstand '{waterLevels[i % waterLevels.Length]}' [m+NAP] voor {calculationType} berekenen.";
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            calculation.InputParameters.BreakWater.Type = breakWaterType;

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);

            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();

            RoundedDouble[] waterLevels = GetWaterLevels(calculation, failureMechanism, assessmentSection).ToArray();
            int nrOfCalculators = GetNrOfCalculators(calculation, failureMechanism, assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(waveConditionsCosineCalculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = waveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(nrOfCalculators, testWaveConditionsInputs.Length);

                WaveConditionsInput input = calculation.InputParameters;
                double norm = assessmentSection.FailureMechanismContribution.LowerLimitNorm * 30;

                var waterLevelIndex = 0;
                GeneralGrassCoverErosionOutwardsInput generalInput = failureMechanism.GeneralInput;
                for (var i = 0; i < testWaveConditionsInputs.Length / 2; i++)
                {
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 norm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 waterLevels[waterLevelIndex++],
                                                                                 generalInput.GeneralWaveRunUpWaveConditionsInput.A,
                                                                                 generalInput.GeneralWaveRunUpWaveConditionsInput.B,
                                                                                 generalInput.GeneralWaveRunUpWaveConditionsInput.C);

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
                                                                                 generalInput.GeneralWaveImpactWaveConditionsInput.A,
                                                                                 generalInput.GeneralWaveImpactWaveConditionsInput.B,
                                                                                 generalInput.GeneralWaveImpactWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, testWaveConditionsInputs[i]);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Cancel_WhenPerformingCalculation_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
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
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator());
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.ProgressChanged += (sender, args) =>
                {
                    if (activity.State != ActivityState.Canceled)
                    {
                        // Call
                        activity.Cancel();
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(() => activity.Run(), messages =>
                {
                    string[] msgs = messages.ToArray();
                    RoundedDouble firstWaterLevel = GetWaterLevels(calculation, failureMechanism, assessmentSection).First();

                    Assert.AreEqual(10, msgs.Length);
                    Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);

                    Assert.AreEqual("Berekening voor golfoploop is gestart.", msgs[4]);
                    Assert.AreEqual($"Berekening voor waterstand '{firstWaterLevel}' is gestart.", msgs[5]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[6]);
                    Assert.AreEqual($"Berekening voor waterstand '{firstWaterLevel}' is beëindigd.", msgs[7]);
                    Assert.AreEqual("Berekening voor golfoploop is beëindigd.", msgs[8]);

                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[9]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
                Assert.IsNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_WhenCanceled_OutputNull()
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
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);

            int nrOfCalculators = GetNrOfCalculators(calculation, failureMechanism, assessmentSection);

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
                activity.Run();

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.WaveRunUpOutput.Count());
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var calculator = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            int nrOfCalculators = GetWaterLevels(calculation, failureMechanism, assessmentSection).Count();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);

            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            int nrOfCalculators = GetWaterLevels(calculation, failureMechanism, assessmentSection).Count();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(waveConditionsCosineCalculator)
                             .Repeat
                             .Times(nrOfCalculators);
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabases.First().Locations.First();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);
            int nrOfCalculators = GetNrOfCalculators(calculation, failureMechanism, assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabases.First().Locations.First();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = validPreprocessorDirectory;

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);
            int nrOfCalculators = GetNrOfCalculators(calculation, failureMechanism, assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
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
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabases.First().Locations.First();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
                                                                                                                                        assessmentSection);
            int nrOfCalculators = GetNrOfCalculators(calculation, failureMechanism, assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
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
            calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestWaveConditionsCosineCalculator
                             {
                                 EndInFailure = endInFailure
                             });
            mockRepository.ReplayAll();

            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection, new[]
                {
                    new TestHydraulicBoundaryLocation()
                });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            calculation.Attach(observer);

            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
                                                                                                                                        failureMechanism,
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

        private static int GetNrOfCalculators(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                              GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                              IAssessmentSection assessmentSection)
        {
            return GetWaterLevels(calculation, failureMechanism, assessmentSection).Count() * 2;
        }

        private static void ConfigureAssessmentSectionWithHydraulicBoundaryOutput(IAssessmentSection assessmentSection)
        {
            assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation location)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = location,
                    CategoryType = FailureMechanismCategoryType.FactorizedLowerLimitNorm,
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

        private static AssessmentSectionStub CreateAssessmentSection()
        {
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