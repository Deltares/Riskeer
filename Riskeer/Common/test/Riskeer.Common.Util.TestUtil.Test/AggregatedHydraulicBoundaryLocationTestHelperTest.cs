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

namespace Riskeer.Common.Util.TestUtil.Test
{
    [TestFixture]
    public class AggregatedHydraulicBoundaryLocationTestHelperTest
    {
        [Test]
        public void Create_WithoutParameters_ReturnAggregatedHydraulicBoundaryLocation()
        {
            // Call
            AggregatedHydraulicBoundaryLocation location = AggregatedHydraulicBoundaryLocationTestHelper.Create();

            // Assert
            Assert.AreEqual(1, location.Id);
            Assert.AreEqual("test", location.Name);
            Assert.AreEqual(new Point2D(0, 0), location.Location);
            Assert.IsFalse(double.IsNaN(location.WaterLevelCalculationForFactorizedSignalingNorm));
            Assert.IsFalse(double.IsNaN(location.WaterLevelCalculationForSignalingNorm));
            Assert.IsFalse(double.IsNaN(location.WaterLevelCalculationForLowerLimitNorm));
            Assert.IsFalse(double.IsNaN(location.WaterLevelCalculationForFactorizedLowerLimitNorm));
            Assert.IsFalse(double.IsNaN(location.WaveHeightCalculationForFactorizedSignalingNorm));
            Assert.IsFalse(double.IsNaN(location.WaveHeightCalculationForSignalingNorm));
            Assert.IsFalse(double.IsNaN(location.WaveHeightCalculationForLowerLimitNorm));
            Assert.IsFalse(double.IsNaN(location.WaveHeightCalculationForFactorizedLowerLimitNorm));
        }

        [Test]
        public void Create_WithParameters_ReturnAggregatedHydraulicBoundaryLocation()
        {
            // Setup
            var random = new Random(11);
            RoundedDouble waterLevelCalculationForFactorizedSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForFactorizedLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForFactorizedSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForFactorizedLowerLimitNorm = random.NextRoundedDouble();

            // Call
            AggregatedHydraulicBoundaryLocation location = AggregatedHydraulicBoundaryLocationTestHelper.Create(
                waterLevelCalculationForFactorizedSignalingNorm,
                waterLevelCalculationForSignalingNorm,
                waterLevelCalculationForLowerLimitNorm,
                waterLevelCalculationForFactorizedLowerLimitNorm,
                waveHeightCalculationForFactorizedSignalingNorm,
                waveHeightCalculationForSignalingNorm,
                waveHeightCalculationForLowerLimitNorm,
                waveHeightCalculationForFactorizedLowerLimitNorm);

            // Assert
            Assert.AreEqual(1, location.Id);
            Assert.AreEqual("test", location.Name);
            Assert.AreEqual(new Point2D(0, 0), location.Location);
            Assert.AreEqual(waterLevelCalculationForFactorizedSignalingNorm, location.WaterLevelCalculationForFactorizedSignalingNorm);
            Assert.AreEqual(waterLevelCalculationForSignalingNorm, location.WaterLevelCalculationForSignalingNorm);
            Assert.AreEqual(waterLevelCalculationForLowerLimitNorm, location.WaterLevelCalculationForLowerLimitNorm);
            Assert.AreEqual(waterLevelCalculationForFactorizedLowerLimitNorm, location.WaterLevelCalculationForFactorizedLowerLimitNorm);
            Assert.AreEqual(waveHeightCalculationForFactorizedSignalingNorm, location.WaveHeightCalculationForFactorizedSignalingNorm);
            Assert.AreEqual(waveHeightCalculationForSignalingNorm, location.WaveHeightCalculationForSignalingNorm);
            Assert.AreEqual(waveHeightCalculationForLowerLimitNorm, location.WaveHeightCalculationForLowerLimitNorm);
            Assert.AreEqual(waveHeightCalculationForFactorizedLowerLimitNorm, location.WaveHeightCalculationForFactorizedLowerLimitNorm);
        }
    }
}