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
using System.ComponentModel;
using System.Linq;
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using Ringtoets.Common.Data.AssemblyTool;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="AssessmentSectionAssemblyCategory"/> instances.
    /// </summary>
    internal static class AssessmentSectionAssemblyCategoryCreator
    {
        /// <summary>
        /// Creates an <see cref="IEnumerable{AssessmentSectionAssemblyCategoryResult}"/>
        /// based on the information given in the <paramref name="output"/>.
        /// </summary>
        /// <param name="output">The output to create the result for.</param>
        /// <returns>An <see cref="IEnumerable{AssessmentSectionAssemblyCategoryResult}"/>
        /// with information taken from the <paramref name="output"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="AssessmentSectionCategoryGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="AssessmentSectionCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static IEnumerable<AssessmentSectionAssemblyCategory> CreateAssessmentSectionAssemblyCategories(
            CalculationOutput<AssessmentSectionCategory[]> output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return output.Result.Select(
                categoriesOutput => new AssessmentSectionAssemblyCategory(categoriesOutput.LowerBoundary,
                                                                          categoriesOutput.UpperBoundary,
                                                                          ConvertAssessmentSectionCategoryGroup(categoriesOutput.CategoryGroup))).ToArray();
        }

        /// <summary>
        /// Converts a <see cref="category"/> into a <see cref="AssessmentSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="category">The <see cref="AssessmentSectionCategoryGroup"/> to convert.</param>
        /// <returns>A <see cref="AssessmentSectionAssemblyCategoryGroup"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        private static AssessmentSectionAssemblyCategoryGroup ConvertAssessmentSectionCategoryGroup(AssessmentSectionCategoryGroup category)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionCategoryGroup), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(AssessmentSectionCategoryGroup));
            }

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
    }
}