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
using Assembly.Kernel.Old.Exceptions;
using Assembly.Kernel.Old.Interfaces;
using Assembly.Kernel.Old.Model;
using Assembly.Kernel.Old.Model.CategoryLimits;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Class representing an assessment section assembly calculator.
    /// </summary>
    public class AssessmentSectionAssemblyCalculatorOld : IAssessmentSectionAssemblyCalculatorOld
    {
        private readonly IAssemblyToolKernelFactoryOld factory;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyCalculatorOld"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssessmentSectionAssemblyCalculatorOld(IAssemblyToolKernelFactoryOld factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public FailureMechanismAssembly AssembleFailureMechanisms(IEnumerable<FailureMechanismAssembly> input,
                                                                  double signalingNorm, double lowerLimitNorm,
                                                                  double failureProbabilityMarginFactor)
        {
            try
            {
                ICategoryLimitsCalculator categoriesKernel = factory.CreateAssemblyCategoriesKernel();
                CategoriesList<FailureMechanismCategory> categories = categoriesKernel.CalculateFailureMechanismCategoryLimitsWbi11(
                    new AssessmentSection(1, signalingNorm, lowerLimitNorm),
                    new FailureMechanism(1, failureProbabilityMarginFactor));

                IAssessmentGradeAssembler kernel = factory.CreateAssessmentSectionAssemblyKernel();
                FailureMechanismAssemblyResult output = kernel.AssembleAssessmentSectionWbi2B1(
                    input.Select(AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult).ToArray(),
                    categories,
                    false);

                return FailureMechanismAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreatorOld.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismAssemblyCategoryGroup AssembleFailureMechanisms(IEnumerable<FailureMechanismAssemblyCategoryGroup> input)
        {
            try
            {
                IAssessmentGradeAssembler kernel = factory.CreateAssessmentSectionAssemblyKernel();
                EFailureMechanismCategory output = kernel.AssembleAssessmentSectionWbi2A1(
                    input.Select(AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult).ToArray(),
                    false);

                return FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup(output);
            }
            catch (AssemblyException e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreatorOld.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), e);
            }
        }

        public AssessmentSectionAssemblyCategoryGroup AssembleAssessmentSection(FailureMechanismAssemblyCategoryGroup failureMechanismsWithoutProbability,
                                                                                FailureMechanismAssembly failureMechanismsWithProbability)
        {
            try
            {
                IAssessmentGradeAssembler kernel = factory.CreateAssessmentSectionAssemblyKernel();
                EAssessmentGrade output = kernel.AssembleAssessmentSectionWbi2C1(
                    AssessmentSectionAssemblyInputCreator.CreateFailureMechanismCategory(failureMechanismsWithoutProbability),
                    AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult(failureMechanismsWithProbability));

                return AssemblyCategoryCreatorOld.CreateAssessmentSectionAssemblyCategory(output);
            }
            catch (AssemblyException e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreatorOld.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), e);
            }
        }

        public IEnumerable<CombinedFailureMechanismSectionAssemblyOld> AssembleCombinedFailureMechanismSections(
            IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSectionOld>> input, double assessmentSectionLength)
        {
            try
            {
                ICommonFailureMechanismSectionAssembler kernel = factory.CreateCombinedFailureMechanismSectionAssemblyKernel();
                AssemblyResult output = kernel.AssembleCommonFailureMechanismSections(FailureMechanismSectionListCreatorOld.Create(input), assessmentSectionLength, false);

                return CombinedFailureMechanismSectionAssemblyCreatorOld.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreatorOld.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new AssessmentSectionAssemblyCalculatorException(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), e);
            }
        }
    }
}