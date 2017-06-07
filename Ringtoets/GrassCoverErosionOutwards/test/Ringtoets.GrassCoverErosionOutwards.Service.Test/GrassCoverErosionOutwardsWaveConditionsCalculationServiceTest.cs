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
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationServiceTest
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
            var service = new GrassCoverErosionOutwardsWaveConditionsCalculationService();

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationServiceBase>(service);
        }

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(null, validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Validate_NoHydraulicBoundaryDatabase_LogValidationMessageAndReturnFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, testFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' gestart.", msgs[0]);
                    Assert.AreEqual($"Validatie mislukt: Fout bij het lezen van bestand '{testFilePath}': het bestand bestaat niet.", msgs[1]);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' beëindigd.", msgs[2]);
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

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, testFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' gestart.", msgs[0]);
                    Assert.AreEqual($"Validatie mislukt: Fout bij het lezen van bestand '{testFilePath}': kon geen locaties verkrijgen van de database.", msgs[1]);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' beëindigd.", msgs[2]);
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

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, testFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' gestart.", msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' beëindigd.", msgs[2]);
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

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' gestart.", msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' beëindigd.", msgs[2]);
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

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevelOutput = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' gestart.", msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Kan de waterstand bij doorsnede-eis niet afleiden op basis van de invoer.", msgs[1]);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' beëindigd.", msgs[2]);
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

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment;
            calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' gestart.", msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' beëindigd.", msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(NaN)")]
        public void Validate_CalculationWithForeshoreAndUsesBreakWaterAndHasInvalidBreakWaterHeight_LogsValidationMessageAndReturnFalse(double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);

                    Assert.AreEqual($"Validatie van '{calculation.Name}' gestart.", msgs[0]);
                    Assert.AreEqual("Validatie mislukt: De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                    Assert.AreEqual($"Validatie van '{calculation.Name}' beëindigd.", msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(null,
                                                                                                                failureMechanism,
                                                                                                                assessmentSectionStub,
                                                                                                                validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_FailureMechniamsNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                null,
                                                                                                                assessmentSectionStub,
                                                                                                                validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                failureMechanism,
                                                                                                                null,
                                                                                                                validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterNoInvalidBreakWaterHeight_PerformAndLog(NaN)")]
        public void Calculate_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Twice();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                              grassCoverErosionOutwardsFailureMechanism,
                                                                                                              assessmentSectionStub,
                                                                                                              validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    Assert.AreEqual($"Berekening van '{calculation.Name}' gestart.", msgs[0]);

                    var i = 0;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i + 2]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i + 3]);

                        i = i + 3;
                    }

                    Assert.AreEqual($"Berekening van '{calculation.Name}' beëindigd.", msgs[7]);
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        public void Run_CalculationWithValidInputAndValidForeshore_LogStartAndEnd(CalculationType calculationType)
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Twice();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
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
                Action call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                              grassCoverErosionOutwardsFailureMechanism,
                                                                                                              assessmentSectionStub,
                                                                                                              validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    Assert.AreEqual($"Berekening van '{calculation.Name}' gestart.", msgs[0]);

                    var i = 0;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i + 2]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i + 3]);

                        i = i + 3;
                    }

                    Assert.AreEqual($"Berekening van '{calculation.Name}' beëindigd.", msgs[7]);
                });
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_Always_InputPropertiesCorrectlySendToCalculator()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };
            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(waveConditionsCosineCalculator).Repeat.Times(3);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          grassCoverErosionOutwardsFailureMechanism,
                                                                                          assessmentSectionStub,
                                                                                          validFilePath);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = waveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(3, testWaveConditionsInputs.Length);

                var waterLevelIndex = 0;
                foreach (WaveConditionsCosineCalculationInput actualInput in testWaveConditionsInputs)
                {
                    GeneralGrassCoverErosionOutwardsInput generalInput = grassCoverErosionOutwardsFailureMechanism.GeneralInput;

                    double mechanismSpecificNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                        assessmentSectionStub.FailureMechanismContribution.Norm,
                        grassCoverErosionOutwardsFailureMechanism.Contribution,
                        grassCoverErosionOutwardsFailureMechanism.GeneralInput.N);

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
        public void Calculate_Canceled_HasNoOutput()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                grassCoverErosionOutwardsFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var grassCoverErosionOutwardsWaveConditionsCalculationService = new GrassCoverErosionOutwardsWaveConditionsCalculationService();
                grassCoverErosionOutwardsWaveConditionsCalculationService.Cancel();

                // Call
                grassCoverErosionOutwardsWaveConditionsCalculationService.Calculate(calculation,
                                                                                    grassCoverErosionOutwardsFailureMechanism,
                                                                                    assessmentSectionStub,
                                                                                    validFilePath);

                // Assert
                Assert.IsFalse(calculation.HasOutput);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CanceledCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };
            var waveConditionsCosineCalculator = new TestWaveConditionsCosineCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(waveConditionsCosineCalculator);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var grassCoverErosionOutwardsWaveConditionsCalculationService = new GrassCoverErosionOutwardsWaveConditionsCalculationService();
                waveConditionsCosineCalculator.CalculationFinishedHandler += (s, e) => grassCoverErosionOutwardsWaveConditionsCalculationService.Cancel();

                // Call
                grassCoverErosionOutwardsWaveConditionsCalculationService.Calculate(calculation,
                                                                                    grassCoverErosionOutwardsFailureMechanism,
                                                                                    assessmentSectionStub,
                                                                                    validFilePath);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(waveConditionsCosineCalculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_WithValidInput_SetsOutput()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(3);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          grassCoverErosionOutwardsFailureMechanism,
                                                                                          assessmentSectionStub,
                                                                                          validFilePath);

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.Items.Count());
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(WaveConditionsCosineCalculatorTestHelper), nameof(WaveConditionsCosineCalculatorTestHelper.FailingWaveConditionsCosineCalculators))]
        public void Calculate_ThreeCalculationsFail_ThrowsHydraRingCalculationExceptionAndLogError(TestWaveConditionsCosineCalculator calculatorThatFails,
                                                                                                   string detailedReport)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(calculatorThatFails)
                             .Repeat
                             .Times(3);
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                  failureMechanism,
                                                                                                  assessmentSectionStub,
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
                    Assert.AreEqual(15, msgs.Length);

                    Assert.AreEqual($"Berekening van '{calculation.Name}' gestart.", msgs[0]);

                    RoundedDouble[] waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                    RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                    RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                    RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' gestart.", msgs[1]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[2]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' beëindigd.", msgs[4]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' gestart.", msgs[5]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor waterstand '{waterLevelMiddleRevetment}'. {detailedReport}", msgs[6]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[7]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' beëindigd.", msgs[8]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' gestart.", msgs[9]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor waterstand '{waterLevelLowerBoundaryRevetment}'. {detailedReport}", msgs[10]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[11]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' beëindigd.", msgs[12]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor alle waterstanden.", msgs[13]);
                    Assert.AreEqual($"Berekening van '{calculation.Name}' beëindigd.", msgs[14]);
                });
                Assert.IsInstanceOf<HydraRingCalculationException>(exception);
                Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor alle waterstanden.", exception.Message);
                Assert.IsNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(WaveConditionsCosineCalculatorTestHelper), nameof(WaveConditionsCosineCalculatorTestHelper.FailingWaveConditionsCosineCalculators))]
        public void Calculate_OneOutOfThreeCalculationsFails_ReturnsOutputsAndLogError(TestWaveConditionsCosineCalculator calculatorThatFails,
                                                                                       string detailedReport)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(calculatorThatFails);
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath))
                             .Return(new TestWaveConditionsCosineCalculator())
                             .Repeat
                             .Twice();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () =>
                {
                    new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                              failureMechanism,
                                                                                              assessmentSectionStub,
                                                                                              validFilePath);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(12, msgs.Length);

                    Assert.AreEqual($"Berekening van '{calculation.Name}' gestart.", msgs[0]);

                    RoundedDouble[] waterLevels = calculation.InputParameters.WaterLevels.ToArray();
                    RoundedDouble waterLevelUpperBoundaryRevetment = waterLevels[0];
                    RoundedDouble waterLevelMiddleRevetment = waterLevels[1];
                    RoundedDouble waterLevelLowerBoundaryRevetment = waterLevels[2];

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' gestart.", msgs[1]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' is mislukt voor waterstand '{waterLevelUpperBoundaryRevetment}'. {detailedReport}", msgs[2]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelUpperBoundaryRevetment}' beëindigd.", msgs[4]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' gestart.", msgs[5]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[6]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelMiddleRevetment}' beëindigd.", msgs[7]);

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' gestart.", msgs[8]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[9]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevelLowerBoundaryRevetment}' beëindigd.", msgs[10]);

                    Assert.AreEqual($"Berekening van '{calculation.Name}' beëindigd.", msgs[11]);
                });

                WaveConditionsOutput[] waveConditionsOutputs = calculation.Output.Items.ToArray();
                Assert.AreEqual(3, waveConditionsOutputs.Length);
                AssertFailedCalculationOutput(waveConditionsOutputs[0]);
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

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation()
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "locationName", 0, 0)
                    {
                        DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(9.3)
                    },
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
            return calculation;
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetDefaultCalculation()
        {
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 5.4;

            return calculation;
        }
    }
}