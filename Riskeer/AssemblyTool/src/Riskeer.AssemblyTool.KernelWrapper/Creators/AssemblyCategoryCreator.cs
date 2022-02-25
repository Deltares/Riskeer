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
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Model.Categories;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates assembly categories.
    /// </summary>
    public static class AssemblyCategoryCreator
    {
        /// <summary>
        /// Creates a collection of <see cref="AssessmentSectionAssemblyGroupBoundaries"/>
        /// based on the information given in the <paramref name="categories"/>.
        /// </summary>
        /// <param name="categories">The <see cref="Assembly.Kernel.Old.Model.CategoryLimits.CategoriesList{TCategory}"/> with
        /// <see cref="Assembly.Kernel.Old.Model.CategoryLimits.AssessmentSectionCategory"/> to create the result for.</param>
        /// <returns>A collection of <see cref="AssessmentSectionAssemblyGroupBoundaries"/>
        /// with information taken from the <paramref name="categories"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categories"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="categories"/>
        /// contains an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="categories"/>
        /// contains a valid value, but unsupported.</exception>
        public static IEnumerable<AssessmentSectionAssemblyGroupBoundaries> CreateAssessmentSectionAssemblyCategories(CategoriesList<AssessmentSectionCategory> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            return categories.Categories.Select(
                categoriesOutput => new AssessmentSectionAssemblyGroupBoundaries(
                    categoriesOutput.LowerLimit,
                    categoriesOutput.UpperLimit,
                    CreateAssessmentSectionAssemblyCategory(categoriesOutput.Category))).ToArray();
        }

        /// <summary>
        /// Creates a <see cref="AssessmentSectionAssemblyGroup"/> based on <paramref name="category"/>.
        /// </summary>
        /// <param name="category">The <see cref="EAssessmentGrade"/> to convert.</param>
        /// <returns>A <see cref="AssessmentSectionAssemblyGroup"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static AssessmentSectionAssemblyGroup CreateAssessmentSectionAssemblyCategory(EAssessmentGrade category)
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
                    return AssessmentSectionAssemblyGroup.APlus;
                case EAssessmentGrade.A:
                    return AssessmentSectionAssemblyGroup.A;
                case EAssessmentGrade.B:
                    return AssessmentSectionAssemblyGroup.B;
                case EAssessmentGrade.C:
                    return AssessmentSectionAssemblyGroup.C;
                case EAssessmentGrade.D:
                    return AssessmentSectionAssemblyGroup.D;
                case EAssessmentGrade.Gr:
                    return AssessmentSectionAssemblyGroup.None;
                case EAssessmentGrade.Ngo:
                    return AssessmentSectionAssemblyGroup.NotAssessed;
                case EAssessmentGrade.Nvt:
                    return AssessmentSectionAssemblyGroup.NotApplicable;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}