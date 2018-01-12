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
using Ringtoets.AssemblyTool.Data.Input;
using Ringtoets.AssemblyTool.Data.Output;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories
{
    /// <summary>
    /// Interface representing an assembly categories calculator.
    /// </summary>
    /// <remarks>
    /// This interface is introduced for being able to test the conversion of:
    /// <list type="bullet">
    /// <item>Ringtoets assembly categories input into calculator input;</item>
    /// <item>calculator output into Ringtoets assembly categories output.</item>
    /// </list>
    /// </remarks>
    public interface IAssemblyCategoriesCalculator
    {
        /// <summary>
        /// Performs the calculation for getting the assessment section categories.
        /// </summary>
        /// <param name="input">The <see cref="AssemblyCategoriesCalculatorInput"/> containing
        /// all the values required for performing the assembly categories calculation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with categories of
        /// <see cref="AssessmentSectionAssemblyCategoryResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        IEnumerable<AssessmentSectionAssemblyCategoryResult> CalculateAssessmentSectionCategories(
            AssemblyCategoriesCalculatorInput input);
    }
}