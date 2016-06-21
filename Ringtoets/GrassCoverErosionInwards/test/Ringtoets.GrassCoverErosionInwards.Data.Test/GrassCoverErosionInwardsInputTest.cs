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

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;

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
            [Values(true, false)] bool withForeshore)
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();
            var originalBreakWaterType = input.BreakWater.Type;
            var originalBreakWaterHeight = input.BreakWater.Height;
            var originalCriticalFlowRate = input.CriticalFlowRate;
            var originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var dikeProfile = new DikeProfile(new Point2D(0, 0))
            {
                Orientation = (RoundedDouble) 1.1,
                DikeGeometry =
                {
                    new RoughnessPoint(new Point2D(2.2, 3.3), 0.6)
                },
                CrestLevel = (RoundedDouble) 4.4
            };

            if (withBreakWater)
            {
                var nonDefaultBreakWaterType = BreakWaterType.Wall;
                var nonDefaultBreakWaterHeight = 5.5;

                // Precondition
                Assert.AreNotEqual(nonDefaultBreakWaterType, input.BreakWater.Type);
                Assert.AreNotEqual(nonDefaultBreakWaterHeight, input.BreakWater.Height);

                dikeProfile.BreakWater = new BreakWater(nonDefaultBreakWaterType, nonDefaultBreakWaterHeight);
            }

            if (withForeshore)
            {
                dikeProfile.ForeshoreGeometry.Add(new Point2D(6.6, 7.7));
            }

            // Call
            input.DikeProfile = dikeProfile;

            // Assert
            Assert.AreSame(dikeProfile, input.DikeProfile);
            Assert.AreEqual(dikeProfile.Orientation, input.Orientation);
            Assert.AreEqual(withBreakWater, input.UseBreakWater);
            Assert.AreEqual(withBreakWater ? dikeProfile.BreakWater.Type : originalBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(withBreakWater ? dikeProfile.BreakWater.Height : originalBreakWaterHeight, input.BreakWater.Height);
            Assert.AreEqual(withForeshore, input.UseForeshore);
            Assert.AreSame(dikeProfile.ForeshoreGeometry, input.ForeshoreGeometry);
            Assert.AreSame(dikeProfile.DikeGeometry, input.DikeGeometry);
            Assert.AreEqual(dikeProfile.CrestLevel, input.DikeHeight);
            Assert.AreEqual(originalHydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(originalCriticalFlowRate, input.CriticalFlowRate);
        }

        [Test]
        [Combinatorial]
        public void DikeProfile_SetNullValue_InputSyncedToDefaults()
        {
            // Setup
            var input = new GrassCoverErosionInwardsInput();
            var originalBreakWaterType = input.BreakWater.Type;
            var originalBreakWaterHeight = input.BreakWater.Height;
            var originalCriticalFlowRate = input.CriticalFlowRate;
            var originalHydraulicBoundaryLocation = input.HydraulicBoundaryLocation;

            var dikeProfile = new DikeProfile(new Point2D(0, 0))
            {
                Orientation = (RoundedDouble) 1.1,
                BreakWater = new BreakWater(BreakWaterType.Caisson, 2.2),
                ForeshoreGeometry = { new Point2D(3.3, 4.4) },
                DikeGeometry =
                {
                    new RoughnessPoint(new Point2D(5.5, 6.6), 0.7)
                },
                CrestLevel = (RoundedDouble) 8.8
            };

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
    }
}