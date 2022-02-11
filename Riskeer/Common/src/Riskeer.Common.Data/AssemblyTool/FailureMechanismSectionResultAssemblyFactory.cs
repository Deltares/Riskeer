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
    public static class FailureMechanismSectionResultAssemblyFactory
    {
        /// <summary>
        /// Assembles a failure mechanism section result based on the input arguments. 
        /// </summary>
        /// <param name="sectionResult">The section result to assemble for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to assemble with.</param>
        /// <param name="calculateProbabilityStrategy">The <see cref="IFailureMechanismSectionResultCalculateProbabilityStrategy"/>
        /// to assemble with.</param>
        /// <param name="useLengthEffect">Indicator whether to use the length effect or not.</param>
        /// <param name="sectionN">The N value for the section.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any <paramref name="sectionResult"/>,
        /// <paramref name="assessmentSection"/> or <paramref name="calculateProbabilityStrategy"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult, IAssessmentSection assessmentSection,
            IFailureMechanismSectionResultCalculateProbabilityStrategy calculateProbabilityStrategy,
            bool useLengthEffect, double sectionN)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculateProbabilityStrategy == null)
            {
                throw new ArgumentNullException(nameof(calculateProbabilityStrategy));
            }

            bool adopt = sectionResult.InitialFailureMechanismResultType == AdoptableInitialFailureMechanismResultType.Adopt;
            double initialFailureMechanismResultSectionProbability =
                adopt
                    ? calculateProbabilityStrategy.CalculateSectionProbability()
                    : sectionResult.ManualInitialFailureMechanismResultSectionProbability;

            FailureMechanismSectionAssemblyInput input;

            if (useLengthEffect)
            {
                double initialFailureMechanismResultProfileProbability =
                    adopt
                        ? calculateProbabilityStrategy.CalculateProfileProbability()
                        : sectionResult.ManualInitialFailureMechanismResultProfileProbability;

                input = CreateInput(
                    assessmentSection, sectionResult.IsRelevant, sectionResult.InitialFailureMechanismResultType,
                    initialFailureMechanismResultProfileProbability, initialFailureMechanismResultSectionProbability, sectionResult.FurtherAnalysisType,
                    sectionResult.RefinedProfileProbability, sectionResult.RefinedSectionProbability, sectionResult.ProbabilityRefinementType, sectionN);
            }
            else
            {
                bool hasProbabilitySpecified = sectionResult.InitialFailureMechanismResultType != AdoptableInitialFailureMechanismResultType.NoFailureProbability;

                input = CreateInput(
                    assessmentSection, sectionResult.IsRelevant, initialFailureMechanismResultSectionProbability,
                    sectionResult.FurtherAnalysisType, sectionResult.RefinedSectionProbability, hasProbabilitySpecified);
            }

            return PerformAssembly(input);
        }

        /// <summary>
        /// Assembles a failure mechanism section result based on the input arguments. 
        /// </summary>
        /// <param name="sectionResult">The section result to assemble for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to assemble with.</param>
        /// <param name="calculateProbabilityFunc">The <see cref="Func{TResult}"/> to calculate the probability with.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            AdoptableFailureMechanismSectionResult sectionResult, IAssessmentSection assessmentSection, Func<double> calculateProbabilityFunc)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (calculateProbabilityFunc == null)
            {
                throw new ArgumentNullException(nameof(calculateProbabilityFunc));
            }

            double initialFailureMechanismResultSectionProbability =
                sectionResult.InitialFailureMechanismResultType == AdoptableInitialFailureMechanismResultType.Adopt
                    ? calculateProbabilityFunc()
                    : sectionResult.ManualInitialFailureMechanismResultSectionProbability;

            bool hasProbabilitySpecified = sectionResult.InitialFailureMechanismResultType != AdoptableInitialFailureMechanismResultType.NoFailureProbability;

            FailureMechanismSectionAssemblyInput input = CreateInput(
                assessmentSection, sectionResult.IsRelevant, initialFailureMechanismResultSectionProbability,
                sectionResult.FurtherAnalysisType, sectionResult.RefinedSectionProbability, hasProbabilitySpecified);

            return PerformAssembly(input);
        }

        /// <summary>
        /// Assembles a failure mechanism section result based on the input arguments. 
        /// </summary>
        /// <param name="sectionResult">The section result to assemble for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to assemble with.</param>
        /// <param name="useLengthEffect">Indicator whether to use the length effect or not.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any <paramref name="sectionResult"/> or <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult, IAssessmentSection assessmentSection,
            bool useLengthEffect)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            bool hasProbabilitySpecified = sectionResult.InitialFailureMechanismResultType != NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;

            FailureMechanismSectionAssemblyInput input = useLengthEffect
                                                             ? CreateInput(
                                                                 assessmentSection, sectionResult.IsRelevant, hasProbabilitySpecified,
                                                                 sectionResult.ManualInitialFailureMechanismResultProfileProbability,
                                                                 sectionResult.ManualInitialFailureMechanismResultSectionProbability,
                                                                 sectionResult.FurtherAnalysisType, sectionResult.RefinedProfileProbability,
                                                                 sectionResult.RefinedSectionProbability)
                                                             : CreateInput(
                                                                 assessmentSection, sectionResult.IsRelevant,
                                                                 sectionResult.ManualInitialFailureMechanismResultSectionProbability,
                                                                 sectionResult.FurtherAnalysisType, sectionResult.RefinedSectionProbability,
                                                                 hasProbabilitySpecified);

            return PerformAssembly(input);
        }

        /// <summary>
        /// Assembles a failure mechanism section result based on the input arguments. 
        /// </summary>
        /// <param name="sectionResult">The section result to assemble for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to assemble with.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(
            NonAdoptableFailureMechanismSectionResult sectionResult, IAssessmentSection assessmentSection)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            bool hasProbabilitySpecified = sectionResult.InitialFailureMechanismResultType != NonAdoptableInitialFailureMechanismResultType.NoFailureProbability;

            FailureMechanismSectionAssemblyInput input = CreateInput(
                assessmentSection, sectionResult.IsRelevant, sectionResult.ManualInitialFailureMechanismResultSectionProbability,
                sectionResult.FurtherAnalysisType, sectionResult.RefinedSectionProbability, hasProbabilitySpecified);

            return PerformAssembly(input);
        }

        private static FailureMechanismSectionAssemblyInput CreateInput(
            IAssessmentSection assessmentSection, bool isRelevant, double initialSectionProbability,
            FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType,
            double refinedSectionProbability, bool hasProbabilitySpecified)
        {
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            return new FailureMechanismSectionAssemblyInput(
                failureMechanismContribution.LowerLimitNorm, failureMechanismContribution.SignalingNorm,
                isRelevant, hasProbabilitySpecified, initialSectionProbability, furtherAnalysisType, refinedSectionProbability);
        }

        private static FailureMechanismSectionWithProfileProbabilityAssemblyInput CreateInput(
            IAssessmentSection assessmentSection,
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

        private static FailureMechanismSectionWithProfileProbabilityAssemblyInput CreateInput(
            IAssessmentSection assessmentSection,
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
                return input is FailureMechanismSectionWithProfileProbabilityAssemblyInput probabilityAssemblyInput
                           ? calculator.AssembleFailureMechanismSection(probabilityAssemblyInput)
                           : calculator.AssembleFailureMechanismSection(input);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }
    }
}