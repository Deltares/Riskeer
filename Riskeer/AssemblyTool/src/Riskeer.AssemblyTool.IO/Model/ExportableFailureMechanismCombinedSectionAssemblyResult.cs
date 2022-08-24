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
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class that holds all the information to export a combined section assembly result of a failure mechanism.
    /// </summary>
    public class ExportableFailureMechanismCombinedSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismCombinedSectionAssemblyResult"/>
        /// </summary>
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        /// <param name="assemblyGroup">The assembly group of this section.</param>
        /// <param name="failureMechanismSectionResult">The associated failure mechanism section result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResult"/> is <c>null</c>.</exception>
        public ExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyGroup assemblyGroup,
                                                                       ExportableAssemblyMethod assemblyMethod,
                                                                       ExportableFailureMechanismSectionAssemblyResult failureMechanismSectionResult)
        {
            if (failureMechanismSectionResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResult));
            }

            AssemblyGroup = assemblyGroup;
            AssemblyMethod = assemblyMethod;
            FailureMechanismSectionResult = failureMechanismSectionResult;
        }

        /// <summary>
        /// Gets the assembly group of this section.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup AssemblyGroup { get; }
        
        /// <summary>
        /// Gets the assembly method that was used to assemble the assembly result.
        /// </summary>
        public ExportableAssemblyMethod AssemblyMethod { get; }
        
        /// <summary>
        /// Gets the associated failure mechanism section result.
        /// </summary>
        public ExportableFailureMechanismSectionAssemblyResult FailureMechanismSectionResult { get; }
    }
}