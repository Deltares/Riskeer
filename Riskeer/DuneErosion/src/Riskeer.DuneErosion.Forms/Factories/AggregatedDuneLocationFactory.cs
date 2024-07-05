// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base.Data;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.Views;

namespace Riskeer.DuneErosion.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="AggregatedDuneLocation"/> instances.
    /// </summary>
    public static class AggregatedDuneLocationFactory
    {
        /// <summary>
        /// Creates the aggregated dune locations based on the locations and calculations.
        /// </summary>
        /// <param name="duneLocations">The locations.</param>
        /// <param name="calculationsForTargetProbabilities">The calculations.</param>
        /// <returns>A collection of <see cref="AggregatedDuneLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<AggregatedDuneLocation> CreateAggregatedDuneLocations(
            IEnumerable<DuneLocation> duneLocations,
            IEnumerable<DuneLocationCalculationsForTargetProbability> calculationsForTargetProbabilities)
        {
            if (duneLocations == null)
            {
                throw new ArgumentNullException(nameof(duneLocations));
            }

            if (calculationsForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(calculationsForTargetProbabilities));
            }

            return duneLocations.Select(location =>
                                {
                                    Tuple<double, DuneLocationCalculation>[] calculationsForLocation =
                                        calculationsForTargetProbabilities.Select(c => new Tuple<double, DuneLocationCalculation>(
                                                                                      c.TargetProbability, c.DuneLocationCalculations.Single(
                                                                                          tp => tp.DuneLocation.Equals(location))))
                                                                          .ToArray();
                                    return new AggregatedDuneLocation(
                                        location.Id, location.Name, location.Location, location.CoastalAreaId, location.Offset,
                                        GetValues(calculationsForLocation, GetWaterLevel),
                                        GetValues(calculationsForLocation, GetWaveHeight),
                                        GetValues(calculationsForLocation, GetWavePeriod),
                                        GetValues(calculationsForLocation, GetMeanTidalAmplitude),
                                        GetValues(calculationsForLocation, GetWaveDirectionalSpread),
                                        GetValues(calculationsForLocation, GetTideSurgePhaseDifference));
                                })
                                .ToArray();
        }

        private static IEnumerable<Tuple<double, RoundedDouble>> GetValues(
            IEnumerable<Tuple<double, DuneLocationCalculation>> calculationsForLocation,
            Func<DuneLocationCalculation, RoundedDouble> getValueFunc)
        {
            return calculationsForLocation.Select(c => new Tuple<double, RoundedDouble>(c.Item1, getValueFunc(c.Item2)))
                                          .ToArray();
        }

        private static RoundedDouble GetWaterLevel(DuneLocationCalculation calculation)
        {
            return calculation.Output?.WaterLevel ?? RoundedDouble.NaN;
        }

        private static RoundedDouble GetWaveHeight(DuneLocationCalculation calculation)
        {
            return calculation.Output?.WaveHeight ?? RoundedDouble.NaN;
        }

        private static RoundedDouble GetWavePeriod(DuneLocationCalculation calculation)
        {
            return calculation.Output?.WavePeriod ?? RoundedDouble.NaN;
        }

        private static RoundedDouble GetMeanTidalAmplitude(DuneLocationCalculation calculation)
        {
            return calculation.Output?.MeanTidalAmplitude ?? RoundedDouble.NaN;
        }

        private static RoundedDouble GetWaveDirectionalSpread(DuneLocationCalculation calculation)
        {
            return calculation.Output?.WaveDirectionalSpread ?? RoundedDouble.NaN;
        }

        private static RoundedDouble GetTideSurgePhaseDifference(DuneLocationCalculation calculation)
        {
            return calculation.Output?.TideSurgePhaseDifference ?? RoundedDouble.NaN;
        }
    }
}