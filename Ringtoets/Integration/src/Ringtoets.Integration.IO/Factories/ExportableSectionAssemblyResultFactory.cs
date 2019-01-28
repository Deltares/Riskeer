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
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableSectionAssemblyResult"/>.
    /// </summary>
    public static class ExportableSectionAssemblyResultFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ExportableSectionAssemblyResultWithProbability"/>
        /// based on the <paramref name="failureMechanismSectionAssembly"/>.
        /// </summary>
        /// <param name="failureMechanismSectionAssembly">The assembly result of this section.</param>
        /// <param name="assemblyMethod">The assembly method <see cref="ExportableAssemblyMethod"/>
        /// which was used to generate the result.</param>
        /// <returns>An <see cref="ExportableSectionAssemblyResultWithProbability"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionAssembly"/>
        /// is <c>null</c>.</exception>
        public static ExportableSectionAssemblyResult CreateExportableSectionAssemblyResult(
            FailureMechanismSectionAssemblyCategoryGroup failureMechanismSectionAssembly,
            ExportableAssemblyMethod assemblyMethod)
        {
            return new ExportableSectionAssemblyResult(assemblyMethod, failureMechanismSectionAssembly);
        }

        /// <summary>
        /// Creates an instance of <see cref="ExportableSectionAssemblyResultWithProbability"/>
        /// based on the <paramref name="failureMechanismSectionAssembly"/>.
        /// </summary>
        /// <param name="failureMechanismSectionAssembly">The assembly result of this section.</param>
        /// <param name="assemblyMethod">The assembly method <see cref="ExportableAssemblyMethod"/>
        /// which was used to generate the result.</param>
        /// <returns>An <see cref="ExportableSectionAssemblyResultWithProbability"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionAssembly"/>
        /// is <c>null</c>.</exception>
        public static ExportableSectionAssemblyResultWithProbability CreateExportableSectionAssemblyResultWithProbability(
            FailureMechanismSectionAssembly failureMechanismSectionAssembly,
            ExportableAssemblyMethod assemblyMethod)
        {
            if (failureMechanismSectionAssembly == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionAssembly));
            }

            return new ExportableSectionAssemblyResultWithProbability(assemblyMethod,
                                                                      failureMechanismSectionAssembly.Group,
                                                                      failureMechanismSectionAssembly.Probability);
        }
    }
}