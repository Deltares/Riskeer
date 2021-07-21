﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.GrassCoverErosionInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GrassCoverErosionInwardsCalculationScenario"/>
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
        internal static GrassCoverErosionInwardsCalculationScenario Read(this GrassCoverErosionInwardsCalculationEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var calculation = new GrassCoverErosionInwardsCalculationScenario(0.1)
            {
                Name = entity.Name,
                Comments =
                {
                    Body = entity.Comments
                },
                IsRelevant = Convert.ToBoolean(entity.RelevantForScenario),
                Contribution = (RoundedDouble) entity.ScenarioContribution
            };
            ReadInput(calculation.InputParameters, entity, collector);
            ReadOutput(calculation, entity);

            return calculation;
        }

        private static void ReadInput(GrassCoverErosionInwardsInput inputParameters, GrassCoverErosionInwardsCalculationEntity entity, ReadConversionCollector collector)
        {
            inputParameters.DikeProfile = GetDikeProfileValue(entity.DikeProfileEntity, collector);
            inputParameters.HydraulicBoundaryLocation = GetHydraulicBoundaryLocationValue(entity.HydraulicLocationEntity, collector);
            inputParameters.Orientation = (RoundedDouble) entity.Orientation.ToNullAsNaN();
            inputParameters.CriticalFlowRate.Mean = (RoundedDouble) entity.CriticalFlowRateMean.ToNullAsNaN();
            inputParameters.CriticalFlowRate.StandardDeviation = (RoundedDouble) entity.CriticalFlowRateStandardDeviation.ToNullAsNaN();
            inputParameters.UseForeshore = Convert.ToBoolean(entity.UseForeshore);
            inputParameters.DikeHeight = (RoundedDouble) entity.DikeHeight.ToNullAsNaN();
            inputParameters.UseBreakWater = Convert.ToBoolean(entity.UseBreakWater);
            inputParameters.BreakWater.Height = (RoundedDouble) entity.BreakWaterHeight.ToNullAsNaN();
            inputParameters.BreakWater.Type = (BreakWaterType) entity.BreakWaterType;
            inputParameters.DikeHeightCalculationType = (DikeHeightCalculationType) entity.DikeHeightCalculationType;
            inputParameters.OvertoppingRateCalculationType = (OvertoppingRateCalculationType) entity.OvertoppingRateCalculationType;
            inputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated = Convert.ToBoolean(entity.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            inputParameters.ShouldDikeHeightIllustrationPointsBeCalculated = Convert.ToBoolean(entity.ShouldDikeHeightIllustrationPointsBeCalculated);
            inputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated = Convert.ToBoolean(entity.ShouldOvertoppingRateIllustrationPointsBeCalculated);
        }

        private static void ReadOutput(GrassCoverErosionInwardsCalculation calculation, GrassCoverErosionInwardsCalculationEntity entity)
        {
            GrassCoverErosionInwardsOutputEntity output = entity.GrassCoverErosionInwardsOutputEntities.SingleOrDefault();
            if (output != null)
            {
                calculation.Output = output.Read();
            }
        }

        private static DikeProfile GetDikeProfileValue(DikeProfileEntity dikeProfileEntity, ReadConversionCollector collector)
        {
            return dikeProfileEntity?.Read(collector);
        }

        private static HydraulicBoundaryLocation GetHydraulicBoundaryLocationValue(HydraulicLocationEntity hydraulicLocationEntity, ReadConversionCollector collector)
        {
            return hydraulicLocationEntity?.Read(collector);
        }
    }
}