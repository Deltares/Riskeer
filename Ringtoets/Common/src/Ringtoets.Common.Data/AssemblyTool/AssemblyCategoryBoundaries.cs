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

namespace Ringtoets.Common.Data.AssemblyTool
{
    /// <summary>
    /// Class that defines the assembly category boundaries.
    /// </summary>
    /// <typeparam name="T">The type of the assembly categories.</typeparam>
    public class AssemblyCategoryBoundaries<T> where T : AssemblyCategory
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyCategoryBoundaries{T}"/>.
        /// </summary>
        /// <param name="categories">The categories of the category boundaries.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categories"/>
        /// is <c>null</c>.</exception>
        public AssemblyCategoryBoundaries(IEnumerable<T> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            Categories = categories;
        }

        /// <summary>
        /// Gets the categories of the category boundaries.
        /// </summary>
        public IEnumerable<T> Categories { get; }
    }
}