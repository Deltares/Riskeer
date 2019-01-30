// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// Assembly category base.
    /// </summary>
    public abstract class AssemblyCategory
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyCategory"/>.
        /// </summary>
        /// <param name="lowerBoundary">The lower boundary of the category.</param>
        /// <param name="upperBoundary">The upper boundary of the category.</param>
        protected AssemblyCategory(double lowerBoundary, double upperBoundary)
        {
            LowerBoundary = lowerBoundary;
            UpperBoundary = upperBoundary;
        }

        /// <summary>
        /// Gets the lower boundary of the assembly category.
        /// </summary>
        public double LowerBoundary { get; }

        /// <summary>
        /// Gets the upper boundary of the assembly category.
        /// </summary>
        public double UpperBoundary { get; }
    }
}