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
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    /// <summary>
    /// A ring is a collection of points which together form a closed line.
    /// </summary>
    public class Ring
    {
        public Point2D[] Points { get; }

        /// <summary>
        /// Creates a new instance of <see cref="Ring"/>.
        /// </summary>
        /// <param name="points">The points that form the ring.</param>
        /// <remarks>While a ring is defined to be closed line, it's not required
        /// that the given <paramref name="points"/>' first point and last point
        /// are equal.</remarks>
        public Ring(Point2D[] points)
        {
            ValidateAndTrimPoints(points);
            Points = points;
        }

        /// <summary>
        /// Validates the points collection and trims the last point from the collection
        /// if it is equal to the first.
        /// </summary>
        /// <param name="points">The points to validate and trim if required.</param>
        /// <exception cref="ArgumentNullException">Thrown when points is <c>null</c>.</exception>
        private void ValidateAndTrimPoints(Point2D[] points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }
            if (points.Length < 2)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(points));
            }
        }
    }
}