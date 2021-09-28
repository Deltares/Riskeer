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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
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
                Enumerable.Empty<Tuple<double, RoundedDouble>>(),
                Enumerable.Empty<Tuple<double, RoundedDouble>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void Constructor_WaterLevelCalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedHydraulicBoundaryLocation(
                0, string.Empty, new Point2D(0, 0),
                null, Enumerable.Empty<Tuple<double, RoundedDouble>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waterLevelCalculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void Constructor_WaveHeightCalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AggregatedHydraulicBoundaryLocation(
                0, string.Empty, new Point2D(0, 0),
                Enumerable.Empty<Tuple<double, RoundedDouble>>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waveHeightCalculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAllData_ExpectedValues()
        {
            // Setup
            const string name = "location";

            var random = new Random(39);
            long id = random.Next();
            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var waterLevelCalculationsForTargetProbabilities = new List<Tuple<double, RoundedDouble>>();
            var waveHeightCalculationsForTargetProbabilities = new List<Tuple<double, RoundedDouble>>();

            // Call
            var aggregatedLocation = new AggregatedHydraulicBoundaryLocation(
                id, name, location, waterLevelCalculationsForTargetProbabilities, waveHeightCalculationsForTargetProbabilities);

            // Assert
            Assert.AreEqual(id, aggregatedLocation.Id);
            Assert.AreEqual(name, aggregatedLocation.Name);
            Assert.AreSame(location, aggregatedLocation.Location);
            Assert.AreSame(waterLevelCalculationsForTargetProbabilities, aggregatedLocation.WaterLevelCalculationsForTargetProbabilities);
            Assert.AreSame(waveHeightCalculationsForTargetProbabilities, aggregatedLocation.WaveHeightCalculationsForTargetProbabilities);
        }
    }
}