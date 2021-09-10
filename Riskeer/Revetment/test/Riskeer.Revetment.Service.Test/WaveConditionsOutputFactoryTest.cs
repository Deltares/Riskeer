// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Core.Common.Util;
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
        [TestCase(true, CalculationConvergence.CalculatedConverged)]
        [TestCase(false, CalculationConvergence.CalculatedNotConverged)]
        [TestCase(null, CalculationConvergence.NotCalculated)]
        public void CreateOutput_ValidParameters_ReturnsExpectedValues(bool? convergence, CalculationConvergence expectedConvergence)
        {
            // Setup
            const double waterLevel = 1.1;
            const double waveHeight = 2.2;
            const double wavePeakPeriod = 3.3;
            const double waveAngle = 4.4;
            const double waveDirection = 5.5;
            const double targetProbability = 1 / 6.6;
            const double calculatedReliability = -7.7;

            // Call 
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateOutput(waterLevel, waveHeight, wavePeakPeriod,
                                                                                   waveAngle, waveDirection, targetProbability,
                                                                                   calculatedReliability, convergence);

            // Assert
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeakPeriod, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(waveAngle, output.WaveAngle, output.WaveAngle.GetAccuracy());
            Assert.AreEqual(waveDirection, output.WaveDirection, output.WaveDirection.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability, 1e-6);
            Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(calculatedReliability), output.CalculatedProbability, 1e-6);
            Assert.AreEqual(expectedConvergence, output.CalculationConvergence);
        }

        [Test]
        public void CreateFailedOutput_ValidParameters_ReturnsExpectedValues()
        {
            // Setup
            const double waterLevel = 1.1;
            const double targetProbability = 1 / 2.2;

            // Call
            WaveConditionsOutput output = WaveConditionsOutputFactory.CreateFailedOutput(waterLevel, targetProbability);

            // Assert
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.IsNaN(output.WaveHeight);
            Assert.IsNaN(output.WavePeakPeriod);
            Assert.IsNaN(output.WaveAngle);
            Assert.IsNaN(output.WaveDirection);
            Assert.AreEqual(targetProbability, output.TargetProbability, 1e-6);
            Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, output.CalculationConvergence);
        }
    }
}