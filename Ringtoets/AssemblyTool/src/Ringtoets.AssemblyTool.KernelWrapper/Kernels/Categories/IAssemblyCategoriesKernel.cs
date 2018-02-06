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

using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;

namespace Ringtoets.AssemblyTool.KernelWrapper.Kernels.Categories
{
    /// <summary>
    /// Interface representing assembly categories kernel input, methods and output.
    /// </summary>
    /// <remarks>
    /// This interface is introduced for being able to test the conversion of:
    /// <list type="bullet">
    /// <item><see cref="AssemblyCategoriesCalculator"/> input into kernel input;</item>
    /// <item>kernel output into <see cref="AssemblyCategoriesCalculator"/> output.</item>
    /// </list>
    /// </remarks>
    public interface IAssemblyCategoriesKernel
    {
        /// <summary>
        /// Performs the calculation.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerBoundaryNorm">The lower boundary norm to use in the calculation.</param>
        /// <returns>The calculation output.</returns>
        /// <exception cref="AssemblyCategoriesKernelWrapperException">Thrown when an error occurs
        /// when performing the calculation.</exception>
        CalculationOutput<AssessmentSectionCategory[]> Calculate(double signalingNorm, double lowerBoundaryNorm);
    }
}