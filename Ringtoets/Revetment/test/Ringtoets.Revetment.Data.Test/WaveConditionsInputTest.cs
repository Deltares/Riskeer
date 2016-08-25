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

using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsInputTest
    {
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
            Assert.AreEqual(new RoundedDouble(2), input.UpperRevetmentLevel);
            Assert.AreEqual(new RoundedDouble(2), input.LowerRevetmentLevel);
            Assert.AreEqual(new RoundedDouble(2), input.LowerWaterLevel);
            Assert.AreEqual(new RoundedDouble(2), input.UpperWaterLevel);
            Assert.AreEqual(new RoundedDouble(1), input.StepSize);
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
        public void UpperRevetmentLevel_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.UpperRevetmentLevel.NumberOfDecimalPlaces;

            // Call
            input.UpperRevetmentLevel = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.UpperRevetmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.UpperRevetmentLevel.Value);
        }

        [Test]
        public void LowerRevetmentLevel_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.LowerRevetmentLevel.NumberOfDecimalPlaces;

            // Call
            input.LowerRevetmentLevel = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.LowerRevetmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.LowerRevetmentLevel.Value);
        }

        [Test]
        public void LowerWaterLevel_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new WaveConditionsInput();

            int originalNumberOfDecimalPlaces = input.LowerWaterLevel.NumberOfDecimalPlaces;

            // Call
            input.LowerWaterLevel = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.LowerWaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.LowerWaterLevel.Value);
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
        public void HydraulicBoundaryLocation_SetNewValue_UpperWaterLevelUpdated()
        {
            // Setup
            var input = new WaveConditionsInput();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
            {
                DesignWaterLevel = new RoundedDouble(2, 6.34)
            };

            // Call
            input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Assert
            Assert.AreEqual(6.33, input.UpperWaterLevel, input.UpperWaterLevel.GetAccuracy());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void HydraulicBoundaryLocation_WithoutDesignWaterLevel_UpperWaterLevelSetToDefault(bool withDesignWaterLevel)
        {
            // Setup
            var input = new WaveConditionsInput();

            if (withDesignWaterLevel)
            {
                var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = new RoundedDouble(2, 6.34)
                };

                input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

                // Precondition
                Assert.AreEqual(6.33, input.UpperWaterLevel, input.UpperWaterLevel.GetAccuracy());
            }

            var newHydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            // Call
            input.HydraulicBoundaryLocation = newHydraulicBoundaryLocation;

            // Assert
            Assert.AreEqual(new RoundedDouble(2), input.UpperWaterLevel);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void HydraulicBoundaryLocation_HydraulicBoundaryLocationNull_UpperWaterLevelSetToDefault(bool withDesignWaterLevel)
        {
            // Setup
            var input = new WaveConditionsInput();

            if (withDesignWaterLevel)
            {
                var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0)
                {
                    DesignWaterLevel = new RoundedDouble(2, 6.34)
                };

                input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

                // Precondition
                Assert.AreEqual(6.33, input.UpperWaterLevel, input.UpperWaterLevel.GetAccuracy());
            }


            // Call
            input.HydraulicBoundaryLocation = null;

            // Assert
            Assert.AreEqual(new RoundedDouble(2), input.UpperWaterLevel);
        }
    }
}