﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Factory for assembling the assembly tool results for piping failure mechanism section results.
    /// </summary>
    public static class PipingFailureMechanismSectionResultAssemblyFactory
    {
        /// <summary>
        /// Assembles the simple assessment results.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to assemble the 
        /// simple assembly results for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/> based on the <paramref name="failureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when <see cref="FailureMechanismSectionAssembly"/>
        /// cannot be assembled.</exception>
        public static FailureMechanismSectionAssembly AssembleSimpleAssessment(
            PipingFailureMechanismSectionResult failureMechanismSectionResult)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.AssembleSimpleAssessment(failureMechanismSectionResult.SimpleAssessmentResult);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyFactoryException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the detailed assessment result.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to
        /// assemble the detailed assembly for.</param>
        /// <param name="calculationScenarios">The calculations belonging to this section.</param>
        /// <param name="failureMechanism">The failure mechanism.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/>.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when <see cref="FailureMechanismSectionAssembly"/>
        /// cannot be assembled.</exception>
        public static FailureMechanismSectionAssembly AssembleDetailedAssembly(
            PipingFailureMechanismSectionResult failureMechanismSectionResult,
            IEnumerable<PipingCalculationScenario> calculationScenarios,
            PipingFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator calculator = calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                IEnumerable<FailureMechanismSectionAssemblyCategory> categories = AssemblyToolCategoriesFactory.CreateFailureMechanismSectionAssemblyCategories(
                    assessmentSection.FailureMechanismContribution.SignalingNorm,
                    assessmentSection.FailureMechanismContribution.LowerLimitNorm,
                    failureMechanism.Contribution,
                    failureMechanism.PipingProbabilityAssessmentInput.GetN(failureMechanism.PipingProbabilityAssessmentInput.SectionLength));

                return calculator.AssembleDetailedAssessment(failureMechanismSectionResult.GetDetailedAssessmentProbability(calculationScenarios, failureMechanism, assessmentSection),
                                                             categories,
                                                             failureMechanism.PipingProbabilityAssessmentInput.GetN(failureMechanismSectionResult.Section.Length));
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyFactoryException(e.Message, e);
            }
        }
    }
}