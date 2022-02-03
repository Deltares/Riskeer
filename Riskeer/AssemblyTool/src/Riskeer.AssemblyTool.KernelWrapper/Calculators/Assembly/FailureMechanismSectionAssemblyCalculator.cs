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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.Categories;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using AssemblyFailureMechanismSectionAssemblyResult = Assembly.Kernel.Model.FailureMechanismSections.FailureMechanismSectionAssemblyResult;
using RiskeerFailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

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
        /// <exception cref="ArgumentNullException">Thrown when any <paramref name="factory"/> is <c>null</c>.</exception>
        public FailureMechanismSectionAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public RiskeerFailureMechanismSectionAssemblyResult AssembleFailureMechanismSection(FailureMechanismSectionAssemblyInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            try
            {
                ICategoryLimitsCalculator assemblyCategoriesKernel = factory.CreateAssemblyCategoriesKernel();
                CategoriesList<InterpretationCategory> categories = assemblyCategoriesKernel.CalculateInterpretationCategoryLimitsWbi03(
                    new AssessmentSection(CreateProbability(input.SignalingNorm), CreateProbability(input.LowerLimitNorm)));

                IAssessmentResultsTranslator kernel = factory.CreateFailureMechanismSectionAssemblyKernel();

                AssemblyFailureMechanismSectionAssemblyResult output = kernel.TranslateAssessmentResultWbi0A2(GetInitialMechanismProbabilitySpecification(input),
                                                                                                              CreateProbability(input.InitialSectionProbability),
                                                                                                              input.FurtherAnalysisNeeded,
                                                                                                              CreateProbability(input.RefinedSectionProbability),
                                                                                                              categories);

                return FailureMechanismSectionAssemblyResultCreator.CreateFailureMechanismSectionAssemblyResult(output);
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

        public RiskeerFailureMechanismSectionAssemblyResult AssembleFailureMechanismSection(FailureMechanismSectionWithProfileProbabilityAssemblyInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            try
            {
                ICategoryLimitsCalculator assemblyCategoriesKernel = factory.CreateAssemblyCategoriesKernel();
                CategoriesList<InterpretationCategory> categories = assemblyCategoriesKernel.CalculateInterpretationCategoryLimitsWbi03(
                    new AssessmentSection(CreateProbability(input.SignalingNorm), CreateProbability(input.LowerLimitNorm)));

                IAssessmentResultsTranslator kernel = factory.CreateFailureMechanismSectionAssemblyKernel();

                AssemblyFailureMechanismSectionAssemblyResult output = kernel.TranslateAssessmentResultWbi0A2(GetInitialMechanismProbabilitySpecification(input),
                                                                                                              CreateProbability(input.InitialProfileProbability),
                                                                                                              CreateProbability(input.InitialSectionProbability),
                                                                                                              input.FurtherAnalysisNeeded,
                                                                                                              CreateProbability(input.RefinedProfileProbability),
                                                                                                              CreateProbability(input.RefinedSectionProbability),
                                                                                                              categories);

                return FailureMechanismSectionAssemblyResultCreator.CreateFailureMechanismSectionAssemblyResult(output);
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

        private static ESectionInitialMechanismProbabilitySpecification GetInitialMechanismProbabilitySpecification(FailureMechanismSectionAssemblyInput input)
        {
            if (!input.IsRelevant)
            {
                return ESectionInitialMechanismProbabilitySpecification.NotRelevant;
            }

            return input.HasProbabilitySpecified
                       ? ESectionInitialMechanismProbabilitySpecification.RelevantWithProbabilitySpecification
                       : ESectionInitialMechanismProbabilitySpecification.RelevantNoProbabilitySpecification;
        }

        private static Probability CreateProbability(double value)
        {
            return new Probability(value);
        }
    }
}