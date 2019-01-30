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

using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Revetment.Data.TestUtil
{
    /// <summary>
    /// A test helper to assert <see cref="WaveConditionsOutput"/>.
    /// </summary>
    public static class WaveConditionsOutputTestHelper
    {
        /// <summary>
        /// Asserts the <see cref="WaveConditionsOutput"/> after a calculation has failed.
        /// </summary>
        /// <param name="waterLevel">The water level for which the calculation was performed for.</param>
        /// <param name="targetNorm">The norm for which the calculation was calculated for.</param>
        /// <param name="actualOutput">The actual <see cref="WaveConditionsOutput"/>.</param>
        /// <exception cref="AssertionException">Thrown when differences are found between 
        /// the expected output and the actual output.</exception>
        public static void AssertFailedOutput(double waterLevel,
                                              double targetNorm,
                                              WaveConditionsOutput actualOutput)
        {
            double targetReliability = StatisticsConverter.ProbabilityToReliability(targetNorm);
            double targetProbability = StatisticsConverter.ReliabilityToProbability(targetReliability);

            Assert.IsNotNull(actualOutput);
            Assert.AreEqual(waterLevel, actualOutput.WaterLevel, actualOutput.WaterLevel.GetAccuracy());
            Assert.IsNaN(actualOutput.WaveHeight);
            Assert.IsNaN(actualOutput.WavePeakPeriod);
            Assert.IsNaN(actualOutput.WaveAngle);
            Assert.IsNaN(actualOutput.WaveDirection);
            Assert.AreEqual(targetProbability, actualOutput.TargetProbability);
            Assert.AreEqual(targetReliability, actualOutput.TargetReliability, actualOutput.TargetReliability.GetAccuracy());
            Assert.IsNaN(actualOutput.CalculatedProbability);
            Assert.IsNaN(actualOutput.CalculatedReliability);
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, actualOutput.CalculationConvergence);
        }
    }
}