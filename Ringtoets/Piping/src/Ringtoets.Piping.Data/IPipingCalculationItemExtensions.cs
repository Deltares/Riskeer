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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="IPipingCalculationItem"/> instances.
    /// </summary>
    public static class IPipingCalculationItemExtensions
    {
        /// <summary>
        /// Recursively enumerates across over the contents of the piping calculation item, 
        /// yielding the piping calculations found within the calculation item.
        /// </summary>
        /// <param name="pipingCalculationItem">The calculation item to be evaluated.</param>
        /// <returns>Returns all contained piping calculations as an enumerable result.</returns>
        public static IEnumerable<PipingCalculationScenario> GetPipingCalculations(this IPipingCalculationItem pipingCalculationItem)
        {
            var calculation = pipingCalculationItem as PipingCalculationScenario;
            if (calculation != null)
            {
                yield return calculation;
            }
            var group = pipingCalculationItem as PipingCalculationGroup;
            if (group != null)
            {
                foreach (PipingCalculationScenario calculationInGroup in group.Children.GetPipingCalculations())
                {
                    yield return calculationInGroup;
                }
            }
        }

        /// <summary>
        /// Recursively enumerates across over the contents of all the piping calculation 
        /// items, yielding the piping calculations found within those calculation items.
        /// </summary>
        /// <param name="pipingCalculationItems">The calculation items to be evaluated.</param>
        /// <returns>Returns all contained piping calculations as an enumerable result.</returns>
        public static IEnumerable<PipingCalculationScenario> GetPipingCalculations(this IEnumerable<IPipingCalculationItem> pipingCalculationItems)
        {
            return pipingCalculationItems.SelectMany(GetPipingCalculations);
        }

        /// <summary>
        /// Determines if the surfaceline of a calculation is intersecting with the section reference line.
        /// </summary>
        /// <param name="pipingCalculationItem">The calculation containing the surface line.</param>
        /// <param name="lineSegments">The line segments that defines the reference line.</param>
        /// <returns><c>true</c> when intersecting. <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="lineSegments"/> contains no elements.</exception>
        public static bool IsSurfaceLineIntersectionWithReferenceLineInSection(this IPipingCalculationItem pipingCalculationItem, IEnumerable<Segment2D> lineSegments)
        {
            var pipingCalculation = pipingCalculationItem as PipingCalculationScenario;

            if (pipingCalculation == null)
            {
                return false;
            }

            var surfaceLine = pipingCalculation.InputParameters.SurfaceLine;
            if (surfaceLine == null)
            {
                return false;
            }
            var minimalDistance = lineSegments.Min(segment => segment.GetEuclideanDistanceToPoint(surfaceLine.ReferenceLineIntersectionWorldPoint));
            return minimalDistance < 1e-6;
        }
    }
}