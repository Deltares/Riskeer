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

namespace Riskeer.AssemblyTool.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export the assembly of a failure mechanism.
    /// </summary>
    public class ExportableFailureMechanism
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanismAssembly">The assembly result of the failure mechanism.</param>
        /// <param name="sectionAssemblyResults">The assembly results for the failure mechanism sections.</param>
        /// <param name="failureMechanismType">The type of the failure mechanism.</param>
        /// <param name="code">The code of the failure mechanism.</param>
        /// <param name="name">The name of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismAssembly"/>
        /// or <paramref name="sectionAssemblyResults"/> is <c>null</c>.</exception>
        public ExportableFailureMechanism(ExportableFailureMechanismAssemblyResult failureMechanismAssembly,
                                          IEnumerable<ExportableFailureMechanismSectionAssemblyWithProbabilityResult> sectionAssemblyResults,
                                          ExportableFailureMechanismType failureMechanismType, string code, string name)
        {
            if (failureMechanismAssembly == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismAssembly));
            }

            if (sectionAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(sectionAssemblyResults));
            }

            FailureMechanismAssembly = failureMechanismAssembly;
            SectionAssemblyResults = sectionAssemblyResults;
            FailureMechanismType = failureMechanismType;
            Code = code;
            Name = name;
        }

        /// <summary>
        /// Gets the assembly result of the failure mechanism.
        /// </summary>
        public ExportableFailureMechanismAssemblyResult FailureMechanismAssembly { get; }

        /// <summary>
        /// Gets the collection of assembly results.
        /// </summary>
        public IEnumerable<ExportableFailureMechanismSectionAssemblyWithProbabilityResult> SectionAssemblyResults { get; }

        /// <summary>
        /// Gets the type of the failure mechanism.
        /// </summary>
        public ExportableFailureMechanismType FailureMechanismType { get; }

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the name of the failure mechanism.
        /// </summary>
        public string Name { get; }
    }
}