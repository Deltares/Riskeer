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
    /// Class that holds all the information to export the assembly result of a failure mechanism section.
    /// </summary>
    public class ExportableAggregatedFailureMechanismSectionAssemblyResult
        : ExportableAggregatedFailureMechanismSectionAssemblyResultBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableAggregatedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The failure mechanism section.</param>
        /// <param name="simpleAssembly">The simple assembly result of the failure mechanism section.</param>
        /// <param name="detailedAssembly">The detailed assembly result of the failure mechanism section.</param>
        /// <param name="tailorMadeAssembly">The tailor made assembly result of the failure mechanism section.</param>
        /// <param name="combinedAssembly">The combined assembly result of the failure mechanism section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ExportableAggregatedFailureMechanismSectionAssemblyResult(ExportableFailureMechanismSection failureMechanismSection,
                                                                         ExportableSectionAssemblyResult simpleAssembly,
                                                                         ExportableSectionAssemblyResult detailedAssembly,
                                                                         ExportableSectionAssemblyResult tailorMadeAssembly,
                                                                         ExportableSectionAssemblyResult combinedAssembly)
            : base(failureMechanismSection)
        {
            if (simpleAssembly == null)
            {
                throw new ArgumentNullException(nameof(simpleAssembly));
            }

            if (detailedAssembly == null)
            {
                throw new ArgumentNullException(nameof(detailedAssembly));
            }

            if (tailorMadeAssembly == null)
            {
                throw new ArgumentNullException(nameof(tailorMadeAssembly));
            }

            if (combinedAssembly == null)
            {
                throw new ArgumentNullException(nameof(combinedAssembly));
            }

            SimpleAssembly = simpleAssembly;
            DetailedAssembly = detailedAssembly;
            TailorMadeAssembly = tailorMadeAssembly;
            CombinedAssembly = combinedAssembly;
        }

        /// <summary>
        /// Gets the simple assembly result.
        /// </summary>
        public ExportableSectionAssemblyResult SimpleAssembly { get; }

        /// <summary>
        /// Gets the detailed assembly result.
        /// </summary>
        public ExportableSectionAssemblyResult DetailedAssembly { get; }

        /// <summary>
        /// Gets the tailor made assembly result.
        /// </summary>
        public ExportableSectionAssemblyResult TailorMadeAssembly { get; }

        /// <summary>
        /// Gets the combined assembly result.
        /// </summary>
        public ExportableSectionAssemblyResult CombinedAssembly { get; }
    }
}