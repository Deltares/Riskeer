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

using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Interface representing a failure mechanism section assembly calculator.
    /// </summary>
    /// <remarks>
    /// This interface is introduced for being able to test the conversion of:
    /// <list type="bullet">
    /// <item>Ringtoets failure mechanism section assembly input into kernel input;</item>
    /// <item>kernel output into Ringtoets failure mechanism section assembly output.</item>
    /// </list>
    /// </remarks>
    public interface IFailureMechanismSectionAssemblyCalculator
    {
        /// <summary>
        /// Assembles the simple assessment for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultType"/> to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultType input);

        /// <summary>
        /// Assembles the simple assessment for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultValidityOnlyType"/> to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType input);
    }
}