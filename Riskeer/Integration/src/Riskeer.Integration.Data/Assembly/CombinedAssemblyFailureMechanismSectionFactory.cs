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
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Data.Assembly
{
    /// <summary>
    /// Factory that creates <see cref="CombinedAssemblyFailureMechanismSection"/> instances.
    /// </summary>
    internal static class CombinedAssemblyFailureMechanismSectionFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CombinedAssemblyFailureMechanismSection"/> collections.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to use.</param>
        /// <param name="failureMechanisms">The failure mechanisms to build input for.</param>
        /// <returns>A collection of <see cref="CombinedAssemblyFailureMechanismSection"/> collections.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> CreateInput(
            AssessmentSection assessmentSection, IEnumerable<IFailureMechanism> failureMechanisms)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (failureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(failureMechanisms));
            }

            var inputs = new List<IEnumerable<CombinedAssemblyFailureMechanismSection>>();

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.Piping;
            if (failureMechanisms.Contains(pipingFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(pipingFailureMechanism.SectionResults, assessmentSection, PipingAssemblyFunc));
            }

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            if (failureMechanisms.Contains(grassCoverErosionInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassCoverErosionInwardsFailureMechanism.SectionResults, assessmentSection, GrassCoverErosionInwardsAssemblyFunc));
            }

            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            if (failureMechanisms.Contains(macroStabilityInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(macroStabilityInwardsFailureMechanism.SectionResults, assessmentSection, MacroStabilityInwardsAssemblyFunc));
            }

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            if (failureMechanisms.Contains(microstabilityFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(microstabilityFailureMechanism.SectionResults, assessmentSection, MicrostabilityAssemblyFunc));
            }

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            if (failureMechanisms.Contains(stabilityStoneCoverFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(stabilityStoneCoverFailureMechanism.SectionResults, assessmentSection, StabilityStoneCoverAssemblyFunc));
            }

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            if (failureMechanisms.Contains(waveImpactAsphaltCoverFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(waveImpactAsphaltCoverFailureMechanism.SectionResults, assessmentSection, WaveImpactAsphaltCoverAssemblyFunc));
            }

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            if (failureMechanisms.Contains(waterPressureAsphaltCoverFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(waterPressureAsphaltCoverFailureMechanism.SectionResults, assessmentSection, WaterPressureAsphaltCoverAssemblyFunc));
            }

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            if (failureMechanisms.Contains(grassCoverErosionOutwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassCoverErosionOutwardsFailureMechanism.SectionResults, assessmentSection, GrassCoverErosionOutwardsAssemblyFunc));
            }

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            if (failureMechanisms.Contains(grassCoverSlipOffOutwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassCoverSlipOffOutwardsFailureMechanism.SectionResults, assessmentSection, GrassCoverSlipOffOutwardsAssemblyFunc));
            }

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            if (failureMechanisms.Contains(grassCoverSlipOffInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassCoverSlipOffInwardsFailureMechanism.SectionResults, assessmentSection, GrassCoverSlipOffInwardsAssemblyFunc));
            }

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            if (failureMechanisms.Contains(heightStructuresFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(heightStructuresFailureMechanism.SectionResults, assessmentSection, HeightStructuresAssemblyFunc));
            }

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            if (failureMechanisms.Contains(closingStructuresFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(closingStructuresFailureMechanism.SectionResults, assessmentSection, ClosingStructuresAssemblyFunc));
            }

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            if (failureMechanisms.Contains(pipingStructureFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(pipingStructureFailureMechanism.SectionResults, assessmentSection, PipingStructureAssemblyFunc));
            }

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            if (failureMechanisms.Contains(stabilityPointStructuresFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(stabilityPointStructuresFailureMechanism.SectionResults, assessmentSection, StabilityPointStructuresAssemblyFunc));
            }

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            if (failureMechanisms.Contains(duneErosionFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(duneErosionFailureMechanism.SectionResults, assessmentSection, DuneErosionAssemblyFunc));
            }

            return inputs;
        }

        private static IEnumerable<CombinedAssemblyFailureMechanismSection> CreateCombinedSections<TFailureMechanismSectionResult>(
            IEnumerable<TFailureMechanismSectionResult> sectionResults, AssessmentSection assessmentSection,
            Func<TFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> getAssemblyFunc)
            where TFailureMechanismSectionResult : FailureMechanismSectionResult
        {
            double totalSectionsLength = 0;

            return sectionResults.Select(sectionResult =>
                                 {
                                     CombinedAssemblyFailureMechanismSection section = CreateSection(
                                         sectionResult, getAssemblyFunc(sectionResult, assessmentSection).AssemblyGroup, totalSectionsLength);
                                     totalSectionsLength = section.SectionEnd;
                                     return section;
                                 })
                                 .ToArray();
        }

        private static CombinedAssemblyFailureMechanismSection CreateSection<TFailureMechanismSectionResult>(
            TFailureMechanismSectionResult sectionResult, FailureMechanismSectionAssemblyGroup assemblyGroup, double sectionStart)
            where TFailureMechanismSectionResult : FailureMechanismSectionResult
        {
            double sectionEnd = sectionResult.Section.Length + sectionStart;
            return new CombinedAssemblyFailureMechanismSection(sectionStart, sectionEnd, assemblyGroup);
        }

        #region Assembly Funcs

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> PipingAssemblyFunc =>
            (sectionResult, assessmentSection) => PipingFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.Piping, assessmentSection);

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> GrassCoverErosionInwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverErosionInwards, assessmentSection);

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> MacroStabilityInwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.MacroStabilityInwards, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> MicrostabilityAssemblyFunc =>
            (sectionResult, assessmentSection) => StandAloneFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.Microstability, assessmentSection);

        private static Func<AdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> HeightStructuresAssemblyFunc =>
            (sectionResult, assessmentSection) => StructuresFailureMechanismAssemblyFactory.AssembleSection<HeightStructuresInput>(
                sectionResult, assessmentSection.HeightStructures, assessmentSection);

        private static Func<AdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> ClosingStructuresAssemblyFunc =>
            (sectionResult, assessmentSection) => StructuresFailureMechanismAssemblyFactory.AssembleSection<ClosingStructuresInput>(
                sectionResult, assessmentSection.HeightStructures, assessmentSection);

        private static Func<AdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> StabilityPointStructuresAssemblyFunc =>
            (sectionResult, assessmentSection) => StructuresFailureMechanismAssemblyFactory.AssembleSection<StabilityPointStructuresInput>(
                sectionResult, assessmentSection.HeightStructures, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> GrassCoverErosionOutwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverErosionOutwards, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> StabilityStoneCoverAssemblyFunc =>
            (sectionResult, assessmentSection) => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.StabilityStoneCover, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> WaveImpactAsphaltCoverAssemblyFunc =>
            (sectionResult, assessmentSection) => WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.WaveImpactAsphaltCover, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> WaterPressureAsphaltCoverAssemblyFunc =>
            (sectionResult, assessmentSection) => StandAloneFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.WaterPressureAsphaltCover, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> GrassCoverSlipOffOutwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => StandAloneFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverSlipOffOutwards, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> GrassCoverSlipOffInwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => StandAloneFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverSlipOffInwards, assessmentSection);

        private static Func<NonAdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> PipingStructureAssemblyFunc =>
            FailureMechanismSectionResultAssemblyFactory.AssembleSection;

        private static Func<NonAdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> DuneErosionAssemblyFunc =>
            FailureMechanismSectionResultAssemblyFactory.AssembleSection;

        #endregion
    }
}