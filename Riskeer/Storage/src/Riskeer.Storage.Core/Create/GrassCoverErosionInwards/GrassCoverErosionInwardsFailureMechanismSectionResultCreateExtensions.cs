// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionInwardsFailureMechanismSectionResultOld"/> related to creating a 
    /// <see cref="GrassCoverErosionInwardsSectionResultEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsFailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverErosionInwardsSectionResultEntity"/>
        /// based on the information of the <see cref="GrassCoverErosionInwardsFailureMechanismSectionResultOld"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsSectionResultEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        internal static GrassCoverErosionInwardsSectionResultEntity Create(this AdoptableWithProfileProbabilityFailureMechanismSectionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var sectionResultEntity = new GrassCoverErosionInwardsSectionResultEntity
            {
                IsRelevant = Convert.ToByte(result.IsRelevant),
                AdoptableInitialFailureMechanismResultType = Convert.ToByte(result.InitialFailureMechanismResult),
                ManualInitialFailureMechanismResultProfileProbability = result.ManualInitialFailureMechanismResultProfileProbability.ToNaNAsNull(),
                ManualInitialFailureMechanismResultSectionProbability = result.ManualInitialFailureMechanismResultSectionProbability.ToNaNAsNull(),
                FurtherAnalysisNeeded = Convert.ToByte(result.FurtherAnalysisNeeded),
                ProbabilityRefinementType = Convert.ToByte(result.ProbabilityRefinementType),
                RefinedProfileProbability = result.RefinedProfileProbability.ToNaNAsNull(),
                RefinedSectionProbability = result.RefinedSectionProbability.ToNaNAsNull()
            };

            return sectionResultEntity;
        }
    }
}