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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// This class provides a collection of <see cref="Point2D"/> with <see cref="RoundedDouble"/> coordinates.
    /// </summary>
    public class RoundedPoint2DCollection : IEnumerable<Point2D>
    {
        private readonly IEnumerable<Point2D> points;

        /// <summary>
        /// Creates a new instance of <see cref="RoundedPoint2DCollection"/>.
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places for the coordinates.</param>
        /// <param name="originalPoints">The original collection of <see cref="Point2D"/> to round 
        /// the coordinates for.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when 
        /// <paramref name="numberOfDecimalPlaces"/> is not in range [0, 15].</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="originalPoints"/>
        /// is <c>null</c>.</exception>
        public RoundedPoint2DCollection(int numberOfDecimalPlaces, IEnumerable<Point2D> originalPoints)
        {
            if (originalPoints == null)
            {
                throw new ArgumentNullException(nameof(originalPoints));
            }

            if (numberOfDecimalPlaces < 0 || numberOfDecimalPlaces > RoundedDouble.MaximumNumberOfDecimalPlaces)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfDecimalPlaces), @"Value must be in range [0, 15].");
            }

            points = originalPoints.Select(p => new Point2D(new RoundedDouble(numberOfDecimalPlaces, p.X),
                                                            new RoundedDouble(numberOfDecimalPlaces, p.Y)))
                                   .ToArray();

            NumberOfDecimalPlaces = numberOfDecimalPlaces;
        }

        /// <summary>
        /// Gets the number of decimal places of the <see cref="Point2D"/> coordinates.
        /// </summary>
        public int NumberOfDecimalPlaces { get; }

        public IEnumerator<Point2D> GetEnumerator()
        {
            return points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}