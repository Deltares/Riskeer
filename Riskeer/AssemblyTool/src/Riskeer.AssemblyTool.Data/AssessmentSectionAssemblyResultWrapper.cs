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

namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// Wrapper class to link the used <see cref="AssemblyMethod"/> to the <see cref="AssessmentSectionAssemblyResult"/>.
    /// </summary>
    public class AssessmentSectionAssemblyResultWrapper
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyResultWrapper"/>.
        /// </summary>
        /// <param name="assemblyResult">The <see cref="AssessmentSectionAssemblyResult"/> to wrap.</param>
        /// <param name="probabilityMethod">The <see cref="AssemblyMethod"/> that is used to assemble the probabilities.</param>
        /// <param name="assemblyGroupMethod">The <see cref="AssemblyMethod"/> that is used to assemble the assembly group.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assemblyResult"/> is <c>null</c>.</exception>
        public AssessmentSectionAssemblyResultWrapper(AssessmentSectionAssemblyResult assemblyResult,
                                                      AssemblyMethod probabilityMethod, AssemblyMethod assemblyGroupMethod)
        {
            if (assemblyResult == null)
            {
                throw new ArgumentNullException(nameof(assemblyResult));
            }

            AssemblyResult = assemblyResult;
            ProbabilityMethod = probabilityMethod;
            AssemblyGroupMethod = assemblyGroupMethod;
        }

        /// <summary>
        /// Gets the wrapped <see cref="AssessmentSectionAssemblyResult"/>.
        /// </summary>
        public AssessmentSectionAssemblyResult AssemblyResult { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyMethod"/> that is used to assemble the probability.
        /// </summary>
        public AssemblyMethod ProbabilityMethod { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyMethod"/> that is used to assemble the assembly group.
        /// </summary>
        public AssemblyMethod AssemblyGroupMethod { get; }
    }
}