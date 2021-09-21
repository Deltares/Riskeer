﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class AggregatedHydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedHydraulicBoundaryLocation(
                0, null, new Point2D(0, 0),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                Enumerable.Empty<Tuple<double, RoundedDouble>>(),
                Enumerable.Empty<Tuple<double, RoundedDouble>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_LocationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedHydraulicBoundaryLocation(
                0, string.Empty, null,
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                Enumerable.Empty<Tuple<double, RoundedDouble>>(),
                Enumerable.Empty<Tuple<double, RoundedDouble>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void Constructor_WaterLevelCalculationForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedHydraulicBoundaryLocation(
                0, string.Empty, new Point2D(0, 0),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                null,
                Enumerable.Empty<Tuple<double, RoundedDouble>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waterLevelCalculationForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void Constructor_WaveHeightCalculationForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedHydraulicBoundaryLocation(
                0, string.Empty, new Point2D(0, 0),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                new RoundedDouble(), new RoundedDouble(),
                Enumerable.Empty<Tuple<double, RoundedDouble>>(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waveHeightCalculationForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAllData_ExpectedValues()
        {
            // Setup
            const string name = "location";

            var random = new Random(39);
            long id = random.Next();
            var location = new Point2D(random.NextDouble(), random.NextDouble());
            RoundedDouble waterLevelCalculationForFactorizedSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waterLevelCalculationForFactorizedLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForFactorizedSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForSignalingNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForLowerLimitNorm = random.NextRoundedDouble();
            RoundedDouble waveHeightCalculationForFactorizedLowerLimitNorm = random.NextRoundedDouble();
            Tuple<double, RoundedDouble>[] waterLevelCalculationForTargetProbabilities = Array.Empty<Tuple<double, RoundedDouble>>();
            Tuple<double, RoundedDouble>[] waveHeightCalculationForTargetProbabilities = Array.Empty<Tuple<double, RoundedDouble>>();

            // Call
            var aggregatedLocation = new AggregatedHydraulicBoundaryLocation(
                id, name, location, waterLevelCalculationForFactorizedSignalingNorm, waterLevelCalculationForSignalingNorm,
                waterLevelCalculationForLowerLimitNorm, waterLevelCalculationForFactorizedLowerLimitNorm, waveHeightCalculationForFactorizedSignalingNorm,
                waveHeightCalculationForSignalingNorm, waveHeightCalculationForLowerLimitNorm, waveHeightCalculationForFactorizedLowerLimitNorm,
                waterLevelCalculationForTargetProbabilities, waveHeightCalculationForTargetProbabilities);

            // Assert
            Assert.AreEqual(id, aggregatedLocation.Id);
            Assert.AreEqual(name, aggregatedLocation.Name);
            Assert.AreSame(location, aggregatedLocation.Location);
            Assert.AreEqual(waterLevelCalculationForFactorizedSignalingNorm, aggregatedLocation.WaterLevelCalculationForFactorizedSignalingNorm);
            Assert.AreEqual(waterLevelCalculationForSignalingNorm, aggregatedLocation.WaterLevelCalculationForSignalingNorm);
            Assert.AreEqual(waterLevelCalculationForLowerLimitNorm, aggregatedLocation.WaterLevelCalculationForLowerLimitNorm);
            Assert.AreEqual(waterLevelCalculationForFactorizedLowerLimitNorm, aggregatedLocation.WaterLevelCalculationForFactorizedLowerLimitNorm);
            Assert.AreEqual(waveHeightCalculationForFactorizedSignalingNorm, aggregatedLocation.WaveHeightCalculationForFactorizedSignalingNorm);
            Assert.AreEqual(waveHeightCalculationForSignalingNorm, aggregatedLocation.WaveHeightCalculationForSignalingNorm);
            Assert.AreEqual(waveHeightCalculationForLowerLimitNorm, aggregatedLocation.WaveHeightCalculationForLowerLimitNorm);
            Assert.AreEqual(waveHeightCalculationForFactorizedLowerLimitNorm, aggregatedLocation.WaveHeightCalculationForFactorizedLowerLimitNorm);
            Assert.AreSame(waterLevelCalculationForTargetProbabilities, aggregatedLocation.WaterLevelCalculationForTargetProbabilities);
            Assert.AreSame(waveHeightCalculationForTargetProbabilities, aggregatedLocation.WaveHeightCalculationForTargetProbabilities);
        }
    }
}