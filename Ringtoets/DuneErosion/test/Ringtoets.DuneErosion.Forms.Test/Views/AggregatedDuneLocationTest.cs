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
using Ringtoets.DuneErosion.Forms.Views;

namespace Ringtoets.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class AggregatedDuneLocationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AggregatedDuneLocation(0, null, new Point2D(0, 0), 0, new RoundedDouble(), new RoundedDouble(),
                                                                 new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(),
                                                                 new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(),
                                                                 new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_LocationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AggregatedDuneLocation(0, string.Empty, null, 0, new RoundedDouble(), new RoundedDouble(),
                                                                 new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(),
                                                                 new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(),
                                                                 new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble(), new RoundedDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);

            long id = random.Next();
            const string name = "DuneLocationName";
            var location = new Point2D(random.NextDouble(), random.NextDouble());
            int coastalAreaId = random.Next();
            RoundedDouble offset = random.NextRoundedDouble();
            RoundedDouble d50 = random.NextRoundedDouble();
            RoundedDouble waterLevelForMechanismSpecificFactorizedSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelForMechanismSpecificSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelForMechanismSpecificLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelForLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelForFactorizedLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightForMechanismSpecificFactorizedSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightForMechanismSpecificSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightForMechanismSpecificLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightForLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightForFactorizedLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble wavePeriodForMechanismSpecificFactorizedSignalingNorm = random.NextRoundedDouble();
            RoundedDouble wavePeriodForMechanismSpecificSignalingNorm = random.NextRoundedDouble();
            RoundedDouble wavePeriodForMechanismSpecificLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble wavePeriodForLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble wavePeriodForFactorizedLowerLimitNorm = random.NextRoundedDouble();

            // Call
            var aggregatedDuneLocation = new AggregatedDuneLocation(id, name, location, coastalAreaId, offset, d50,
                                                                    waterLevelForMechanismSpecificFactorizedSignalingNorm,
                                                                    waterLevelForMechanismSpecificSignalingNorm,
                                                                    waterLevelForMechanismSpecificLowerLimitNorm,
                                                                    waterLevelForLowerLimitNorm,
                                                                    waterLevelForFactorizedLowerLimitNorm,
                                                                    waveHeightForMechanismSpecificFactorizedSignalingNorm,
                                                                    waveHeightForMechanismSpecificSignalingNorm,
                                                                    waveHeightForMechanismSpecificLowerLimitNorm,
                                                                    waveHeightForLowerLimitNorm,
                                                                    waveHeightForFactorizedLowerLimitNorm,
                                                                    wavePeriodForMechanismSpecificFactorizedSignalingNorm,
                                                                    wavePeriodForMechanismSpecificSignalingNorm,
                                                                    wavePeriodForMechanismSpecificLowerLimitNorm,
                                                                    wavePeriodForLowerLimitNorm,
                                                                    wavePeriodForFactorizedLowerLimitNorm);

            // Assert
            Assert.AreEqual(id, aggregatedDuneLocation.Id);
            Assert.AreEqual(name, aggregatedDuneLocation.Name);
            Assert.AreSame(location, aggregatedDuneLocation.Location);
            Assert.AreEqual(coastalAreaId, aggregatedDuneLocation.CoastalAreaId);
            Assert.AreEqual(offset, aggregatedDuneLocation.Offset);
            Assert.AreEqual(d50, aggregatedDuneLocation.D50);
            Assert.AreEqual(waterLevelForMechanismSpecificFactorizedSignalingNorm, aggregatedDuneLocation.WaterLevelForMechanismSpecificFactorizedSignalingNorm);
            Assert.AreEqual(waterLevelForMechanismSpecificSignalingNorm, aggregatedDuneLocation.WaterLevelForMechanismSpecificSignalingNorm);
            Assert.AreEqual(waterLevelForMechanismSpecificLowerLimitNorm, aggregatedDuneLocation.WaterLevelForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(waterLevelForLowerLimitNorm, aggregatedDuneLocation.WaterLevelForLowerLimitNorm);
            Assert.AreEqual(waterLevelForFactorizedLowerLimitNorm, aggregatedDuneLocation.WaterLevelForFactorizedLowerLimitNorm);
            Assert.AreEqual(waveHeightForMechanismSpecificFactorizedSignalingNorm, aggregatedDuneLocation.WaveHeightForMechanismSpecificFactorizedSignalingNorm);
            Assert.AreEqual(waveHeightForMechanismSpecificSignalingNorm, aggregatedDuneLocation.WaveHeightForMechanismSpecificSignalingNorm);
            Assert.AreEqual(waveHeightForMechanismSpecificLowerLimitNorm, aggregatedDuneLocation.WaveHeightForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(waveHeightForLowerLimitNorm, aggregatedDuneLocation.WaveHeightForLowerLimitNorm);
            Assert.AreEqual(waveHeightForFactorizedLowerLimitNorm, aggregatedDuneLocation.WaveHeightForFactorizedLowerLimitNorm);
            Assert.AreEqual(wavePeriodForMechanismSpecificFactorizedSignalingNorm, aggregatedDuneLocation.WavePeriodForMechanismSpecificFactorizedSignalingNorm);
            Assert.AreEqual(wavePeriodForMechanismSpecificSignalingNorm, aggregatedDuneLocation.WavePeriodForMechanismSpecificSignalingNorm);
            Assert.AreEqual(wavePeriodForMechanismSpecificLowerLimitNorm, aggregatedDuneLocation.WavePeriodForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(wavePeriodForLowerLimitNorm, aggregatedDuneLocation.WavePeriodForLowerLimitNorm);
            Assert.AreEqual(wavePeriodForFactorizedLowerLimitNorm, aggregatedDuneLocation.WavePeriodForFactorizedLowerLimitNorm);
        }
    }
}