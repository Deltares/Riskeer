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
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data.Properties;

namespace Ringtoets.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsInputTest
    {
        private static IEnumerable<TestCaseData> WaterLevels
        {
            get
            {
                yield return new TestCaseData(0.5, 6.10, 3.58, 5.88, 3.40, 5.99, new[]
                {
                    new RoundedDouble(2, 3.58),
                    new RoundedDouble(2, 4),
                    new RoundedDouble(2, 4.5),
                    new RoundedDouble(2, 5),
                    new RoundedDouble(2, 5.5),
                    new RoundedDouble(2, 5.88)
                });

                yield return new TestCaseData(1, 6.01, -1.30, 5.80, -1.20, 6.01, new[]
                {
                    new RoundedDouble(2, -1.20),
                    new RoundedDouble(2, -1),
                    new RoundedDouble(2),
                    new RoundedDouble(2, 1),
                    new RoundedDouble(2, 2),
                    new RoundedDouble(2, 3),
                    new RoundedDouble(2, 4),
                    new RoundedDouble(2, 5),
                    new RoundedDouble(2, 5.80)
                });

                yield return new TestCaseData(2, 8.67, -4.29, 8.58, -4.29, 8.58, new[]
                {
                    new RoundedDouble(2, -4.29),
                    new RoundedDouble(2, -4),
                    new RoundedDouble(2, -2),
                    new RoundedDouble(2),
                    new RoundedDouble(2, 2),
                    new RoundedDouble(2, 4),
                    new RoundedDouble(2, 6),
                    new RoundedDouble(2, 8),
                    new RoundedDouble(2, 8.57)
                });
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new WaveConditionsInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.IsNull(input.DikeProfile);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, input.BreakWater.Type);
            Assert.AreEqual(new RoundedDouble(2), input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(new RoundedDouble(2, double.NaN), input.UpperBoundaryRevetment);
            Assert.AreEqual(new RoundedDouble(2, double.NaN), input.LowerBoundaryRevetment);
            Assert.AreEqual(new RoundedDouble(1, double.NaN), input.StepSize);
            Assert.AreEqual(new RoundedDouble(2, double.NaN), input.UpperBoundaryWaterLevels);
            Assert.AreEqual(new RoundedDouble(2, double.NaN), input.LowerBoundaryWaterLevels);
            CollectionAssert.IsEmpty(input.WaterLevels);
        }

        [Test]
        [Combinatorial]
        public void DikeProfile_SetNewValue_InputSyncedAccordingly(
            [Values(true, false)] bool withBreakWater,
            [Values(true, false)] bool withValidForeshore)
        {
            // Setup
            var input = new WaveConditionsInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreShoreGeometry = new List<Point2D>
            {
                new Point2D(2.2, 3.3)
            };

            if (withValidForeshore)
            {
                foreShoreGeometry.Add(new Point2D(4.4, 5.5));
            }

            BreakWater breakWater = null;
            if (withBreakWater)
            {
                var nonDefaultBreakWaterType = BreakWaterType.Wall;
                var nonDefaultBreakWaterHeight = 5.5;

                // Precondition
                Assert.AreNotEqual(nonDefaultBreakWaterType, input.BreakWater.Type);
                Assert.AreNotEqual(nonDefaultBreakWaterHeight, input.BreakWater.Height);

                breakWater = new BreakWater(nonDefaultBreakWaterType, nonDefaultBreakWaterHeight);
            }

            var dikeProfile = new DikeProfile(new Point2D(0, 0),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(6.6, 7.7), 0.8)
                                              }, foreShoreGeometry.ToArray(), breakWater,
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Orientation = 1.1,
                                                  DikeHeight = 4.4
                                              });

            // Call
            input.DikeProfile = dikeProfile;

            // Assert
            Assert.AreSame(dikeProfile, input.DikeProfile);
            Assert.AreEqual(withBreakWater, input.UseBreakWater);
            Assert.AreEqual(withBreakWater ? dikeProfile.BreakWater.Type : originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(withBreakWater ? dikeProfile.BreakWater.Height : originalBreakWaterHeight, input.BreakWater.Height);
            Assert.AreEqual(withValidForeshore, input.UseForeshore);
            CollectionAssert.AreEqual(dikeProfile.ForeshoreGeometry, input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void DikeProfile_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            var input = new WaveConditionsInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var dikeProfile = new DikeProfile(new Point2D(0, 0),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(7.7, 8.8), 0.6)
                                              }, new[]
                                              {
                                                  new Point2D(3.3, 4.4),
                                                  new Point2D(5.5, 6.6)
                                              },
                                              new BreakWater(BreakWaterType.Caisson, 2.2),
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Orientation = 1.1,
                                                  DikeHeight = 9.9
                                              });

            input.DikeProfile = dikeProfile;

            // Precondition
            Assert.AreSame(dikeProfile, input.DikeProfile);
            Assert.IsTrue(input.UseBreakWater);
            Assert.AreNotEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreNotEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsTrue(input.UseForeshore);
            CollectionAssert.IsNotEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);

            // Call
            input.DikeProfile = null;

            // Assert
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
        }

        [Test]
        public void LowerBoundaryRevetment_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.LowerBoundaryRevetment.NumberOfDecimalPlaces;

            // Call
            input.LowerBoundaryRevetment = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.LowerBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.LowerBoundaryRevetment.Value);
        }

        [Test]
        [TestCase(4.0, 5.0)]
        [TestCase(4.0, double.NaN)]
        [TestCase(double.NaN, 5.0)]
        [TestCase(double.NaN, double.NaN)]
        public void LowerBoundaryRevetment_ValidValueAccordingtoUpperBoundaryRevetment_ValueIsSet(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment
            };

            // Call
            input.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment;

            // Assert
            Assert.AreEqual(lowerBoundaryRevetment, input.LowerBoundaryRevetment, input.LowerBoundaryRevetment.GetAccuracy());
        }

        [Test]
        [TestCase(5.0, 4.0)]
        [TestCase(4.0, 4.0)]
        [TestCase(3.995, 4.0)]
        public void LowerBoundaryRevetment_InvalidValueAccordingtoUpperBoundaryRevetment_ThrowsArgumentOutOfRangeException(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment
            };

            // Call
            TestDelegate test = () => input.LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment;

            // Assert
            string expectedMessage = Resources.WaveConditionsInput_ValidateRevetmentBoundaries_Upper_boundary_revetment_must_be_above_lower_boundary_revetment;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        public void UpperBoundaryRevetment_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.UpperBoundaryRevetment.NumberOfDecimalPlaces;

            // Call
            input.UpperBoundaryRevetment = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.UpperBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.UpperBoundaryRevetment.Value);
        }

        [Test]
        [TestCase(4.0, 5.0)]
        [TestCase(4.0, double.NaN)]
        [TestCase(double.NaN, 5.0)]
        [TestCase(double.NaN, double.NaN)]
        public void UpperBoundaryRevetment_ValidValueAccordingtoLowerBoundaryRevetment_ValueIsSet(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment
            };

            // Call
            input.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment;

            // Assert
            Assert.AreEqual(upperBoundaryRevetment, input.UpperBoundaryRevetment, input.UpperBoundaryRevetment.GetAccuracy());
        }

        [Test]
        [TestCase(4.0, 3.0)]
        [TestCase(4.0, 4.0)]
        [TestCase(4.0, 4.004)]
        public void UpperBoundaryRevetment_InvalidValueAccordingtoLowerBoundaryRevetment_ThrowsArgumentOutOfRangeException(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment
            };

            // Call
            TestDelegate test = () => input.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment;

            // Assert
            string expectedMessage = Resources.WaveConditionsInput_ValidateRevetmentBoundaries_Upper_boundary_revetment_must_be_above_lower_boundary_revetment;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        public void StepSize_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.StepSize.NumberOfDecimalPlaces;

            // Call
            input.StepSize = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.StepSize.NumberOfDecimalPlaces);
            Assert.AreEqual(1.2, input.StepSize.Value);
        }

        [Test]
        public void LowerBoundaryWaterLevels_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.LowerBoundaryWaterLevels.NumberOfDecimalPlaces;

            // Call
            input.LowerBoundaryWaterLevels = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.LowerBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.LowerBoundaryWaterLevels.Value);
        }

        [Test]
        [TestCase(4.0, 5.0)]
        [TestCase(4.0, double.NaN)]
        [TestCase(double.NaN, 5.0)]
        [TestCase(double.NaN, double.NaN)]
        public void LowerBoundaryWaterLevels_ValidValueAccordingtoUpperBoundaryWaterLevels_ValueIsSet(double lowerBoundaryWaterLevels, double upperBoundaryWaterLevels)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperBoundaryWaterLevels = (RoundedDouble) upperBoundaryWaterLevels
            };

            // Call
            input.LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels;

            // Assert
            Assert.AreEqual(lowerBoundaryWaterLevels, input.LowerBoundaryWaterLevels, input.LowerBoundaryWaterLevels.GetAccuracy());
        }

        [Test]
        [TestCase(5.0, 4.0)]
        [TestCase(4.0, 4.0)]
        [TestCase(3.995, 4.0)]
        public void LowerBoundaryWaterLevels_InvalidValueAccordingtoUpperBoundaryWaterLevels_ThrowsArgumentOutOfRangeException(double lowerBoundaryWaterLevels, double upperBoundaryWaterLevels)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                UpperBoundaryWaterLevels = (RoundedDouble) upperBoundaryWaterLevels
            };

            // Call
            TestDelegate test = () => input.LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels;

            // Assert
            string expectedMessage = Resources.WaveConditionsInput_ValidateWaterLevelBoundaries_Upper_boundary_water_levels_must_be_above_lower_boundary_water_levels;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        public void UpperBoundaryWaterLevels_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.UpperBoundaryWaterLevels.NumberOfDecimalPlaces;

            // Call
            input.UpperBoundaryWaterLevels = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.UpperBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.UpperBoundaryWaterLevels.Value);
        }

        [Test]
        [TestCase(4.0, 5.0)]
        [TestCase(4.0, double.NaN)]
        [TestCase(double.NaN, 5.0)]
        [TestCase(double.NaN, double.NaN)]
        public void UpperBoundaryWaterLevels_ValidValueAccordingtoLowerBoundaryWaterLevels_ValueIsSet(double lowerBoundaryWaterLevels, double upperBoundaryWaterLevels)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels
            };

            // Call
            input.UpperBoundaryWaterLevels = (RoundedDouble) upperBoundaryWaterLevels;

            // Assert
            Assert.AreEqual(upperBoundaryWaterLevels, input.UpperBoundaryWaterLevels, input.UpperBoundaryWaterLevels.GetAccuracy());
        }

        [Test]
        [TestCase(4.0, 3.0)]
        [TestCase(4.0, 4.0)]
        [TestCase(4.0, 4.004)]
        public void UpperBoundaryWaterLevels_InvalidValueAccordingtoLowerBoundaryWaterLevels_ThrowsArgumentOutOfRangeException(double lowerBoundaryWaterLevels, double upperBoundaryWaterLevels)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryWaterLevels
            };

            // Call
            TestDelegate test = () => input.UpperBoundaryRevetment = (RoundedDouble) upperBoundaryWaterLevels;

            // Assert
            string expectedMessage = Resources.WaveConditionsInput_ValidateRevetmentBoundaries_Upper_boundary_revetment_must_be_above_lower_boundary_revetment;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCase(0.1, 0.02, 0)]
        [TestCase(double.NaN, double.NaN, double.NaN)]
        [TestCase(1, 8.01, 7.99)]
        [TestCase(2, 3.52, 3.5)]
        public void WaterLevels_InvalidData_NoWaterLevels(double stepSize, double upperBoundaryLevel, double lowerBoundaryLevel)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                StepSize = (RoundedDouble) stepSize,
                UpperBoundaryRevetment = (RoundedDouble) (upperBoundaryLevel - 0.01),
                UpperBoundaryWaterLevels = (RoundedDouble) (upperBoundaryLevel - 0.01),
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) upperBoundaryLevel
                },
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryLevel,
                LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryLevel
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = input.WaterLevels;

            // Assert
            CollectionAssert.IsEmpty(waterLevels);
        }

        [Test]
        [TestCaseSource("WaterLevels")]
        public void WaterLevels_ValidData_ReturnsWaterLevels(double stepSize, double upperBoundaryRevetment,
                                                             double lowerBoundaryRevetment, double upperBoundaryWaterLevels, double lowerBoundaryWaterLevels,
                                                             double designWaterLevel, IEnumerable<RoundedDouble> expectedWaterLevels)
        {
            // Setup
            var input = new WaveConditionsInput
            {
                StepSize = (RoundedDouble) stepSize,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                UpperBoundaryWaterLevels = (RoundedDouble) upperBoundaryWaterLevels,
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) designWaterLevel
                },
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = input.WaterLevels;

            // Assert
            Assert.AreEqual(expectedWaterLevels, waterLevels);
        }
    }
}