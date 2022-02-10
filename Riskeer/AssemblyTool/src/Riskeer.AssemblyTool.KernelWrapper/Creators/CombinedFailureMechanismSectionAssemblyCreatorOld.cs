// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Assembly.Kernel.Old.Model;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="CombinedFailureMechanismSectionAssemblyOld"/> instances.
    /// </summary>
    internal static class CombinedFailureMechanismSectionAssemblyCreatorOld
    {
        /// <summary>
        /// Creates a collection of <see cref="CombinedFailureMechanismSectionAssemblyOld"/>
        /// based on the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The <see cref="AssemblyResult"/> to create the
        /// <see cref="CombinedFailureMechanismSectionAssemblyOld"/> for.</param>
        /// <returns>A collection of <see cref="CombinedFailureMechanismSectionAssemblyOld"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<CombinedFailureMechanismSectionAssemblyOld> Create(AssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var sectionAssemblies = new List<CombinedFailureMechanismSectionAssemblyOld>();

            for (var i = 0; i < result.CombinedSectionResult.Count(); i++)
            {
                FmSectionWithDirectCategory section = result.CombinedSectionResult.ElementAt(i);
                sectionAssemblies.Add(new CombinedFailureMechanismSectionAssemblyOld(
                                          CreateSection(section),
                                          result.ResultPerFailureMechanism
                                                .Select(failureMechanismSectionList =>
                                                            (FmSectionWithDirectCategory) failureMechanismSectionList.Sections.ElementAt(i))
                                                .Select(element =>
                                                            FailureMechanismSectionAssemblyCreatorOld.CreateFailureMechanismSectionAssemblyCategoryGroup(
                                                                element.Category)).ToArray()));
            }

            return sectionAssemblies;
        }

        private static CombinedAssemblyFailureMechanismSectionOld CreateSection(FmSectionWithDirectCategory section)
        {
            return new CombinedAssemblyFailureMechanismSectionOld(
                section.SectionStart, section.SectionEnd,
                FailureMechanismSectionAssemblyCreatorOld.CreateFailureMechanismSectionAssemblyCategoryGroup(section.Category));
        }
    }
}