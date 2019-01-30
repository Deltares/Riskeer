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
using Core.Common.Util.Extensions;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.StabilityPointStructures
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
                throw new ArgumentNullException(nameof(registry));
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
                StructureNormalOrientation = structure.StructureNormalOrientation.ToNaNAsNull(),
                StorageStructureAreaMean = structure.StorageStructureArea.Mean.ToNaNAsNull(),
                StorageStructureAreaCoefficientOfVariation = structure.StorageStructureArea.CoefficientOfVariation.ToNaNAsNull(),
                AllowedLevelIncreaseStorageMean = structure.AllowedLevelIncreaseStorage.Mean.ToNaNAsNull(),
                AllowedLevelIncreaseStorageStandardDeviation = structure.AllowedLevelIncreaseStorage.StandardDeviation.ToNaNAsNull(),
                WidthFlowAperturesMean = structure.WidthFlowApertures.Mean.ToNaNAsNull(),
                WidthFlowAperturesStandardDeviation = structure.WidthFlowApertures.StandardDeviation.ToNaNAsNull(),
                InsideWaterLevelMean = structure.InsideWaterLevel.Mean.ToNaNAsNull(),
                InsideWaterLevelStandardDeviation = structure.InsideWaterLevel.StandardDeviation.ToNaNAsNull(),
                ThresholdHeightOpenWeirMean = structure.ThresholdHeightOpenWeir.Mean.ToNaNAsNull(),
                ThresholdHeightOpenWeirStandardDeviation = structure.ThresholdHeightOpenWeir.StandardDeviation.ToNaNAsNull(),
                CriticalOvertoppingDischargeMean = structure.CriticalOvertoppingDischarge.Mean.ToNaNAsNull(),
                CriticalOvertoppingDischargeCoefficientOfVariation = structure.CriticalOvertoppingDischarge.CoefficientOfVariation.ToNaNAsNull(),
                FlowWidthAtBottomProtectionMean = structure.FlowWidthAtBottomProtection.Mean.ToNaNAsNull(),
                FlowWidthAtBottomProtectionStandardDeviation = structure.FlowWidthAtBottomProtection.StandardDeviation.ToNaNAsNull(),
                ConstructiveStrengthLinearLoadModelMean = structure.ConstructiveStrengthLinearLoadModel.Mean.ToNaNAsNull(),
                ConstructiveStrengthLinearLoadModelCoefficientOfVariation = structure.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation.ToNaNAsNull(),
                ConstructiveStrengthQuadraticLoadModelMean = structure.ConstructiveStrengthQuadraticLoadModel.Mean.ToNaNAsNull(),
                ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation = structure.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation.ToNaNAsNull(),
                BankWidthMean = structure.BankWidth.Mean.ToNaNAsNull(),
                BankWidthStandardDeviation = structure.BankWidth.StandardDeviation.ToNaNAsNull(),
                InsideWaterLevelFailureConstructionMean = structure.InsideWaterLevelFailureConstruction.Mean.ToNaNAsNull(),
                InsideWaterLevelFailureConstructionStandardDeviation = structure.InsideWaterLevelFailureConstruction.StandardDeviation.ToNaNAsNull(),
                EvaluationLevel = structure.EvaluationLevel.ToNaNAsNull(),
                LevelCrestStructureMean = structure.LevelCrestStructure.Mean.ToNaNAsNull(),
                LevelCrestStructureStandardDeviation = structure.LevelCrestStructure.StandardDeviation.ToNaNAsNull(),
                VerticalDistance = structure.VerticalDistance.ToNaNAsNull(),
                FailureProbabilityRepairClosure = structure.FailureProbabilityRepairClosure.ToNaNAsNull(),
                FailureCollisionEnergyMean = structure.FailureCollisionEnergy.Mean.ToNaNAsNull(),
                FailureCollisionEnergyCoefficientOfVariation = structure.FailureCollisionEnergy.CoefficientOfVariation.ToNaNAsNull(),
                ShipMassMean = structure.ShipMass.Mean.ToNaNAsNull(),
                ShipMassCoefficientOfVariation = structure.ShipMass.CoefficientOfVariation.ToNaNAsNull(),
                ShipVelocityMean = structure.ShipVelocity.Mean.ToNaNAsNull(),
                ShipVelocityCoefficientOfVariation = structure.ShipVelocity.CoefficientOfVariation.ToNaNAsNull(),
                LevellingCount = structure.LevellingCount,
                ProbabilityCollisionSecondaryStructure = structure.ProbabilityCollisionSecondaryStructure.ToNaNAsNull(),
                FlowVelocityStructureClosableMean = structure.FlowVelocityStructureClosable.Mean.ToNaNAsNull(),
                StabilityLinearLoadModelMean = structure.StabilityLinearLoadModel.Mean.ToNaNAsNull(),
                StabilityLinearLoadModelCoefficientOfVariation = structure.StabilityLinearLoadModel.CoefficientOfVariation.ToNaNAsNull(),
                StabilityQuadraticLoadModelMean = structure.StabilityQuadraticLoadModel.Mean.ToNaNAsNull(),
                StabilityQuadraticLoadModelCoefficientOfVariation = structure.StabilityQuadraticLoadModel.CoefficientOfVariation.ToNaNAsNull(),
                AreaFlowAperturesMean = structure.AreaFlowApertures.Mean.ToNaNAsNull(),
                AreaFlowAperturesStandardDeviation = structure.AreaFlowApertures.StandardDeviation.ToNaNAsNull(),
                InflowModelType = Convert.ToByte(structure.InflowModelType),
                Order = order
            };

            registry.Register(entity, structure);

            return entity;
        }
    }
}