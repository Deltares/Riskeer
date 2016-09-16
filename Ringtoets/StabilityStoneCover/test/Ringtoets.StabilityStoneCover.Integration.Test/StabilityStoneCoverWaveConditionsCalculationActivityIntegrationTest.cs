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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service;
using Ringtoets.Revetment.Service.TestUtil;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Service;
using Ringtoets.StabilityStoneCover.Service.Properties;

namespace Ringtoets.StabilityStoneCover.Integration.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationActivityIntegrationTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void OnRun_NoHydraulicBoundaryDatabase_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
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

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testDataPath,
                                                                                    assessmentSection.StabilityStoneCover,
                                                                                    assessmentSection);

            using (new HydraRingCalculationServiceConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        public void OnRun_InvalidHydraulicBoundaryDatabase_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            {
                FilePath = Path.Combine(testDataPath, "NonExisting.sqlite")
            };

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
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

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testDataPath,
                                                                                    assessmentSection.StabilityStoneCover,
                                                                                    assessmentSection);

            using (new HydraRingCalculationServiceConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        public void OnRun_NoHydraulicBoundaryLocation_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
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

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testDataPath,
                                                                                    assessmentSection.StabilityStoneCover,
                                                                                    assessmentSection);

            using (new HydraRingCalculationServiceConfig())
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
        public void OnRun_NoDesignWaterLevel_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
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

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testDataPath,
                                                                                    assessmentSection.StabilityStoneCover,
                                                                                    assessmentSection);

            using (new HydraRingCalculationServiceConfig())
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
                    StringAssert.StartsWith("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        [TestCase(double.NaN, 10.0)]
        [TestCase(1.0, double.NaN)]
        public void OnRun_NoWaterLevels_DoesNotPerformCalculationAndLogsError(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
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

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testDataPath,
                                                                                    assessmentSection.StabilityStoneCover,
                                                                                    assessmentSection);

            using (new HydraRingCalculationServiceConfig())
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
        public void OnRun_CalculationWithWaterLevels_PerformCalculationAndLogStartAndEnd()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation, testDataPath, assessmentSection.StabilityStoneCover, assessmentSection);

            using (new WaveConditionsCalculationServiceConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(14, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);

                    int i = 0;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        StringAssert.StartsWith(string.Format("Blokken berekening '{0}' voor waterstand '{1}' gestart om: ", calculation.Name, waterLevel), msgs[i + 1]);
                        StringAssert.StartsWith(string.Format("Blokken berekening '{0}' voor waterstand '{1}' beëindigd om: ", calculation.Name, waterLevel), msgs[i + 2]);
                        StringAssert.StartsWith(string.Format("Zuilen berekening '{0}' voor waterstand '{1}' gestart om: ", calculation.Name, waterLevel), msgs[i + 3]);
                        StringAssert.StartsWith(string.Format("Zuilen berekening '{0}' voor waterstand '{1}' beëindigd om: ", calculation.Name, waterLevel), msgs[i + 4]);

                        i = i + 4;
                    }

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[13]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
        }

        [Test]
        public void OnRun_CalculationWithValidInputConditionsCannotPerformCalculation_LogStartAndErrorAndEnd()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation, testDataPath, assessmentSection.StabilityStoneCover, assessmentSection);

            using (new HydraRingCalculationServiceConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(22, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);

                    int i = 2;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        StringAssert.StartsWith(string.Format("Blokken berekening '{0}' voor waterstand '{1}' gestart om: ", calculation.Name, waterLevel), msgs[i + 1]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' is niet gelukt.", calculation.Name, waterLevel), msgs[i + 2]);
                        StringAssert.StartsWith(string.Format("Blokken berekening '{0}' voor waterstand '{1}' beëindigd om: ", calculation.Name, waterLevel), msgs[i + 3]);
                        StringAssert.StartsWith(string.Format("Zuilen berekening '{0}' voor waterstand '{1}' gestart om: ", calculation.Name, waterLevel), msgs[i + 4]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' is niet gelukt.", calculation.Name, waterLevel), msgs[i + 5]);
                        StringAssert.StartsWith(string.Format("Zuilen berekening '{0}' voor waterstand '{1}' beëindigd om: ", calculation.Name, waterLevel), msgs[i + 6]);

                        i = i + 6;
                    }

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[21]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
        }

        [Test]
        public void OnRun_Always_SetProgressTexts()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation, testDataPath, assessmentSection.StabilityStoneCover, assessmentSection);

            using (new HydraRingCalculationServiceConfig())
            {
                List<string> progessTexts = new List<string>();
                activity.ProgressChanged += (sender, args) => { progessTexts.Add(activity.ProgressText); };

                // Call
                activity.Run();

                // Assert
                var waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                for (var i = 0; i < waterLevels.Length; i++)
                {
                    var blocksText = string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_OnRun_Calculate_blocks_waterlevel_0_, waterLevels[i]);
                    var columnsText = string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_OnRun_Calculate_columns_waterlevel_0_, waterLevels[i]);
                    Assert.AreEqual(progessTexts[i*2], blocksText);
                    Assert.AreEqual(progessTexts[i*2 + 1], columnsText);
                }
            }
        }

        [Test]
        public void OnRun_Always_InputPropertiesCorrectlySendToService()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testDataPath,
                                                                                    assessmentSection.StabilityStoneCover,
                                                                                    assessmentSection);

            using (new WaveConditionsCalculationServiceConfig())
            {
                var testService = (TestWaveConditionsCalculationService) WaveConditionsCalculationService.Instance;

                // Call
                activity.Run();

                // Assert
                TestWaveConditionsCalculationServiceInput[] testWaveConditionsInputs = testService.Inputs.ToArray();
                Assert.AreEqual(6, testWaveConditionsInputs.Length);

                int waterLevelIndex = 0;

                for (int i = 0; i < testWaveConditionsInputs.Length; i++)
                {
                    GeneralStabilityStoneCoverWaveConditionsInput generalInput = assessmentSection.StabilityStoneCover.GeneralInput;

                    if (i%2 == 0)
                    {
                        Assert.AreEqual(calculation.InputParameters.WaterLevels.ToArray()[waterLevelIndex], testWaveConditionsInputs[i].WaterLevel);
                        Assert.AreEqual(generalInput.GeneralBlocksWaveConditionsInput.A, testWaveConditionsInputs[i].A, generalInput.GeneralBlocksWaveConditionsInput.A.GetAccuracy());
                        Assert.AreEqual(generalInput.GeneralBlocksWaveConditionsInput.B, testWaveConditionsInputs[i].B, generalInput.GeneralBlocksWaveConditionsInput.B.GetAccuracy());
                        Assert.AreEqual(generalInput.GeneralBlocksWaveConditionsInput.C, testWaveConditionsInputs[i].C, generalInput.GeneralBlocksWaveConditionsInput.C.GetAccuracy());
                    }
                    else
                    {
                        Assert.AreEqual(calculation.InputParameters.WaterLevels.ToArray()[waterLevelIndex], testWaveConditionsInputs[i].WaterLevel);
                        Assert.AreEqual(generalInput.GeneralColumnsWaveConditionsInput.A, testWaveConditionsInputs[i].A, generalInput.GeneralColumnsWaveConditionsInput.A.GetAccuracy());
                        Assert.AreEqual(generalInput.GeneralColumnsWaveConditionsInput.B, testWaveConditionsInputs[i].B, generalInput.GeneralColumnsWaveConditionsInput.B.GetAccuracy());
                        Assert.AreEqual(generalInput.GeneralColumnsWaveConditionsInput.C, testWaveConditionsInputs[i].C, generalInput.GeneralColumnsWaveConditionsInput.C.GetAccuracy());

                        waterLevelIndex++;
                    }

                    Assert.AreEqual(assessmentSection.FailureMechanismContribution.Norm, testWaveConditionsInputs[i].Norm);
                    Assert.AreSame(calculation.InputParameters, testWaveConditionsInputs[i].WaveConditionsInput);
                    Assert.AreEqual(testDataPath, testWaveConditionsInputs[i].HlcdDirectory);
                    Assert.AreEqual(assessmentSection.Id, testWaveConditionsInputs[i].RingId);
                    Assert.AreEqual(calculation.Name, testWaveConditionsInputs[i].Name);
                }
            }
        }

        [Test]
        public void Cancel_WhenPerformingCalculationForBlocks_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testDataPath,
                                                                                    assessmentSection.StabilityStoneCover,
                                                                                    assessmentSection);

            using (new WaveConditionsCalculationServiceConfig())
            {
                activity.ProgressChanged += (sender, args) =>
                {
                    if (activity.State != ActivityState.Canceled && activity.ProgressText.Contains("Blokken"))
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

                    Assert.AreEqual(4, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Blokken berekening '{0}' voor waterstand '{1}' gestart om: ", calculation.Name, firstWaterLevel), msgs[1]);
                    StringAssert.StartsWith(string.Format("Blokken berekening '{0}' voor waterstand '{1}' beëindigd om: ", calculation.Name, firstWaterLevel), msgs[2]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[3]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
            }
        }

        [Test]
        public void Cancel_WhenPerformingCalculationForColumns_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testDataPath,
                                                                                    assessmentSection.StabilityStoneCover,
                                                                                    assessmentSection);

            using (new WaveConditionsCalculationServiceConfig())
            {
                activity.ProgressChanged += (sender, args) =>
                {
                    if (activity.State != ActivityState.Canceled && activity.ProgressText.Contains("Zuilen"))
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
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Blokken berekening '{0}' voor waterstand '{1}' gestart om: ", calculation.Name, firstWaterLevel), msgs[1]);
                    StringAssert.StartsWith(string.Format("Blokken berekening '{0}' voor waterstand '{1}' beëindigd om: ", calculation.Name, firstWaterLevel), msgs[2]);
                    StringAssert.StartsWith(string.Format("Zuilen berekening '{0}' voor waterstand '{1}' gestart om: ", calculation.Name, firstWaterLevel), msgs[3]);
                    StringAssert.StartsWith(string.Format("Zuilen berekening '{0}' voor waterstand '{1}' beëindigd om: ", calculation.Name, firstWaterLevel), msgs[4]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[5]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
            }
        }

        [Test]
        public void OnFinish_WhenCancelled_OutputNull()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testDataPath,
                                                                                    assessmentSection.StabilityStoneCover,
                                                                                    assessmentSection);

            using (new WaveConditionsCalculationServiceConfig())
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
        }

        [Test]
        public void OnFinish_CalculationPerformed_SetsOutput()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation, testDataPath, assessmentSection.StabilityStoneCover, assessmentSection);

            using (new WaveConditionsCalculationServiceConfig())
            {
                activity.Run();

                // Call
                activity.Finish();

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.ColumnsOutput.Count());
                Assert.AreEqual(3, calculation.Output.BlocksOutput.Count());
            }
        }

        [Test]
        public void OnFinish_CalculationFailed_OutputNull()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            ImportHydraulicBoundaryDatabase(assessmentSection);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection);

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation, testDataPath, assessmentSection.StabilityStoneCover, assessmentSection);

            using (new HydraRingCalculationServiceConfig())
            {
                activity.Run();

                // Call
                activity.Finish();

                // Assert
                Assert.IsNull(calculation.Output);
            }
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetValidCalculation(AssessmentSection assessmentSection)
        {
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
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

        private void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }
        }

        private static ForeshoreProfile CreateForeshoreProfile()
        {
            return new ForeshoreProfile(new Point2D(0, 0),
                                        new[]
                                        {
                                            new Point2D(3.3, 4.4),
                                            new Point2D(5.5, 6.6)
                                        },
                                        new BreakWater(BreakWaterType.Dam, 10.0),
                                        new ForeshoreProfile.ConstructionProperties());
        }
    }
}