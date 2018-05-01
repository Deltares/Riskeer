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
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Factory for assembling assembly results for a piping failure mechanism.
    /// </summary>
    public static class PipingFailureMechanismAssemblyFactory
    {
        /// <summary>
        /// Assembles the simple assessment results.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to assemble the 
        /// simple assembly results for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/> based on the <paramref name="failureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
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
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the detailed assessment result.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to
        /// assemble the detailed assembly for.</param>
        /// <param name="calculationScenarios">The calculation scenarios belonging to this section.</param>
        /// <param name="failureMechanism">The failure mechanism this section belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> this section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssembly AssembleDetailedAssessment(
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
            IFailureMechanismSectionAssemblyCalculator calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.AssembleDetailedAssessment(
                    failureMechanismSectionResult.DetailedAssessmentResult,
                    failureMechanismSectionResult.GetDetailedAssessmentProbability(calculationScenarios, failureMechanism, assessmentSection),
                    failureMechanism.PipingProbabilityAssessmentInput.GetN(failureMechanismSectionResult.Section.Length),
                    CreateAssemblyCategoriesInput(failureMechanism, assessmentSection));
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the tailor made assessment result.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to
        /// assemble the tailor made assembly for.</param>
        /// <param name="failureMechanism">The failure mechanism this section belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> this section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssembly AssembleTailorMadeAssessment(
            PipingFailureMechanismSectionResult failureMechanismSectionResult,
            PipingFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
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
            IFailureMechanismSectionAssemblyCalculator calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.AssembleTailorMadeAssessment(
                    failureMechanismSectionResult.TailorMadeAssessmentResult,
                    failureMechanismSectionResult.TailorMadeAssessmentProbability,
                    failureMechanism.PipingProbabilityAssessmentInput.GetN(failureMechanismSectionResult.Section.Length),
                    CreateAssemblyCategoriesInput(failureMechanism, assessmentSection));
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the combined assembly.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to
        /// combine the assemblies for.</param>
        /// <param name="calculationScenarios">The calculation scenarios belonging to this section.</param>
        /// <param name="failureMechanism">The failure mechanism this section belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> this section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssembly AssembleCombinedAssessment(
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

            FailureMechanismSectionAssembly simpleAssembly = AssembleSimpleAssessment(failureMechanismSectionResult);
            FailureMechanismSectionAssembly detailedAssembly = AssembleDetailedAssessment(
                failureMechanismSectionResult, calculationScenarios, failureMechanism, assessmentSection);
            FailureMechanismSectionAssembly tailorMadeAssembly = AssembleTailorMadeAssessment(
                failureMechanismSectionResult, failureMechanism, assessmentSection);

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Assembles the failure mechanism assembly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assemble for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the failure mechanism belongs to.</param>
        /// <param name="considerManualAssembly">Indicator whether the manual assembly should be used in the assembly.</param>
        /// <returns>A <see cref="FailureMechanismAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismAssembly AssembleFailureMechanism(
            PipingFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection,
            bool considerManualAssembly = true)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator sectionCalculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput(failureMechanism, assessmentSection);
            var sectionAssemblies = new List<FailureMechanismSectionAssembly>();

            try
            {
                foreach (PipingFailureMechanismSectionResult sectionResult in failureMechanism.SectionResults)
                {
                    if (sectionResult.UseManualAssemblyProbability && considerManualAssembly)
                    {
                        sectionAssemblies.Add(sectionCalculator.AssembleDetailedAssessment(
                                                  DetailedAssessmentProbabilityOnlyResultType.Probability,
                                                  sectionResult.ManualAssemblyProbability,
                                                  failureMechanism.PipingProbabilityAssessmentInput.GetN(sectionResult.Section.Length),
                                                  assemblyCategoriesInput));
                    }
                    else
                    {
                        sectionAssemblies.Add(AssembleCombinedAssessment(sectionResult,
                                                                         failureMechanism.Calculations.Cast<PipingCalculationScenario>(),
                                                                         failureMechanism,
                                                                         assessmentSection));
                    }
                }

                IFailureMechanismAssemblyCalculator calculator =
                    calculatorFactory.CreateFailureMechanismAssemblyCalculator(AssemblyToolKernelFactory.Instance);

                return calculator.Assemble(sectionAssemblies, assemblyCategoriesInput);
            }
            catch (Exception e) when (e is FailureMechanismAssemblyCalculatorException || e is FailureMechanismSectionAssemblyCalculatorException)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput(PipingFailureMechanism failureMechanism,
                                                                             IAssessmentSection assessmentSection)
        {
            return new AssemblyCategoriesInput(failureMechanism.PipingProbabilityAssessmentInput.GetN(
                                                   failureMechanism.PipingProbabilityAssessmentInput.SectionLength),
                                               failureMechanism.Contribution,
                                               assessmentSection.FailureMechanismContribution.SignalingNorm,
                                               assessmentSection.FailureMechanismContribution.LowerLimitNorm);
        }
    }
}