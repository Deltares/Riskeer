// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export an combined section assembly result.
    /// </summary>
    public class ExportableCombinedSectionAssembly
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableCombinedSectionAssembly"/>.
        /// </summary>
        /// <param name="section">The section that belongs to the assembly result.</param>
        /// <param name="combinedSectionAssemblyResult">The combined assembly result of this section.</param>
        /// <param name="failureMechanismResults">The assembly results per failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ExportableCombinedSectionAssembly(ExportableCombinedFailureMechanismSection section,
                                                 ExportableSectionAssemblyResult combinedSectionAssemblyResult,
                                                 IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismResults)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (combinedSectionAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionAssemblyResult));
            }

            if (failureMechanismResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismResults));
            }

            Section = section;
            CombinedSectionAssemblyResult = combinedSectionAssemblyResult;
            FailureMechanismResults = failureMechanismResults;
        }

        /// <summary>
        /// Gets the section of the assembly.
        /// </summary>
        public ExportableCombinedFailureMechanismSection Section { get; }

        /// <summary>
        /// Gets the combined assembly result of this section.
        /// </summary>
        public ExportableSectionAssemblyResult CombinedSectionAssemblyResult { get; }

        /// <summary>
        /// Gets the assembly results per failure mechanism.
        /// </summary>
        public IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> FailureMechanismResults { get; }
    }
}