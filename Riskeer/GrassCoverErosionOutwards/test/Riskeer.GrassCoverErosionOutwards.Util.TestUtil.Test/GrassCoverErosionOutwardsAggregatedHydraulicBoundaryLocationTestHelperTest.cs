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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionOutwards.Util.TestUtil.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationTestHelperTest
    {
        [Test]
        public void Create_WithoutParameters_ReturnGrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation()
        {
            // Call
            GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation location = GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationTestHelper.Create();

            // Assert
            Assert.AreEqual(1, location.Id);
            Assert.AreEqual("test", location.Name);
            Assert.AreEqual(new Point2D(0, 0), location.Location);
            Assert.IsFalse(double.IsNaN(location.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm));
            Assert.IsFalse(double.IsNaN(location.WaterLevelCalculationForMechanismSpecificSignalingNorm));
            Assert.IsFalse(double.IsNaN(location.WaterLevelCalculationForMechanismSpecificLowerLimitNorm));
            Assert.IsFalse(double.IsNaN(location.WaterLevelCalculationForLowerLimitNorm));
            Assert.IsFalse(double.IsNaN(location.WaterLevelCalculationForFactorizedLowerLimitNorm));
            Assert.IsFalse(double.IsNaN(location.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm));
            Assert.IsFalse(double.IsNaN(location.WaveHeightCalculationForMechanismSpecificSignalingNorm));
            Assert.IsFalse(double.IsNaN(location.WaveHeightCalculationForMechanismSpecificLowerLimitNorm));
            Assert.IsFalse(double.IsNaN(location.WaveHeightCalculationForLowerLimitNorm));
            Assert.IsFalse(double.IsNaN(location.WaveHeightCalculationForFactorizedLowerLimitNorm));
        }

        [Test]
        public void Create_WithParameters_ReturnGrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation()
        {
            // Setup
            var random = new Random(11);
            RoundedDouble waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForMechanismSpecificSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForMechanismSpecificLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForFactorizedLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForMechanismSpecificSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForMechanismSpecificLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForFactorizedLowerLimitNorm = random.NextRoundedDouble();

            // Call
            GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation location = GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationTestHelper.Create(
                waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm,
                waterLevelCalculationForMechanismSpecificSignalingNorm,
                waterLevelCalculationForMechanismSpecificLowerLimitNorm,
                waterLevelCalculationForLowerLimitNorm,
                waterLevelCalculationForFactorizedLowerLimitNorm,
                waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm,
                waveHeightCalculationForMechanismSpecificSignalingNorm,
                waveHeightCalculationForMechanismSpecificLowerLimitNorm,
                waveHeightCalculationForLowerLimitNorm,
                waveHeightCalculationForFactorizedLowerLimitNorm);

            // Assert
            Assert.AreEqual(1, location.Id);
            Assert.AreEqual("test", location.Name);
            Assert.AreEqual(new Point2D(0, 0), location.Location);
            Assert.AreEqual(waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm, location.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm);
            Assert.AreEqual(waterLevelCalculationForMechanismSpecificSignalingNorm, location.WaterLevelCalculationForMechanismSpecificSignalingNorm);
            Assert.AreEqual(waterLevelCalculationForMechanismSpecificLowerLimitNorm, location.WaterLevelCalculationForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(waterLevelCalculationForLowerLimitNorm, location.WaterLevelCalculationForLowerLimitNorm);
            Assert.AreEqual(waterLevelCalculationForFactorizedLowerLimitNorm, location.WaterLevelCalculationForFactorizedLowerLimitNorm);
            Assert.AreEqual(waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm, location.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm);
            Assert.AreEqual(waveHeightCalculationForMechanismSpecificSignalingNorm, location.WaveHeightCalculationForMechanismSpecificSignalingNorm);
            Assert.AreEqual(waveHeightCalculationForMechanismSpecificLowerLimitNorm, location.WaveHeightCalculationForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(waveHeightCalculationForLowerLimitNorm, location.WaveHeightCalculationForLowerLimitNorm);
            Assert.AreEqual(waveHeightCalculationForFactorizedLowerLimitNorm, location.WaveHeightCalculationForFactorizedLowerLimitNorm);
        }
    }
}