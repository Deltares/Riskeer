﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;

namespace Core.Components.Gis.Theme
{
    /// <summary>
    /// Class to define categories for <see cref="MapPolygonData"/>.
    /// </summary>
    public class PolygonCategoryTheme : CategoryTheme
    {
        /// <summary>
        /// Creates a new instance of <see cref="PolygonCategoryTheme"/>.
        /// </summary>
        /// <param name="criterion">The criterion belonging to the category.</param>
        /// <param name="style">The <see cref="PolygonStyle"/> belonging to the category.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PolygonCategoryTheme(ValueCriterion criterion, PolygonStyle style) : base(criterion)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            Style = style;
        }

        /// <summary>
        /// Gets the <see cref="PolygonStyle"/> of the category.
        /// </summary>
        public PolygonStyle Style { get; }
    }
}