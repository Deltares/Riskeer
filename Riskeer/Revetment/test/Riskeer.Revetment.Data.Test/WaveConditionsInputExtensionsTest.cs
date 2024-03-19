// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base.Data;
using NUnit.Framework;

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
            var waveConditionsInput = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetments,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetments,
                StepSize = (RoundedDouble) 1.0,
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
            var waveConditionsInput = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) 6,
                UpperBoundaryRevetment = (RoundedDouble) 6.10,
                LowerBoundaryWaterLevels = (RoundedDouble) 6.20,
                UpperBoundaryWaterLevels = (RoundedDouble) 10,
                StepSize = (RoundedDouble) 0.5
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
            var waveConditionsInput = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = (RoundedDouble) 1.0,
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
        public void GetWaterLevels_ValidInput_ReturnsExpectedWaterLevels(double stepSize,
                                                                         double lowerBoundaryRevetment,
                                                                         double upperBoundaryRevetment,
                                                                         double lowerBoundaryWaterLevels,
                                                                         double upperBoundaryWaterLevels,
                                                                         double assessmentLevel,
                                                                         IEnumerable<RoundedDouble> expectedWaterLevels)
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                StepSize = (RoundedDouble) stepSize,
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
            yield return new TestCaseData(2, 2.58, 6.10, 2.40, 3.89, 5.99, new[]
            {
                new RoundedDouble(2, 3.89),
                new RoundedDouble(2, 2.58)
            });

            yield return new TestCaseData(0.5, 3.58, 6.10, 3.40, 5.88, 5.99, new[]
            {
                new RoundedDouble(2, 5.88),
                new RoundedDouble(2, 5.5),
                new RoundedDouble(2, 5),
                new RoundedDouble(2, 4.5),
                new RoundedDouble(2, 4),
                new RoundedDouble(2, 3.58)
            });

            yield return new TestCaseData(1, -1.30, 5.80, -1.20, 6.01, 6.10, new[]
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

            yield return new TestCaseData(2, -4.29, 8.67, -4.29, 8.58, 8.58, new[]
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

            yield return new TestCaseData(2, -4.29, 8.67, double.NaN, double.NaN, 8.58, new[]
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
            
            yield return new TestCaseData(0.28, -0.6, 2.90, -0.5, 2.80, 2.0, new[]
            {
                new RoundedDouble(2, 1.99),
                new RoundedDouble(2, 1.96),
                new RoundedDouble(2, 1.68),
                new RoundedDouble(2, 1.4),
                new RoundedDouble(2, 1.12),
                new RoundedDouble(2, 0.84),
                new RoundedDouble(2, 0.56),
                new RoundedDouble(2, 0.28),
                new RoundedDouble(2),
                new RoundedDouble(2, -0.28),
                new RoundedDouble(2, -0.5)
            });
            
            yield return new TestCaseData(0.45, 0, 2.37, 0.18, 2.80, 2.5, new[]
            {
                new RoundedDouble(2, 2.37),
                new RoundedDouble(2, 2.25),
                new RoundedDouble(2, 1.8),
                new RoundedDouble(2, 1.35),
                new RoundedDouble(2, 0.9),
                new RoundedDouble(2, 0.45),
                new RoundedDouble(2, 0.18)
            });
            
            yield return new TestCaseData(0.33, -4, -1.3, -3.89, -1.54, -1.4, new[]
            {
                new RoundedDouble(2, -1.54),
                new RoundedDouble(2, -1.65),
                new RoundedDouble(2, -1.98),
                new RoundedDouble(2, -2.31),
                new RoundedDouble(2, -2.64),
                new RoundedDouble(2, -2.97),
                new RoundedDouble(2, -3.3),
                new RoundedDouble(2, -3.63),
                new RoundedDouble(2, -3.89)
            });
            
            yield return new TestCaseData(1.9, 6, 14, 5, 15, 10, new[]
            {
                new RoundedDouble(2, 9.99),
                new RoundedDouble(2, 9.5),
                new RoundedDouble(2, 7.6),
                new RoundedDouble(2, 6)
            });
            
            yield return new TestCaseData(0.01, 1, 1.13, double.NaN, double.NaN, 1.15, new[]
            {
                new RoundedDouble(2, 1.13),
                new RoundedDouble(2, 1.12),
                new RoundedDouble(2, 1.11),
                new RoundedDouble(2, 1.10),
                new RoundedDouble(2, 1.09),
                new RoundedDouble(2, 1.08),
                new RoundedDouble(2, 1.07),
                new RoundedDouble(2, 1.06),
                new RoundedDouble(2, 1.05),
                new RoundedDouble(2, 1.04),
                new RoundedDouble(2, 1.03),
                new RoundedDouble(2, 1.02),
                new RoundedDouble(2, 1.01),
                new RoundedDouble(2, 1)

            });
        }
    }
}