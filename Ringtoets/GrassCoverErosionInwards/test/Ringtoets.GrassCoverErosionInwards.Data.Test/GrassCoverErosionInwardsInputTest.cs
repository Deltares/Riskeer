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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var input = new GrassCoverErosionInwardsInput();

            // Assert
            Assert.IsInstanceOf<Observable>(input);
            Assert.IsInstanceOf<ICalculationInput>(input);

            Assert.IsNull(input.DikeProfile);
            Assert.AreEqual(new RoundedDouble(2), input.Orientation);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, input.BreakWater.Type);
            Assert.AreEqual(new RoundedDouble(2), input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            CollectionAssert.IsEmpty(input.DikeGeometry);
            Assert.AreEqual(new RoundedDouble(2), input.DikeHeight);
            Assert.IsNull(input.HydraulicBoundaryLocation);
            Assert.IsFalse(input.CalculateDikeHeight);

            var criticalFlowRate = new LogNormalDistribution(4)
            {
                Mean = new RoundedDouble(4, 0.004),
                StandardDeviation = new RoundedDouble(4, 0.0006)
            };
            Assert.AreEqual(criticalFlowRate.Mean, input.CriticalFlowRate.Mean);
            Assert.AreEqual(criticalFlowRate.StandardDeviation, input.CriticalFlowRate.StandardDeviation);
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
            Assert.AreEqual(new RoundedDouble(0), input.Orientation);
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(originalBreakWaterHeight, input.BreakWater.Height);
            Assert.IsFalse(input.UseForeshore);
            CollectionAssert.IsEmpty(input.ForeshoreGeometry);
            CollectionAssert.IsEmpty(input.DikeGeometry);
            Assert.AreEqual(new RoundedDouble(0), input.DikeHeight);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(originalCriticalFlowRate, input.CriticalFlowRate);
        }

        [Test]
        public void Orientation_SetNewValue_ValueIsRounded()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();

            int originalNumberOfDecimalPlaces = input.Orientation.NumberOfDecimalPlaces;

            // Call
            input.Orientation = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, input.Orientation.Value);
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
            const double meanValue = 1.2345689;
            const double standardDeviationValue = 9.87654321;
            var input = new GrassCoverErosionInwardsInput();

            int originalNumberOfDecimalPlacesMean = input.CriticalFlowRate.Mean.NumberOfDecimalPlaces;
            int originalNumberOfDecimalPlacesStandardDeviation = input.CriticalFlowRate.StandardDeviation.NumberOfDecimalPlaces;

            // Call
            input.CriticalFlowRate = new LogNormalDistribution(10)
            {
                Mean = (RoundedDouble) meanValue,
                StandardDeviation = (RoundedDouble) standardDeviationValue
            };

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlacesMean, input.CriticalFlowRate.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(originalNumberOfDecimalPlacesMean, meanValue), input.CriticalFlowRate.Mean);
            Assert.AreEqual(originalNumberOfDecimalPlacesStandardDeviation, input.CriticalFlowRate.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(originalNumberOfDecimalPlacesStandardDeviation, standardDeviationValue), input.CriticalFlowRate.StandardDeviation);
        }
    }
}