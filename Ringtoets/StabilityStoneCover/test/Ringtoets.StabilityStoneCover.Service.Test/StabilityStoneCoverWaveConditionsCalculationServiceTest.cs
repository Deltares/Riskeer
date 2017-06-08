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
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, testFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual($"Validatie mislukt: Fout bij het lezen van bestand '{testFilePath}': het bestand bestaat niet.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, testFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual($"Validatie mislukt: Fout bij het lezen van bestand '{testFilePath}': kon geen locaties verkrijgen van de database.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_ValidHydraulicBoundaryDatabaseWithoutSettings_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, testFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_NoDesignWaterLevel_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevelOutput = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN, 10.0)]
        [TestCase(1.0, double.NaN)]
        public void Validate_NoWaterLevels_LogsValidationMessageAndReturnFalse(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment;
            calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Validate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Validate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Validate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(NaN)")]
        public void Validate_CalculationWithForeshoreAndUsesBreakWaterAndHasInvalidBreakWaterHeight_LogsValidationMessageAndReturnFalse(double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);

                    CalculationServiceTestHelper.AssertValidationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual("Validatie mislukt: De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(calculation.Name, msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
            mockRepository.VerifyAll();
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

            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count() * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(nrOfCalculators);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                        assessmentSectionStub,
                                                                                                        stabilityStoneCoverFailureMechanism.GeneralInput,
                                                                                                        validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(18, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[0]);

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
                    Assert.AreEqual($"Berekening van '{calculation.Name}' beëindigd.", msgs[17]);
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Calculate_CalculationWithValidInputConditionsAndValidForeshore_LogStartAndEnd(CalculationType calculationType)
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();

            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count() * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(nrOfCalculators);
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

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                        assessmentSectionStub,
                                                                                                        stabilityStoneCoverFailureMechanism.GeneralInput,
                                                                                                        validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(18, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[0]);
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
                    Assert.AreEqual($"Berekening van '{calculation.Name}' beëindigd.", msgs[17]);
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

            RoundedDouble[] waterLevels = calculation.InputParameters.WaterLevels.ToArray();
            int nrOfCalculators = waterLevels.Length * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(nrOfCalculators);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var currentStep = 1;
                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                stabilityStoneCoverWaveConditionsCalculationService.OnProgress += (description, step, steps) =>
                {
                    // Assert
                    string text = $"Waterstand '{waterLevels[(step - 1) % waterLevels.Length]}' berekenen.";
                    Assert.AreEqual(text, description);
                    Assert.AreEqual(currentStep++, step);
                    Assert.AreEqual(nrOfCalculators, steps);
                };

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                              assessmentSectionStub,
                                                                              stabilityStoneCoverFailureMechanism.GeneralInput,
                                                                              validFilePath);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Always_InputPropertiesCorrectlySendToCalculator()
        {
            // Setup
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculator = new TestWaveConditionsCosineCalculator();

            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count() * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(calculator).Repeat.Times(nrOfCalculators);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new StabilityStoneCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                    assessmentSectionStub,
                                                                                    stabilityStoneCoverFailureMechanism.GeneralInput,
                                                                                    validFilePath);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(6, testWaveConditionsInputs.Length);

                GeneralStabilityStoneCoverWaveConditionsInput generalInput = stabilityStoneCoverFailureMechanism.GeneralInput;

                WaveConditionsInput input = calculation.InputParameters;

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

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                stabilityStoneCoverWaveConditionsCalculationService.Cancel();

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                              assessmentSectionStub,
                                                                              stabilityStoneCoverFailureMechanism.GeneralInput,
                                                                              validFilePath);

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
            var calculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(calculator);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var stabilityStoneCoverWaveConditionsCalculationService = new StabilityStoneCoverWaveConditionsCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => stabilityStoneCoverWaveConditionsCalculationService.Cancel();

                // Call
                stabilityStoneCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                              assessmentSectionStub, stabilityStoneCoverFailureMechanism.GeneralInput, validFilePath);

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
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var stabilityStoneCoverFailureMechanism = new StabilityStoneCoverFailureMechanism();
            int nrOfCalculators = calculation.InputParameters.WaterLevels.Count() * 2;

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(nrOfCalculators);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                stabilityStoneCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
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
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_ThreeCalculationsFail_ThrowsHydraRingCalculationExceptionAndLogError)
        })]
        public void Calculate_ThreeCalculationsFail_ThrowsHydraRingCalculationExceptionAndLogError(bool endInFailure,
                                                                                                   string lastErrorFileContent,
                                                                                                   string detailedReport)
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
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
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(calculatorThatFails).Repeat.Times(3);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

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
                        exception = e;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(16, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[1]);

                    RoundedDouble[] waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                    RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                    RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                    RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' gestart.", msgs[2]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[3]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' beëindigd.", msgs[5]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' gestart.", msgs[6]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor waterstand '{waterLevelMiddleRevetment}'. {detailedReport}", msgs[7]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[8]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' beëindigd.", msgs[9]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' gestart.", msgs[10]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor waterstand '{waterLevelLowerBoundaryRevetment}'. {detailedReport}", msgs[11]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[12]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' beëindigd.", msgs[13]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor alle waterstanden.", msgs[14]);
                    Assert.AreEqual($"Berekening van '{calculation.Name}' beëindigd.", msgs[15]);
                });
                Assert.IsInstanceOf<HydraRingCalculationException>(exception);
                Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor alle waterstanden.", exception.Message);
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_OneOutOfThreeBlocksCalculationsFails_ReturnsOutputsAndLogError)
        })]
        public void Calculate_OneOutOfThreeBlocksCalculationsFails_ReturnsOutputsAndLogError(bool endInFailure,
                                                                                             string lastErrorFileContent,
                                                                                             string detailedReport)
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
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
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(calculatorThatFails);
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(5);

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new StabilityStoneCoverWaveConditionsCalculationService();

                // Call
                Action call = () => service.Calculate(calculation,
                                                      assessmentSectionStub,
                                                      failureMechanism.GeneralInput,
                                                      validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(25, msgs.Length);

                    RoundedDouble[] waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                    RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                    RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                    RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[1]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' gestart.", msgs[2]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[3]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' beëindigd.", msgs[5]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' gestart.", msgs[6]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[7]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' beëindigd.", msgs[8]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' gestart.", msgs[9]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[10]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' beëindigd.", msgs[11]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken beëindigd.", msgs[12]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen gestart.", msgs[13]);

                    var i = 14;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i++]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i++]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i++]);
                    }

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen beëindigd.", msgs[23]);
                    Assert.AreEqual($"Berekening van '{calculation.Name}' beëindigd.", msgs[24]);
                });

                WaveConditionsOutput[] blocksWaveConditionsOutputs = calculation.Output.BlocksOutput.ToArray();
                Assert.AreEqual(3, blocksWaveConditionsOutputs.Length);
                AssertFailedCalculationOutput(blocksWaveConditionsOutputs[0]);

                WaveConditionsOutput[] columnsWaveConditionsOutputs = calculation.Output.ColumnsOutput.ToArray();
                Assert.AreEqual(3, columnsWaveConditionsOutputs.Length);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_OneOutOfThreeColumnsCalculationsFails_ReturnsOutputsAndLogError)
        })]
        public void Calculate_OneOutOfThreeColumnsCalculationsFails_ReturnsOutputsAndLogError(bool endInFailure,
                                                                                              string lastErrorFileContent,
                                                                                              string detailedReport)
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
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
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Times(3);
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(calculatorThatFails);
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Twice();

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new StabilityStoneCoverWaveConditionsCalculationService();

                // Call
                Action call = () => service.Calculate(calculation,
                                                      assessmentSectionStub,
                                                      failureMechanism.GeneralInput,
                                                      validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(25, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(calculation.Name, msgs[0]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken gestart.", msgs[1]);

                    var i = 2;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i++]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i++]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i++]);
                    }

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor blokken beëindigd.", msgs[11]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen gestart.", msgs[12]);

                    RoundedDouble[] waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                    RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                    RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                    RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' gestart.", msgs[13]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[14]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[15]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' beëindigd.", msgs[16]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' gestart.", msgs[17]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[18]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' beëindigd.", msgs[19]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' gestart.", msgs[20]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[21]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' beëindigd.", msgs[22]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor zuilen beëindigd.", msgs[23]);
                    Assert.AreEqual($"Berekening van '{calculation.Name}' beëindigd.", msgs[24]);
                });

                WaveConditionsOutput[] blocksWaveConditionsOutputs = calculation.Output.BlocksOutput.ToArray();
                Assert.AreEqual(3, blocksWaveConditionsOutputs.Length);

                WaveConditionsOutput[] columnsWaveConditionsOutputs = calculation.Output.ColumnsOutput.ToArray();
                Assert.AreEqual(3, columnsWaveConditionsOutputs.Length);
                AssertFailedCalculationOutput(columnsWaveConditionsOutputs[0]);
            }
            mockRepository.VerifyAll();
        }

        private static void AssertFailedCalculationOutput(WaveConditionsOutput actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(RoundedDouble.NaN, actual.WaterLevel);
            Assert.AreEqual(RoundedDouble.NaN, actual.WaveHeight);
            Assert.AreEqual(RoundedDouble.NaN, actual.WavePeakPeriod);
            Assert.AreEqual(RoundedDouble.NaN, actual.WaveAngle);
            Assert.AreEqual(RoundedDouble.NaN, actual.WaveDirection);
            Assert.AreEqual(double.NaN, actual.TargetProbability);
            Assert.AreEqual(RoundedDouble.NaN, actual.TargetReliability);
            Assert.AreEqual(double.NaN, actual.CalculatedProbability);
            Assert.AreEqual(RoundedDouble.NaN, actual.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, actual.CalculationConvergence);
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