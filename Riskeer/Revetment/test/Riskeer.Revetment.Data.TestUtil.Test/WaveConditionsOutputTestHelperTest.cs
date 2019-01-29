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
using System.Collections.Generic;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Revetment.Data.TestUtil.Test
{
    [TestFixture]
    public class WaveConditionsOutputTestHelperTest
    {
        [Test]
        public void AssertFailedOutput_AllValuesMatch_DoesNotThrowException()
        {
            // Setup
            var random = new Random(21);
            double norm = random.NextDouble();
            double waterLevel = random.NextDouble();

            double targetReliability = StatisticsConverter.ProbabilityToReliability(norm);
            double targetProbability = StatisticsConverter.ReliabilityToProbability(targetReliability);

            var output = new WaveConditionsOutput(waterLevel,
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  targetProbability,
                                                  targetReliability,
                                                  double.NaN,
                                                  double.NaN,
                                                  CalculationConvergence.CalculatedNotConverged);

            // Call
            TestDelegate call = () =>
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevel, norm, output);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void AssertFailedOutput_ActualOutputNull_ThrowsAssertionException()
        {
            // Setup
            var random = new Random(21);
            double norm = random.NextDouble();
            double waterLevel = random.NextDouble();

            // Call
            TestDelegate call = () =>
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevel, norm, null);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertFailedOutput_DifferentWaterLevel_ThrowsAssertionException()
        {
            // Setup
            var random = new Random(21);
            double norm = random.NextDouble();
            double waterLevel = random.NextDouble();

            double targetReliability = StatisticsConverter.ProbabilityToReliability(norm);
            double targetProbability = StatisticsConverter.ReliabilityToProbability(targetReliability);

            var output = new WaveConditionsOutput(waterLevel + random.NextDouble(),
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  targetProbability,
                                                  targetReliability,
                                                  double.NaN,
                                                  double.NaN,
                                                  CalculationConvergence.CalculatedNotConverged);

            // Call
            TestDelegate call = () =>
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevel, norm, output);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertFailedOutput_DifferentTargetProbability_ThrowsAssertionException()
        {
            // Setup
            var random = new Random(21);
            double norm = random.NextDouble();
            double waterLevel = random.NextDouble();

            double targetReliability = StatisticsConverter.ProbabilityToReliability(norm);

            var output = new WaveConditionsOutput(waterLevel,
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  random.NextDouble(),
                                                  targetReliability,
                                                  double.NaN,
                                                  double.NaN,
                                                  CalculationConvergence.CalculatedNotConverged);

            // Call
            TestDelegate call = () =>
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevel, norm, output);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertFailedOutput_DifferentTargetReliability_ThrowsAssertionException()
        {
            // Setup
            var random = new Random(21);
            double norm = random.NextDouble();
            double waterLevel = random.NextDouble();

            double targetReliability = StatisticsConverter.ProbabilityToReliability(norm);
            double targetProbability = StatisticsConverter.ReliabilityToProbability(targetReliability);

            var output = new WaveConditionsOutput(waterLevel,
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  targetProbability,
                                                  targetReliability + random.NextDouble(),
                                                  double.NaN,
                                                  double.NaN,
                                                  CalculationConvergence.CalculatedNotConverged);

            // Call
            TestDelegate call = () =>
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevel, norm, output);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCase(CalculationConvergence.CalculatedConverged)]
        [TestCase(CalculationConvergence.NotCalculated)]
        public void AssertFailedOutput_DifferentCalculationConvergence_ThrowsAssertionException(
            CalculationConvergence convergence)
        {
            // Setup
            var random = new Random(21);
            double norm = random.NextDouble();
            double waterLevel = random.NextDouble();

            double targetReliability = StatisticsConverter.ProbabilityToReliability(norm);
            double targetProbability = StatisticsConverter.ReliabilityToProbability(targetReliability);

            var output = new WaveConditionsOutput(waterLevel,
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  double.NaN,
                                                  targetProbability,
                                                  targetReliability,
                                                  double.NaN,
                                                  double.NaN,
                                                  convergence);

            // Call
            TestDelegate call = () =>
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevel, norm, output);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCaseSource(nameof(GetVariousWaveConditionsOutputs))]
        public void AssertFailedOutput_PropertyHasDifferentValue_ThrowsAssertionException(
            WaveConditionsOutput output)
        {
            // Setup
            const double waterLevel = 0.7;
            const double norm = 0.5;

            // Call
            TestDelegate call = () =>
                WaveConditionsOutputTestHelper.AssertFailedOutput(waterLevel, norm, output);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        private static IEnumerable<TestCaseData> GetVariousWaveConditionsOutputs()
        {
            const double waterLevel = 0.7;
            const double norm = 0.5;
            const CalculationConvergence convergence = CalculationConvergence.CalculatedNotConverged;

            double targetReliability = StatisticsConverter.ProbabilityToReliability(norm);
            double targetProbability = StatisticsConverter.ReliabilityToProbability(targetReliability);

            yield return new TestCaseData(new WaveConditionsOutput(waterLevel,
                                                                   1,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   targetProbability,
                                                                   targetReliability,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   convergence))
                .SetName("WaveHeightDifferent");
            yield return new TestCaseData(new WaveConditionsOutput(waterLevel,
                                                                   double.NaN,
                                                                   2,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   targetProbability,
                                                                   targetReliability,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   convergence))
                .SetName("WavePeakPeriodDifferent");
            yield return new TestCaseData(new WaveConditionsOutput(waterLevel,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   3,
                                                                   double.NaN,
                                                                   targetProbability,
                                                                   targetReliability,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   convergence))
                .SetName("WaveAngleDifferent");
            yield return new TestCaseData(new WaveConditionsOutput(waterLevel,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   4,
                                                                   targetProbability,
                                                                   targetReliability,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   convergence))
                .SetName("DifferentWaveDirection");
            yield return new TestCaseData(new WaveConditionsOutput(waterLevel,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   targetProbability,
                                                                   targetReliability,
                                                                   0.5,
                                                                   double.NaN,
                                                                   convergence))
                .SetName("CalculatedProbabilityDifferent");
            yield return new TestCaseData(new WaveConditionsOutput(waterLevel,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   double.NaN,
                                                                   targetProbability,
                                                                   targetReliability,
                                                                   double.NaN,
                                                                   6,
                                                                   convergence))
                .SetName("CalculatedReliabilityDifferent");
        }
    }
}