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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Service.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationServiceTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private string validFilePath;

        [SetUp]
        public void SetUp()
        {
            validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        }

        [Test]
        public void Constructor_ReturnsNewInstance()
        {
            // Call
            var service = new StabilityStoneCoverWaveConditionsCalculationService();

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationServiceBase>(service);
        }

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => StabilityStoneCoverWaveConditionsCalculationService.Validate(null, validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryDatabase_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();

            string testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, testFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual($"Validatie mislukt: Fout bij het lezen van bestand '{testFilePath}': het bestand bestaat niet.", msgs[1]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, testFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual($"Validatie mislukt: Fout bij het lezen van bestand '{testFilePath}': kon geen locaties verkrijgen van de database.", msgs[1]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, testFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_NoDesignWaterLevel_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevelOutput = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        [TestCase(double.NaN, 10.0)]
        [TestCase(1.0, double.NaN)]
        public void Validate_NoWaterLevels_LogsValidationMessageAndReturnFalse(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment;
            calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Validate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Validate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Validate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(NaN)")]
        public void Validate_CalculationWithForeshoreAndUsesBreakWaterAndHasInvalidBreakWaterHeight_LogsValidationMessageAndReturnFalse(double breakWaterHeight)
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);

                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual("Validatie mislukt: De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(null,
                                                                                                          assessmentSectionStub,
                                                                                                          failureMechanism.GeneralInput,
                                                                                                          validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                          null,
                                                                                                          failureMechanism.GeneralInput,
                                                                                                          validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Calculate_GeneralInpuNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();

            // Call
            TestDelegate test = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                          assessmentSectionStub,
                                                                                                          null,
                                                                                                          validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalWaveConditionsInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Calculate_CalculationWithForsShoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(NaN)")]
        public void Calculate_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                        assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(18, msgs.Length);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[1]);

                    var i = 2;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i++]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i++]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i++]);
                    }

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken beëindigd.", msgs[8]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen gestart.", msgs[9]);

                    i = 10;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i++]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i++]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i++]);
                    }

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen beëindigd.", msgs[16]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[17]);
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Calculate_CalculationWithValidInputConditionsAndValidForeshore_LogCalculationStartAndEnd(CalculationType calculationType)
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
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

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                        assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(18, msgs.Length);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[1]);

                    var i = 2;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i++]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i++]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i++]);
                    }

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken beëindigd.", msgs[8]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen gestart.", msgs[9]);

                    i = 10;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i++]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i++]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i++]);
                    }

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen beëindigd.", msgs[16]);
                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[17]);
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Always_SendsProgressNotifications()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                RoundedDouble[] waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                var currentStep = 1;
                int totalSteps = waterLevels.Length * 2;

                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                stabilityStoneCoverWaveConditionsCalculationService.OnProgress += (description, step, steps) =>
                {
                    // Assert
                    string text = $"Waterstand '{waterLevels[(step - 1) % waterLevels.Length]}' berekenen.";
                    Assert.AreEqual(text, description);
                    Assert.AreEqual(currentStep++, step);
                    Assert.AreEqual(totalSteps, steps);
                };

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                              assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Always_InputPropertiesCorrectlySendToCalculator()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;

                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                    assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = testWaveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(6, testWaveConditionsInputs.Length);

                GeneralStabilityStoneCoverWaveConditionsInput generalInput = stabilityStoneCoverFailureMechanism.GeneralInput;

                WaveConditionsInput input = calculation.InputParameters;

                Assert.AreEqual(testDataPath, testWaveConditionsCosineCalculator.HydraulicBoundaryDatabaseDirectory);

                var waterLevelIndex = 0;
                for (var i = 0; i < testWaveConditionsInputs.Length / 2; i++)
                {
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 assessmentSectionStub.FailureMechanismContribution.Norm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
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
                                                                                 new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
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
        public void Calculate_Canceled_HasNoOutput()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                stabilityStoneCoverWaveConditionsCalculationService.Cancel();

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                              assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                Assert.IsFalse(calculation.HasOutput);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                testWaveConditionsCosineCalculator.CalculationFinishedHandler += (s, e) => stabilityStoneCoverWaveConditionsCalculationService.Cancel();

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                              assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(testWaveConditionsCosineCalculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_WithValidInput_SetsOutput()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                    assessmentSectionStub,
                                                                                    stabilityStoneCoverFailureMechanism.GeneralInput,
                                                                                    validFilePath);

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.ColumnsOutput.Count());
                Assert.AreEqual(3, calculation.Output.BlocksOutput.Count());
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.LastErrorFileContent = "An error occurred";
                calculator.EndInFailure = true;

                var exception = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                            assessmentSectionStub,
                                                                                            failureMechanism.GeneralInput,
                                                                                            validFilePath);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exception = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[1]);

                    RoundedDouble waterLevel = calculation.InputParameters.WaterLevels.First();

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[2]);
                    StringAssert.StartsWith($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[3]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[5]);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
                });
                Assert.IsTrue(exception);
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = true;

                var exception = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                            assessmentSectionStub,
                                                                                            failureMechanism.GeneralInput,
                                                                                            validFilePath);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exception = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[1]);

                    RoundedDouble waterLevel = calculation.InputParameters.WaterLevels.First();

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[2]);
                    StringAssert.StartsWith($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[3]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[5]);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
                });
                Assert.IsTrue(exception);
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorFileContent = "An error occurred";

                var exception = false;
                string exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                            assessmentSectionStub,
                                                                                            failureMechanism.GeneralInput,
                                                                                            validFilePath);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exception = true;
                        exceptionMessage = e.Message;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[1]);

                    RoundedDouble waterLevel = calculation.InputParameters.WaterLevels.First();

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[2]);
                    StringAssert.StartsWith($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[3]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[5]);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[6]);
                });
                Assert.IsTrue(exception);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_InnerCalculationFails_ThrowsException()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = true;

                // Call
                TestDelegate test = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                              assessmentSectionStub,
                                                                                                              stabilityStoneCoverFailureMechanism.GeneralInput,
                                                                                                              validFilePath);

                // Assert
                Assert.Throws<HydraRingCalculationException>(test);
            }
            mockRepository.VerifyAll();
        }

        public enum CalculationType
        {
            NoForeshore,
            ForeshoreWithValidBreakWater,
            ForeshoreWithoutBreakWater
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetValidCalculation()
        {
            return new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(9.3),
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

        private static StabilityStoneCoverWaveConditionsCalculation GetDefaultCalculation()
        {
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 5.4;

            return calculation;
        }
    }
}