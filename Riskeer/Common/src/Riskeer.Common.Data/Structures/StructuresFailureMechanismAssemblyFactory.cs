// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.Structures
{
    /// <summary>
    /// Factory for assembling assembly results for a structures failure mechanism.
    /// </summary>
    public static class StructuresFailureMechanismAssemblyFactory
    {
        /// <summary>
        /// Assembles the section based on the input arguments.
        /// </summary>
        /// <typeparam name="TStructuresInput">The type of structures input contained by the calculation.</typeparam>
        /// <param name="sectionResult">The section result to assemble.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> the section result belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResultWrapper"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be assembled.</exception>
        public static FailureMechanismSectionAssemblyResultWrapper AssembleSection<TStructuresInput>(AdoptableFailureMechanismSectionResult sectionResult,
                                                                                                     ICalculatableFailureMechanism failureMechanism,
                                                                                                     IAssessmentSection assessmentSection)
            where TStructuresInput : IStructuresCalculationInput<StructureBase>, new()
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            StructuresCalculationScenario<TStructuresInput>[] calculationScenarios = failureMechanism.Calculations
                                                                                                     .OfType<StructuresCalculationScenario<TStructuresInput>>()
                                                                                                     .ToArray();

            var calculateStrategy = new StructuresFailureMechanismSectionResultCalculateProbabilityStrategy<TStructuresInput>(sectionResult, calculationScenarios);
            return FailureMechanismSectionAssemblyResultFactory.AssembleSection(sectionResult, assessmentSection, calculateStrategy);
        }
    }
}