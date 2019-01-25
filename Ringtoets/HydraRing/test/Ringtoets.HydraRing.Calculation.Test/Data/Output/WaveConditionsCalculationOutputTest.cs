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

using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data.Output;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Output
{
    [TestFixture]
    public class WaveConditionsCalculationOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double waveAngle = 3.0;
            const double waveDirection = 5.0;
            const double waveHeight = 9.93;
            const double wavePeakPeriod = 18;

            // Call
            var output = new WaveConditionsCalculationOutput(waveHeight, wavePeakPeriod, waveAngle, waveDirection);

            // Assert
            Assert.AreEqual(waveHeight, output.WaveHeight);
            Assert.AreEqual(wavePeakPeriod, output.WavePeakPeriod);
            Assert.AreEqual(waveAngle, output.WaveAngle);
            Assert.AreEqual(waveDirection, output.WaveDirection);
        }
    }
}