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
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.Data.Assembly
{
    /// <summary>
    /// Factory that creates <see cref="CombinedAssemblyFailureMechanismInput"/> instances.
    /// </summary>
    internal static class CombinedAssemblyFailureMechanismInputFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="CombinedAssemblyFailureMechanismInput"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to use.</param>
        /// <param name="failureMechanisms">The failure mechanisms to build input for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="CombinedAssemblyFailureMechanismInput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<CombinedAssemblyFailureMechanismInput> CreateInput(AssessmentSection assessmentSection,
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

            var inputs = new List<CombinedAssemblyFailureMechanismInput>();

            PipingFailureMechanism pipingFailureMechanism = assessmentSection.Piping;
            if (failureMechanisms.Contains(pipingFailureMechanism))
            {
                inputs.Add(CreateCombinedAssemblyFailureMechanismInputItem(fm => fm.PipingProbabilityAssessmentInput.GetN(
                                                                               fm.PipingProbabilityAssessmentInput.SectionLength),
                                                                           pipingFailureMechanism,
                                                                           CreateCombinedSections(pipingFailureMechanism.SectionResults,
                                                                                                  assessmentSection, PipingAssemblyFunc)
                                                                               .ToArray()));
            }

            GrassCoverErosionInwardsFailureMechanism grassInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            if (failureMechanisms.Contains(grassInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedAssemblyFailureMechanismInputItem(fm => fm.GeneralInput.N,
                                                                           grassInwardsFailureMechanism,
                                                                           CreateCombinedSections(grassInwardsFailureMechanism.SectionResults,
                                                                                                  assessmentSection, GrassCoverErosionInwardsAssemblyFunc)
                                                                               .ToArray()));
            }

            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            if (failureMechanisms.Contains(macroStabilityInwardsFailureMechanism))
            {
                inputs.Add(CreateCombinedAssemblyFailureMechanismInputItem(fm => fm.MacroStabilityInwardsProbabilityAssessmentInput.GetN(
                                                                               fm.MacroStabilityInwardsProbabilityAssessmentInput.SectionLength),
                                                                           macroStabilityInwardsFailureMechanism,
                                                                           CreateCombinedSections(macroStabilityInwardsFailureMechanism.SectionResults,
                                                                                                  assessmentSection, MacroStabilityInwardsAssemblyFunc)
                                                                               .ToArray()));
            }

            return inputs;
        }

        private static CombinedAssemblyFailureMechanismInput CreateCombinedAssemblyFailureMechanismInputItem<TFailureMechanism>(
            Func<TFailureMechanism, double> getLengthEffectFunc, TFailureMechanism failureMechanism,
            IEnumerable<CombinedAssemblyFailureMechanismSection> combinedAssemblyFailureMechanismSections)
            where TFailureMechanism : IFailureMechanism
        {
            return new CombinedAssemblyFailureMechanismInput(getLengthEffectFunc(failureMechanism),
                                                             failureMechanism.Contribution,
                                                             combinedAssemblyFailureMechanismSections);
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

        #endregion
    }
}