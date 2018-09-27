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
using Core.Common.Util.Extensions;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.ClosingStructures
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
                throw new ArgumentNullException(nameof(registry));
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
                StructureNormalOrientation = structure.StructureNormalOrientation.ToNaNAsNull(),
                StorageStructureAreaMean = structure.StorageStructureArea.Mean.ToNaNAsNull(),
                StorageStructureAreaCoefficientOfVariation = structure.StorageStructureArea.CoefficientOfVariation.ToNaNAsNull(),
                AllowedLevelIncreaseStorageMean = structure.AllowedLevelIncreaseStorage.Mean.ToNaNAsNull(),
                AllowedLevelIncreaseStorageStandardDeviation = structure.AllowedLevelIncreaseStorage.StandardDeviation.ToNaNAsNull(),
                WidthFlowAperturesMean = structure.WidthFlowApertures.Mean.ToNaNAsNull(),
                WidthFlowAperturesStandardDeviation = structure.WidthFlowApertures.StandardDeviation.ToNaNAsNull(),
                LevelCrestStructureNotClosingMean = structure.LevelCrestStructureNotClosing.Mean.ToNaNAsNull(),
                LevelCrestStructureNotClosingStandardDeviation = structure.LevelCrestStructureNotClosing.StandardDeviation.ToNaNAsNull(),
                InsideWaterLevelMean = structure.InsideWaterLevel.Mean.ToNaNAsNull(),
                InsideWaterLevelStandardDeviation = structure.InsideWaterLevel.StandardDeviation.ToNaNAsNull(),
                ThresholdHeightOpenWeirMean = structure.ThresholdHeightOpenWeir.Mean.ToNaNAsNull(),
                ThresholdHeightOpenWeirStandardDeviation = structure.ThresholdHeightOpenWeir.StandardDeviation.ToNaNAsNull(),
                AreaFlowAperturesMean = structure.AreaFlowApertures.Mean.ToNaNAsNull(),
                AreaFlowAperturesStandardDeviation = structure.AreaFlowApertures.StandardDeviation.ToNaNAsNull(),
                CriticalOvertoppingDischargeMean = structure.CriticalOvertoppingDischarge.Mean.ToNaNAsNull(),
                CriticalOvertoppingDischargeCoefficientOfVariation = structure.CriticalOvertoppingDischarge.CoefficientOfVariation.ToNaNAsNull(),
                FlowWidthAtBottomProtectionMean = structure.FlowWidthAtBottomProtection.Mean.ToNaNAsNull(),
                FlowWidthAtBottomProtectionStandardDeviation = structure.FlowWidthAtBottomProtection.StandardDeviation.ToNaNAsNull(),
                ProbabilityOpenStructureBeforeFlooding = structure.ProbabilityOpenStructureBeforeFlooding.ToNaNAsNull(),
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