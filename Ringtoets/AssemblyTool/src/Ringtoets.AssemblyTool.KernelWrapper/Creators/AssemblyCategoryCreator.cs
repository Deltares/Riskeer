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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;
using Assembly.Kernel.Model.FmSectionTypes;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="AssemblyCategory"/> instances.
    /// </summary>
    internal static class AssemblyCategoryCreator
    {
        /// <summary>
        /// Creates an <see cref="IEnumerable{AssessmentSectionAssemblyCategoryResult}"/>
        /// based on the information given in the <paramref name="output"/>.
        /// </summary>
        /// <param name="output">The output to create the result for.</param>
        /// <returns>An <see cref="IEnumerable{AssessmentSectionAssemblyCategoryResult}"/>
        /// with information taken from the <paramref name="output"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="AssessmentSectionCategoryLimits"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="AssessmentSectionCategoryLimits"/>
        /// is a valid value, but unsupported.</exception>
        public static IEnumerable<AssessmentSectionAssemblyCategory> CreateAssessmentSectionAssemblyCategories(
            IEnumerable<AssessmentSectionCategoryLimits> output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return output.Select(
                categoriesOutput => new AssessmentSectionAssemblyCategory(categoriesOutput.LowerLimit,
                                                                          categoriesOutput.UpperLimit,
                                                                          ConvertAssessmentSectionCategoryGroup(categoriesOutput.Category))).ToArray();
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{FailureMechanismSectionAssemblyCategory}"/>
        /// based on the information given in the <paramref name="output"/>.
        /// </summary>
        /// <param name="output">The output to create the result for.</param>
        /// <returns>An <see cref="IEnumerable{FailureMechanismSectionAssemblyCategory}"/>
        /// with information taken from the <paramref name="output"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FmSectionCategoryLimits"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FmSectionCategoryLimits"/>
        /// is a valid value, but unsupported.</exception>
        public static IEnumerable<FailureMechanismSectionAssemblyCategory> CreateFailureMechanismSectionAssemblyCategories(
            IEnumerable<FmSectionCategoryLimits> output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return output.Select(
                categoriesOutput => new FailureMechanismSectionAssemblyCategory(categoriesOutput.LowerLimit,
                                                                                categoriesOutput.UpperLimit,
                                                                                ConvertFailureMechanismSectionCategoryGroup(categoriesOutput.Category))).ToArray();
        }

        /// <summary>
        /// Converts a <see cref="EAssessmentGrade"/> into a <see cref="AssessmentSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="category">The <see cref="EAssessmentGrade"/> to convert.</param>
        /// <returns>A <see cref="AssessmentSectionAssemblyCategoryGroup"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        private static AssessmentSectionAssemblyCategoryGroup ConvertAssessmentSectionCategoryGroup(EAssessmentGrade category)
        {
            if (!Enum.IsDefined(typeof(EAssessmentGrade), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(EAssessmentGrade));
            }

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

        /// <summary>
        /// Converts a <see cref="EFmSectionCategory"/> into a <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="category">The <see cref="EFmSectionCategory"/> to convert.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        private static FailureMechanismSectionAssemblyCategoryGroup ConvertFailureMechanismSectionCategoryGroup(EFmSectionCategory category)
        {
            if (!Enum.IsDefined(typeof(EFmSectionCategory), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(EFmSectionCategory));
            }

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