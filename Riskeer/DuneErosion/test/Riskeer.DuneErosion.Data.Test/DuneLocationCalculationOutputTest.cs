// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneLocationCalculationOutputTest
    {
        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneLocationCalculationOutput(CalculationConvergence.CalculatedConverged, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1e-6)]
        [TestCase(1.0 + 1e-6)]
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
            var convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            TestDelegate call = () => new DuneLocationCalculationOutput(convergence,
                                                                        new DuneLocationCalculationOutput.ConstructionProperties
                                                                        {
                                                                            WaterLevel = waterLevel,
                                                                            WaveHeight = waveHeight,
                                                                            WavePeriod = wavePeriod,
                                                                            TargetProbability = targetProbability,
                                                                            TargetReliability = targetReliability,
                                                                            CalculatedProbability = calculatedProbability,
                                                                            CalculatedReliability = calculatedReliability
                                                                        });

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("TargetProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0,0, 1,0] liggen.", exception.Message);
        }

        [Test]
        [SetCulture("nl-NL")]
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
            var convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            TestDelegate call = () => new DuneLocationCalculationOutput(convergence,
                                                                        new DuneLocationCalculationOutput.ConstructionProperties
                                                                        {
                                                                            WaterLevel = waterLevel,
                                                                            WaveHeight = waveHeight,
                                                                            WavePeriod = wavePeriod,
                                                                            TargetProbability = targetProbability,
                                                                            TargetReliability = targetReliability,
                                                                            CalculatedProbability = calculatedProbability,
                                                                            CalculatedReliability = calculatedReliability
                                                                        });

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            Assert.AreEqual("CalculatedProbability", exception.ParamName);
            StringAssert.Contains("Kans moet in het bereik [0,0, 1,0] liggen.", exception.Message);
        }

        [Test]
        [TestCase(double.NaN, 0.5)]
        [TestCase(0.2, double.NaN)]
        public void Constructor_ValidInput_ExpectedProperties(double targetProbability, double calculatedProbability)
        {
            // Setup
            var random = new Random(32);
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            // Call
            var output = new DuneLocationCalculationOutput(convergence,
                                                           new DuneLocationCalculationOutput.ConstructionProperties
                                                           {
                                                               WaterLevel = waterLevel,
                                                               WaveHeight = waveHeight,
                                                               WavePeriod = wavePeriod,
                                                               TargetProbability = targetProbability,
                                                               TargetReliability = targetReliability,
                                                               CalculatedProbability = calculatedProbability,
                                                               CalculatedReliability = calculatedReliability
                                                           });

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

        [Test]
        public void Constructor_EmptyConstructionProperties_DefaultValues()
        {
            // Call
            var output = new DuneLocationCalculationOutput(CalculationConvergence.CalculatedConverged,
                                                           new DuneLocationCalculationOutput.ConstructionProperties());

            // Assert
            Assert.IsNaN(output.WaterLevel);
            Assert.IsNaN(output.WaveHeight);
            Assert.IsNaN(output.WavePeriod);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.IsNaN(output.CalculatedProbability);
        }
    }
}