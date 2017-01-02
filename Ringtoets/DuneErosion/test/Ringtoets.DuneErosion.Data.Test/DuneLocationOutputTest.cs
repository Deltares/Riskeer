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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneLocationOutputTest
    {
        [Test]
        [TestCase(-0.01)]
        [TestCase(1.01)]
        public void Constructor_InvalidTargetProbability_ThrowsArgumentOutOfRangeException(double targetProbability)
        {
            // Setup
            var random = new Random(32);
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            CalculationConvergence convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            TestDelegate call = () => new DuneLocationOutput(waterLevel,
                                                             waveHeight,
                                                             wavePeriod,
                                                             targetProbability,
                                                             targetReliability,
                                                             calculatedProbability,
                                                             calculatedReliability,
                                                             convergence);

            // Assert
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("targetProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0, 1] liggen.", exception.Message);
        }

        [Test]
        [TestCase(-0.01)]
        [TestCase(1.01)]
        public void Constructor_InvalidCalculatedProbability_ThrowsArgumentOutOfRangeException(double calculatedProbability)
        {
            // Setup
            var random = new Random(32);
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            CalculationConvergence convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            TestDelegate call = () => new DuneLocationOutput(waterLevel,
                                                             waveHeight,
                                                             wavePeriod,
                                                             targetProbability,
                                                             targetReliability,
                                                             calculatedProbability,
                                                             calculatedReliability,
                                                             convergence);

            // Assert
            ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("calculatedProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0, 1] liggen.", exception.Message);
        }

        [Test]
        public void Constructor_ValidInput_ExpectedProperties()
        {
            // Setup
            var random = new Random(32);
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            CalculationConvergence convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            var output = new DuneLocationOutput(waterLevel,
                                                waveHeight,
                                                wavePeriod,
                                                targetProbability,
                                                targetReliability,
                                                calculatedProbability,
                                                calculatedReliability,
                                                convergence);

            // Assert
            Assert.AreEqual(2, output.WaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(2, output.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(2, output.WavePeriod.NumberOfDecimalPlaces);
            Assert.AreEqual(wavePeriod, output.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(5, output.TargetReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(5, output.CalculatedReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }
    }
}