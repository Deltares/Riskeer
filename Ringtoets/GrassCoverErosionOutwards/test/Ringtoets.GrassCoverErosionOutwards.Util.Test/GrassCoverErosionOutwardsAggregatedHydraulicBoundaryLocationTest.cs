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

namespace Ringtoets.GrassCoverErosionOutwards.Util.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation(
                0, null, new Point2D(0, 0), new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_LocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation(
                0, string.Empty, null, new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAllData_ExpectedValues()
        {
            // Setup
            const string name = "location";

            var random = new Random(39);
            long id = random.Next();
            var location = new Point2D(random.NextDouble(), random.NextDouble());
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
            var aggregatedLocation = new GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation(
                id, name, location,
                waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm, waterLevelCalculationForMechanismSpecificSignalingNorm,
                waterLevelCalculationForMechanismSpecificLowerLimitNorm, waterLevelCalculationForLowerLimitNorm,
                waterLevelCalculationForFactorizedLowerLimitNorm, waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm,
                waveHeightCalculationForMechanismSpecificSignalingNorm, waveHeightCalculationForMechanismSpecificLowerLimitNorm,
                waveHeightCalculationForLowerLimitNorm, waveHeightCalculationForFactorizedLowerLimitNorm);

            // Assert
            Assert.AreEqual(id, aggregatedLocation.Id);
            Assert.AreEqual(name, aggregatedLocation.Name);
            Assert.AreSame(location, aggregatedLocation.Location);

            Assert.AreEqual(waterLevelCalculationForMechanismSpecificFactorizedSignalingNorm, aggregatedLocation.WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm);
            Assert.AreEqual(waterLevelCalculationForMechanismSpecificSignalingNorm, aggregatedLocation.WaterLevelCalculationForMechanismSpecificSignalingNorm);
            Assert.AreEqual(waterLevelCalculationForMechanismSpecificLowerLimitNorm, aggregatedLocation.WaterLevelCalculationForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(waterLevelCalculationForLowerLimitNorm, aggregatedLocation.WaterLevelCalculationForLowerLimitNorm);
            Assert.AreEqual(waterLevelCalculationForFactorizedLowerLimitNorm, aggregatedLocation.WaterLevelCalculationForFactorizedLowerLimitNorm);

            Assert.AreEqual(waveHeightCalculationForMechanismSpecificFactorizedSignalingNorm, aggregatedLocation.WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm);
            Assert.AreEqual(waveHeightCalculationForMechanismSpecificSignalingNorm, aggregatedLocation.WaveHeightCalculationForMechanismSpecificSignalingNorm);
            Assert.AreEqual(waveHeightCalculationForMechanismSpecificLowerLimitNorm, aggregatedLocation.WaveHeightCalculationForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(waveHeightCalculationForLowerLimitNorm, aggregatedLocation.WaveHeightCalculationForLowerLimitNorm);
            Assert.AreEqual(waveHeightCalculationForFactorizedLowerLimitNorm, aggregatedLocation.WaveHeightCalculationForFactorizedLowerLimitNorm);
        }
    }
}