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

namespace Ringtoets.DuneErosion.Forms.Views
{
    /// <summary>
    /// Class that holds all the information of a dune location and calculations.
    /// </summary>
    public class AggregatedDuneLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="AggregatedDuneLocation"/>.
        /// </summary>
        /// <param name="id">The id of the dune location.</param>
        /// <param name="name">The name of the dune location.</param>
        /// <param name="location">The location of the dune location</param>
        /// <param name="coastalAreaId">The coastal area id of the dune location.</param>
        /// <param name="offset">The offset of the dune location.</param>
        /// <param name="d50">The d50 of the dune location.</param>
        /// <param name="waterLevelForMechanismSpecificFactorizedSignalingNorm">The result of the water level calculation 
        /// for the mechanism specific factorized signaling norm.</param>
        /// <param name="waterLevelForMechanismSpecificSignalingNorm">The result of the water level calculation 
        /// for the mechanism specific signaling norm.</param>
        /// <param name="waterLevelForMechanismSpecificLowerLimitNorm">The result of the water level calculation
        /// for the mechanism specific lower limit norm.</param>
        /// <param name="waterLevelForLowerLimitNorm">The result of the water level calculation 
        /// for the lower limit norm.</param>
        /// <param name="waterLevelForFactorizedLowerLimitNorm">The result of the water level calculation 
        /// for the factorized lower limit norm.</param>
        /// <param name="waveHeightForMechanismSpecificFactorizedSignalingNorm">The result of the wave height calculation 
        /// for the mechanism specific factorized signaling norm.</param>
        /// <param name="waveHeightForMechanismSpecificSignalingNorm">The result of the wave height calculation 
        /// for the mechanism specific signaling norm.</param>
        /// <param name="waveHeightForMechanismSpecificLowerLimitNorm">The result of the wave height calculation
        /// for the mechanism specific lower limit norm.</param>
        /// <param name="waveHeightForLowerLimitNorm">The result of the wave height calculation 
        /// for the lower limit norm.</param>
        /// <param name="waveHeightForFactorizedLowerLimitNorm">The result of the wave height calculation 
        /// for the factorized lower limit norm.</param>
        /// <param name="wavePeriodForMechanismSpecificFactorizedSignalingNorm">The result of the wave period calculation 
        /// for the mechanism specific factorized signaling norm.</param>
        /// <param name="wavePeriodForMechanismSpecificSignalingNorm">The result of the wave period calculation 
        /// for the mechanism specific signaling norm.</param>
        /// <param name="wavePeriodForMechanismSpecificLowerLimitNorm">The result of the wave period calculation
        /// for the mechanism specific lower limit norm.</param>
        /// <param name="wavePeriodForLowerLimitNorm">The result of the wave period calculation 
        /// for the lower limit norm.</param>
        /// <param name="wavePeriodForFactorizedLowerLimitNorm">The result of the wave period calculation 
        /// for the factorized lower limit norm.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or 
        /// <paramref name="location"/> is <c>null</c>.</exception>
        public AggregatedDuneLocation(long id, string name, Point2D location, int coastalAreaId, RoundedDouble offset, RoundedDouble d50,
                                      RoundedDouble waterLevelForMechanismSpecificFactorizedSignalingNorm,
                                      RoundedDouble waterLevelForMechanismSpecificSignalingNorm,
                                      RoundedDouble waterLevelForMechanismSpecificLowerLimitNorm,
                                      RoundedDouble waterLevelForLowerLimitNorm,
                                      RoundedDouble waterLevelForFactorizedLowerLimitNorm,
                                      RoundedDouble waveHeightForMechanismSpecificFactorizedSignalingNorm,
                                      RoundedDouble waveHeightForMechanismSpecificSignalingNorm,
                                      RoundedDouble waveHeightForMechanismSpecificLowerLimitNorm,
                                      RoundedDouble waveHeightForLowerLimitNorm,
                                      RoundedDouble waveHeightForFactorizedLowerLimitNorm,
                                      RoundedDouble wavePeriodForMechanismSpecificFactorizedSignalingNorm,
                                      RoundedDouble wavePeriodForMechanismSpecificSignalingNorm,
                                      RoundedDouble wavePeriodForMechanismSpecificLowerLimitNorm,
                                      RoundedDouble wavePeriodForLowerLimitNorm,
                                      RoundedDouble wavePeriodForFactorizedLowerLimitNorm)
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
            CoastalAreaId = coastalAreaId;
            Offset = offset;
            D50 = d50;
            WaterLevelForMechanismSpecificFactorizedSignalingNorm = waterLevelForMechanismSpecificFactorizedSignalingNorm;
            WaterLevelForMechanismSpecificSignalingNorm = waterLevelForMechanismSpecificSignalingNorm;
            WaterLevelForMechanismSpecificLowerLimitNorm = waterLevelForMechanismSpecificLowerLimitNorm;
            WaterLevelForLowerLimitNorm = waterLevelForLowerLimitNorm;
            WaterLevelForFactorizedLowerLimitNorm = waterLevelForFactorizedLowerLimitNorm;
            WaveHeightForMechanismSpecificFactorizedSignalingNorm = waveHeightForMechanismSpecificFactorizedSignalingNorm;
            WaveHeightForMechanismSpecificSignalingNorm = waveHeightForMechanismSpecificSignalingNorm;
            WaveHeightForMechanismSpecificLowerLimitNorm = waveHeightForMechanismSpecificLowerLimitNorm;
            WaveHeightForLowerLimitNorm = waveHeightForLowerLimitNorm;
            WaveHeightForFactorizedLowerLimitNorm = waveHeightForFactorizedLowerLimitNorm;
            WavePeriodForMechanismSpecificFactorizedSignalingNorm = wavePeriodForMechanismSpecificFactorizedSignalingNorm;
            WavePeriodForMechanismSpecificSignalingNorm = wavePeriodForMechanismSpecificSignalingNorm;
            WavePeriodForMechanismSpecificLowerLimitNorm = wavePeriodForMechanismSpecificLowerLimitNorm;
            WavePeriodForLowerLimitNorm = wavePeriodForLowerLimitNorm;
            WavePeriodForFactorizedLowerLimitNorm = wavePeriodForFactorizedLowerLimitNorm;
        }

        /// <summary>
        /// Gets the id of the dune location.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the name of the dune location.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the location of the dune location.
        /// </summary>
        public Point2D Location { get; }

        /// <summary>
        /// Gets the coastal area id of the dune location.
        /// </summary>
        public int CoastalAreaId { get; }

        /// <summary>
        /// Gets the offset of the dune location.
        /// </summary>
        public RoundedDouble Offset { get; }

        /// <summary>
        /// Gets the d50 of the dune location. 
        /// </summary>
        public RoundedDouble D50 { get; }

        /// <summary>
        /// Gets the result of the water level for the mechanism specific factorized signaling norm.
        /// </summary>
        public RoundedDouble WaterLevelForMechanismSpecificFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the water level for the mechanism specific signaling norm.
        /// </summary>
        public RoundedDouble WaterLevelForMechanismSpecificSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the water level for the mechanism specific lower limit norm.
        /// </summary>
        public RoundedDouble WaterLevelForMechanismSpecificLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the water level for the lower limit norm.
        /// </summary>
        public RoundedDouble WaterLevelForLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the water level for the factorized lower limit norm.
        /// </summary>
        public RoundedDouble WaterLevelForFactorizedLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave height for the mechanism specific factorized signaling norm.
        /// </summary>
        public RoundedDouble WaveHeightForMechanismSpecificFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the wave height for the mechanism specific signaling norm.
        /// </summary>
        public RoundedDouble WaveHeightForMechanismSpecificSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the wave height for the mechanism specific lower limit norm.
        /// </summary>
        public RoundedDouble WaveHeightForMechanismSpecificLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave height for the lower limit norm.
        /// </summary>
        public RoundedDouble WaveHeightForLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave height for the factorized lower limit norm.
        /// </summary>
        public RoundedDouble WaveHeightForFactorizedLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave period for the mechanism specific factorized signaling norm.
        /// </summary>
        public RoundedDouble WavePeriodForMechanismSpecificFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the wave period for the mechanism specific signaling norm.
        /// </summary>
        public RoundedDouble WavePeriodForMechanismSpecificSignalingNorm { get; }

        /// <summary>
        /// Gets the result of the wave period for the mechanism specific lower limit norm.
        /// </summary>
        public RoundedDouble WavePeriodForMechanismSpecificLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave period for the lower limit norm.
        /// </summary>
        public RoundedDouble WavePeriodForLowerLimitNorm { get; }

        /// <summary>
        /// Gets the result of the wave period for the factorized lower limit norm.
        /// </summary>
        public RoundedDouble WavePeriodForFactorizedLowerLimitNorm { get; }
    }
}