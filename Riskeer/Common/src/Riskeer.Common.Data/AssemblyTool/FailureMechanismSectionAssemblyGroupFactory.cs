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
using System.ComponentModel;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// Factory for creating results of a failure mechanism section assembly.
    /// </summary>
    public static class FailureMechanismSectionAssemblyGroupFactory
    {
        /// <summary>
        /// Assembles the failure mechanism section based on the input arguments.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the section belongs to.</param>
        /// <param name="isRelevant">The indicator whether the section is relevant.</param>
        /// <param name="initialFailureMechanismResultType">The <see cref="AdoptableInitialFailureMechanismResultType"/> of the section.</param>
        /// <param name="initialSectionProbability">The initial probability for the section.</param>
        /// <param name="furtherAnalysisNeeded">The indicator whether the section needs further analysis.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="initialFailureMechanismResultType"/> is invalid.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            IAssessmentSection assessmentSection,
            bool isRelevant, AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
            double initialSectionProbability, bool furtherAnalysisNeeded, double refinedSectionProbability)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!Enum.IsDefined(typeof(AdoptableInitialFailureMechanismResultType), initialFailureMechanismResultType))
            {
                throw new InvalidEnumArgumentException(nameof(initialFailureMechanismResultType),
                                                       (int) initialFailureMechanismResultType,
                                                       typeof(AdoptableInitialFailureMechanismResultType));
            }

            FailureMechanismSectionAssemblyInput input = CreateInput(
                assessmentSection, isRelevant, initialFailureMechanismResultType,
                initialSectionProbability, initialSectionProbability, furtherAnalysisNeeded,
                refinedSectionProbability, refinedSectionProbability);

            return PerformAssembly(input);
        }

        /// <summary>
        /// Assembles the failure mechanism section based on the input arguments.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the section belongs to.</param>
        /// <param name="isRelevant">The indicator whether the section is relevant.</param>
        /// <param name="initialFailureMechanismResultType">The <see cref="AdoptableInitialFailureMechanismResultType"/> of the section.</param>
        /// <param name="initialProfileProbability">The initial probability for the profile.</param>
        /// <param name="initialSectionProbability">The initial probability for the section.</param>
        /// <param name="furtherAnalysisNeeded">The indicator whether the section needs further analysis.</param>
        /// <param name="refinedProfileProbability">The refined probability for the profile.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        /// <param name="probabilityRefinementType">The <see cref="ProbabilityRefinementType"/> of the section.</param>
        /// <param name="getNFunc">The <see cref="Func{TResult}"/> to get the N of the section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// or <paramref name="getNFunc"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="initialFailureMechanismResultType"/>
        /// or <paramref name="probabilityRefinementType"/> is invalid.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            IAssessmentSection assessmentSection,
            bool isRelevant, AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
            double initialProfileProbability, double initialSectionProbability,
            bool furtherAnalysisNeeded,
            double refinedProfileProbability, double refinedSectionProbability,
            ProbabilityRefinementType probabilityRefinementType, Func<double> getNFunc)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getNFunc == null)
            {
                throw new ArgumentNullException(nameof(getNFunc));
            }

            if (!Enum.IsDefined(typeof(AdoptableInitialFailureMechanismResultType), initialFailureMechanismResultType))
            {
                throw new InvalidEnumArgumentException(nameof(initialFailureMechanismResultType),
                                                       (int) initialFailureMechanismResultType,
                                                       typeof(AdoptableInitialFailureMechanismResultType));
            }

            if (!Enum.IsDefined(typeof(ProbabilityRefinementType), probabilityRefinementType))
            {
                throw new InvalidEnumArgumentException(nameof(probabilityRefinementType),
                                                       (int) probabilityRefinementType,
                                                       typeof(ProbabilityRefinementType));
            }

            FailureMechanismSectionAssemblyInput input = CreateInput(
                assessmentSection, isRelevant, initialFailureMechanismResultType,
                initialProfileProbability, initialSectionProbability, furtherAnalysisNeeded,
                refinedProfileProbability, refinedSectionProbability, probabilityRefinementType, getNFunc);

            return PerformAssembly(input);
        }

        private static FailureMechanismSectionAssemblyResult PerformAssembly(FailureMechanismSectionAssemblyInput input)
        {
            IFailureMechanismSectionAssemblyCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateFailureMechanismSectionAssemblyCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.AssembleFailureMechanismSection(input);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        private static FailureMechanismSectionAssemblyInput CreateInput(IAssessmentSection assessmentSection,
                                                                        bool isRelevant, AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
                                                                        double initialProfileProbability, double initialSectionProbability,
                                                                        bool furtherAnalysisNeeded,
                                                                        double refinedProfileProbability, double refinedSectionProbability,
                                                                        ProbabilityRefinementType probabilityRefinementType, Func<double> getNFunc)
        {
            double sectionN = getNFunc();

            if (probabilityRefinementType == ProbabilityRefinementType.Profile)
            {
                refinedSectionProbability = refinedProfileProbability * sectionN;
            }

            if (probabilityRefinementType == ProbabilityRefinementType.Section)
            {
                refinedProfileProbability = refinedSectionProbability / sectionN;
            }

            return CreateInput(assessmentSection, isRelevant, initialFailureMechanismResultType,
                               initialProfileProbability, initialSectionProbability, furtherAnalysisNeeded,
                               refinedProfileProbability, refinedSectionProbability);
        }

        private static FailureMechanismSectionAssemblyInput CreateInput(IAssessmentSection assessmentSection,
                                                                        bool isRelevant, AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
                                                                        double initialProfileProbability, double initialSectionProbability,
                                                                        bool furtherAnalysisNeeded,
                                                                        double refinedProfileProbability, double refinedSectionProbability)
        {
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            bool hasProbabilitySpecified = initialFailureMechanismResultType != AdoptableInitialFailureMechanismResultType.NoFailureProbability;

            return new FailureMechanismSectionAssemblyInput(
                failureMechanismContribution.LowerLimitNorm, failureMechanismContribution.SignalingNorm,
                isRelevant, hasProbabilitySpecified, initialProfileProbability, initialSectionProbability,
                furtherAnalysisNeeded, refinedProfileProbability, refinedSectionProbability);
        }
    }
}