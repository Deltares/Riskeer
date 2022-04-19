// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.Categories;
using Assembly.Kernel.Model.FailureMechanismSections;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Class representing an assessment section assembly calculator.
    /// </summary>
    public class AssessmentSectionAssemblyCalculator : IAssessmentSectionAssemblyCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
        public AssessmentSectionAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public AssessmentSectionAssemblyResultWrapper AssembleAssessmentSection(IEnumerable<double> failureMechanismProbabilities,
                                                                                double maximumAllowableFloodingProbability,
                                                                                double signalFloodingProbability)
        {
            if (failureMechanismProbabilities == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismProbabilities));
            }

            try
            {
                ICategoryLimitsCalculator categoryLimitsKernel = factory.CreateAssemblyGroupsKernel();
                CategoriesList<AssessmentSectionCategory> assessmentSectionCategories = categoryLimitsKernel.CalculateAssessmentSectionCategoryLimitsBoi21(
                    new AssessmentSection(AssemblyCalculatorInputCreator.CreateProbability(signalFloodingProbability),
                                          AssemblyCalculatorInputCreator.CreateProbability(maximumAllowableFloodingProbability)));

                IAssessmentGradeAssembler assessmentSectionAssemblyKernel = factory.CreateAssessmentSectionAssemblyKernel();
                IEnumerable<Probability> probabilities = failureMechanismProbabilities.Select(AssemblyCalculatorInputCreator.CreateProbability)
                                                                                      .ToArray();

                Probability assemblyProbability = assessmentSectionAssemblyKernel.CalculateAssessmentSectionFailureProbabilityBoi2A1(probabilities, false);
                EAssessmentGrade assemblyCategory = assessmentSectionAssemblyKernel.DetermineAssessmentGradeBoi2B1(assemblyProbability, assessmentSectionCategories);

                return new AssessmentSectionAssemblyResultWrapper(
                    new AssessmentSectionAssemblyResult(assemblyProbability,
                                                        AssessmentSectionAssemblyGroupCreator.CreateAssessmentSectionAssemblyGroup(assemblyCategory)),
                    AssemblyMethod.BOI2A1, AssemblyMethod.BOI2A2);
            }
            catch (AssemblyException e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public IEnumerable<CombinedFailureMechanismSectionAssembly> AssembleCombinedFailureMechanismSections(
            IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input, double assessmentSectionLength)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            try
            {
                ICommonFailureMechanismSectionAssembler kernel = factory.CreateCombinedFailureMechanismSectionAssemblyKernel();
                GreatestCommonDenominatorAssemblyResult output = kernel.AssembleCommonFailureMechanismSections(FailureMechanismSectionListCreator.Create(input), assessmentSectionLength, false);

                return CombinedFailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }
    }
}