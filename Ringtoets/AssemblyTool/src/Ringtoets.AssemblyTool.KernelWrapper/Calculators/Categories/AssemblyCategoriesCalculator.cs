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
using System.Collections.Generic;
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Categories;
using AssemblyTool.Kernel.Categories.CalculatorInput;
using AssemblyTool.Kernel.Data;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.Common.Data.AssemblyTool;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories
{
    /// <summary>
    /// Class representing an assembly categories calculator.
    /// </summary>
    public class AssemblyCategoriesCalculator : IAssemblyCategoriesCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyCategoriesCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly categories kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public AssemblyCategoriesCalculator(IAssemblyToolKernelFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        public IEnumerable<AssessmentSectionAssemblyCategory> CalculateAssessmentSectionCategories(double signalingNorm, double lowerLimitNorm)
        {
            try
            {
                var input = new CalculateAssessmentSectionCategoriesInput(new Probability(signalingNorm), new Probability(lowerLimitNorm));
                ICategoriesCalculator kernel = factory.CreateAssemblyCategoriesKernel();
                CalculationOutput<AssessmentSectionCategory[]> output = kernel.CalculateAssessmentSectionCategories(input);

                return AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategories(output);
            }
            catch (Exception e)
            {
                throw new AssemblyCategoriesCalculatorException(e.Message, e);
            }
        }

        public IEnumerable<FailureMechanismSectionAssemblyCategory> CalculateFailureMechanismSectionCategories(double signalingNorm,
                                                                                                               double lowerLimitNorm,
                                                                                                               double probabilityDistributionFactor,
                                                                                                               double n)
        {
            try
            {
                var input = new CalculateFailureMechanismSectionCategoriesInput(new Probability(signalingNorm), new Probability(lowerLimitNorm),
                                                                                probabilityDistributionFactor, n);
                ICategoriesCalculator kernel = factory.CreateAssemblyCategoriesKernel();
                CalculationOutput<FailureMechanismSectionCategory[]> output = kernel.CalculateFailureMechanismSectionCategories(input);

                return AssemblyCategoryCreator.CreateFailureMechanismSectionAssemblyCategories(output);
            }
            catch (Exception e)
            {
                throw new AssemblyCategoriesCalculatorException(e.Message, e);
            }
        }

        public IEnumerable<FailureMechanismSectionAssemblyCategory> CalculateGeotechnicFailureMechanismSectionCategories(double signalingNorm,
                                                                                                                         double lowerLimitNorm,
                                                                                                                         double probabilityDistributionFactor,
                                                                                                                         double n)
        {
            try
            {
                var input = new CalculateFailureMechanismSectionCategoriesInput(new Probability(signalingNorm), new Probability(lowerLimitNorm),
                                                                                probabilityDistributionFactor, n);
                ICategoriesCalculator kernel = factory.CreateAssemblyCategoriesKernel();
                CalculationOutput<FailureMechanismSectionCategory[]> output = kernel.CalculateGeotechnicFailureMechanismSectionCategories(input);

                return AssemblyCategoryCreator.CreateFailureMechanismSectionAssemblyCategories(output);
            }
            catch (Exception e)
            {
                throw new AssemblyCategoriesCalculatorException(e.Message, e);
            }
        }
    }
}