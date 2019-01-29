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
using System.Globalization;
using Core.Common.Base.Properties;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// Defines a mathematical, immutable point in 2D Euclidean space.
    /// </summary>
    public sealed class Point2D : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="Point2D"/>, with <see cref="X"/> set to <paramref name="x"/>
        /// and <see cref="Y"/> set to <paramref name="y"/>.
        /// </summary>
        /// <param name="x">The X-coordinate to set.</param>
        /// <param name="y">The Y-coordinate to set.</param>
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Point2D"/>, with <see cref="X"/> and <see cref="Y"/>
        /// taken from <paramref name="point"/>.
        /// </summary>
        /// <param name="point">The <see cref="Point2D"/> to take the properties from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
        public Point2D(Point2D point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            X = point.X;
            Y = point.Y;
        }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Determines the 2D vector defined by the difference of two <see cref="Point2D"/>.
        /// </summary>
        /// <param name="p1">Head of the vector.</param>
        /// <param name="p2">Tail of the vector.</param>
        /// <returns>A 2D vector.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="p1"/>
        /// or <paramref name="p2"/> is <c>null</c>.</exception>
        public static Vector<double> operator -(Point2D p1, Point2D p2)
        {
            if (p1 == null)
            {
                throw new ArgumentNullException(nameof(p1));
            }

            if (p2 == null)
            {
                throw new ArgumentNullException(nameof(p2));
            }

            return new DenseVector(2)
            {
                [0] = p1.X - p2.X,
                [1] = p1.Y - p2.Y
            };
        }

        /// <summary>
        /// Determines the new 2D point given a point and a 2D vector.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="vector">The 2D vector.</param>
        /// <returns>
        /// A 2D point.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="vector"/> is 
        /// not a 2D vector.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/>
        /// or <paramref name="vector"/> is <c>null</c>.</exception>
        public static Point2D operator +(Point2D point, Vector<double> vector)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (vector == null)
            {
                throw new ArgumentNullException(nameof(vector));
            }

            if (vector.Count != 2)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               Resources.Point2D_AddVector_Vector_must_be_2D_but_has_Dimensionality_0_,
                                               vector.Count);
                throw new ArgumentException(message, nameof(vector));
            }

            double x = point.X + vector[0];
            double y = point.Y + vector[1];
            return new Point2D(x, y);
        }

        /// <summary>
        /// Gets the euclidean distance from this point to another.
        /// </summary>
        /// <param name="secondPoint">The second point.</param>
        /// <returns>A value of 0 or greater.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="secondPoint"/> 
        /// is <c>null</c>.</exception>
        public double GetEuclideanDistanceTo(Point2D secondPoint)
        {
            if (secondPoint == null)
            {
                throw new ArgumentNullException(nameof(secondPoint));
            }

            Vector<double> vector = this - secondPoint;
            return Math.Sqrt(vector.DotProduct(vector));
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
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 "({0}, {1})",
                                 X, Y);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Compares the <see cref="Point2D"/> with <paramref name="other"/> based on <see cref="X"/>
        /// and <see cref="Y"/>.
        /// </summary>
        /// <param name="other">A <see cref="Point2D"/> to compare with.</param>
        /// <returns><c>true</c> if the coordinates of the <see cref="Point2D"/> matches the 
        /// coordinate of <paramref name="other"/>. <c>false</c> otherwise.</returns>
        private bool Equals(Point2D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }
    }
}