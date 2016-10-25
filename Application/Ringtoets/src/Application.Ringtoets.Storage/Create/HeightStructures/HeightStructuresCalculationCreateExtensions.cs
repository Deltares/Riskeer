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
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;

namespace Application.Ringtoets.Storage.Create.HeightStructures
{
    /// <summary>
    /// Extension methods for <see cref="StructuresCalculation{T}"/> related
    /// to creating a <see cref="HeightStructuresCalculationEntity"/>.
    /// </summary>
    internal static class HeightStructuresCalculationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HeightStructuresCalculationEntity"/> based
        /// on the information of the <see cref="StructuresCalculation{T}"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at where <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <returns>A new <see cref="HeightStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static HeightStructuresCalculationEntity Create(this StructuresCalculation<HeightStructuresInput> calculation, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new HeightStructuresCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.DeepClone(),
                Order = order
            };
            SetInputValues(entity, calculation.InputParameters, registry);

            return entity;
        }

        private static void SetInputValues(HeightStructuresCalculationEntity entity, HeightStructuresInput input, PersistenceRegistry registry)
        {
            if (input.HydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = registry.Get<HydraulicLocationEntity>(input.HydraulicBoundaryLocation);
            }
            if (input.Structure != null)
            {
                entity.HeightStructureEntity = registry.Get(input.Structure);
            }
            if (input.ForeshoreProfile != null)
            {
                entity.ForeshoreProfileEntity = registry.Get(input.ForeshoreProfile);
            }
            entity.StructureNormalOrientation = input.StructureNormalOrientation.Value.ToNaNAsNull();

            entity.ModelFactorSuperCriticalFlowMean = input.ModelFactorSuperCriticalFlow.Mean.Value.ToNaNAsNull();
            entity.AllowedLevelIncreaseStorageMean = input.AllowedLevelIncreaseStorage.Mean.Value.ToNaNAsNull();
            entity.AllowedLevelIncreaseStorageStandardDeviation = input.AllowedLevelIncreaseStorage.StandardDeviation.Value.ToNaNAsNull();

            entity.StorageStructureAreaMean = input.StorageStructureArea.Mean.Value.ToNaNAsNull();
            entity.StorageStructureAreaCoefficientOfVariation = input.StorageStructureArea.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.FlowWidthAtBottomProtectionMean = input.FlowWidthAtBottomProtection.Mean.Value.ToNaNAsNull();
            entity.FlowWidthAtBottomProtectionStandardDeviation = input.FlowWidthAtBottomProtection.StandardDeviation.Value.ToNaNAsNull();

            entity.CriticalOvertoppingDischargeMean = input.CriticalOvertoppingDischarge.Mean.Value.ToNaNAsNull();
            entity.CriticalOvertoppingDischargeCoefficientOfVariation = input.CriticalOvertoppingDischarge.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion.ToNaNAsNull();

            entity.WidthFlowAperturesMean = input.WidthFlowApertures.Mean.Value.ToNaNAsNull();
            entity.WidthFlowAperturesCoefficientOfVariation = input.WidthFlowApertures.CoefficientOfVariation.Value.ToNaNAsNull();

            entity.StormDurationMean = input.StormDuration.Mean.Value.ToNaNAsNull();
            entity.LevelCrestStructureMean = input.LevelCrestStructure.Mean.Value.ToNaNAsNull();
            entity.LevelCrestStructureStandardDeviation = input.LevelCrestStructure.StandardDeviation.Value.ToNaNAsNull();
            entity.DeviationWaveDirection = input.DeviationWaveDirection.Value.ToNaNAsNull();

            entity.BreakWaterHeight = input.BreakWater.Height.Value.ToNaNAsNull();
            entity.BreakWaterType = Convert.ToInt16(input.BreakWater.Type);
            entity.UseBreakWater = Convert.ToByte(input.UseBreakWater);
            entity.UseForeshore = Convert.ToByte(input.UseForeshore);
        }
    }
}