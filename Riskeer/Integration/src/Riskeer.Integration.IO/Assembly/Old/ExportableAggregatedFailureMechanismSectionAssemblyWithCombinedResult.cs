﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Integration.IO.Assembly.Old
{
    /// <summary>
    /// Class that holds all the information to export an assembly result with a combined assembly result
    /// of a failure mechanism section.
    /// </summary>
    public class ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult
        : ExportableAggregatedFailureMechanismSectionAssemblyResultBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The failure mechanism section.</param>
        /// <param name="combinedAssembly">The combined assembly result.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult(ExportableFailureMechanismSection failureMechanismSection,
                                                                                     ExportableSectionAssemblyResult combinedAssembly)
            : base(failureMechanismSection)
        {
            if (combinedAssembly == null)
            {
                throw new ArgumentNullException(nameof(combinedAssembly));
            }

            CombinedAssembly = combinedAssembly;
        }

        /// <summary>
        /// Gets the combined assembly result.
        /// </summary>
        public ExportableSectionAssemblyResult CombinedAssembly { get; }
    }
}