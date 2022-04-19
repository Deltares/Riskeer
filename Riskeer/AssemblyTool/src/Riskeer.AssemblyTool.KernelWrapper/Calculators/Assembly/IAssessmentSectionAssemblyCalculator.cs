// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
    /// Interface representing an assessment section assembly calculator.
    /// </summary>
    public interface IAssessmentSectionAssemblyCalculator
    {
        /// <summary>
        /// Assembles an assessment section based on the input arguments.
        /// </summary>
        /// <param name="failureMechanismProbabilities">The collection of failure mechanism probabilities.</param>
        /// <param name="maximumAllowableFloodingProbability">The maximum allowable flooding probability to assemble with.</param>
        /// <param name="signalFloodingProbability">The signal flooding probability to assemble with.</param>
        /// <returns>An <see cref="AssessmentSectionAssemblyResultWrapper"/> representing the assembly result of the assessment section.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismProbabilities"/> is <c>null</c>.</exception>
        /// <exception cref="AssessmentSectionAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        AssessmentSectionAssemblyResultWrapper AssembleAssessmentSection(IEnumerable<double> failureMechanismProbabilities,
                                                                         double maximumAllowableFloodingProbability,
                                                                         double signalFloodingProbability);

        /// <summary>
        /// Assembles the combined assessment section for the given input.
        /// </summary>
        /// <param name="input">The collection of failure mechanism section collections to assemble for.</param>
        /// <param name="assessmentSectionLength">The length of the assessment section.</param>
        /// <returns>A collection of <see cref="CombinedFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <exception cref="AssessmentSectionAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        IEnumerable<CombinedFailureMechanismSectionAssembly> AssembleCombinedFailureMechanismSections(
            IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input, double assessmentSectionLength);
    }
}