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
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class that holds all the information to export an assembly with probability result for a failure mechanism section.
    /// </summary>
    public class ExportableFailureMechanismSectionAssemblyWithProbabilityResult : ExportableFailureMechanismSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The failure mechanism section.</param>
        /// <param name="assemblyGroup">The assembly group of this section.</param>
        /// <param name="probability">The probability of this section.</param>
        /// <param name="assemblyGroupAssemblyMethod">The method used to assemble the assembly group for this section.</param>
        /// <param name="probabilityAssemblyMethod">The method used to assemble the probability for this section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSection"/> is <c>null</c>.</exception>
        public ExportableFailureMechanismSectionAssemblyWithProbabilityResult(ExportableFailureMechanismSection failureMechanismSection,
                                                                              FailureMechanismSectionAssemblyGroup assemblyGroup,
                                                                              double probability,
                                                                              ExportableAssemblyMethod assemblyGroupAssemblyMethod,
                                                                              ExportableAssemblyMethod probabilityAssemblyMethod)
            : base(failureMechanismSection, assemblyGroup, assemblyGroupAssemblyMethod)
        {
            Probability = probability;
            ProbabilityAssemblyMethod = probabilityAssemblyMethod;
        }

        /// <summary>
        /// Gets the probability of this section.
        /// </summary>
        public double Probability { get; }

        /// <summary>
        /// Gets the method used to assemble the probability for this section.
        /// </summary>
        public ExportableAssemblyMethod ProbabilityAssemblyMethod { get; }
    }
}