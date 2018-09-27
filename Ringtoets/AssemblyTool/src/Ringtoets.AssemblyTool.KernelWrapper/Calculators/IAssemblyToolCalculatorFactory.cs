// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators
{
    /// <summary>
    /// Interface for a factory which creates calculators for performing assembly tool calculations.
    /// </summary>
    public interface IAssemblyToolCalculatorFactory
    {
        /// <summary>
        /// Creates an assembly categories calculator.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly categories kernel.</param>
        /// <returns>The assembly categories calculator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
        IAssemblyCategoriesCalculator CreateAssemblyCategoriesCalculator(IAssemblyToolKernelFactory factory);

        /// <summary>
        /// Creates a failure mechanism section assembly calculator.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <returns>The failure mechanism section assembly calculator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
        IFailureMechanismSectionAssemblyCalculator CreateFailureMechanismSectionAssemblyCalculator(IAssemblyToolKernelFactory factory);

        /// <summary>
        /// Creates a failure mechanism assembly calculator.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <returns>The failure mechanism assembly calculator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
        IFailureMechanismAssemblyCalculator CreateFailureMechanismAssemblyCalculator(IAssemblyToolKernelFactory factory);

        /// <summary>
        /// Creates an assessment section assembly calculator.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <returns>The assessment section assembly calculator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
        IAssessmentSectionAssemblyCalculator CreateAssessmentSectionAssemblyCalculator(IAssemblyToolKernelFactory factory);
    }
}