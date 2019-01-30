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

using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Revetment.Data.TestUtil
{
    /// <summary>
    /// Simple wave conditions output that can be used for testing.
    /// </summary>
    public class TestWaveConditionsOutput : WaveConditionsOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestWaveConditionsOutput"/> with default values.
        /// </summary>
        public TestWaveConditionsOutput() : this(1.1, 2.2, 3.3, 4.4, 5.5) {}

        private TestWaveConditionsOutput(double waterLevel, double waveHeight, double wavePeakPeriod,
                                         double waveAngle, double waveDirection) :
            base(waterLevel, waveHeight, wavePeakPeriod, waveAngle, waveDirection,
                 0.1, 1.282, 0.4, 0.253, CalculationConvergence.CalculatedConverged) {}
    }
}