﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

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
                Comments =
                {
                    Comments = entity.Comments
                }
            };
            ReadInputParameters(calculation.InputParameters, entity, collector);
            ReadOutput(calculation, entity);

            return calculation;
        }

        private static void ReadInputParameters(WaveConditionsInput inputParameters, WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity, ReadConversionCollector collector)
        {
            inputParameters.ForeshoreProfile = GetDikeProfileValue(entity.ForeshoreProfileEntity, collector);
            inputParameters.HydraulicBoundaryLocation = GetHydraulicBoundaryLocationValue(entity.HydraulicLocationEntity, collector);
            inputParameters.Orientation = (RoundedDouble) entity.Orientation.ToNullAsNaN();
            inputParameters.UseForeshore = Convert.ToBoolean(entity.UseForeshore);
            inputParameters.UseBreakWater = Convert.ToBoolean(entity.UseBreakWater);
            inputParameters.BreakWater.Height = (RoundedDouble) entity.BreakWaterHeight.ToNullAsNaN();
            inputParameters.BreakWater.Type = (BreakWaterType) entity.BreakWaterType;
            inputParameters.UpperBoundaryRevetment = (RoundedDouble) entity.UpperBoundaryRevetment.ToNullAsNaN();
            inputParameters.LowerBoundaryRevetment = (RoundedDouble) entity.LowerBoundaryRevetment.ToNullAsNaN();
            inputParameters.UpperBoundaryWaterLevels = (RoundedDouble) entity.UpperBoundaryWaterLevels.ToNullAsNaN();
            inputParameters.LowerBoundaryWaterLevels = (RoundedDouble) entity.LowerBoundaryWaterLevels.ToNullAsNaN();
            inputParameters.StepSize = (WaveConditionsInputStepSize) entity.StepSize;
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

        private static void ReadOutput(WaveImpactAsphaltCoverWaveConditionsCalculation calculation, WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity)
        {
            if (entity.WaveImpactAsphaltCoverWaveConditionsOutputEntities.Any())
            {
                calculation.Output = new WaveImpactAsphaltCoverWaveConditionsOutput(entity.WaveImpactAsphaltCoverWaveConditionsOutputEntities
                                                                                          .OrderBy(oe => oe.Order)
                                                                                          .Select(oe => oe.Read()));
            }
        }
    }
}