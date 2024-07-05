// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// Class that contains helper methods for performing assembly routines.
    /// </summary>
    public static class AssemblyToolHelper
    {
        /// <summary>
        /// Assembles the failure mechanism section.
        /// </summary>
        /// <param name="sectionResult">The section result to assemble.</param>
        /// <param name="performSectionAssemblyFunc">The <see cref="Func{T1,TResult}"/>to perform the assembly.</param>
        /// <typeparam name="TSectionResult">The type of section result.</typeparam>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <remarks>When the failure mechanism section cannot be assembled,
        /// a <see cref="DefaultFailureMechanismSectionAssemblyResult"/> is created.</remarks>
        public static FailureMechanismSectionAssemblyResult AssembleFailureMechanismSection<TSectionResult>(
            TSectionResult sectionResult, Func<TSectionResult, FailureMechanismSectionAssemblyResultWrapper> performSectionAssemblyFunc)
            where TSectionResult : FailureMechanismSectionResult
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (performSectionAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(performSectionAssemblyFunc));
            }

            try
            {
                return performSectionAssemblyFunc(sectionResult).AssemblyResult;
            }
            catch (AssemblyException)
            {
                return new DefaultFailureMechanismSectionAssemblyResult();
            }
        }
    }
}