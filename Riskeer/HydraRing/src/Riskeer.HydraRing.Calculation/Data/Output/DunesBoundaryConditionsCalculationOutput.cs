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

namespace Riskeer.HydraRing.Calculation.Data.Output
{
    /// <summary>
    /// Class containing the results of a Hydra-Ring dunes boundary conditions calculation.
    /// </summary>
    public class DunesBoundaryConditionsCalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="DunesBoundaryConditionsCalculationOutput"/>.
        /// </summary>
        /// <param name="waterLevel">The calculated water level.</param>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="wavePeriod">The calculated wave period.</param>
        /// <param name="meanTidalAmplitude">The calculated mean tidal amplitude.</param>
        /// <param name="waveDirectionalSpread">The calculated wave directional spread.</param>
        /// <param name="tideSurgePhaseDifference">The calculated tide surge phase difference.</param>
        public DunesBoundaryConditionsCalculationOutput(
            double waterLevel, double waveHeight, double wavePeriod,
            double meanTidalAmplitude, double waveDirectionalSpread, double tideSurgePhaseDifference)
        {
            WaterLevel = waterLevel;
            WaveHeight = waveHeight;
            WavePeriod = wavePeriod;
            MeanTidalAmplitude = meanTidalAmplitude;
            WaveDirectionalSpread = waveDirectionalSpread;
            TideSurgePhaseDifference = tideSurgePhaseDifference;
        }

        /// <summary>
        /// Gets the calculated water level.
        /// </summary>
        public double WaterLevel { get; }

        /// <summary>
        /// Gets the calculated wave height.
        /// </summary>
        public double WaveHeight { get; }

        /// <summary>
        /// Gets the calculated wave period.
        /// </summary>
        public double WavePeriod { get; }

        /// <summary>
        /// Gets the calculated mean tidal amplitude.
        /// </summary>
        public double MeanTidalAmplitude { get; }

        /// <summary>
        /// Gets the calculated wave directional spread.
        /// </summary>
        public double WaveDirectionalSpread { get; }

        /// <summary>
        /// Gets the calculated tide surge phase difference.
        /// </summary>
        public double TideSurgePhaseDifference { get; }
    }
}