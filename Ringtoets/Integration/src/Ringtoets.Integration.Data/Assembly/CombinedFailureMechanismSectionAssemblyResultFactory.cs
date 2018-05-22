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
            var constructionProperties = new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties();

            if (failureMechanisms.ContainsKey(assessmentSection.Piping))
            {
                constructionProperties.Piping = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.Piping]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.GrassCoverErosionInwards))
            {
                constructionProperties.GrassCoverErosionInwards = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionInwards]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.MacroStabilityInwards))
            {
                constructionProperties.MacroStabilityInwards = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.MacroStabilityInwards]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.MacroStabilityOutwards))
            {
                constructionProperties.MacroStabilityOutwards = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.MacroStabilityOutwards]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.Microstability))
            {
                constructionProperties.Microstability = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.Microstability]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.StabilityStoneCover))
            {
                constructionProperties.StabilityStoneCover = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.StabilityStoneCover]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.WaveImpactAsphaltCover))
            {
                constructionProperties.WaveImpactAsphaltCover = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.WaveImpactAsphaltCover]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.WaterPressureAsphaltCover))
            {
                constructionProperties.WaterPressureAsphaltCover = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.WaterPressureAsphaltCover]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.GrassCoverErosionOutwards))
            {
                constructionProperties.GrassCoverErosionOutwards = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverErosionOutwards]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.GrassCoverSlipOffOutwards))
            {
                constructionProperties.GrassCoverSlipOffOutwards = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffOutwards]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.GrassCoverSlipOffInwards))
            {
                constructionProperties.GrassCoverSlipOffInwards = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.GrassCoverSlipOffInwards]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.HeightStructures))
            {
                constructionProperties.HeightStructures = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.HeightStructures]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.ClosingStructures))
            {
                constructionProperties.ClosingStructures = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.ClosingStructures]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.PipingStructure))
            {
                constructionProperties.PipingStructure = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.PipingStructure]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.StabilityPointStructures))
            {
                constructionProperties.StabilityPointStructures = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.StabilityPointStructures]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.StrengthStabilityLengthwiseConstruction))
            {
                constructionProperties.StrengthStabilityLengthwise = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.StrengthStabilityLengthwiseConstruction]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.DuneErosion))
            {
                constructionProperties.DuneErosion = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.DuneErosion]);
            }

            if (failureMechanisms.ContainsKey(assessmentSection.TechnicalInnovation))
            {
                constructionProperties.TechnicalInnovation = failureMechanismResults.ElementAt(failureMechanisms[assessmentSection.TechnicalInnovation]);
            }

            return constructionProperties;
        }
    }
}