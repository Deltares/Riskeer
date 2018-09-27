// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
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

            MacroStabilityOutwardsFailureMechanism macroStabilityOutwardsFailureMechanism = assessmentSection.MacroStabilityOutwards;
            if (failureMechanisms.Contains(macroStabilityOutwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(macroStabilityOutwardsFailureMechanism.SectionResults,
                                                  assessmentSection, MacroStabilityOutwardsAssemblyFunc, useManual));
            }

            MicrostabilityFailureMechanism microstabilityFailureMechanism = assessmentSection.Microstability;
            if (failureMechanisms.Contains(microstabilityFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(microstabilityFailureMechanism.SectionResults,
                                                  MicrostabilityAssemblyFunc, useManual));
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

            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCoverFailureMechanism = assessmentSection.WaterPressureAsphaltCover;
            if (failureMechanisms.Contains(waterPressureAsphaltCoverFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(waterPressureAsphaltCoverFailureMechanism.SectionResults,
                                                  WaterPressureAsphaltCoverAssemblyFunc, useManual));
            }

            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;
            if (failureMechanisms.Contains(grassCoverErosionOutwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassCoverErosionOutwardsFailureMechanism.SectionResults,
                                                  GrassCoverErosionOutwardsAssemblyFunc, useManual));
            }

            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwardsFailureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            if (failureMechanisms.Contains(grassCoverSlipOffOutwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassCoverSlipOffOutwardsFailureMechanism.SectionResults,
                                                  GrassCoverSlipOffOutwardsAssemblyFunc, useManual));
            }

            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwardsFailureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            if (failureMechanisms.Contains(grassCoverSlipOffInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(grassCoverSlipOffInwardsFailureMechanism.SectionResults,
                                                  GrassCoverSlipOffInwardsAssemblyFunc, useManual));
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

            PipingStructureFailureMechanism pipingStructureFailureMechanism = assessmentSection.PipingStructure;
            if (failureMechanisms.Contains(pipingStructureFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(pipingStructureFailureMechanism.SectionResults,
                                                  PipingStructureAssemblyFunc, useManual));
            }

            StabilityPointStructuresFailureMechanism stabilityPointStructuresFailureMechanism = assessmentSection.StabilityPointStructures;
            if (failureMechanisms.Contains(stabilityPointStructuresFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(stabilityPointStructuresFailureMechanism.SectionResults,
                                                  assessmentSection, StabilityPointStructuresAssemblyFunc, useManual));
            }

            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstructionFailureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            if (failureMechanisms.Contains(strengthStabilityLengthwiseConstructionFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(strengthStabilityLengthwiseConstructionFailureMechanism.SectionResults,
                                                  StrengthStabilityLengthwiseConstructionAssemblyFunc, useManual));
            }

            DuneErosionFailureMechanism duneErosionFailureMechanism = assessmentSection.DuneErosion;
            if (failureMechanisms.Contains(duneErosionFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(duneErosionFailureMechanism.SectionResults,
                                                  DuneErosionAssemblyFunc, useManual));
            }

            TechnicalInnovationFailureMechanism technicalInnovationFailureMechanism = assessmentSection.TechnicalInnovation;
            if (failureMechanisms.Contains(technicalInnovationFailureMechanism))
            {
                inputs.Add(CreateCombinedSections(technicalInnovationFailureMechanism.SectionResults,
                                                  TechnicalInnovationAssemblyFunc, useManual));
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

        private static Func<MacroStabilityOutwardsFailureMechanismSectionResult, AssessmentSection, bool, FailureMechanismSectionAssemblyCategoryGroup> MacroStabilityOutwardsAssemblyFunc
        {
            get
            {
                return (sectionResult, assessmentSection, useManual) => MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult, assessmentSection.MacroStabilityOutwards, assessmentSection, useManual);
            }
        }

        private static Func<MicrostabilityFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> MicrostabilityAssemblyFunc
        {
            get
            {
                return MicrostabilityFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
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

        private static Func<WaterPressureAsphaltCoverFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> WaterPressureAsphaltCoverAssemblyFunc
        {
            get
            {
                return WaterPressureAsphaltCoverFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        private static Func<GrassCoverErosionOutwardsFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> GrassCoverErosionOutwardsAssemblyFunc
        {
            get
            {
                return GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        private static Func<GrassCoverSlipOffOutwardsFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> GrassCoverSlipOffOutwardsAssemblyFunc
        {
            get
            {
                return GrassCoverSlipOffOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        private static Func<GrassCoverSlipOffInwardsFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> GrassCoverSlipOffInwardsAssemblyFunc
        {
            get
            {
                return GrassCoverSlipOffInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
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

        private static Func<PipingStructureFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> PipingStructureAssemblyFunc
        {
            get
            {
                return PipingStructureFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
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

        private static Func<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> StrengthStabilityLengthwiseConstructionAssemblyFunc
        {
            get
            {
                return StrengthStabilityLengthwiseConstructionFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        private static Func<DuneErosionFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> DuneErosionAssemblyFunc
        {
            get
            {
                return DuneErosionFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        private static Func<TechnicalInnovationFailureMechanismSectionResult, bool, FailureMechanismSectionAssemblyCategoryGroup> TechnicalInnovationAssemblyFunc
        {
            get
            {
                return TechnicalInnovationFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup;
            }
        }

        #endregion
    }
}