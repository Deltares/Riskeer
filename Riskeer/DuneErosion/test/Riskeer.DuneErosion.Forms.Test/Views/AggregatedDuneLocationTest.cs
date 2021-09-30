// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.DuneErosion.Forms.Views;

namespace Riskeer.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class AggregatedDuneLocationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedDuneLocation(0, null, new Point2D(0, 0), 0,
                                                      RoundedDouble.NaN, RoundedDouble.NaN,
                                                      Array.Empty<Tuple<double, RoundedDouble>>(),
                                                      Array.Empty<Tuple<double, RoundedDouble>>(),
                                                      Array.Empty<Tuple<double, RoundedDouble>>());

            ;

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_WaterLevelCalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedDuneLocation(0, string.Empty, new Point2D(0, 0), 0,
                                                      RoundedDouble.NaN, RoundedDouble.NaN, null,
                                                      Array.Empty<Tuple<double, RoundedDouble>>(),
                                                      Array.Empty<Tuple<double, RoundedDouble>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waterLevelCalculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void Constructor_WaveHeightCalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedDuneLocation(0, string.Empty, new Point2D(0, 0), 0,
                                                      RoundedDouble.NaN, RoundedDouble.NaN,
                                                      Array.Empty<Tuple<double, RoundedDouble>>(),
                                                      null,
                                                      Array.Empty<Tuple<double, RoundedDouble>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waveHeightCalculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void Constructor_WavePeriodCalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedDuneLocation(0, string.Empty, new Point2D(0, 0), 0,
                                                      RoundedDouble.NaN, RoundedDouble.NaN,
                                                      Array.Empty<Tuple<double, RoundedDouble>>(),
                                                      Array.Empty<Tuple<double, RoundedDouble>>(),
                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("wavePeriodCalculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);

            long id = random.Next();
            const string name = "DuneLocationName";
            var location = new Point2D(random.NextDouble(), random.NextDouble());
            int coastalAreaId = random.Next();
            RoundedDouble offset = random.NextRoundedDouble();
            RoundedDouble d50 = random.NextRoundedDouble();
            var waterLevelCalculationsForTargetProbabilities = new List<Tuple<double, RoundedDouble>>();
            var waveHeightCalculationsForTargetProbabilities = new List<Tuple<double, RoundedDouble>>();
            var wavePeriodCalculationsForTargetProbabilities = new List<Tuple<double, RoundedDouble>>();

            // Call
            var aggregatedDuneLocation = new AggregatedDuneLocation(id, name, location, coastalAreaId, offset, d50,
                                                                    waterLevelCalculationsForTargetProbabilities,
                                                                    waveHeightCalculationsForTargetProbabilities,
                                                                    wavePeriodCalculationsForTargetProbabilities);

            // Assert
            Assert.AreEqual(id, aggregatedDuneLocation.Id);
            Assert.AreEqual(name, aggregatedDuneLocation.Name);
            Assert.AreSame(location, aggregatedDuneLocation.Location);
            Assert.AreEqual(coastalAreaId, aggregatedDuneLocation.CoastalAreaId);
            Assert.AreEqual(offset, aggregatedDuneLocation.Offset);
            Assert.AreEqual(d50, aggregatedDuneLocation.D50);
            Assert.AreSame(waterLevelCalculationsForTargetProbabilities, aggregatedDuneLocation.WaterLevelCalculationsForTargetProbabilities);
            Assert.AreSame(waveHeightCalculationsForTargetProbabilities, aggregatedDuneLocation.WaveHeightCalculationsForTargetProbabilities);
            Assert.AreSame(wavePeriodCalculationsForTargetProbabilities, aggregatedDuneLocation.WavePeriodCalculationsForTargetProbabilities);
        }
    }
}