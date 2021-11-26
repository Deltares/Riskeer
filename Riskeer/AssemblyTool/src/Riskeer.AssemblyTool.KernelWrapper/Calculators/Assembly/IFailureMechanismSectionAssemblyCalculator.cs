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

using System;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Exceptions;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Interface representing a failure mechanism section assembly calculator.
    /// </summary>
    public interface IFailureMechanismSectionAssemblyCalculator
    {
        /// <summary>
        /// Assembles the failure mechanism section based on the input.
        /// </summary>
        /// <param name="lowerLimitNorm">The lower limit norm to assemble with.</param>
        /// <param name="signalingNorm">The signaling norm to assemble with.</param>
        /// <param name="input">The <see cref="FailureMechanismSectionAssemblyInput"/> to assemble with.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        FailureMechanismSectionAssembly AssembleFailureMechanismSection(double lowerLimitNorm,
                                                                        double signalingNorm,
                                                                        FailureMechanismSectionAssemblyInput input);
    }
}