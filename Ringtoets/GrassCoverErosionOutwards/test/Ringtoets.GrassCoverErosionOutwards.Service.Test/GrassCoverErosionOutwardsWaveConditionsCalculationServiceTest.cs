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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;
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
        public void Run_NoHydraulicBoundaryDatabase_DoesNotPerformCalculationAndLogsError()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "NonExisting.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, testFilePath);
                };

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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            string testFilePath = Path.Combine(testDataPath, "corruptschema.sqlite");

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, testFilePath);
                };

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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation = null;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);
                };

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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevel = RoundedDouble.NaN;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                    StringAssert.StartsWith("Validatie mislukt: Kan de waterstand bij doorsnede-eis niet afleiden op basis van de invoer.", msgs[1]);
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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble)lowerBoundaryRevetment;
            calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble)upperBoundaryRevetment;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);
                };

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
        [TestCase(double.NegativeInfinity, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Validate_CalculationWithForeshoreBreakWaterAndInvalidBreakWaterHeight_LogAndReturnFalse(NaN)")]
        public void Validate_CalculationWithForeshoreAndUsesBreakWaterAndHasInvalidBreakWaterHeight_LogsValidationMessageAndReturnFalse(double breakWaterHeight)
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                 breakWaterHeight));
            calculation.InputParameters.UseBreakWater = true;

            var isValid = true;

            using (new HydraRingCalculatorFactoryConfig())
            {
                // Call
                Action call = () =>
                {
                    isValid = GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, validFilePath);
                };

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
        [TestCase(double.NegativeInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(negativeInfinity)")]
        [TestCase(double.PositiveInfinity, TestName = "Calculate_CalculationWithForeshoreNoBreakWaterAndInvalidBreakWaterHeight_PerformAndLog(positiveInfinity)")]
        [TestCase(double.NaN, TestName = "Calculate_CalculationWithForeshoreAndBreakWaterNoInvalidBreakWaterHeight_PerformAndLog(NaN)")]
        public void Calculate_CalculationWithForeshoreAndDoesNotUseBreakWaterAndHasInvalidBreakWaterHeight_PerformCalculationAndLogStartAndEnd(double breakWaterHeight)
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            calculation.InputParameters.ForeshoreProfile = CreateForeshoreProfile(new BreakWater(BreakWaterType.Dam,
                                                                                                 breakWaterHeight));
            calculation.InputParameters.UseBreakWater = false;
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(grassCoverErosionOutwardsFailureMechanism, mockRepository);
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
                    var msgs = messages.ToArray();
                    Assert.AreEqual(8, msgs.Length);

                    StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[0]);

                    int i = 0;
                    foreach (var waterLevel in calculation.InputParameters.WaterLevels)
                    {
                        Assert.AreEqual(string.Format("Berekening '{0}' voor waterstand '{1}' gestart.", calculation.Name, waterLevel), msgs[i + 1]);
                        StringAssert.StartsWith("Golfcondities berekeningsverslag. Klik op details voor meer informatie.", msgs[i + 2]);
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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetDefaultCalculation();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(grassCoverErosionOutwardsFailureMechanism, mockRepository);
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
                Action call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                            grassCoverErosionOutwardsFailureMechanism,
                                                                            assessmentSectionStub,
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
                        StringAssert.StartsWith("Golfcondities berekeningsverslag. Klik op details voor meer informatie.", msgs[i + 2]);
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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(grassCoverErosionOutwardsFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;

                // Call
                new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                            grassCoverErosionOutwardsFailureMechanism,
                                                                            assessmentSectionStub,
                                                                            validFilePath);

                // Assert
                WaveConditionsCosineCalculationInput[] testWaveConditionsInputs = testWaveConditionsCosineCalculator.ReceivedInputs.ToArray();
                Assert.AreEqual(3, testWaveConditionsInputs.Length);

                Assert.AreEqual(testDataPath, testWaveConditionsCosineCalculator.HydraulicBoundaryDatabaseDirectory);
                Assert.AreEqual(assessmentSectionStub.Id, testWaveConditionsCosineCalculator.RingId);

                int waterLevelIndex = 0;
                foreach (WaveConditionsCosineCalculationInput actualInput in testWaveConditionsInputs)
                {

                    GeneralGrassCoverErosionOutwardsInput generalInput = grassCoverErosionOutwardsFailureMechanism.GeneralInput;

                    var input = calculation.InputParameters;
                    var expectedInput = new WaveConditionsCosineCalculationInput(1,
                                                                                 input.Orientation,
                                                                                 input.HydraulicBoundaryLocation.Id,
                                                                                 grassCoverErosionOutwardsFailureMechanism.CalculationBeta(assessmentSectionStub),
                                                                                 input.ForeshoreProfile.Geometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                                                                                 new HydraRingBreakWater((int)input.BreakWater.Type, input.BreakWater.Height),
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
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(grassCoverErosionOutwardsFailureMechanism, mockRepository);
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
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(grassCoverErosionOutwardsFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var testWaveConditionsCosineCalculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
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
            var grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(grassCoverErosionOutwardsFailureMechanism, mockRepository);
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
        public void Calculate_InnerCalculationFails_ThrowsException()
        {
            // Setup
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var mockRepository = new MockRepository();
            var assessmentSectionStub = CreateAssessmentSectionStub(grassCoverErosionOutwardsFailureMechanism, mockRepository);
            mockRepository.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig())
            {
                var calculator = ((TestHydraRingCalculatorFactory)HydraRingCalculatorFactory.Instance).WaveConditionsCosineCalculator;
                calculator.EndInFailure = true;

                // Call
                TestDelegate test = () => new GrassCoverErosionOutwardsWaveConditionsCalculationService().Calculate(calculation,
                                                                                        grassCoverErosionOutwardsFailureMechanism,
                                                                                        assessmentSectionStub,
                                                                                        validFilePath);

                // Assert
                Assert.Throws<HydraRingFileParserException>(test);
            }
            mockRepository.VerifyAll();
        }

        public enum CalculationType
        {
            NoForeshore,
            ForeshoreWithValidBreakWater,
            ForeshoreWithoutBreakWater
        }

        private static IAssessmentSection CreateAssessmentSectionStub(IFailureMechanism failureMechanism, MockRepository mockRepository)
        {
            failureMechanism.Contribution = 20;
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Stub(a => a.Id).Return("21");
            assessmentSectionStub.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, 2));
            return assessmentSectionStub;
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation()
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
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

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetDefaultCalculation()
        {
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();
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