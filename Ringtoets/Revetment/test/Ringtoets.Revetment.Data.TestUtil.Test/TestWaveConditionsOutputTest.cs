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

using System;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.TestUtil.Test
{
    [TestFixture]
    public class TestWaveConditionsOutputTest
    {
        [Test]
        public void Constructor_Always_ReturnsWithCalculationConvergenceNotCalculated()
        {
            // Call
            WaveConditionsOutput output = new TestWaveConditionsOutput();

            // Assert
            Assert.AreEqual(1.1, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(2.2, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(3.3, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(4.4, output.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(5.5, output.WaveDirection, output.WaveDirection.GetAccuracy());

            double expectedTargetReliability = StatisticsConverter.ReturnPeriodToReliability(3000);
            Assert.AreEqual(expectedTargetReliability, output.TargetReliability);
            Assert.AreEqual(12.3, output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.NotCalculated, output.CalculationConvergence);
        }

        [Test]
        [TestCase(CalculationConvergence.NotCalculated)]
        [TestCase(CalculationConvergence.CalculatedConverged)]
        [TestCase(CalculationConvergence.CalculatedNotConverged)]
        public void Constructor_WithParameters_ReturnsWithExpectedCalculationConvergence(CalculationConvergence convergence)
        {
            // Setup 
            var random = new Random(21);
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeakPeriod = random.NextDouble();
            double waveAngle = random.NextDouble();

            // Call
            WaveConditionsOutput output = new TestWaveConditionsOutput(waterLevel, waveHeight, wavePeakPeriod, waveAngle, convergence);

            // Assert
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeakPeriod, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(waveAngle, output.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(5.5, output.WaveDirection, output.WaveDirection.GetAccuracy());

            double expectedTargetReliability = StatisticsConverter.ReturnPeriodToReliability(3000);
            Assert.AreEqual(expectedTargetReliability, output.TargetReliability);
            Assert.AreEqual(12.3, output.CalculatedReliability);
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }
    }
}