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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Riskeer.GrassCoverErosionOutwards.Util
{
    /// <summary>
    /// Class that holds all the information of a hydraulic boundary location and calculations
    /// for grass cover erosion outwards.
    /// </summary>
    public class GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="id">The id of the hydraulic boundary location.</param>
        /// <param name="name">The name of the hydraulic boundary location.</param>
        /// <param name="location">The location of the hydraulic boundary location.</param>
        /// <param name="waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm">The result of the
        /// water level calculation for the mechanism specific factorized signaling norm.</param>
        /// <param name="waterLevelCalculationForMechanismSpecificSignalingNorm">The result of the water level
        /// calculation for the mechanism specific signaling norm.</param>
        /// <param name="waterLevelCalculationForMechanismSpecificLowerLimitNorm">The result of the water level
        /// calculation for the mechanism specific lower limit norm.</param>
        /// <param name="waterLevelCalculationForLowerLimitNorm">The result of the water level calculation for
        /// the lower limit norm.</param>
        /// <param name="waterLevelCalculationForFactorizedLowerLimitNorm">The result of the water level
        /// calculation for the factorized lower limit norm.</param>
        /// <param name="waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm">The result of the
        /// wave height level calculation for the mechanism specific factorized signaling norm.</param>
        /// <param name="waveHeightCalculationForMechanismSpecificSignalingNorm">The result of the wave height
        /// calculation for the mechanism specific signaling norm.</param>
        /// <param name="waveHeightCalculationForMechanismSpecificLowerLimitNorm">The result of the wave height
        /// calculation for the mechanism specific lower limit norm.</param>
        /// <param name="waveHeightCalculationForLowerLimitNorm">The result of the wave height calculation for
        /// the lower limit norm.</param>
        /// <param name="waveHeightCalculationForFactorizedLowerLimitNorm">The result of the wave height
        /// calculation for the factorized lower limit norm.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or 
        /// <paramref name="location"/> is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation(
            long id, string name, Point2D location,
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
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            Id = id;
            Name = name;
            Location = location;
            WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm = waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm;
            WaterLevelCalculationForMechanismSpecificSignalingNorm = waterLevelCalculationForMechanismSpecificSignalingNorm;
            WaterLevelCalculationForMechanismSpecificLowerLimitNorm = waterLevelCalculationForMechanismSpecificLowerLimitNorm;
            WaterLevelCalculationForLowerLimitNorm = waterLevelCalculationForLowerLimitNorm;
            WaterLevelCalculationForFactorizedLowerLimitNorm = waterLevelCalculationForFactorizedLowerLimitNorm;
            WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm = waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm;
            WaveHeightCalculationForMechanismSpecificSignalingNorm = waveHeightCalculationForMechanismSpecificSignalingNorm;
            WaveHeightCalculationForMechanismSpecificLowerLimitNorm = waveHeightCalculationForMechanismSpecificLowerLimitNorm;
            WaveHeightCalculationForLowerLimitNorm = waveHeightCalculationForLowerLimitNorm;
            WaveHeightCalculationForFactorizedLowerLimitNorm = waveHeightCalculationForFactorizedLowerLimitNorm;
        }

        /// <summary>
        /// Gets the id of the hydraulic boundary location.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the name of the hydraulic boundary location.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the location of the hydraulic boundary location.
        /// </summary>
        public Point2D Location { get; }

        /// <summary>
        /// Gets the result of the water level calculation for the mechanism specific
        /// factorized signaling norm.
        /// </summary>
        public RoundedDouble WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the water level calculation for the mechanism specific
        /// signaling norm.
        /// </summary>
        public RoundedDouble WaterLevelCalculationForMechanismSpecificSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the water level calculation for the mechanism specific
        /// lower limit norm.
        /// </summary>
        public RoundedDouble WaterLevelCalculationForMechanismSpecificLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the water level calculation for the lower limit norm.
        /// </summary>
        public RoundedDouble WaterLevelCalculationForLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the water level calculation for the factorized lower limit norm.
        /// </summary>
        public RoundedDouble WaterLevelCalculationForFactorizedLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave height calculation for the mechanism specific
        /// factorized signaling norm.
        /// </summary>
        public RoundedDouble WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the wave height calculation for the mechanism specific
        /// signaling norm.
        /// </summary>
        public RoundedDouble WaveHeightCalculationForMechanismSpecificSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the wave height calculation for the mechanism specific
        /// lower limit norm.
        /// </summary>
        public RoundedDouble WaveHeightCalculationForMechanismSpecificLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave height calculation for the lower limit norm.
        /// </summary>
        public RoundedDouble WaveHeightCalculationForLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave height calculation for the factorized lower limit norm.
        /// </summary>
        public RoundedDouble WaveHeightCalculationForFactorizedLowerLimitNorm { get; }
    }
}