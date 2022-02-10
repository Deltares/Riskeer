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
using System.Collections.Generic;
using System.Linq;
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.FailureMechanismSections;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
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
                FailureMechanismSectionWithCategory section = result.CombinedSectionResult.ElementAt(i);
                sectionAssemblies.Add(new CombinedFailureMechanismSectionAssembly(
                                          CreateSection(section),
                                          result.ResultPerFailureMechanism
                                                .Select(failureMechanismSectionList => failureMechanismSectionList.Sections.ElementAt(i))
                                                .Cast<FailureMechanismSectionWithCategory>()
                                                .Select(element => FailureMechanismSectionAssemblyGroupConverter.ConvertTo(element.Category))
                                                .ToArray()));
            }

            return sectionAssemblies;
        }

        private static CombinedAssemblyFailureMechanismSection CreateSection(FailureMechanismSectionWithCategory section)
        {
            return new CombinedAssemblyFailureMechanismSection(
                section.SectionStart, section.SectionEnd,
                FailureMechanismSectionAssemblyGroupConverter.ConvertTo(section.Category));
        }
    }
}