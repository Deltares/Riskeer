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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, testFilePath);

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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, testFilePath);

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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, testFilePath);

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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);

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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevelOutput = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Kan de waterstand bij doorsnede-eis niet afleiden op basis van de invoer.", msgs[1]);
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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment;
            calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);

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
        [TestCase(double.NegativeInfinity, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(NaN)")]
        public void Validate_CalculationWithForeshoreAndUsesBreakWaterAndHasInvalidBreakWaterHeight_LogsValidationMessageAndReturnFalse(double breakWaterHeight)
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);

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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(null,
                                                                                                                failureMechanism,
                                                                                                                assessmentSectionStub,
                                                                                                                validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_FailureMechniamsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            // Call
            TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                null,
                                                                                                                assessmentSectionStub,
                                                                                                                validFilePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
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

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);

                    var i = 0;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i + 2]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i + 3]);

                        i = i + 3;
                    }

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[7]);
                });
            }
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

            using (new HydraRingCalculatorFactoryConfig())
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

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);

                    var i = 0;
                    foreach (RoundedDouble waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[i + 2]);
                        Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[i + 3]);

                        i = i + 3;
                    }

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[7]);
                });
            }
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

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;

                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                          grassCoverErosionOutwardsFailureMechanism,
                                                                                          assessmentSectionStub,
                                                                                          validFilePath);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = testWaveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(3, testWaveConditionsInputs.Length);

                Assert.AreEqual(testDataPath, testWaveConditionsCosineCalculator.HydraulicBoundaryDatabaseDirectory);

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
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
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

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                var grassCoverErosionOutwardsWaveConditionsCalculationService = new GrassCoverErosionOutwardsWaveConditionsCalculationService();
                testWaveConditionsCosineCalculator.CalculationFinishedHandler += (s, e) => grassCoverErosionOutwardsWaveConditionsCalculationService.Cancel();

                // Call
                grassCoverErosionOutwardsWaveConditionsCalculationService.Calculate(calculation,
                                                                                    grassCoverErosionOutwardsFailureMechanism,
                                                                                    assessmentSectionStub,
                                                                                    validFilePath);

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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
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
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();

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
                        new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                  failureMechanism,
                                                                                                  assessmentSectionStub,
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
                    Assert.AreEqual(6, msgs.Length);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);

                    RoundedDouble waterLevel = calculation.InputParameters.WaterLevels.First();

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[1]);
                    StringAssert.StartsWith($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[2]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[4]);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[5]);
                });
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(exception);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();

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
                        new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                  failureMechanism,
                                                                                                  assessmentSectionStub,
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
                    Assert.AreEqual(6, msgs.Length);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);

                    RoundedDouble waterLevel = calculation.InputParameters.WaterLevels.First();

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[1]);
                    StringAssert.StartsWith($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' is niet gelukt. Er is geen foutrapport beschikbaar.", msgs[2]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[4]);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[5]);
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();

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
                        new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                  failureMechanism,
                                                                                                  assessmentSectionStub,
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
                    Assert.AreEqual(6, msgs.Length);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[0]);

                    RoundedDouble waterLevel = calculation.InputParameters.WaterLevels.First();

                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' gestart.", msgs[1]);
                    StringAssert.StartsWith($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' is niet gelukt. Bekijk het foutrapport door op details te klikken.", msgs[2]);
                    StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                    Assert.AreEqual($"Berekening '{calculation.Name}' voor waterstand '{waterLevel}' beëindigd.", msgs[4]);

                    StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[5]);
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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(grassCoverErosionOutwardsFailureMechanism,
                                                                                                                                  mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                TestWaveConditionsCosineCalculator calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = true;

                // Call
                TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                    grassCoverErosionOutwardsFailureMechanism,
                                                                                                                    assessmentSectionStub,
                                                                                                                    validFilePath);

                // Assert
                Assert.Throws<HydraRingCalculationException>(test);
            }
            mockRepository.VerifyAll();
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