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

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new WaveConditionsInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.IsNull(input.ForeshoreProfile);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, input.BreakWater.Type);
            Assert.AreEqual(new RoundedDouble(2), input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(0, input.Orientation.Value);
            Assert.AreEqual(2, input.Orientation.NumberOfDecimalPlaces);
            Assert.IsNaN(input.AssessmentLevel.Value);
            Assert.AreEqual(2, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.IsNaN(input.UpperBoundaryDesignWaterLevel.Value);
            Assert.AreEqual(2, input.UpperBoundaryDesignWaterLevel.NumberOfDecimalPlaces);
            Assert.IsNaN(input.LowerBoundaryRevetment.Value);
            Assert.AreEqual(2, input.LowerBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.IsNaN(input.UpperBoundaryRevetment.Value);
            Assert.AreEqual(2, input.UpperBoundaryRevetment.NumberOfDecimalPlaces);
            Assert.IsNaN(input.LowerBoundaryWaterLevels.Value);
            Assert.AreEqual(2, input.LowerBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.IsNaN(input.UpperBoundaryWaterLevels.Value);
            Assert.AreEqual(2, input.UpperBoundaryWaterLevels.NumberOfDecimalPlaces);
            Assert.AreEqual(WaveConditionsInputStepSize.Half, input.StepSize);
            CollectionAssert.IsEmpty(input.WaterLevels);
        }

        [Test]
        public void HydraulicBoundaryLocation_SetNewValue_AssessmentLevelAndUpperBoundaryDesignWaterLevelExpectedValue()
        {
            // Setup
            var input = new WaveConditionsInput();
            const double assessmentLevel = 3.2;

            // Call
            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0)
            {
                DesignWaterLevel = (RoundedDouble) assessmentLevel
            };

            // Assert
            Assert.AreEqual(assessmentLevel, input.AssessmentLevel.Value, input.UpperBoundaryDesignWaterLevel.GetAccuracy());
            Assert.AreEqual(assessmentLevel - 0.01, input.UpperBoundaryDesignWaterLevel.Value, input.UpperBoundaryDesignWaterLevel.GetAccuracy());
        }

        [Test]
        public void HydraulicBoundaryLocation_SetNullValue_AssessmentLevelAndUpperBoundaryDesignWaterLevelNaN()
        {
            // Setup
            var input = new WaveConditionsInput()
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 3.2
                }
            };

            // Call
            input.HydraulicBoundaryLocation = null;

            // Assert
            Assert.IsNaN(input.AssessmentLevel.Value);
            Assert.IsNaN(input.UpperBoundaryDesignWaterLevel.Value);
        }

        [Test]
        [Combinatorial]
        public void ForeshoreProfile_SetNewValue_InputSyncedAccordingly(
            [Values(true, false)] bool withBreakWater,
            [Values(true, false)] bool withValidForeshore)
        {
            // Setup
            var input = new WaveConditionsInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreshoreGeometry = new List<Point2D>
            {
                new Point2D(2.2, 3.3)
            };

            if (withValidForeshore)
            {
                foreshoreGeometry.Add(new Point2D(4.4, 5.5));
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

            double orientation = 96;
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        foreshoreGeometry.ToArray(),
                                                        breakWater,
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Orientation = orientation
                                                        });

            // Call
            input.ForeshoreProfile = foreshoreProfile;

            // Assert
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.AreEqual(withBreakWater, input.UseBreakWater);
            Assert.AreEqual(withBreakWater ? foreshoreProfile.BreakWater.Type : originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(withBreakWater ? foreshoreProfile.BreakWater.Height : originalBreakWaterHeight, input.BreakWater.Height);
            Assert.AreEqual(withValidForeshore, input.UseForeshore);
            CollectionAssert.AreEqual(foreshoreProfile.Geometry, input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(orientation, input.Orientation.Value);
            Assert.AreEqual(2, input.Orientation.NumberOfDecimalPlaces);
        }

        [Test]
        public void Foreshore_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            var input = new WaveConditionsInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            HydraulicBoundaryLocation originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        new[]
                                                        {
                                                            new Point2D(3.3, 4.4),
                                                            new Point2D(5.5, 6.6)
                                                        },
                                                        new BreakWater(BreakWaterType.Caisson, 2.2),
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Orientation = 96
                                                        });

            input.ForeshoreProfile = foreshoreProfile;

            // Precondition
            Assert.AreSame(foreshoreProfile, input.ForeshoreProfile);
            Assert.IsTrue(input.UseBreakWater);
            Assert.AreNotEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreNotEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsTrue(input.UseForeshore);
            CollectionAssert.IsNotEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);

            // Call
            input.ForeshoreProfile = null;

            // Assert
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(0, input.Orientation.Value);
            Assert.AreEqual(2, input.Orientation.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-0.004)]
        public void Orientation_ValidValue_NewValueSet(double orientation)
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            input.Orientation = (RoundedDouble) orientation;

            // Assert
            Assert.AreEqual(orientation, input.Orientation, input.Orientation.GetAccuracy());
        }

        [Test]
        [TestCase(400)]
        [TestCase(360.05)]
        [TestCase(-0.005)]
        [TestCase(-23)]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Orientation_InvalidValue_ThrowsArgumentOutOfRangeException(double invalidValue)
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            TestDelegate call = () => input.Orientation = (RoundedDouble) invalidValue;

            // Assert
            string paramName = Assert.Throws<ArgumentOutOfRangeException>(call).ParamName;
            Assert.AreEqual("value", paramName);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de oriëntatie moet in het bereik tussen [0, 360] graden liggen.");
        }

        [Test]
        public void UpperBoundaryDesignWaterLevel_NoHydraulicBoundaryLocation_ReturnNaN()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();

            // Call
            waveConditionsInput.HydraulicBoundaryLocation = null;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, double.NaN), waveConditionsInput.UpperBoundaryDesignWaterLevel);
        }

        [Test]
        public void UpperBoundaryDesignWaterLevel_NoDesignWaterLevel_ReturnNaN()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput();

            // Call
            waveConditionsInput.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            // Assert
            Assert.AreEqual(new RoundedDouble(2, double.NaN), waveConditionsInput.UpperBoundaryDesignWaterLevel);
        }

        [Test]
        public void UpperBoundaryDesignWaterLevel_DesignWaterLevelSet_ReturnValueJustBelowDesignWaterLevel()
        {
            // Setup
            var designWaterLevel = (RoundedDouble) 1.0;
            var waveConditionsInput = new WaveConditionsInput();

            // Call
            waveConditionsInput.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            {
                DesignWaterLevel = designWaterLevel
            };

            // Assert
            Assert.AreEqual(new RoundedDouble(2, designWaterLevel - 0.01), waveConditionsInput.UpperBoundaryDesignWaterLevel);
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
        [TestCase(4.994, 5.0)]
        [TestCase(4.0, double.NaN)]
        [TestCase(double.NaN, 5.0)]
        [TestCase(double.NaN, double.NaN)]
        public void LowerBoundaryRevetment_ValidValueAccordingtoUpperBoundaryRevetment_ValueIsSet(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var input = new WaveConditionsInput()
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
            var input = new WaveConditionsInput()
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
        [TestCase(-50.005)]
        [TestCase(-100)]
        public void LowerBoundaryRevetment_BoundarySmallerThanValid_SetValueToValidBoundary(double newValue)
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            input.LowerBoundaryRevetment = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(-50, input.LowerBoundaryRevetment, input.LowerBoundaryRevetment.GetAccuracy());
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
        [TestCase(4.0, 4.005)]
        [TestCase(4.0, double.NaN)]
        [TestCase(double.NaN, 5.0)]
        [TestCase(double.NaN, double.NaN)]
        public void UpperBoundaryRevetment_ValidValueAccordingtoLowerBoundaryRevetment_ValueIsSet(double lowerBoundaryRevetment, double upperBoundaryRevetment)
        {
            // Setup
            var input = new WaveConditionsInput()
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
            var input = new WaveConditionsInput()
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
        [TestCase(1000.005)]
        [TestCase(1030)]
        public void UpperBoundaryRevetment_BoundaryLargerThanValid_SetValueToValidBoundary(double newValue)
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            input.UpperBoundaryRevetment = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(1000, input.UpperBoundaryRevetment, input.UpperBoundaryRevetment.GetAccuracy());
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
        [TestCase(4.994, 5.0)]
        [TestCase(4.0, double.NaN)]
        [TestCase(double.NaN, 5.0)]
        [TestCase(double.NaN, double.NaN)]
        public void LowerBoundaryWaterLevels_ValidValueAccordingtoUpperBoundaryWaterLevels_ValueIsSet(double lowerBoundaryWaterLevels, double upperBoundaryWaterLevels)
        {
            // Setup
            var input = new WaveConditionsInput()
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
            var input = new WaveConditionsInput()
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
        [TestCase(-50.005)]
        [TestCase(-100)]
        public void LowerBoundaryWaterLevels_BoundarySmallerThanValid_SetValueToValidBoundary(double newValue)
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            input.LowerBoundaryWaterLevels = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(-50, input.LowerBoundaryWaterLevels, input.LowerBoundaryWaterLevels.GetAccuracy());
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
        [TestCase(4.0, 4.005)]
        [TestCase(4.0, double.NaN)]
        [TestCase(double.NaN, 5.0)]
        [TestCase(double.NaN, double.NaN)]
        public void UpperBoundaryWaterLevels_ValidValueAccordingtoLowerBoundaryWaterLevels_ValueIsSet(double lowerBoundaryWaterLevels, double upperBoundaryWaterLevels)
        {
            // Setup
            var input = new WaveConditionsInput()
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
            var input = new WaveConditionsInput()
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
        [TestCase(1000.005)]
        [TestCase(1030)]
        public void UpperBoundaryWaterLevels_BoundaryLargerThanValid_SetValueToValidBoundary(double newValue)
        {
            // Setup
            var input = new WaveConditionsInput();

            // Call
            input.UpperBoundaryWaterLevels = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(1000, input.UpperBoundaryWaterLevels, input.UpperBoundaryWaterLevels.GetAccuracy());
        }

        [Test]
        [TestCase(double.NaN, 10.0, 12.0)]
        [TestCase(1.0, double.NaN, 12.0)]
        [TestCase(1.0, 10.0, double.NaN)]
        public void WaterLevels_InvalidInput_NoWaterLevels(double lowerBoundaryRevetments, double upperBoundaryRevetments, double designWaterLevel)
        {
            // Setup
            var input = new WaveConditionsInput()
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) designWaterLevel
                },
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetments,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetments,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = input.WaterLevels;

            // Assert
            CollectionAssert.IsEmpty(waterLevels);
        }

        [Test]
        public void WaterLevels_AllBoundariesAboveUpperBoundaryDesignWaterLevel_NoWaterLevels()
        {
            // Setup
            var input = new WaveConditionsInput()
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) 5.78
                },
                LowerBoundaryRevetment = (RoundedDouble) 6,
                UpperBoundaryRevetment = (RoundedDouble) 6.10,
                LowerBoundaryWaterLevels = (RoundedDouble) 6.20,
                UpperBoundaryWaterLevels = (RoundedDouble) 10,
                StepSize = WaveConditionsInputStepSize.Half
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = input.WaterLevels;

            // Assert
            CollectionAssert.IsEmpty(waterLevels);
        }

        [Test]
        public void WaterLevels_HydraulicBoundaryLocationNull_NoWaterLevels()
        {
            // Setup
            var input = new WaveConditionsInput()
            {
                LowerBoundaryRevetment = (RoundedDouble) 1.0,
                UpperBoundaryRevetment = (RoundedDouble) 10.0,
                StepSize = WaveConditionsInputStepSize.One,
                LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                UpperBoundaryWaterLevels = (RoundedDouble) 10.0
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = input.WaterLevels;

            // Assert
            CollectionAssert.IsEmpty(waterLevels);
        }

        [Test]
        [TestCaseSource("WaterLevels")]
        public void WaterLevels_ValidInput_ReturnsWaterLevels(WaveConditionsInputStepSize stepSize, double lowerBoundaryRevetment, double upperBoundaryRevetment,
                                                              double lowerBoundaryWaterLevels, double upperBoundaryWaterLevels,
                                                              double designWaterLevel, IEnumerable<RoundedDouble> expectedWaterLevels)
        {
            // Setup
            var input = new WaveConditionsInput()
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = (RoundedDouble) designWaterLevel
                },
                LowerBoundaryRevetment = (RoundedDouble) lowerBoundaryRevetment,
                UpperBoundaryRevetment = (RoundedDouble) upperBoundaryRevetment,
                StepSize = stepSize,
                LowerBoundaryWaterLevels = (RoundedDouble) lowerBoundaryWaterLevels,
                UpperBoundaryWaterLevels = (RoundedDouble) upperBoundaryWaterLevels
            };

            // Call
            IEnumerable<RoundedDouble> waterLevels = input.WaterLevels;

            // Assert
            CollectionAssert.AreEqual(expectedWaterLevels, waterLevels);
        }
    }
}