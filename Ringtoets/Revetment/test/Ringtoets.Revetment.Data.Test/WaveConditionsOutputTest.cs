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

using System;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double waterLevel = 3.09378;
            const double waveHeight = 4.29884;
            const double wavePeakPeriod = 0.19435;
            const double waveAngle = 180.62353;
            const double waveDirection = 230.67893;
            const double targetProbability = 0.5;
            const double targetReliability = 3000;
            const double calculatedProbability = 0.7;
            const double calculatedReliability = 4000;

            // Call
            var output = new WaveConditionsOutput(waterLevel, waveHeight, wavePeakPeriod, waveAngle, waveDirection, targetProbability,
                                                  targetReliability, calculatedProbability, calculatedReliability);

            // Assert
            Assert.IsInstanceOf<Observable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeakPeriod, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(waveAngle, output.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(waveDirection, output.WaveDirection, output.WaveDirection.GetAccuracy());

            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(6, output.TargetReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(6, output.CalculatedReliability.NumberOfDecimalPlaces);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.NotCalculated, output.CalculationConvergence);
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(1.0)]
        [TestCase(0.5)]
        [TestCase(double.NaN)]
        public void TargetProbability_ValidValues_ReturnsExpectedValue(double targetProbability)
        {
            // Call 
            var output = new WaveConditionsOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, targetProbability,
                                                  double.NaN, double.NaN, double.NaN);

            // Assert
            Assert.AreEqual(targetProbability, output.TargetProbability);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(-100)]
        [TestCase(100)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void TargetProbability_InvalidValues_ThrowsArgumentOutOfRangeException(double targetProbability)
        {
            // Call 
            TestDelegate call = () => new WaveConditionsOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, targetProbability,
                                                               double.NaN, double.NaN, double.NaN);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0, 1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }


        [Test]
        [TestCase(0.0)]
        [TestCase(1.0)]
        [TestCase(0.5)]
        [TestCase(double.NaN)]
        public void CalculatedProbability_ValidValues_ReturnsExpectedValue(double calculatedProbability)
        {
            // Call 
            var output = new WaveConditionsOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                                                  double.NaN, calculatedProbability, double.NaN);

            // Assert
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(1 + 1e-6)]
        [TestCase(-100)]
        [TestCase(100)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void CalculatedProbability_InvalidValues_ThrowsArgumentOutOfRangeException(double calculatedProbability)
        {
            // Call 
            TestDelegate call = () => new WaveConditionsOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                                                               double.NaN, calculatedProbability, double.NaN);

            // Assert
            const string expectedMessage = "Kans moet in het bereik [0, 1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(CalculationConvergence.NotCalculated)]
        [TestCase(CalculationConvergence.CalculatedConverged)]
        [TestCase(CalculationConvergence.CalculatedNotConverged)]
        public void CalculationConvergence_ValidValues_SetsCalculationConvergence(CalculationConvergence convergence)
        {
            // Setup
            var output = new WaveConditionsOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                                                  double.NaN, double.NaN, double.NaN);

            // Call
            output.CalculationConvergence = convergence;

            // Assert
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }

        [Test]
        public void CalculationConvergence_Invalidvalue_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var output = new WaveConditionsOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                                                  double.NaN, double.NaN, double.NaN);

            // Call
            TestDelegate call = () => output.CalculationConvergence = (CalculationConvergence) 9001;

            // Assert
            const string expectedMessage = "The value of argument 'CalculationConvergence' (9001) is invalid for Enum type 'CalculationConvergence'.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call,
                                                                                                                    expectedMessage).ParamName;
            Assert.AreEqual("CalculationConvergence", paramName);
        }
    }
}