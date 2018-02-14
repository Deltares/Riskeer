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
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Data.AssemblyCategories;
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
        /// <param name="original">The original <see cref="CalculationOutput{T}"/>.</param>
        /// <param name="actual">The actual <see cref="IEnumerable{T}"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="original"/>.</exception>
        public static void AssertAssessmentSectionAssemblyCategories(CalculationOutput<AssessmentSectionCategory[]> original,
                                                                     IEnumerable<AssessmentSectionAssemblyCategory> actual)
        {
            Assert.AreEqual(original.Result.Length, actual.Count());

            CollectionAssert.AreEqual(original.Result.Select(o => GetAssessmentSectionCategoryGroup(o.CategoryGroup)), actual.Select(r => r.Group));
            CollectionAssert.AreEqual(original.Result.Select(o => o.LowerBoundary), actual.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(original.Result.Select(o => o.UpperBoundary), actual.Select(r => r.UpperBoundary));
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="CalculationOutput{T}"/>.</param>
        /// <param name="actual">The actual <see cref="IEnumerable{T}"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="original"/>.</exception>
        public static void AssertFailureMechanismSectionAssemblyCategories(CalculationOutput<FailureMechanismSectionCategory[]> original,
                                                                           IEnumerable<FailureMechanismSectionAssemblyCategory> actual)
        {
            Assert.AreEqual(original.Result.Length, actual.Count());

            CollectionAssert.AreEqual(original.Result.Select(o => GetFailureMechanismSectionCategoryGroup(o.CategoryGroup)), actual.Select(r => r.Group));
            CollectionAssert.AreEqual(original.Result.Select(o => o.LowerBoundary), actual.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(original.Result.Select(o => o.UpperBoundary), actual.Select(r => r.UpperBoundary));
        }

        private static AssessmentSectionAssemblyCategoryGroup GetAssessmentSectionCategoryGroup(AssessmentSectionCategoryGroup category)
        {
            switch (category)
            {
                case AssessmentSectionCategoryGroup.APlus:
                    return AssessmentSectionAssemblyCategoryGroup.APlus;
                case AssessmentSectionCategoryGroup.A:
                    return AssessmentSectionAssemblyCategoryGroup.A;
                case AssessmentSectionCategoryGroup.B:
                    return AssessmentSectionAssemblyCategoryGroup.B;
                case AssessmentSectionCategoryGroup.C:
                    return AssessmentSectionAssemblyCategoryGroup.C;
                case AssessmentSectionCategoryGroup.D:
                    return AssessmentSectionAssemblyCategoryGroup.D;
                default:
                    throw new NotSupportedException();
            }
        }

        private static FailureMechanismSectionAssemblyCategoryGroup GetFailureMechanismSectionCategoryGroup(FailureMechanismSectionCategoryGroup category)
        {
            switch (category)
            {
                case FailureMechanismSectionCategoryGroup.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case FailureMechanismSectionCategoryGroup.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case FailureMechanismSectionCategoryGroup.IIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case FailureMechanismSectionCategoryGroup.IVv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IVv;
                case FailureMechanismSectionCategoryGroup.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case FailureMechanismSectionCategoryGroup.VIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIv;
                case FailureMechanismSectionCategoryGroup.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                case FailureMechanismSectionCategoryGroup.NotApplicable:
                    return FailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                case FailureMechanismSectionCategoryGroup.None:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}