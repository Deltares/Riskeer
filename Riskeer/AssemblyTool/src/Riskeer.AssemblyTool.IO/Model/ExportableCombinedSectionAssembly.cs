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
using Riskeer.AssemblyTool.IO.Helpers;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class that holds all the information to export an combined section assembly result.
    /// </summary>
    public class ExportableCombinedSectionAssembly
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableCombinedSectionAssembly"/>.
        /// </summary>
        /// <param name="id">The id of the section assembly result.</param>
        /// <param name="section">The section that belongs to the assembly result.</param>
        /// <param name="assemblyGroup">The assembly group of this combined section.</param>
        /// <param name="assemblyGroupAssemblyMethod">The method used to assemble the assembly group for this combined section.</param>
        /// <param name="failureMechanismResults">The assembly results per failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> or
        /// <paramref name="failureMechanismResults"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public ExportableCombinedSectionAssembly(string id,
                                                 ExportableCombinedFailureMechanismSection section,
                                                 ExportableFailureMechanismSectionAssemblyGroup assemblyGroup,
                                                 ExportableAssemblyMethod assemblyGroupAssemblyMethod,
                                                 IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults)
        {
            IdValidationHelper.ThrowIfInvalid(id);

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (failureMechanismResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismResults));
            }

            Id = id;
            Section = section;
            AssemblyGroup = assemblyGroup;
            AssemblyGroupAssemblyMethod = assemblyGroupAssemblyMethod;
            FailureMechanismResults = failureMechanismResults;
        }

        /// <summary>
        /// Gets the id of the combined section assembly.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the section.
        /// </summary>
        public ExportableCombinedFailureMechanismSection Section { get; }

        /// <summary>
        /// Gets the assembly group.
        /// </summary>
        public ExportableFailureMechanismSectionAssemblyGroup AssemblyGroup { get; }

        /// <summary>
        /// Gets the method that was used to assemble the assembly group for this combined section.
        /// </summary>
        public ExportableAssemblyMethod AssemblyGroupAssemblyMethod { get; }

        /// <summary>
        /// Gets the assembly results per failure mechanism.
        /// </summary>
        public IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> FailureMechanismResults { get; }
    }
}