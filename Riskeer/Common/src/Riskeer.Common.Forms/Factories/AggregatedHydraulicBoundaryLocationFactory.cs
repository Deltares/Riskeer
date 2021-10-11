// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="AggregatedHydraulicBoundaryLocation"/> instances.
    /// </summary>
    public static class AggregatedHydraulicBoundaryLocationFactory
    {
        /// <summary>
        /// Creates the aggregated hydraulic boundary locations based on the locations and calculations.
        /// </summary>
        /// <param name="locations">The locations.</param>
        /// <param name="waterLevelCalculations">The water level calculations.</param>
        /// <param name="waveHeightCalculations">The wave height calculations.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="AggregatedHydraulicBoundaryLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<AggregatedHydraulicBoundaryLocation> CreateAggregatedHydraulicBoundaryLocations(
            IEnumerable<HydraulicBoundaryLocation> locations,
            IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> waterLevelCalculations,
            IReadOnlyDictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string> waveHeightCalculations)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            if (waterLevelCalculations == null)
            {
                throw new ArgumentNullException(nameof(waterLevelCalculations));
            }

            if (waveHeightCalculations == null)
            {
                throw new ArgumentNullException(nameof(waveHeightCalculations));
            }

            return locations.Select(location => new AggregatedHydraulicBoundaryLocation(
                                        location.Id, location.Name, location.Location,
                                        waterLevelCalculations.Select(c => new Tuple<string, RoundedDouble>(
                                                                          c.Value, GetCalculationResult(
                                                                              c.Key.Single(x => x.HydraulicBoundaryLocation
                                                                                                 .Equals(location))
                                                                               .Output)))
                                                              .ToArray(),
                                        waveHeightCalculations.Select(c => new Tuple<string, RoundedDouble>(
                                                                          c.Value, GetCalculationResult(
                                                                              c.Key.Single(x => x.HydraulicBoundaryLocation
                                                                                                 .Equals(location))
                                                                               .Output)))
                                                              .ToArray()))
                            .ToArray();
        }

        private static RoundedDouble GetCalculationResult(HydraulicBoundaryLocationCalculationOutput output)
        {
            return output?.Result ?? RoundedDouble.NaN;
        }
    }
}