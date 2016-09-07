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
using Ringtoets.HydraRing.Calculation.TestUtil;
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
        public void OnRun_NoWaterLevels_LogStartAndEnd()
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

            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation, testDataPath, assessmentSection.StabilityStoneCover, assessmentSection);

            using (new WaveConditionsCalculationServiceConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(2, msgs.Length);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
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
        public void OnRun_CalculationWithWaterLevelsCannotPerformCalculation_LogStartAndErrorAndEnd()
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
                foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                {
                    var blocksText = string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_OnRun_Calculate_blocks_waterlevel_0_, waterLevel);
                    var columnsText = string.Format(Resources.StabilityStoneCoverWaveConditionsCalculationActivity_OnRun_Calculate_columns_waterlevel_0_, waterLevel);
                    CollectionAssert.Contains(progessTexts, blocksText);
                    CollectionAssert.Contains(progessTexts, columnsText);
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
                    GeneralStabilityStoneCoverWaveConditionsInput generalWaveConditionsInput = assessmentSection.StabilityStoneCover.GeneralInput;

                    if (i%2 == 0)
                    {
                        Assert.AreEqual(calculation.InputParameters.WaterLevels.ToArray()[waterLevelIndex], testWaveConditionsInputs[i].WaterLevel);
                        Assert.AreEqual(generalWaveConditionsInput.GeneralBlocksWaveConditionsInput.A, testWaveConditionsInputs[i].A);
                        Assert.AreEqual(generalWaveConditionsInput.GeneralBlocksWaveConditionsInput.B, testWaveConditionsInputs[i].B);
                        Assert.AreEqual(generalWaveConditionsInput.GeneralBlocksWaveConditionsInput.C, testWaveConditionsInputs[i].C);
                    }
                    else
                    {
                        Assert.AreEqual(calculation.InputParameters.WaterLevels.ToArray()[waterLevelIndex], testWaveConditionsInputs[i].WaterLevel);
                        Assert.AreEqual(generalWaveConditionsInput.GeneralColumnsWaveConditionsInput.A, testWaveConditionsInputs[i].A);
                        Assert.AreEqual(generalWaveConditionsInput.GeneralColumnsWaveConditionsInput.B, testWaveConditionsInputs[i].B);
                        Assert.AreEqual(generalWaveConditionsInput.GeneralColumnsWaveConditionsInput.C, testWaveConditionsInputs[i].C);

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