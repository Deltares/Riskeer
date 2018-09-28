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
using System.Globalization;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// Defines a mathematical, immutable point in 3D Euclidean space.
    /// </summary>
    public sealed class Point3D
    {
        /// <summary>
        /// Creates a new instance of <see cref="Point3D"/>.
        /// </summary>
        /// <param name="x">The x-coordinate of the new <see cref="Point3D"/>.</param>
        /// <param name="y">The y-coordinate of the new <see cref="Point3D"/>.</param>
        /// <param name="z">The z-coordinate of the new <see cref="Point3D"/>.</param>
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Point3D"/>, with <see cref="X"/>, <see cref="Y"/>
        /// and <see cref="Z"/> taken from <paramref name="point"/>.
        /// </summary>
        /// <param name="point">The <see cref="Point3D"/> to take the properties from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
        public Point3D(Point3D point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        /// <summary>
        /// Gets or sets the x coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets or sets the y coordinate.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets or sets the z coordinate.
        /// </summary>
        public double Z { get; }

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

            return Equals((Point3D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 "({0}, {1}, {2})",
                                 X, Y, Z);
        }

        /// <summary>
        /// Compares the <see cref="Point3D"/> with <paramref name="other"/> based on <see cref="X"/>, <see cref="Y"/> and <see cref="Z"/>.
        /// </summary>
        /// <param name="other">A <see cref="Point3D"/> to compare with.</param>
        /// <returns>True if the coordinates of the <see cref="Point3D"/> matches the coordinate of <paramref name="other"/>. False otherwise.</returns>
        private bool Equals(Point3D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }
    }
}