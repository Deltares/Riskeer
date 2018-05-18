﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        public static void AssertCombinedFailureMechanismInput(CombinedAssemblyFailureMechanismSection[][] original, FailureMechanismSectionList[] actual)
        {
            Assert.AreEqual(original.Length, actual.Length);

            for (var i = 0; i < original.Length; i++)
            {
                CombinedAssemblyFailureMechanismSection[] sections = original[i];
                FailureMechanismSectionList sectionList = actual[i];
                AssertSections(sections.ToArray(), sectionList.Results.ToArray());
            }
        }

        private static void AssertSections(CombinedAssemblyFailureMechanismSection[] originalSections, FmSectionWithCategory[] fmSectionWithCategories)
        {
            Assert.AreEqual(originalSections.Length, fmSectionWithCategories.Length);
            Assert.IsTrue(fmSectionWithCategories.All(r => r.GetType() == typeof(FmSectionWithDirectCategory)));
            CollectionAssert.AreEqual(originalSections.Select(s => s.SectionStart), fmSectionWithCategories.Select(r => r.SectionStart));
            CollectionAssert.AreEqual(originalSections.Select(s => s.SectionEnd), fmSectionWithCategories.Select(r => r.SectionEnd));
            CollectionAssert.AreEqual(originalSections.Select(s => ConvertCategoryGroup(s.CategoryGroup)),
                                      fmSectionWithCategories.Select(r => (FmSectionWithDirectCategory)r)
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