// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// A collection of points which together form a closed line.
    /// </summary>
    public class Ring
    {
        /// <summary>
        /// Creates a new instance of <see cref="Ring"/>.
        /// </summary>
        /// <param name="points">The points that form the ring.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="points"/> contains less than 2 unique points.</exception>
        /// <remarks>While a ring is defined to be closed line, it's not required
        /// that the given <paramref name="points"/>' first point and last point
        /// are equal.</remarks>
        public Ring(IEnumerable<Point2D> points)
        {
            ValidateAndTrimPoints(points);
            Points = new RoundedPoint2DCollection(2, points);
        }

        /// <summary>
        /// Gets the points that form the ring.
        /// </summary>
        public RoundedPoint2DCollection Points { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Ring) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 397;
                foreach (Point2D p in Points)
                {
                    hashCode = (hashCode * 397) ^ p.GetHashCode();
                }

                return hashCode;
            }
        }

        private bool Equals(Ring other)
        {
            return Points.SequenceEqual(other.Points);
        }

        /// <summary>
        /// Validates the points collection.
        /// </summary>
        /// <param name="points">The points to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="points"/> contains less than 2 unique points.</exception>
        private void ValidateAndTrimPoints(IEnumerable<Point2D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            if (points.Distinct().Count() < 2)
            {
                throw new ArgumentException($@"Need at least two distinct points to define a {typeof(Ring).Name}.", nameof(points));
            }
        }
    }
}