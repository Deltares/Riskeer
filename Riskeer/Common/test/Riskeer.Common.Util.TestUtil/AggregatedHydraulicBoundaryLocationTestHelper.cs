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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;

namespace Riskeer.Common.Util.TestUtil
{
    /// <summary>
    /// Helper for creating <see cref="AggregatedHydraulicBoundaryLocation"/>
    /// instances that can be used in tests.
    /// </summary>
    public static class AggregatedHydraulicBoundaryLocationTestHelper
    {
        /// <summary>
        /// Creates a new <see cref="AggregatedHydraulicBoundaryLocation"/> with random output.
        /// </summary>
        /// <returns>The created <see cref="AggregatedHydraulicBoundaryLocation"/>.</returns>
        public static AggregatedHydraulicBoundaryLocation Create()
        {
            var random = new Random(39);
            return Create(random.NextRoundedDouble(), random.NextRoundedDouble(),
                          random.NextRoundedDouble(), random.NextRoundedDouble(),
                          random.NextRoundedDouble(), random.NextRoundedDouble(),
                          random.NextRoundedDouble(), random.NextRoundedDouble(),
                          new[]
                          {
                              new Tuple<double, RoundedDouble>(random.NextDouble(), random.NextRoundedDouble())
                          },
                          new[]
                          {
                              new Tuple<double, RoundedDouble>(random.NextDouble(), random.NextRoundedDouble())
                          });
        }

        private static AggregatedHydraulicBoundaryLocation Create(
            RoundedDouble waterLevelCalculationForFactorizedSignalingNorm,
            RoundedDouble waterLevelCalculationForSignalingNorm,
            RoundedDouble waterLevelCalculationForLowerLimitNorm,
            RoundedDouble waterLevelCalculationForFactorizedLowerLimitNorm,
            RoundedDouble waveHeightCalculationForFactorizedSignalingNorm,
            RoundedDouble waveHeightCalculationForSignalingNorm,
            RoundedDouble waveHeightCalculationForLowerLimitNorm,
            RoundedDouble waveHeightCalculationForFactorizedLowerLimitNorm,
            IEnumerable<Tuple<double, RoundedDouble>> waterLevelCalculationForTargetProbabilities,
            IEnumerable<Tuple<double, RoundedDouble>> waveHeightCalculationForTargetProbabilities)
        {
            return new AggregatedHydraulicBoundaryLocation(
                1, "test", new Point2D(0, 0),
                waterLevelCalculationForFactorizedSignalingNorm,
                waterLevelCalculationForSignalingNorm,
                waterLevelCalculationForLowerLimitNorm,
                waterLevelCalculationForFactorizedLowerLimitNorm,
                waveHeightCalculationForFactorizedSignalingNorm,
                waveHeightCalculationForSignalingNorm,
                waveHeightCalculationForLowerLimitNorm,
                waveHeightCalculationForFactorizedLowerLimitNorm,
                waterLevelCalculationForTargetProbabilities,
                waveHeightCalculationForTargetProbabilities);
        }
    }
}