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
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Create.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionInwardsOutput"/> related to creating a 
    /// <see cref="GrassCoverErosionInwardsOutputEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverErosionInwardsOutputEntity"/> based on
        /// the information of the <see cref="GrassCoverErosionInwardsOutput"/>.
        /// </summary>
        /// <param name="output">The grass cover erosion inwards calculation output to create
        /// a database entity for.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsOutputEntity"/>.</returns>
        internal static GrassCoverErosionInwardsOutputEntity Create(this GrassCoverErosionInwardsOutput output)
        {
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                IsOvertoppingDominant = Convert.ToByte(output.IsOvertoppingDominant),
                WaveHeight = output.WaveHeight.ToNaNAsNull(),
                RequiredProbability = output.ProbabilityAssessmentOutput.RequiredProbability.ToNaNAsNull(),
                RequiredReliability = output.ProbabilityAssessmentOutput.RequiredReliability.ToNaNAsNull(),
                Probability = output.ProbabilityAssessmentOutput.Probability.ToNaNAsNull(),
                Reliability = output.ProbabilityAssessmentOutput.Reliability.ToNaNAsNull(),
                FactorOfSafety = output.ProbabilityAssessmentOutput.FactorOfSafety.ToNaNAsNull()
            };

            AddEntityForDikeHeightOutput(entity, output.DikeHeightOutput);
            AddEntityForOvertoppingRateOutput(entity, output.OvertoppingRateOutput);

            return entity;
        }

        private static void AddEntityForDikeHeightOutput(GrassCoverErosionInwardsOutputEntity entity,
                                                         HydraulicLoadsOutput output)
        {
            if (output != null)
            {
                entity.GrassCoverErosionInwardsDikeHeightOutputEntities.Add(output.CreateDikeHeight());
            }
        }

        private static void AddEntityForOvertoppingRateOutput(GrassCoverErosionInwardsOutputEntity entity,
                                                              HydraulicLoadsOutput output)
        {
            if (output != null)
            {
                entity.GrassCoverErosionInwardsOvertoppingRateOutputEntities.Add(output.CreateOvertoppingRate());
            }
        }
    }
}