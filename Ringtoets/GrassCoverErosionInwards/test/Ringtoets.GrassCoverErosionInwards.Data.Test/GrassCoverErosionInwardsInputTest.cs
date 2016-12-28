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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup 
            var criticalFlowRate = new LogNormalDistribution(4)
            {
                Mean = (RoundedDouble) 0.004,
                StandardDeviation = (RoundedDouble) 0.0006
            };

            // Call
            var input = new GrassCoverErosionInwardsInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);
            Assert.IsInstanceOf<IUseBreakWater>(input);
            Assert.IsInstanceOf<IUseForeshore>(input);

            Assert.IsNull(input.DikeProfile);
            Assert.AreEqual(2, input.Orientation.NumberOfDecimalPlaces);
            Assert.IsNaN(input.Orientation);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, input.BreakWater.Type);
            Assert.AreEqual(new RoundedDouble(2), input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            CollectionAssert.IsEmpty(input.DikeGeometry);
            Assert.AreEqual(2, input.DikeHeight.NumberOfDecimalPlaces);
            Assert.IsNaN(input.DikeHeight);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.AreEqual(DikeHeightCalculationType.NoCalculation, input.DikeHeightCalculationType);

            DistributionAssert.AreEqual(criticalFlowRate, input.CriticalFlowRate);
        }

        [Test]
        [Combinatorial]
        public void DikeProfile_SetNewValue_InputSyncedAccordingly(
            [Values(true, false)] bool withBreakWater,
            [Values(true, false)] bool withValidForeshore)
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            LogNormalDistribution originalCriticalFlowRate = input.CriticalFlowRate;
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

            var dikeProfile = new DikeProfile(new Point2D(0, 0),
                                              new[]
                                              {
                                                  new RoughnessPoint(new Point2D(6.6, 7.7), 0.8)
                                              }, foreshoreGeometry.ToArray(), breakWater,
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Orientation = 1.1, DikeHeight = 4.4
                                              });

            // Call
            input.DikeProfile = dikeProfile;

            // Assert
            Assert.AreSame(dikeProfile, input.DikeProfile);
            Assert.AreEqual(dikeProfile.Orientation, input.Orientation);
            Assert.AreEqual(withBreakWater, input.UseBreakWater);
            Assert.AreEqual(withBreakWater ? dikeProfile.BreakWater.Type : originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(withBreakWater ? dikeProfile.BreakWater.Height : originalBreakWaterHeight, input.BreakWater.Height);
            Assert.AreEqual(withValidForeshore, input.UseForeshore);
            CollectionAssert.AreEqual(dikeProfile.ForeshoreGeometry, input.ForeshoreGeometry);
            CollectionAssert.AreEqual(dikeProfile.DikeGeometry, input.DikeGeometry);
            Assert.AreEqual(dikeProfile.DikeHeight, input.DikeHeight);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(originalCriticalFlowRate, input.CriticalFlowRate);
        }

        [Test]
        public void DikeProfile_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();
            BreakWaterType originalBreakWaterType = input.BreakWater.Type;
            RoundedDouble originalBreakWaterHeight = input.BreakWater.Height;
            LogNormalDistribution originalCriticalFlowRate = input.CriticalFlowRate;
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
            Assert.AreNotEqual(new RoundedDouble(0), input.Orientation);
            Assert.IsTrue(input.UseBreakWater);
            Assert.AreNotEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreNotEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsTrue(input.UseForeshore);
            CollectionAssert.IsNotEmpty(input.ForeshoreGeometry);
            CollectionAssert.IsNotEmpty(input.DikeGeometry);
            Assert.AreNotEqual(new RoundedDouble(0), input.DikeHeight);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(originalCriticalFlowRate, input.CriticalFlowRate);

            // Call
            input.DikeProfile = null;

            // Assert
            Assert.AreEqual(2, input.Orientation.NumberOfDecimalPlaces);
            Assert.IsNaN(input.Orientation);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            CollectionAssert.IsEmpty(input.DikeGeometry);
            Assert.AreEqual(2, input.DikeHeight.NumberOfDecimalPlaces);
            Assert.IsNaN(input.DikeHeight);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(originalCriticalFlowRate, input.CriticalFlowRate);
        }

        [Test]
        [TestCase(360.004)]
        [TestCase(300)]
        [TestCase(0)]
        [TestCase(-0.004)]
        [TestCase(double.NaN)]
        public void Orientation_SetNewValue_ValueIsRounded(double validOrientation)
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            int originalNumberOfDecimalPlaces = input.Orientation.NumberOfDecimalPlaces;

            // Call
            input.Orientation = new RoundedDouble(5, validOrientation);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(validOrientation, input.Orientation.Value, input.Orientation.GetAccuracy());
        }

        [Test]
        [TestCase(400)]
        [TestCase(360.05)]
        [TestCase(-0.005)]
        [TestCase(-23)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void Orientation_SetInvalidValue_ThrowsArgumentOutOfRangeException(double invalidOrientation)
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            // Call
            TestDelegate call = () => input.Orientation = (RoundedDouble) invalidOrientation;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "De waarde voor de oriëntatie moet in het bereik [0, 360] liggen.");
        }

        [Test]
        public void DikeHeight_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            int originalNumberOfDecimalPlaces = input.DikeHeight.NumberOfDecimalPlaces;

            // Call
            input.DikeHeight = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.DikeHeight.Value);
        }

        [Test]
        public void CriticalFlowRate_SetNewValue_GetNewValues()
        {
            // Setup
            var random = new Random(22);
            var input = new GrassCoverErosionInwardsInput();
            var mean = (RoundedDouble) (0.01 + random.NextDouble());
            var standardDeviation = (RoundedDouble) (0.01 + random.NextDouble());
            var expectedDistribution = new LogNormalDistribution(4)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };
            var distributionToSet = new LogNormalDistribution(5)
            {
                Mean = mean,
                StandardDeviation = standardDeviation
            };

            // Call
            input.CriticalFlowRate = distributionToSet;

            // Assert
            Assert.AreNotSame(distributionToSet, input.CriticalFlowRate);
            DistributionAssert.AreEqual(expectedDistribution, input.CriticalFlowRate);
        }
    }
}