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

using System.Collections;
using System.Collections.Generic;
using Core.Common.Base.Geometry;

namespace Core.Common.Base.TestUtil.Geometry
{
    /// <summary>
    /// This class compares the distance of two <see cref="Point2D"/> instances to determine
    /// whether they're equal to each other or not. This class shouldn't be used to sort point
    /// instances.
    /// </summary>
    public class Point2DComparerWithTolerance : IComparer<Point2D>, IComparer
    {
        private readonly double tolerance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point2DComparerWithTolerance"/> class.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        public Point2DComparerWithTolerance(double tolerance)
        {
            this.tolerance = tolerance;
        }

        public int Compare(object x, object y)
        {
            return Compare(x as Point2D, y as Point2D);
        }

        public int Compare(Point2D p0, Point2D p1)
        {
            double diff = p0.GetEuclideanDistanceTo(p1);
            return diff <= tolerance ? 0 : 1;
        }
    }
}