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
using System.ComponentModel;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.Structures;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.ClosingStructures.Service.Test
{
    [TestFixture]
    public class ClosingStructuresCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var service = new ClosingStructuresCalculationService();

            // Assert
            Assert.IsInstanceOf<StructuresCalculationServiceBase<ClosingStructuresValidationRulesRegistry, ClosingStructuresInput,
                ClosingStructure, GeneralClosingStructuresInput, StructuresClosureCalculationInput>>(service);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_InvalidVerticalWallCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           Path.Combine(testDataPath, "HRD dutch coast south.sqlite"));
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation();

            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            var isValid = false;

            // Call 
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(20, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stormDuration}' moet een positief getal zijn.", msgs[1]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stormDuration}' moet groter zijn dan of gelijk zijn aan 0.", msgs[2]);
                Assert.AreEqual($"De verwachtingswaarde voor '{modelFactorSuperCriticalFlow}' moet een concreet getal zijn.", msgs[3]);
                Assert.AreEqual($"De standaardafwijking voor '{modelFactorSuperCriticalFlow}' moet groter zijn dan of gelijk zijn aan 0.", msgs[4]);
                Assert.AreEqual($"De waarde voor '{factorStormDurationOpenStructure}' moet een concreet getal zijn.", msgs[5]);
                Assert.AreEqual($"De verwachtingswaarde voor '{widthFlowApertures}' moet een concreet getal zijn.", msgs[6]);
                Assert.AreEqual($"De standaardafwijking voor '{widthFlowApertures}' moet groter zijn dan of gelijk zijn aan 0.", msgs[7]);
                Assert.AreEqual($"De waarde voor '{structureNormalOrientation}' moet een concreet getal zijn.", msgs[8]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowWidthAtBottomProtection}' moet een positief getal zijn.", msgs[9]);
                Assert.AreEqual($"De standaardafwijking voor '{flowWidthAtBottomProtection}' moet groter zijn dan of gelijk zijn aan 0.", msgs[10]);
                Assert.AreEqual($"De verwachtingswaarde voor '{storageStructureArea}' moet een positief getal zijn.", msgs[11]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{storageStructureArea}' moet groter zijn dan of gelijk zijn aan 0.", msgs[12]);
                Assert.AreEqual($"De verwachtingswaarde voor '{allowedLevelIncreaseStorage}' moet een positief getal zijn.", msgs[13]);
                Assert.AreEqual($"De standaardafwijking voor '{allowedLevelIncreaseStorage}' moet groter zijn dan of gelijk zijn aan 0.", msgs[14]);
                Assert.AreEqual($"De verwachtingswaarde voor '{levelCrestStructureNotClosing}' moet een concreet getal zijn.", msgs[15]);
                Assert.AreEqual($"De standaardafwijking voor '{levelCrestStructureNotClosing}' moet groter zijn dan of gelijk zijn aan 0.", msgs[16]);
                Assert.AreEqual($"De verwachtingswaarde voor '{criticalOvertoppingDischarge}' moet een positief getal zijn.", msgs[17]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{criticalOvertoppingDischarge}' moet groter zijn dan of gelijk zijn aan 0.", msgs[18]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[19]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_InvalidLowSillCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           Path.Combine(testDataPath, "HRD dutch coast south.sqlite"));
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = ClosingStructureInflowModelType.LowSill
                }
            };
            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            var isValid = false;

            // Call 
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(21, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stormDuration}' moet een positief getal zijn.", msgs[1]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stormDuration}' moet groter zijn dan of gelijk zijn aan 0.", msgs[2]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevel}' moet een concreet getal zijn.", msgs[3]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevel}' moet groter zijn dan of gelijk zijn aan 0.", msgs[4]);
                Assert.AreEqual($"De verwachtingswaarde voor '{modelFactorSuperCriticalFlow}' moet een concreet getal zijn.", msgs[5]);
                Assert.AreEqual($"De standaardafwijking voor '{modelFactorSuperCriticalFlow}' moet groter zijn dan of gelijk zijn aan 0.", msgs[6]);
                Assert.AreEqual($"De waarde voor '{factorStormDurationOpenStructure}' moet een concreet getal zijn.", msgs[7]);
                Assert.AreEqual($"De verwachtingswaarde voor '{widthFlowApertures}' moet een concreet getal zijn.", msgs[8]);
                Assert.AreEqual($"De standaardafwijking voor '{widthFlowApertures}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowWidthAtBottomProtection}' moet een positief getal zijn.", msgs[10]);
                Assert.AreEqual($"De standaardafwijking voor '{flowWidthAtBottomProtection}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                Assert.AreEqual($"De verwachtingswaarde voor '{storageStructureArea}' moet een positief getal zijn.", msgs[12]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{storageStructureArea}' moet groter zijn dan of gelijk zijn aan 0.", msgs[13]);
                Assert.AreEqual($"De verwachtingswaarde voor '{allowedLevelIncreaseStorage}' moet een positief getal zijn.", msgs[14]);
                Assert.AreEqual($"De standaardafwijking voor '{allowedLevelIncreaseStorage}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                Assert.AreEqual($"De verwachtingswaarde voor '{thresholdHeightOpenWeir}' moet een concreet getal zijn.", msgs[16]);
                Assert.AreEqual($"De standaardafwijking voor '{thresholdHeightOpenWeir}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                Assert.AreEqual($"De verwachtingswaarde voor '{criticalOvertoppingDischarge}' moet een positief getal zijn.", msgs[18]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{criticalOvertoppingDischarge}' moet groter zijn dan of gelijk zijn aan 0.", msgs[19]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[20]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Validate_InvalidFloodedCulvertCalculation_LogsErrorAndReturnsFalse(double value)
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           Path.Combine(testDataPath, "HRD dutch coast south.sqlite"));
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = ClosingStructureInflowModelType.FloodedCulvert
                }
            };
            SetInvalidInputParameters(calculation.InputParameters, (RoundedDouble) value);

            var isValid = false;

            // Call 
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(19, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual($"De verwachtingswaarde voor '{stormDuration}' moet een positief getal zijn.", msgs[1]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{stormDuration}' moet groter zijn dan of gelijk zijn aan 0.", msgs[2]);
                Assert.AreEqual($"De verwachtingswaarde voor '{insideWaterLevel}' moet een concreet getal zijn.", msgs[3]);
                Assert.AreEqual($"De standaardafwijking voor '{insideWaterLevel}' moet groter zijn dan of gelijk zijn aan 0.", msgs[4]);
                Assert.AreEqual($"De verwachtingswaarde voor '{drainCoefficient}' moet een concreet getal zijn.", msgs[5]);
                Assert.AreEqual($"De standaardafwijking voor '{drainCoefficient}' moet groter zijn dan of gelijk zijn aan 0.", msgs[6]);
                Assert.AreEqual($"De waarde voor '{factorStormDurationOpenStructure}' moet een concreet getal zijn.", msgs[7]);
                Assert.AreEqual($"De verwachtingswaarde voor '{areaFlowApertures}' moet een positief getal zijn.", msgs[8]);
                Assert.AreEqual($"De standaardafwijking voor '{areaFlowApertures}' moet groter zijn dan of gelijk zijn aan 0.", msgs[9]);
                Assert.AreEqual($"De verwachtingswaarde voor '{flowWidthAtBottomProtection}' moet een positief getal zijn.", msgs[10]);
                Assert.AreEqual($"De standaardafwijking voor '{flowWidthAtBottomProtection}' moet groter zijn dan of gelijk zijn aan 0.", msgs[11]);
                Assert.AreEqual($"De verwachtingswaarde voor '{storageStructureArea}' moet een positief getal zijn.", msgs[12]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{storageStructureArea}' moet groter zijn dan of gelijk zijn aan 0.", msgs[13]);
                Assert.AreEqual($"De verwachtingswaarde voor '{allowedLevelIncreaseStorage}' moet een positief getal zijn.", msgs[14]);
                Assert.AreEqual($"De standaardafwijking voor '{allowedLevelIncreaseStorage}' moet groter zijn dan of gelijk zijn aan 0.", msgs[15]);
                Assert.AreEqual($"De verwachtingswaarde voor '{criticalOvertoppingDischarge}' moet een positief getal zijn.", msgs[16]);
                Assert.AreEqual($"De variatiecoëfficiënt voor '{criticalOvertoppingDischarge}' moet groter zijn dan of gelijk zijn aan 0.", msgs[17]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[18]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Validate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup 
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           Path.Combine(testDataPath, "HRD dutch coast south.sqlite"));
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = (ClosingStructureInflowModelType) 9001
                }
            };

            // Call
            var isValid = false;
            TestDelegate call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            const string expectedMessage = "The value of argument 'input' (9001) is invalid for Enum type 'ClosingStructureInflowModelType'.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                    expectedMessage).ParamName;
            Assert.AreEqual("input", paramName);
            Assert.IsFalse(isValid);
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Validate_UsesBreakWaterAndHasInvalidBreakWaterSettings_LogsErrorAndReturnsFalse(
            [Values(ClosingStructureInflowModelType.VerticalWall, ClosingStructureInflowModelType.LowSill, ClosingStructureInflowModelType.FloodedCulvert)]
            ClosingStructureInflowModelType inflowModelType,
            [Values(double.NaN, double.NegativeInfinity, double.PositiveInfinity)]
            double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new ClosingStructuresFailureMechanism(),
                                                                                                           mockRepository,
                                                                                                           Path.Combine(testDataPath, "HRD dutch coast south.sqlite"));
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = inflowModelType,
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight)),
                    UseBreakWater = true
                }
            };

            var isValid = false;

            // Call
            Action call = () => isValid = ClosingStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De waarde voor 'hoogte' van de dam moet een concreet getal zijn.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_InvalidInFlowModelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    InflowModelType = (ClosingStructureInflowModelType) 100
                }
            };

            var service = new ClosingStructuresCalculationService();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                TestDelegate call = () => service.Calculate(calculation,
                                                            failureMechanism.GeneralInput,
                                                            validFilePath,
                                                            validPreprocessorDirectory);

                // Assert
                const string expectedMessage = "The value of argument 'structureInput' (100) is invalid for Enum type 'ClosingStructureInflowModelType'.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                        expectedMessage).ParamName;
                Assert.AreEqual("structureInput", paramName);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_VariousVerticalWallCalculationsWithoutBreakWater_InputPropertiesCorrectlySentToCalculator(bool useForeshore)
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);

            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    failureMechanism.GeneralInput,
                                                                    validFilePath,
                                                                    validPreprocessorDirectory);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureVerticalWallCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    null,
                    generalInput.GravitationalAcceleration,
                    input.FactorStormDurationOpenStructure,
                    input.FailureProbabilityOpenStructure,
                    input.FailureProbabilityReparation,
                    input.IdenticalApertures,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation,
                    input.ProbabilityOpenStructureBeforeFlooding,
                    generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.ModelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    input.LevelCrestStructureNotClosing.Mean, input.LevelCrestStructureNotClosing.StandardDeviation,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.StandardDeviation,
                    input.DeviationWaveDirection);

                var actualInput = (StructuresClosureVerticalWallCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_VariousVerticalWallCalculationsWithBreakWater_InputPropertiesCorrectlySentToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);

            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Type = breakWaterType
                        }
                    }
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    failureMechanism.GeneralInput,
                                                                    validFilePath,
                                                                    validPreprocessorDirectory);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureVerticalWallCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                    new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                    generalInput.GravitationalAcceleration,
                    input.FactorStormDurationOpenStructure,
                    input.FailureProbabilityOpenStructure,
                    input.FailureProbabilityReparation,
                    input.IdenticalApertures,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation,
                    input.ProbabilityOpenStructureBeforeFlooding,
                    generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.ModelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    input.LevelCrestStructureNotClosing.Mean, input.LevelCrestStructureNotClosing.StandardDeviation,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.StandardDeviation,
                    input.DeviationWaveDirection);

                var actualInput = (StructuresClosureVerticalWallCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_VariousLowSillCalculationsWithoutBreakWater_InputPropertiesCorrectlySentToCalculator(bool useForeshore)
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);

            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = ClosingStructureInflowModelType.LowSill
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    failureMechanism.GeneralInput,
                                                                    validFilePath,
                                                                    validPreprocessorDirectory);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureLowSillCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    null,
                    generalInput.GravitationalAcceleration,
                    input.FactorStormDurationOpenStructure,
                    input.FailureProbabilityOpenStructure,
                    input.FailureProbabilityReparation,
                    input.IdenticalApertures,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation,
                    input.ProbabilityOpenStructureBeforeFlooding,
                    input.ThresholdHeightOpenWeir.Mean, input.ThresholdHeightOpenWeir.StandardDeviation,
                    input.InsideWaterLevel.Mean, input.InsideWaterLevel.StandardDeviation,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.StandardDeviation,
                    generalInput.ModelFactorLongThreshold.Mean, generalInput.ModelFactorLongThreshold.StandardDeviation);

                var actualInput = (StructuresClosureLowSillCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_VariousLowSillCalculationsWithBreakWater_InputPropertiesCorrectlySentToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);

            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = ClosingStructureInflowModelType.LowSill,
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Type = breakWaterType
                        }
                    }
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    failureMechanism.GeneralInput,
                                                                    validFilePath,
                                                                    validPreprocessorDirectory);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureLowSillCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                    new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                    generalInput.GravitationalAcceleration,
                    input.FactorStormDurationOpenStructure,
                    input.FailureProbabilityOpenStructure,
                    input.FailureProbabilityReparation,
                    input.IdenticalApertures,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation,
                    input.ProbabilityOpenStructureBeforeFlooding,
                    input.ThresholdHeightOpenWeir.Mean, input.ThresholdHeightOpenWeir.StandardDeviation,
                    input.InsideWaterLevel.Mean, input.InsideWaterLevel.StandardDeviation,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.StandardDeviation,
                    generalInput.ModelFactorLongThreshold.Mean, generalInput.ModelFactorLongThreshold.StandardDeviation);

                var actualInput = (StructuresClosureLowSillCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_VariousFloodedCulvertCalculationsWithoutBreakWater_InputPropertiesCorrectlySentToCalculator(bool useForeshore)
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = ClosingStructureInflowModelType.FloodedCulvert
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    failureMechanism.GeneralInput,
                                                                    validFilePath,
                                                                    validPreprocessorDirectory);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureFloodedCulvertCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    null,
                    generalInput.GravitationalAcceleration,
                    input.FactorStormDurationOpenStructure,
                    input.FailureProbabilityOpenStructure,
                    input.FailureProbabilityReparation,
                    input.IdenticalApertures,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation,
                    input.ProbabilityOpenStructureBeforeFlooding,
                    input.DrainCoefficient.Mean, input.DrainCoefficient.StandardDeviation,
                    input.AreaFlowApertures.Mean, input.AreaFlowApertures.StandardDeviation,
                    input.InsideWaterLevel.Mean, input.InsideWaterLevel.StandardDeviation);

                var actualInput = (StructuresClosureFloodedCulvertCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void Calculate_VariousFloodedCulvertCalculationsWithBreakWater_InputPropertiesCorrectlySentToCalculator(BreakWaterType breakWaterType)
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = ClosingStructureInflowModelType.FloodedCulvert,
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Type = breakWaterType
                        }
                    }
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    failureMechanism.GeneralInput,
                                                                    validFilePath,
                                                                    validPreprocessorDirectory);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                GeneralClosingStructuresInput generalInput = failureMechanism.GeneralInput;
                ClosingStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresClosureFloodedCulvertCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)),
                    new HydraRingBreakWater(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), input.BreakWater.Height),
                    generalInput.GravitationalAcceleration,
                    input.FactorStormDurationOpenStructure,
                    input.FailureProbabilityOpenStructure,
                    input.FailureProbabilityReparation,
                    input.IdenticalApertures,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation,
                    input.ProbabilityOpenStructureBeforeFlooding,
                    input.DrainCoefficient.Mean, input.DrainCoefficient.StandardDeviation,
                    input.AreaFlowApertures.Mean, input.AreaFlowApertures.StandardDeviation,
                    input.InsideWaterLevel.Mean, input.InsideWaterLevel.StandardDeviation);

                var actualInput = (StructuresClosureFloodedCulvertCalculationInput) calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Calculate_PreprocessorDirectorySet_InputPropertiesCorrectlySentToCalculator([Values(true, false)] bool usePreprocessor)
        {
            // Setup
            string preprocessorDirectory = usePreprocessor
                                               ? validPreprocessorDirectory
                                               : string.Empty;

            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, preprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new ClosingStructuresCalculationService().Calculate(calculation,
                                                                    failureMechanism.GeneralInput,
                                                                    validFilePath,
                                                                    preprocessorDirectory);

                // Assert
                StructuresClosureCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                StructuresClosureCalculationInput actualInput = calculationInputs[0];
                Assert.AreEqual(usePreprocessor, actualInput.PreprocessorSetting.RunPreprocessor);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void Calculate_ValidCalculation_LogStartAndEndAndReturnOutput(
            [Values(ClosingStructureInflowModelType.VerticalWall, ClosingStructureInflowModelType.LowSill, ClosingStructureInflowModelType.FloodedCulvert)]
            ClosingStructureInflowModelType inflowModelType,
            [Values(CalculationType.NoForeshore, CalculationType.ForeshoreWithoutBreakWater, CalculationType.ForeshoreWithValidBreakWater)]
            CalculationType calculationType)
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    InflowModelType = inflowModelType,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true
                }
            };

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

            // Call
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = () => new ClosingStructuresCalculationService().Calculate(calculation,
                                                                                        failureMechanism.GeneralInput,
                                                                                        validFilePath,
                                                                                        validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                });
                Assert.IsNotNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new ClosingStructuresCalculationService().Calculate(calculation,
                                                                            failureMechanism.GeneralInput,
                                                                            validFilePath,
                                                                            validPreprocessorDirectory);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    StringAssert.StartsWith($"De berekening voor kunstwerk sluiten '{calculation.Name}' is mislukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>
            {
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new ClosingStructuresCalculationService().Calculate(calculation,
                                                                            failureMechanism.GeneralInput,
                                                                            validFilePath,
                                                                            validPreprocessorDirectory);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"De berekening voor kunstwerk sluiten '{calculation.Name}' is mislukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<StructuresClosureCalculationInput>
            {
                EndInFailure = false,
                LastErrorFileContent = "An error occurred"
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;
                string exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new ClosingStructuresCalculationService().Calculate(calculation,
                                                                            failureMechanism.GeneralInput,
                                                                            validFilePath,
                                                                            validPreprocessorDirectory);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exceptionThrown = true;
                        exceptionMessage = e.Message;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    StringAssert.StartsWith($"De berekening voor kunstwerk sluiten '{calculation.Name}' is mislukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }

            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Sets all input parameters of <see cref="ClosingStructuresInput"/> to invalid values.
        /// </summary>
        /// <param name="input">The input to be updated.</param>
        /// <param name="value">The invalid value to be set on all input properties.</param>
        /// <remarks>If <paramref name="value"/> cannot be set on an input property, that
        /// value is set to <see cref="RoundedDouble.NaN"/>.</remarks>
        private static void SetInvalidInputParameters(ClosingStructuresInput input, RoundedDouble value)
        {
            input.DeviationWaveDirection = RoundedDouble.NaN;
            input.DrainCoefficient.Mean = value;
            input.FactorStormDurationOpenStructure = value;
            input.InsideWaterLevel.Mean = value;
            input.LevelCrestStructureNotClosing.Mean = value;
            input.ModelFactorSuperCriticalFlow.Mean = value;
            input.StructureNormalOrientation = RoundedDouble.NaN;
            input.ThresholdHeightOpenWeir.Mean = value;
            input.WidthFlowApertures.Mean = value;

            if (double.IsNegativeInfinity(value))
            {
                input.AllowedLevelIncreaseStorage.Mean = RoundedDouble.NaN;
                input.AllowedLevelIncreaseStorage.StandardDeviation = RoundedDouble.NaN;
                input.AreaFlowApertures.Mean = RoundedDouble.NaN;
                input.AreaFlowApertures.StandardDeviation = RoundedDouble.NaN;
                input.CriticalOvertoppingDischarge.Mean = RoundedDouble.NaN;
                input.CriticalOvertoppingDischarge.CoefficientOfVariation = RoundedDouble.NaN;
                input.DrainCoefficient.StandardDeviation = RoundedDouble.NaN;
                input.FlowWidthAtBottomProtection.Mean = RoundedDouble.NaN;
                input.FlowWidthAtBottomProtection.StandardDeviation = RoundedDouble.NaN;
                input.InsideWaterLevel.StandardDeviation = RoundedDouble.NaN;
                input.LevelCrestStructureNotClosing.StandardDeviation = RoundedDouble.NaN;
                input.ModelFactorSuperCriticalFlow.StandardDeviation = RoundedDouble.NaN;
                input.StorageStructureArea.Mean = RoundedDouble.NaN;
                input.StorageStructureArea.CoefficientOfVariation = RoundedDouble.NaN;
                input.StormDuration.Mean = RoundedDouble.NaN;
                input.StormDuration.CoefficientOfVariation = RoundedDouble.NaN;
                input.ThresholdHeightOpenWeir.StandardDeviation = RoundedDouble.NaN;
                input.WidthFlowApertures.StandardDeviation = RoundedDouble.NaN;
            }
            else
            {
                input.AllowedLevelIncreaseStorage.Mean = value;
                input.AllowedLevelIncreaseStorage.StandardDeviation = value;
                input.AreaFlowApertures.Mean = value;
                input.AreaFlowApertures.StandardDeviation = value;
                input.CriticalOvertoppingDischarge.Mean = value;
                input.CriticalOvertoppingDischarge.CoefficientOfVariation = value;
                input.DrainCoefficient.StandardDeviation = value;
                input.FlowWidthAtBottomProtection.Mean = value;
                input.FlowWidthAtBottomProtection.StandardDeviation = value;
                input.InsideWaterLevel.StandardDeviation = value;
                input.LevelCrestStructureNotClosing.StandardDeviation = value;
                input.ModelFactorSuperCriticalFlow.StandardDeviation = value;
                input.StorageStructureArea.Mean = value;
                input.StorageStructureArea.CoefficientOfVariation = value;
                input.StormDuration.Mean = value;
                input.StormDuration.CoefficientOfVariation = value;
                input.ThresholdHeightOpenWeir.StandardDeviation = value;
                input.WidthFlowApertures.StandardDeviation = value;
            }
        }

        #region Parameter name mappings

        private const string insideWaterLevel = "Binnenwaterstand";
        private const string stormDuration = "Stormduur";
        private const string factorStormDurationOpenStructure = "Factor voor stormduur hoogwater";
        private const string modelFactorSuperCriticalFlow = "Modelfactor overloopdebiet volkomen overlaat";
        private const string drainCoefficient = "Afvoercoëfficiënt";
        private const string structureNormalOrientation = "Oriëntatie";
        private const string thresholdHeightOpenWeir = "Drempelhoogte";
        private const string areaFlowApertures = "Doorstroomoppervlak";
        private const string levelCrestStructureNotClosing = "Kruinhoogte niet gesloten kering";
        private const string allowedLevelIncreaseStorage = "Toegestane peilverhoging komberging";
        private const string storageStructureArea = "Kombergend oppervlak";
        private const string flowWidthAtBottomProtection = "Stroomvoerende breedte bodembescherming";
        private const string criticalOvertoppingDischarge = "Kritiek instromend debiet";
        private const string widthFlowApertures = "Breedte van doorstroomopening";

        #endregion
    }
}