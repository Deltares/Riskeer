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

namespace Riskeer.Common.Util
{
    /// <summary>
    /// Class that holds all the information of a hydraulic boundary location and calculations.
    /// </summary>
    public class AggregatedHydraulicBoundaryLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="AggregatedHydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="id">The id of the hydraulic boundary location.</param>
        /// <param name="name">The name of the hydraulic boundary location.</param>
        /// <param name="location">The location of the hydraulic boundary location.</param>
        /// <param name="waterLevelCalculationForFactorizedSignalingNorm">The result of the
        /// water level calculation for the factorized signaling norm.</param>
        /// <param name="waterLevelCalculationForSignalingNorm">The result of the water level
        /// calculation for the signaling norm.</param>
        /// <param name="waterLevelCalculationForLowerLimitNorm">The result of the water level
        /// calculation for the lower limit norm.</param>
        /// <param name="waterLevelCalculationForFactorizedLowerLimitNorm">The result of the
        /// water level calculation for the factorized lower limit norm.</param>
        /// <param name="waveHeightCalculationForFactorizedSignalingNorm">The result of the
        /// wave height calculation for the factorized signaling norm.</param>
        /// <param name="waveHeightCalculationForSignalingNorm">The result of the wave height
        /// calculation for the signaling norm.</param>
        /// <param name="waveHeightCalculationForLowerLimitNorm">The result of the wave height
        /// calculation for the lower limit norm.</param>
        /// <param name="waveHeightCalculationForFactorizedLowerLimitNorm">The result of the
        /// wave height calculation for the factorized lower limit norm.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or 
        /// <paramref name="location"/> is <c>null</c>.</exception>
        public AggregatedHydraulicBoundaryLocation(
            long id, string name, Point2D location,
            RoundedDouble waterLevelCalculationForFactorizedSignalingNorm,
            RoundedDouble waterLevelCalculationForSignalingNorm,
            RoundedDouble waterLevelCalculationForLowerLimitNorm,
            RoundedDouble waterLevelCalculationForFactorizedLowerLimitNorm,
            RoundedDouble waveHeightCalculationForFactorizedSignalingNorm,
            RoundedDouble waveHeightCalculationForSignalingNorm,
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
            WaterLevelCalculationForFactorizedSignalingNorm = waterLevelCalculationForFactorizedSignalingNorm;
            WaterLevelCalculationForSignalingNorm = waterLevelCalculationForSignalingNorm;
            WaterLevelCalculationForLowerLimitNorm = waterLevelCalculationForLowerLimitNorm;
            WaterLevelCalculationForFactorizedLowerLimitNorm = waterLevelCalculationForFactorizedLowerLimitNorm;
            WaveHeightCalculationForFactorizedSignalingNorm = waveHeightCalculationForFactorizedSignalingNorm;
            WaveHeightCalculationForSignalingNorm = waveHeightCalculationForSignalingNorm;
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
        /// Gets the result of the water level calculation for the factorized signaling norm.
        /// </summary>
        public RoundedDouble WaterLevelCalculationForFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the water level calculation for the signaling norm.
        /// </summary>
        public RoundedDouble WaterLevelCalculationForSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the water level calculation for the lower limit norm.
        /// </summary>
        public RoundedDouble WaterLevelCalculationForLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the water level calculation for the factorized lower limit norm.
        /// </summary>
        public RoundedDouble WaterLevelCalculationForFactorizedLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave height calculation for the factorized signaling norm.
        /// </summary>
        public RoundedDouble WaveHeightCalculationForFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the wave height calculation for the signaling norm.
        /// </summary>
        public RoundedDouble WaveHeightCalculationForSignalingNorm { get; }

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