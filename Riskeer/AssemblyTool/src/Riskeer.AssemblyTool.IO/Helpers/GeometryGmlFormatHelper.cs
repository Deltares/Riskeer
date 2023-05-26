﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Globalization;
using System.Linq;
using Core.Common.Base.Geometry;

namespace Riskeer.AssemblyTool.IO.Helpers
{
    /// <summary>
    /// Helper methods to format geometry that can be written to GML.
    /// </summary>
    public static class GeometryGmlFormatHelper
    {
        /// <summary>
        /// Formats a collection of <see cref="Point2D"/> to a string for GML.
        /// </summary>
        /// <param name="geometry">The collection of points to format.</param>
        /// <returns>A formatted string of all given points.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="geometry"/> contains no elements.</exception>
        public static string Format(IEnumerable<Point2D> geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            if (!geometry.Any())
            {
                throw new ArgumentException(@"Geometry cannot be empty.", nameof(geometry));
            }

            return geometry.Select(Format).Aggregate((c1, c2) => c1 + " " + c2);
        }

        private static string Format(Point2D point)
        {
            return point.X.ToString(CultureInfo.InvariantCulture) + " " + point.Y.ToString(CultureInfo.InvariantCulture);
        }
    }
}