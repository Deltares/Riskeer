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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// The result of a wave conditions calculation.
    /// </summary>
    public class WaveConditionsOutput : Observable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="waterLevel">The water level for which the calculation has been performed.</param>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="wavePeakPeriod">The calculated wave peak period.</param>
        /// <param name="waveAngle">The calculated wave angle.</param>
        /// <remarks>All provided output values will be rounded to 2 decimals.</remarks>
        public WaveConditionsOutput(double waterLevel, double waveHeight, double wavePeakPeriod, double waveAngle)
        {
            WaterLevel = new RoundedDouble(2, waterLevel);
            WaveHeight = new RoundedDouble(2, waveHeight);
            WavePeakPeriod = new RoundedDouble(2, wavePeakPeriod);
            WaveAngle = new RoundedDouble(2, waveAngle);
        }

        /// <summary>
        /// Gets the water level for which the calculation has been performed.
        /// </summary>
        public RoundedDouble WaterLevel { get; private set; }

        /// <summary>
        /// Gets the calculated wave height.
        /// </summary>
        public RoundedDouble WaveHeight { get; private set; }

        /// <summary>
        /// Gets the calculated wave peak period.
        /// </summary>
        public RoundedDouble WavePeakPeriod { get; private set; }

        /// <summary>
        /// Gets the calculated wave angle.
        /// </summary>
        public RoundedDouble WaveAngle { get; private set; }
    }
}