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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Riskeer.Common.Data.DikeProfiles
{
    /// <summary>
    /// This class represents a geometry point with a roughness.
    /// </summary>
    public class RoughnessPoint
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RoughnessPoint"/> class.
        /// </summary>
        /// <param name="point">The geometry point.</param>
        /// <param name="roughness">The roughness.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
        public RoughnessPoint(Point2D point, double roughness)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            Point = new Point2D(
                new RoundedDouble(2, point.X),
                new RoundedDouble(2, point.Y));

            Roughness = new RoundedDouble(2, roughness);
        }

        /// <summary>
        /// Gets the geometry point of the <see cref="RoughnessPoint"/>.
        /// </summary>
        public Point2D Point { get; }

        /// <summary>
        /// Gets the roughness of the <see cref="RoughnessPoint"/>.
        /// </summary>
        public RoundedDouble Roughness { get; }

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

            return Equals((RoughnessPoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Roughness.GetHashCode();
                hashCode = (hashCode * 397) ^ (Point?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        private bool Equals(RoughnessPoint other)
        {
            return Equals(Point, other.Point) && Roughness.Equals(other.Roughness);
        }
    }
}