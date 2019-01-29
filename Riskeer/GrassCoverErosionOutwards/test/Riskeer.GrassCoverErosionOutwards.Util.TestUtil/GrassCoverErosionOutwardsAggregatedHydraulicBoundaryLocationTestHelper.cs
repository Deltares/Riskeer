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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;

namespace Riskeer.GrassCoverErosionOutwards.Util.TestUtil
{
    /// <summary>
    /// Helper for creating <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/>
    /// instances that can be used in tests.
    /// </summary>
    public static class GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationTestHelper
    {
        /// <summary>
        /// Creates a new <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/>
        /// with random output.
        /// </summary>
        /// <returns>The created <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/>.</returns>
        public static GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation Create()
        {
            var random = new Random(39);
            return Create(random.NextRoundedDouble(), random.NextRoundedDouble(),
                          random.NextRoundedDouble(), random.NextRoundedDouble(),
                          random.NextRoundedDouble(), random.NextRoundedDouble(),
                          random.NextRoundedDouble(), random.NextRoundedDouble(),
                          random.NextRoundedDouble(), random.NextRoundedDouble());
        }

        /// <summary>
        /// Creates a new <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/>
        /// with given output.
        /// </summary>
        /// <param name="waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm">The water level calculation for mechanism specific factorized signaling norm.</param>
        /// <param name="waterLevelCalculationForMechanismSpecificSignalingNorm">The water level calculation for mechanism specific signaling norm.</param>
        /// <param name="waterLevelCalculationForMechanismSpecificLowerLimitNorm">The water level calculation for mechanism specific lower limit norm.</param>
        /// <param name="waterLevelCalculationForLowerLimitNorm">The water level calculation for lower limit norm.</param>
        /// <param name="waterLevelCalculationForFactorizedLowerLimitNorm">The water level calculation for factorized lower limit norm.</param>
        /// <param name="waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm">The wave height calculation for mechanism specific factorized signaling norm.</param>
        /// <param name="waveHeightCalculationForMechanismSpecificSignalingNorm">The wave height calculation for mechanism specific signaling norm.</param>
        /// <param name="waveHeightCalculationForMechanismSpecificLowerLimitNorm">The wave height calculation for mechanism specific lower limit norm.</param>
        /// <param name="waveHeightCalculationForLowerLimitNorm">The wave height calculation for lower limit norm.</param>
        /// <param name="waveHeightCalculationForFactorizedLowerLimitNorm">The wave height calculation for factorized lower limit norm.</param>
        /// <returns>The created <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/>.</returns>
        public static GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation Create(
            RoundedDouble waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm,
            RoundedDouble waterLevelCalculationForMechanismSpecificSignalingNorm,
            RoundedDouble waterLevelCalculationForMechanismSpecificLowerLimitNorm,
            RoundedDouble waterLevelCalculationForLowerLimitNorm,
            RoundedDouble waterLevelCalculationForFactorizedLowerLimitNorm,
            RoundedDouble waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm,
            RoundedDouble waveHeightCalculationForMechanismSpecificSignalingNorm,
            RoundedDouble waveHeightCalculationForMechanismSpecificLowerLimitNorm,
            RoundedDouble waveHeightCalculationForLowerLimitNorm,
            RoundedDouble waveHeightCalculationForFactorizedLowerLimitNorm)
        {
            return new GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation(
                1, "test", new Point2D(0, 0),
                waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm,
                waterLevelCalculationForMechanismSpecificSignalingNorm,
                waterLevelCalculationForMechanismSpecificLowerLimitNorm,
                waterLevelCalculationForLowerLimitNorm,
                waterLevelCalculationForFactorizedLowerLimitNorm,
                waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm,
                waveHeightCalculationForMechanismSpecificSignalingNorm,
                waveHeightCalculationForMechanismSpecificLowerLimitNorm,
                waveHeightCalculationForLowerLimitNorm,
                waveHeightCalculationForFactorizedLowerLimitNorm);
        }
    }
}