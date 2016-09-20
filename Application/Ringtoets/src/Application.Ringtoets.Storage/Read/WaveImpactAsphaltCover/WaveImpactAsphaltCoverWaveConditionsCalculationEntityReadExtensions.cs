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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Application.Ringtoets.Storage.Read.WaveImpactAsphaltCover
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/>
    /// based on the <see cref="WaveImpactAsphaltCoverWaveConditionsCalculationEntity"/>.
    /// </summary>
    internal static class WaveImpactAsphaltCoverWaveConditionsCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionInwardsCalculationEntity"/> and use the
        /// information to update a <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsCalculationEntity"/>
        /// to create <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static WaveImpactAsphaltCoverWaveConditionsCalculation Read(this WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = entity.Name,
                Comments = entity.Comments,
                InputParameters =
                {
                    ForeshoreProfile = GetDikeProfileValue(entity.ForeshoreProfileEntity, collector),
                    HydraulicBoundaryLocation = GetHydraulicBoundaryLocationValue(entity.HydraulicLocationEntity, collector),
                    Orientation = (RoundedDouble) entity.Orientation.ToNullAsNaN(),
                    UseForeshore = Convert.ToBoolean(entity.UseForeshore),
                    UseBreakWater = Convert.ToBoolean(entity.UseBreakWater),
                    BreakWater =
                    {
                        Height = (RoundedDouble) entity.BreakWaterHeight.ToNullAsNaN(),
                        Type = (BreakWaterType) entity.BreakWaterType
                    },
                    UpperBoundaryRevetment = (RoundedDouble) entity.UpperBoundaryRevetment.ToNullAsNaN(),
                    LowerBoundaryRevetment = (RoundedDouble) entity.LowerBoundaryRevetment.ToNullAsNaN(),
                    UpperBoundaryWaterLevels = (RoundedDouble) entity.UpperBoundaryWaterLevels.ToNullAsNaN(),
                    LowerBoundaryWaterLevels = (RoundedDouble) entity.LowerBoundaryWaterLevels.ToNullAsNaN(),
                    StepSize = (WaveConditionsInputStepSize) entity.StepSize
                },
                Output = ReadCalculationOutputs(entity.WaveImpactAsphaltCoverWaveConditionsOutputEntities)
            };

            return calculation;
        }
        
        private static ForeshoreProfile GetDikeProfileValue(ForeshoreProfileEntity foreshoreProfileEntity, ReadConversionCollector collector)
        {
            if (foreshoreProfileEntity != null)
            {
                return foreshoreProfileEntity.Read(collector);
            }
            return null;
        }

        private static HydraulicBoundaryLocation GetHydraulicBoundaryLocationValue(
            HydraulicLocationEntity hydraulicLocationEntity,
            ReadConversionCollector collector)
        {
            if (hydraulicLocationEntity != null)
            {
                return hydraulicLocationEntity.Read(collector);
            }
            return null;
        }
        private static WaveImpactAsphaltCoverWaveConditionsOutput ReadCalculationOutputs(ICollection<WaveImpactAsphaltCoverWaveConditionsOutputEntity> waveImpactAsphaltCoverWaveConditionsOutputEntities)
        {
            if (waveImpactAsphaltCoverWaveConditionsOutputEntities.Any())
            {
                return new WaveImpactAsphaltCoverWaveConditionsOutput(waveImpactAsphaltCoverWaveConditionsOutputEntities
                    .OrderBy(oe => oe.Order)
                    .Select(oe => oe.Read()));
            }
            return null;
        }
    }
}