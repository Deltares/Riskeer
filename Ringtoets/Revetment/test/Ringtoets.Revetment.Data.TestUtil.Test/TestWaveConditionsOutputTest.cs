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
            Assert.AreEqual(1.0, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(2.0, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(3.0, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(4.0, output.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(5.0, output.WaveDirection, output.WaveDirection.GetAccuracy());

            double expectedTargetReliability = StatisticsConverter.ReturnPeriodToReliability(3000);
            Assert.AreEqual(expectedTargetReliability, output.TargetReliability);
            Assert.AreEqual(35.0, output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.NotCalculated, output.CalculationConvergence);
        }

        [Test]
        [TestCase(CalculationConvergence.NotCalculated)]
        [TestCase(CalculationConvergence.CalculatedConverged)]
        [TestCase(CalculationConvergence.CalculatedNotConverged)]
        public void Constructor_CalculationConvergence_ReturnsWithExpectedCalculationConvergence(CalculationConvergence convergence)
        {
            // Call
            WaveConditionsOutput output = new TestWaveConditionsOutput(convergence);

            // Assert
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }
    }
}