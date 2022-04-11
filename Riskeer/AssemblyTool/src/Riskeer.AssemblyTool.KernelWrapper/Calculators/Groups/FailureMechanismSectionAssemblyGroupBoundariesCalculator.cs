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
using System.Collections.Generic;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.Categories;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Groups
{
    /// <summary>
    /// Class representing a failure mechanism section assembly group boundaries calculator.
    /// </summary>
    public class FailureMechanismSectionAssemblyGroupBoundariesCalculator : IFailureMechanismSectionAssemblyGroupBoundariesCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyGroupBoundariesCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly groups kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
        public FailureMechanismSectionAssemblyGroupBoundariesCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> CalculateFailureMechanismSectionAssemblyGroupBoundaries(
            double signalFloodingProbability, double maximumAllowableFloodingProbability)
        {
            try
            {
                ICategoryLimitsCalculator kernel = factory.CreateAssemblyGroupsKernel();
                CategoriesList<InterpretationCategory> interpretationCategories = kernel.CalculateInterpretationCategoryLimitsBoi01(
                    new AssessmentSection(AssemblyCalculatorInputCreator.CreateProbability(signalFloodingProbability),
                                          AssemblyCalculatorInputCreator.CreateProbability(maximumAllowableFloodingProbability)));

                return FailureMechanismSectionAssemblyGroupCreator.CreateFailureMechanismSectionAssemblyGroupBoundaries(interpretationCategories);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyGroupBoundariesCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyGroupBoundariesCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }
    }
}