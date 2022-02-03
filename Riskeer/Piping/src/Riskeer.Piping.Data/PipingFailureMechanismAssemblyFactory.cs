﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Primitives;
using Riskeer.Piping.Data.SemiProbabilistic;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Piping.Data
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
        /// <returns>A <see cref="FailureMechanismSectionAssemblyOld"/> based on the <paramref name="failureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyOld"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyOld AssembleSimpleAssessment(
            PipingFailureMechanismSectionResultOld failureMechanismSectionResult)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
            IFailureMechanismSectionAssemblyCalculatorOld calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

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
        /// <returns>A <see cref="FailureMechanismSectionAssemblyOld"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyOld"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyOld AssembleDetailedAssessment(
            PipingFailureMechanismSectionResultOld failureMechanismSectionResult,
            IEnumerable<SemiProbabilisticPipingCalculationScenario> calculationScenarios,
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

            IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
            IFailureMechanismSectionAssemblyCalculatorOld calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

            try
            {
                return calculator.AssembleDetailedAssessment(
                    failureMechanismSectionResult.DetailedAssessmentResult,
                    failureMechanismSectionResult.GetDetailedAssessmentProbability(calculationScenarios, assessmentSection.FailureMechanismContribution.Norm),
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
        /// <returns>A <see cref="FailureMechanismSectionAssemblyOld"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyOld"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyOld AssembleTailorMadeAssessment(
            PipingFailureMechanismSectionResultOld failureMechanismSectionResult,
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

            IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
            IFailureMechanismSectionAssemblyCalculatorOld calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

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
        /// <returns>A <see cref="FailureMechanismSectionAssemblyOld"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyOld"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyOld AssembleCombinedAssessment(
            PipingFailureMechanismSectionResultOld failureMechanismSectionResult,
            IEnumerable<SemiProbabilisticPipingCalculationScenario> calculationScenarios,
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

            IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
            IFailureMechanismSectionAssemblyCalculatorOld calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

            try
            {
                FailureMechanismSectionAssemblyOld simpleAssembly = AssembleSimpleAssessment(failureMechanismSectionResult);

                if (failureMechanismSectionResult.SimpleAssessmentResult == SimpleAssessmentResultType.ProbabilityNegligible
                    || failureMechanismSectionResult.SimpleAssessmentResult == SimpleAssessmentResultType.NotApplicable)
                {
                    return calculator.AssembleCombined(simpleAssembly);
                }

                return calculator.AssembleCombined(
                    simpleAssembly,
                    AssembleDetailedAssessment(failureMechanismSectionResult, calculationScenarios, failureMechanism, assessmentSection),
                    AssembleTailorMadeAssessment(failureMechanismSectionResult, failureMechanism, assessmentSection));
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the assembly category group of the given <paramref name="failureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to get the assembly category group for.</param>
        /// <param name="failureMechanism">The failure mechanism this section belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> this section belongs to.</param>
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup GetSectionAssemblyCategoryGroup(PipingFailureMechanismSectionResultOld failureMechanismSectionResult,
                                                                                                   PipingFailureMechanism failureMechanism,
                                                                                                   IAssessmentSection assessmentSection,
                                                                                                   bool useManual)
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

            return GetSectionAssembly(failureMechanismSectionResult, failureMechanism, assessmentSection, useManual).Group;
        }

        /// <summary>
        /// Assembles the failure mechanism assembly.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assemble for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the failure mechanism belongs to.</param>
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A <see cref="FailureMechanismAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismAssembly AssembleFailureMechanism(
            PipingFailureMechanism failureMechanism,
            IAssessmentSection assessmentSection,
            bool useManual)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!failureMechanism.InAssembly)
            {
                return FailureMechanismAssemblyResultFactoryOld.CreateNotApplicableAssembly();
            }

            try
            {
                IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
                AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput(failureMechanism, assessmentSection);
                IEnumerable<FailureMechanismSectionAssemblyOld> sectionAssemblies = failureMechanism.SectionResultsOld
                                                                                                    .Select(sr => GetSectionAssembly(sr, failureMechanism, assessmentSection, useManual))
                                                                                                    .ToArray();

                IFailureMechanismAssemblyCalculatorOld calculator =
                    calculatorFactory.CreateFailureMechanismAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

                return calculator.Assemble(sectionAssemblies, assemblyCategoriesInput);
            }
            catch (FailureMechanismAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
            catch (AssemblyException e)
            {
                throw new AssemblyException(RiskeerCommonDataResources.FailureMechanismAssemblyFactory_Error_while_assembling_failureMechanism, e);
            }
        }

        /// <summary>
        /// Gets the assembly of the given <paramref name="failureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResult">The failure mechanism section result to get the assembly for.</param>
        /// <param name="failureMechanism">The failure mechanism to assemble for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the failure mechanism belongs to.</param>
        /// <param name="useManual">Indicator that determines whether the manual assembly should be considered when assembling the result.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyOld"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when a <see cref="FailureMechanismSectionAssemblyOld"/>
        /// could not be created.</exception>
        private static FailureMechanismSectionAssemblyOld GetSectionAssembly(PipingFailureMechanismSectionResultOld failureMechanismSectionResult,
                                                                             PipingFailureMechanism failureMechanism,
                                                                             IAssessmentSection assessmentSection,
                                                                             bool useManual)
        {
            FailureMechanismSectionAssemblyOld sectionAssembly;
            if (failureMechanismSectionResult.UseManualAssembly && useManual)
            {
                sectionAssembly = AssembleManualAssessment(failureMechanismSectionResult,
                                                           failureMechanism,
                                                           CreateAssemblyCategoriesInput(failureMechanism, assessmentSection));
            }
            else
            {
                sectionAssembly = AssembleCombinedAssessment(failureMechanismSectionResult,
                                                             failureMechanism.Calculations.OfType<SemiProbabilisticPipingCalculationScenario>(),
                                                             failureMechanism,
                                                             assessmentSection);
            }

            return sectionAssembly;
        }

        /// <summary>
        /// Assembles the manual assembly.
        /// </summary>
        /// <param name="sectionResult">The failure mechanism section result to assemble the 
        /// manual assembly for.</param>
        /// <param name="failureMechanism">The failure mechanism to assemble for.</param>
        /// <param name="assemblyCategoriesInput">The input parameters used to determine the assembly categories.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyOld"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssemblyOld"/>
        /// could not be created.</exception>
        private static FailureMechanismSectionAssemblyOld AssembleManualAssessment(PipingFailureMechanismSectionResultOld sectionResult,
                                                                                   PipingFailureMechanism failureMechanism,
                                                                                   AssemblyCategoriesInput assemblyCategoriesInput)
        {
            IAssemblyToolCalculatorFactoryOld calculatorFactory = AssemblyToolCalculatorFactoryOld.Instance;
            IFailureMechanismSectionAssemblyCalculatorOld calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactoryOld.Instance);

            try
            {
                return calculator.AssembleManual(sectionResult.ManualAssemblyProbability,
                                                 failureMechanism.PipingProbabilityAssessmentInput.GetN(
                                                     sectionResult.Section.Length),
                                                 assemblyCategoriesInput);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput(PipingFailureMechanism failureMechanism,
                                                                             IAssessmentSection assessmentSection)
        {
            return AssemblyCategoriesInputFactory.CreateAssemblyCategoriesInput(failureMechanism.PipingProbabilityAssessmentInput.GetN(
                                                                                    assessmentSection.ReferenceLine.Length),
                                                                                failureMechanism,
                                                                                assessmentSection);
        }
    }
}