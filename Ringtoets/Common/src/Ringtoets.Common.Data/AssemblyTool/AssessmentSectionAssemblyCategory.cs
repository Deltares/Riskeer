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
using System.ComponentModel;

namespace Ringtoets.Common.Data.AssemblyTool
{
    /// <summary>
    /// Assembly category for assessment section.
    /// </summary>
    public class AssessmentSectionAssemblyCategory : AssemblyCategory
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyCategory"/>.
        /// </summary>
        /// <param name="lowerBoundary">The lower boundary of the category.</param>
        /// <param name="upperBoundary">The upper boundary of the category.</param>
        /// <param name="type">The type of the category.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="type"/>
        /// is an invalid value.</exception>
        public AssessmentSectionAssemblyCategory(double lowerBoundary, double upperBoundary, AssessmentSectionAssemblyCategoryType type)
            : base(lowerBoundary, upperBoundary)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionAssemblyCategoryType), type))
            {
                throw new InvalidEnumArgumentException(nameof(type),
                                                       (int) type,
                                                       typeof(AssessmentSectionAssemblyCategoryType));
            }

            Type = type;
        }

        /// <summary>
        /// Gets the type of the assembly category.
        /// </summary>
        public AssessmentSectionAssemblyCategoryType Type { get; }
    }
}