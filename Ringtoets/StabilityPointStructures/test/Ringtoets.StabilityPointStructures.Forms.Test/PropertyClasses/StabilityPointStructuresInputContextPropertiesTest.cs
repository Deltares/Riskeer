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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;

namespace Ringtoets.StabilityPointStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityPointStructuresInputContextPropertiesTest
    {
        #region LowSill + Linear Model property Indices

        private const int linear_LowSill_hydraulicBoundaryLocationPropertyIndex = 0;
        private const int linear_LowSill_volumicWeightWaterPropertyIndex = 1;
        private const int linear_LowSill_stormDurationPropertyIndex = 2;
        private const int linear_LowSill_insideWaterLevelPropertyIndex = 3;
        private const int linear_LowSill_insideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int linear_LowSill_flowVelocityStructureClosablePropertyIndex = 5;
        private const int linear_LowSill_modelFactorSuperCriticalFlowPropertyIndex = 6;
        private const int linear_LowSill_factorStormDurationOpenStructurePropertyIndex = 7;
        private const int linear_LowSill_structurePropertyIndex = 8;
        private const int linear_LowSill_structureLocationPropertyIndex = 9;
        private const int linear_LowSill_structureNormalOrientationPropertyIndex = 10;
        private const int linear_LowSill_inflowModelTypePropertyIndex = 11;
        private const int linear_LowSill_loadSchematizationTypePropertyIndex = 12;
        private const int linear_LowSill_widthFlowAperturesPropertyIndex = 13;
        private const int linear_LowSill_flowWidthAtBottomProtectionPropertyIndex = 14;
        private const int linear_LowSill_storageStructureAreaPropertyIndex = 15;
        private const int linear_LowSill_allowedLevelIncreaseStoragePropertyIndex = 16;
        private const int linear_LowSill_levelCrestStructurePropertyIndex = 17;
        private const int linear_LowSill_thresholdHeightOpenWeirPropertyIndex = 18;
        private const int linear_LowSill_criticalOvertoppingDischargePropertyIndex = 19;
        private const int linear_LowSill_constructiveStrengthLinearLoadModelPropertyIndex = 20;
        private const int linear_LowSill_bankWidthPropertyIndex = 21;
        private const int linear_LowSill_evaluationLevelPropertyIndex = 22;
        private const int linear_LowSill_verticalDistancePropertyIndex = 23;
        private const int linear_LowSill_failureProbabilityRepairClosurePropertyIndex = 24;
        private const int linear_LowSill_failureCollisionEnergyPropertyIndex = 25;
        private const int linear_LowSill_shipMassPropertyIndex = 26;
        private const int linear_LowSill_shipVelocityPropertyIndex = 27;
        private const int linear_LowSill_levellingCountPropertyIndex = 28;
        private const int linear_LowSill_probabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int linear_LowSill_stabilityLinearLoadModelPropertyIndex = 30;
        private const int linear_LowSill_failureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int linear_LowSill_foreshoreProfilePropertyIndex = 32;
        private const int linear_LowSill_useBreakWaterPropertyIndex = 33;
        private const int linear_LowSill_useForeshorePropertyIndex = 34;

        #endregion

        #region FloodedCulvert + Linear Model property Indices

        private const int linear_FloodedCulvert_hydraulicBoundaryLocationPropertyIndex = 0;
        private const int linear_FloodedCulvert_volumicWeightWaterPropertyIndex = 1;
        private const int linear_FloodedCulvert_stormDurationPropertyIndex = 2;
        private const int linear_FloodedCulvert_insideWaterLevelPropertyIndex = 3;
        private const int linear_FloodedCulvert_insideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int linear_FloodedCulvert_flowVelocityStructureClosablePropertyIndex = 5;
        private const int linear_FloodedCulvert_drainCoefficientPropertyIndex = 6;
        private const int linear_FloodedCulvert_factorStormDurationOpenStructurePropertyIndex = 7;
        private const int linear_FloodedCulvert_structurePropertyIndex = 8;
        private const int linear_FloodedCulvert_structureLocationPropertyIndex = 9;
        private const int linear_FloodedCulvert_structureNormalOrientationPropertyIndex = 10;
        private const int linear_FloodedCulvert_inflowModelTypePropertyIndex = 11;
        private const int linear_FloodedCulvert_loadSchematizationTypePropertyIndex = 12;
        private const int linear_FloodedCulvert_areaFlowAperturesPropertyIndex = 13;
        private const int linear_FloodedCulvert_flowWidthAtBottomProtectionPropertyIndex = 14;
        private const int linear_FloodedCulvert_storageStructureAreaPropertyIndex = 15;
        private const int linear_FloodedCulvert_allowedLevelIncreaseStoragePropertyIndex = 16;
        private const int linear_FloodedCulvert_levelCrestStructurePropertyIndex = 17;
        private const int linear_FloodedCulvert_thresholdHeightOpenWeirPropertyIndex = 18;
        private const int linear_FloodedCulvert_criticalOvertoppingDischargePropertyIndex = 19;
        private const int linear_FloodedCulvert_constructiveStrengthLinearLoadModelPropertyIndex = 20;
        private const int linear_FloodedCulvert_bankWidthPropertyIndex = 21;
        private const int linear_FloodedCulvert_evaluationLevelPropertyIndex = 22;
        private const int linear_FloodedCulvert_verticalDistancePropertyIndex = 23;
        private const int linear_FloodedCulvert_failureProbabilityRepairClosurePropertyIndex = 24;
        private const int linear_FloodedCulvert_failureCollisionEnergyPropertyIndex = 25;
        private const int linear_FloodedCulvert_shipMassPropertyIndex = 26;
        private const int linear_FloodedCulvert_shipVelocityPropertyIndex = 27;
        private const int linear_FloodedCulvert_levellingCountPropertyIndex = 28;
        private const int linear_FloodedCulvert_probabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int linear_FloodedCulvert_stabilityLinearLoadModelPropertyIndex = 30;
        private const int linear_FloodedCulvert_failureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int linear_FloodedCulvert_foreshoreProfilePropertyIndex = 32;
        private const int linear_FloodedCulvert_useBreakWaterPropertyIndex = 33;
        private const int linear_FloodedCulvert_useForeshorePropertyIndex = 34;

        #endregion

        #region LowSill + Quadratic Model property Indices

        private const int quadratic_LowSill_hydraulicBoundaryLocationPropertyIndex = 0;
        private const int quadratic_LowSill_volumicWeightWaterPropertyIndex = 1;
        private const int quadratic_LowSill_stormDurationPropertyIndex = 2;
        private const int quadratic_LowSill_insideWaterLevelPropertyIndex = 3;
        private const int quadratic_LowSill_insideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int quadratic_LowSill_flowVelocityStructureClosablePropertyIndex = 5;
        private const int quadratic_LowSill_modelFactorSuperCriticalFlowPropertyIndex = 6;
        private const int quadratic_LowSill_factorStormDurationOpenStructurePropertyIndex = 7;
        private const int quadratic_LowSill_structurePropertyIndex = 8;
        private const int quadratic_LowSill_structureLocationPropertyIndex = 9;
        private const int quadratic_LowSill_structureNormalOrientationPropertyIndex = 10;
        private const int quadratic_LowSill_inflowModelTypePropertyIndex = 11;
        private const int quadratic_LowSill_loadSchematizationTypePropertyIndex = 12;
        private const int quadratic_LowSill_widthFlowAperturesPropertyIndex = 13;
        private const int quadratic_LowSill_flowWidthAtBottomProtectionPropertyIndex = 14;
        private const int quadratic_LowSill_storageStructureAreaPropertyIndex = 15;
        private const int quadratic_LowSill_allowedLevelIncreaseStoragePropertyIndex = 16;
        private const int quadratic_LowSill_levelCrestStructurePropertyIndex = 17;
        private const int quadratic_LowSill_thresholdHeightOpenWeirPropertyIndex = 18;
        private const int quadratic_LowSill_criticalOvertoppingDischargePropertyIndex = 19;
        private const int quadratic_LowSill_constructiveStrengthQuadraticLoadModelPropertyIndex = 20;
        private const int quadratic_LowSill_bankWidthPropertyIndex = 21;
        private const int quadratic_LowSill_evaluationLevelPropertyIndex = 22;
        private const int quadratic_LowSill_verticalDistancePropertyIndex = 23;
        private const int quadratic_LowSill_failureProbabilityRepairClosurePropertyIndex = 24;
        private const int quadratic_LowSill_failureCollisionEnergyPropertyIndex = 25;
        private const int quadratic_LowSill_shipMassPropertyIndex = 26;
        private const int quadratic_LowSill_shipVelocityPropertyIndex = 27;
        private const int quadratic_LowSill_levellingCountPropertyIndex = 28;
        private const int quadratic_LowSill_probabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int quadratic_LowSill_stabilityQuadraticLoadModelPropertyIndex = 30;
        private const int quadratic_LowSill_failureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int quadratic_LowSill_foreshoreProfilePropertyIndex = 32;
        private const int quadratic_LowSill_useBreakWaterPropertyIndex = 33;
        private const int quadratic_LowSill_useForeshorePropertyIndex = 34;

        #endregion

        #region FloodedCulvert + Quadratic Model property Indices

        private const int quadratic_FloodedCulvert_hydraulicBoundaryLocationPropertyIndex = 0;
        private const int quadratic_FloodedCulvert_volumicWeightWaterPropertyIndex = 1;
        private const int quadratic_FloodedCulvert_stormDurationPropertyIndex = 2;
        private const int quadratic_FloodedCulvert_insideWaterLevelPropertyIndex = 3;
        private const int quadratic_FloodedCulvert_insideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int quadratic_FloodedCulvert_flowVelocityStructureClosablePropertyIndex = 5;
        private const int quadratic_FloodedCulvert_drainCoefficientPropertyIndex = 6;
        private const int quadratic_FloodedCulvert_factorStormDurationOpenStructurePropertyIndex = 7;
        private const int quadratic_FloodedCulvert_structurePropertyIndex = 8;
        private const int quadratic_FloodedCulvert_structureLocationPropertyIndex = 9;
        private const int quadratic_FloodedCulvert_structureNormalOrientationPropertyIndex = 10;
        private const int quadratic_FloodedCulvert_inflowModelTypePropertyIndex = 11;
        private const int quadratic_FloodedCulvert_loadSchematizationTypePropertyIndex = 12;
        private const int quadratic_FloodedCulvert_areaFlowAperturesPropertyIndex = 13;
        private const int quadratic_FloodedCulvert_flowWidthAtBottomProtectionPropertyIndex = 14;
        private const int quadratic_FloodedCulvert_storageStructureAreaPropertyIndex = 15;
        private const int quadratic_FloodedCulvert_allowedLevelIncreaseStoragePropertyIndex = 16;
        private const int quadratic_FloodedCulvert_levelCrestStructurePropertyIndex = 17;
        private const int quadratic_FloodedCulvert_thresholdHeightOpenWeirPropertyIndex = 18;
        private const int quadratic_FloodedCulvert_criticalOvertoppingDischargePropertyIndex = 19;
        private const int quadratic_FloodedCulvert_constructiveStrengthQuadraticLoadModelPropertyIndex = 20;
        private const int quadratic_FloodedCulvert_bankWidthPropertyIndex = 21;
        private const int quadratic_FloodedCulvert_evaluationLevelPropertyIndex = 22;
        private const int quadratic_FloodedCulvert_verticalDistancePropertyIndex = 23;
        private const int quadratic_FloodedCulvert_failureProbabilityRepairClosurePropertyIndex = 24;
        private const int quadratic_FloodedCulvert_failureCollisionEnergyPropertyIndex = 25;
        private const int quadratic_FloodedCulvert_shipMassPropertyIndex = 26;
        private const int quadratic_FloodedCulvert_shipVelocityPropertyIndex = 27;
        private const int quadratic_FloodedCulvert_levellingCountPropertyIndex = 28;
        private const int quadratic_FloodedCulvert_probabilityCollisionSecondaryStructurePropertyIndex = 29;
        private const int quadratic_FloodedCulvert_stabilityQuadraticLoadModelPropertyIndex = 30;
        private const int quadratic_FloodedCulvert_failureProbabilityStructureWithErosionPropertyIndex = 31;
        private const int quadratic_FloodedCulvert_foreshoreProfilePropertyIndex = 32;
        private const int quadratic_FloodedCulvert_useBreakWaterPropertyIndex = 33;
        private const int quadratic_FloodedCulvert_useForeshorePropertyIndex = 34;

        #endregion

        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new StabilityPointStructuresInputContextProperties();

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<StabilityPointStructure, StabilityPointStructuresInput, StructuresCalculation<StabilityPointStructuresInput>, StabilityPointStructuresFailureMechanism>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var properties = new StabilityPointStructuresInputContextProperties();

            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            properties.Data = inputContext;

            // Assert
            StabilityPointStructuresInput input = calculation.InputParameters;
            var expectedFailureProbabilityRepairClosure = ProbabilityFormattingHelper.Format(input.FailureProbabilityRepairClosure);
            var expectedProbabilityCollisionSecondaryStructure = ProbabilityFormattingHelper.Format(input.ProbabilityCollisionSecondaryStructure);

            Assert.AreEqual(input.VolumicWeightWater, properties.VolumicWeightWater);
            Assert.AreSame(input.InsideWaterLevelFailureConstruction, properties.InsideWaterLevelFailureConstruction.Data);
            Assert.AreSame(input.InsideWaterLevel, properties.InsideWaterLevel.Data);
            Assert.AreSame(input.DrainCoefficient, properties.DrainCoefficient.Data);
            Assert.AreEqual(input.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure);
            Assert.AreSame(input.FlowVelocityStructureClosable, properties.FlowVelocityStructureClosable.Data);
            Assert.AreEqual(input.InflowModelType, properties.InflowModelType);
            Assert.AreEqual(input.LoadSchematizationType, properties.LoadSchematizationType);
            Assert.AreSame(input.LevelCrestStructure, properties.LevelCrestStructure.Data);
            Assert.AreSame(input.ThresholdHeightOpenWeir, properties.ThresholdHeightOpenWeir.Data);
            Assert.AreSame(input.AreaFlowApertures, properties.AreaFlowApertures.Data);
            Assert.AreSame(input.ConstructiveStrengthLinearLoadModel, properties.ConstructiveStrengthLinearLoadModel.Data);
            Assert.AreSame(input.ConstructiveStrengthQuadraticLoadModel, properties.ConstructiveStrengthQuadraticLoadModel.Data);
            Assert.AreSame(input.StabilityLinearLoadModel, properties.StabilityLinearLoadModel.Data);
            Assert.AreSame(input.StabilityQuadraticLoadModel, properties.StabilityQuadraticLoadModel.Data);
            Assert.AreEqual(expectedFailureProbabilityRepairClosure, properties.FailureProbabilityRepairClosure);
            Assert.AreSame(input.FailureCollisionEnergy, properties.FailureCollisionEnergy.Data);
            Assert.AreSame(input.ShipMass, properties.ShipMass.Data);
            Assert.AreSame(input.ShipVelocity, properties.ShipVelocity.Data);
            Assert.AreEqual(input.LevellingCount, properties.LevellingCount);
            Assert.AreEqual(expectedProbabilityCollisionSecondaryStructure, properties.ProbabilityCollisionSecondaryStructure);
            Assert.AreSame(input.BankWidth, properties.BankWidth.Data);
            Assert.AreEqual(input.EvaluationLevel, properties.EvaluationLevel);
            Assert.AreEqual(input.VerticalDistance, properties.VerticalDistance);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableForeshoreProfiles_SetInputContextInstanceWithForeshoreProfiles_ReturnForeshoreProfiles()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                ForeshoreProfiles =
                {
                    new TestForeshoreProfile()
                }
            };
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            var availableForeshoreProfiles = properties.GetAvailableForeshoreProfiles();

            // Assert
            Assert.AreSame(failureMechanism.ForeshoreProfiles, availableForeshoreProfiles);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetAvailableStructures_SetInputContextInstanceWithStructures_ReturnStructures()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                StabilityPointStructures =
                {
                    new TestStabilityPointStructure()
                }
            };
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            var availableStructures = properties.GetAvailableStructures();

            // Assert
            Assert.AreSame(failureMechanism.StabilityPointStructures, availableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 9;
            var observerMock = mockRepository.StrictMock<IObserver>();
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();

            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);

            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            input.Attach(observerMock);

            var random = new Random(100);
            double newVolumicWeightWater = random.NextDouble();
            double newFactorStormDurationOpenStructure = random.NextDouble();
            var newInflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert;
            var newLoadSchematizationType = LoadSchematizationType.Quadratic;
            var newLevellingCount = 2;
            double newEvaluationLevel = random.NextDouble();
            double newVerticalDistance = random.NextDouble();

            // Call
            properties.VolumicWeightWater = (RoundedDouble) newVolumicWeightWater;
            properties.FactorStormDurationOpenStructure = (RoundedDouble) newFactorStormDurationOpenStructure;
            properties.InflowModelType = newInflowModelType;
            properties.LoadSchematizationType = newLoadSchematizationType;
            properties.FailureProbabilityRepairClosure = "1e-2";
            properties.LevellingCount = newLevellingCount;
            properties.ProbabilityCollisionSecondaryStructure = "1e-3";
            properties.EvaluationLevel = (RoundedDouble) newEvaluationLevel;
            properties.VerticalDistance = (RoundedDouble) newVerticalDistance;

            // Assert
            var expectedFailureProbabilityRepairClosure = ProbabilityFormattingHelper.Format(0.01);
            var expectedProbabilityCollisionSecondaryStructure = ProbabilityFormattingHelper.Format(0.001);

            Assert.AreEqual(newVolumicWeightWater, properties.VolumicWeightWater, properties.VolumicWeightWater.GetAccuracy());
            Assert.AreEqual(newFactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure, properties.FactorStormDurationOpenStructure.GetAccuracy());
            Assert.AreEqual(newInflowModelType, properties.InflowModelType);
            Assert.AreEqual(newLoadSchematizationType, properties.LoadSchematizationType);
            Assert.AreEqual(expectedFailureProbabilityRepairClosure, properties.FailureProbabilityRepairClosure);
            Assert.AreEqual(newLevellingCount, properties.LevellingCount);
            Assert.AreEqual(expectedProbabilityCollisionSecondaryStructure, properties.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(newEvaluationLevel, properties.EvaluationLevel, properties.EvaluationLevel.GetAccuracy());
            Assert.AreEqual(newVerticalDistance, properties.VerticalDistance, properties.VerticalDistance.GetAccuracy());

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void SetFailureProbabilityRepairClosure_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityRepairClosure = newValue.ToString(CultureInfo.InvariantCulture);

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetFailureProbabilityRepairClosure_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityRepairClosure = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetFailureProbabilityRepairClosure_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.FailureProbabilityRepairClosure = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void SetProbabilityCollisionSecondaryStructure_InvalidValues_ThrowsArgumentException(double newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.ProbabilityCollisionSecondaryStructure = newValue.ToString(CultureInfo.InvariantCulture);

            // Assert
            var expectedMessage = "De waarde voor de faalkans is te groot of te klein.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase("no double value")]
        [TestCase("")]
        public void SetProbabilityCollisionSecondaryStructure_ValuesUnableToParse_ThrowsArgumentException(string newValue)
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.ProbabilityCollisionSecondaryStructure = newValue;

            // Assert
            var expectedMessage = "De waarde voor de faalkans kon niet geïnterpreteerd worden als een getal.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetProbabilityCollisionSecondaryStructure_NullValue_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var input = calculation.InputParameters;
            var inputContext = new StabilityPointStructuresInputContext(input,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call
            TestDelegate call = () => properties.ProbabilityCollisionSecondaryStructure = null;

            // Assert
            var expectedMessage = "De waarde voor de faalkans moet ingevuld zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_LinearLowSillStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";
            const string criticalValuesCategory = "Kritieke waarden";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[linear_LowSill_volumicWeightWaterPropertyIndex];
            Assert.IsFalse(volumicWeightWaterProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, volumicWeightWaterProperty.Category);
            Assert.AreEqual("Volumiek gewicht van water [kN/m³]", volumicWeightWaterProperty.DisplayName);
            Assert.AreEqual("Volumiek gewicht van water.", volumicWeightWaterProperty.Description);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[linear_LowSill_insideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[linear_LowSill_insideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelFailureConstructionProperty.Category);
            Assert.AreEqual("Binnenwaterstand bij constructief falen [m+NAP]", insideWaterLevelFailureConstructionProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand bij constructief falen.", insideWaterLevelFailureConstructionProperty.Description);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[linear_LowSill_flowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            Assert.AreEqual(criticalValuesCategory, flowVelocityStructureClosableProperty.Category);
            Assert.AreEqual("Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]", flowVelocityStructureClosableProperty.DisplayName);
            Assert.AreEqual("Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.", flowVelocityStructureClosableProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[linear_LowSill_factorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[linear_LowSill_inflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[linear_LowSill_loadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, loadSchematizationTypeProperty.Category);
            Assert.AreEqual("Belastingschematisering", loadSchematizationTypeProperty.DisplayName);
            Assert.AreEqual("Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.", loadSchematizationTypeProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[linear_LowSill_levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[linear_LowSill_thresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[linear_LowSill_constructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthLinearLoadModelProperty.Category);
            Assert.AreEqual("Lineaire belastingschematisering constructieve sterkte [kN/m²]", constructiveStrengthLinearLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke sterkte constructie volgens de lineaire belastingschematisatie.", constructiveStrengthLinearLoadModelProperty.Description);

            PropertyDescriptor bankWidthProperty = dynamicProperties[linear_LowSill_bankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            Assert.AreEqual(schematizationCategory, bankWidthProperty.Category);
            Assert.AreEqual("Bermbreedte [m]", bankWidthProperty.DisplayName);
            Assert.AreEqual("Bermbreedte.", bankWidthProperty.Description);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[linear_LowSill_evaluationLevelPropertyIndex];
            Assert.IsFalse(evaluationLevelProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, evaluationLevelProperty.Category);
            Assert.AreEqual("Analysehoogte [m+NAP]", evaluationLevelProperty.DisplayName);
            Assert.AreEqual("Hoogte waarop de constructieve sterkte wordt beoordeeld.", evaluationLevelProperty.Description);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[linear_LowSill_verticalDistancePropertyIndex];
            Assert.IsFalse(verticalDistanceProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, verticalDistanceProperty.Category);
            Assert.AreEqual("Afstand onderkant wand en teen van de dijk/berm [m]", verticalDistanceProperty.DisplayName);
            Assert.AreEqual("Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.", verticalDistanceProperty.Description);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[linear_LowSill_failureProbabilityRepairClosurePropertyIndex];
            Assert.IsFalse(failureProbabilityRepairClosureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityRepairClosureProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityRepairClosureProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityRepairClosureProperty.Description);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[linear_LowSill_failureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            Assert.AreEqual(schematizationCategory, failureCollisionEnergyProperty.Category);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie [kN m]", failureCollisionEnergyProperty.DisplayName);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie.", failureCollisionEnergyProperty.Description);

            PropertyDescriptor shipMassProperty = dynamicProperties[linear_LowSill_shipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipMassProperty.Category);
            Assert.AreEqual("Massa van het schip [ton]", shipMassProperty.DisplayName);
            Assert.AreEqual("Massa van het schip.", shipMassProperty.Description);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[linear_LowSill_shipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipVelocityProperty.Category);
            Assert.AreEqual("Aanvaarsnelheid [m/s]", shipVelocityProperty.DisplayName);
            Assert.AreEqual("Aanvaarsnelheid.", shipVelocityProperty.Description);

            PropertyDescriptor levellingCountProperty = dynamicProperties[linear_LowSill_levellingCountPropertyIndex];
            Assert.IsFalse(levellingCountProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, levellingCountProperty.Category);
            Assert.AreEqual("Aantal nivelleringen per jaar [1/jaar]", levellingCountProperty.DisplayName);
            Assert.AreEqual("Aantal nivelleringen per jaar.", levellingCountProperty.Description);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[linear_LowSill_probabilityCollisionSecondaryStructurePropertyIndex];
            Assert.IsFalse(probabilityCollisionSecondaryStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityCollisionSecondaryStructureProperty.Category);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]", probabilityCollisionSecondaryStructureProperty.DisplayName);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering.", probabilityCollisionSecondaryStructureProperty.Description);

            PropertyDescriptor stabilityLinearLoadModel = dynamicProperties[linear_LowSill_stabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModel.Converter);
            Assert.AreEqual(schematizationCategory, stabilityLinearLoadModel.Category);
            Assert.AreEqual("Lineaire belastingschematisering stabiliteit [kN/m²]", stabilityLinearLoadModel.DisplayName);
            Assert.AreEqual("Kritieke stabiliteit constructie volgens de lineaire belastingschematisatie.", stabilityLinearLoadModel.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[linear_LowSill_structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[linear_LowSill_structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[linear_LowSill_structureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[linear_LowSill_flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[linear_LowSill_widthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[linear_LowSill_storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[linear_LowSill_allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[linear_LowSill_criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[linear_LowSill_failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[linear_LowSill_modelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[linear_LowSill_foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[linear_LowSill_useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[linear_LowSill_useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[linear_LowSill_hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[linear_LowSill_stormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_QuadraticLowSillStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";
            const string criticalValuesCategory = "Kritieke waarden";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[quadratic_LowSill_volumicWeightWaterPropertyIndex];
            Assert.IsFalse(volumicWeightWaterProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, volumicWeightWaterProperty.Category);
            Assert.AreEqual("Volumiek gewicht van water [kN/m³]", volumicWeightWaterProperty.DisplayName);
            Assert.AreEqual("Volumiek gewicht van water.", volumicWeightWaterProperty.Description);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[quadratic_LowSill_insideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[quadratic_LowSill_insideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelFailureConstructionProperty.Category);
            Assert.AreEqual("Binnenwaterstand bij constructief falen [m+NAP]", insideWaterLevelFailureConstructionProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand bij constructief falen.", insideWaterLevelFailureConstructionProperty.Description);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[quadratic_LowSill_flowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            Assert.AreEqual(criticalValuesCategory, flowVelocityStructureClosableProperty.Category);
            Assert.AreEqual("Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]", flowVelocityStructureClosableProperty.DisplayName);
            Assert.AreEqual("Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.", flowVelocityStructureClosableProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[quadratic_LowSill_factorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[quadratic_LowSill_inflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[quadratic_LowSill_loadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, loadSchematizationTypeProperty.Category);
            Assert.AreEqual("Belastingschematisering", loadSchematizationTypeProperty.DisplayName);
            Assert.AreEqual("Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.", loadSchematizationTypeProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[quadratic_LowSill_levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[quadratic_LowSill_thresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor constructiveStrengthQuadraticLoadModelProperty = dynamicProperties[quadratic_LowSill_constructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthQuadraticLoadModelProperty.Category);
            Assert.AreEqual("Kwadratische belastingschematisering constructieve sterkte [kN/m]", constructiveStrengthQuadraticLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke sterkte constructie volgens de kwadratische belastingschematisatie.", constructiveStrengthQuadraticLoadModelProperty.Description);

            PropertyDescriptor bankWidthProperty = dynamicProperties[quadratic_LowSill_bankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            Assert.AreEqual(schematizationCategory, bankWidthProperty.Category);
            Assert.AreEqual("Bermbreedte [m]", bankWidthProperty.DisplayName);
            Assert.AreEqual("Bermbreedte.", bankWidthProperty.Description);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[quadratic_LowSill_evaluationLevelPropertyIndex];
            Assert.IsFalse(evaluationLevelProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, evaluationLevelProperty.Category);
            Assert.AreEqual("Analysehoogte [m+NAP]", evaluationLevelProperty.DisplayName);
            Assert.AreEqual("Hoogte waarop de constructieve sterkte wordt beoordeeld.", evaluationLevelProperty.Description);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[quadratic_LowSill_verticalDistancePropertyIndex];
            Assert.IsFalse(verticalDistanceProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, verticalDistanceProperty.Category);
            Assert.AreEqual("Afstand onderkant wand en teen van de dijk/berm [m]", verticalDistanceProperty.DisplayName);
            Assert.AreEqual("Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.", verticalDistanceProperty.Description);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[quadratic_LowSill_failureProbabilityRepairClosurePropertyIndex];
            Assert.IsFalse(failureProbabilityRepairClosureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityRepairClosureProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityRepairClosureProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityRepairClosureProperty.Description);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[quadratic_LowSill_failureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            Assert.AreEqual(schematizationCategory, failureCollisionEnergyProperty.Category);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie [kN m]", failureCollisionEnergyProperty.DisplayName);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie.", failureCollisionEnergyProperty.Description);

            PropertyDescriptor shipMassProperty = dynamicProperties[quadratic_LowSill_shipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipMassProperty.Category);
            Assert.AreEqual("Massa van het schip [ton]", shipMassProperty.DisplayName);
            Assert.AreEqual("Massa van het schip.", shipMassProperty.Description);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[quadratic_LowSill_shipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipVelocityProperty.Category);
            Assert.AreEqual("Aanvaarsnelheid [m/s]", shipVelocityProperty.DisplayName);
            Assert.AreEqual("Aanvaarsnelheid.", shipVelocityProperty.Description);

            PropertyDescriptor levellingCountProperty = dynamicProperties[quadratic_LowSill_levellingCountPropertyIndex];
            Assert.IsFalse(levellingCountProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, levellingCountProperty.Category);
            Assert.AreEqual("Aantal nivelleringen per jaar [1/jaar]", levellingCountProperty.DisplayName);
            Assert.AreEqual("Aantal nivelleringen per jaar.", levellingCountProperty.Description);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[quadratic_LowSill_probabilityCollisionSecondaryStructurePropertyIndex];
            Assert.IsFalse(probabilityCollisionSecondaryStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityCollisionSecondaryStructureProperty.Category);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]", probabilityCollisionSecondaryStructureProperty.DisplayName);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering.", probabilityCollisionSecondaryStructureProperty.Description);

            PropertyDescriptor stabilityQuadraticLoadModelProperty = dynamicProperties[quadratic_LowSill_stabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, stabilityQuadraticLoadModelProperty.Category);
            Assert.AreEqual("Kwadratische belastingschematisering stabiliteit [kN/m]", stabilityQuadraticLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke stabiliteit constructie volgens de kwadratische belastingschematisatie.", stabilityQuadraticLoadModelProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[quadratic_LowSill_structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[quadratic_LowSill_structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[quadratic_LowSill_structureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[quadratic_LowSill_flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Breedte van doorstroomopening [m]", dynamicProperties[quadratic_LowSill_widthFlowAperturesPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[quadratic_LowSill_storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[quadratic_LowSill_allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[quadratic_LowSill_criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[quadratic_LowSill_failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Modelfactor overloopdebiet volkomen overlaat [-]", dynamicProperties[quadratic_LowSill_modelFactorSuperCriticalFlowPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[quadratic_LowSill_foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[quadratic_LowSill_useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[quadratic_LowSill_useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[quadratic_LowSill_hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[quadratic_LowSill_stormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_LinearFloodedCulvertStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";
            const string criticalValuesCategory = "Kritieke waarden";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[linear_FloodedCulvert_volumicWeightWaterPropertyIndex];
            Assert.IsFalse(volumicWeightWaterProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, volumicWeightWaterProperty.Category);
            Assert.AreEqual("Volumiek gewicht van water [kN/m³]", volumicWeightWaterProperty.DisplayName);
            Assert.AreEqual("Volumiek gewicht van water.", volumicWeightWaterProperty.Description);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[linear_FloodedCulvert_insideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[linear_FloodedCulvert_insideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelFailureConstructionProperty.Category);
            Assert.AreEqual("Binnenwaterstand bij constructief falen [m+NAP]", insideWaterLevelFailureConstructionProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand bij constructief falen.", insideWaterLevelFailureConstructionProperty.Description);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[linear_FloodedCulvert_flowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            Assert.AreEqual(criticalValuesCategory, flowVelocityStructureClosableProperty.Category);
            Assert.AreEqual("Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]", flowVelocityStructureClosableProperty.DisplayName);
            Assert.AreEqual("Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.", flowVelocityStructureClosableProperty.Description);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[linear_FloodedCulvert_drainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            Assert.AreEqual(modelSettingsCategory, drainCoefficientProperty.Category);
            Assert.AreEqual("Afvoercoëfficient [-]", drainCoefficientProperty.DisplayName);
            Assert.AreEqual("Afvoercoëfficient.", drainCoefficientProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[linear_FloodedCulvert_factorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[linear_FloodedCulvert_inflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[linear_FloodedCulvert_loadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, loadSchematizationTypeProperty.Category);
            Assert.AreEqual("Belastingschematisering", loadSchematizationTypeProperty.DisplayName);
            Assert.AreEqual("Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.", loadSchematizationTypeProperty.Description);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[linear_FloodedCulvert_areaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, areaFlowAperturesProperty.Category);
            Assert.AreEqual("Doorstroomoppervlak [m²]", areaFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Doorstroomoppervlak van doorstroomopeningen.", areaFlowAperturesProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[linear_FloodedCulvert_levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[linear_FloodedCulvert_thresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor constructiveStrengthLinearLoadModelProperty = dynamicProperties[linear_FloodedCulvert_constructiveStrengthLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthLinearLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthLinearLoadModelProperty.Category);
            Assert.AreEqual("Lineaire belastingschematisering constructieve sterkte [kN/m²]", constructiveStrengthLinearLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke sterkte constructie volgens de lineaire belastingschematisatie.", constructiveStrengthLinearLoadModelProperty.Description);

            PropertyDescriptor bankWidthProperty = dynamicProperties[linear_FloodedCulvert_bankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            Assert.AreEqual(schematizationCategory, bankWidthProperty.Category);
            Assert.AreEqual("Bermbreedte [m]", bankWidthProperty.DisplayName);
            Assert.AreEqual("Bermbreedte.", bankWidthProperty.Description);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[linear_FloodedCulvert_evaluationLevelPropertyIndex];
            Assert.IsFalse(evaluationLevelProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, evaluationLevelProperty.Category);
            Assert.AreEqual("Analysehoogte [m+NAP]", evaluationLevelProperty.DisplayName);
            Assert.AreEqual("Hoogte waarop de constructieve sterkte wordt beoordeeld.", evaluationLevelProperty.Description);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[linear_FloodedCulvert_verticalDistancePropertyIndex];
            Assert.IsFalse(verticalDistanceProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, verticalDistanceProperty.Category);
            Assert.AreEqual("Afstand onderkant wand en teen van de dijk/berm [m]", verticalDistanceProperty.DisplayName);
            Assert.AreEqual("Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.", verticalDistanceProperty.Description);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[linear_FloodedCulvert_failureProbabilityRepairClosurePropertyIndex];
            Assert.IsFalse(failureProbabilityRepairClosureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityRepairClosureProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityRepairClosureProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityRepairClosureProperty.Description);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[linear_FloodedCulvert_failureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            Assert.AreEqual(schematizationCategory, failureCollisionEnergyProperty.Category);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie [kN m]", failureCollisionEnergyProperty.DisplayName);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie.", failureCollisionEnergyProperty.Description);

            PropertyDescriptor shipMassProperty = dynamicProperties[linear_FloodedCulvert_shipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipMassProperty.Category);
            Assert.AreEqual("Massa van het schip [ton]", shipMassProperty.DisplayName);
            Assert.AreEqual("Massa van het schip.", shipMassProperty.Description);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[linear_FloodedCulvert_shipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipVelocityProperty.Category);
            Assert.AreEqual("Aanvaarsnelheid [m/s]", shipVelocityProperty.DisplayName);
            Assert.AreEqual("Aanvaarsnelheid.", shipVelocityProperty.Description);

            PropertyDescriptor levellingCountProperty = dynamicProperties[linear_FloodedCulvert_levellingCountPropertyIndex];
            Assert.IsFalse(levellingCountProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, levellingCountProperty.Category);
            Assert.AreEqual("Aantal nivelleringen per jaar [1/jaar]", levellingCountProperty.DisplayName);
            Assert.AreEqual("Aantal nivelleringen per jaar.", levellingCountProperty.Description);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[linear_FloodedCulvert_probabilityCollisionSecondaryStructurePropertyIndex];
            Assert.IsFalse(probabilityCollisionSecondaryStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityCollisionSecondaryStructureProperty.Category);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]", probabilityCollisionSecondaryStructureProperty.DisplayName);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering.", probabilityCollisionSecondaryStructureProperty.Description);

            PropertyDescriptor stabilityLinearLoadModel = dynamicProperties[linear_FloodedCulvert_stabilityLinearLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityLinearLoadModel.Converter);
            Assert.AreEqual(schematizationCategory, stabilityLinearLoadModel.Category);
            Assert.AreEqual("Lineaire belastingschematisering stabiliteit [kN/m²]", stabilityLinearLoadModel.DisplayName);
            Assert.AreEqual("Kritieke stabiliteit constructie volgens de lineaire belastingschematisatie.", stabilityLinearLoadModel.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[linear_FloodedCulvert_structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[linear_FloodedCulvert_structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[linear_FloodedCulvert_structureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[linear_FloodedCulvert_flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[linear_FloodedCulvert_storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[linear_FloodedCulvert_allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[linear_FloodedCulvert_criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[linear_FloodedCulvert_failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[linear_FloodedCulvert_foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[linear_FloodedCulvert_useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[linear_FloodedCulvert_useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[linear_FloodedCulvert_hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[linear_FloodedCulvert_stormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_QuadraticFloodedCulvertStructure_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            // Call
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Assert
            const string schematizationCategory = "Schematisatie";
            const string hydraulicDataCategory = "Hydraulische gegevens";
            const string modelSettingsCategory = "Modelinstellingen";
            const string criticalValuesCategory = "Kritieke waarden";

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });
            Assert.AreEqual(35, dynamicProperties.Count);

            PropertyDescriptor volumicWeightWaterProperty = dynamicProperties[quadratic_FloodedCulvert_volumicWeightWaterPropertyIndex];
            Assert.IsFalse(volumicWeightWaterProperty.IsReadOnly);
            Assert.AreEqual(hydraulicDataCategory, volumicWeightWaterProperty.Category);
            Assert.AreEqual("Volumiek gewicht van water [kN/m³]", volumicWeightWaterProperty.DisplayName);
            Assert.AreEqual("Volumiek gewicht van water.", volumicWeightWaterProperty.Description);

            PropertyDescriptor insideWaterLevelProperty = dynamicProperties[quadratic_FloodedCulvert_insideWaterLevelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelProperty.Category);
            Assert.AreEqual("Binnenwaterstand [m+NAP]", insideWaterLevelProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand.", insideWaterLevelProperty.Description);

            PropertyDescriptor insideWaterLevelFailureConstructionProperty = dynamicProperties[quadratic_FloodedCulvert_insideWaterLevelFailureConstructionPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(insideWaterLevelFailureConstructionProperty.Converter);
            Assert.AreEqual(hydraulicDataCategory, insideWaterLevelFailureConstructionProperty.Category);
            Assert.AreEqual("Binnenwaterstand bij constructief falen [m+NAP]", insideWaterLevelFailureConstructionProperty.DisplayName);
            Assert.AreEqual("Binnenwaterstand bij constructief falen.", insideWaterLevelFailureConstructionProperty.Description);

            PropertyDescriptor flowVelocityStructureClosableProperty = dynamicProperties[quadratic_FloodedCulvert_flowVelocityStructureClosablePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(flowVelocityStructureClosableProperty.Converter);
            Assert.AreEqual(criticalValuesCategory, flowVelocityStructureClosableProperty.Category);
            Assert.AreEqual("Kritieke stroomsnelheid sluiting eerste keermiddel [m/s]", flowVelocityStructureClosableProperty.DisplayName);
            Assert.AreEqual("Stroomsnelheid waarbij na aanvaring het eerste keermiddel nog net kan worden gesloten.", flowVelocityStructureClosableProperty.Description);

            PropertyDescriptor drainCoefficientProperty = dynamicProperties[quadratic_FloodedCulvert_drainCoefficientPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(drainCoefficientProperty.Converter);
            Assert.AreEqual(modelSettingsCategory, drainCoefficientProperty.Category);
            Assert.AreEqual("Afvoercoëfficient [-]", drainCoefficientProperty.DisplayName);
            Assert.AreEqual("Afvoercoëfficient.", drainCoefficientProperty.Description);

            PropertyDescriptor factorStormDurationOpenStructureProperty = dynamicProperties[quadratic_FloodedCulvert_factorStormDurationOpenStructurePropertyIndex];
            Assert.IsFalse(factorStormDurationOpenStructureProperty.IsReadOnly);
            Assert.AreEqual(modelSettingsCategory, factorStormDurationOpenStructureProperty.Category);
            Assert.AreEqual("Factor voor stormduur hoogwater [-]", factorStormDurationOpenStructureProperty.DisplayName);
            Assert.AreEqual("Factor voor stormduur hoogwater gegeven geopend kunstwerk.", factorStormDurationOpenStructureProperty.Description);

            PropertyDescriptor inflowModelTypeProperty = dynamicProperties[quadratic_FloodedCulvert_inflowModelTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(inflowModelTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, inflowModelTypeProperty.Category);
            Assert.AreEqual("Instroommodel", inflowModelTypeProperty.DisplayName);
            Assert.AreEqual("Instroommodel van het kunstwerk.", inflowModelTypeProperty.Description);

            PropertyDescriptor loadSchematizationTypeProperty = dynamicProperties[quadratic_FloodedCulvert_loadSchematizationTypePropertyIndex];
            Assert.IsInstanceOf<EnumConverter>(loadSchematizationTypeProperty.Converter);
            Assert.AreEqual(schematizationCategory, loadSchematizationTypeProperty.Category);
            Assert.AreEqual("Belastingschematisering", loadSchematizationTypeProperty.DisplayName);
            Assert.AreEqual("Geeft aan of het lineaire belastingmodel of het kwadratische belastingmodel moet worden gebruikt.", loadSchematizationTypeProperty.Description);

            PropertyDescriptor areaFlowAperturesProperty = dynamicProperties[quadratic_FloodedCulvert_areaFlowAperturesPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(areaFlowAperturesProperty.Converter);
            Assert.AreEqual(schematizationCategory, areaFlowAperturesProperty.Category);
            Assert.AreEqual("Doorstroomoppervlak [m²]", areaFlowAperturesProperty.DisplayName);
            Assert.AreEqual("Doorstroomoppervlak van doorstroomopeningen.", areaFlowAperturesProperty.Description);

            PropertyDescriptor levelCrestStructureProperty = dynamicProperties[quadratic_FloodedCulvert_levelCrestStructurePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(levelCrestStructureProperty.Converter);
            Assert.AreEqual(schematizationCategory, levelCrestStructureProperty.Category);
            Assert.AreEqual("Kerende hoogte [m+NAP]", levelCrestStructureProperty.DisplayName);
            Assert.AreEqual("Kerende hoogte van het kunstwerk.", levelCrestStructureProperty.Description);

            PropertyDescriptor thresholdHeightOpenWeirProperty = dynamicProperties[quadratic_FloodedCulvert_thresholdHeightOpenWeirPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(thresholdHeightOpenWeirProperty.Converter);
            Assert.AreEqual(schematizationCategory, thresholdHeightOpenWeirProperty.Category);
            Assert.AreEqual("Drempelhoogte [m+NAP]", thresholdHeightOpenWeirProperty.DisplayName);
            Assert.AreEqual("Drempelhoogte niet gesloten kering of hoogte van de onderkant van de wand/drempel.", thresholdHeightOpenWeirProperty.Description);

            PropertyDescriptor constructiveStrengthQuadraticLoadModelProperty = dynamicProperties[quadratic_FloodedCulvert_constructiveStrengthQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(constructiveStrengthQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, constructiveStrengthQuadraticLoadModelProperty.Category);
            Assert.AreEqual("Kwadratische belastingschematisering constructieve sterkte [kN/m]", constructiveStrengthQuadraticLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke sterkte constructie volgens de kwadratische belastingschematisatie.", constructiveStrengthQuadraticLoadModelProperty.Description);

            PropertyDescriptor bankWidthProperty = dynamicProperties[quadratic_FloodedCulvert_bankWidthPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(bankWidthProperty.Converter);
            Assert.AreEqual(schematizationCategory, bankWidthProperty.Category);
            Assert.AreEqual("Bermbreedte [m]", bankWidthProperty.DisplayName);
            Assert.AreEqual("Bermbreedte.", bankWidthProperty.Description);

            PropertyDescriptor evaluationLevelProperty = dynamicProperties[quadratic_FloodedCulvert_evaluationLevelPropertyIndex];
            Assert.IsFalse(evaluationLevelProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, evaluationLevelProperty.Category);
            Assert.AreEqual("Analysehoogte [m+NAP]", evaluationLevelProperty.DisplayName);
            Assert.AreEqual("Hoogte waarop de constructieve sterkte wordt beoordeeld.", evaluationLevelProperty.Description);

            PropertyDescriptor verticalDistanceProperty = dynamicProperties[quadratic_FloodedCulvert_verticalDistancePropertyIndex];
            Assert.IsFalse(verticalDistanceProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, verticalDistanceProperty.Category);
            Assert.AreEqual("Afstand onderkant wand en teen van de dijk/berm [m]", verticalDistanceProperty.DisplayName);
            Assert.AreEqual("Verticale afstand tussen de onderkant van de wand en de teen van de dijk/berm.", verticalDistanceProperty.Description);

            PropertyDescriptor failureProbabilityRepairClosureProperty = dynamicProperties[quadratic_FloodedCulvert_failureProbabilityRepairClosurePropertyIndex];
            Assert.IsFalse(failureProbabilityRepairClosureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, failureProbabilityRepairClosureProperty.Category);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie [1/jaar]", failureProbabilityRepairClosureProperty.DisplayName);
            Assert.AreEqual("Faalkans herstel van gefaalde situatie.", failureProbabilityRepairClosureProperty.Description);

            PropertyDescriptor failureCollisionEnergyProperty = dynamicProperties[quadratic_FloodedCulvert_failureCollisionEnergyPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(failureCollisionEnergyProperty.Converter);
            Assert.AreEqual(schematizationCategory, failureCollisionEnergyProperty.Category);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie [kN m]", failureCollisionEnergyProperty.DisplayName);
            Assert.AreEqual("Bezwijkwaarde aanvaarenergie.", failureCollisionEnergyProperty.Description);

            PropertyDescriptor shipMassProperty = dynamicProperties[quadratic_FloodedCulvert_shipMassPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipMassProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipMassProperty.Category);
            Assert.AreEqual("Massa van het schip [ton]", shipMassProperty.DisplayName);
            Assert.AreEqual("Massa van het schip.", shipMassProperty.Description);

            PropertyDescriptor shipVelocityProperty = dynamicProperties[quadratic_FloodedCulvert_shipVelocityPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(shipVelocityProperty.Converter);
            Assert.AreEqual(schematizationCategory, shipVelocityProperty.Category);
            Assert.AreEqual("Aanvaarsnelheid [m/s]", shipVelocityProperty.DisplayName);
            Assert.AreEqual("Aanvaarsnelheid.", shipVelocityProperty.Description);

            PropertyDescriptor levellingCountProperty = dynamicProperties[quadratic_FloodedCulvert_levellingCountPropertyIndex];
            Assert.IsFalse(levellingCountProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, levellingCountProperty.Category);
            Assert.AreEqual("Aantal nivelleringen per jaar [1/jaar]", levellingCountProperty.DisplayName);
            Assert.AreEqual("Aantal nivelleringen per jaar.", levellingCountProperty.Description);

            PropertyDescriptor probabilityCollisionSecondaryStructureProperty = dynamicProperties[quadratic_FloodedCulvert_probabilityCollisionSecondaryStructurePropertyIndex];
            Assert.IsFalse(probabilityCollisionSecondaryStructureProperty.IsReadOnly);
            Assert.AreEqual(schematizationCategory, probabilityCollisionSecondaryStructureProperty.Category);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering [1/jaar/niv]", probabilityCollisionSecondaryStructureProperty.DisplayName);
            Assert.AreEqual("Kans op aanvaring tweede keermiddel per nivellering.", probabilityCollisionSecondaryStructureProperty.Description);

            PropertyDescriptor stabilityQuadraticLoadModelProperty = dynamicProperties[quadratic_FloodedCulvert_stabilityQuadraticLoadModelPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(stabilityQuadraticLoadModelProperty.Converter);
            Assert.AreEqual(schematizationCategory, stabilityQuadraticLoadModelProperty.Category);
            Assert.AreEqual("Kwadratische belastingschematisering stabiliteit [kN/m]", stabilityQuadraticLoadModelProperty.DisplayName);
            Assert.AreEqual("Kritieke stabiliteit constructie volgens de kwadratische belastingschematisatie.", stabilityQuadraticLoadModelProperty.Description);

            // Only check the order of the base properties
            Assert.AreEqual("Kunstwerk", dynamicProperties[quadratic_FloodedCulvert_structurePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie (RD) [m]", dynamicProperties[quadratic_FloodedCulvert_structureLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Oriëntatie [°]", dynamicProperties[quadratic_FloodedCulvert_structureNormalOrientationPropertyIndex].DisplayName);
            Assert.AreEqual("Stroomvoerende breedte bodembescherming [m]", dynamicProperties[quadratic_FloodedCulvert_flowWidthAtBottomProtectionPropertyIndex].DisplayName);
            Assert.AreEqual("Kombergend oppervlak [m²]", dynamicProperties[quadratic_FloodedCulvert_storageStructureAreaPropertyIndex].DisplayName);
            Assert.AreEqual("Toegestane peilverhoging komberging [m]", dynamicProperties[quadratic_FloodedCulvert_allowedLevelIncreaseStoragePropertyIndex].DisplayName);
            Assert.AreEqual("Kritiek instromend debiet [m³/s/m]", dynamicProperties[quadratic_FloodedCulvert_criticalOvertoppingDischargePropertyIndex].DisplayName);
            Assert.AreEqual("Faalkans gegeven erosie bodem [1/jaar]", dynamicProperties[quadratic_FloodedCulvert_failureProbabilityStructureWithErosionPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandprofiel", dynamicProperties[quadratic_FloodedCulvert_foreshoreProfilePropertyIndex].DisplayName);
            Assert.AreEqual("Dam", dynamicProperties[quadratic_FloodedCulvert_useBreakWaterPropertyIndex].DisplayName);
            Assert.AreEqual("Voorlandgeometrie", dynamicProperties[quadratic_FloodedCulvert_useForeshorePropertyIndex].DisplayName);
            Assert.AreEqual("Locatie met hydraulische randvoorwaarden", dynamicProperties[quadratic_FloodedCulvert_hydraulicBoundaryLocationPropertyIndex].DisplayName);
            Assert.AreEqual("Stormduur [uur]", dynamicProperties[quadratic_FloodedCulvert_stormDurationPropertyIndex].DisplayName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void SetStructure_StructureInSection_UpdateSectionResults()
        {
            // Setup
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);
            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            failureMechanism.AddSection(new FailureMechanismSection("Section", new List<Point2D>
            {
                new Point2D(-10.0, -10.0),
                new Point2D(10.0, 10.0)
            }));

            // Call
            properties.Structure = new TestStabilityPointStructure();

            // Assert
            Assert.AreSame(calculation, failureMechanism.SectionResults.ElementAt(0).Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void DynamicVisibileValidationMethod_LowSillStructure_ReturnExpectedValues(LoadSchematizationType schematizationType)
        {
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = schematizationType
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.DrainCoefficient)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.AreaFlowApertures)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear)]
        [TestCase(LoadSchematizationType.Quadratic)]
        public void DynamicVisibileValidationMethod_FloodedCulvertStructure_ReturnExpectedValues(LoadSchematizationType schematizationType)
        {
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = StabilityPointStructureInflowModelType.FloodedCulvert,
                    LoadSchematizationType = schematizationType
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.DrainCoefficient)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.AreaFlowApertures)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.WidthFlowApertures)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        public void DynamicVisibileValidationMethod_LinearModel_ReturnExpectedValues(StabilityPointStructureInflowModelType structureType)
        {
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = structureType,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthLinearLoadModel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthQuadraticLoadModel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityLinearLoadModel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityQuadraticLoadModel)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill)]
        public void DynamicVisibileValidationMethod_QuadraticModel_ReturnExpectedValues(StabilityPointStructureInflowModelType structureType)
        {
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    InflowModelType = structureType,
                    LoadSchematizationType = LoadSchematizationType.Quadratic
                }
            };
            var inputContext = new StabilityPointStructuresInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        failureMechanism,
                                                                        assessmentSectionStub);

            var properties = new StabilityPointStructuresInputContextProperties
            {
                Data = inputContext
            };

            // Call & Assert
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthLinearLoadModel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthQuadraticLoadModel)));
            Assert.IsFalse(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityLinearLoadModel)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityQuadraticLoadModel)));

            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}