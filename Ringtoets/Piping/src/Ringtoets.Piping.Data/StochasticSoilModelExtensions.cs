﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Extension methods dealing with <see cref="StochasticSoilModel"/> instances.
    /// </summary>
    public static class StochasticSoilModelExtensions
    {
        /// <summary>
        /// Indicates whether a stochastic soil model intersects with a surface line.
        /// </summary>
        /// <param name="stochasticSoilModel">The stochastic soil model used to match a surface line.</param>
        /// <param name="surfaceLine">The surface line used to match a stochastic soil model.</param>
        /// <returns><c>true</c> when the <paramref name="stochasticSoilModel"/> intersects with the <paramref name="surfaceLine"/>;
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static bool IntersectsWithSurfaceLineGeometry(this StochasticSoilModel stochasticSoilModel,
                                                             RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (stochasticSoilModel == null)
            {
                throw new ArgumentNullException(nameof(stochasticSoilModel));
            }
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            Segment2D[] surfaceLineSegments = Math2D.ConvertLinePointsToLineSegments(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y))).ToArray();
            return DoesSoilModelGeometryIntersectWithSurfaceLineGeometry(stochasticSoilModel, surfaceLineSegments);
        }

        private static bool DoesSoilModelGeometryIntersectWithSurfaceLineGeometry(StochasticSoilModel stochasticSoilModel, Segment2D[] surfaceLineSegments)
        {
            IEnumerable<Segment2D> soilProfileGeometrySegments = Math2D.ConvertLinePointsToLineSegments(stochasticSoilModel.Geometry);
            return soilProfileGeometrySegments.Any(s => DoesSegmentIntersectWithSegmentArray(s, surfaceLineSegments));
        }

        private static bool DoesSegmentIntersectWithSegmentArray(Segment2D segment, Segment2D[] segmentArray)
        {
            // Consider intersections and overlaps similarly
            return segmentArray.Any(sls => Math2D.GetIntersectionBetweenSegments(segment, sls).IntersectionType != Intersection2DType.DoesNotIntersect);
        }
    }
}