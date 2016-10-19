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
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Service;

namespace Ringtoets.StabilityStoneCover.Integration.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationActivityIntegrationTest
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
            var mockRepository = new MockRepository();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble)5.3,
                    UpperBoundaryRevetment = (RoundedDouble)10,
                    UpperBoundaryWaterLevels = (RoundedDouble)5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble)5
                }
            };

            var testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

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
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble)5.3,
                    UpperBoundaryRevetment = (RoundedDouble)10,
                    UpperBoundaryWaterLevels = (RoundedDouble)5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble)5
                }
            };
            var testFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    testFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

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
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_NoHydraulicBoundaryLocation_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble)5.3,
                    UpperBoundaryRevetment = (RoundedDouble)10,
                    UpperBoundaryWaterLevels = (RoundedDouble)5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble)5
                }
            };

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

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
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_NoDesignWaterLevel_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(13001, string.Empty, 0, 0),
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble)5.3,
                    UpperBoundaryRevetment = (RoundedDouble)10,
                    UpperBoundaryWaterLevels = (RoundedDouble)5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble)5
                }
            };

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

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
                    StringAssert.StartsWith("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN, 10.0)]
        [TestCase(1.0, double.NaN)]
        public void Run_NoWaterLevels_DoesNotPerformCalculationAndLogsError(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(13001, string.Empty, 0, 0)
                    {
                        DesignWaterLevel = (RoundedDouble)12.0
                    },
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble)lowerBoundaryRevetment,
                    UpperBoundaryRevetment = (RoundedDouble)upperBoundaryRevetment,
                    UpperBoundaryWaterLevels = (RoundedDouble)5.4,
                    LowerBoundaryWaterLevels = (RoundedDouble)5
                }
            };

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

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
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NaN)]
        public void Run_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                 breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(16, msgs.Length);

                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);

                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken gestart.", calculation.Name), msgs[3]);

                    int i = 4;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i++]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i++]);
                    }

                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken beëindigd.", calculation.Name), msgs[8]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen gestart.", calculation.Name), msgs[9]);

                    i = 10;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i++]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i++]);
                    }

                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen beëindigd.", calculation.Name), msgs[14]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[15]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NaN)]
        public void Run_CalculationWithForeshoreAndUsesBreakWaterAndHasInvalidBreakWaterHeight_DoesNotPerformCalculationAndLogStartEnd(double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                 breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

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
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Run_CalculationWithValidInputConditionsAndValidForeshore_LogCalculationStartAndEnd(CalculationType calculationType)
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

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

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(16, msgs.Length);

                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);

                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken gestart.", calculation.Name), msgs[3]);

                    int i = 4;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i++]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i++]);
                    }

                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken beëindigd.", calculation.Name), msgs[8]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen gestart.", calculation.Name), msgs[9]);

                    i = 10;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i++]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i++]);
                    }

                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen beëindigd.", calculation.Name), msgs[14]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[15]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_Always_SetProgressTexts()
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                List<string> progessTexts = new List<string>();
                activity.ProgressChanged += (sender, args) => { progessTexts.Add(activity.ProgressText); };

                // Call
                activity.Run();

                // Assert
                var waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                var totalSteps = waterLevels.Length * 2;
                for (var i = 0; i < totalSteps; i++)
                {
                    var text = string.Format("Stap {0} van {1} | Waterstand '{2}' berekenen.", i + 1, totalSteps, waterLevels[i % waterLevels.Length]);
                    Assert.AreEqual(text, progessTexts[i]);
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_Always_InputPropertiesCorrectlySendToService()
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSectionStub = CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository);
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    assessmentSectionStub);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;

                // Call
                activity.Run();

                // Assert
                var testWaveConditionsInputs = testWaveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(6, testWaveConditionsInputs.Length);

                GeneralStabilityStoneCoverWaveConditionsInput generalInput = stabilityStoneCoverFailureMechanism.GeneralInput;

                var input = calculation.InputParameters;

                Assert.AreEqual(testDataPath, testWaveConditionsCosineCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, testWaveConditionsCosineCalculator.RingId);

                int waterLevelIndex = 0;
                for (int i = 0; i < testWaveConditionsInputs.Length / 2; i++)
                {
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 assessmentSectionStub.FailureMechanismContribution.Norm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater((int)input.BreakWater.Type, input.BreakWater.Height),
                                                                                 input.WaterLevels.ElementAt(waterLevelIndex++),
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
                                                                                 assessmentSectionStub.FailureMechanismContribution.Norm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater((int)input.BreakWater.Type, input.BreakWater.Height),
                                                                                 input.WaterLevels.ElementAt(waterLevelIndex++),
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
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
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
                    RoundedDouble firstWaterLevel = calculation.InputParameters.WaterLevels.First();

                    Assert.AreEqual(8, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken gestart.", calculation.Name), msgs[3]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, firstWaterLevel), msgs[4]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, firstWaterLevel), msgs[5]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken beëindigd.", calculation.Name), msgs[6]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[7]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Cancel_WhenPerformingCalculationForColumns_CurrentCalculationForWaterLevelCompletesAndSubsequentCalculationsDidNotRun()
        {
            // Setup
            var mockRepository = new MockRepository();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
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
                    var waterLevels = calculation.InputParameters.WaterLevels.ToArray();

                    Assert.AreEqual(16, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken gestart.", calculation.Name), msgs[3]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevels[0]), msgs[4]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevels[0]), msgs[5]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevels[1]), msgs[6]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevels[1]), msgs[7]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevels[2]), msgs[8]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevels[2]), msgs[9]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor blokken beëindigd.", calculation.Name), msgs[10]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen gestart.", calculation.Name), msgs[11]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevels[0]), msgs[12]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevels[0]), msgs[13]);
                    Assert.AreEqual(string.Format("Berekening '{0}' voor zuilen beëindigd.", calculation.Name), msgs[14]);
                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[15]);
                });

                Assert.AreEqual(ActivityState.Canceled, activity.State);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_WhenCancelled_OutputNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
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
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
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
        public void Run_CalculationFailed_OutputNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var activity = new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                    validFilePath,
                                                                                    stabilityStoneCoverFailureMechanism,
                                                                                    CreateAssessmentSectionStub(stabilityStoneCoverFailureMechanism, mockRepository));
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = true;

                // Call
                activity.Run();

                // Assert
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        private static IAssessmentSection CreateAssessmentSectionStub(IFailureMechanism failureMechanism, MockRepository mockRepository)
        {
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Stub(a => a.Id).Return("21");
            assessmentSectionStub.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, 2));
            return assessmentSectionStub;
        }

        public enum CalculationType
        {
            NoForeshore,
            ForeshoreWithValidBreakWater,
            ForeshoreWithoutBreakWater
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetValidCalculation()
        {
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "locationName", 0, 0)
                    {
                        DesignWaterLevel = (RoundedDouble)9.3
                    },
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble)4,
                    UpperBoundaryRevetment = (RoundedDouble)10,
                    UpperBoundaryWaterLevels = (RoundedDouble)8,
                    LowerBoundaryWaterLevels = (RoundedDouble)7.1
                }
            };
            return calculation;
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetDefaultCalculation()
        {
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble)5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble)5.4;

            return calculation;
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