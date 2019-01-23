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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.StabilityPointStructures;
using Riskeer.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.StabilityPointStructures
{
    [TestFixture]
    public class StabilityPointStructureCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            StabilityPointStructure structure = new TestStabilityPointStructure();

            // Call
            TestDelegate call = () => structure.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_ValidStructure_ReturnEntity()
        {
            // Setup
            StabilityPointStructure structure = new TestStabilityPointStructure();
            var registry = new PersistenceRegistry();

            const int order = 4;

            // Call
            StabilityPointStructureEntity entity = structure.Create(registry, order);

            // Assert
            Assert.AreEqual(structure.Name, entity.Name);
            Assert.AreNotSame(structure.Name, entity.Name);
            Assert.AreEqual(structure.Id, entity.Id);
            Assert.AreNotSame(structure.Id, entity.Id);
            Assert.AreEqual(structure.Location.X, entity.X);
            Assert.AreEqual(structure.Location.Y, entity.Y);
            Assert.AreEqual(structure.StructureNormalOrientation.Value, entity.StructureNormalOrientation);

            Assert.AreEqual(structure.StorageStructureArea.Mean.Value, entity.StorageStructureAreaMean);
            Assert.AreEqual(structure.StorageStructureArea.CoefficientOfVariation.Value, entity.StorageStructureAreaCoefficientOfVariation);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage.Mean.Value, entity.AllowedLevelIncreaseStorageMean);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage.StandardDeviation.Value, entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.AreEqual(structure.WidthFlowApertures.Mean.Value, entity.WidthFlowAperturesMean);
            Assert.AreEqual(structure.WidthFlowApertures.StandardDeviation.Value, entity.WidthFlowAperturesStandardDeviation);
            Assert.AreEqual(structure.InsideWaterLevel.Mean.Value, entity.InsideWaterLevelMean);
            Assert.AreEqual(structure.InsideWaterLevel.StandardDeviation.Value, entity.InsideWaterLevelStandardDeviation);
            Assert.AreEqual(structure.ThresholdHeightOpenWeir.Mean.Value, entity.ThresholdHeightOpenWeirMean);
            Assert.AreEqual(structure.ThresholdHeightOpenWeir.StandardDeviation.Value, entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge.Mean.Value, entity.CriticalOvertoppingDischargeMean);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value, entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection.Mean.Value, entity.FlowWidthAtBottomProtectionMean);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection.StandardDeviation.Value, entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.AreEqual(structure.ConstructiveStrengthLinearLoadModel.Mean.Value, entity.ConstructiveStrengthLinearLoadModelMean);
            Assert.AreEqual(structure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.Value, entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation);
            Assert.AreEqual(structure.ConstructiveStrengthQuadraticLoadModel.Mean.Value, entity.ConstructiveStrengthQuadraticLoadModelMean);
            Assert.AreEqual(structure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.Value, entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation);
            Assert.AreEqual(structure.BankWidth.Mean.Value, entity.BankWidthMean);
            Assert.AreEqual(structure.BankWidth.StandardDeviation.Value, entity.BankWidthStandardDeviation);
            Assert.AreEqual(structure.InsideWaterLevelFailureConstruction.Mean.Value, entity.InsideWaterLevelFailureConstructionMean);
            Assert.AreEqual(structure.InsideWaterLevelFailureConstruction.StandardDeviation.Value, entity.InsideWaterLevelFailureConstructionStandardDeviation);
            Assert.AreEqual(structure.EvaluationLevel.Value, entity.EvaluationLevel);
            Assert.AreEqual(structure.LevelCrestStructure.Mean.Value, entity.LevelCrestStructureMean);
            Assert.AreEqual(structure.LevelCrestStructure.StandardDeviation.Value, entity.LevelCrestStructureStandardDeviation);
            Assert.AreEqual(structure.VerticalDistance.Value, entity.VerticalDistance);
            Assert.AreEqual(structure.FailureProbabilityRepairClosure, entity.FailureProbabilityRepairClosure);
            Assert.AreEqual(structure.FailureCollisionEnergy.Mean.Value, entity.FailureCollisionEnergyMean);
            Assert.AreEqual(structure.FailureCollisionEnergy.CoefficientOfVariation.Value, entity.FailureCollisionEnergyCoefficientOfVariation);
            Assert.AreEqual(structure.ShipMass.Mean.Value, entity.ShipMassMean);
            Assert.AreEqual(structure.ShipMass.CoefficientOfVariation.Value, entity.ShipMassCoefficientOfVariation);
            Assert.AreEqual(structure.ShipVelocity.Mean.Value, entity.ShipVelocityMean);
            Assert.AreEqual(structure.ShipVelocity.CoefficientOfVariation.Value, entity.ShipVelocityCoefficientOfVariation);
            Assert.AreEqual(structure.LevellingCount, entity.LevellingCount);
            Assert.AreEqual(structure.ProbabilityCollisionSecondaryStructure, entity.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(structure.FlowVelocityStructureClosable.Mean.Value, entity.FlowVelocityStructureClosableMean);
            Assert.AreEqual(structure.StabilityLinearLoadModel.Mean.Value, entity.StabilityLinearLoadModelMean);
            Assert.AreEqual(structure.StabilityLinearLoadModel.CoefficientOfVariation.Value, entity.StabilityLinearLoadModelCoefficientOfVariation);
            Assert.AreEqual(structure.StabilityQuadraticLoadModel.Mean.Value, entity.StabilityQuadraticLoadModelMean);
            Assert.AreEqual(structure.StabilityQuadraticLoadModel.CoefficientOfVariation.Value, entity.StabilityQuadraticLoadModelCoefficientOfVariation);
            Assert.AreEqual(structure.AreaFlowApertures.Mean.Value, entity.AreaFlowAperturesMean);
            Assert.AreEqual(structure.AreaFlowApertures.StandardDeviation.Value, entity.AreaFlowAperturesStandardDeviation);
            Assert.AreEqual(Convert.ToByte(structure.InflowModelType), entity.InflowModelType);
            Assert.AreEqual(order, entity.Order);

            Assert.IsTrue(registry.Contains(structure));
        }

        [Test]
        public void Create_NaNValue_ReturnEntityWithNullValue()
        {
            // Setup
            var structure = new StabilityPointStructure(new StabilityPointStructure.ConstructionProperties
            {
                Name = "A",
                Id = "B",
                Location = new Point2D(double.NaN, double.NaN),
                StructureNormalOrientation = RoundedDouble.NaN,
                StorageStructureArea =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                WidthFlowApertures =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                InsideWaterLevel =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                ConstructiveStrengthLinearLoadModel =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                ConstructiveStrengthQuadraticLoadModel =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                BankWidth =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                InsideWaterLevelFailureConstruction =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                EvaluationLevel = RoundedDouble.NaN,
                LevelCrestStructure =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                },
                VerticalDistance = RoundedDouble.NaN,
                FailureProbabilityRepairClosure = double.NaN,
                FailureCollisionEnergy =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                ShipMass =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                ShipVelocity =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                ProbabilityCollisionSecondaryStructure = double.NaN,
                FlowVelocityStructureClosable =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                StabilityLinearLoadModel =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                StabilityQuadraticLoadModel =
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                AreaFlowApertures =
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN
                }
            });
            var registry = new PersistenceRegistry();

            // Call
            StabilityPointStructureEntity entity = structure.Create(registry, 0);

            // Assert
            Assert.IsNull(entity.X);
            Assert.IsNull(entity.Y);
            Assert.IsNull(entity.StructureNormalOrientation);

            Assert.IsNull(entity.StorageStructureAreaMean);
            Assert.IsNull(entity.StorageStructureAreaCoefficientOfVariation);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageMean);
            Assert.IsNull(entity.AllowedLevelIncreaseStorageStandardDeviation);
            Assert.IsNull(entity.WidthFlowAperturesMean);
            Assert.IsNull(entity.WidthFlowAperturesStandardDeviation);
            Assert.IsNull(entity.InsideWaterLevelMean);
            Assert.IsNull(entity.InsideWaterLevelStandardDeviation);
            Assert.IsNull(entity.ThresholdHeightOpenWeirMean);
            Assert.IsNull(entity.ThresholdHeightOpenWeirStandardDeviation);
            Assert.IsNull(entity.CriticalOvertoppingDischargeMean);
            Assert.IsNull(entity.CriticalOvertoppingDischargeCoefficientOfVariation);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionMean);
            Assert.IsNull(entity.FlowWidthAtBottomProtectionStandardDeviation);
            Assert.IsNull(entity.ConstructiveStrengthLinearLoadModelMean);
            Assert.IsNull(entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation);
            Assert.IsNull(entity.ConstructiveStrengthQuadraticLoadModelMean);
            Assert.IsNull(entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation);
            Assert.IsNull(entity.BankWidthMean);
            Assert.IsNull(entity.BankWidthStandardDeviation);
            Assert.IsNull(entity.InsideWaterLevelFailureConstructionMean);
            Assert.IsNull(entity.InsideWaterLevelFailureConstructionStandardDeviation);
            Assert.IsNull(entity.EvaluationLevel);
            Assert.IsNull(entity.LevelCrestStructureMean);
            Assert.IsNull(entity.LevelCrestStructureStandardDeviation);
            Assert.IsNull(entity.VerticalDistance);
            Assert.IsNull(entity.FailureProbabilityRepairClosure);
            Assert.IsNull(entity.FailureCollisionEnergyMean);
            Assert.IsNull(entity.FailureCollisionEnergyCoefficientOfVariation);
            Assert.IsNull(entity.ShipMassMean);
            Assert.IsNull(entity.ShipMassCoefficientOfVariation);
            Assert.IsNull(entity.ShipVelocityMean);
            Assert.IsNull(entity.ShipVelocityCoefficientOfVariation);
            Assert.IsNull(entity.ProbabilityCollisionSecondaryStructure);
            Assert.IsNull(entity.FlowVelocityStructureClosableMean);
            Assert.IsNull(entity.StabilityLinearLoadModelMean);
            Assert.IsNull(entity.StabilityLinearLoadModelCoefficientOfVariation);
            Assert.IsNull(entity.StabilityQuadraticLoadModelMean);
            Assert.IsNull(entity.StabilityQuadraticLoadModelCoefficientOfVariation);
            Assert.IsNull(entity.AreaFlowAperturesMean);
            Assert.IsNull(entity.AreaFlowAperturesStandardDeviation);
        }

        [Test]
        public void Create_StructureAlreadyRegistered_ReturnRegisteredEntity()
        {
            // Setup
            var structure = new TestStabilityPointStructure();

            var registeredEntity = new StabilityPointStructureEntity();
            var registry = new PersistenceRegistry();
            registry.Register(registeredEntity, structure);

            // Call
            StabilityPointStructureEntity entity = structure.Create(registry, 0);

            // Assert
            Assert.AreSame(registeredEntity, entity);
        }
    }
}