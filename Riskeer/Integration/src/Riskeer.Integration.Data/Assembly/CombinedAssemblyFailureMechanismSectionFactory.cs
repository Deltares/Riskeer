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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
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
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A collection of <see cref="CombinedAssemblyFailureMechanismSection"/> collections.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> CreateInput(AssessmentSection assessmentSection,
                                                                                                    IEnumerable<IFailureMechanism> failureMechanisms,
                                                                                                    bool useManual)
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
                inputs.Add(CreateCombinedSections(pipingFailureMechanism.SectionResults,
                                                  assessmentSection, PipingAssemblyFunc, useManual));
            }

            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            if (failureMechanisms.Contains(grassCoverErosionInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassCoverErosionInwardsFailureMechanism.SectionResults,
                                                  assessmentSection, GrassCoverErosionInwardsAssemblyFunc, useManual));
            }

            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            if (failureMechanisms.Contains(macroStabilityInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(macroStabilityInwardsFailureMechanism.SectionResults,
                                                  assessmentSection, MacroStabilityInwardsAssemblyFunc, useManual));
            }

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            if (failureMechanisms.Contains(stabilityStoneCoverFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(stabilityStoneCoverFailureMechanism.SectionResults,
                                                  StabilityStoneCoverAssemblyFunc, useManual));
            }

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            if (failureMechanisms.Contains(waveImpactAsphaltCoverFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(waveImpactAsphaltCoverFailureMechanism.SectionResults,
                                                  WaveImpactAsphaltCoverAssemblyFunc, useManual));
            }

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            if (failureMechanisms.Contains(grassCoverErosionOutwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassCoverErosionOutwardsFailureMechanism.SectionResults,
                                                  GrassCoverErosionOutwardsAssemblyFunc, useManual));
            }

            HeightStructuresFailureMechanism heightStructuresFailureMechanism = assessmentSection.HeightStructures;
            if (failureMechanisms.Contains(heightStructuresFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(heightStructuresFailureMechanism.SectionResults,
                                                  assessmentSection, HeightStructuresAssemblyFunc, useManual));
            }

            ClosingStructuresFailureMechanism closingStructuresFailureMechanism = assessmentSection.ClosingStructures;
            if (failureMechanisms.Contains(closingStructuresFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(closingStructuresFailureMechanism.SectionResults,
                                                  assessmentSection, ClosingStructuresAssemblyFunc, useManual));
            }

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            if (failureMechanisms.Contains(stabilityPointStructuresFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(stabilityPointStructuresFailureMechanism.SectionResults,
                                                  assessmentSection, StabilityPointStructuresAssemblyFunc, useManual));
            }

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            if (failureMechanisms.Contains(duneErosionFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(duneErosionFailureMechanism.SectionResults,
                                                  DuneErosionAssemblyFunc, useManual));
            }

            return inputs;
        }

        private static IEnumerable<CombinedAssemblyFailureMechanismSection> CreateCombinedSections<TFailureMechanismSectionResult>(
            IEnumerable<TFailureMechanismSectionResult> sectionResults,
            AssessmentSection assessmentSection,
            Func<TFailureMechanismSectionResult, AssessmentSection, bool, FailureMechanismSectionAssemblyCategoryGroup> getAssemblyFunc,
            bool useManual)
            where TFailureMechanismSectionResult : FailureMechanismSectionResult
        {
            double totalSectionsLength = 0;

            return sectionResults.Select(sectionResult =>
                                 {
                                     CombinedAssemblyFailureMechanismSection section = CreateSection(sectionResult,
                                                                                                     getAssemblyFunc(sectionResult,
                                                                                                                     assessmentSection,
                                                                                                                     useManual),
                                                                                                     totalSectionsLength);
                                     totalSectionsLength = section.SectionEnd;
                                     return section;
                                 })
                                 .ToArray();
        }

        private static IEnumerable<CombinedAssemblyFailureMechanismSection> CreateCombinedSections<TFailureMechanismSectionResult>(
            IEnumerable<TFailureMechanismSectionResult> sectionResults,
            Func<TFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> getAssemblyFunc,
            bool useManual)
            where TFailureMechanismSectionResult : FailureMechanismSectionResult
        {
            double totalSectionsLength = 0;

            return sectionResults.Select(sectionResult =>
                                 {
                                     CombinedAssemblyFailureMechanismSection section = CreateSection(sectionResult, getAssemblyFunc(sectionResult, useManual), totalSectionsLength);
                                     totalSectionsLength = section.SectionEnd;
                                     return section;
                                 })
                                 .ToArray();
        }

        private static CombinedAssemblyFailureMechanismSection CreateSection<TFailureMechanismSectionResult>(
            TFailureMechanismSectionResult sectionResult, FailureMechanismSectionAssemblyCategoryGroup assemblyCategoryGroup,
            double sectionStart)
            where TFailureMechanismSectionResult : FailureMechanismSectionResult
        {
            double sectionEnd = sectionResult.Section.Length + sectionStart;
            return new CombinedAssemblyFailureMechanismSection(sectionStart, sectionEnd, assemblyCategoryGroup);
        }

        #region Assembly Funcs

        private static Func<PipingFailureMechanismSectionResult, AssessmentSection, bool, FailureMechanismSectionAssemblyCategoryGroup> PipingAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection, useManual) => PipingFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.Piping, assessmentSection, useManual);
            }
        }

        private static Func<GrassCoverErosionInwardsFailureMechanismSectionResult, AssessmentSection, bool, FailureMechanismSectionAssemblyCategoryGroup> GrassCoverErosionInwardsAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection, useManual) => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.GrassCoverErosionInwards, assessmentSection, useManual);
            }
        }

        private static Func<MacroStabilityInwardsFailureMechanismSectionResult, AssessmentSection, bool, FailureMechanismSectionAssemblyCategoryGroup> MacroStabilityInwardsAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection, useManual) => MacroStabilityInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.MacroStabilityInwards, assessmentSection, useManual);
            }
        }

        private static Func<StabilityStoneCoverFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> StabilityStoneCoverAssemblyFunc
        {
            get
            {
                return StabilityStoneCoverFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        private static Func<WaveImpactAsphaltCoverFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> WaveImpactAsphaltCoverAssemblyFunc
        {
            get
            {
                return WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        private static Func<GrassCoverErosionOutwardsFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> GrassCoverErosionOutwardsAssemblyFunc
        {
            get
            {
                return GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        private static Func<HeightStructuresFailureMechanismSectionResult, AssessmentSection, bool, FailureMechanismSectionAssemblyCategoryGroup> HeightStructuresAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection, useManual) => HeightStructuresFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.HeightStructures, assessmentSection, useManual);
            }
        }

        private static Func<ClosingStructuresFailureMechanismSectionResult, AssessmentSection, bool, FailureMechanismSectionAssemblyCategoryGroup> ClosingStructuresAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection, useManual) => ClosingStructuresFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.ClosingStructures, assessmentSection, useManual);
            }
        }
        
        private static Func<StabilityPointStructuresFailureMechanismSectionResult, AssessmentSection, bool, FailureMechanismSectionAssemblyCategoryGroup> StabilityPointStructuresAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection, useManual) => StabilityPointStructuresFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.StabilityPointStructures, assessmentSection, useManual);
            }
        }

        private static Func<DuneErosionFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> DuneErosionAssemblyFunc
        {
            get
            {
                return DuneErosionFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        #endregion
    }
}