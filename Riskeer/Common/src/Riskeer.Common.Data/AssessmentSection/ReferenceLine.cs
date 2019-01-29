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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Riskeer.Common.Data.AssessmentSection
{
    /// <summary>
    /// Class representing the reference line used as a basis for assessment.
    /// </summary>
    public class ReferenceLine : Observable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLine"/> class with no coordinate points.
        /// </summary>
        public ReferenceLine()
        {
            Points = new Point2D[0];
        }

        /// <summary>
        /// Gets the 2D points describing the geometry of the reference line.
        /// </summary>
        public IEnumerable<Point2D> Points { get; private set; }

        /// <summary>
        /// Gets the total length of the reference line.
        /// [m]
        /// </summary>
        public double Length { get; private set; }

        /// <summary>
        /// Sets the geometry of the reference line.
        /// </summary>
        /// <param name="newPoints">The sequence of points defining the reference line geometry.</param>
        /// <exception cref="ArgumentException">Thrown when any element of <paramref name="newPoints"/> is <c>null</c>.</exception>
        public void SetGeometry(IEnumerable<Point2D> newPoints)
        {
            if (newPoints == null)
            {
                throw new ArgumentNullException(nameof(newPoints), RingtoetsCommonDataResources.ReferenceLine_SetGeometry_New_geometry_cannot_be_null);
            }

            Point2D[] point2Ds = newPoints.ToArray();
            if (point2Ds.Any(p => p == null))
            {
                throw new ArgumentException(RingtoetsCommonDataResources.ReferenceLine_SetGeometry_New_geometry_has_null_coordinate, nameof(newPoints));
            }

            Points = point2Ds;
            Length = Math2D.Length(Points);
        }
    }
}