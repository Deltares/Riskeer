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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data.Assembly
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
        public static IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> CreateInput(AssessmentSection assessmentSection,
                                                                                                    IEnumerable<IFailureMechanism> failureMechanisms)
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
                                                  assessmentSection, PipingAssemblyFunc)
                               .ToArray());
            }

            GrassCoverErosionInwardsFailureMechanism grassInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            if (failureMechanisms.Contains(grassInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassInwardsFailureMechanism.SectionResults,
                                                  assessmentSection, GrassCoverErosionInwardsAssemblyFunc)
                               .ToArray());
            }

            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            if (failureMechanisms.Contains(macroStabilityInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(macroStabilityInwardsFailureMechanism.SectionResults,
                                                  assessmentSection, MacroStabilityInwardsAssemblyFunc)
                               .ToArray());
            }

            MacroStabilityOutwardsFailureMechanism macroStabilityOutwardsFailureMechanism = assessmentSection.MacroStabilityOutwards;
            if (failureMechanisms.Contains(macroStabilityOutwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(macroStabilityOutwardsFailureMechanism.SectionResults,
                                                  assessmentSection, MacroStabilityOutwardsAssemblyFunc)
                               .ToArray());
            }

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            if (failureMechanisms.Contains(microstabilityFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(microstabilityFailureMechanism.SectionResults,
                                                  assessmentSection, MicrostabilityAssemblyFunc)
                               .ToArray());
            }

            StabilityStoneCoverFailureMechanism stabilityStoneCoverFailureMechanism = assessmentSection.StabilityStoneCover;
            if (failureMechanisms.Contains(stabilityStoneCoverFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(stabilityStoneCoverFailureMechanism.SectionResults,
                                                  assessmentSection, StabilityStoneCoverAssemblyFunc)
                               .ToArray());
            }

            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCoverFailureMechanism = assessmentSection.WaveImpactAsphaltCover;
            if (failureMechanisms.Contains(waveImpactAsphaltCoverFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(waveImpactAsphaltCoverFailureMechanism.SectionResults,
                                                  assessmentSection, WaveImpactAsphaltCoverAssemblyFunc)
                               .ToArray());
            }

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            if (failureMechanisms.Contains(waterPressureAsphaltCoverFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(waterPressureAsphaltCoverFailureMechanism.SectionResults,
                                                  assessmentSection, WaterPressureAsphaltCoverAssemblyFunc)
                               .ToArray());
            }

            return inputs;
        }

        private static IEnumerable<CombinedAssemblyFailureMechanismSection> CreateCombinedSections<TFailureMechanismSectionResult>(
            IEnumerable<TFailureMechanismSectionResult> sectionResults,
            AssessmentSection assessmentSection,
            Func<TFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyCategoryGroup> getAssemblyFunc)
            where TFailureMechanismSectionResult : FailureMechanismSectionResult
        {
            double totalSectionsLength = 0;

            var combinedSections = new List<CombinedAssemblyFailureMechanismSection>();
            foreach (TFailureMechanismSectionResult sectionResult in sectionResults)
            {
                double endPoint = sectionResult.Section.Length + totalSectionsLength;
                combinedSections.Add(new CombinedAssemblyFailureMechanismSection(totalSectionsLength,
                                                                                 endPoint,
                                                                                 getAssemblyFunc(sectionResult, assessmentSection)));

                totalSectionsLength = endPoint;
            }

            return combinedSections.ToArray();
        }

        #region Assembly Funcs

        private static Func<PipingFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyCategoryGroup> PipingAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection) => PipingFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.Piping, assessmentSection);
            }
        }

        private static Func<GrassCoverErosionInwardsFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyCategoryGroup> GrassCoverErosionInwardsAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection) => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.GrassCoverErosionInwards, assessmentSection);
            }
        }

        private static Func<MacroStabilityInwardsFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyCategoryGroup> MacroStabilityInwardsAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection) => MacroStabilityInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.MacroStabilityInwards, assessmentSection);
            }
        }

        private static Func<MacroStabilityOutwardsFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyCategoryGroup> MacroStabilityOutwardsAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection) => MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.MacroStabilityOutwards, assessmentSection);
            }
        }

        private static Func<MicrostabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyCategoryGroup> MicrostabilityAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection) => MicrostabilityFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(sectionResult);
            }
        }

        private static Func<StabilityStoneCoverFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyCategoryGroup> StabilityStoneCoverAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection) => StabilityStoneCoverFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(sectionResult);
            }
        }

        private static Func<WaveImpactAsphaltCoverFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyCategoryGroup> WaveImpactAsphaltCoverAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection) => WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(sectionResult);
            }
        }

        private static Func<WaterPressureAsphaltCoverFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyCategoryGroup> WaterPressureAsphaltCoverAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection) => WaterPressureAsphaltCoverFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(sectionResult);
            }
        }

        #endregion
    }
}