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
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.Service.Test
{
    [TestFixture]
    public class WaveConditionsOutputFactoryTest
    {
        [Test]
        public void CreateOutput_NonProbabilisticInput_ReturnsExpectedNonProbabilisticValues()
        {
            // Setup
            const double waterLevel = 1.1;
            const double waveHeight = 2.2;
            const double wavePeakPeriod = 3.3;
            const double waveAngle = 4.4;
            const double waveDirection = 5.5;

            // Call 
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateOutput(waterLevel, waveHeight, wavePeakPeriod,
                                                                                   waveAngle, waveDirection, double.MinValue, double.NaN, null);

            // Assert
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeakPeriod, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(waveAngle, output.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(waveDirection, output.WaveDirection, output.WaveDirection.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.NotCalculated, output.CalculationConvergence);
        }

        [Test]
        [TestCase(true, CalculationConvergence.CalculatedConverged)]
        [TestCase(false, CalculationConvergence.CalculatedNotConverged)]
        [TestCase(null, CalculationConvergence.NotCalculated)]
        public void CreateOutput_WithConvergedResults_CalculationConvergedTrue(bool? convergence, CalculationConvergence expectedConvergence)
        {
            // Call
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateOutput(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                                   double.NaN, double.NaN, double.NaN, convergence);

            // Assert
            Assert.AreEqual(expectedConvergence, output.CalculationConvergence);
        }

        [Test]
        [TestCase(1.1, -1.335177736118940)]
        [TestCase(3000, 3.402932835385330)]
        [TestCase(20000, 3.890591886413120)]
        [TestCase(30000, 3.987878936606940)]
        [TestCase(600000, 4.649132934007460)]
        [TestCase(1000000, 4.75342430881709)]
        [TestCase(6000000, 5.103554002888150)]
        public void CreateOutput_DifferentReturnPeriods_ReturnsExpectedTargetReliability(double returnPeriod, double expectedReliability)
        {
            // Setup
            double norm = 1.0 / returnPeriod;

            // Call 
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateOutput(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                                   double.NaN, norm, double.NaN, null);

            // Assert
            Assert.AreEqual(expectedReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
        }

        [Test]
        [TestCase(1.1, 9.090909E-01)]
        [TestCase(3000, 3.333333E-04)]
        [TestCase(20000, 5.000000E-05)]
        [TestCase(30000, 3.333333E-05)]
        [TestCase(600000, 1.666667E-06)]
        [TestCase(1000000, 1.000000E-06)]
        [TestCase(6000000, 1.666667E-07)]
        public void CreateOutput_DifferentReturnPeriods_ReturnsExpectedargetProbability(double returnPeriod, double expectedProbability)
        {
            // Setup
            double norm = 1.0 / returnPeriod;

            // Call 
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateOutput(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                                   double.NaN, norm, double.NaN, null);

            // Assert
            Assert.AreEqual(expectedProbability, output.TargetProbability, 1e-6);
        }

        [Test]
        [TestCase(-1.335177736118940, -1.335177736118940)]
        [TestCase(3.402932835385330, 3.402932835385330)]
        public void CreateOutput_DifferentReliabilities_ReturnsExpectedCalculatedReliability(double reliability, double expectedReliability)
        {
            // Call 
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateOutput(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                                   double.NaN, double.MinValue, reliability, null);

            // Assert
            Assert.AreEqual(expectedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
        }

        [Test]
        [TestCase(-1.335177736118940, 9.090909E-01)]
        [TestCase(3.402932835385330, 3.333333E-04)]
        [TestCase(3.890591886413120, 5.000000E-05)]
        [TestCase(3.987878936606940, 3.333333E-05)]
        [TestCase(4.649132934007460, 1.666667E-06)]
        [TestCase(4.753424308817090, 1.000000E-06)]
        [TestCase(5.103554002888150, 1.666667E-07)]
        public void CreateOutput_DifferentReliabilities_ReturnsExpectedCalculatedProbability(double reliability, double expectedProbability)
        {
            // Call 
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateOutput(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                                   double.NaN, double.MinValue, reliability, null);

            // Assert
            Assert.AreEqual(expectedProbability, output.CalculatedProbability, 1e-6);
        }

        [Test]
        [TestCase(double.NaN, double.NaN, double.NaN)]
        [TestCase(double.MaxValue, 1, double.NegativeInfinity)]
        [TestCase(double.MinValue, 0, double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity, 0, double.PositiveInfinity)]
        [TestCase(double.PositiveInfinity, 1, double.NegativeInfinity)]
        public void CreateFailedOutput_InvalidNorm_ReturnsExpectedValues(double targetNorm,
                                                                         double expectedTargetProbability,
                                                                         double expectedTargetReliability)
        {
            // Call
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateFailedOutput(double.NaN, targetNorm);

            // Assert
            Assert.IsNaN(output.WaterLevel);
            Assert.IsNaN(output.WaveHeight);
            Assert.IsNaN(output.WavePeakPeriod);
            Assert.IsNaN(output.WaveAngle);
            Assert.IsNaN(output.WaveDirection);
            Assert.AreEqual(expectedTargetProbability, output.TargetProbability, 1e-6);
            Assert.AreEqual(expectedTargetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, output.CalculationConvergence);
        }

        [Test]
        [TestCase(1.1, 9.090909E-01, -1.335177736118940)]
        [TestCase(3000, 3.333333E-04, 3.402932835385330)]
        [TestCase(20000, 5.000000E-05, 3.890591886413120)]
        [TestCase(30000, 3.333333E-05, 3.987878936606940)]
        [TestCase(600000, 1.666667E-06, 4.649132934007460)]
        public void CreateFailedOutput_ValidParameters_ReturnsExpectedValues(double returnPeriod,
                                                                             double expectedTargetProbability,
                                                                             double expectedTargetReliability)
        {
            // Setup
            var random = new Random(12);
            double waterLevel = random.NextDouble();
            double targetNorm = 1.0 / returnPeriod;

            // Call
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateFailedOutput(waterLevel, targetNorm);

            // Assert
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.IsNaN(output.WaveHeight);
            Assert.IsNaN(output.WavePeakPeriod);
            Assert.IsNaN(output.WaveAngle);
            Assert.IsNaN(output.WaveDirection);
            Assert.AreEqual(expectedTargetProbability, output.TargetProbability, 1e-6);
            Assert.AreEqual(expectedTargetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, output.CalculationConvergence);
        }
    }
}