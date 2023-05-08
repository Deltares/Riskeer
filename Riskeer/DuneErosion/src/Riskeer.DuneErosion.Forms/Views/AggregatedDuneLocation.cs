﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

namespace Riskeer.DuneErosion.Forms.Views
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
        /// <param name="waterLevelCalculationsForTargetProbabilities">The results of the
        /// water level calculations for different target probabilities.</param>
        /// <param name="waveHeightCalculationsForTargetProbabilities">The results of the
        /// wave height calculations for different target probabilities.</param>
        /// <param name="wavePeriodCalculationsForTargetProbabilities">The results of the
        /// wave period calculations for different target probabilities.</param>
        /// <param name="meanTidalAmplitudeCalculationsForTargetProbabilities">The results of the
        /// mean tide calculations for different target probabilities.</param>
        /// <param name="waveDirectionalSpreadCalculationsForTargetProbabilities">The results of the
        /// wave directional spread calculations for different target probabilities.</param>
        /// <param name="tideSurgePhaseDifferenceCalculationsForTargetProbabilities">The results of the
        /// tide surge phase difference calculations for different target probabilities.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter except <paramref name="id"/>, <paramref name="coastalAreaId"/>
        /// or <paramref name="offset"/> is <c>null</c>.</exception>
        public AggregatedDuneLocation(long id, string name, Point2D location, int coastalAreaId, RoundedDouble offset,
                                      IEnumerable<Tuple<double, RoundedDouble>> waterLevelCalculationsForTargetProbabilities,
                                      IEnumerable<Tuple<double, RoundedDouble>> waveHeightCalculationsForTargetProbabilities,
                                      IEnumerable<Tuple<double, RoundedDouble>> wavePeriodCalculationsForTargetProbabilities,
                                      IEnumerable<Tuple<double, RoundedDouble>> meanTidalAmplitudeCalculationsForTargetProbabilities,
                                      IEnumerable<Tuple<double, RoundedDouble>> waveDirectionalSpreadCalculationsForTargetProbabilities,
                                      IEnumerable<Tuple<double, RoundedDouble>> tideSurgePhaseDifferenceCalculationsForTargetProbabilities)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (waterLevelCalculationsForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(waterLevelCalculationsForTargetProbabilities));
            }

            if (waveHeightCalculationsForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(waveHeightCalculationsForTargetProbabilities));
            }

            if (wavePeriodCalculationsForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(wavePeriodCalculationsForTargetProbabilities));
            }

            if (meanTidalAmplitudeCalculationsForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(meanTidalAmplitudeCalculationsForTargetProbabilities));
            }

            if (waveDirectionalSpreadCalculationsForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(waveDirectionalSpreadCalculationsForTargetProbabilities));
            }

            if (tideSurgePhaseDifferenceCalculationsForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(tideSurgePhaseDifferenceCalculationsForTargetProbabilities));
            }

            Id = id;
            Name = name;
            Location = location;
            CoastalAreaId = coastalAreaId;
            Offset = offset;
            WaterLevelCalculationsForTargetProbabilities = waterLevelCalculationsForTargetProbabilities;
            WaveHeightCalculationsForTargetProbabilities = waveHeightCalculationsForTargetProbabilities;
            WavePeriodCalculationsForTargetProbabilities = wavePeriodCalculationsForTargetProbabilities;
            MeanTidalAmplitudeCalculationsForTargetProbabilities = meanTidalAmplitudeCalculationsForTargetProbabilities;
            WaveDirectionalSpreadCalculationsForTargetProbabilities = waveDirectionalSpreadCalculationsForTargetProbabilities;
            TideSurgePhaseDifferenceCalculationsForTargetProbabilities = tideSurgePhaseDifferenceCalculationsForTargetProbabilities;
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
        /// Gets the results of the water level calculations for different target probabilities.
        /// </summary>
        public IEnumerable<Tuple<double, RoundedDouble>> WaterLevelCalculationsForTargetProbabilities { get; }

        /// <summary>
        /// Gets the results of the wave height calculations for different target probabilities.
        /// </summary>
        public IEnumerable<Tuple<double, RoundedDouble>> WaveHeightCalculationsForTargetProbabilities { get; }

        /// <summary>
        /// Gets the results of the wave period calculations for different target probabilities.
        /// </summary>
        public IEnumerable<Tuple<double, RoundedDouble>> WavePeriodCalculationsForTargetProbabilities { get; }

        /// <summary>
        /// Gets the results of the mean tidal amplitude calculations for different target probabilities.
        /// </summary>
        public IEnumerable<Tuple<double, RoundedDouble>> MeanTidalAmplitudeCalculationsForTargetProbabilities { get; }

        /// <summary>
        /// Gets the results of the wave directional spread calculations for different target probabilities.
        /// </summary>
        public IEnumerable<Tuple<double, RoundedDouble>> WaveDirectionalSpreadCalculationsForTargetProbabilities { get; }

        /// <summary>
        /// Gets the results of the tide surge phase difference calculations for different target probabilities.
        /// </summary>
        public IEnumerable<Tuple<double, RoundedDouble>> TideSurgePhaseDifferenceCalculationsForTargetProbabilities { get; }
    }
}