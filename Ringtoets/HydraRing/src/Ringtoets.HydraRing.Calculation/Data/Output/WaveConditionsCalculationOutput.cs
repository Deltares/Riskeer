// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
    public class WaveConditionsCalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationOutput"/>.
        /// </summary>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="wavePeakPeriod">The calculated wave peak period.</param>
        /// <param name="waveAngle">The calculated wave angle.</param>
        public WaveConditionsCalculationOutput(double waveHeight, double wavePeakPeriod, double waveAngle)
        {
            WaveHeight = waveHeight;
            WavePeakPeriod = wavePeakPeriod;
            WaveAngle = waveAngle;
        }

        /// <summary>
        /// Gets the calculated wave height.
        /// </summary>
        public double WaveHeight { get; private set; }

        /// <summary>
        /// Gets the calculated wave peak period.
        /// </summary>
        public double WavePeakPeriod { get; private set; }

        /// <summary>
        /// Gets the calculated wave angle.
        /// </summary>
        public double WaveAngle { get; private set; }
    }
}
