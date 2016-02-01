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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class represents lines between two <see cref="Point2D"/>.
    /// </summary>
    public class Segment2D
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
                throw new ArgumentNullException("first",Resources.Segment2D_Constructor_Segment_must_be_created_with_two_points);
            }
            if (second == null)
            {
                throw new ArgumentNullException("second",Resources.Segment2D_Constructor_Segment_must_be_created_with_two_points);
            }
            FirstPoint = first;
            SecondPoint = second;
        }

        /// <summary>
        /// The first <see cref="Point2D"/> of the <see cref="Segment2D"/>.
        /// </summary>
        public Point2D FirstPoint { get; private set; }

        /// <summary>
        /// The second <see cref="Point2D"/> of the <see cref="Segment2D"/>.
        /// </summary>
        public Point2D SecondPoint { get; private set; }

        /// <summary>
        /// This method determines whether <paramref name="x"/> is contained by the <see cref="FirstPoint"/>
        /// and <see cref="SecondPoint"/> x coordinates.
        /// </summary>
        /// <param name="x">The x for which to find out whether it is contained by the <see cref="FirstPoint"/>
        /// and <see cref="SecondPoint"/>.</param>
        /// <returns><c>true</c> if x is on or between the points' x coordinates. <c>false</c> otherwise.</returns>
        public bool ContainsX(double x)
        {
            var distanceFirstPoint = FirstPoint.X - x;
            var distanceSecondPoint = SecondPoint.X - x;

            var onPoint = Math.Abs(FirstPoint.X - x) < 1e-8 || Math.Abs(SecondPoint.X - x) < 1e-8;

            return onPoint || Math.Sign(distanceFirstPoint) != Math.Sign(distanceSecondPoint);
        }

        /// <summary>
        /// Determines whether the <see cref="Segment2D"/> is vertical.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="Segment2D"/> is vertical. <c>false</c> otherwise.</returns>
        public bool IsVertical()
        {
            return Math.Abs(FirstPoint.X - SecondPoint.X) < 1e-8 && Math.Abs(FirstPoint.Y - SecondPoint.Y) >= 1e-8;
        }

        /// <summary>
        /// Determines whether two segments are connected by each other's <see cref="FirstPoint"/>
        /// and <see cref="SecondPoint"/>.
        /// </summary>
        /// <param name="segment">The segment which may be connected to the <see cref="Segment2D"/>.</param>
        /// <returns><c>true</c> if the segments are connected. <c>false</c> otherwise.</returns>
        public bool IsConnected(Segment2D segment)
        {
            return
                FirstPoint.Equals(segment.FirstPoint) ||
                FirstPoint.Equals(segment.SecondPoint) ||
                SecondPoint.Equals(segment.FirstPoint) ||
                SecondPoint.Equals(segment.SecondPoint);
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
                return ((FirstPoint.X + SecondPoint.X).GetHashCode()*397) ^ (FirstPoint.Y + SecondPoint.Y).GetHashCode();
            }
        }

        private bool Equals(Segment2D other)
        {
            return FirstPoint.Equals(other.FirstPoint) && SecondPoint.Equals(other.SecondPoint) ||
                   FirstPoint.Equals(other.SecondPoint) && SecondPoint.Equals(other.FirstPoint);
        }
    }
}