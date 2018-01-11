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
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.CategoryBoundaries;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators
{
    /// <summary>
    /// Interface for a factory which creates calculators for performing assembly tool calculations.
    /// </summary>
    public interface IAssemblyToolCalculatorFactory
    {
        /// <summary>
        /// Creates an assembly category boundaries calculator.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly category boundaries kernel.</param>
        /// <returns>The assembly category boundaries calculator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is <c>null</c>.</exception>
        IAssemblyCategoryBoundariesCalculator CreateAssemblyCategoryBoundariesCalculator(IAssemblyToolKernelFactory factory);
    }
}