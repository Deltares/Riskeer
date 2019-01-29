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
using Riskeer.HeightStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.HeightStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="HeightStructure"/>
    /// based on the <see cref="HeightStructureEntity"/>.
    /// </summary>
    internal static class HeightStructureEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="HeightStructureEntity"/> and use the information to update a 
        /// <see cref="HeightStructure"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructureEntity"/> to create <see cref="HeightStructure"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="HeightStructure"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static HeightStructure Read(this HeightStructureEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var structure = new HeightStructure(new HeightStructure.ConstructionProperties
            {
                Name = entity.Name,
                Id = entity.Id,
                Location = new Point2D(entity.X.ToNullAsNaN(), entity.Y.ToNullAsNaN()),
                StructureNormalOrientation = (RoundedDouble) entity.StructureNormalOrientation.ToNullAsNaN(),
                LevelCrestStructure =
                {
                    Mean = (RoundedDouble) entity.LevelCrestStructureMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.LevelCrestStructureStandardDeviation.ToNullAsNaN()
                },
                FlowWidthAtBottomProtection =
                {
                    Mean = (RoundedDouble) entity.FlowWidthAtBottomProtectionMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.FlowWidthAtBottomProtectionStandardDeviation.ToNullAsNaN()
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
                FailureProbabilityStructureWithErosion = entity.FailureProbabilityStructureWithErosion.ToNullAsNaN(),
                StorageStructureArea =
                {
                    Mean = (RoundedDouble) entity.StorageStructureAreaMean.ToNullAsNaN(),
                    CoefficientOfVariation = (RoundedDouble) entity.StorageStructureAreaCoefficientOfVariation.ToNullAsNaN()
                },
                AllowedLevelIncreaseStorage =
                {
                    Mean = (RoundedDouble) entity.AllowedLevelIncreaseStorageMean.ToNullAsNaN(),
                    StandardDeviation = (RoundedDouble) entity.AllowedLevelIncreaseStorageStandardDeviation.ToNullAsNaN()
                }
            });

            collector.Read(entity, structure);

            return structure;
        }
    }
}