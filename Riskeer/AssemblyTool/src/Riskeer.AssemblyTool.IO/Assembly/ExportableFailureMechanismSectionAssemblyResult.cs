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

namespace Riskeer.AssemblyTool.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export an assembly result for a failure mechanism section.
    /// </summary>
    public class ExportableFailureMechanismSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The failure mechanism section.</param>
        /// <param name="assemblyGroup">The assembly group of this section.</param>
        /// <param name="assemblyGroupAssemblyMethod">The method used to assemble the assembly group for this section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSection"/> is <c>null</c>.</exception>
        public ExportableFailureMechanismSectionAssemblyResult(ExportableFailureMechanismSection failureMechanismSection,
                                                               FailureMechanismSectionAssemblyGroup assemblyGroup,
                                                               ExportableAssemblyMethod assemblyGroupAssemblyMethod)
        {
            if (failureMechanismSection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSection));
            }

            FailureMechanismSection = failureMechanismSection;
            AssemblyGroup = assemblyGroup;
            AssemblyGroupAssemblyMethod = assemblyGroupAssemblyMethod;
        }

        /// <summary>
        /// Gets the failure mechanism section.
        /// </summary>
        public ExportableFailureMechanismSection FailureMechanismSection { get; }

        /// <summary>
        /// Gets the assembly group.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup AssemblyGroup { get; }

        /// <summary>
        /// Gets the method that was used to assemble the assembly group for this section.
        /// </summary>
        public ExportableAssemblyMethod AssemblyGroupAssemblyMethod { get; }
    }
}