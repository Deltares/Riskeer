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
using System.Collections.Generic;
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Integration.Data.Assembly
{
    /// <summary>
    /// Factory that creates <see cref="CombinedFailureMechanismSectionAssemblyResult"/> instances.
    /// </summary>
    internal static class CombinedFailureMechanismSectionAssemblyResultFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CombinedFailureMechanismSectionAssemblyResult"/> based
        /// on the given <paramref name="output"/> and <paramref name="failureMechanisms"/>.
        /// </summary>
        /// <param name="output">The output to create the results for.</param>
        /// <param name="failureMechanisms">The failure mechanisms to create the results for.</param>
        /// <param name="assessmentSection">The assessment section to use while creating the results.</param>
        /// <returns>A collection of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<CombinedFailureMechanismSectionAssemblyResult> Create(IEnumerable<CombinedFailureMechanismSectionAssembly> output,
                                                                                        IDictionary<IFailureMechanism, int> failureMechanisms,
                                                                                        AssessmentSection assessmentSection)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (failureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(failureMechanisms));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return output.Select((assembly, sectionNumber) => new CombinedFailureMechanismSectionAssemblyResult(
                                     sectionNumber + 1,
                                     assembly.Section.SectionStart,
                                     assembly.Section.SectionEnd,
                                     assembly.Section.CategoryGroup,
                                     CreateFailureMechanismResults(assembly.FailureMechanismResults,
                                                                   failureMechanisms, assessmentSection)))
                         .ToArray();
        }

        private static CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties CreateFailureMechanismResults(
            IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismResults,
            IDictionary<IFailureMechanism, int> failureMechanisms,
            AssessmentSection assessmentSection)
        {
            var constructionProperties = new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
            {
                Piping = GetCategoryGroup(assessmentSection.Piping, failureMechanisms, failureMechanismResults),
                GrassCoverErosionInwards = GetCategoryGroup(assessmentSection.GrassCoverErosionInwards, failureMechanisms, failureMechanismResults),
                MacroStabilityInwards = GetCategoryGroup(assessmentSection.MacroStabilityInwards, failureMechanisms, failureMechanismResults),
                MacroStabilityOutwards = GetCategoryGroup(assessmentSection.MacroStabilityOutwards, failureMechanisms, failureMechanismResults),
                Microstability = GetCategoryGroup(assessmentSection.Microstability, failureMechanisms, failureMechanismResults),
                StabilityStoneCover = GetCategoryGroup(assessmentSection.StabilityStoneCover, failureMechanisms, failureMechanismResults),
                WaveImpactAsphaltCover = GetCategoryGroup(assessmentSection.WaveImpactAsphaltCover, failureMechanisms, failureMechanismResults),
                WaterPressureAsphaltCover = GetCategoryGroup(assessmentSection.WaterPressureAsphaltCover, failureMechanisms, failureMechanismResults),
                GrassCoverErosionOutwards = GetCategoryGroup(assessmentSection.GrassCoverErosionOutwards, failureMechanisms, failureMechanismResults),
                GrassCoverSlipOffOutwards = GetCategoryGroup(assessmentSection.GrassCoverSlipOffOutwards, failureMechanisms, failureMechanismResults),
                GrassCoverSlipOffInwards = GetCategoryGroup(assessmentSection.GrassCoverSlipOffInwards, failureMechanisms, failureMechanismResults),
                HeightStructures = GetCategoryGroup(assessmentSection.HeightStructures, failureMechanisms, failureMechanismResults),
                ClosingStructures = GetCategoryGroup(assessmentSection.ClosingStructures, failureMechanisms, failureMechanismResults),
                PipingStructure = GetCategoryGroup(assessmentSection.PipingStructure, failureMechanisms, failureMechanismResults),
                StabilityPointStructures = GetCategoryGroup(assessmentSection.StabilityPointStructures, failureMechanisms, failureMechanismResults),
                StrengthStabilityLengthwiseConstruction = GetCategoryGroup(assessmentSection.StrengthStabilityLengthwiseConstruction, failureMechanisms, failureMechanismResults),
                DuneErosion = GetCategoryGroup(assessmentSection.DuneErosion, failureMechanisms, failureMechanismResults),
                TechnicalInnovation = GetCategoryGroup(assessmentSection.TechnicalInnovation, failureMechanisms, failureMechanismResults)
            };

            return constructionProperties;
        }

        private static FailureMechanismSectionAssemblyCategoryGroup GetCategoryGroup(IFailureMechanism failureMechanism,
                                                                                     IDictionary<IFailureMechanism, int> failureMechanisms,
                                                                                     IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismResults)
        {
            return failureMechanisms.ContainsKey(failureMechanism)
                       ? failureMechanismResults.ElementAt(failureMechanisms[failureMechanism])
                       : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
        }
    }
}