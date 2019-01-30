// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.StabilityPointStructures;

namespace Riskeer.Storage.Core.Test.Read.StabilityPointStructures
{
    [TestFixture]
    public class StabilityPointStructuresCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new StabilityPointStructuresCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Read_EntityNotReadBefore_RegisterEntity()
        {
            // Setup
            var entity = new StabilityPointStructuresCalculationEntity();

            var collector = new ReadConversionCollector();

            // Precondition
            Assert.IsFalse(collector.Contains(entity));

            // Call
            StructuresCalculation<StabilityPointStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(entity));
            Assert.AreSame(calculation, collector.Get(entity));
        }

        [Test]
        public void Read_ValidEntity_ReturnStabilityPointStructuresCalculation()
        {
            // Setup
            var entity = new StabilityPointStructuresCalculationEntity
            {
                Name = "name",
                Comments = "comments",
                StructureNormalOrientation = 1.1,
                AllowedLevelIncreaseStorageMean = 2.2,
                AllowedLevelIncreaseStorageStandardDeviation = 3.3,
                StorageStructureAreaMean = 4.4,
                StorageStructureAreaCoefficientOfVariation = 5.5,
                FlowWidthAtBottomProtectionMean = 6.6,
                FlowWidthAtBottomProtectionStandardDeviation = 7.7,
                CriticalOvertoppingDischargeMean = 8.8,
                CriticalOvertoppingDischargeCoefficientOfVariation = 9.9,
                FailureProbabilityStructureWithErosion = 0.10,
                WidthFlowAperturesMean = 11.11,
                WidthFlowAperturesStandardDeviation = 12.12,
                StormDurationMean = 13.13,
                UseBreakWater = Convert.ToByte(true),
                BreakWaterType = Convert.ToByte(BreakWaterType.Wall),
                BreakWaterHeight = 14.14,
                UseForeshore = Convert.ToByte(true),
                InsideWaterLevelMean = 15.15,
                InsideWaterLevelStandardDeviation = 16.16,
                ThresholdHeightOpenWeirMean = 17.17,
                ThresholdHeightOpenWeirStandardDeviation = 18.18,
                ConstructiveStrengthLinearLoadModelMean = 19.19,
                ConstructiveStrengthLinearLoadModelCoefficientOfVariation = 20.20,
                ConstructiveStrengthQuadraticLoadModelMean = 21.21,
                ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation = 22.22,
                BankWidthMean = 23.23,
                BankWidthStandardDeviation = 24.24,
                InsideWaterLevelFailureConstructionMean = 25.25,
                InsideWaterLevelFailureConstructionStandardDeviation = 26.26,
                EvaluationLevel = 27.27,
                LevelCrestStructureMean = 28.28,
                LevelCrestStructureStandardDeviation = 29.29,
                VerticalDistance = 30.30,
                FailureProbabilityRepairClosure = 0.31,
                FailureCollisionEnergyMean = 32.32,
                FailureCollisionEnergyCoefficientOfVariation = 33.33,
                ShipMassMean = 34.34,
                ShipMassCoefficientOfVariation = 35.35,
                ShipVelocityMean = 36.36,
                ShipVelocityCoefficientOfVariation = 37.37,
                LevellingCount = 38,
                ProbabilityCollisionSecondaryStructure = 0.39,
                FlowVelocityStructureClosableMean = 40.40,
                StabilityLinearLoadModelMean = 41.41,
                StabilityLinearLoadModelCoefficientOfVariation = 42.42,
                StabilityQuadraticLoadModelMean = 43.43,
                StabilityQuadraticLoadModelCoefficientOfVariation = 44.44,
                AreaFlowAperturesMean = 45.45,
                AreaFlowAperturesStandardDeviation = 46.46,
                InflowModelType = Convert.ToByte(StabilityPointStructureInflowModelType.FloodedCulvert),
                LoadSchematizationType = Convert.ToByte(LoadSchematizationType.Quadratic),
                VolumicWeightWater = 47.47,
                FactorStormDurationOpenStructure = 48.48,
                DrainCoefficientMean = 49.49
            };
            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculation<StabilityPointStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Name, calculation.Name);
            Assert.AreEqual(entity.Comments, calculation.Comments.Body);

            StabilityPointStructuresInput inputParameters = calculation.InputParameters;
            Assert.IsNull(inputParameters.ForeshoreProfile);
            Assert.IsNull(inputParameters.Structure);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);
            Assert.AreEqual(entity.StructureNormalOrientation, inputParameters.StructureNormalOrientation.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageMean, inputParameters.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageStandardDeviation, inputParameters.AllowedLevelIncreaseStorage.StandardDeviation.Value);
            Assert.AreEqual(entity.StorageStructureAreaMean, inputParameters.StorageStructureArea.Mean.Value);
            Assert.AreEqual(entity.StorageStructureAreaCoefficientOfVariation, inputParameters.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionMean, inputParameters.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionStandardDeviation, inputParameters.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeMean, inputParameters.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeCoefficientOfVariation, inputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.FailureProbabilityStructureWithErosion, inputParameters.FailureProbabilityStructureWithErosion);
            Assert.AreEqual(entity.WidthFlowAperturesMean, inputParameters.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(entity.WidthFlowAperturesStandardDeviation, inputParameters.WidthFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(entity.StormDurationMean, inputParameters.StormDuration.Mean.Value);
            Assert.AreEqual(Convert.ToBoolean(entity.UseBreakWater), inputParameters.UseBreakWater);
            Assert.AreEqual((BreakWaterType) entity.BreakWaterType, inputParameters.BreakWater.Type);
            Assert.AreEqual(entity.BreakWaterHeight, inputParameters.BreakWater.Height.Value);
            Assert.AreEqual(Convert.ToBoolean(entity.UseForeshore), inputParameters.UseForeshore);

            Assert.AreEqual(entity.InsideWaterLevelMean, inputParameters.InsideWaterLevel.Mean.Value);
            Assert.AreEqual(entity.InsideWaterLevelStandardDeviation, inputParameters.InsideWaterLevel.StandardDeviation.Value);
            Assert.AreEqual(entity.ThresholdHeightOpenWeirMean, inputParameters.ThresholdHeightOpenWeir.Mean.Value);
            Assert.AreEqual(entity.ThresholdHeightOpenWeirStandardDeviation, inputParameters.ThresholdHeightOpenWeir.StandardDeviation.Value);
            Assert.AreEqual(entity.ConstructiveStrengthLinearLoadModelMean, inputParameters.ConstructiveStrengthLinearLoadModel.Mean.Value);
            Assert.AreEqual(entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation, inputParameters.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.ConstructiveStrengthQuadraticLoadModelMean, inputParameters.ConstructiveStrengthQuadraticLoadModel.Mean.Value);
            Assert.AreEqual(entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation, inputParameters.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.BankWidthMean, inputParameters.BankWidth.Mean.Value);
            Assert.AreEqual(entity.BankWidthStandardDeviation, inputParameters.BankWidth.StandardDeviation.Value);
            Assert.AreEqual(entity.InsideWaterLevelFailureConstructionMean, inputParameters.InsideWaterLevelFailureConstruction.Mean.Value);
            Assert.AreEqual(entity.InsideWaterLevelFailureConstructionStandardDeviation, inputParameters.InsideWaterLevelFailureConstruction.StandardDeviation.Value);
            Assert.AreEqual(entity.EvaluationLevel, inputParameters.EvaluationLevel.Value);
            Assert.AreEqual(entity.LevelCrestStructureMean, inputParameters.LevelCrestStructure.Mean.Value);
            Assert.AreEqual(entity.LevelCrestStructureStandardDeviation, inputParameters.LevelCrestStructure.StandardDeviation.Value);
            Assert.AreEqual(entity.VerticalDistance, inputParameters.VerticalDistance.Value);
            Assert.AreEqual(entity.FailureProbabilityRepairClosure, inputParameters.FailureProbabilityRepairClosure);
            Assert.AreEqual(entity.FailureCollisionEnergyMean, inputParameters.FailureCollisionEnergy.Mean.Value);
            Assert.AreEqual(entity.FailureCollisionEnergyCoefficientOfVariation, inputParameters.FailureCollisionEnergy.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.ShipMassMean, inputParameters.ShipMass.Mean.Value);
            Assert.AreEqual(entity.ShipMassCoefficientOfVariation, inputParameters.ShipMass.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.ShipVelocityMean, inputParameters.ShipVelocity.Mean.Value);
            Assert.AreEqual(entity.ShipVelocityCoefficientOfVariation, inputParameters.ShipVelocity.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.LevellingCount, inputParameters.LevellingCount);
            Assert.AreEqual(entity.ProbabilityCollisionSecondaryStructure, inputParameters.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(entity.FlowVelocityStructureClosableMean, inputParameters.FlowVelocityStructureClosable.Mean.Value);
            Assert.AreEqual(entity.StabilityLinearLoadModelMean, inputParameters.StabilityLinearLoadModel.Mean.Value);
            Assert.AreEqual(entity.StabilityLinearLoadModelCoefficientOfVariation, inputParameters.StabilityLinearLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.StabilityQuadraticLoadModelMean, inputParameters.StabilityQuadraticLoadModel.Mean.Value);
            Assert.AreEqual(entity.StabilityQuadraticLoadModelCoefficientOfVariation, inputParameters.StabilityQuadraticLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.AreaFlowAperturesMean, inputParameters.AreaFlowApertures.Mean.Value);
            Assert.AreEqual(entity.AreaFlowAperturesStandardDeviation, inputParameters.AreaFlowApertures.StandardDeviation.Value);
            Assert.AreEqual((StabilityPointStructureInflowModelType) entity.InflowModelType, inputParameters.InflowModelType);
            Assert.AreEqual((LoadSchematizationType) entity.LoadSchematizationType, inputParameters.LoadSchematizationType);
            Assert.AreEqual(entity.VolumicWeightWater, inputParameters.VolumicWeightWater.Value);
            Assert.AreEqual(entity.FactorStormDurationOpenStructure, inputParameters.FactorStormDurationOpenStructure.Value);
            Assert.AreEqual(entity.DrainCoefficientMean, inputParameters.DrainCoefficient.Mean.Value);
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void Read_EntityWithNullParameters_ReturnStabilityPointStructuresCalculationWithInputParametersNaN()
        {
            // Setup
            var entity = new StabilityPointStructuresCalculationEntity
            {
                StructureNormalOrientation = null,
                AllowedLevelIncreaseStorageMean = null,
                AllowedLevelIncreaseStorageStandardDeviation = null,
                StorageStructureAreaMean = null,
                StorageStructureAreaCoefficientOfVariation = null,
                FlowWidthAtBottomProtectionMean = null,
                FlowWidthAtBottomProtectionStandardDeviation = null,
                CriticalOvertoppingDischargeMean = null,
                CriticalOvertoppingDischargeCoefficientOfVariation = null,
                WidthFlowAperturesMean = null,
                WidthFlowAperturesStandardDeviation = null,
                StormDurationMean = null,
                BreakWaterHeight = null,
                InsideWaterLevelMean = null,
                InsideWaterLevelStandardDeviation = null,
                ThresholdHeightOpenWeirMean = null,
                ThresholdHeightOpenWeirStandardDeviation = null,
                ConstructiveStrengthLinearLoadModelMean = null,
                ConstructiveStrengthLinearLoadModelCoefficientOfVariation = null,
                ConstructiveStrengthQuadraticLoadModelMean = null,
                ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation = null,
                BankWidthMean = null,
                BankWidthStandardDeviation = null,
                InsideWaterLevelFailureConstructionMean = null,
                InsideWaterLevelFailureConstructionStandardDeviation = null,
                EvaluationLevel = null,
                LevelCrestStructureMean = null,
                LevelCrestStructureStandardDeviation = null,
                VerticalDistance = null,
                FailureCollisionEnergyMean = null,
                FailureCollisionEnergyCoefficientOfVariation = null,
                ShipMassMean = null,
                ShipMassCoefficientOfVariation = null,
                ShipVelocityMean = null,
                ShipVelocityCoefficientOfVariation = null,
                FlowVelocityStructureClosableMean = null,
                StabilityLinearLoadModelMean = null,
                StabilityLinearLoadModelCoefficientOfVariation = null,
                StabilityQuadraticLoadModelMean = null,
                StabilityQuadraticLoadModelCoefficientOfVariation = null,
                AreaFlowAperturesMean = null,
                AreaFlowAperturesStandardDeviation = null,
                VolumicWeightWater = null,
                FactorStormDurationOpenStructure = null,
                DrainCoefficientMean = null
            };
            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculation<StabilityPointStructuresInput> calculation = entity.Read(collector);

            // Assert
            StabilityPointStructuresInput inputParameters = calculation.InputParameters;
            Assert.IsNaN(inputParameters.StructureNormalOrientation);
            Assert.IsNaN(inputParameters.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(inputParameters.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNaN(inputParameters.StorageStructureArea.Mean);
            Assert.IsNaN(inputParameters.StorageStructureArea.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(inputParameters.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNaN(inputParameters.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(inputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.WidthFlowApertures.Mean);
            Assert.IsNaN(inputParameters.WidthFlowApertures.StandardDeviation);
            Assert.IsNaN(inputParameters.StormDuration.Mean);
            Assert.IsNaN(inputParameters.BreakWater.Height);

            Assert.IsNaN(inputParameters.InsideWaterLevel.Mean);
            Assert.IsNaN(inputParameters.InsideWaterLevel.StandardDeviation);
            Assert.IsNaN(inputParameters.ThresholdHeightOpenWeir.Mean);
            Assert.IsNaN(inputParameters.ThresholdHeightOpenWeir.StandardDeviation);
            Assert.IsNaN(inputParameters.ConstructiveStrengthLinearLoadModel.Mean);
            Assert.IsNaN(inputParameters.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.ConstructiveStrengthQuadraticLoadModel.Mean);
            Assert.IsNaN(inputParameters.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.BankWidth.Mean);
            Assert.IsNaN(inputParameters.BankWidth.StandardDeviation);
            Assert.IsNaN(inputParameters.InsideWaterLevelFailureConstruction.Mean);
            Assert.IsNaN(inputParameters.InsideWaterLevelFailureConstruction.StandardDeviation);
            Assert.IsNaN(inputParameters.EvaluationLevel);
            Assert.IsNaN(inputParameters.LevelCrestStructure.Mean);
            Assert.IsNaN(inputParameters.LevelCrestStructure.StandardDeviation);
            Assert.IsNaN(inputParameters.VerticalDistance);
            Assert.IsNaN(inputParameters.FailureCollisionEnergy.Mean);
            Assert.IsNaN(inputParameters.FailureCollisionEnergy.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.ShipMass.Mean);
            Assert.IsNaN(inputParameters.ShipMass.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.ShipVelocity.Mean);
            Assert.IsNaN(inputParameters.ShipVelocity.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.FlowVelocityStructureClosable.Mean);
            Assert.IsNaN(inputParameters.StabilityLinearLoadModel.Mean);
            Assert.IsNaN(inputParameters.StabilityLinearLoadModel.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.StabilityQuadraticLoadModel.Mean);
            Assert.IsNaN(inputParameters.StabilityQuadraticLoadModel.CoefficientOfVariation);
            Assert.IsNaN(inputParameters.AreaFlowApertures.Mean);
            Assert.IsNaN(inputParameters.AreaFlowApertures.StandardDeviation);
            Assert.IsNaN(inputParameters.VolumicWeightWater);
            Assert.IsNaN(inputParameters.FactorStormDurationOpenStructure);
            Assert.IsNaN(inputParameters.DrainCoefficient.Mean);
        }

        [Test]
        public void Read_EntityWithStructureEntity_ReturnCalculationWithStructure()
        {
            // Setup
            StabilityPointStructure structure = new TestStabilityPointStructure();
            var structureEntity = new StabilityPointStructureEntity();
            var entity = new StabilityPointStructuresCalculationEntity
            {
                StabilityPointStructureEntity = structureEntity
            };
            var collector = new ReadConversionCollector();
            collector.Read(structureEntity, structure);

            // Call
            StructuresCalculation<StabilityPointStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(structure, calculation.InputParameters.Structure);
        }

        [Test]
        public void Read_EntityWithHydraulicLocationEntity_ReturnCalculationWithHydraulicBoundaryLocation()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "A", 2, 3);
            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var entity = new StabilityPointStructuresCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            StructuresCalculation<StabilityPointStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Read_EntityWithForeshoreProfileEntity_ReturnCalculationWithForeshoreProfile()
        {
            // Setup
            var profile = new TestForeshoreProfile();
            var profileEntity = new ForeshoreProfileEntity();
            var entity = new StabilityPointStructuresCalculationEntity
            {
                ForeshoreProfileEntity = profileEntity
            };

            var collector = new ReadConversionCollector();
            collector.Read(profileEntity, profile);

            // Call
            StructuresCalculation<StabilityPointStructuresInput> calculation = entity.Read(collector);

            // Assert
            Assert.AreSame(profile, calculation.InputParameters.ForeshoreProfile);
        }

        [Test]
        public void Read_ValidEntityWithOutputEntity_ReturnCalculationWithOutput()
        {
            // Setup
            var entity = new StabilityPointStructuresCalculationEntity
            {
                StabilityPointStructuresOutputEntities =
                {
                    new StabilityPointStructuresOutputEntity()
                }
            };

            var collector = new ReadConversionCollector();

            // Call
            StructuresCalculation<StabilityPointStructuresInput> calculation = entity.Read(collector);

            // Assert
            StructuresOutput calculationOutput = calculation.Output;
            Assert.IsNaN(calculationOutput.Reliability);
            Assert.IsFalse(calculationOutput.HasGeneralResult);
        }

        [Test]
        public void Read_CalculationEntityAlreadyRead_ReturnReadCalculation()
        {
            // Setup
            var entity = new StabilityPointStructuresCalculationEntity
            {
                StabilityPointStructuresOutputEntities =
                {
                    new StabilityPointStructuresOutputEntity()
                }
            };

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            var collector = new ReadConversionCollector();
            collector.Read(entity, calculation);

            // Call
            StructuresCalculation<StabilityPointStructuresInput> returnedCalculation = entity.Read(collector);

            // Assert
            Assert.AreSame(calculation, returnedCalculation);
        }
    }
}