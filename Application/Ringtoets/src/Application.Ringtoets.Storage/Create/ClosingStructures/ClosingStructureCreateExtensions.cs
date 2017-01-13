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

namespace Application.Ringtoets.Storage.Create.ClosingStructures
{
    /// <summary>
    /// Extension methods for <see cref="ClosingStructure"/> related to creating
    /// a <see cref="ClosingStructureEntity"/>.
    /// </summary>
    internal static class ClosingStructureCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="ClosingStructureEntity"/> based on the information of the <see cref="ClosingStructure"/>.
        /// </summary>
        /// <param name="structure">The structure to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="structure"/> resides within its parent.</param>
        /// <returns>A new <see cref="ClosingStructureEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static ClosingStructureEntity Create(this ClosingStructure structure, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (registry.Contains(structure))
            {
                return registry.Get(structure);
            }

            var entity = new ClosingStructureEntity
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
                LevelCrestStructureNotClosingMean = structure.LevelCrestStructureNotClosing.Mean.Value.ToNaNAsNull(),
                LevelCrestStructureNotClosingStandardDeviation = structure.LevelCrestStructureNotClosing.StandardDeviation.Value.ToNaNAsNull(),
                InsideWaterLevelMean = structure.InsideWaterLevel.Mean.Value.ToNaNAsNull(),
                InsideWaterLevelStandardDeviation = structure.InsideWaterLevel.StandardDeviation.Value.ToNaNAsNull(),
                ThresholdHeightOpenWeirMean = structure.ThresholdHeightOpenWeir.Mean.Value.ToNaNAsNull(),
                ThresholdHeightOpenWeirStandardDeviation = structure.ThresholdHeightOpenWeir.StandardDeviation.Value.ToNaNAsNull(),
                AreaFlowAperturesMean = structure.AreaFlowApertures.Mean.Value.ToNaNAsNull(),
                AreaFlowAperturesStandardDeviation = structure.AreaFlowApertures.StandardDeviation.Value.ToNaNAsNull(),
                CriticalOvertoppingDischargeMean = structure.CriticalOvertoppingDischarge.Mean.Value.ToNaNAsNull(),
                CriticalOvertoppingDischargeCoefficientOfVariation = structure.CriticalOvertoppingDischarge.CoefficientOfVariation.Value.ToNaNAsNull(),
                FlowWidthAtBottomProtectionMean = structure.FlowWidthAtBottomProtection.Mean.Value.ToNaNAsNull(),
                FlowWidthAtBottomProtectionStandardDeviation = structure.FlowWidthAtBottomProtection.StandardDeviation.Value.ToNaNAsNull(),
                ProbabilityOrFrequencyOpenStructureBeforeFlooding = structure.ProbabilityOrFrequencyOpenStructureBeforeFlooding.ToNaNAsNull(),
                FailureProbabilityOpenStructure = structure.FailureProbabilityOpenStructure.ToNaNAsNull(),
                IdenticalApertures = structure.IdenticalApertures,
                FailureProbabilityReparation = structure.FailureProbabilityReparation.ToNaNAsNull(),
                InflowModelType = Convert.ToByte(structure.InflowModelType),
                Order = order
            };

            registry.Register(entity, structure);

            return entity;
        }
    }
}