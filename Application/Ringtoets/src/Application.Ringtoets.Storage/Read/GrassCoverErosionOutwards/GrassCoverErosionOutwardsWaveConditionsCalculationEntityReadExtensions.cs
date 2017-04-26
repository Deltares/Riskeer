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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Revetment.Data;

namespace Application.Ringtoets.Storage.Read.GrassCoverErosionOutwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>
    /// based on the <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionOutwardsWaveConditionsCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionInwardsCalculationEntity"/> and use the
        /// information to update a <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsCalculationEntity"/>
        /// to create <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static GrassCoverErosionOutwardsWaveConditionsCalculation Read(this GrassCoverErosionOutwardsWaveConditionsCalculationEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = entity.Name,
                Comments =
                {
                    Body = entity.Comments
                }
            };
            ReadCalculationInputs(calculation.InputParameters, entity, collector);
            ReadCalculationOutputs(calculation, entity);

            return calculation;
        }

        private static void ReadCalculationInputs(WaveConditionsInput inputParameters, GrassCoverErosionOutwardsWaveConditionsCalculationEntity entity, ReadConversionCollector collector)
        {
            inputParameters.ForeshoreProfile = GetDikeProfileValue(entity.ForeshoreProfileEntity, collector);
            inputParameters.HydraulicBoundaryLocation = GetHydraulicBoundaryLocationValue(entity.GrassCoverErosionOutwardsHydraulicLocationEntity, collector);
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

        private static void ReadCalculationOutputs(GrassCoverErosionOutwardsWaveConditionsCalculation calculation, GrassCoverErosionOutwardsWaveConditionsCalculationEntity entity)
        {
            if (!entity.GrassCoverErosionOutwardsWaveConditionsOutputEntities.Any())
            {
                return;
            }

            List<WaveConditionsOutput> waveConditionsOutputs = entity.GrassCoverErosionOutwardsWaveConditionsOutputEntities.OrderBy(e => e.Order).Select(e => e.Read()).ToList();
            calculation.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(waveConditionsOutputs);
        }

        private static ForeshoreProfile GetDikeProfileValue(ForeshoreProfileEntity foreshoreProfileEntity, ReadConversionCollector collector)
        {
            return foreshoreProfileEntity?.Read(collector);
        }

        private static HydraulicBoundaryLocation GetHydraulicBoundaryLocationValue(
            GrassCoverErosionOutwardsHydraulicLocationEntity hydraulicLocationEntity,
            ReadConversionCollector collector)
        {
            return hydraulicLocationEntity?.Read(collector);
        }
    }
}