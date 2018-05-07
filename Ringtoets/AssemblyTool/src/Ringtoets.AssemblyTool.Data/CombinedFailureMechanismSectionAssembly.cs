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

namespace Ringtoets.AssemblyTool.Data
{
    /// <summary>
    /// Assembly for the combined failure mechanism section.
    /// </summary>
    public class CombinedFailureMechanismSectionAssembly
    {
        /// <summary>
        /// Creates a new instance of <see cref="CombinedFailureMechanismSectionAssembly"/>.
        /// </summary>
        /// <param name="sectionStart">The start of the section from the beginning of the reference line.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of the reference line.</param>
        /// <param name="combinedResult">The combined assembly result.</param>
        /// <param name="failureMechanismResults">The assembly results per failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismResults"/>
        /// is <c>null</c>.</exception>
        public CombinedFailureMechanismSectionAssembly(double sectionStart, double sectionEnd,
                                                       FailureMechanismSectionAssemblyCategoryGroup combinedResult,
                                                       IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismResults)
        {
            if (failureMechanismResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismResults));
            }

            SectionStart = sectionStart;
            SectionEnd = sectionEnd;
            CombinedResult = combinedResult;
            FailureMechanismResults = failureMechanismResults;
        }

        /// <summary>
        /// Gets the start of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public double SectionStart { get; }

        /// <summary>
        /// Gets the end of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public double SectionEnd { get; }

        /// <summary>
        /// Gets the combined assembly result.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup CombinedResult { get; }

        /// <summary>
        /// Gets the assembly results per failure mechanism.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyCategoryGroup> FailureMechanismResults { get; }
    }
}
