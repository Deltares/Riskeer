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

using System;
using System.Collections.Generic;
using System.Linq;
using Assembly.Kernel.Model.Categories;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class for asserting assessment section assembly groups boundaries.
    /// </summary>
    public static class AssessmentSectionAssemblyGroupBoundariesAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="CategoriesList{TCategory}"/> with
        /// <see cref="AssessmentSectionCategory"/>.</param>
        /// <param name="actual">The actual collection of <see cref="AssessmentSectionAssemblyGroupBoundaries"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="original"/>.</exception>
        public static void AssertAssessmentSectionAssemblyGroupBoundaries(CategoriesList<AssessmentSectionCategory> original,
                                                                          IEnumerable<AssessmentSectionAssemblyGroupBoundaries> actual)
        {
            Assert.AreEqual(original.Categories.Count(), actual.Count());

            CollectionAssert.AreEqual(original.Categories.Select(o => GetAssessmentSectionAssemblyGroup(o.Category)),
                                      actual.Select(r => r.AssessmentSectionAssemblyGroup));
            CollectionAssert.AreEqual(original.Categories.Select(o => (double) o.LowerLimit), actual.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(original.Categories.Select(o => (double) o.UpperLimit), actual.Select(r => r.UpperBoundary));
        }

        private static AssessmentSectionAssemblyGroup GetAssessmentSectionAssemblyGroup(EAssessmentGrade group)
        {
            switch (group)
            {
                case EAssessmentGrade.APlus:
                    return AssessmentSectionAssemblyGroup.APlus;
                case EAssessmentGrade.A:
                    return AssessmentSectionAssemblyGroup.A;
                case EAssessmentGrade.B:
                    return AssessmentSectionAssemblyGroup.B;
                case EAssessmentGrade.C:
                    return AssessmentSectionAssemblyGroup.C;
                case EAssessmentGrade.D:
                    return AssessmentSectionAssemblyGroup.D;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}