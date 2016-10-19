// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.FileImporters;
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
        public void Run_NoHydraulicBoundaryDatabase_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 5.3,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble) 5
                }
            };

            var testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");
            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          testFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    Assert.AreEqual(string.Format("Validatie mislukt: Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.", testFilePath), msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 5.3,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble) 5
                }
            };

            var testFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");
            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          testFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    Assert.AreEqual(string.Format("Validatie mislukt: Fout bij het lezen van bestand '{0}': Kon geen locaties verkrijgen van de database.", testFilePath), msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        public void Run_NoHydraulicBoundaryLocation_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 5.3,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble) 5
                }
            };

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        public void Run_NoDesignWaterLevel_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 5.3,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble) 5
                }
            };

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                activity.Run();
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Kan de waterstand bij doorsnede-eis niet afleiden op basis van de invoer.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        [TestCase(double.NaN, 10.0)]
        [TestCase(1.0, double.NaN)]
        public void Run_NoWaterLevels_DoesNotPerformCalculationAndLogsError(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                    UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                    UpperBoundaryWaterLevels = (RoundedDouble) 5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble) 5
                }
            };
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevel = (RoundedDouble) 12.0;

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.NaN)]
        public void Run_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartEnd(double breakWaterHeight)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection);
            calculation.InputParameters.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                 breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);

                    int i = 2;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i + 1]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i + 2]);

                        i = i + 2;
                    }

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
        }

        [Test]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.NaN)]
        public void Run_CalculationWithForeshoreAndUsesBreakWaterAndHasInvalidBreakWaterHeight_DoesNotPerformCalculationAndLogStartEnd(double breakWaterHeight)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection);
            calculation.InputParameters.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                 breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);

                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Er is geen geldige damhoogte ingevoerd.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Run_CalculationWithValidInputConditionsAndValidForeshore_LogCalculationStartAndErrorAndEnd(CalculationType calculationType)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation(assessmentSection);

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

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);

                    int i = 2;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i + 1]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i + 2]);

                        i = i + 2;
                    }

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[7]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(10, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);

                    int i = 2;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i + 1]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i + 2]);
                        i = i + 2;
                    }

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[9]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                List<string> progessTexts = new List<string>();
                activity.ProgressChanged += (sender, args) => { progessTexts.Add(activity.ProgressText); };

                // Call
                activity.Run();

                // Assert
                var waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                for (var i = 0; i < waterLevels.Length; i++)
                {
                    var text = string.Format("Stap {0} van {1} | Waterstand '{2}' berekenen.", i + 1, waterLevels.Length, waterLevels[i]);
                    Assert.AreEqual(text, progessTexts[i]);
                }
            }
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                activity.Run();

                // Assert
                var testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = testWaveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(3, testWaveConditionsInputs.Length);

                int waterLevelIndex = 0;
                foreach (WaveConditionsCosineCalculationInput actualInput in testWaveConditionsInputs)
                {
                    GeneralGrassCoverErosionOutwardsInput generalInput = assessmentSection.GrassCoverErosionOutwards.GeneralInput;

                    var input = calculation.InputParameters;
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 assessmentSection.GrassCoverErosionOutwards.CalculationBeta(assessmentSection),
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
                                                                                 calculation.InputParameters.WaterLevels.ElementAt(waterLevelIndex++),
                                                                                 generalInput.GeneralWaveConditionsInput.A,
                                                                                 generalInput.GeneralWaveConditionsInput.B,
                                                                                 generalInput.GeneralWaveConditionsInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                    Assert.AreEqual(testDataPath, testWaveConditionsCosineCalculator.HydraulicBoundaryDatabaseDirectory);
                    Assert.AreEqual(assessmentSection.Id, testWaveConditionsCosineCalculator.RingId);
                }
            }
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

            using (new HydraRingCalculatorFactoryConfig())
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

                    Assert.AreEqual(6, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, firstWaterLevel), msgs[3]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, firstWaterLevel), msgs[4]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[5]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
                Assert.IsNull(calculation.Output);
            }
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                activity.Run();

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.Items.Count());
            }
        }

        [Test]
        public void Run_CalculationFailed_OutputNull()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                          validFilePath,
                                                                                          assessmentSection.GrassCoverErosionOutwards,
                                                                                          assessmentSection);

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = true;

                // Call
                activity.Run();

                // Assert
                Assert.IsNull(calculation.Output);
            }
        }
        
        public enum CalculationType
        {
            NoForeshore,
            ForeshoreWithValidBreakWater,
            ForeshoreWithoutBreakWater
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation(AssessmentSection assessmentSection)
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
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
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevel = (RoundedDouble) 9.3;
            return calculation;
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetDefaultCalculation(AssessmentSection assessmentSection)
        {
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 5.4;

            return calculation;
        }

        private void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }
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