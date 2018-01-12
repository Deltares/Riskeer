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
using AssemblyTool.Kernel.CategoriesOutput;
using AssemblyTool.Kernel.Data;
using Ringtoets.AssemblyTool.Data.Output;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="AssemblyCategoryResult"/> instances.
    /// </summary>
    internal static class AssemblyCategoryResultCreator
    {
        /// <summary>
        /// Creates an <see cref="IEnumerable{AssessmentSectionAssemblyCategoryResult}"/>
        /// based on the information given in the <paramref name="output"/>.
        /// </summary>
        /// <param name="output">The output to create the result for.</param>
        /// <returns>An <see cref="IEnumerable{AssessmentSectionAssemblyCategoryResult}"/>
        /// with information taken from the <paramref name="output"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="AssessmentSectionAssemblyCategory"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="AssessmentSectionAssemblyCategory"/>
        /// is a valid value but unsupported.</exception>
        public static IEnumerable<AssessmentSectionAssemblyCategoryResult> CreateAssessmentSectionAssemblyCategoryResult(
            CalculationOutput<AssessmentSectionCategoriesOutput[]> output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return output.Result.Select(
                categoriesOutput => new AssessmentSectionAssemblyCategoryResult(categoriesOutput.LowerBoundary,
                                                                                categoriesOutput.UpperBoundary, ConvertAssessmentSectionCategoryType(categoriesOutput.Category))).ToArray();
        }

        /// <summary>
        /// Converts a <see cref="category"/> into a <see cref="AssessmentSectionAssemblyCategoryResultType"/>.
        /// </summary>
        /// <param name="category">The <see cref="AssessmentSectionAssemblyCategory"/> to convert.</param>
        /// <returns>A <see cref="AssessmentSectionAssemblyCategoryResultType"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value but unsupported.</exception>
        private static AssessmentSectionAssemblyCategoryResultType ConvertAssessmentSectionCategoryType(AssessmentSectionAssemblyCategory category)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionAssemblyCategory), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(AssessmentSectionAssemblyCategory));
            }

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