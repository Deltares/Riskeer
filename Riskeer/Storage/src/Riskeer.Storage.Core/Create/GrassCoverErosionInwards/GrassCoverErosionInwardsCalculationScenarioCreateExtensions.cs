﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionInwardsCalculation"/> related
    /// to creating a <see cref="GrassCoverErosionInwardsCalculationEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsCalculationScenarioCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverErosionInwardsCalculationEntity"/> based
        /// on the information of the <see cref="GrassCoverErosionInwardsCalculationScenario"/>.
        /// </summary>
        /// <param name="calculation">The calculation scenario to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at where <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static GrassCoverErosionInwardsCalculationEntity Create(this GrassCoverErosionInwardsCalculationScenario calculation, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.Body.DeepClone(),
                RelevantForScenario = Convert.ToByte(calculation.IsRelevant),
                ScenarioContribution = calculation.Contribution,
                Order = order
            };
            SetInputValues(entity, calculation.InputParameters, registry);
            SetOutputEntity(entity, calculation);

            return entity;
        }

        private static void SetOutputEntity(GrassCoverErosionInwardsCalculationEntity entity, GrassCoverErosionInwardsCalculation calculation)
        {
            if (calculation.HasOutput)
            {
                entity.GrassCoverErosionInwardsOutputEntities.Add(calculation.Output.Create());
            }
        }

        private static void SetInputValues(GrassCoverErosionInwardsCalculationEntity entity, GrassCoverErosionInwardsInput input, PersistenceRegistry registry)
        {
            if (input.DikeProfile != null)
            {
                entity.DikeProfileEntity = registry.Get(input.DikeProfile);
            }

            if (input.HydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = registry.Get(input.HydraulicBoundaryLocation);
            }

            entity.Orientation = input.Orientation.ToNaNAsNull();
            entity.DikeHeight = input.DikeHeight.ToNaNAsNull();
            entity.UseForeshore = Convert.ToByte(input.UseForeshore);
            entity.UseBreakWater = Convert.ToByte(input.UseBreakWater);
            entity.BreakWaterType = Convert.ToByte(input.BreakWater.Type);
            entity.BreakWaterHeight = input.BreakWater.Height.ToNaNAsNull();
            entity.CriticalFlowRateMean = input.CriticalFlowRate.Mean.ToNaNAsNull();
            entity.CriticalFlowRateStandardDeviation = input.CriticalFlowRate.StandardDeviation.ToNaNAsNull();
            entity.ShouldOvertoppingOutputIllustrationPointsBeCalculated = Convert.ToByte(input.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            entity.ShouldDikeHeightBeCalculated = Convert.ToByte(input.ShouldDikeHeightBeCalculated);
            entity.DikeHeightTargetProbability = input.DikeHeightTargetProbability;
            entity.ShouldDikeHeightIllustrationPointsBeCalculated = Convert.ToByte(input.ShouldDikeHeightIllustrationPointsBeCalculated);
            entity.ShouldOvertoppingRateBeCalculated = Convert.ToByte(input.ShouldOvertoppingRateBeCalculated);
            entity.OvertoppingRateTargetProbability = input.OvertoppingRateTargetProbability;
            entity.ShouldOvertoppingRateIllustrationPointsBeCalculated = Convert.ToByte(input.ShouldOvertoppingRateIllustrationPointsBeCalculated);
        }
    }
}