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

using System.Collections.Generic;
using System.Linq;
using Assembly.Kernel.Model.FailureMechanismSections;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class that asserts combined failure mechanism section assemblies.
    /// </summary>
    public static class CombinedFailureMechanismSectionAssemblyAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to
        /// <paramref name="failureMechanismResults"/> and <paramref name="combinedResults"/>.
        /// </summary>
        /// <param name="failureMechanismResults">The original <see cref="IEnumerable{T}"/> of <see cref="FailureMechanismSectionList"/>.</param>
        /// <param name="combinedResults">The original <see cref="IEnumerable{T}"/> of <see cref="FailureMechanismSectionWithCategory"/>.</param>
        /// <param name="actual">The actual collection of <see cref="CombinedFailureMechanismSectionAssembly"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/> is not equal to
        /// <paramref name="failureMechanismResults"/> and <paramref name="combinedResults"/>.</exception>
        public static void AssertAssembly(IEnumerable<FailureMechanismSectionList> failureMechanismResults,
                                          IEnumerable<FailureMechanismSectionWithCategory> combinedResults,
                                          IEnumerable<CombinedFailureMechanismSectionAssembly> actual)
        {
            Assert.AreEqual(combinedResults.Count(), actual.Count());
            for (var i = 0; i < combinedResults.Count(); i++)
            {
                FailureMechanismSectionWithCategory combinedResult = combinedResults.ElementAt(i);
                CombinedFailureMechanismSectionAssembly actualCombinedFailureMechanismSectionAssembly = actual.ElementAt(i);

                Assert.AreEqual(combinedResult.Start, actualCombinedFailureMechanismSectionAssembly.Section.SectionStart);
                Assert.AreEqual(combinedResult.End, actualCombinedFailureMechanismSectionAssembly.Section.SectionEnd);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(combinedResult.Category),
                                actualCombinedFailureMechanismSectionAssembly.Section.FailureMechanismSectionAssemblyGroup);

                Assert.AreEqual(failureMechanismResults.Count(), actualCombinedFailureMechanismSectionAssembly.FailureMechanismSectionAssemblyGroupResults.Count());

                for (var j = 0; j < failureMechanismResults.Count(); j++)
                {
                    FailureMechanismSectionAssemblyGroup expectedGroup = FailureMechanismSectionAssemblyGroupConverter.ConvertTo(
                        ((FailureMechanismSectionWithCategory) failureMechanismResults.ElementAt(j).Sections.ElementAt(i)).Category);
                    Assert.AreEqual(expectedGroup, actualCombinedFailureMechanismSectionAssembly.FailureMechanismSectionAssemblyGroupResults.ElementAt(j));
                }
            }
        }
    }
}