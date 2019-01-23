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
using Ringtoets.GrassCoverErosionInwards.Data;
using Riskeer.Storage.Core.Create.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.GrassCoverErosionInwards
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
            OvertoppingOutput overtoppingOutput = output.OvertoppingOutput;
            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                IsOvertoppingDominant = Convert.ToByte(overtoppingOutput.IsOvertoppingDominant),
                WaveHeight = overtoppingOutput.WaveHeight.ToNaNAsNull(),
                Reliability = overtoppingOutput.Reliability.ToNaNAsNull(),
                GeneralResultFaultTreeIllustrationPointEntity = overtoppingOutput.GeneralResult?.CreateGeneralResultFaultTreeIllustrationPointEntity()
            };

            AddEntityForDikeHeightOutput(entity, output.DikeHeightOutput);
            AddEntityForOvertoppingRateOutput(entity, output.OvertoppingRateOutput);

            return entity;
        }

        private static void AddEntityForDikeHeightOutput(GrassCoverErosionInwardsOutputEntity entity,
                                                         DikeHeightOutput output)
        {
            if (output != null)
            {
                entity.GrassCoverErosionInwardsDikeHeightOutputEntities.Add(output.Create());
            }
        }

        private static void AddEntityForOvertoppingRateOutput(GrassCoverErosionInwardsOutputEntity entity,
                                                              OvertoppingRateOutput output)
        {
            if (output != null)
            {
                entity.GrassCoverErosionInwardsOvertoppingRateOutputEntities.Add(output.Create());
            }
        }
    }
}