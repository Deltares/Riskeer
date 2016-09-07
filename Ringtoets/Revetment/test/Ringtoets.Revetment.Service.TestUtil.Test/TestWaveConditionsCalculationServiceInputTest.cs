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
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Service.TestUtil.Test
{
    [TestFixture]
    public class TestWaveConditionsCalculationServiceInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            RoundedDouble waterLevel = (RoundedDouble) 23.5;
            const double a = 1.0;
            const double b = 0.3;
            const double c = 0.8;
            const int norm = 5;
            var input = new WaveConditionsInput();
            const string hlcdDirectory = "C/temp";
            const string ringId = "11-1";
            const string name = "test";

            // Call
            var testInput = new TestWaveConditionsCalculationServiceInput(waterLevel, a, b, c, norm, input, hlcdDirectory, ringId, name);

            // Assert
            Assert.AreEqual(waterLevel, testInput.WaterLevel);
            Assert.AreEqual(a, testInput.A);
            Assert.AreEqual(b, testInput.B);
            Assert.AreEqual(c, testInput.C);
            Assert.AreEqual(norm, testInput.Norm);
            Assert.AreSame(input, testInput.WaveConditionsInput);
            Assert.AreEqual(hlcdDirectory, testInput.HlcdDirectory);
            Assert.AreEqual(ringId, testInput.RingId);
            Assert.AreEqual(name, testInput.Name);
        }
    }
}