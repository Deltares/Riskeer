// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Storage.Core.Create.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.Piping.Probabilistic
{
    /// <summary>
    /// Extension methods for <see cref="ProbabilisticPipingOutput"/> related to creating
    /// a <see cref="ProbabilisticPipingCalculationOutputEntity"/>.
    /// </summary>
    internal static class ProbabilisticPipingOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="ProbabilisticPipingCalculationOutputEntity"/> based on the information
        /// of the <see cref="ProbabilisticPipingOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for piping failure mechanism to 
        /// create a database entity for.</param>
        /// <returns>A new <see cref="ProbabilisticPipingCalculationOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/>
        /// is <c>null</c>.</exception>
        public static ProbabilisticPipingCalculationOutputEntity Create(this ProbabilisticPipingOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var outputEntity = new ProbabilisticPipingCalculationOutputEntity
            {
                ProfileSpecificReliability = output.ProfileSpecificOutput.Reliability.ToNaNAsNull(),
                SectionSpecificReliability = output.SectionSpecificOutput.Reliability.ToNaNAsNull()
            };

            if (output.ProfileSpecificOutput is PartialProbabilisticFaultTreePipingOutput profileSpecificFaultTreePipingOutput
                && output.SectionSpecificOutput is PartialProbabilisticFaultTreePipingOutput sectionSpecificFaultTreePipingOutput)
            {
                outputEntity.GeneralResultFaultTreeIllustrationPointEntity = profileSpecificFaultTreePipingOutput.GeneralResult?.CreateGeneralResultFaultTreeIllustrationPointEntity();
                outputEntity.GeneralResultFaultTreeIllustrationPointEntity1 = sectionSpecificFaultTreePipingOutput.GeneralResult?.CreateGeneralResultFaultTreeIllustrationPointEntity();
            }

            if (output.ProfileSpecificOutput is PartialProbabilisticSubMechanismPipingOutput profileSpecificSubMechanismPipingOutput
                && output.SectionSpecificOutput is PartialProbabilisticSubMechanismPipingOutput sectionSpecificSubMechanismPipingOutput)
            {
                outputEntity.GeneralResultSubMechanismIllustrationPointEntity = profileSpecificSubMechanismPipingOutput.GeneralResult?.CreateGeneralResultSubMechanismIllustrationPointEntity();
                outputEntity.GeneralResultSubMechanismIllustrationPointEntity1 = sectionSpecificSubMechanismPipingOutput.GeneralResult?.CreateGeneralResultSubMechanismIllustrationPointEntity();
            }

            return outputEntity;
        }
    }
}