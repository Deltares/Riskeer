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

using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators
{
    /// <summary>
    /// Factory which creates calculators for performing assembly tool calculations.
    /// </summary>
    public class AssemblyToolCalculatorFactory : IAssemblyToolCalculatorFactory
    {
        private static IAssemblyToolCalculatorFactory instance;

        private AssemblyToolCalculatorFactory() {}

        /// <summary>
        /// Gets or sets an instance of <see cref="IAssemblyToolCalculatorFactoryOld"/>.
        /// </summary>
        public static IAssemblyToolCalculatorFactory Instance
        {
            get => instance ?? (instance = new AssemblyToolCalculatorFactory());
            internal set => instance = value;
        }

        public IAssemblyGroupBoundariesCalculator CreateAssemblyGroupBoundariesCalculator(IAssemblyToolKernelFactory factory)
        {
            return new AssemblyGroupBoundariesCalculator(factory);
        }

        public IFailureMechanismSectionAssemblyCalculator CreateFailureMechanismSectionAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            return new FailureMechanismSectionAssemblyCalculator(factory);
        }

        public IFailurePathAssemblyCalculator CreateFailureMechanismAssemblyCalculator(IAssemblyToolKernelFactory factory)
        {
            return new FailureMechanismAssemblyCalculator(factory);
        }
    }
}