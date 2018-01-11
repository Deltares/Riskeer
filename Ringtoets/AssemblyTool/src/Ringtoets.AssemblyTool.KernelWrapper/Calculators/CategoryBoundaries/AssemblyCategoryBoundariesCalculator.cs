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
using Ringtoets.AssemblyTool.Data.Input;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.CategoryBoundaries
{
    /// <summary>
    /// Class representing an assembly category boundaries calculator.
    /// </summary>
    public class AssemblyCategoryBoundariesCalculator : IAssemblyCategoryBoundariesCalculator
    {
        private readonly AssemblyCategoryBoundariesCalculatorInput input;
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyCategoryBoundariesCalculator"/>.
        /// </summary>
        /// <param name="input">The <see cref="AssemblyCategoryBoundariesCalculatorInput"/> containing
        /// all the values required for performing the assembly category boundaries calculation.</param>
        /// <param name="factory">The factory responsible for creating the assembly category boundaries kernel.</param>
        public AssemblyCategoryBoundariesCalculator(AssemblyCategoryBoundariesCalculatorInput input, IAssemblyToolKernelFactory factory)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            this.input = input;
            this.factory = factory;
        }
    }
}