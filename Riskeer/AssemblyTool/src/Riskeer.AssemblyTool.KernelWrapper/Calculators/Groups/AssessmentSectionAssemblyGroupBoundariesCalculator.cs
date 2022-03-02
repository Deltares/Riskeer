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
    /// Class representing an assessment section assembly group boundaries calculator.
    /// </summary>
    public class AssessmentSectionAssemblyGroupBoundariesCalculator : IAssessmentSectionAssemblyGroupBoundariesCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyGroupBoundariesCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly groups kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssessmentSectionAssemblyGroupBoundariesCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public IEnumerable<AssessmentSectionAssemblyGroupBoundaries> CalculateAssessmentSectionAssemblyGroupBoundaries(double signalingNorm, double lowerLimitNorm)
        {
            try
            {
                ICategoryLimitsCalculator kernel = factory.CreateAssemblyGroupsKernel();
                CategoriesList<AssessmentSectionCategory> output = kernel.CalculateAssessmentSectionCategoryLimitsWbi21(
                    new AssessmentSection(AssemblyCalculatorInputCreator.CreateProbability(signalingNorm),
                                          AssemblyCalculatorInputCreator.CreateProbability(lowerLimitNorm)));

                return AssessmentSectionAssemblyGroupCreator.CreateAssessmentSectionAssemblyGroupBoundaries(output);
            }
            catch (AssemblyException e)
            {
                throw new AssessmentSectionAssemblyGroupBoundariesCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new AssessmentSectionAssemblyGroupBoundariesCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }
    }
}