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
using Assembly.Kernel.Model.CategoryLimits;
using Assembly.Kernel.Model.FmSectionTypes;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class for asserting categories result.
    /// </summary>
    public static class AssemblyCategoryAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original collection of <see cref="AssessmentSectionCategoryLimits"/>.</param>
        /// <param name="actual">The actual collection of <see cref="AssessmentSectionAssemblyCategory"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="original"/>.</exception>
        public static void AssertAssessmentSectionAssemblyCategories(IEnumerable<AssessmentSectionCategoryLimits> original,
                                                                     IEnumerable<AssessmentSectionAssemblyCategory> actual)
        {
            Assert.AreEqual(original.Count(), actual.Count());

            CollectionAssert.AreEqual(original.Select(o => GetAssessmentSectionCategoryGroup(o.Category)), actual.Select(r => r.Group));
            CollectionAssert.AreEqual(original.Select(o => o.LowerLimit), actual.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(original.Select(o => o.UpperLimit), actual.Select(r => r.UpperBoundary));
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original collection of <see cref="FmSectionCategoryLimits"/>.</param>
        /// <param name="actual">The actual collection of <see cref="FailureMechanismSectionAssemblyCategory"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="original"/>.</exception>
        public static void AssertFailureMechanismSectionAssemblyCategories(IEnumerable<FmSectionCategoryLimits> original,
                                                                           IEnumerable<FailureMechanismSectionAssemblyCategory> actual)
        {
            Assert.AreEqual(original.Count(), actual.Count());

            CollectionAssert.AreEqual(original.Select(o => GetFailureMechanismSectionCategoryGroup(o.Category)), actual.Select(r => r.Group));
            CollectionAssert.AreEqual(original.Select(o => o.LowerLimit), actual.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(original.Select(o => o.UpperLimit), actual.Select(r => r.UpperBoundary));
        }

        private static AssessmentSectionAssemblyCategoryGroup GetAssessmentSectionCategoryGroup(EAssessmentGrade category)
        {
            switch (category)
            {
                case EAssessmentGrade.APlus:
                    return AssessmentSectionAssemblyCategoryGroup.APlus;
                case EAssessmentGrade.A:
                    return AssessmentSectionAssemblyCategoryGroup.A;
                case EAssessmentGrade.B:
                    return AssessmentSectionAssemblyCategoryGroup.B;
                case EAssessmentGrade.C:
                    return AssessmentSectionAssemblyCategoryGroup.C;
                case EAssessmentGrade.D:
                    return AssessmentSectionAssemblyCategoryGroup.D;
                default:
                    throw new NotSupportedException();
            }
        }

        private static FailureMechanismSectionAssemblyCategoryGroup GetFailureMechanismSectionCategoryGroup(EFmSectionCategory category)
        {
            switch (category)
            {
                case EFmSectionCategory.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case EFmSectionCategory.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case EFmSectionCategory.IIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case EFmSectionCategory.IVv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IVv;
                case EFmSectionCategory.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case EFmSectionCategory.VIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIv;
                case EFmSectionCategory.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                case EFmSectionCategory.NotApplicable:
                    return FailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                case EFmSectionCategory.Gr:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}