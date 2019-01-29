// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// Extension methods for <see cref="Segment2D"/> collections.
    /// </summary>
    public static class Segment2DCollectionExtensions
    {
        /// <summary>
        /// Interpolates the segments on <paramref name="x"/>.
        /// </summary>
        /// <param name="segments">The segments to interpolate on.</param>
        /// <param name="x">The x value to use for interpolation.</param>
        /// <returns>The interpolated y value of the segment that is closest to <paramref name="x"/> 
        /// or <see cref="double.NaN"/> when no interpolation was found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="segments"/> is <c>null</c>.</exception>
        public static double Interpolate(this IEnumerable<Segment2D> segments, double x)
        {
            if (segments == null)
            {
                throw new ArgumentNullException(nameof(segments));
            }

            Segment2D segment = segments.LastOrDefault(s => s.FirstPoint.X <= x) ?? segments.First();

            return segment.Interpolate(x);
        }
    }
}