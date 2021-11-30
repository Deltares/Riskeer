// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    /// Class for asserting assembly group limits.
    /// </summary>
    public static class AssemblyGroupLimitsAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="CategoriesList{TCategory}"/> with
        /// <see cref="InterpretationCategory"/>.</param>
        /// <param name="actual">The actual collection of <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="original"/>.</exception>
        public static void AssertFailureMechanismSectionAssemblyGroupLimits(CategoriesList<InterpretationCategory> original,
                                                                            IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> actual)
        {
            int expectedNrOfCategories = original.Categories.Length;
            Assert.AreEqual(expectedNrOfCategories, actual.Count());

            for (int i = 0; i < expectedNrOfCategories; i++)
            {
                InterpretationCategory originalItem = original.Categories.ElementAt(i);
                FailureMechanismSectionAssemblyGroupBoundaries actualItem = actual.ElementAt(i);

                Assert.AreEqual(GetAssemblyGroup(originalItem.Category), actualItem.Group);
                ProbabilityAssert.AreEqual(actualItem.LowerBoundary, originalItem.LowerLimit);
                ProbabilityAssert.AreEqual(actualItem.UpperBoundary, originalItem.UpperLimit);
            }
        }

        private static FailureMechanismSectionAssemblyGroup GetAssemblyGroup(EInterpretationCategory category)
        {
            switch (category)
            {
                case EInterpretationCategory.ND:
                    return FailureMechanismSectionAssemblyGroup.ND;
                case EInterpretationCategory.III:
                    return FailureMechanismSectionAssemblyGroup.III;
                case EInterpretationCategory.II:
                    return FailureMechanismSectionAssemblyGroup.II;
                case EInterpretationCategory.I:
                    return FailureMechanismSectionAssemblyGroup.I;
                case EInterpretationCategory.ZeroPlus:
                    return FailureMechanismSectionAssemblyGroup.ZeroPlus;
                case EInterpretationCategory.Zero:
                    return FailureMechanismSectionAssemblyGroup.Zero;
                case EInterpretationCategory.IMin:
                    return FailureMechanismSectionAssemblyGroup.IMin;
                case EInterpretationCategory.IIMin:
                    return FailureMechanismSectionAssemblyGroup.IIMin;
                case EInterpretationCategory.IIIMin:
                    return FailureMechanismSectionAssemblyGroup.IIIMin;
                case EInterpretationCategory.D:
                    return FailureMechanismSectionAssemblyGroup.D;
                case EInterpretationCategory.Gr:
                    return FailureMechanismSectionAssemblyGroup.Gr;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}