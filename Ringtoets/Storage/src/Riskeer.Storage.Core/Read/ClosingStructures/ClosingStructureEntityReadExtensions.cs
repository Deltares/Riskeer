// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.ClosingStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="ClosingStructure"/>
    /// based on the <see cref="ClosingStructureEntity"/>.
    /// </summary>
    internal static class ClosingStructureEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="ClosingStructureEntity"/> and use the information to update a 
        /// <see cref="ClosingStructure"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructureEntity"/> to create <see cref="ClosingStructure"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="ClosingStructure"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static ClosingStructure Read(this ClosingStructureEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var structure = new ClosingStructure(new ClosingStructure.ConstructionProperties
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
                WidthFlowApertures =
                {
                    Mean = (RoundedDouble) entity.WidthFlowAperturesMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.WidthFlowAperturesStandardDeviation.ToNullAsNaN()
                },
                LevelCrestStructureNotClosing =
                {
                    Mean = (RoundedDouble) entity.LevelCrestStructureNotClosingMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.LevelCrestStructureNotClosingStandardDeviation.ToNullAsNaN()
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
                AreaFlowApertures =
                {
                    Mean = (RoundedDouble) entity.AreaFlowAperturesMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.AreaFlowAperturesStandardDeviation.ToNullAsNaN()
                },
                CriticalOvertoppingDischarge =
                {
                    Mean = (RoundedDouble) entity.CriticalOvertoppingDischargeMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.CriticalOvertoppingDischargeCoefficientOfVariation.ToNullAsNaN()
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) entity.FlowWidthAtBottomProtectionMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.FlowWidthAtBottomProtectionStandardDeviation.ToNullAsNaN()
                },
                ProbabilityOpenStructureBeforeFlooding = entity.ProbabilityOpenStructureBeforeFlooding.ToNullAsNaN(),
                FailureProbabilityOpenStructure = entity.FailureProbabilityOpenStructure.ToNullAsNaN(),
                IdenticalApertures = entity.IdenticalApertures,
                FailureProbabilityReparation = entity.FailureProbabilityReparation.ToNullAsNaN(),
                InflowModelType = (ClosingStructureInflowModelType) entity.InflowModelType
            });

            collector.Read(entity, structure);

            return structure;
        }
    }
}