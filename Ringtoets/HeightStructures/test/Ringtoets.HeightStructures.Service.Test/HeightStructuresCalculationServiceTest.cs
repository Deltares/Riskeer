﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.Structures;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.HeightStructures.Service.Test
{
    [TestFixture]
    public class HeightStructuresCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var service = new HeightStructuresCalculationService();

            // Assert
            Assert.IsInstanceOf<StructuresCalculationServiceBase<HeightStructuresValidationRulesRegistry, HeightStructuresInput,
                HeightStructure, GeneralHeightStructuresInput, StructuresOvertoppingCalculationInput>>(service);
        }

        [Test]
        public void Validate_StructureNormalOrientationInvalid_ReturnsFalse()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string expectedValidationMessage = "De waarde voor 'oriëntatie' moet een concreet getal zijn.";

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    Structure = new TestHeightStructure()
                }
            };

            calculation.InputParameters.StructureNormalOrientation = RoundedDouble.NaN;

            // Call
            var isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(NormalDistributionsWithInvalidMean))]
        public void Validate_NormalDistributionMeanInvalid_LogsErrorAndReturnsFalse(double meanOne, double meanTwo, double meanThree, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(),
                                                                                                       mockRepository);
            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;

            string expectedValidationMessage = $"De verwachtingswaarde voor '{parameterName}' moet een concreet getal zijn.";

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean = (RoundedDouble) meanOne;
            calculation.InputParameters.LevelCrestStructure.Mean = (RoundedDouble) meanTwo;
            calculation.InputParameters.WidthFlowApertures.Mean = (RoundedDouble) meanThree;

            // Call
            var isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(LogNormalDistributionsWithInvalidMean))]
        public void Validate_LogNormalDistributionMeanInvalid_LogsErrorAndReturnsFalse(double meanOne, double meanTwo, double meanThree,
                                                                                       double meanFour, double meanFive, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;

            string expectedValidationMessage = $"De verwachtingswaarde voor '{parameterName}' moet een positief getal zijn.";

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            calculation.InputParameters.StormDuration.Mean = (RoundedDouble) meanOne;
            calculation.InputParameters.AllowedLevelIncreaseStorage.Mean = (RoundedDouble) meanTwo;
            calculation.InputParameters.StorageStructureArea.Mean = (RoundedDouble) meanThree;
            calculation.InputParameters.FlowWidthAtBottomProtection.Mean = (RoundedDouble) meanFour;
            calculation.InputParameters.CriticalOvertoppingDischarge.Mean = (RoundedDouble) meanFive;

            // Call
            var isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(DistributionsWithInvalidDeviation))]
        public void Validate_DistributionStandardDeviationInvalid_LogsErrorAndReturnsFalse(double deviationOne, double deviationTwo,
                                                                                           double deviationThree, double deviationFour,
                                                                                           double deviationFive, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;

            string expectedValidationMessage = $"De standaardafwijking voor '{parameterName}' moet groter zijn dan of gelijk zijn aan 0.";

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation = (RoundedDouble) deviationOne;
            calculation.InputParameters.LevelCrestStructure.StandardDeviation = (RoundedDouble) deviationTwo;
            calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation = (RoundedDouble) deviationThree;
            calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation = (RoundedDouble) deviationFour;
            calculation.InputParameters.WidthFlowApertures.StandardDeviation = (RoundedDouble) deviationFive;

            // Call
            var isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(DistributionsWithInvalidCoefficient))]
        public void Validate_DistributionVariationCoefficientInvalid_LogsErrorAndReturnsFalse(
            double coefficientOne, double coefficientTwo, double coefficientThree, string parameterName)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;

            string expectedValidationMessage = $"De variatiecoëfficiënt voor '{parameterName}' moet groter zijn dan of gelijk zijn aan 0.";

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            calculation.InputParameters.StormDuration.CoefficientOfVariation = (RoundedDouble) coefficientOne;
            calculation.InputParameters.StorageStructureArea.CoefficientOfVariation = (RoundedDouble) coefficientTwo;
            calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation = (RoundedDouble) coefficientThree;

            // Call
            var isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual(expectedValidationMessage, msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_UsesBreakWaterAndHasInvalidBreakWaterSettings_LogsErrorAndReturnsFalse(double breakWaterHeight)
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    ForeshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Dam, breakWaterHeight)),
                    UseBreakWater = true
                }
            };

            // Call
            var isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

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
        public void Validate_ValidCalculationInputAndHydraulicBoundaryDatabase_ReturnsTrue()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new HeightStructuresFailureMechanism(), mockRepository);
            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2)
                }
            };

            // Call
            var isValid = false;
            Action call = () => isValid = HeightStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
            Assert.IsTrue(isValid);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(CalculationType.NoForeshore)]
        [TestCase(CalculationType.ForeshoreWithValidBreakWater)]
        [TestCase(CalculationType.ForeshoreWithoutBreakWater)]
        public void Calculate_ValidCalculationInputAndForeshoreWithValidBreakWater_LogStartAndEndAndReturnOutput(CalculationType calculationType)
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(new TestStructuresCalculator<StructuresOvertoppingCalculationInput>());
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseBreakWater = true,
                    UseForeshore = true
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
                Action call = () => new HeightStructuresCalculationService().Calculate(calculation,
                                                                                       failureMechanism.GeneralInput,
                                                                                       failureMechanism.GeneralInput.N,
                                                                                       assessmentSection.FailureMechanismContribution.Norm,
                                                                                       failureMechanism.Contribution,
                                                                                       validFilePath,
                                                                                       validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie" +
                                            "" +
                                            "", msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                });
                Assert.IsNotNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Calculate_ValidCalculationInputAndForeshoreWithInvalidBreakWater_LogStartAndEndAndReturnOutput(double height)
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(new TestStructuresCalculator<StructuresOvertoppingCalculationInput>());
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001),
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseBreakWater = false,
                    UseForeshore = true
                }
            };

            // Call
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = () => new HeightStructuresCalculationService().Calculate(calculation,
                                                                                       failureMechanism.GeneralInput,
                                                                                       failureMechanism.GeneralInput.N,
                                                                                       assessmentSection.FailureMechanismContribution.Norm,
                                                                                       failureMechanism.Contribution,
                                                                                       validFilePath,
                                                                                       validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                });
                Assert.IsNotNull(calculation.Output);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void Calculate_VariousCalculations_InputPropertiesCorrectlySentToCalculator(bool useForeshore, bool useBreakWater)
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository);

            var calculator = new TestStructuresCalculator<StructuresOvertoppingCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            if (useForeshore)
            {
                calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile(useBreakWater);
            }

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new HeightStructuresCalculationService().Calculate(calculation,
                                                                   failureMechanism.GeneralInput,
                                                                   failureMechanism.GeneralInput.N,
                                                                   assessmentSection.FailureMechanismContribution.Norm,
                                                                   failureMechanism.Contribution,
                                                                   validFilePath,
                                                                   validPreprocessorDirectory);

                // Assert
                StructuresOvertoppingCalculationInput[] overtoppingCalculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, overtoppingCalculationInputs.Length);

                GeneralHeightStructuresInput generalInput = failureMechanism.GeneralInput;
                HeightStructuresInput input = calculation.InputParameters;
                var expectedInput = new StructuresOvertoppingCalculationInput(
                    1300001,
                    input.StructureNormalOrientation,
                    useForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0],
                    useBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null,
                    generalInput.GravitationalAcceleration,
                    generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                    input.LevelCrestStructure.Mean, input.LevelCrestStructure.StandardDeviation,
                    input.StructureNormalOrientation,
                    input.ModelFactorSuperCriticalFlow.Mean, input.ModelFactorSuperCriticalFlow.StandardDeviation,
                    input.AllowedLevelIncreaseStorage.Mean, input.AllowedLevelIncreaseStorage.StandardDeviation,
                    generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                    input.StorageStructureArea.Mean, input.StorageStructureArea.CoefficientOfVariation,
                    generalInput.ModelFactorInflowVolume,
                    input.FlowWidthAtBottomProtection.Mean, input.FlowWidthAtBottomProtection.StandardDeviation,
                    input.CriticalOvertoppingDischarge.Mean, input.CriticalOvertoppingDischarge.CoefficientOfVariation,
                    input.FailureProbabilityStructureWithErosion,
                    input.WidthFlowApertures.Mean, input.WidthFlowApertures.StandardDeviation,
                    input.DeviationWaveDirection,
                    input.StormDuration.Mean, input.StormDuration.CoefficientOfVariation);

                StructuresOvertoppingCalculationInput actualInput = overtoppingCalculationInputs[0];
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

            var failureMechanism = new HeightStructuresFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mockRepository);
            var calculator = new TestStructuresCalculator<StructuresOvertoppingCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, preprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new HeightStructuresCalculationService().Calculate(calculation,
                                                                   failureMechanism.GeneralInput,
                                                                   failureMechanism.GeneralInput.N,
                                                                   assessmentSection.FailureMechanismContribution.Norm,
                                                                   failureMechanism.Contribution,
                                                                   validFilePath,
                                                                   preprocessorDirectory);

                // Assert
                StructuresOvertoppingCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                StructuresOvertoppingCalculationInput actualInput = calculationInputs[0];
                Assert.AreEqual(usePreprocessor, actualInput.PreprocessorSetting.RunPreprocessor);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mockRepository);

            var calculator = new TestStructuresCalculator<StructuresOvertoppingCalculationInput>
            {
                EndInFailure = true,
                LastErrorFileContent = "An error occurred"
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
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
                        new HeightStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.GeneralInput.N,
                                                                           assessmentSection.FailureMechanismContribution.Norm,
                                                                           failureMechanism.Contribution,
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
                    StringAssert.StartsWith($"De berekening voor hoogte kunstwerk '{calculation.Name}' is mislukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
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
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mockRepository);

            var calculator = new TestStructuresCalculator<StructuresOvertoppingCalculationInput>
            {
                EndInFailure = true
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
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
                        new HeightStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.GeneralInput.N,
                                                                           assessmentSection.FailureMechanismContribution.Norm,
                                                                           failureMechanism.Contribution,
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
                    Assert.AreEqual($"De berekening voor hoogte kunstwerk '{calculation.Name}' is mislukt. Er is geen foutrapport beschikbaar.", msgs[1]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
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
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mockRepository);

            var calculator = new TestStructuresCalculator<StructuresOvertoppingCalculationInput>
            {
                LastErrorFileContent = "An error occurred"
            };
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestHeightStructuresCalculation
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
                        new HeightStructuresCalculationService().Calculate(calculation,
                                                                           failureMechanism.GeneralInput,
                                                                           failureMechanism.GeneralInput.N,
                                                                           assessmentSection.FailureMechanismContribution.Norm,
                                                                           failureMechanism.Contribution,
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
                    StringAssert.StartsWith($"De berekening voor hoogte kunstwerk '{calculation.Name}' is mislukt. Bekijk het foutrapport door op details te klikken.", msgs[1]);
                    StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }
            mockRepository.VerifyAll();
        }

        #region Test cases

        private static IEnumerable<TestCaseData> NormalDistributionsWithInvalidMean
        {
            get
            {
                yield return new TestCaseData(double.NaN, 1, 2, "modelfactor overloopdebiet volkomen overlaat");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, "modelfactor overloopdebiet volkomen overlaat");
                yield return new TestCaseData(double.NegativeInfinity, 1, 2, "modelfactor overloopdebiet volkomen overlaat");

                yield return new TestCaseData(1, double.NaN, 2, "kerende hoogte");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, "kerende hoogte");
                yield return new TestCaseData(1, double.NegativeInfinity, 2, "kerende hoogte");

                yield return new TestCaseData(1, 2, double.NaN, "breedte van doorstroomopening");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, "breedte van doorstroomopening");
                yield return new TestCaseData(1, 2, double.NegativeInfinity, "breedte van doorstroomopening");
            }
        }

        private static IEnumerable<TestCaseData> LogNormalDistributionsWithInvalidMean
        {
            get
            {
                yield return new TestCaseData(double.NaN, 1, 2, 3, 4, "stormduur");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, 3, 4, "stormduur");

                yield return new TestCaseData(1, double.NaN, 2, 3, 4, "toegestane peilverhoging komberging");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, 3, 4, "toegestane peilverhoging komberging");

                yield return new TestCaseData(1, 2, double.NaN, 3, 4, "kombergend oppervlak");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, 3, 4, "kombergend oppervlak");

                yield return new TestCaseData(1, 2, 3, double.NaN, 4, "stroomvoerende breedte bodembescherming");
                yield return new TestCaseData(1, 2, 3, double.PositiveInfinity, 4, "stroomvoerende breedte bodembescherming");

                yield return new TestCaseData(1, 2, 3, 4, double.NaN, "kritiek instromend debiet");
                yield return new TestCaseData(1, 2, 3, 4, double.PositiveInfinity, "kritiek instromend debiet");
            }
        }

        private static IEnumerable<TestCaseData> DistributionsWithInvalidDeviation
        {
            get
            {
                yield return new TestCaseData(double.NaN, 1, 2, 3, 4, "modelfactor overloopdebiet volkomen overlaat");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, 3, 4, "modelfactor overloopdebiet volkomen overlaat");

                yield return new TestCaseData(1, double.NaN, 2, 3, 4, "kerende hoogte");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, 3, 4, "kerende hoogte");

                yield return new TestCaseData(1, 2, double.NaN, 3, 4, "toegestane peilverhoging komberging");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, 3, 4, "toegestane peilverhoging komberging");

                yield return new TestCaseData(1, 2, 3, double.NaN, 4, "stroomvoerende breedte bodembescherming");
                yield return new TestCaseData(1, 2, 3, double.PositiveInfinity, 4, "stroomvoerende breedte bodembescherming");

                yield return new TestCaseData(1, 2, 3, 4, double.NaN, "breedte van doorstroomopening");
                yield return new TestCaseData(1, 2, 3, 4, double.PositiveInfinity, "breedte van doorstroomopening");
            }
        }

        private static IEnumerable<TestCaseData> DistributionsWithInvalidCoefficient
        {
            get
            {
                yield return new TestCaseData(double.NaN, 1, 2, "stormduur");
                yield return new TestCaseData(double.PositiveInfinity, 1, 2, "stormduur");

                yield return new TestCaseData(1, double.NaN, 2, "kombergend oppervlak");
                yield return new TestCaseData(1, double.PositiveInfinity, 2, "kombergend oppervlak");

                yield return new TestCaseData(1, 2, double.NaN, "kritiek instromend debiet");
                yield return new TestCaseData(1, 2, double.PositiveInfinity, "kritiek instromend debiet");
            }
        }

        #endregion
    }
}