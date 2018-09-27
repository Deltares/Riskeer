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
using Core.Common.Base.Properties;
using MathNet.Numerics.LinearAlgebra;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// This class represents an immutable line-segment between two <see cref="Point2D"/>.
    /// </summary>
    public sealed class Segment2D
    {
        /// <summary>
        /// Creates a new instance of <see cref="Segment2D"/>, with the <see cref="FirstPoint"/> set to
        /// <paramref name="first"/> and the <see cref="SecondPoint"/> set to <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first <see cref="Point2D"/> of the <see cref="Segment2D"/>.</param>
        /// <param name="second">The second <see cref="Point2D"/> of the <see cref="Segment2D"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either the <paramref name="first"/> or <paramref name="second"/>
        /// point is <c>null</c>.</exception>
        public Segment2D(Point2D first, Point2D second)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first), Resources.Segment2D_Constructor_Segment_must_be_created_with_two_points);
            }

            if (second == null)
            {
                throw new ArgumentNullException(nameof(second), Resources.Segment2D_Constructor_Segment_must_be_created_with_two_points);
            }

            FirstPoint = first;
            SecondPoint = second;
            Length = FirstPoint.GetEuclideanDistanceTo(SecondPoint);
        }

        /// <summary>
        /// The first <see cref="Point2D"/> of the <see cref="Segment2D"/>.
        /// </summary>
        public Point2D FirstPoint { get; }

        /// <summary>
        /// The second <see cref="Point2D"/> of the <see cref="Segment2D"/>.
        /// </summary>
        public Point2D SecondPoint { get; }

        /// <summary>
        /// Gets the (euclidean) length of the segment.
        /// </summary>
        public double Length { get; }

        /// <summary>
        /// This method determines whether <paramref name="x"/> is contained by the <see cref="FirstPoint"/>
        /// and <see cref="SecondPoint"/> X-coordinates.
        /// </summary>
        /// <param name="x">The x for which to find out whether it is contained by the <see cref="FirstPoint"/>
        /// and <see cref="SecondPoint"/>.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="x"/> is on or between the points' X-coordinates.
        /// <c>false</c> otherwise.
        /// </returns>
        public bool ContainsX(double x)
        {
            double distanceFirstPoint = FirstPoint.X - x;
            double distanceSecondPoint = SecondPoint.X - x;

            bool onPoint = Math.Abs(FirstPoint.X - x) < 1e-6 || Math.Abs(SecondPoint.X - x) < 1e-6;

            return onPoint || Math.Sign(distanceFirstPoint) != Math.Sign(distanceSecondPoint);
        }

        /// <summary>
        /// Determines whether the <see cref="Segment2D"/> is vertical.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="Segment2D"/> is vertical. <c>false</c> otherwise.</returns>
        public bool IsVertical()
        {
            return Math.Abs(FirstPoint.X - SecondPoint.X) < 1e-6 && Math.Abs(FirstPoint.Y - SecondPoint.Y) >= 1e-6;
        }

        /// <summary>
        /// Gets the euclidean distance from this 2D line segment to some 2D point.
        /// </summary>
        /// <param name="point">The point to measure distance to.</param>
        /// <returns>A value of 0 or greater.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
        public double GetEuclideanDistanceToPoint(Point2D point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            Vector<double> segmentVector = SecondPoint - FirstPoint; // Vector from FirstPoint to SecondPoint
            Vector<double> orientationVector = point - FirstPoint; // Vector from FirstPoint to 'point'

            // Situation sketch, normalized along the segment:
            //   A  :   B  :  C
            // .....p1----p2......
            //      :      :
            // 1. Use numerator part of vector projection to determine relative location of 'point':
            double dotProductOrientationVector = segmentVector.DotProduct(orientationVector);
            if (dotProductOrientationVector <= 0)
            {
                // 'point' falls outside the perpendicular area defined by segment, specifically: Zone A
                return point.GetEuclideanDistanceTo(FirstPoint);
            }

            // 2. Use denominator part of vector projection to determine relative location of 'point':
            double dotProductSegmentVector = segmentVector.DotProduct(segmentVector);
            if (dotProductSegmentVector <= dotProductOrientationVector)
            {
                // 'point' falls outside the perpendicular area defined by segment, specifically: Zone C
                return point.GetEuclideanDistanceTo(SecondPoint);
            }

            // 'point' falls within the perpendicular area defined by the segment (zone B).
            // 3. Use remainder of vector projection to determine point on segment for perpendicular line:
            double projectionFactor = dotProductOrientationVector / dotProductSegmentVector;
            double perpendicularOnSegmentX = FirstPoint.X + projectionFactor * segmentVector[0];
            double perpendicularOnSegmentY = FirstPoint.Y + projectionFactor * segmentVector[1];
            var perpendicularLineIntersectionPoint = new Point2D(perpendicularOnSegmentX, perpendicularOnSegmentY);

            return point.GetEuclideanDistanceTo(perpendicularLineIntersectionPoint);
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

            return Equals((Segment2D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FirstPoint.X + SecondPoint.X).GetHashCode() * 397) ^ (FirstPoint.Y + SecondPoint.Y).GetHashCode();
            }
        }

        private bool Equals(Segment2D other)
        {
            return FirstPoint.Equals(other.FirstPoint) && SecondPoint.Equals(other.SecondPoint) ||
                   FirstPoint.Equals(other.SecondPoint) && SecondPoint.Equals(other.FirstPoint);
        }
    }
}