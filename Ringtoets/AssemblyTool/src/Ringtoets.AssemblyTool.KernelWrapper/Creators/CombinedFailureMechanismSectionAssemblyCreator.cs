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
using System.Linq;
using Assembly.Kernel.Model;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="CombinedFailureMechanismSectionAssembly"/> instances.
    /// </summary>
    internal static class CombinedFailureMechanismSectionAssemblyCreator
    {
        /// <summary>
        /// Creates a collection of <see cref="CombinedFailureMechanismSectionAssembly"/>
        /// based on the <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The <see cref="AssemblyResult"/> to create the
        /// <see cref="CombinedFailureMechanismSectionAssembly"/> for.</param>
        /// <returns>A collection of <see cref="CombinedFailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<CombinedFailureMechanismSectionAssembly> Create(AssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var sectionAssemblies = new List<CombinedFailureMechanismSectionAssembly>();

            for (var i = 0; i < result.CombinedSectionResult.Count(); i++)
            {
                FmSectionWithDirectCategory section = result.CombinedSectionResult.ElementAt(i);
                sectionAssemblies.Add(new CombinedFailureMechanismSectionAssembly(
                                          CreateSection(section),
                                          result.ResultPerFailureMechanism
                                                .Select(failureMechanismSectionList =>
                                                            (FmSectionWithDirectCategory) failureMechanismSectionList.Sections.ElementAt(i))
                                                .Select(element =>
                                                            FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(
                                                                element.Category)).ToArray()));
            }

            return sectionAssemblies;
        }

        private static CombinedAssemblyFailureMechanismSection CreateSection(FmSectionWithDirectCategory section)
        {
            return new CombinedAssemblyFailureMechanismSection(
                section.SectionStart, section.SectionEnd,
                FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(section.Category));
        }
    }
}