// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class that creates valid instances of <see cref="CategoriesList{TCategory}"/>
    /// which can be used for testing.
    /// </summary>
    public static class CategoriesListTestFactory
    {
        private static readonly Random random = new Random(21);

        /// <summary>
        /// Creates a valid instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="FmSectionCategory"/>.
        /// </summary>
        /// <returns>An instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="FmSectionCategory"/>.</returns>
        public static CategoriesList<FmSectionCategory> CreateFailureMechanismSectionCategories()
        {
            return new CategoriesList<FmSectionCategory>(new[]
            {
                new FmSectionCategory(random.NextEnumValue<EFmSectionCategory>(), 0, 0.25),
                new FmSectionCategory(random.NextEnumValue<EFmSectionCategory>(), 0.25, 0.5),
                new FmSectionCategory(random.NextEnumValue<EFmSectionCategory>(), 0.5, 0.75),
                new FmSectionCategory(random.NextEnumValue<EFmSectionCategory>(), 0.75, 1.0)
            });
        }

        /// <summary>
        /// Creates a valid instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="FailureMechanismCategory"/>.
        /// </summary>
        /// <returns>An instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="FailureMechanismCategory"/>.</returns>
        public static CategoriesList<FailureMechanismCategory> CreateFailureMechanismCategories()
        {
            return new CategoriesList<FailureMechanismCategory>(new[]
            {
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0, 0.25),
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0.25, 0.5),
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0.5, 0.75),
                new FailureMechanismCategory(random.NextEnumValue<EFailureMechanismCategory>(), 0.75, 1.0)
            });
        }

        /// <summary>
        /// Creates a valid instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="AssessmentSectionCategory"/>.
        /// </summary>
        /// <returns>An instance of <see cref="CategoriesList{TCategory}"/>
        /// containing <see cref="AssessmentSectionCategory"/>.</returns>
        public static CategoriesList<AssessmentSectionCategory> CreateAssessmentSectionCategories()
        {
            return new CategoriesList<AssessmentSectionCategory>(new[]
            {
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0, 0.25),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.25, 0.5),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.5, 0.75),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), 0.75, 1.0)
            });
        }
    }
}