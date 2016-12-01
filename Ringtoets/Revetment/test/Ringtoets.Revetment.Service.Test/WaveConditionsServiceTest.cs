﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Revetment.Service.Test
{
    [TestFixture]
    public class WaveConditionsServiceTest
    {
        [Test]
        public void Calculate_NonProbabilisticInput_ReturnsExpectedNonProbabilisticValues()
        {
            // Setup
            const double waterLevel = 1.1;
            const double waveHeight = 2.2;
            const double wavePeakPeriod = 3.3;
            const double waveAngle = 4.4;
            const double waveDirection = 5.5;

            // Call 
            WaveConditionsOutput output = WaveConditionsService.Calculate(waterLevel, waveHeight, wavePeakPeriod,
                                                                          waveAngle, waveDirection, double.MinValue, double.NaN);

            // Assert
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeakPeriod, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(waveAngle, output.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(waveDirection, output.WaveDirection, output.WaveDirection.GetAccuracy());
        }

        [Test]
        public void CalculationConverged_WithConvergedResults_CalculationConvergedTrue()
        {
            // Setup
            double returnPeriod = 1.0e3;
            double calculatedReliability = StatisticsConverter.ReturnPeriodToReliability(returnPeriod);

            // Call
            WaveConditionsOutput output = WaveConditionsService.Calculate(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                          double.NaN, returnPeriod, calculatedReliability);

            // Assert
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, output.CalculationConvergence);
        }

        [Test]
        public void CalculationConverged_WithoutConvergedResults_CalculationConvergedFalse()
        {
            // Call 
            WaveConditionsOutput output = WaveConditionsService.Calculate(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                          double.NaN, 1, 5.0e-3);

            // Assert
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, output.CalculationConvergence);
        }

        [Test]
        [TestCase(1.1, -1.335177736118940)]
        [TestCase(3000, 3.402932835385330)]
        [TestCase(20000, 3.890591886413120)]
        [TestCase(30000, 3.987878936606940)]
        [TestCase(600000, 4.649132934007460)]
        [TestCase(1000000, 4.75342430881709)]
        [TestCase(6000000, 5.103554002888150)]
        public void TargetReliability_DifferentReturnPeriods_ReturnsExpectedValues(double returnPeriod, double expectedReliability)
        {
            // Call 
            WaveConditionsOutput output = WaveConditionsService.Calculate(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                          double.NaN, returnPeriod, double.NaN);

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
        public void TargetProbability_DifferentReturnPeriods_ReturnsExpectedValues(double returnPeriod, double expectedProbability)
        {
            // Call 
            WaveConditionsOutput output = WaveConditionsService.Calculate(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                          double.NaN, returnPeriod, double.NaN);

            // Assert
            Assert.AreEqual(expectedProbability, output.TargetProbability, 1e-6);
        }

        [Test]
        [TestCase(-1.335177736118940, -1.335177736118940)]
        [TestCase(3.402932835385330, 3.402932835385330)] 
        public void CalculatedReliability_DifferentReliabilities_ReturnsExpectedValues(double reliability, double expectedReliability)
        {
            // Call 
            WaveConditionsOutput output = WaveConditionsService.Calculate(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                          double.NaN, double.MinValue, reliability);

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
        public void CalculatedProbability_DifferentReliabilities_ReturnsExpectedValues(double reliability, double expectedProbability)
        {
            // Call 
            WaveConditionsOutput output = WaveConditionsService.Calculate(double.NaN, double.NaN, double.NaN, double.NaN,
                                                                          double.NaN, double.MinValue, reliability);

            // Assert
            Assert.AreEqual(expectedProbability, output.CalculatedProbability, 1e-6);
        }
    }
}