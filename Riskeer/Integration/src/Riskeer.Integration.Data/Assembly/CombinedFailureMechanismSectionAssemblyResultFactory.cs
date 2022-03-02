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
                                     assembly.Section.FailureMechanismSectionAssemblyGroup,
                                     CreateFailureMechanismResults(assembly.FailureMechanismSectionAssemblyGroupResults,
                                                                   failureMechanisms, assessmentSection)))
                         .ToArray();
        }

        private static CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties CreateFailureMechanismResults(
            IEnumerable<FailureMechanismSectionAssemblyGroup> failureMechanismResults,
            IDictionary<IFailureMechanism, int> failureMechanisms,
            AssessmentSection assessmentSection)
        {
            var constructionProperties = new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties
            {
                Piping = GetAssemblyGroup(assessmentSection.Piping, failureMechanisms, failureMechanismResults),
                GrassCoverErosionInwards = GetAssemblyGroup(assessmentSection.GrassCoverErosionInwards, failureMechanisms, failureMechanismResults),
                MacroStabilityInwards = GetAssemblyGroup(assessmentSection.MacroStabilityInwards, failureMechanisms, failureMechanismResults),
                Microstability = GetAssemblyGroup(assessmentSection.Microstability, failureMechanisms, failureMechanismResults),
                StabilityStoneCover = GetAssemblyGroup(assessmentSection.StabilityStoneCover, failureMechanisms, failureMechanismResults),
                WaveImpactAsphaltCover = GetAssemblyGroup(assessmentSection.WaveImpactAsphaltCover, failureMechanisms, failureMechanismResults),
                WaterPressureAsphaltCover = GetAssemblyGroup(assessmentSection.WaterPressureAsphaltCover, failureMechanisms, failureMechanismResults),
                GrassCoverErosionOutwards = GetAssemblyGroup(assessmentSection.GrassCoverErosionOutwards, failureMechanisms, failureMechanismResults),
                GrassCoverSlipOffOutwards = GetAssemblyGroup(assessmentSection.GrassCoverSlipOffOutwards, failureMechanisms, failureMechanismResults),
                GrassCoverSlipOffInwards = GetAssemblyGroup(assessmentSection.GrassCoverSlipOffInwards, failureMechanisms, failureMechanismResults),
                HeightStructures = GetAssemblyGroup(assessmentSection.HeightStructures, failureMechanisms, failureMechanismResults),
                ClosingStructures = GetAssemblyGroup(assessmentSection.ClosingStructures, failureMechanisms, failureMechanismResults),
                PipingStructure = GetAssemblyGroup(assessmentSection.PipingStructure, failureMechanisms, failureMechanismResults),
                StabilityPointStructures = GetAssemblyGroup(assessmentSection.StabilityPointStructures, failureMechanisms, failureMechanismResults),
                DuneErosion = GetAssemblyGroup(assessmentSection.DuneErosion, failureMechanisms, failureMechanismResults)
            };

            return constructionProperties;
        }

        private static FailureMechanismSectionAssemblyGroup GetAssemblyGroup(IFailureMechanism failureMechanism,
                                                                             IDictionary<IFailureMechanism, int> failureMechanisms,
                                                                             IEnumerable<FailureMechanismSectionAssemblyGroup> failureMechanismResults)
        {
            return failureMechanisms.ContainsKey(failureMechanism)
                       ? failureMechanismResults.ElementAt(failureMechanisms[failureMechanism])
                       : FailureMechanismSectionAssemblyGroup.Gr;
        }
    }
}