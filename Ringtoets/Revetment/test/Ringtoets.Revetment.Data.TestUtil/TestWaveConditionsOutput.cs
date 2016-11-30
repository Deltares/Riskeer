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

using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.TestUtil
{
    /// <summary>
    /// Simple wave conditions output that can be used for testing.
    /// </summary>
    public class TestWaveConditionsOutput : WaveConditionsOutput
    {
        /// <summary>
        /// Instantiates a <see cref="TestWaveConditionsOutput"/> with default values.
        /// </summary>
        public TestWaveConditionsOutput() : this(CalculationConvergence.NotCalculated) {}

        /// <summary>
        /// Instantiates a <see cref="TestWaveConditionsOutput"/> with a specified 
        /// <see cref="CalculationConvergence"/> state.
        /// </summary>
        /// <param name="convergence">The status of the calculation.</param>
        public TestWaveConditionsOutput(CalculationConvergence convergence) :
            base(1.0, 2.0, 3.0, 4.0, 5.0, 3000, 35.0)
        {
            CalculationConvergence = convergence;
        }
    }
}