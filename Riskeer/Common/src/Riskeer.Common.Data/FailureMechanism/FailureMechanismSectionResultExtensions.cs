// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Extension methods for <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    public static class FailureMechanismSectionResultExtensions
    {
        /// <summary>
        /// Gets a collection of the relevant <typeparamref name="T"/>.
        /// </summary>
        /// <param name="sectionResult">The section result to get the relevant scenarios for.</param>
        /// <param name="calculationScenarios">The calculation scenarios to get the relevant scenarios from.</param>
        /// <param name="intersectionFunc">The function to determine whether a scenario is belonging to the given <paramref name="sectionResult"/>.</param>
        /// <typeparam name="T">The type of the calculation scenarios.</typeparam>
        /// <returns>A collection of relevant calculation scenarios.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<T> GetRelevantCalculationScenarios<T>(this FailureMechanismSectionResult sectionResult,
                                                                        IEnumerable<ICalculationScenario> calculationScenarios,
                                                                        Func<T, IEnumerable<Segment2D>, bool> intersectionFunc)
            where T : ICalculationScenario
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (calculationScenarios == null)
            {
                throw new ArgumentNullException(nameof(calculationScenarios));
            }

            if (intersectionFunc == null)
            {
                throw new ArgumentNullException(nameof(intersectionFunc));
            }

            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(sectionResult.Section.Points);

            return calculationScenarios.OfType<T>().Where(scenario => scenario.IsRelevant && intersectionFunc(scenario, lineSegments));
        }
    }
}