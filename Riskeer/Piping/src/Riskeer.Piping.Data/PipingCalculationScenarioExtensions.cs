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
using Core.Common.Base.Geometry;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="PipingCalculationScenario"/> instances.
    /// </summary>
    public static class PipingCalculationScenarioExtensions
    {
        /// <summary>
        /// Determines if the surface line of a calculation is intersecting with the section reference line.
        /// </summary>
        /// <param name="pipingCalculationScenario">The piping calculation scenario containing the surface line.</param>
        /// <param name="lineSegments">The line segments that define the reference line.</param>
        /// <returns><c>true</c> when intersecting. <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="lineSegments"/> contains no elements.</exception>
        public static bool IsSurfaceLineIntersectionWithReferenceLineInSection(this PipingCalculationScenario pipingCalculationScenario, IEnumerable<Segment2D> lineSegments)
        {
            PipingSurfaceLine surfaceLine = pipingCalculationScenario?.InputParameters.SurfaceLine;
            if (surfaceLine == null)
            {
                return false;
            }

            double minimalDistance = lineSegments.Min(segment => segment.GetEuclideanDistanceToPoint(surfaceLine.ReferenceLineIntersectionWorldPoint));
            return minimalDistance < 1e-6;
        }
    }
}