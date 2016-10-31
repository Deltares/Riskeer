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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="StructuresCalculation{T}"/> related
    /// to creating structures calculation entities.
    /// </summary>
    internal static class StructuresCalculationCreateExtensions
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
        internal static HeightStructuresCalculationEntity CreateForHeightStructures(this StructuresCalculation<HeightStructuresInput> calculation, PersistenceRegistry registry, int order)
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

            if (calculation.HasOutput)
            {
                entity.HeightStructuresOutputEntities.Add(calculation.Output.Create<HeightStructuresOutputEntity>(registry));
            }

            registry.Register(entity, calculation);

            return entity;
        }

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
        internal static ClosingStructuresCalculationEntity CreateForClosingStructures(this StructuresCalculation<ClosingStructuresInput> calculation, PersistenceRegistry registry, int order)
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

            if (calculation.HasOutput)
            {
                entity.ClosingStructuresOutputEntities.Add(calculation.Output.Create<ClosingStructuresOutputEntity>(registry));
            }

            registry.Register(entity, calculation);

            return entity;
        }

        private static void SetInputValues(HeightStructuresCalculationEntity entity, HeightStructuresInput input, PersistenceRegistry registry)
        {
            input.Create(entity, registry);

            if (input.Structure != null)
            {
                entity.HeightStructureEntity = registry.Get(input.Structure);
            }

            entity.LevelCrestStructureMean = input.LevelCrestStructure.Mean.Value.ToNaNAsNull();
            entity.LevelCrestStructureStandardDeviation = input.LevelCrestStructure.StandardDeviation.Value.ToNaNAsNull();

            entity.DeviationWaveDirection = input.DeviationWaveDirection.Value.ToNaNAsNull();
        }

        private static void SetInputValues(ClosingStructuresCalculationEntity entity, ClosingStructuresInput input, PersistenceRegistry registry)
        {
            input.Create(entity, registry);

            if (input.Structure != null)
            {
                entity.ClosingStructureEntity = registry.Get(input.Structure);
            }

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

            entity.FailureProbabilityReparation = input.FailureProbabilityReparation;

            entity.IdenticalApertures = input.IdenticalApertures;

            entity.LevelCrestStructureNotClosingMean = input.LevelCrestStructureNotClosing.Mean.Value.ToNaNAsNull();
            entity.LevelCrestStructureNotClosingStandardDeviation = input.LevelCrestStructureNotClosing.StandardDeviation.Value.ToNaNAsNull();

            entity.ProbabilityOpenStructureBeforeFlooding = input.ProbabilityOpenStructureBeforeFlooding;
        }

        private static void Create<T>(this StructuresInputBase<T> input, IStructuresCalculationEntity entityToUpdate, PersistenceRegistry registry)
            where T : StructureBase
        {
            if (entityToUpdate == null)
            {
                throw new ArgumentNullException("entityToUpdate");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            entityToUpdate.StormDurationMean = input.StormDuration.Mean.Value.ToNaNAsNull();
            entityToUpdate.StructureNormalOrientation = input.StructureNormalOrientation.Value.ToNaNAsNull();
            entityToUpdate.FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion;

            if (input.HydraulicBoundaryLocation != null)
            {
                entityToUpdate.HydraulicLocationEntity = registry.Get<HydraulicLocationEntity>(input.HydraulicBoundaryLocation);
            }

            if (input.ForeshoreProfile != null)
            {
                entityToUpdate.ForeshoreProfileEntity = registry.Get(input.ForeshoreProfile);
            }
            entityToUpdate.UseForeshore = Convert.ToByte(input.UseForeshore);

            entityToUpdate.UseBreakWater = Convert.ToByte(input.UseBreakWater);
            entityToUpdate.BreakWaterType = Convert.ToInt16(input.BreakWater.Type);
            entityToUpdate.BreakWaterHeight = input.BreakWater.Height.Value.ToNaNAsNull();

            entityToUpdate.AllowedLevelIncreaseStorageMean = input.AllowedLevelIncreaseStorage.Mean.Value.ToNaNAsNull();
            entityToUpdate.AllowedLevelIncreaseStorageStandardDeviation = input.AllowedLevelIncreaseStorage.StandardDeviation.Value.ToNaNAsNull();

            entityToUpdate.StorageStructureAreaMean = input.StorageStructureArea.Mean.Value.ToNaNAsNull();
            entityToUpdate.StorageStructureAreaCoefficientOfVariation = input.StorageStructureArea.CoefficientOfVariation.Value.ToNaNAsNull();

            entityToUpdate.FlowWidthAtBottomProtectionMean = input.FlowWidthAtBottomProtection.Mean.Value.ToNaNAsNull();
            entityToUpdate.FlowWidthAtBottomProtectionStandardDeviation = input.FlowWidthAtBottomProtection.StandardDeviation.Value.ToNaNAsNull();

            entityToUpdate.CriticalOvertoppingDischargeMean = input.CriticalOvertoppingDischarge.Mean.Value.ToNaNAsNull();
            entityToUpdate.CriticalOvertoppingDischargeCoefficientOfVariation = input.CriticalOvertoppingDischarge.CoefficientOfVariation.Value.ToNaNAsNull();

            entityToUpdate.ModelFactorSuperCriticalFlowMean = input.ModelFactorSuperCriticalFlow.Mean.Value.ToNaNAsNull();

            entityToUpdate.WidthFlowAperturesMean = input.WidthFlowApertures.Mean.Value.ToNaNAsNull();
            entityToUpdate.WidthFlowAperturesCoefficientOfVariation = input.WidthFlowApertures.CoefficientOfVariation.Value.ToNaNAsNull();
        }
    }
}