// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Application.Ringtoets.Storage.Create.IllustrationPoints;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="StructuresOutput"/> related to creating structures 
    /// calculation output entities.
    /// </summary>
    internal static class StructuresOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="TOutputEntity"/> based on the information of the 
        /// <paramref name="calculationOutput"/>.
        /// </summary>
        /// <param name="calculationOutput">The calculation output to create a database entity for.</param>
        /// <returns>A new <see cref="TOutputEntity"/>.</returns>
        public static TOutputEntity Create<TOutputEntity>(this StructuresOutput calculationOutput)
            where TOutputEntity : IProbabilityAssessmentOutputEntity,
            IHasGeneralResultFaultTreeIllustrationPointEntity,
            new()
        {
            ProbabilityAssessmentOutput probabilityAssessmentOutput = calculationOutput.ProbabilityAssessmentOutput;
            var outputEntity = probabilityAssessmentOutput.Create<TOutputEntity>();

            SetGeneralResult(calculationOutput, outputEntity);

            return outputEntity;
        }

        private static void SetGeneralResult(StructuresOutput calculationOutput, IHasGeneralResultFaultTreeIllustrationPointEntity outputEntity)
        {
            if (calculationOutput.HasGeneralResult)
            {
                outputEntity.GeneralResultFaultTreeIllustrationPointEntity =
                    calculationOutput.GeneralResult
                                     .CreateGeneralResultFaultTreeIllustrationPointEntity();
            }
        }
    }
}