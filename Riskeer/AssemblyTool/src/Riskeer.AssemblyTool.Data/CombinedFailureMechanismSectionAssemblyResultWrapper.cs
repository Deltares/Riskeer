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

namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// Wrapper class to link the used <see cref="AssemblyMethod"/> to the <see cref="CombinedFailureMechanismSectionAssembly"/>.
    /// </summary>
    public class CombinedFailureMechanismSectionAssemblyResultWrapper
    {
        /// <summary>
        /// Creates a new instance of <see cref="CombinedFailureMechanismSectionAssemblyResultWrapper"/>.
        /// </summary>
        /// <param name="assemblyResults">The assembly results to wrap.</param>
        /// <param name="commonSectionAssemblyMethod">The <see cref="AssemblyMethod"/> that is used to get the common sections.</param>
        /// <param name="failureMechanismResultsAssemblyMethod">The <see cref="AssemblyMethod"/> that is used to assemble the failure mechanism results.</param>
        /// <param name="combinedSectionResultAssemblyMethod">The <see cref="AssemblyMethod"/> that is used to assemble the combined section results.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assemblyResults"/> is <c>null</c>.</exception>
        public CombinedFailureMechanismSectionAssemblyResultWrapper(IEnumerable<CombinedFailureMechanismSectionAssembly> assemblyResults,
                                                                    AssemblyMethod commonSectionAssemblyMethod,
                                                                    AssemblyMethod failureMechanismResultsAssemblyMethod,
                                                                    AssemblyMethod combinedSectionResultAssemblyMethod)
        {
            if (assemblyResults == null)
            {
                throw new ArgumentNullException(nameof(assemblyResults));
            }

            AssemblyResults = assemblyResults;
            CommonSectionAssemblyMethod = commonSectionAssemblyMethod;
            FailureMechanismResultsAssemblyMethod = failureMechanismResultsAssemblyMethod;
            CombinedSectionResultAssemblyMethod = combinedSectionResultAssemblyMethod;
        }

        /// <summary>
        /// Gets the wrapped combined failure mechanism assembly results.
        /// </summary>
        public IEnumerable<CombinedFailureMechanismSectionAssembly> AssemblyResults { get; }
        
        /// <summary>
        /// Gets the <see cref="AssemblyMethod"/> that is used to get the common sections.
        /// </summary>
        public AssemblyMethod CommonSectionAssemblyMethod { get; }
        
        /// <summary>
        /// Gets the <see cref="AssemblyMethod"/> that is used to assemble the failure mechanism results.
        /// </summary>
        public AssemblyMethod FailureMechanismResultsAssemblyMethod { get; }
        
        /// <summary>
        /// Gets the <see cref="AssemblyMethod"/> that is used to assemble the combined section results.
        /// </summary>
        public AssemblyMethod CombinedSectionResultAssemblyMethod { get; }
    }
}