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
using Application.Ringtoets.Storage.TestUtil;
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
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.Service.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationServiceTest
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
            var service = new WaveImpactAsphaltCoverWaveConditionsCalculationService();

            // Assert
            Assert.IsInstanceOf<WaveConditionsCalculationServiceBase>(service);
        }

        [Test]
        public void Run_NoHydraulicBoundaryDatabase_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => { isValid = WaveImpactAsphaltCoverWaveConditionsCalculationService.Validate(calculation, testFilePath); };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    Assert.AreEqual(string.Format("Validatie mislukt: Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.", testFilePath), msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => { isValid = WaveImpactAsphaltCoverWaveConditionsCalculationService.Validate(calculation, testFilePath); };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    Assert.AreEqual(string.Format("Validatie mislukt: Fout bij het lezen van bestand '{0}': Kon geen locaties verkrijgen van de database.", testFilePath), msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_NoHydraulicBoundaryLocation_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => { isValid = WaveImpactAsphaltCoverWaveConditionsCalculationService.Validate(calculation, validFilePath); };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        public void Validate_NoDesignWaterLevel_LogsValidationMessageAndReturnFalse()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevel = RoundedDouble.NaN;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => { isValid = WaveImpactAsphaltCoverWaveConditionsCalculationService.Validate(calculation, validFilePath); };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
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
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment;
            calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => { isValid = WaveImpactAsphaltCoverWaveConditionsCalculationService.Validate(calculation, validFilePath); };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Kan geen waterstanden afleiden op basis van de invoer. Controleer de opgegeven boven- en ondergrenzen.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
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
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => { isValid = WaveImpactAsphaltCoverWaveConditionsCalculationService.Validate(calculation, validFilePath); };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);

                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    Assert.AreEqual("Validatie mislukt: Er is geen geldige damhoogte ingevoerd.", msgs[1]);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[2]);
                });
                Assert.IsFalse(isValid);
            }
        }

        [Test]
        [TestCase(double.NegativeInfinity, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterAndInvalidBreakWaterHeight_LogAndReturnsFalse(NaN)")]
        public void Calculate_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                   breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                waveImpactAsphaltCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                           assessmentSectionStub,
                                                                                                           waveImpactAsphaltCoverFailureMechanism.GeneralInput,
                                                                                                           validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);

                    int i = 0;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie:", msgs[i + 2]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i + 3]);

                        i = i + 3;
                    }

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[7]);
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
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                waveImpactAsphaltCoverFailureMechanism, mockRepository);
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
                Action call = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                           assessmentSectionStub,
                                                                                                           waveImpactAsphaltCoverFailureMechanism.GeneralInput,
                                                                                                           validFilePath);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);

                    int i = 0;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie:", msgs[i + 2]);
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' beëindigd.", calculation.Name, waterLevel), msgs[i + 3]);

                        i = i + 3;
                    }

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[7]);
                });
            }
        }

        [Test]
        public void Calculate_Always_InputPropertiesCorrectlySendToCalculator()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                waveImpactAsphaltCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;

                // Call
                new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                       assessmentSectionStub,
                                                                                       waveImpactAsphaltCoverFailureMechanism.GeneralInput,
                                                                                       validFilePath);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = testWaveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(3, testWaveConditionsInputs.Length);

                Assert.AreEqual(testDataPath, testWaveConditionsCosineCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, testWaveConditionsCosineCalculator.RingId);

                int waterLevelIndex = 0;
                foreach (WaveConditionsCosineCalculationInput actualInput in testWaveConditionsInputs)
                {
                    GeneralWaveConditionsInput generalInput = waveImpactAsphaltCoverFailureMechanism.GeneralInput;

                    var input = calculation.InputParameters;
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 assessmentSectionStub.FailureMechanismContribution.Norm,
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height),
                                                                                 calculation.InputParameters.WaterLevels.ElementAt(waterLevelIndex++),
                                                                                 generalInput.A,
                                                                                 generalInput.B,
                                                                                 generalInput.C);

                    HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                }
            }
        }

        [Test]
        public void Calculate_Canceled_HasNoOutput()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                waveImpactAsphaltCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var waveImpactAsphaltCoverWaveConditionsCalculationService = new WaveImpactAsphaltCoverWaveConditionsCalculationService();
                waveImpactAsphaltCoverWaveConditionsCalculationService.Cancel();

                // Call
                waveImpactAsphaltCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                                 assessmentSectionStub,
                                                                                 waveImpactAsphaltCoverFailureMechanism.GeneralInput,
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
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetDefaultCalculation();
            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                waveImpactAsphaltCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                var waveImpactAsphaltCoverWaveConditionsCalculationService = new WaveImpactAsphaltCoverWaveConditionsCalculationService();
                testWaveConditionsCosineCalculator.CalculationFinishedHandler += (s, e) => waveImpactAsphaltCoverWaveConditionsCalculationService.Cancel();

                // Call
                waveImpactAsphaltCoverWaveConditionsCalculationService.Calculate(calculation,
                                                                                 assessmentSectionStub,
                                                                                 waveImpactAsphaltCoverFailureMechanism.GeneralInput,
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
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation();
            var waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                waveImpactAsphaltCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                       assessmentSectionStub,
                                                                                       waveImpactAsphaltCoverFailureMechanism.GeneralInput,
                                                                                       validFilePath);

                // Assert
                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(3, calculation.Output.Items.Count());
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_ExceptionThrown()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            var assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(failureMechanism, mockRepository);
            mockRepository.ReplayAll();

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.LastErrorContent = "An error occured";
                calculator.EndInFailure = true;

                // Call
                TestDelegate call = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                 assessmentSectionStub,
                                                                                                                 failureMechanism.GeneralInput,
                                                                                                                 testDataPath);

                // Assert
                Assert.Throws<HydraRingFileParserException>(call);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_ExceptionThrown()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = true;

                // Call
                TestDelegate call = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                 assessmentSectionStub,
                                                                                                                 failureMechanism.GeneralInput,
                                                                                                                 testDataPath);

                // Assert
                Assert.Throws<HydraRingFileParserException>(call);
            }
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                Contribution = 20
            };

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            mockRepository.ReplayAll();

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = false;
                calculator.LastErrorContent = "An error occured";

                // Call
                TestDelegate call = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                 assessmentSectionStub,
                                                                                                                 failureMechanism.GeneralInput,
                                                                                                                 testDataPath);

                // Assert
                var exception = Assert.Throws<HydraRingFileParserException>(call);
                Assert.AreEqual(calculator.LastErrorContent, exception.Message);
            }
        }

        [Test]
        public void Calculate_InnerCalculationFails_ThrowsException()
        {
            // Setup
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation();
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                waveImpactAsphaltCoverFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = true;

                // Call
                TestDelegate test = () => new WaveImpactAsphaltCoverWaveConditionsCalculationService().Calculate(calculation,
                                                                                                                 assessmentSectionStub,
                                                                                                                 waveImpactAsphaltCoverFailureMechanism.GeneralInput,
                                                                                                                 validFilePath);

                // Assert
                Assert.Throws<HydraRingFileParserException>(test);
            }
            mockRepository.VerifyAll();
        }

        private static WaveImpactAsphaltCoverWaveConditionsCalculation GetValidCalculation()
        {
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "locationName", 0, 0)
                    {
                        DesignWaterLevel = (RoundedDouble) 9.3
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

        private static WaveImpactAsphaltCoverWaveConditionsCalculation GetDefaultCalculation()
        {
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation();
            calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 5;
            calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 5.4;

            return calculation;
        }
    }
}