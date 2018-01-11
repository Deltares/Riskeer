﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.CategoriesOutput;
using Ringtoets.AssemblyTool.Data.Input;
using Ringtoets.AssemblyTool.Data.Output;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels.CategoryBoundaries;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.CategoryBoundaries
{
    /// <summary>
    /// Class representing an assembly category boundaries calculator.
    /// </summary>
    public class AssemblyCategoryBoundariesCalculator : IAssemblyCategoryBoundariesCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyCategoryBoundariesCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly category boundaries kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssemblyCategoryBoundariesCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            this.factory = factory;
        }

        public AssemblyCategoryBoundariesResult<AssessmentSectionAssemblyCategoryResult> CalculateAssessmentSectionCategories(
            AssemblyCategoryBoundariesCalculatorInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            IAssemblyCategoryBoundariesKernel kernel = factory.CreateAssemblyCategoryBoundariesKernel();
            kernel.LowerBoundaryNorm = input.LowerBoundaryNorm;
            kernel.SignalingNorm = input.SignalingNorm;

            kernel.Calculate();

            CalculationOutput<AssessmentSectionCategoriesOutput[]> output = kernel.AssessmentSectionCategoriesOutput;

            return null;
        }
    }
}