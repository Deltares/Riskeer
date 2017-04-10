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
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Read.GrassCoverErosionInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GrassCoverErosionInwardsOutput"/>
    /// based on the <see cref="GrassCoverErosionInwardsOutputEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsOutputEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionInwardsOutputEntity"/>
        /// and use the information to construct a <see cref="GrassCoverErosionInwardsOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsOutputEntity"/>
        /// to create <see cref="GeneralGrassCoverErosionInwardsInput"/> for.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsOutput"/>.</returns>
        internal static GrassCoverErosionInwardsOutput Read(this GrassCoverErosionInwardsOutputEntity entity)
        {
            return new GrassCoverErosionInwardsOutput(entity.WaveHeight.ToNullAsNaN(),
                                                      Convert.ToBoolean(entity.IsOvertoppingDominant),
                                                      ReadProbabilityAssessmentOutput(entity),
                                                      GetDikeHeightAssessmentOutput(entity),
                                                      null);
        }

        private static ProbabilityAssessmentOutput ReadProbabilityAssessmentOutput(GrassCoverErosionInwardsOutputEntity entity)
        {
            return new ProbabilityAssessmentOutput(entity.RequiredProbability.ToNullAsNaN(),
                                                   entity.RequiredReliability.ToNullAsNaN(),
                                                   entity.Probability.ToNullAsNaN(),
                                                   entity.Reliability.ToNullAsNaN(),
                                                   entity.FactorOfSafety.ToNullAsNaN());
        }

        private static DikeHeightAssessmentOutput GetDikeHeightAssessmentOutput(GrassCoverErosionInwardsOutputEntity entity)
        {
            GrassCoverErosionInwardsDikeHeightOutputEntity dikeHeightOutputEntity = entity.GrassCoverErosionInwardsDikeHeightOutputEntities.FirstOrDefault();
            return dikeHeightOutputEntity != null ? dikeHeightOutputEntity.Read() : null;
        }
    }
}