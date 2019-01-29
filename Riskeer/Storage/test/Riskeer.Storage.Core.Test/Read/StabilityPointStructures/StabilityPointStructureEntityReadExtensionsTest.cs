// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using NUnit.Framework;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.StabilityPointStructures;

namespace Riskeer.Storage.Core.Test.Read.StabilityPointStructures
{
    [TestFixture]
    public class StabilityPointStructureEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new StabilityPointStructureEntity();

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
            var entity = new StabilityPointStructureEntity
            {
                Name = "name",
                Id = "id"
            };

            var collector = new ReadConversionCollector();

            // Precondition
            Assert.IsFalse(collector.Contains(entity));

            // Call
            StabilityPointStructure calculation = entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(entity));
            Assert.AreSame(calculation, collector.Get(entity));
        }

        [Test]
        public void Read_ValidEntity_ReturnStabilityPointStructure()
        {
            // Setup
            var entity = new StabilityPointStructureEntity
            {
                Name = "A",
                Id = "B",
                X = 1.1,
                Y = 2.2,
                StructureNormalOrientation = 3.3,
                StorageStructureAreaMean = 4.4,
                StorageStructureAreaCoefficientOfVariation = 5.5,
                AllowedLevelIncreaseStorageMean = 6.6,
                AllowedLevelIncreaseStorageStandardDeviation = 7.7,
                WidthFlowAperturesMean = 8.8,
                WidthFlowAperturesStandardDeviation = 9.9,
                InsideWaterLevelMean = 10.10,
                InsideWaterLevelStandardDeviation = 11.11,
                ThresholdHeightOpenWeirMean = 12.12,
                ThresholdHeightOpenWeirStandardDeviation = 13.13,
                CriticalOvertoppingDischargeMean = 14.14,
                CriticalOvertoppingDischargeCoefficientOfVariation = 15.15,
                FlowWidthAtBottomProtectionMean = 16.16,
                FlowWidthAtBottomProtectionStandardDeviation = 17.17,
                ConstructiveStrengthLinearLoadModelMean = 18.18,
                ConstructiveStrengthLinearLoadModelCoefficientOfVariation = 19.19,
                ConstructiveStrengthQuadraticLoadModelMean = 20.20,
                ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation = 21.21,
                BankWidthMean = 22.22,
                BankWidthStandardDeviation = 23.23,
                InsideWaterLevelFailureConstructionMean = 24.24,
                InsideWaterLevelFailureConstructionStandardDeviation = 25.25,
                EvaluationLevel = 26.26,
                LevelCrestStructureMean = 27.27,
                LevelCrestStructureStandardDeviation = 28.28,
                VerticalDistance = 29.29,
                FailureProbabilityRepairClosure = 30.30,
                FailureCollisionEnergyMean = 31.31,
                FailureCollisionEnergyCoefficientOfVariation = 32.32,
                ShipMassMean = 33.33,
                ShipMassCoefficientOfVariation = 34.34,
                ShipVelocityMean = 35.35,
                ShipVelocityCoefficientOfVariation = 36.36,
                LevellingCount = 37,
                ProbabilityCollisionSecondaryStructure = 38.38,
                FlowVelocityStructureClosableMean = 39.39,
                StabilityLinearLoadModelMean = 41.41,
                StabilityLinearLoadModelCoefficientOfVariation = 42.42,
                StabilityQuadraticLoadModelMean = 43.43,
                StabilityQuadraticLoadModelCoefficientOfVariation = 44.44,
                AreaFlowAperturesMean = 45.45,
                AreaFlowAperturesStandardDeviation = 46.46,
                InflowModelType = Convert.ToByte(StabilityPointStructureInflowModelType.FloodedCulvert)
            };

            var collector = new ReadConversionCollector();

            // Call
            StabilityPointStructure structure = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.Name, structure.Name);
            Assert.AreEqual(entity.Id, structure.Id);
            Assert.AreEqual(entity.X, structure.Location.X);
            Assert.AreEqual(entity.Y, structure.Location.Y);
            Assert.AreEqual(entity.StructureNormalOrientation, structure.StructureNormalOrientation.Value);

            Assert.AreEqual(entity.StorageStructureAreaMean, structure.StorageStructureArea.Mean.Value);
            Assert.AreEqual(entity.StorageStructureAreaCoefficientOfVariation, structure.StorageStructureArea.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageMean, structure.AllowedLevelIncreaseStorage.Mean.Value);
            Assert.AreEqual(entity.AllowedLevelIncreaseStorageStandardDeviation, structure.AllowedLevelIncreaseStorage.StandardDeviation.Value);
            Assert.AreEqual(entity.WidthFlowAperturesMean, structure.WidthFlowApertures.Mean.Value);
            Assert.AreEqual(entity.WidthFlowAperturesStandardDeviation, structure.WidthFlowApertures.StandardDeviation.Value);
            Assert.AreEqual(entity.InsideWaterLevelMean, structure.InsideWaterLevel.Mean.Value);
            Assert.AreEqual(entity.InsideWaterLevelStandardDeviation, structure.InsideWaterLevel.StandardDeviation.Value);
            Assert.AreEqual(entity.ThresholdHeightOpenWeirMean, structure.ThresholdHeightOpenWeir.Mean.Value);
            Assert.AreEqual(entity.ThresholdHeightOpenWeirStandardDeviation, structure.ThresholdHeightOpenWeir.StandardDeviation.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeMean, structure.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(entity.CriticalOvertoppingDischargeCoefficientOfVariation, structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionMean, structure.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(entity.FlowWidthAtBottomProtectionStandardDeviation, structure.FlowWidthAtBottomProtection.StandardDeviation.Value);
            Assert.AreEqual(entity.ConstructiveStrengthLinearLoadModelMean, structure.ConstructiveStrengthLinearLoadModel.Mean.Value);
            Assert.AreEqual(entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation, structure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.ConstructiveStrengthQuadraticLoadModelMean, structure.ConstructiveStrengthQuadraticLoadModel.Mean.Value);
            Assert.AreEqual(entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation, structure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.BankWidthMean, structure.BankWidth.Mean.Value);
            Assert.AreEqual(entity.BankWidthStandardDeviation, structure.BankWidth.StandardDeviation.Value);
            Assert.AreEqual(entity.InsideWaterLevelFailureConstructionMean, structure.InsideWaterLevelFailureConstruction.Mean.Value);
            Assert.AreEqual(entity.InsideWaterLevelFailureConstructionStandardDeviation, structure.InsideWaterLevelFailureConstruction.StandardDeviation.Value);
            Assert.AreEqual(entity.EvaluationLevel, structure.EvaluationLevel.Value);
            Assert.AreEqual(entity.LevelCrestStructureMean, structure.LevelCrestStructure.Mean.Value);
            Assert.AreEqual(entity.LevelCrestStructureStandardDeviation, structure.LevelCrestStructure.StandardDeviation.Value);
            Assert.AreEqual(entity.VerticalDistance, structure.VerticalDistance.Value);
            Assert.AreEqual(entity.FailureProbabilityRepairClosure, structure.FailureProbabilityRepairClosure);
            Assert.AreEqual(entity.FailureCollisionEnergyMean, structure.FailureCollisionEnergy.Mean.Value);
            Assert.AreEqual(entity.FailureCollisionEnergyCoefficientOfVariation, structure.FailureCollisionEnergy.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.ShipMassMean, structure.ShipMass.Mean.Value);
            Assert.AreEqual(entity.ShipMassCoefficientOfVariation, structure.ShipMass.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.ShipVelocityMean, structure.ShipVelocity.Mean.Value);
            Assert.AreEqual(entity.ShipVelocityCoefficientOfVariation, structure.ShipVelocity.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.LevellingCount, structure.LevellingCount);
            Assert.AreEqual(entity.ProbabilityCollisionSecondaryStructure, structure.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(entity.FlowVelocityStructureClosableMean, structure.FlowVelocityStructureClosable.Mean.Value);
            Assert.AreEqual(entity.StabilityLinearLoadModelMean, structure.StabilityLinearLoadModel.Mean.Value);
            Assert.AreEqual(entity.StabilityLinearLoadModelCoefficientOfVariation, structure.StabilityLinearLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.StabilityQuadraticLoadModelMean, structure.StabilityQuadraticLoadModel.Mean.Value);
            Assert.AreEqual(entity.StabilityQuadraticLoadModelCoefficientOfVariation, structure.StabilityQuadraticLoadModel.CoefficientOfVariation.Value);
            Assert.AreEqual(entity.AreaFlowAperturesMean, structure.AreaFlowApertures.Mean.Value);
            Assert.AreEqual(entity.AreaFlowAperturesStandardDeviation, structure.AreaFlowApertures.StandardDeviation.Value);
            Assert.AreEqual((StabilityPointStructureInflowModelType) entity.InflowModelType, structure.InflowModelType);
        }

        [Test]
        public void Read_NullValues_ReturnStabilityPointStructureWithNaN()
        {
            // Setup
            var entity = new StabilityPointStructureEntity
            {
                Name = "A",
                Id = "B",
                X = null,
                Y = null,
                StructureNormalOrientation = null,
                StorageStructureAreaMean = null,
                StorageStructureAreaCoefficientOfVariation = null,
                AllowedLevelIncreaseStorageMean = null,
                AllowedLevelIncreaseStorageStandardDeviation = null,
                WidthFlowAperturesMean = null,
                WidthFlowAperturesStandardDeviation = null,
                InsideWaterLevelMean = null,
                InsideWaterLevelStandardDeviation = null,
                ThresholdHeightOpenWeirMean = null,
                ThresholdHeightOpenWeirStandardDeviation = null,
                CriticalOvertoppingDischargeMean = null,
                CriticalOvertoppingDischargeCoefficientOfVariation = null,
                FlowWidthAtBottomProtectionMean = null,
                FlowWidthAtBottomProtectionStandardDeviation = null,
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
                FailureProbabilityRepairClosure = null,
                FailureCollisionEnergyMean = null,
                FailureCollisionEnergyCoefficientOfVariation = null,
                ShipMassMean = null,
                ShipMassCoefficientOfVariation = null,
                ShipVelocityMean = null,
                ShipVelocityCoefficientOfVariation = null,
                ProbabilityCollisionSecondaryStructure = null,
                FlowVelocityStructureClosableMean = null,
                StabilityLinearLoadModelMean = null,
                StabilityLinearLoadModelCoefficientOfVariation = null,
                AreaFlowAperturesMean = null,
                AreaFlowAperturesStandardDeviation = null
            };

            var collector = new ReadConversionCollector();

            // Call
            StabilityPointStructure structure = entity.Read(collector);

            // Assert
            Assert.IsNaN(structure.Location.X);
            Assert.IsNaN(structure.Location.Y);
            Assert.IsNaN(structure.StructureNormalOrientation);

            Assert.IsNaN(structure.StorageStructureArea.Mean);
            Assert.IsNaN(structure.StorageStructureArea.CoefficientOfVariation);
            Assert.IsNaN(structure.AllowedLevelIncreaseStorage.Mean);
            Assert.IsNaN(structure.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.IsNaN(structure.WidthFlowApertures.Mean);
            Assert.IsNaN(structure.WidthFlowApertures.StandardDeviation);
            Assert.IsNaN(structure.InsideWaterLevel.Mean);
            Assert.IsNaN(structure.InsideWaterLevel.StandardDeviation);
            Assert.IsNaN(structure.ThresholdHeightOpenWeir.Mean);
            Assert.IsNaN(structure.ThresholdHeightOpenWeir.StandardDeviation);
            Assert.IsNaN(structure.CriticalOvertoppingDischarge.Mean);
            Assert.IsNaN(structure.CriticalOvertoppingDischarge.CoefficientOfVariation);
            Assert.IsNaN(structure.FlowWidthAtBottomProtection.Mean);
            Assert.IsNaN(structure.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.IsNaN(structure.ConstructiveStrengthLinearLoadModel.Mean);
            Assert.IsNaN(structure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation);
            Assert.IsNaN(structure.ConstructiveStrengthQuadraticLoadModel.Mean);
            Assert.IsNaN(structure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation);
            Assert.IsNaN(structure.BankWidth.Mean);
            Assert.IsNaN(structure.BankWidth.StandardDeviation);
            Assert.IsNaN(structure.InsideWaterLevelFailureConstruction.Mean);
            Assert.IsNaN(structure.InsideWaterLevelFailureConstruction.StandardDeviation);
            Assert.IsNaN(structure.EvaluationLevel);
            Assert.IsNaN(structure.LevelCrestStructure.Mean);
            Assert.IsNaN(structure.LevelCrestStructure.StandardDeviation);
            Assert.IsNaN(structure.VerticalDistance);
            Assert.IsNaN(structure.FailureProbabilityRepairClosure);
            Assert.IsNaN(structure.FailureCollisionEnergy.Mean);
            Assert.IsNaN(structure.FailureCollisionEnergy.CoefficientOfVariation);
            Assert.IsNaN(structure.ShipMass.Mean);
            Assert.IsNaN(structure.ShipMass.CoefficientOfVariation);
            Assert.IsNaN(structure.ShipVelocity.Mean);
            Assert.IsNaN(structure.ShipVelocity.CoefficientOfVariation);
            Assert.IsNaN(structure.ProbabilityCollisionSecondaryStructure);
            Assert.IsNaN(structure.FlowVelocityStructureClosable.Mean);
            Assert.IsNaN(structure.StabilityLinearLoadModel.Mean);
            Assert.IsNaN(structure.StabilityLinearLoadModel.CoefficientOfVariation);
            Assert.IsNaN(structure.StabilityQuadraticLoadModel.Mean);
            Assert.IsNaN(structure.StabilityQuadraticLoadModel.CoefficientOfVariation);
            Assert.IsNaN(structure.AreaFlowApertures.Mean);
            Assert.IsNaN(structure.AreaFlowApertures.StandardDeviation);
        }

        [Test]
        public void Read_EntityRegistered_ReturnRegisteredStructure()
        {
            // Setup
            var entity = new StabilityPointStructureEntity();
            StabilityPointStructure registeredStructure = new TestStabilityPointStructure();
            var collector = new ReadConversionCollector();
            collector.Read(entity, registeredStructure);

            // Call
            StabilityPointStructure readStructure = entity.Read(collector);

            // Assert
            Assert.AreSame(registeredStructure, readStructure);
        }
    }
}