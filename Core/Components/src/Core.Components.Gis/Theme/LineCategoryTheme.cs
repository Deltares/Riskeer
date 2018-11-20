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
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;

namespace Core.Components.Gis.Theme
{
    /// <summary>
    /// Class to define categories for <see cref="MapLineData"/>.
    /// </summary>
    public class LineCategoryTheme : CategoryTheme
    {
        /// <summary>
        /// Creates a new instance of <see cref="LineCategoryTheme"/>.
        /// </summary>
        /// <param name="criterion">The <see cref="ValueCriterion"/> belonging to the category.</param>
        /// <param name="style">The <see cref="LineStyle"/> belonging to the category.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public LineCategoryTheme(ValueCriterion criterion, LineStyle style) : base(criterion)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            Style = style;
        }

        /// <summary>
        /// Gets the <see cref="LineStyle"/> of the category.
        /// </summary>
        public LineStyle Style { get; }
    }
}