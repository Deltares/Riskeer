// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.Data.Output;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.Common.Data.AssemblyTool;

namespace Ringtoets.Common.Service.AssemblyTool
{
    /// <summary>
    /// Calculation service for calculating the assembly tool categories.
    /// </summary>
    public static class AssemblyToolCategoriesCalculationService
    {
        /// <summary>
        /// Calculates the assessment section assembly categories.
        /// </summary>
        /// <param name="input">The input to use in the calculation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="AssessmentSectionAssemblyCategory"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        public static IEnumerable<AssessmentSectionAssemblyCategory> CalculateAssessmentSectionAssemblyCategories(AssemblyCategoryInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            IAssemblyCategoriesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyCategoriesCalculator(
                AssemblyToolKernelWrapperFactory.Instance);

            IEnumerable<AssessmentSectionAssemblyCategoryResult> categories = calculator.CalculateAssessmentSectionCategories(
                AssemblyCategoryInputConverter.Convert(input));

            return AssemblyCategoryConverter.ConvertAssessmentSectionAssemblyCategories(categories);
        }
    }
}