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
using Core.Common.Base.Data;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.Views;

namespace Riskeer.DuneErosion.Forms
{
    /// <summary>
    /// Factory for creating <see cref="AggregatedDuneLocation"/> instances.
    /// </summary>
    public static class AggregatedDuneLocationFactory
    {
        /// <summary>
        /// Creates the aggregated dune locations based on the locations and calculations
        /// from the failure mechanism.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to get the locations and calculations from.</param>
        /// <returns>A collection of <see cref="AggregatedDuneLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public static IEnumerable<AggregatedDuneLocation> CreateAggregatedDuneLocations(DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            Dictionary<DuneLocation, DuneLocationCalculation> duneLocationCalculationsForMechanismSpecificFactorizedSignalingNormLookup =
                failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.ToDictionary(calc => calc.DuneLocation,
                                                                                                      calc => calc);

            Dictionary<DuneLocation, DuneLocationCalculation> duneLocationCalculationsForMechanismSpecificSignalingNormLookup =
                failureMechanism.CalculationsForMechanismSpecificSignalingNorm.ToDictionary(calc => calc.DuneLocation,
                                                                                            calc => calc);

            Dictionary<DuneLocation, DuneLocationCalculation> duneLocationCalculationsForMechanismSpecificLowerLimitNormLookup =
                failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.ToDictionary(calc => calc.DuneLocation,
                                                                                             calc => calc);

            Dictionary<DuneLocation, DuneLocationCalculation> duneLocationCalculationsForLowerLimitNormLookup =
                failureMechanism.CalculationsForLowerLimitNorm.ToDictionary(calc => calc.DuneLocation,
                                                                            calc => calc);

            Dictionary<DuneLocation, DuneLocationCalculation> duneLocationCalculationsForFactorizedLowerLimitNormLookup =
                failureMechanism.CalculationsForFactorizedLowerLimitNorm.ToDictionary(calc => calc.DuneLocation,
                                                                                      calc => calc);

            return failureMechanism.DuneLocations
                                   .Select(location => new AggregatedDuneLocation(
                                               location.Id, location.Name, location.Location, location.CoastalAreaId, location.Offset, location.D50,
                                               GetWaterLevel(duneLocationCalculationsForMechanismSpecificFactorizedSignalingNormLookup[location]),
                                               GetWaterLevel(duneLocationCalculationsForMechanismSpecificSignalingNormLookup[location]),
                                               GetWaterLevel(duneLocationCalculationsForMechanismSpecificLowerLimitNormLookup[location]),
                                               GetWaterLevel(duneLocationCalculationsForLowerLimitNormLookup[location]),
                                               GetWaterLevel(duneLocationCalculationsForFactorizedLowerLimitNormLookup[location]),
                                               GetWaveHeight(duneLocationCalculationsForMechanismSpecificFactorizedSignalingNormLookup[location]),
                                               GetWaveHeight(duneLocationCalculationsForMechanismSpecificSignalingNormLookup[location]),
                                               GetWaveHeight(duneLocationCalculationsForMechanismSpecificLowerLimitNormLookup[location]),
                                               GetWaveHeight(duneLocationCalculationsForLowerLimitNormLookup[location]),
                                               GetWaveHeight(duneLocationCalculationsForFactorizedLowerLimitNormLookup[location]),
                                               GetWavePeriod(duneLocationCalculationsForMechanismSpecificFactorizedSignalingNormLookup[location]),
                                               GetWavePeriod(duneLocationCalculationsForMechanismSpecificSignalingNormLookup[location]),
                                               GetWavePeriod(duneLocationCalculationsForMechanismSpecificLowerLimitNormLookup[location]),
                                               GetWavePeriod(duneLocationCalculationsForLowerLimitNormLookup[location]),
                                               GetWavePeriod(duneLocationCalculationsForFactorizedLowerLimitNormLookup[location])))
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
    }
}