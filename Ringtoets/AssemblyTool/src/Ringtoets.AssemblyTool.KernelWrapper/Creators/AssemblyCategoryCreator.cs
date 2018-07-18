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
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="AssemblyCategory"/> instances.
    /// </summary>
    internal static class AssemblyCategoryCreator
    {
        /// <summary>
        /// Creates a collection of <see cref="AssessmentSectionAssemblyCategory"/>
        /// based on the information given in the <paramref name="categoryLimits"/>.
        /// </summary>
        /// <param name="categoryLimits">The <see cref="CategoriesList{TCategory}"/> with
        /// <see cref="AssessmentSectionCategory"/> to create the result for.</param>
        /// <returns>A collection of <see cref="AssessmentSectionAssemblyCategory"/>
        /// with information taken from the <paramref name="categoryLimits"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categoryLimits"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categoryLimits"/>
        /// contains an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categoryLimits"/>
        /// contains a valid value, but unsupported.</exception>
        public static IEnumerable<AssessmentSectionAssemblyCategory> CreateAssessmentSectionAssemblyCategories(
            CategoriesList<AssessmentSectionCategory> categoryLimits)
        {
            if (categoryLimits == null)
            {
                throw new ArgumentNullException(nameof(categoryLimits));
            }

            return categoryLimits.Categories.Select(
                categoriesOutput => new AssessmentSectionAssemblyCategory(
                    categoriesOutput.LowerLimit,
                    categoriesOutput.UpperLimit,
                    CreateAssessmentSectionAssemblyCategory(categoriesOutput.Category))).ToArray();
        }

        /// <summary>
        /// Creates a collection of <see cref="FailureMechanismAssemblyCategory"/>
        /// based on the information given in the <paramref name="categoryLimits"/>.
        /// </summary>
        /// <param name="categoryLimits">The <see cref="CategoriesList{TCategory}"/>
        /// with <see cref="FailureMechanismCategory"/> to create the result for.</param>
        /// <returns>A collection of <see cref="FailureMechanismAssemblyCategory"/>
        /// with information taken from the <paramref name="categoryLimits"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categoryLimits"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categoryLimits"/>
        /// contains an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categoryLimits"/>
        /// contains a valid value, but unsupported.</exception>
        public static IEnumerable<FailureMechanismAssemblyCategory> CreateFailureMechanismAssemblyCategories(
            CategoriesList<FailureMechanismCategory> categoryLimits)
        {
            if (categoryLimits == null)
            {
                throw new ArgumentNullException(nameof(categoryLimits));
            }

            return categoryLimits.Categories.Select(
                categoriesOutput => new FailureMechanismAssemblyCategory(
                    categoriesOutput.LowerLimit,
                    categoriesOutput.UpperLimit,
                    FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup(categoriesOutput.Category))).ToArray();
        }

        /// <summary>
        /// Creates a collection of <see cref="FailureMechanismSectionAssemblyCategory"/>
        /// based on the information given in the <paramref name="categoryLimits"/>.
        /// </summary>
        /// <param name="categoryLimits">The collection of <see cref="FmSectionCategoryLimits"/> to
        /// create the result for.</param>
        /// <returns>A collection of <see cref="FailureMechanismSectionAssemblyCategory"/>
        /// with information taken from the <paramref name="categoryLimits"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categoryLimits"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categoryLimits"/>
        /// contains an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categoryLimits"/>
        /// contains a valid value, but unsupported.</exception>
        public static IEnumerable<FailureMechanismSectionAssemblyCategory> CreateFailureMechanismSectionAssemblyCategories(
            IEnumerable<FmSectionCategoryLimits> categoryLimits)
        {
            if (categoryLimits == null)
            {
                throw new ArgumentNullException(nameof(categoryLimits));
            }

            return categoryLimits.Select(
                categoriesOutput => new FailureMechanismSectionAssemblyCategory(
                    categoriesOutput.LowerLimit,
                    categoriesOutput.UpperLimit,
                    FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(categoriesOutput.Category))).ToArray();
        }

        /// <summary>
        /// Creates a <see cref="AssessmentSectionAssemblyCategoryGroup"/> based on <paramref name="category"/>.
        /// </summary>
        /// <param name="category">The <see cref="EAssessmentGrade"/> to convert.</param>
        /// <returns>A <see cref="AssessmentSectionAssemblyCategoryGroup"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static AssessmentSectionAssemblyCategoryGroup CreateAssessmentSectionAssemblyCategory(EAssessmentGrade category)
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
                case EAssessmentGrade.Gr:
                    return AssessmentSectionAssemblyCategoryGroup.None;
                case EAssessmentGrade.Ngo:
                    return AssessmentSectionAssemblyCategoryGroup.NotAssessed;
                case EAssessmentGrade.Nvt:
                    return AssessmentSectionAssemblyCategoryGroup.NotApplicable;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}