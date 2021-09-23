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

namespace Riskeer.Common.Forms.PresentationObjects
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
        /// <param name="waterLevelCalculationForTargetProbabilities">The results of the
        /// water level calculations for different target probabilities.</param>
        /// <param name="waveHeightCalculationForTargetProbabilities">The results of the
        /// wave height calculations for different target probabilities.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>, 
        /// <paramref name="location"/>, <paramref name="waterLevelCalculationForTargetProbabilities"/>
        /// or <paramref name="waveHeightCalculationForTargetProbabilities"/> is <c>null</c>.</exception>
        public AggregatedHydraulicBoundaryLocation(
            long id, string name, Point2D location,
            IEnumerable<Tuple<double, RoundedDouble>> waterLevelCalculationForTargetProbabilities,
            IEnumerable<Tuple<double, RoundedDouble>> waveHeightCalculationForTargetProbabilities)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (waterLevelCalculationForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(waterLevelCalculationForTargetProbabilities));
            }

            if (waveHeightCalculationForTargetProbabilities == null)
            {
                throw new ArgumentNullException(nameof(waveHeightCalculationForTargetProbabilities));
            }

            Id = id;
            Name = name;
            Location = location;
            WaterLevelCalculationForTargetProbabilities = waterLevelCalculationForTargetProbabilities;
            WaveHeightCalculationForTargetProbabilities = waveHeightCalculationForTargetProbabilities;
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
        /// Gets the results of the water level calculations for different target probabilities.
        /// </summary>
        public IEnumerable<Tuple<double, RoundedDouble>> WaterLevelCalculationForTargetProbabilities { get; }

        /// <summary>
        /// Gets the results of the wave height calculations for different target probabilities.
        /// </summary>
        public IEnumerable<Tuple<double, RoundedDouble>> WaveHeightCalculationForTargetProbabilities { get; }
    }
}