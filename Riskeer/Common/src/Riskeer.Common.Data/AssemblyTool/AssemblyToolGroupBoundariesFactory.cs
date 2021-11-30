// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.Generic;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.Common.Data.Exceptions;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// Factory for calculating the assembly tool group boundaries.
    /// </summary>
    public static class AssemblyToolGroupBoundariesFactory
    {
        /// <summary>
        /// Creates the failure mechanism section assembly group boundaries.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to use in the calculation.</param>
        /// <returns>A collection of <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when an error occurred while creating the assembly group boundaries.</exception>
        public static IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> CreateFailureMechanismSectionAssemblyGroupBoundaries(
            double signalingNorm, double lowerLimitNorm)
        {
            IAssemblyGroupBoundariesCalculator calculator = AssemblyToolCalculatorFactory.Instance.CreateAssemblyGroupBoundariesCalculator(
                AssemblyToolKernelFactory.Instance);

            try
            {
                return calculator.CalculateFailureMechanismSectionAssemblyGroupBoundaries(signalingNorm, lowerLimitNorm);
            }
            catch (AssemblyCategoriesCalculatorException e)
            {
                throw new AssemblyException(e.Message, e);
            }
        }
    }
}