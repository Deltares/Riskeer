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

using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data.Output;

namespace Riskeer.HydraRing.Calculation.Test.Data.Output
{
    [TestFixture]
    public class DunesBoundaryConditionsCalculationOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double waterLevel = 1.1;
            const double waveHeight = 2.2;
            const double wavePeriod = 3.3;
            const double meanTidalAmplitude = 4.4;
            const double waveDirectionalSpread = 5.5;
            const double tideSurgePhaseDifference = 6.6;

            // Call
            var output = new DunesBoundaryConditionsCalculationOutput(
                waterLevel, waveHeight, wavePeriod, meanTidalAmplitude, waveDirectionalSpread, tideSurgePhaseDifference);

            // Assert
            Assert.AreEqual(waterLevel, output.WaterLevel);
            Assert.AreEqual(waveHeight, output.WaveHeight);
            Assert.AreEqual(wavePeriod, output.WavePeriod);
            Assert.AreEqual(meanTidalAmplitude, output.MeanTidalAmplitude);
            Assert.AreEqual(waveDirectionalSpread, output.WaveDirectionalSpread);
            Assert.AreEqual(tideSurgePhaseDifference, output.TideSurgePhaseDifference);
        }
    }
}