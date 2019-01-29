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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Revetment.Data.TestUtil;

namespace Riskeer.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsInputExtensionsTest
    {
        [Test]
        public void GetWaterLevels_WaveConditionsInputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => WaveConditionsInputExtensions.GetWaterLevels(null, RoundedDouble.NaN);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("waveConditionsInput", paramName);
        }

        [Test]
        [TestCase(double.NaN, 10.0, 12.0)]
        [TestCase(1.0, double.NaN, 12.0)]
        [TestCase(1.0, 10.0, double.NaN)]
        public void GetWaterLevels_InvalidWaveConditionsInput_ReturnsEmptyEnumerable(double lowerBoundaryRevetments,
                                                                                     double upperBoundaryRevetments,
                                                                                     double assessmentLevel)
        {
            // Setup
            var waveConditionsInput = new TestWaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetments,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetments,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = waveConditionsInput.GetWaterLevels((RoundedDouble) assessmentLevel);

            // Assert
            CollectionAssert.IsEmpty(waterLevels);
        }

        [Test]
        public void GetWaterLevels_WaveConditionsInputWithWithAllBoundariesAboveUpperBoundaryAssessmentLevel_ReturnsEmptyEnumerable()
        {
            // Setup
            var waveConditionsInput = new TestWaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) 6,
                UpperBoundaryRevetment = (RoundedDouble) 6.10,
                LowerBoundaryWaterLevels = (RoundedDouble) 6.20,
                UpperBoundaryWaterLevels = (RoundedDouble) 10,
                StepSize = WaveConditionsInputStepSize.Half
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = waveConditionsInput.GetWaterLevels((RoundedDouble) 5.78);

            // Assert
            CollectionAssert.IsEmpty(waterLevels);
        }

        [Test]
        public void GetWaterLevels_AssessmentLevelNaN_ReturnsEmptyEnumerable()
        {
            // Setup
            var waveConditionsInput = new TestWaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = waveConditionsInput.GetWaterLevels(RoundedDouble.NaN);

            // Assert
            CollectionAssert.IsEmpty(waterLevels);
        }

        [Test]
        [TestCaseSource(nameof(WaterLevels))]
        public void GetWaterLevels_ValidInput_ReturnsExpectedWaterLevels(WaveConditionsInputStepSize stepSize,
                                                                         double lowerBoundaryRevetment,
                                                                         double upperBoundaryRevetment,
                                                                         double lowerBoundaryWaterLevels,
                                                                         double upperBoundaryWaterLevels,
                                                                         double assessmentLevel,
                                                                         IEnumerable<RoundedDouble> expectedWaterLevels)
        {
            // Setup
            var waveConditionsInput = new TestWaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                StepSize = stepSize,
                LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels,
                UpperBoundaryWaterLevels = (RoundedDouble) upperBoundaryWaterLevels
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = waveConditionsInput.GetWaterLevels((RoundedDouble) assessmentLevel);

            // Assert
            CollectionAssert.AreEqual(expectedWaterLevels, waterLevels);
        }

        private static IEnumerable<TestCaseData> WaterLevels()
        {
            yield return new TestCaseData(WaveConditionsInputStepSize.Two, 2.58, 6.10, 2.40, 3.89, 5.99, new[]
            {
                new RoundedDouble(2, 3.89),
                new RoundedDouble(2, 2.58)
            });

            yield return new TestCaseData(WaveConditionsInputStepSize.Half, 3.58, 6.10, 3.40, 5.88, 5.99, new[]
            {
                new RoundedDouble(2, 5.88),
                new RoundedDouble(2, 5.5),
                new RoundedDouble(2, 5),
                new RoundedDouble(2, 4.5),
                new RoundedDouble(2, 4),
                new RoundedDouble(2, 3.58)
            });

            yield return new TestCaseData(WaveConditionsInputStepSize.One, -1.30, 5.80, -1.20, 6.01, 6.10, new[]
            {
                new RoundedDouble(2, 5.80),
                new RoundedDouble(2, 5),
                new RoundedDouble(2, 4),
                new RoundedDouble(2, 3),
                new RoundedDouble(2, 2),
                new RoundedDouble(2, 1),
                new RoundedDouble(2),
                new RoundedDouble(2, -1),
                new RoundedDouble(2, -1.20)
            });

            yield return new TestCaseData(WaveConditionsInputStepSize.Two, -4.29, 8.67, -4.29, 8.58, 8.58, new[]
            {
                new RoundedDouble(2, 8.57),
                new RoundedDouble(2, 8),
                new RoundedDouble(2, 6),
                new RoundedDouble(2, 4),
                new RoundedDouble(2, 2),
                new RoundedDouble(2),
                new RoundedDouble(2, -2),
                new RoundedDouble(2, -4),
                new RoundedDouble(2, -4.29)
            });

            yield return new TestCaseData(WaveConditionsInputStepSize.Two, -4.29, 8.67, double.NaN, double.NaN, 8.58, new[]
            {
                new RoundedDouble(2, 8.57),
                new RoundedDouble(2, 8),
                new RoundedDouble(2, 6),
                new RoundedDouble(2, 4),
                new RoundedDouble(2, 2),
                new RoundedDouble(2),
                new RoundedDouble(2, -2),
                new RoundedDouble(2, -4),
                new RoundedDouble(2, -4.29)
            });
        }
    }
}