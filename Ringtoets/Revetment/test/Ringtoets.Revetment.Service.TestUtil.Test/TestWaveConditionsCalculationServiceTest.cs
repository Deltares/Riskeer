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

using System.Linq;
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
        public void Constructor_ExpectedValues()
        {
            // Call
            var testService = new TestWaveConditionsCalculationService();

            // Assert
            CollectionAssert.IsEmpty(testService.Inputs);
        }

        [Test]
        public void Validate_Always_ReturnsTrue()
        {
            // Setup
            var testService = new TestWaveConditionsCalculationService();

            // Call
            var valid = testService.Validate(string.Empty);

            // Assert
            Assert.IsTrue(valid);
        }

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
                                                                0,
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

        [Test]
        public void Inputs_Always_ReturnsInputsOfCalculateMethod()
        {
            // Setup
            RoundedDouble waterLevel = (RoundedDouble)23.5;
            const double a = 1.0;
            const double b = 0.3;
            const double c = 0.8;
            const int norm = 5;
            var input = new WaveConditionsInput();
            const string hlcdDirectory = "C/temp";
            const string ringId = "11-1";
            const string name = "test";

            var testService = new TestWaveConditionsCalculationService();

            testService.Calculate(waterLevel, a, b, c, norm, input, hlcdDirectory, ringId, name);

            // Call
            TestWaveConditionsCalculationServiceInput[] inputs = testService.Inputs.ToArray();

            // Assert
            Assert.AreEqual(1, inputs.Length);
            Assert.AreEqual(waterLevel, inputs[0].WaterLevel);
            Assert.AreEqual(a, inputs[0].A);
            Assert.AreEqual(b, inputs[0].B);
            Assert.AreEqual(c, inputs[0].C);
            Assert.AreEqual(norm, inputs[0].Norm);
            Assert.AreSame(input, inputs[0].WaveConditionsInput);
            Assert.AreEqual(hlcdDirectory, inputs[0].HlcdDirectory);
            Assert.AreEqual(ringId, inputs[0].RingId);
            Assert.AreEqual(name, inputs[0].Name);
        }
    }
}