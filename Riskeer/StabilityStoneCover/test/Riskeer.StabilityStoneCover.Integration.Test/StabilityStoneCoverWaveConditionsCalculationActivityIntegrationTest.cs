// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Revetment.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Service;

namespace Riskeer.StabilityStoneCover.Integration.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationActivityIntegrationTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        [Test]
        public void Run_CalculationWithInvalidHrdFilePath_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryData =
                {
                    HydraulicLocationConfigurationDatabase =
                    {
                        FilePath = validHlcdFilePath
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
                }
            };

            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    Assert.AreEqual("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. " +
                                    $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.", msgs[2]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            calculatorFactory.DidNotReceive().CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        public void Run_Always_SetProgressTexts()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(new TestWaveConditionsCosineCalculator());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var progressTexts = new List<string>();
                activity.ProgressChanged += (sender, args) => progressTexts.Add(activity.ProgressText);

                // Call
                activity.Run();

                // Assert
                int totalSteps = waterLevels.Length * 2;
                for (var i = 0; i < totalSteps; i++)
                {
                    string revetment = i < waterLevels.Length ? "blokken" : "zuilen";
                    var text = $"Stap {i + 1} van {totalSteps} | Waterstand '{waterLevels[i % waterLevels.Length]}' [m+NAP] voor {revetment} berekenen.";
                    Assert.AreEqual(text, progressTexts[i]);
                }
            }

            calculatorFactory.Received(waterLevels.Length * 2)
                             .CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Run_Always_InputPropertiesCorrectlySentToService(BreakWaterType breakWaterType)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            calculation.InputParameters.BreakWater.Type = breakWaterType;

            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                                  stabilityStoneCoverFailureMechanism,
                                                                                                                                  assessmentSection);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            var calculator = new TestWaveConditionsCosineCalculator();
            RoundedDouble[] waterLevels = GetWaterLevels(calculation, assessmentSection).ToArray();
            int nrOfCalculators = waterLevels.Length * 2;
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                WaveConditionsCosineCalculationInput[] waveConditionsInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(nrOfCalculators, waveConditionsInputs.Length);

                GeneralStabilityStoneCoverWaveConditionsInput generalInput = stabilityStoneCoverFailureMechanism.GeneralInput;

                WaveConditionsInput input = calculation.InputParameters;
                double targetProbability = assessmentSection.FailureMechanismContribution.MaximumAllowableFloodingProbability;

                var waterLevelIndex = 0;
                for (var i = 0; i < waveConditionsInputs.Length / 2; i++)
                {
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 targetProbability,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 waterLevels[waterLevelIndex++],
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
                                                                                 targetProbability,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                                                                                 waterLevels[waterLevelIndex++],
                                                                                 generalInput.GeneralColumnsWaveConditionsInput.A,
                                                                                 generalInput.GeneralColumnsWaveConditionsInput.B,
                                                                                 generalInput.GeneralColumnsWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, waveConditionsInputs[i]);
                }
            }

            calculatorFactory.Received(nrOfCalculators)
                             .CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        public void Cancel_WhenPerformingCalculationForBlocks_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            var calculator = new TestWaveConditionsCosineCalculator();
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

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
                    Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
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

            calculatorFactory.Received(1)
                             .CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        public void Cancel_WhenPerformingCalculationForColumns_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(new TestWaveConditionsCosineCalculator());

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
                    Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs[0]);
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

            calculatorFactory.Received(4)
                             .CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        public void Run_WhenCanceled_OutputNull()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(new TestWaveConditionsCosineCalculator());

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

            calculatorFactory.Received(1)
                             .CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        public void Run_CalculationPerformed_SetsOutput()
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(new TestWaveConditionsCosineCalculator());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.ColumnsOutput.Count());
                Assert.AreEqual(3, calculation.Output.BlocksOutput.Count());
            }

            calculatorFactory.Received(GetWaterLevels(calculation, assessmentSection).Count() * 2)
                             .CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
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
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(new TestWaveConditionsCosineCalculator
                             {
                                 EndInFailure = endInFailure,
                                 LastErrorFileContent = lastErrorFileContent
                              });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            calculatorFactory.Received(GetWaterLevels(calculation, assessmentSection).Count())
                             .CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
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
            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(new TestWaveConditionsCosineCalculator
                             {
                                 EndInFailure = endInFailure,
                                 LastErrorFileContent = lastErrorFileContent
                              });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.IsNull(calculation.Output);
            }

            calculatorFactory.Received(GetWaterLevels(calculation, assessmentSection).Count())
                             .CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Run_HydraulicBoundaryDataSet_CreateWaveConditionsCosineCalculatorAsExpected(bool usePreprocessorClosure)
        {
            // Setup
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput(usePreprocessorClosure);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First();

            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                calculation,
                new StabilityStoneCoverFailureMechanism(),
                assessmentSection);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(callInfo =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         assessmentSection.HydraulicBoundaryData,
                                         hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) callInfo[0]);

                                 return new TestWaveConditionsCosineCalculator();
                             })
                             ;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            calculatorFactory.Received(GetWaterLevels(calculation, assessmentSection).Count() * 2)
                             .CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Finish_VariousCalculationEndStates_NotifiesObserversOfCalculation(bool endInFailure)
        {
            // Setup
            var observer = Substitute.For<IObserver>();

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(new TestWaveConditionsCosineCalculator
                             {
                                 EndInFailure = endInFailure
                             });

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

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
            observer.Received(1).UpdateObserver();
        }

        private static StabilityStoneCoverWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
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
                    StepSize = (RoundedDouble) 0.5,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
        }

        private static IAssessmentSection CreateAssessmentSectionWithHydraulicBoundaryOutput(bool usePreprocessorClosure = false)
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryData =
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
                            UsePreprocessorClosure = usePreprocessorClosure,
                            Locations =
                            {
                                hydraulicBoundaryLocation
                            }
                        }
                    }
                }
            };

            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);

            return assessmentSection;
        }

        private static IEnumerable<RoundedDouble> GetWaterLevels(StabilityStoneCoverWaveConditionsCalculation calculation, IAssessmentSection assessmentSection)
        {
            RoundedDouble assessmentLevel = WaveConditionsInputHelper.GetAssessmentLevel(calculation.InputParameters, assessmentSection);

            return calculation.InputParameters.GetWaterLevels(assessmentLevel);
        }
    }
}