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
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Primitives;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Data
{
    /// <summary>
    /// Factory for assembling assembly results for a stability point structures failure mechanism.
    /// </summary>
    public static class StabilityPointStructuresFailureMechanismAssemblyFactory
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
            StabilityPointStructuresFailureMechanismSectionResult failureMechanismSectionResult)
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
        /// <param name="calculationScenarios">All calculation scenarios in the failure mechanism.</param>
        /// <param name="failureMechanism">The failure mechanism this section belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> this section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssembly AssembleDetailedAssessment(
            StabilityPointStructuresFailureMechanismSectionResult failureMechanismSectionResult,
            IEnumerable<StructuresCalculationScenario<StabilityPointStructuresInput>> calculationScenarios,
            StabilityPointStructuresFailureMechanism failureMechanism,
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
            StabilityPointStructuresFailureMechanismSectionResult failureMechanismSectionResult,
            StabilityPointStructuresFailureMechanism failureMechanism,
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
        /// <param name="calculationScenarios">All calculation scenarios in the failure mechanism.</param>
        /// <param name="failureMechanism">The failure mechanism this section belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> this section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        public static FailureMechanismSectionAssembly AssembleCombinedAssessment(
            StabilityPointStructuresFailureMechanismSectionResult failureMechanismSectionResult,
            IEnumerable<StructuresCalculationScenario<StabilityPointStructuresInput>> calculationScenarios,
            StabilityPointStructuresFailureMechanism failureMechanism,
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
                FailureMechanismSectionAssembly simpleAssembly = AssembleSimpleAssessment(failureMechanismSectionResult);

                if (failureMechanismSectionResult.SimpleAssessmentResult == SimpleAssessmentValidityOnlyResultType.NotApplicable)
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
        public static FailureMechanismSectionAssemblyCategoryGroup GetSectionAssemblyCategoryGroup(StabilityPointStructuresFailureMechanismSectionResult failureMechanismSectionResult,
                                                                                                   StabilityPointStructuresFailureMechanism failureMechanism,
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
            StabilityPointStructuresFailureMechanism failureMechanism,
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

            if (!failureMechanism.IsRelevant)
            {
                return FailureMechanismAssemblyResultFactory.CreateNotApplicableAssembly();
            }

            try
            {
                IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput(failureMechanism, assessmentSection);
                IEnumerable<FailureMechanismSectionAssembly> sectionAssemblies = failureMechanism.SectionResults
                                                                                                 .Select(sr => GetSectionAssembly(sr, failureMechanism, assessmentSection, useManual))
                                                                                                 .ToArray();

                IFailureMechanismAssemblyCalculator calculator =
                    calculatorFactory.CreateFailureMechanismAssemblyCalculator(AssemblyToolKernelFactory.Instance);

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
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when a <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        private static FailureMechanismSectionAssembly GetSectionAssembly(StabilityPointStructuresFailureMechanismSectionResult failureMechanismSectionResult,
                                                                          StabilityPointStructuresFailureMechanism failureMechanism,
                                                                          IAssessmentSection assessmentSection,
                                                                          bool useManual)
        {
            FailureMechanismSectionAssembly sectionAssembly;
            if (failureMechanismSectionResult.UseManualAssembly && useManual)
            {
                sectionAssembly = AssembleManualAssessment(failureMechanismSectionResult,
                                                           CreateAssemblyCategoriesInput(failureMechanism, assessmentSection));
            }
            else
            {
                sectionAssembly = AssembleCombinedAssessment(failureMechanismSectionResult,
                                                             failureMechanism.Calculations.Cast<StructuresCalculationScenario<StabilityPointStructuresInput>>(),
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
        /// <param name="assemblyCategoriesInput">The input parameters used to determine the assembly categories.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when the <see cref="FailureMechanismSectionAssembly"/>
        /// could not be created.</exception>
        private static FailureMechanismSectionAssembly AssembleManualAssessment(StabilityPointStructuresFailureMechanismSectionResult sectionResult,
                                                                                AssemblyCategoriesInput assemblyCategoriesInput)
        {
            IAssemblyToolCalculatorFactory calculatorFactory = AssemblyToolCalculatorFactory.Instance;
            IFailureMechanismSectionAssemblyCalculator calculator =
                calculatorFactory.CreateFailureMechanismSectionAssemblyCalculator(AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.AssembleManual(sectionResult.ManualAssemblyProbability, assemblyCategoriesInput);
            }
            catch (FailureMechanismSectionAssemblyCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput(StabilityPointStructuresFailureMechanism failureMechanism,
                                                                             IAssessmentSection assessmentSection)
        {
            return AssemblyCategoriesInputFactory.CreateAssemblyCategoriesInput(failureMechanism.GeneralInput.N,
                                                                                failureMechanism,
                                                                                assessmentSection);
        }
    }
}