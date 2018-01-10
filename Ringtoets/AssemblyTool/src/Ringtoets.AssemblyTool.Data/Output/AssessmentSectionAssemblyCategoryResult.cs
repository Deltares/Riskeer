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
using System.ComponentModel;

namespace Ringtoets.AssemblyTool.Data.Output
{
    /// <summary>
    /// The assembly category result of an assessment section category boundaries calculation.
    /// </summary>
    public class AssessmentSectionAssemblyCategoryResult : IAssemblyCategoryResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyCategoryResult"/>.
        /// </summary>
        /// <param name="category">The category type of the result.</param>
        /// <param name="lowerBoundary">The lower boundary of the category.</param>
        /// <param name="upperBoundary">The upper boundary of the category.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// contains an invalid value.</exception>
        public AssessmentSectionAssemblyCategoryResult(AssessmentSectionAssemblyCategoryResultType category,
                                                       double lowerBoundary, double upperBoundary)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionAssemblyCategoryResultType), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(AssessmentSectionAssemblyCategoryResultType));
            }

            Category = category;
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
        }

        /// <summary>
        /// Gets the category type of the result.
        /// </summary>
        public AssessmentSectionAssemblyCategoryResultType Category { get; }

        public double LowerBoundary { get; }

        public double UpperBoundary { get; }
    }
}