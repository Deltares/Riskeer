// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
    /// Assembly for the combined failure mechanism section.
    /// </summary>
    public class CombinedFailureMechanismSectionAssembly
    {
        /// <summary>
        /// Creates a new instance of <see cref="CombinedFailureMechanismSectionAssembly"/>.
        /// </summary>
        /// <param name="section">The section of the assembly.</param>
        /// <param name="failureMechanismResults">The assembly results per failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public CombinedFailureMechanismSectionAssembly(CombinedAssemblyFailureMechanismSection section,
                                                       IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismResults)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (failureMechanismResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismResults));
            }

            Section = section;
            FailureMechanismResults = failureMechanismResults;
        }

        /// <summary>
        /// Gets the section of the assembly.
        /// </summary>
        public CombinedAssemblyFailureMechanismSection Section { get; }

        /// <summary>
        /// Gets the assembly results per failure mechanism.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> FailureMechanismResults { get; }
    }
}