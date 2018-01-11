// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using Core.Components.Gis.Theme.Criteria;

namespace Core.Components.Gis.Theme
{
    /// <summary>
    /// Class to define themes for categories.
    /// </summary>
    public class CategoryTheme
    {
        /// <summary>
        /// Creates a new instance of <see cref="CategoryTheme"/>.
        /// </summary>
        /// <param name="themeColor">The color to be applied for the theme.</param>
        /// <param name="criteria">The criteria belonging to the category.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="criteria"/>
        /// is <c>null</c>.</exception>
        public CategoryTheme(Color themeColor, ICriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            Color = themeColor;
            Criteria = criteria;
        }

        /// <summary>
        /// Gets the color of the category theme.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Gets the criteria that is associated with the category theme.
        /// </summary>
        public ICriteria Criteria { get; }
    }
}