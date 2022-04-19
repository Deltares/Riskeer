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
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Model.Categories;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates assessment section assembly groups.
    /// </summary>
    public static class AssessmentSectionAssemblyGroupCreator
    {
        /// <summary>
        /// Creates a collection of <see cref="AssessmentSectionAssemblyGroupBoundaries"/>
        /// based on the information given in the <paramref name="assessmentSectionCategories"/>.
        /// </summary>
        /// <param name="assessmentSectionCategories">The <see cref="CategoriesList{TCategory}"/> with <see cref="AssessmentSectionCategory"/>
        /// to create the result for.</param>
        /// <returns>A collection of <see cref="AssessmentSectionAssemblyGroupBoundaries"/>
        /// with information taken from the <paramref name="assessmentSectionCategories"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSectionCategories"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assessmentSectionCategories"/>
        /// contains an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assessmentSectionCategories"/>
        /// contains a valid value, but unsupported.</exception>
        public static IEnumerable<AssessmentSectionAssemblyGroupBoundaries> CreateAssessmentSectionAssemblyGroupBoundaries(CategoriesList<AssessmentSectionCategory> assessmentSectionCategories)
        {
            if (assessmentSectionCategories == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionCategories));
            }

            return assessmentSectionCategories.Categories.Select(
                categoriesOutput => new AssessmentSectionAssemblyGroupBoundaries(
                    categoriesOutput.LowerLimit,
                    categoriesOutput.UpperLimit,
                    CreateAssessmentSectionAssemblyGroup(categoriesOutput.Category))).ToArray();
        }

        /// <summary>
        /// Creates an <see cref="AssessmentSectionAssemblyGroup"/> based on <paramref name="assessmentGrade"/>.
        /// </summary>
        /// <param name="assessmentGrade">The <see cref="EAssessmentGrade"/> to convert.</param>
        /// <returns>A <see cref="AssessmentSectionAssemblyGroup"/> based on <paramref name="assessmentGrade"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assessmentGrade"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assessmentGrade"/>
        /// is a valid value, but unsupported.</exception>
        public static AssessmentSectionAssemblyGroup CreateAssessmentSectionAssemblyGroup(EAssessmentGrade assessmentGrade)
        {
            if (!Enum.IsDefined(typeof(EAssessmentGrade), assessmentGrade))
            {
                throw new InvalidEnumArgumentException(nameof(assessmentGrade),
                                                       (int) assessmentGrade,
                                                       typeof(EAssessmentGrade));
            }

            switch (assessmentGrade)
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