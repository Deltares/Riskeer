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
        /// <param name="initialProfileProbability">The initial probability for the profile.</param>
        /// <param name="initialSectionProbability">The initial probability for the section.</param>
        /// <param name="furtherAnalysisNeeded">The indicator whether the section needs further analysis.</param>
        /// <param name="refinedProfileProbability">The refined probability for the profile.</param>
        /// <param name="refinedSectionProbability">The refined probability for the section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be successfully assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(IAssessmentSection assessmentSection,
                                                                            bool isRelevant,
                                                                            double initialProfileProbability, double initialSectionProbability,
                                                                            bool furtherAnalysisNeeded,
                                                                            double refinedProfileProbability, double refinedSectionProbability)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IFailureMechanismSectionAssemblyCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateFailureMechanismSectionAssemblyCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                FailureMechanismSectionAssemblyInput input = CreateInput(assessmentSection,
                                                                         isRelevant,
                                                                         initialProfileProbability, initialSectionProbability,
                                                                         furtherAnalysisNeeded,
                                                                         refinedProfileProbability, refinedSectionProbability);

                return calculator.AssembleFailureMechanismSection(input);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        private static FailureMechanismSectionAssemblyInput CreateInput(IAssessmentSection assessmentSection,
                                                                        bool isRelevant,
                                                                        double initialProfileProbability, double initialSectionProbability,
                                                                        bool furtherAnalysisNeeded,
                                                                        double refinedProfileProbability, double refinedSectionProbability)
        {
            FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
            return new FailureMechanismSectionAssemblyInput(failureMechanismContribution.SignalingNorm,
                                                            failureMechanismContribution.LowerLimitNorm,
                                                            isRelevant,
                                                            initialProfileProbability, initialSectionProbability,
                                                            furtherAnalysisNeeded,
                                                            refinedProfileProbability, refinedSectionProbability);
        }
    }
}