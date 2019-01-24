// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
        /// Assembles the failure mechanisms for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The collection of failure mechanism assemblies to assemble for.</param>
        /// <param name="signalingNorm">The signaling norm to calculate with.</param>
        /// <param name="lowerLimitNorm">The lower limit norm to calculate with.</param>
        /// <param name="failureProbabilityMarginFactor">The failure probability margin factor to
        /// calculate with.</param>
        /// <returns>A <see cref="FailureMechanismAssembly"/>.</returns>
        /// <exception cref="AssessmentSectionAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        FailureMechanismAssembly AssembleFailureMechanisms(IEnumerable<FailureMechanismAssembly> input,
                                                           double signalingNorm,
                                                           double lowerLimitNorm,
                                                           double failureProbabilityMarginFactor);

        /// <summary>
        /// Assembles the failure mechanisms for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The collection of failure mechanism assembly category groups
        /// to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="AssessmentSectionAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        FailureMechanismAssemblyCategoryGroup AssembleFailureMechanisms(IEnumerable<FailureMechanismAssemblyCategoryGroup> input);

        /// <summary>
        /// Assembles the assessment section for the given inputs.
        /// </summary>
        /// <param name="failureMechanismsWithoutProbability">The assembly result for 
        /// failure mechanisms without probability to assemble for.</param>
        /// <param name="failureMechanismsWithProbability">The assembly result for 
        /// failure mechanisms with probability to assemble for.</param>
        /// <returns>An <see cref="AssessmentSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="AssessmentSectionAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        AssessmentSectionAssemblyCategoryGroup AssembleAssessmentSection(FailureMechanismAssemblyCategoryGroup failureMechanismsWithoutProbability,
                                                                         FailureMechanismAssembly failureMechanismsWithProbability);

        /// <summary>
        /// Assembles the combined assessment section for the given input.
        /// </summary>
        /// <param name="input">The collection of failure mechanism section collections to assemble for.</param>
        /// <param name="assessmentSectionLength">The length of the assessment section.</param>
        /// <returns>A collection of <see cref="CombinedFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="AssessmentSectionAssemblyCalculatorException">Thrown when
        /// an error occurs while assembling.</exception>
        IEnumerable<CombinedFailureMechanismSectionAssembly> AssembleCombinedFailureMechanismSections(IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input,
                                                                                                      double assessmentSectionLength);
    }
}