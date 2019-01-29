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
    /// Extension methods for collections of <see cref="Point2D"/>.
    /// </summary>
    public static class Point2DCollectionExtensions
    {
        /// <summary>
        /// Checks whether the <paramref name="points"/> form a reclining geometry. 
        /// That is, given a point from the geometry with an X-coordinate, 
        /// has a point further in the geometry that has an X-coordinate smaller 
        /// than the X-coordinate of the given point.
        /// </summary>
        /// <param name="points">The points forming a line to check.</param>
        /// <returns><c>true</c> if the line is reclining; <c>false</c>
        /// otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/>
        /// is <c>null</c>.</exception>
        public static bool IsReclining(this IEnumerable<Point2D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            double[] lCoordinates = points.Select(p => p.X).ToArray();
            for (var i = 1; i < lCoordinates.Length; i++)
            {
                if (lCoordinates[i - 1] > lCoordinates[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
}