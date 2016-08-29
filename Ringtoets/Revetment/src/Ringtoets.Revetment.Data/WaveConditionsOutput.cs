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

using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// The result of the wave conditions assessment.
    /// </summary>
    public class WaveConditionsOutput : Observable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="waterLevel">The water level which has been calculated.</param>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="wavePeriod">The calculated wave period.</param>
        /// <param name="waveOrientation">The calculated wave orientation.</param>
        public WaveConditionsOutput(double waterLevel, double waveHeight, double wavePeriod, double waveOrientation)
        {
            WaterLevel = waterLevel;
            WaveHeight = waveHeight;
            WavePeriod = wavePeriod;
            WaveOrientation = waveOrientation;
        }

        /// <summary>
        /// Gets the water level which has been calculated.
        /// </summary>
        public double WaterLevel { get; private set; }

        /// <summary>
        /// Gets the calculated wave height.
        /// </summary>
        public double WaveHeight { get; private set; }

        /// <summary>
        /// Gets the calculated wave period.
        /// </summary>
        public double WavePeriod { get; private set; }

        /// <summary>
        /// Gets the calculated wave orientation.
        /// </summary>
        public double WaveOrientation { get; private set; }
    }
}