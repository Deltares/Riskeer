// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
    /// Creates failure mechanism section assembly groups.
    /// </summary>
    internal static class FailureMechanismSectionAssemblyGroupCreator
    {
        /// <summary>
        /// Creates a collection of <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/>
        /// based on the information given in the <paramref name="interpretationCategories"/>.
        /// </summary>
        /// <param name="interpretationCategories">The <see cref="CategoriesList{TCategory}"/> with <see cref="InterpretationCategory"/>
        /// to create the result for.</param>
        /// <returns>A collection of <see cref="FailureMechanismSectionAssemblyGroupBoundaries"/>
        /// with information taken from the <paramref name="interpretationCategories"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="interpretationCategories"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="interpretationCategories"/>
        /// contains an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="interpretationCategories"/>
        /// contains a valid value, but unsupported.</exception>
        public static IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> CreateFailureMechanismSectionAssemblyGroupBoundaries(
            CategoriesList<InterpretationCategory> interpretationCategories)
        {
            if (interpretationCategories == null)
            {
                throw new ArgumentNullException(nameof(interpretationCategories));
            }

            return interpretationCategories.Categories
                                           .Select(CreateFailureMechanismSectionAssemblyGroupBoundaries)
                                           .ToArray();
        }

        private static FailureMechanismSectionAssemblyGroupBoundaries CreateFailureMechanismSectionAssemblyGroupBoundaries(InterpretationCategory interpretationCategory)
        {
            return new FailureMechanismSectionAssemblyGroupBoundaries(
                interpretationCategory.LowerLimit, interpretationCategory.UpperLimit,
                FailureMechanismSectionAssemblyGroupConverter.ConvertTo(interpretationCategory.Category));
        }
    }
}