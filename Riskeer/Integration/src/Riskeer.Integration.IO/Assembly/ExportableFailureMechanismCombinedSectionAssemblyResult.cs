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

namespace Riskeer.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export a combined section assembly result of a failure mechanism.
    /// </summary>
    public class ExportableFailureMechanismCombinedSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismCombinedSectionAssemblyResult"/>
        /// </summary>
        /// <param name="sectionAssemblyResult">The assembly result of the combined section.</param>
        /// <param name="failureMechanismType">The type of the failure mechanism.</param>
        /// <param name="code">The code of the failure mechanism.</param>
        /// <param name="name">The name of the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionAssemblyResult"/> is <c>null</c>.</exception>
        public ExportableFailureMechanismCombinedSectionAssemblyResult(ExportableFailureMechanismSubSectionAssemblyResult sectionAssemblyResult,
                                                                       ExportableFailureMechanismType failureMechanismType,
                                                                       string code,
                                                                       string name)
        {
            if (sectionAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(sectionAssemblyResult));
            }

            SectionAssemblyResult = sectionAssemblyResult;
            FailureMechanismType = failureMechanismType;
            Code = code;
            Name = name;
        }

        /// <summary>
        /// Gets the assembly result of this combined section.
        /// </summary>
        public ExportableFailureMechanismSubSectionAssemblyResult SectionAssemblyResult { get; }

        /// <summary>
        /// Gets the failure mechanism type.
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