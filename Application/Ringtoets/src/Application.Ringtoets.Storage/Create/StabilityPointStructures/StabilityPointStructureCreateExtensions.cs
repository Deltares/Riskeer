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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils.Extensions;
using Ringtoets.StabilityPointStructures.Data;

namespace Application.Ringtoets.Storage.Create.StabilityPointStructures
{
    /// <summary>
    /// Extension methods for <see cref="StabilityPointStructure"/> related to creating
    /// a <see cref="StabilityPointStructureEntity"/>.
    /// </summary>
    internal static class StabilityPointStructureCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="StabilityPointStructureEntity"/> based on the information
        /// of the <see cref="StabilityPointStructure"/>.
        /// </summary>
        /// <param name="structure">The structure to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="structure"/> resides within its parent.</param>
        /// <returns>A new <see cref="StabilityPointStructureEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static StabilityPointStructureEntity Create(this StabilityPointStructure structure, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (registry.Contains(structure))
            {
                return registry.Get(structure);
            }

            var entity = new StabilityPointStructureEntity
            {
                Name = structure.Name.DeepClone(),
                Id = structure.Id.DeepClone(),
                X = structure.Location.X.ToNaNAsNull(),
                Y = structure.Location.Y.ToNaNAsNull(),
                StructureNormalOrientation = structure.StructureNormalOrientation.Value.ToNaNAsNull(),
                StorageStructureAreaMean = structure.StorageStructureArea.Mean.Value.ToNaNAsNull(),
                StorageStructureAreaCoefficientOfVariation = structure.StorageStructureArea.CoefficientOfVariation.Value.ToNaNAsNull(),
                AllowedLevelIncreaseStorageMean = structure.AllowedLevelIncreaseStorage.Mean.Value.ToNaNAsNull(),
                AllowedLevelIncreaseStorageStandardDeviation = structure.AllowedLevelIncreaseStorage.StandardDeviation.Value.ToNaNAsNull(),
                WidthFlowAperturesMean = structure.WidthFlowApertures.Mean.Value.ToNaNAsNull(),
                WidthFlowAperturesCoefficientOfVariation = structure.WidthFlowApertures.StandardDeviation.Value.ToNaNAsNull(),
                InsideWaterLevelMean = structure.InsideWaterLevel.Mean.Value.ToNaNAsNull(),
                InsideWaterLevelStandardDeviation = structure.InsideWaterLevel.StandardDeviation.Value.ToNaNAsNull(),
                ThresholdHeightOpenWeirMean = structure.ThresholdHeightOpenWeir.Mean.Value.ToNaNAsNull(),
                ThresholdHeightOpenWeirStandardDeviation = structure.ThresholdHeightOpenWeir.StandardDeviation.Value.ToNaNAsNull(),
                CriticalOvertoppingDischargeMean = structure.CriticalOvertoppingDischarge.Mean.Value.ToNaNAsNull(),
                CriticalOvertoppingDischargeCoefficientOfVariation = structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value.ToNaNAsNull(),
                FlowWidthAtBottomProtectionMean = structure.FlowWidthAtBottomProtection.Mean.Value.ToNaNAsNull(),
                FlowWidthAtBottomProtectionStandardDeviation = structure.FlowWidthAtBottomProtection.StandardDeviation.Value.ToNaNAsNull(),
                ConstructiveStrengthLinearLoadModelMean = structure.ConstructiveStrengthLinearLoadModel.Mean.Value.ToNaNAsNull(),
                ConstructiveStrengthLinearLoadModelCoefficientOfVariation = structure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.Value.ToNaNAsNull(),
                ConstructiveStrengthQuadraticLoadModelMean = structure.ConstructiveStrengthQuadraticLoadModel.Mean.Value.ToNaNAsNull(),
                ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation = structure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.Value.ToNaNAsNull(),
                BankWidthMean = structure.BankWidth.Mean.Value.ToNaNAsNull(),
                BankWidthStandardDeviation = structure.BankWidth.StandardDeviation.Value.ToNaNAsNull(),
                InsideWaterLevelFailureConstructionMean = structure.InsideWaterLevelFailureConstruction.Mean.Value.ToNaNAsNull(),
                InsideWaterLevelFailureConstructionStandardDeviation = structure.InsideWaterLevelFailureConstruction.StandardDeviation.Value.ToNaNAsNull(),
                EvaluationLevel = structure.EvaluationLevel.Value.ToNaNAsNull(),
                LevelCrestStructureMean = structure.LevelCrestStructure.Mean.Value.ToNaNAsNull(),
                LevelCrestStructureStandardDeviation = structure.LevelCrestStructure.StandardDeviation.Value.ToNaNAsNull(),
                VerticalDistance = structure.VerticalDistance.Value.ToNaNAsNull(),
                FailureProbabilityRepairClosure = structure.FailureProbabilityRepairClosure.ToNaNAsNull(),
                FailureCollisionEnergyMean = structure.FailureCollisionEnergy.Mean.Value.ToNaNAsNull(),
                FailureCollisionEnergyCoefficientOfVariation = structure.FailureCollisionEnergy.CoefficientOfVariation.Value.ToNaNAsNull(),
                ShipMassMean = structure.ShipMass.Mean.Value.ToNaNAsNull(),
                ShipMassCoefficientOfVariation = structure.ShipMass.CoefficientOfVariation.Value.ToNaNAsNull(),
                ShipVelocityMean = structure.ShipVelocity.Mean.Value.ToNaNAsNull(),
                ShipVelocityCoefficientOfVariation = structure.ShipVelocity.CoefficientOfVariation.Value.ToNaNAsNull(),
                LevellingCount = structure.LevellingCount,
                ProbabilityCollisionSecondaryStructure = structure.ProbabilityCollisionSecondaryStructure.ToNaNAsNull(),
                FlowVelocityStructureClosableMean = structure.FlowVelocityStructureClosable.Mean.Value.ToNaNAsNull(),
                FlowVelocityStructureClosableStandardDeviation = structure.FlowVelocityStructureClosable.CoefficientOfVariation.Value.ToNaNAsNull(),
                StabilityLinearLoadModelMean = structure.StabilityLinearLoadModel.Mean.Value.ToNaNAsNull(),
                StabilityLinearLoadModelCoefficientOfVariation = structure.StabilityLinearLoadModel.CoefficientOfVariation.Value.ToNaNAsNull(),
                StabilityQuadraticLoadModelMean = structure.StabilityQuadraticLoadModel.Mean.Value.ToNaNAsNull(),
                StabilityQuadraticLoadModelCoefficientOfVariation = structure.StabilityQuadraticLoadModel.CoefficientOfVariation.Value.ToNaNAsNull(),
                AreaFlowAperturesMean = structure.AreaFlowApertures.Mean.Value.ToNaNAsNull(),
                AreaFlowAperturesStandardDeviation = structure.AreaFlowApertures.StandardDeviation.Value.ToNaNAsNull(),
                InflowModelType = Convert.ToByte(structure.InflowModelType),
                Order = order
            };

            registry.Register(entity, structure);

            return entity;
        }
    }
}