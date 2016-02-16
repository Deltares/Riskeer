// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using MathNet.Numerics.LinearAlgebra.Double;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// Defines a mathematical point in 2D Euclidean space.
    /// </summary>
    public class Point2D
    {
        /// <summary>
        /// Creates a new instance of <see cref="Point2D"/>, with <see cref="X"/> set to <c>0</c>
        /// and <see cref="Y"/> set to <c>0</c>.
        /// </summary>
        public Point2D() {}

        /// <summary>
        /// Creates a new instance of <see cref="Point2D"/>, with <see cref="X"/> set to <paramref name="x"/>
        /// and <see cref="Y"/> set to <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The x coordinate to set.</param>
        /// <param name="y">The y coordinate to set.</param>
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets or sets the x coordinate.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y coordinate.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Determines the 2D vector defined by the difference of two <see cref="Point2D"/>.
        /// </summary>
        /// <param name="p1">Head of the vector.</param>
        /// <param name="p2">Tail of the vector.</param>
        /// <returns>A 2D vector.</returns>
        public static Vector operator -(Point2D p1, Point2D p2)
        {
            var result = new DenseVector(2);
            result[0] = p1.X - p2.X;
            result[1] = p1.Y - p2.Y;
            return result;
        }

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
            return Equals((Point2D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode*397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", X, Y);
        }

        /// <summary>
        /// Compares the <see cref="Point2D"/> with <paramref name="other"/> based on <see cref="X"/> and <see cref="Y"/>.
        /// </summary>
        /// <param name="other">A <see cref="Point2D"/> to compare with.</param>
        /// <returns>True if the coordinates of the <see cref="Point3D"/> matches the coordinate of <paramref name="other"/>. False otherwise.</returns>
        private bool Equals(Point2D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }
    }
}