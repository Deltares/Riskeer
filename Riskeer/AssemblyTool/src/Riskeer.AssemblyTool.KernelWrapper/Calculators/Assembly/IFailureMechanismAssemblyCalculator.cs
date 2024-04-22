// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Interface representing a failure mechanism assembly calculator.
    /// </summary>
    public interface IFailureMechanismAssemblyCalculator
    {
        /// <summary>
        /// Assembles a failure mechanism based on the worst section result.
        /// </summary>
        /// <param name="sectionAssemblyResults">A collection of <see cref="FailureMechanismSectionAssemblyResult"/>.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyResultWrapper"/> containing the assembly result of the failure mechanism.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionAssemblyResults"/> is <c>null</c>.</exception>
        /// <exception cref="FailureMechanismAssemblyCalculatorException">Thrown when an error occurs while assembling.</exception>
        FailureMechanismAssemblyResultWrapper AssembleWithWorstSectionResult(IEnumerable<FailureMechanismSectionAssemblyResult> sectionAssemblyResults);

        /// <summary>
        /// Assembles a failure mechanism based on the input.
        /// </summary>
        /// <param name="sectionAssemblyResults">A collection of <see cref="FailureMechanismSectionAssemblyResult"/>.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyResultWrapper"/> containing the assembly result of the failure mechanism.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionAssemblyResults"/> is <c>null</c>.</exception>
        /// <exception cref="FailureMechanismAssemblyCalculatorException">Thrown when an error occurs while assembling.</exception>
        FailureMechanismAssemblyResultWrapper Assemble(IEnumerable<FailureMechanismSectionAssemblyResult> sectionAssemblyResults);
    }
}