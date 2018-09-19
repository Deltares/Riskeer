// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export the assembly of a failure mechanism.
    /// </summary>
    /// <typeparam name="TFailureMechanismAssemblyResult">The type of <see cref="ExportableFailureMechanismAssemblyResult"/>.</typeparam>
    public class ExportableFailureMechanism<TFailureMechanismAssemblyResult>
        where TFailureMechanismAssemblyResult : ExportableFailureMechanismAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>.
        /// </summary>
        /// <param name="failureMechanismAssembly">The assembly result of the failure mechanism.</param>
        /// <param name="sectionAssemblyResults">The assembly results for the failure mechanism sections.</param>
        /// <param name="code">The code of the failure mechanism.</param>
        /// <param name="group">The group of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismAssembly"/>,
        /// or <paramref name="sectionAssemblyResults"/> is <c>null</c>.</exception>
        public ExportableFailureMechanism(TFailureMechanismAssemblyResult failureMechanismAssembly,
                                          IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase> sectionAssemblyResults,
                                          ExportableFailureMechanismType code,
                                          ExportableFailureMechanismGroup group)
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
            Code = code;
            Group = group;
        }

        /// <summary>
        /// Gets the assembly result of the failure mechanism.
        /// </summary>
        public TFailureMechanismAssemblyResult FailureMechanismAssembly { get; }

        /// <summary>
        /// Gets the collection of assembly results.
        /// </summary>
        public IEnumerable<ExportableAggregatedFailureMechanismSectionAssemblyResultBase> SectionAssemblyResults { get; }

        /// <summary>
        /// Gets the code of the failure mechanism.
        /// </summary>
        public ExportableFailureMechanismType Code { get; }

        /// <summary>
        /// Gets the group of the failure mechanism.
        /// </summary>
        public ExportableFailureMechanismGroup Group { get; }
    }
}