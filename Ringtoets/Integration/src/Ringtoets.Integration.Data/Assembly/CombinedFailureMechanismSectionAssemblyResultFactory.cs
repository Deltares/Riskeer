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

using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.Data.Assembly
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

            return output.Select(assembly => new CombinedFailureMechanismSectionAssemblyResult(
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
                Piping = failureMechanisms.ContainsKey(assessmentSection.Piping)
                             ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.Piping])
                             : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                GrassCoverErosionInwards = failureMechanisms.ContainsKey(assessmentSection.GrassCoverErosionInwards)
                                               ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionInwards])
                                               : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                MacroStabilityInwards = failureMechanisms.ContainsKey(assessmentSection.MacroStabilityInwards)
                                            ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.MacroStabilityInwards])
                                            : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                MacroStabilityOutwards = failureMechanisms.ContainsKey(assessmentSection.MacroStabilityOutwards)
                                             ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.MacroStabilityOutwards])
                                             : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                Microstability = failureMechanisms.ContainsKey(assessmentSection.Microstability)
                                     ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.Microstability])
                                     : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                StabilityStoneCover = failureMechanisms.ContainsKey(assessmentSection.StabilityStoneCover)
                                          ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.StabilityStoneCover])
                                          : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                WaveImpactAsphaltCover = failureMechanisms.ContainsKey(assessmentSection.WaveImpactAsphaltCover)
                                             ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.WaveImpactAsphaltCover])
                                             : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                WaterPressureAsphaltCover = failureMechanisms.ContainsKey(assessmentSection.WaterPressureAsphaltCover)
                                                ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.WaterPressureAsphaltCover])
                                                : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                GrassCoverErosionOutwards = failureMechanisms.ContainsKey(assessmentSection.GrassCoverErosionOutwards)
                                                ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionOutwards])
                                                : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                GrassCoverSlipOffOutwards = failureMechanisms.ContainsKey(assessmentSection.GrassCoverSlipOffOutwards)
                                                ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffOutwards])
                                                : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                GrassCoverSlipOffInwards = failureMechanisms.ContainsKey(assessmentSection.GrassCoverSlipOffInwards)
                                               ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffInwards])
                                               : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                HeightStructures = failureMechanisms.ContainsKey(assessmentSection.HeightStructures)
                                       ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.HeightStructures])
                                       : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                ClosingStructures = failureMechanisms.ContainsKey(assessmentSection.ClosingStructures)
                                        ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.ClosingStructures])
                                        : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                PipingStructure = failureMechanisms.ContainsKey(assessmentSection.PipingStructure)
                                      ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.PipingStructure])
                                      : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                StabilityPointStructures = failureMechanisms.ContainsKey(assessmentSection.StabilityPointStructures)
                                               ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.StabilityPointStructures])
                                               : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                StrengthStabilityLengthwiseConstruction = failureMechanisms.ContainsKey(assessmentSection.StrengthStabilityLengthwiseConstruction)
                                                  ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.StrengthStabilityLengthwiseConstruction])
                                                  : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                DuneErosion = failureMechanisms.ContainsKey(assessmentSection.DuneErosion)
                                  ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.DuneErosion])
                                  : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable,
                TechnicalInnovation = failureMechanisms.ContainsKey(assessmentSection.TechnicalInnovation)
                                          ? failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.TechnicalInnovation])
                                          : FailureMechanismSectionAssemblyCategoryGroup.NotApplicable
            };

            return constructionProperties;
        }
    }
}