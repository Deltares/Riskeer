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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Structures;

namespace Application.Ringtoets.Storage.Create.ClosingStructures
{
    /// <summary>
    /// Extension methods for <see cref="StructuresCalculation{T}"/> related
    /// to creating a <see cref="ClosingStructuresCalculationEntity"/>.
    /// </summary>
    internal static class ClosingStructureCalculationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="ClosingStructuresCalculationEntity"/> based
        /// on the information of the <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at where <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <returns>A new <see cref="ClosingStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static ClosingStructuresCalculationEntity Create(this StructuresCalculation<ClosingStructuresInput> calculation, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new ClosingStructuresCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.DeepClone(),
                Order = order
            };
            SetInputValues(entity, calculation.InputParameters, registry);

            return entity;
        }

        private static void SetInputValues(ClosingStructuresCalculationEntity entity, ClosingStructuresInput input, PersistenceRegistry registry)
        {
            SetBaseStructureInputValues(entity, input, registry);
            SetClosingStructureSpecificInputValues(entity, input);
        }

        private static void SetBaseStructureInputValues(ClosingStructuresCalculationEntity entity, StructuresInputBase<ClosingStructure> input, PersistenceRegistry registry)
        {
            entity.StormDurationMean = input.StormDuration.Mean.Value.ToNaNAsNull();
            entity.StructureNormalOrientation = input.StructureNormalOrientation.Value.ToNaNAsNull();
            entity.FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion;

            if (input.Structure != null)
            {
                entity.ClosingStructureEntity = registry.Get(input.Structure);
            }

            if (input.HydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = registry.Get<HydraulicLocationEntity>(input.HydraulicBoundaryLocation);
            }

            if (input.ForeshoreProfile != null)
            {
                entity.ForeshoreProfileEntity = registry.Get(input.ForeshoreProfile);
            }
            entity.UseForeshore = Convert.ToByte(input.UseForeshore);

            entity.UseBreakWater = Convert.ToByte(input.UseBreakWater);
            entity.BreakWaterType = Convert.ToInt16(input.BreakWater.Type);
            entity.BreakWaterHeight = input.BreakWater.Height.Value.ToNaNAsNull();

            entity.AllowedLevelIncreaseStorageMean = input.AllowedLevelIncreaseStorage.Mean.Value.ToNaNAsNull();
            entity.AllowedLevelIncreaseStorageStandardDeviation = input.AllowedLevelIncreaseStorage.StandardDeviation.Value.ToNaNAsNull();

            entity.StorageStructureAreaMean = input.StorageStructureArea.Mean.Value.ToNaNAsNull();
            entity.StorageStructureAreaCoefficientOfVariation = input.StorageStructureArea.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.FlowWidthAtBottomProtectionMean = input.FlowWidthAtBottomProtection.Mean.Value.ToNaNAsNull();
            entity.FlowWidthAtBottomProtectionStandardDeviation = input.FlowWidthAtBottomProtection.StandardDeviation.Value.ToNaNAsNull();

            entity.CriticalOvertoppingDischargeMean = input.CriticalOvertoppingDischarge.Mean.Value.ToNaNAsNull();
            entity.CriticalOvertoppingDischargeCoefficientOfVariation = input.CriticalOvertoppingDischarge.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.ModelFactorSuperCriticalFlowMean = input.ModelFactorSuperCriticalFlow.Mean.Value.ToNaNAsNull();

            entity.WidthFlowAperturesMean = input.WidthFlowApertures.Mean.Value.ToNaNAsNull();
            entity.WidthFlowAperturesCoefficientOfVariation = input.WidthFlowApertures.CoefficientOfVariation.Value.ToNaNAsNull();
        }

        private static void SetClosingStructureSpecificInputValues(ClosingStructuresCalculationEntity entity, ClosingStructuresInput input)
        {
            entity.InflowModelType = Convert.ToByte(input.InflowModelType);

            entity.InsideWaterLevelMean = input.InsideWaterLevel.Mean.Value.ToNaNAsNull();
            entity.InsideWaterLevelStandardDeviation = input.InsideWaterLevel.StandardDeviation.Value.ToNaNAsNull();

            entity.DeviationWaveDirection = input.DeviationWaveDirection.Value.ToNaNAsNull();

            entity.DrainCoefficientMean = input.DrainCoefficient.Mean.Value.ToNaNAsNull();

            entity.FactorStormDurationOpenStructure = input.FactorStormDurationOpenStructure.Value.ToNaNAsNull();

            entity.ThresholdHeightOpenWeirMean = input.ThresholdHeightOpenWeir.Mean.Value.ToNaNAsNull();
            entity.ThresholdHeightOpenWeirStandardDeviation = input.ThresholdHeightOpenWeir.StandardDeviation.Value.ToNaNAsNull();

            entity.AreaFlowAperturesMean = input.AreaFlowApertures.Mean.Value.ToNaNAsNull();
            entity.AreaFlowAperturesStandardDeviation = input.AreaFlowApertures.StandardDeviation.Value.ToNaNAsNull();

            entity.FailureProbabilityOpenStructure = input.FailureProbabilityOpenStructure;

            entity.FailureProbablityReparation = input.FailureProbabilityReparation;

            entity.IdenticalApertures = input.IdenticalApertures;

            entity.LevelCrestStructureNotClosingMean = input.LevelCrestStructureNotClosing.Mean.Value.ToNaNAsNull();
            entity.LevelCrestStructureNotClosingStandardDeviation = input.LevelCrestStructureNotClosing.StandardDeviation.Value.ToNaNAsNull();

            entity.ProbabilityOpenStructureBeforeFlooding = input.ProbabilityOpenStructureBeforeFlooding;
        }
    }
}