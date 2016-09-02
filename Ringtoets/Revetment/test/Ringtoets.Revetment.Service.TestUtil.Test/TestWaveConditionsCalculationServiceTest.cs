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

using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Service.TestUtil.Test
{
    [TestFixture]
    public class TestWaveConditionsCalculationServiceTest
    {
        [Test]
        public void Calculate_Always_ReturnsOutput()
        {
            // Setup
            RoundedDouble waterLevel = (RoundedDouble) 7.4;
            var testService = new TestWaveConditionsCalculationService();

            // Call
            WaveConditionsOutput output = testService.Calculate(waterLevel,
                                                                double.NaN,
                                                                double.NaN,
                                                                double.NaN,
                                                                double.NaN,
                                                                new WaveConditionsInput(),
                                                                string.Empty,
                                                                string.Empty,
                                                                string.Empty);

            // Assert
            Assert.AreEqual(waterLevel, output.WaterLevel, waterLevel.GetAccuracy());
            Assert.AreEqual(3.0, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(5.39, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(29, output.WaveAngle, output.WaveAngle.GetAccuracy());
        }
    }
}