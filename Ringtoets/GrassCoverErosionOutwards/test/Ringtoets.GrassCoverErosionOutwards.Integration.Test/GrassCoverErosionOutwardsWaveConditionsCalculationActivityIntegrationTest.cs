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
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Service;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Integration.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationActivityIntegrationTest
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
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

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          testFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
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
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[1]);
                    Assert.AreEqual($"Validatie mislukt: Fout bij het lezen van bestand '{testFilePath}': het bestand bestaat niet.", msgs[2]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[3]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationWithValidCalculation_PerformCalculationAndLogStartAndEnd()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(14, msgs.Length);
                    Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[3]);

                    var i = 3;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' is gestart.", msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i + 2]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' is beëindigd.", msgs[i + 3]);
                        i = i + 3;
                    }

                    Assert.AreEqual($"Berekening van '{calculation.Name}' is beëindigd.", msgs[13]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_Always_SetProgressTexts()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count();

            var mockRepository = new MockRepository();
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
                for (var i = 0; i < waterLevels.Length; i++)
                {
                    string text = $"Stap {i + 1} van {waterLevels.Length} | Waterstand '{waterLevels[i]}' berekenen.";
                    Assert.AreEqual(text, progessTexts[i]);
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_Always_InputPropertiesCorrectlySendToService()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();
            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
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
                Assert.AreEqual(3, testWaveConditionsInputs.Length);

                var waterLevelIndex = 0;
                foreach (WaveConditionsCosineCalculationInput actualInput in testWaveConditionsInputs)
                {
                    GeneralGrassCoverErosionOutwardsInput generalInput = assessmentSection.GrassCoverErosionOutwards.GeneralInput;

                    double mechanismSpecificNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                        assessmentSection.FailureMechanismContribution.Norm,
                        assessmentSection.GrassCoverErosionOutwards.Contribution,
                        assessmentSection.GrassCoverErosionOutwards.GeneralInput.N);

                    WaveConditionsInput input = calculation.InputParameters;
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 mechanismSpecificNorm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
                                                                                 calculation.InputParameters.WaterLevels.ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralWaveConditionsInput.A,
                                                                                 generalInput.GeneralWaveConditionsInput.B,
                                                                                 generalInput.GeneralWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Cancel_WhenPerformingCalculation_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator());
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
                    RoundedDouble firstWaterLevel = calculation.InputParameters.WaterLevels.First();

                    Assert.AreEqual(8, msgs.Length);
                    Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[3]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{firstWaterLevel}' is gestart.", msgs[4]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{firstWaterLevel}' is beëindigd.", msgs[6]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(calculation.Name, msgs[7]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
                Assert.IsNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationPerformed_SetsOutput()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count();

            var mockRepository = new MockRepository();
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
                Assert.AreEqual(3, calculation.Output.Items.Count());
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var calculator = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(calculator)
                             .Repeat
                             .Times(nrOfCalculators);
            mockRepository.ReplayAll();

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation, validFilePath, failureMechanism, assessmentSection);
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
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

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation(AssessmentSection assessmentSection)
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
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
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevelCalculation.Output = new TestHydraulicBoundaryLocationOutput(9.3);
            return calculation;
        }

        private void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }
        }
    }
}