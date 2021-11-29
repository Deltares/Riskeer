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

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories
{
    /// <summary>
    /// Interface representing an assembly group limits calculator.
    /// </summary>
    public interface IAssemblyGroupLimitsCalculator
    {
        /// <summary>
        /// Performs the calculation to get a collection of assembly group limits for a failure mechanism section,
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to calculate with.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to calculate with.</param>
        /// <returns>A collection of <see cref="FailureMechanismSectionAssemblyGroupLimits"/>.</returns>
        /// <exception cref="AssemblyGroupLimitsCalculatorException">Thrown when an error occurs
        /// while performing the calculation.</exception>
        IEnumerable<FailureMechanismSectionAssemblyGroupLimits> CalculateFailureMechanismSectionAssemblyGroupLimits(
            double signalingNorm, double lowerLimitNorm);
    }
}