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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.StabilityPointStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="StabilityPointStructure"/>
    /// based on the <see cref="StabilityPointStructureEntity"/>.
    /// </summary>
    internal static class StabilityPointStructureEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="StabilityPointStructureEntity"/> and use the information to update a 
        /// <see cref="StabilityPointStructure"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructureEntity"/> to create <see cref="StabilityPointStructure"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StabilityPointStructure"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static StabilityPointStructure Read(this StabilityPointStructureEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var structure = new StabilityPointStructure(new StabilityPointStructure.ConstructionProperties
            {
                Name = entity.Name,
                Id = entity.Id,
                Location = new Point2D(entity.X.ToNullAsNaN(), entity.Y.ToNullAsNaN()),
                StructureNormalOrientation = (RoundedDouble) entity.StructureNormalOrientation.ToNullAsNaN(),
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) entity.StorageStructureAreaMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.StorageStructureAreaCoefficientOfVariation.ToNullAsNaN()
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) entity.AllowedLevelIncreaseStorageMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.AllowedLevelIncreaseStorageStandardDeviation.ToNullAsNaN()
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) entity.FlowWidthAtBottomProtectionMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.FlowWidthAtBottomProtectionStandardDeviation.ToNullAsNaN()
                },
                InsideWaterLevel =
                {
                    Mean = (RoundedDouble) entity.InsideWaterLevelMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.InsideWaterLevelStandardDeviation.ToNullAsNaN()
                },
                ThresholdHeightOpenWeir =
                {
                    Mean = (RoundedDouble) entity.ThresholdHeightOpenWeirMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.ThresholdHeightOpenWeirStandardDeviation.ToNullAsNaN()
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) entity.CriticalOvertoppingDischargeMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.CriticalOvertoppingDischargeCoefficientOfVariation.ToNullAsNaN()
                },
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) entity.WidthFlowAperturesMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.WidthFlowAperturesStandardDeviation.ToNullAsNaN()
                },
                ConstructiveStrengthLinearLoadModel =
                {
                    Mean = (RoundedDouble) entity.ConstructiveStrengthLinearLoadModelMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.ConstructiveStrengthLinearLoadModelCoefficientOfVariation.ToNullAsNaN()
                },
                ConstructiveStrengthQuadraticLoadModel =
                {
                    Mean = (RoundedDouble) entity.ConstructiveStrengthQuadraticLoadModelMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation.ToNullAsNaN()
                },
                BankWidth =
                {
                    Mean = (RoundedDouble) entity.BankWidthMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.BankWidthStandardDeviation.ToNullAsNaN()
                },
                InsideWaterLevelFailureConstruction =
                {
                    Mean = (RoundedDouble) entity.InsideWaterLevelFailureConstructionMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.InsideWaterLevelFailureConstructionStandardDeviation.ToNullAsNaN()
                },
                EvaluationLevel = (RoundedDouble) entity.EvaluationLevel.ToNullAsNaN(),
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) entity.LevelCrestStructureMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.LevelCrestStructureStandardDeviation.ToNullAsNaN()
                },
                VerticalDistance = (RoundedDouble) entity.VerticalDistance.ToNullAsNaN(),
                FailureProbabilityRepairClosure = entity.FailureProbabilityRepairClosure.ToNullAsNaN(),
                FailureCollisionEnergy =
                {
                    Mean = (RoundedDouble) entity.FailureCollisionEnergyMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.FailureCollisionEnergyCoefficientOfVariation.ToNullAsNaN()
                },
                ShipMass =
                {
                    Mean = (RoundedDouble) entity.ShipMassMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.ShipMassCoefficientOfVariation.ToNullAsNaN()
                },
                ShipVelocity =
                {
                    Mean = (RoundedDouble) entity.ShipVelocityMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.ShipVelocityCoefficientOfVariation.ToNullAsNaN()
                },
                LevellingCount = entity.LevellingCount,
                ProbabilityCollisionSecondaryStructure = entity.ProbabilityCollisionSecondaryStructure.ToNullAsNaN(),
                FlowVelocityStructureClosable =
                {
                    Mean = (RoundedDouble) entity.FlowVelocityStructureClosableMean.ToNullAsNaN()
                },
                StabilityLinearLoadModel =
                {
                    Mean = (RoundedDouble) entity.StabilityLinearLoadModelMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.StabilityLinearLoadModelCoefficientOfVariation.ToNullAsNaN()
                },
                StabilityQuadraticLoadModel =
                {
                    Mean = (RoundedDouble) entity.StabilityQuadraticLoadModelMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.StabilityQuadraticLoadModelCoefficientOfVariation.ToNullAsNaN()
                },
                AreaFlowApertures =
                {
                    Mean = (RoundedDouble) entity.AreaFlowAperturesMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.AreaFlowAperturesStandardDeviation.ToNullAsNaN()
                },
                InflowModelType = (StabilityPointStructureInflowModelType) entity.InflowModelType
            });

            collector.Read(entity, structure);

            return structure;
        }
    }
}