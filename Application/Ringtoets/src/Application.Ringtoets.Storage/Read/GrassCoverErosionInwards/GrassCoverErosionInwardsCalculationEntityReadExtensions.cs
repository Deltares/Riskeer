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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Read.GrassCoverErosionInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GrassCoverErosionInwardsCalculation"/>
    /// based on the <see cref="GrassCoverErosionInwardsCalculationEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionInwardsCalculationEntity"/> and use the
        /// information to update a <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsCalculationEntity"/>
        /// to create <see cref="GrassCoverErosionInwardsCalculation"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsCalculation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static GrassCoverErosionInwardsCalculation Read(this GrassCoverErosionInwardsCalculationEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = entity.Name,
                Comments = entity.Comments,
                InputParameters =
                {
                    DikeProfile = GetDikeProfileValue(entity.DikeProfileEntity, collector),
                    HydraulicBoundaryLocation = GetHydraulicBoundaryLocationValue(entity.HydraulicLocationEntity, collector),
                    Orientation = (RoundedDouble) entity.Orientation.ToNullAsNaN(),
                    CriticalFlowRate =
                    {
                        Mean = (RoundedDouble) entity.CriticalFlowRateMean.ToNullAsNaN(),
                        StandardDeviation = (RoundedDouble) entity.CriticalFlowRateStandardDeviation.ToNullAsNaN()
                    },
                    UseForeshore = Convert.ToBoolean(entity.UseForeshore),
                    DikeHeight = (RoundedDouble) entity.DikeHeight.ToNullAsNaN(),
                    UseBreakWater = Convert.ToBoolean(entity.UseBreakWater),
                    BreakWater =
                    {
                        Height = (RoundedDouble) entity.BreakWaterHeight.ToNullAsNaN(),
                        Type = (BreakWaterType) entity.BreakWaterType
                    },
                    CalculateDikeHeight = Convert.ToBoolean(entity.CalculateDikeHeight)
                }
            };

            GrassCoverErosionInwardsOutputEntity output = entity.GrassCoverErosionInwardsOutputEntities.FirstOrDefault();
            if (output != null)
            {
                calculation.Output = output.Read();
            }

            collector.Read(entity, calculation);

            return calculation;
        }

        private static DikeProfile GetDikeProfileValue(DikeProfileEntity dikeProfileEntity, ReadConversionCollector collector)
        {
            if (dikeProfileEntity != null)
            {
                return dikeProfileEntity.Read(collector);
            }
            return null;
        }

        private static HydraulicBoundaryLocation GetHydraulicBoundaryLocationValue(HydraulicLocationEntity hydraulicLocationEntity, ReadConversionCollector collector)
        {
            if (hydraulicLocationEntity != null)
            {
                return hydraulicLocationEntity.Read(collector);
            }
            return null;
        }
    }
}