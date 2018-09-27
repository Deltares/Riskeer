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

namespace Ringtoets.HydraRing.Calculation.Data.Output
{
    /// <summary>
    /// Class containing the results of a Hydra-Ring wave conditions calculation.
    /// </summary>s
    public class WaveConditionsCalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationOutput"/>.
        /// </summary>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="wavePeakPeriod">The calculated wave peak period.</param>
        /// <param name="waveAngle">The calculated wave angle with respect to the dike normal..</param>
        /// <param name="waveDirection">The calculated wave direction with respect to North.</param>
        public WaveConditionsCalculationOutput(double waveHeight, double wavePeakPeriod, double waveAngle, double waveDirection)
        {
            WaveHeight = waveHeight;
            WavePeakPeriod = wavePeakPeriod;
            WaveAngle = waveAngle;
            WaveDirection = waveDirection;
        }

        /// <summary>
        /// Gets the calculated wave height.
        /// </summary>
        public double WaveHeight { get; }

        /// <summary>
        /// Gets the calculated wave peak period.
        /// </summary>
        public double WavePeakPeriod { get; }

        /// <summary>
        /// Gets the calculated wave angle with respect to the dike normal.
        /// </summary>
        public double WaveAngle { get; }

        /// <summary>
        /// Gets the calculated wave direction with respect to North.
        /// </summary>
        public double WaveDirection { get; }
    }
}