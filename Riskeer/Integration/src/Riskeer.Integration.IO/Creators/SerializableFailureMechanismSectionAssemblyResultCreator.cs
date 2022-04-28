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
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Exceptions;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="SerializableFailureMechanismSectionAssemblyResult"/>
    /// </summary>
    public static class SerializableFailureMechanismSectionAssemblyResultCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="SerializableFailureMechanismSectionAssemblyResult"/>
        /// based on its input parameters.
        /// </summary>
        /// <param name="sectionResult">The <see cref="ExportableFailureMechanismSectionAssemblyWithProbabilityResult"/> to create a
        /// <see cref="SerializableFailureMechanismSectionAssemblyResult"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="sectionResult"/> is invalid to
        /// create a serializable counterpart for.</exception>
        public static SerializableFailureMechanismSectionAssemblyResult Create(ExportableFailureMechanismSectionAssemblyWithProbabilityResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            ValidateAssemblyResult(sectionResult);

            return new SerializableFailureMechanismSectionAssemblyResult(
                SerializableAssemblyMethodCreator.Create(sectionResult.ProbabilityAssemblyMethod),
                SerializableAssemblyMethodCreator.Create(sectionResult.AssemblyGroupAssemblyMethod),
                SerializableFailureMechanismSectionAssemblyGroupCreator.Create(sectionResult.AssemblyGroup),
                sectionResult.Probability);
        }

        private static void ValidateAssemblyResult(ExportableFailureMechanismSectionAssemblyResult sectionResult)
        {
            if (sectionResult.AssemblyGroup == FailureMechanismSectionAssemblyGroup.NoResult
                || sectionResult.AssemblyGroup == FailureMechanismSectionAssemblyGroup.Dominant)
            {
                throw new AssemblyCreatorException(@"The assembly result is invalid and cannot be created.");
            }
        }
    }
}