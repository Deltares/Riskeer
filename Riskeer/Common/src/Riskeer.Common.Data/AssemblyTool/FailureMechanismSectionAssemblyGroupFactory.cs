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
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// Factory for creating assembly results for a failure mechanism section.
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
        /// <param name="furtherAnalysisType">The further analysis type.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="initialFailureMechanismResultType"/> is invalid.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            IAssessmentSection assessmentSection, bool isRelevant, AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
            double initialSectionProbability, FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType, double refinedSectionProbability)
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

            bool hasProbabilitySpecified = initialFailureMechanismResultType != AdoptableInitialFailureMechanismResultType.NoFailureProbability;

            FailureMechanismSectionAssemblyInput input = CreateInput(
                assessmentSection, isRelevant, initialSectionProbability,
                furtherAnalysisType, refinedSectionProbability, hasProbabilitySpecified);

            return PerformAssembly(input);
        }

        /// <summary>
        /// Assembles the failure mechanism section based on the input arguments.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the section belongs to.</param>
        /// <param name="isRelevant">The indicator whether the section is relevant.</param>
        /// <param name="initialFailureMechanismResultType">The <see cref="NonAdoptableInitialFailureMechanismResultType"/> of the section.</param>
        /// <param name="initialSectionProbability">The initial probability for the section.</param>
        /// <param name="furtherAnalysisType">The further analysis type.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="initialFailureMechanismResultType"/> is invalid.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            IAssessmentSection assessmentSection, bool isRelevant, NonAdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
            double initialSectionProbability, FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType, double refinedSectionProbability)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!Enum.IsDefined(typeof(NonAdoptableInitialFailureMechanismResultType), initialFailureMechanismResultType))
            {
                throw new InvalidEnumArgumentException(nameof(initialFailureMechanismResultType),
                                                       (int) initialFailureMechanismResultType,
                                                       typeof(NonAdoptableInitialFailureMechanismResultType));
            }

            bool hasProbabilitySpecified = initialFailureMechanismResultType != NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;

            FailureMechanismSectionAssemblyInput input = CreateInput(
                assessmentSection, isRelevant, initialSectionProbability,
                furtherAnalysisType, refinedSectionProbability, hasProbabilitySpecified);

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
        /// <param name="furtherAnalysisType">The further analysis type.</param>
        /// <param name="refinedProfileProbability">The refined probability for the profile.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        /// <param name="probabilityRefinementType">The <see cref="ProbabilityRefinementType"/> of the section.</param>
        /// <param name="sectionN">The N of the section.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="initialFailureMechanismResultType"/>
        /// or <paramref name="probabilityRefinementType"/> is invalid.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            IAssessmentSection assessmentSection,
            bool isRelevant, AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
            double initialProfileProbability, double initialSectionProbability,
            FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
            double refinedProfileProbability, double refinedSectionProbability,
            ProbabilityRefinementType probabilityRefinementType, double sectionN)
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

            if (!Enum.IsDefined(typeof(ProbabilityRefinementType), probabilityRefinementType))
            {
                throw new InvalidEnumArgumentException(nameof(probabilityRefinementType),
                                                       (int) probabilityRefinementType,
                                                       typeof(ProbabilityRefinementType));
            }

            FailureMechanismSectionWithProfileProbabilityAssemblyInput input = CreateInput(
                assessmentSection, isRelevant, initialFailureMechanismResultType,
                initialProfileProbability, initialSectionProbability, furtherAnalysisType,
                refinedProfileProbability, refinedSectionProbability, probabilityRefinementType, sectionN);

            return PerformAssembly(input);
        }

        /// <summary>
        /// Assembles the failure mechanism section based on the input arguments.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the section belongs to.</param>
        /// <param name="isRelevant">The indicator whether the section is relevant.</param>
        /// <param name="initialFailureMechanismResultType">The <see cref="NonAdoptableInitialFailureMechanismResultType"/> of the section.</param>
        /// <param name="initialProfileProbability">The initial probability for the profile.</param>
        /// <param name="initialSectionProbability">The initial probability for the section.</param>
        /// <param name="furtherAnalysisType">The further analysis type.</param>
        /// <param name="refinedProfileProbability">The refined probability for the profile.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="initialFailureMechanismResultType"/> is invalid.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            IAssessmentSection assessmentSection,
            bool isRelevant, NonAdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
            double initialProfileProbability, double initialSectionProbability,
            FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
            double refinedProfileProbability, double refinedSectionProbability)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!Enum.IsDefined(typeof(NonAdoptableInitialFailureMechanismResultType), initialFailureMechanismResultType))
            {
                throw new InvalidEnumArgumentException(nameof(initialFailureMechanismResultType),
                                                       (int) initialFailureMechanismResultType,
                                                       typeof(NonAdoptableInitialFailureMechanismResultType));
            }

            bool hasProbabilitySpecified = initialFailureMechanismResultType != NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;
            FailureMechanismSectionWithProfileProbabilityAssemblyInput input = CreateInput(
                assessmentSection, isRelevant, hasProbabilitySpecified, initialProfileProbability, initialSectionProbability,
                furtherAnalysisType, refinedProfileProbability, refinedSectionProbability);

            return PerformAssembly(input);
        }

        private static FailureMechanismSectionAssemblyInput CreateInput(IAssessmentSection assessmentSection, bool isRelevant, double initialSectionProbability,
                                                                        FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
                                                                        double refinedSectionProbability, bool hasProbabilitySpecified)
        {
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            return new FailureMechanismSectionAssemblyInput(
                failureMechanismContribution.LowerLimitNorm, failureMechanismContribution.SignalingNorm,
                isRelevant, hasProbabilitySpecified, initialSectionProbability, furtherAnalysisType, refinedSectionProbability);
        }

        private static FailureMechanismSectionWithProfileProbabilityAssemblyInput CreateInput(IAssessmentSection assessmentSection,
                                                                                              bool isRelevant, AdoptableInitialFailureMechanismResultType initialFailureMechanismResultType,
                                                                                              double initialProfileProbability, double initialSectionProbability,
                                                                                              FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
                                                                                              double refinedProfileProbability, double refinedSectionProbability,
                                                                                              ProbabilityRefinementType probabilityRefinementType, double sectionN)
        {
            if (probabilityRefinementType == ProbabilityRefinementType.Profile)
            {
                refinedSectionProbability = refinedProfileProbability * sectionN;
            }

            if (probabilityRefinementType == ProbabilityRefinementType.Section)
            {
                refinedProfileProbability = refinedSectionProbability / sectionN;
            }

            bool hasProbabilitySpecified = initialFailureMechanismResultType != AdoptableInitialFailureMechanismResultType.NoFailureProbability;

            return CreateInput(assessmentSection, isRelevant, hasProbabilitySpecified,
                               initialProfileProbability, initialSectionProbability,
                               furtherAnalysisType, refinedProfileProbability, refinedSectionProbability);
        }

        private static FailureMechanismSectionWithProfileProbabilityAssemblyInput CreateInput(IAssessmentSection assessmentSection,
                                                                                              bool isRelevant, bool hasProbabilitySpecified,
                                                                                              double initialProfileProbability, double initialSectionProbability,
                                                                                              FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
                                                                                              double refinedProfileProbability, double refinedSectionProbability)
        {
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            return new FailureMechanismSectionWithProfileProbabilityAssemblyInput(
                failureMechanismContribution.LowerLimitNorm, failureMechanismContribution.SignalingNorm,
                isRelevant, hasProbabilitySpecified, initialProfileProbability, initialSectionProbability,
                furtherAnalysisType, refinedProfileProbability, refinedSectionProbability);
        }

        /// <summary>
        /// Performs the assembly based on the <see cref="FailureMechanismSectionAssemblyInput"/>.
        /// </summary>
        /// <param name="input">The input to use in the assembly.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
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

        /// <summary>
        /// Performs the assembly based on the <see cref="FailureMechanismSectionWithProfileProbabilityAssemblyInput"/>.
        /// </summary>
        /// <param name="input">The input to use in the assembly.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        private static FailureMechanismSectionAssemblyResult PerformAssembly(FailureMechanismSectionWithProfileProbabilityAssemblyInput input)
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
    }
}