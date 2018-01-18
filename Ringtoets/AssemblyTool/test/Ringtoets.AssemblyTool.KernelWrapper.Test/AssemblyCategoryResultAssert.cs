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
using System.Collections.Generic;
using System.Linq;
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.CategoriesOutput;
using AssemblyTool.Kernel.Data;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data.Output;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test
{
    /// <summary>
    /// Class for asserting categories result.
    /// </summary>
    public static class AssemblyCategoryResultAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="CalculationOutput{AssessmentSectionCategoriesOutput}"/>.</param>
        /// <param name="actual">The actual <see cref="IEnumerable{AssessmentSectionAssemblyCategoryResult}"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="original"/>.</exception>
        public static void AssertAssessmentSectionAssemblyCategoriesResult(CalculationOutput<AssessmentSectionCategoriesOutput[]> original,
                                                                           IEnumerable<AssessmentSectionAssemblyCategoryResult> actual)
        {
            Assert.AreEqual(original.Result.Length, actual.Count());

            CollectionAssert.AreEqual(original.Result.Select(o => GetResultType(o.Category)), actual.Select(r => r.Category));
            CollectionAssert.AreEqual(original.Result.Select(o => o.LowerBoundary), actual.Select(r => r.LowerBoundary));
            CollectionAssert.AreEqual(original.Result.Select(o => o.UpperBoundary), actual.Select(r => r.UpperBoundary));
        }

        private static AssessmentSectionAssemblyCategoryResultType GetResultType(AssessmentSectionAssemblyCategory category)
        {
            switch (category)
            {
                case AssessmentSectionAssemblyCategory.APlus:
                    return AssessmentSectionAssemblyCategoryResultType.APlus;
                case AssessmentSectionAssemblyCategory.A:
                    return AssessmentSectionAssemblyCategoryResultType.A;
                case AssessmentSectionAssemblyCategory.B:
                    return AssessmentSectionAssemblyCategoryResultType.B;
                case AssessmentSectionAssemblyCategory.C:
                    return AssessmentSectionAssemblyCategoryResultType.C;
                case AssessmentSectionAssemblyCategory.D:
                    return AssessmentSectionAssemblyCategoryResultType.D;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}