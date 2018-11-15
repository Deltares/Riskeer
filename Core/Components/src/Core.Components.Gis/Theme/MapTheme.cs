// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Components.Gis.Theme
{
    /// <summary>
    /// Class that contains the definition for a categorical theming of 
    /// map data objects.
    /// </summary>
    /// <typeparam name="TCategoryTheme">The type of category theme for the categorical theming.</typeparam>
    public class MapTheme<TCategoryTheme> where TCategoryTheme : CategoryTheme
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapTheme{T}"/>.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to which the theme is applicable for.</param>
        /// <param name="categoryThemes">The category themes that are applicable for the map theme.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categoryThemes"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="attributeName"/> is null, empty or consists of only whitespace.</item>
        /// <item><paramref name="categoryThemes"/> is empty.</item>
        /// </list></exception>
        public MapTheme(string attributeName, IEnumerable<TCategoryTheme> categoryThemes)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentException($@"{nameof(attributeName)} is null, empty or consists of whitespace.", nameof(attributeName));
            }

            if (categoryThemes == null)
            {
                throw new ArgumentNullException(nameof(categoryThemes));
            }

            if (!categoryThemes.Any())
            {
                throw new ArgumentException($@"{nameof(categoryThemes)} is empty.", nameof(categoryThemes));
            }

            CategoryThemes = categoryThemes;
            AttributeName = attributeName;
        }

        /// <summary>
        /// Gets the attribute to which the theme applies to.
        /// </summary>
        public string AttributeName { get; }

        /// <summary>
        /// Gets the category themes that are applicable for the theme.
        /// </summary>
        public IEnumerable<TCategoryTheme> CategoryThemes { get; }
    }
}