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

using System;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Riskeer.DuneErosion.Data.TestUtil.Test
{
    [TestFixture]
    public class TestDuneLocationCalculationOutputTest
    {
        [Test]
        public void Constructor_WithoutArguments_ExpectedValues()
        {
            // Call
            var output = new TestDuneLocationCalculationOutput();

            // Assert
            Assert.IsInstanceOf<DuneLocationCalculationOutput>(output);
            Assert.AreEqual(0, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(0, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(0, output.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(0, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(0, output.TargetProbability);
            Assert.AreEqual(0, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(0, output.CalculatedProbability);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, output.CalculationConvergence);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();

            // Call
            var output = new TestDuneLocationCalculationOutput(waterLevel, waveHeight, wavePeriod);

            // Assert
            Assert.IsInstanceOf<DuneLocationCalculationOutput>(output);
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeriod, output.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(0, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(0, output.TargetProbability);
            Assert.AreEqual(0, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(0, output.CalculatedProbability);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, output.CalculationConvergence);
        }
    }
}