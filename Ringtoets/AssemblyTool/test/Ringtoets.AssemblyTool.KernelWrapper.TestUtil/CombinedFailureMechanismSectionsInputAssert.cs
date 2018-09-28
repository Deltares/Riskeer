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
using System.Collections.Generic;
using System.Linq;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FmSectionTypes;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class for asserting combined failure mechanism input.
    /// </summary>
    public static class CombinedFailureMechanismSectionsInputAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original collection of <see cref="CombinedAssemblyFailureMechanismSection"/>
        /// collections.</param>
        /// <param name="actual">The actual collection of <see cref="FailureMechanismSectionList"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="original"/>.</exception>
        public static void AssertCombinedFailureMechanismInput(IEnumerable<CombinedAssemblyFailureMechanismSection[]> original, IEnumerable<FailureMechanismSectionList> actual)
        {
            Assert.AreEqual(original.Count(), actual.Count());

            for (var i = 0; i < original.Count(); i++)
            {
                CombinedAssemblyFailureMechanismSection[] sections = original.ElementAt(i);
                FailureMechanismSectionList sectionList = actual.ElementAt(i);
                Assert.IsEmpty(sectionList.FailureMechanismId);
                AssertSections(sections, sectionList.Sections);
            }
        }

        private static void AssertSections(IEnumerable<CombinedAssemblyFailureMechanismSection> originalSections, IEnumerable<FailureMechanismSection> failureMechanismSections)
        {
            Assert.AreEqual(originalSections.Count(), failureMechanismSections.Count());
            Assert.IsTrue(failureMechanismSections.All(r => r.GetType() == typeof(FmSectionWithDirectCategory)));
            CollectionAssert.AreEqual(originalSections.Select(s => s.SectionStart), failureMechanismSections.Select(r => r.SectionStart));
            CollectionAssert.AreEqual(originalSections.Select(s => s.SectionEnd), failureMechanismSections.Select(r => r.SectionEnd));
            CollectionAssert.AreEqual(originalSections.Select(s => ConvertCategoryGroup(s.CategoryGroup)),
                                      failureMechanismSections.Select(r => (FmSectionWithDirectCategory) r)
                                                              .Select(category => category.Category));
        }

        private static EFmSectionCategory ConvertCategoryGroup(FailureMechanismSectionAssemblyCategoryGroup categoryGroup)
        {
            switch (categoryGroup)
            {
                case FailureMechanismSectionAssemblyCategoryGroup.None:
                    return EFmSectionCategory.Gr;
                case FailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return EFmSectionCategory.NotApplicable;
                case FailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return EFmSectionCategory.Iv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return EFmSectionCategory.IIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIIv:
                    return EFmSectionCategory.IIIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IVv:
                    return EFmSectionCategory.IVv;
                case FailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return EFmSectionCategory.Vv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIv:
                    return EFmSectionCategory.VIv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return EFmSectionCategory.VIIv;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}