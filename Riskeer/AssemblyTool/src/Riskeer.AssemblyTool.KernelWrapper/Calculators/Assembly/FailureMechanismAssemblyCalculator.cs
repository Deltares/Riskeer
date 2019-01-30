// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Assembly.Kernel.Model.CategoryLimits;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Class representing a failure mechanism assembly calculator.
    /// </summary>
    public class FailureMechanismAssemblyCalculator : IFailureMechanismAssemblyCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public FailureMechanismAssemblyCategoryGroup Assemble(IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> sectionCategories)
        {
            try
            {
                IFailureMechanismResultAssembler kernel = factory.CreateFailureMechanismAssemblyKernel();
                EFailureMechanismCategory output = kernel.AssembleFailureMechanismWbi1A1(
                    sectionCategories.Select(FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult).ToArray(),
                    false);

                return FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismAssembly Assemble(IEnumerable<FailureMechanismSectionAssembly> sectionAssemblies,
                                                 AssemblyCategoriesInput assemblyCategoriesInput)
        {
            try
            {
                ICategoryLimitsCalculator categoriesKernel = factory.CreateAssemblyCategoriesKernel();
                CategoriesList<FailureMechanismCategory> categories = categoriesKernel.CalculateFailureMechanismCategoryLimitsWbi11(
                    new AssessmentSection(1, assemblyCategoriesInput.SignalingNorm, assemblyCategoriesInput.LowerLimitNorm),
                    new FailureMechanism(assemblyCategoriesInput.N, assemblyCategoriesInput.FailureMechanismContribution));

                IFailureMechanismResultAssembler kernel = factory.CreateFailureMechanismAssemblyKernel();
                FailureMechanismAssemblyResult output = kernel.AssembleFailureMechanismWbi1B1(
                    new FailureMechanism(assemblyCategoriesInput.N, assemblyCategoriesInput.FailureMechanismContribution),
                    sectionAssemblies.Select(FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult).ToArray(),
                    categories,
                    false);

                return FailureMechanismAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }
    }
}