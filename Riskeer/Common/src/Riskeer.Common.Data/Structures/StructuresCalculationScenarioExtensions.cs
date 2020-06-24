// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.Common.Data.Structures
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="StructuresCalculationScenario{T}"/> instances.
    /// </summary>
    public static class StructuresCalculationScenarioExtensions
    {
        /// <summary>
        /// Determines if the structure of a calculation is intersecting with the section reference line.
        /// </summary>
        /// <param name="calculationScenario">The calculation scenario containing the structure.</param>
        /// <param name="lineSegments">The line segments that define the reference line.</param>
        /// <returns><c>true</c> when intersecting. <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="lineSegments"/> contains no elements.</exception>
        public static bool IsStructureIntersectionWithReferenceLineInSection<TStructuresInput>(this StructuresCalculationScenario<TStructuresInput> calculationScenario,
                                                                                               IEnumerable<Segment2D> lineSegments)
            where TStructuresInput : IStructuresCalculationInput<StructureBase>, new()
        {
            if (calculationScenario == null)
            {
                throw new ArgumentNullException(nameof(calculationScenario));
            }

            if (lineSegments == null)
            {
                throw new ArgumentNullException(nameof(lineSegments));
            }

            StructureBase structure = calculationScenario.InputParameters.Structure;
            if (structure == null)
            {
                return false;
            }

            return lineSegments.Min(segment => segment.GetEuclideanDistanceToPoint(structure.Location)) <= 1;
        }
    }
}
