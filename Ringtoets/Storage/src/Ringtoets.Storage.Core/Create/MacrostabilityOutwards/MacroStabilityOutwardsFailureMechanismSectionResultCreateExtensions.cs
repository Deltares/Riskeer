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
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.MacroStabilityOutwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/> related to creating a 
    /// <see cref="MacroStabilityOutwardsSectionResultEntity"/>.
    /// </summary>
    internal static class MacroStabilityOutwardsFailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityOutwardsSectionResultEntity"/> based on the information of the <see cref="MacroStabilityOutwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>A new <see cref="MacroStabilityOutwardsSectionResultEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        internal static MacroStabilityOutwardsSectionResultEntity Create(this MacroStabilityOutwardsFailureMechanismSectionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var sectionResultEntity = new MacroStabilityOutwardsSectionResultEntity
            {
                SimpleAssessmentResult = Convert.ToByte(result.SimpleAssessmentResult),
                DetailedAssessmentResult = Convert.ToByte(result.DetailedAssessmentResult),
                DetailedAssessmentProbability = result.DetailedAssessmentProbability.ToNaNAsNull(),
                TailorMadeAssessmentResult = Convert.ToByte(result.TailorMadeAssessmentResult),
                TailorMadeAssessmentProbability = result.TailorMadeAssessmentProbability.ToNaNAsNull(),
                UseManualAssembly = Convert.ToByte(result.UseManualAssembly),
                ManualAssemblyCategoryGroup = Convert.ToByte(result.ManualAssemblyCategoryGroup)
            };

            return sectionResultEntity;
        }
    }
}