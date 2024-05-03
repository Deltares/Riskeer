// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.Common.Primitives;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Class representing a failure mechanism section assembly calculator.
    /// </summary>
    public class FailureMechanismSectionAssemblyCalculator : IFailureMechanismSectionAssemblyCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
        public FailureMechanismSectionAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public FailureMechanismSectionAssemblyResultWrapper AssembleFailureMechanismSection(FailureMechanismSectionAssemblyInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            try
            {
                IAssessmentResultsTranslator kernel = factory.CreateFailureMechanismSectionAssemblyKernel();

                if (!IsProbabilityDefined(input))
                {
                    return AssembleWithUndefinedProbabilities(input, kernel);
                }

                Probability sectionProbability = kernel.DetermineRepresentativeProbabilityBoi0A1(
                    input.FurtherAnalysisType != FailureMechanismSectionResultFurtherAnalysisType.NotNecessary,
                    AssemblyCalculatorInputCreator.CreateProbability(input.InitialSectionProbability),
                    AssemblyCalculatorInputCreator.CreateProbability(input.RefinedSectionProbability));
                EInterpretationCategory interpretationCategory = AssembleInterpretationCategory(input, kernel, sectionProbability);

                return new FailureMechanismSectionAssemblyResultWrapper(
                    FailureMechanismSectionAssemblyResultCreator.Create(sectionProbability, interpretationCategory),
                    AssemblyMethod.BOI0A1, AssemblyMethod.BOI0B1);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        private static bool IsProbabilityDefined(FailureMechanismSectionAssemblyInput input)
        {
            if (!input.IsRelevant)
            {
                return false;
            }

            return input.FurtherAnalysisType == FailureMechanismSectionResultFurtherAnalysisType.NotNecessary
                   && input.HasProbabilitySpecified
                   || input.FurtherAnalysisType == FailureMechanismSectionResultFurtherAnalysisType.Executed;
        }

        private static FailureMechanismSectionAssemblyResultWrapper AssembleWithUndefinedProbabilities(FailureMechanismSectionAssemblyInput input, IAssessmentResultsTranslator kernel)
        {
            EInterpretationCategory interpretationCategory = kernel.DetermineInterpretationCategoryWithoutProbabilityEstimationBoi0C1(GetAnalysisStatus(input));
            Probability sectionProbability = kernel.TranslateInterpretationCategoryToProbabilityBoi0C2(interpretationCategory);

            return new FailureMechanismSectionAssemblyResultWrapper(
                FailureMechanismSectionAssemblyResultCreator.Create(sectionProbability, interpretationCategory),
                AssemblyMethod.BOI0C2, AssemblyMethod.BOI0C1);
        }

        private static EAnalysisState GetAnalysisStatus(FailureMechanismSectionAssemblyInput input)
        {
            if (!input.IsRelevant)
            {
                return EAnalysisState.NotRelevant;
            }

            if (input.FurtherAnalysisType == FailureMechanismSectionResultFurtherAnalysisType.NotNecessary)
            {
                return !input.HasProbabilitySpecified
                           ? EAnalysisState.NoProbabilityEstimationNecessary
                           : EAnalysisState.ProbabilityEstimated;
            }

            return input.FurtherAnalysisType == FailureMechanismSectionResultFurtherAnalysisType.Necessary
                       ? EAnalysisState.ProbabilityEstimationNecessary
                       : EAnalysisState.ProbabilityEstimated;
        }

        private EInterpretationCategory AssembleInterpretationCategory(FailureMechanismSectionAssemblyInput input, IAssessmentResultsTranslator kernel, Probability probability)
        {
            ICategoryLimitsCalculator assemblyGroupsKernel = factory.CreateAssemblyGroupsKernel();
            CategoriesList<InterpretationCategory> interpretationCategories = assemblyGroupsKernel.CalculateInterpretationCategoryLimitsBoi01(
                new AssessmentSection(AssemblyCalculatorInputCreator.CreateProbability(input.SignalFloodingProbability),
                                      AssemblyCalculatorInputCreator.CreateProbability(input.MaximumAllowableFloodingProbability)));

            return kernel.DetermineInterpretationCategoryFromFailureMechanismSectionProbabilityBoi0B1(
                probability, interpretationCategories);
        }
    }
}