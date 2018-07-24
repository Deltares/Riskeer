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

using System.Collections.Generic;
using System.Linq;
using Assembly.Kernel.Model;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class that asserts combined failure mechanism section assemblies.
    /// </summary>
    public static class CombinedFailureMechanismSectionAssemblyAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="AssemblyResult"/>.</param>
        /// <param name="actual">The actual collection of <see cref="CombinedFailureMechanismSectionAssembly"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="original"/>.</exception>
        public static void AssertAssembly(AssemblyResult original, IEnumerable<CombinedFailureMechanismSectionAssembly> actual)
        {
            FmSectionWithDirectCategory[] combinedResults = original.CombinedSectionResult.ToArray();
            Assert.AreEqual(combinedResults.Length, actual.Count());
            for (var i = 0; i < combinedResults.Length; i++)
            {
                Assert.AreEqual(combinedResults[i].SectionStart, actual.ElementAt(i).Section.SectionStart);
                Assert.AreEqual(combinedResults[i].SectionEnd, actual.ElementAt(i).Section.SectionEnd);
                Assert.AreEqual(AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(combinedResults[i].Category), actual.ElementAt(i).Section.CategoryGroup);

                FailureMechanismSectionList[] failureMechanismResults = original.ResultPerFailureMechanism.ToArray();
                Assert.AreEqual(failureMechanismResults.Length, actual.ElementAt(i).FailureMechanismResults.Count());

                for (var j = 0; j < failureMechanismResults.Length; j++)
                {
                    FailureMechanismSectionAssemblyCategoryGroup expectedGroup = AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(
                        ((FmSectionWithDirectCategory) failureMechanismResults[j].Sections.ElementAt(i)).Category);
                    Assert.AreEqual(expectedGroup, actual.ElementAt(i).FailureMechanismResults.ElementAt(j));
                }
            }
        }
    }
}