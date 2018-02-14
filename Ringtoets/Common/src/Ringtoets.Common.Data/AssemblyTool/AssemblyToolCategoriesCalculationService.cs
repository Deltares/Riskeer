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

using System.Collections.Generic;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;

namespace Ringtoets.Common.Data.AssemblyTool
{
    /// <summary>
    /// Calculation service for calculating the assembly tool categories.
    /// </summary>
    public static class AssemblyToolCategoriesCalculationService
    {
        /// <summary>
        /// Calculates the assessment section assembly categories.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to use in the calculation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="AssessmentSectionAssemblyCategory"/>.</returns>
        public static IEnumerable<AssessmentSectionAssemblyCategory> CalculateAssessmentSectionAssemblyCategories(double signalingNorm, double lowerLimitNorm)
        { 
            IAssemblyCategoriesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyCategoriesCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);
            }
            catch (AssemblyCategoriesCalculatorException e)
            {
//                CalculationServiceHelper.LogExceptionAsError(Resources.AssemblyToolCategoriesCalculationService_CalculateAssessmentSectionAssemblyCategories_Error_in_assembly_categories_calculation, e);
                return new AssessmentSectionAssemblyCategory[0];
            }
        }
    }
}