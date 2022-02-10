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

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Interface representing an assessment section assembly calculator.
    /// </summary>
    public interface IAssessmentSectionAssemblyCalculator
    {
        /// <summary>
        /// Assembles the combined assessment section for the given input.
        /// </summary>
        /// <param name="input">The collection of failure mechanism section collections to assemble for.</param>
        /// <param name="assessmentSectionLength">The length of the assessment section.</param>
        /// <returns>A collection of <see cref="CombinedFailureMechanismSectionAssemblyOld"/>.</returns>
        /// <exception cref="AssessmentSectionAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        IEnumerable<CombinedFailureMechanismSectionAssemblyOld> AssembleCombinedFailureMechanismSections(
            IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input, double assessmentSectionLength);
    }
}