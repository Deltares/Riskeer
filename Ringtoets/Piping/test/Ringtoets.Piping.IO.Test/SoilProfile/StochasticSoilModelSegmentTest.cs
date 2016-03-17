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

using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilModelSegmentTest
    {
        [Test]
        [TestCase(1234L, "", "")]
        [TestCase(5678L, "segmentSoilModelName", "segmentName")]
        public void Constructor_Always_ExpectedValues(long segmentSoilModelId, string segmentSoilModelName, string segmentName)
        {
            // Call
            StochasticSoilModelSegment stochasticSoilModelSegment = new StochasticSoilModelSegment(segmentSoilModelId, segmentSoilModelName, segmentName);

            // Assert
            Assert.IsInstanceOf<StochasticSoilModelSegment>(stochasticSoilModelSegment);
            Assert.AreEqual(segmentSoilModelId, stochasticSoilModelSegment.SegmentSoilModelId);
            Assert.AreEqual(segmentSoilModelName, stochasticSoilModelSegment.SegmentSoilModelName);
            Assert.AreEqual(segmentName, stochasticSoilModelSegment.SegmentName);
            CollectionAssert.IsEmpty(stochasticSoilModelSegment.SegmentPoints);
            CollectionAssert.IsEmpty(stochasticSoilModelSegment.StochasticSoilProfileProbabilities);
        }

        [Test]
        public void PropertySegmentPoints_Always_ReturnsExpectedValues()
        {
            // Setup
            const long expectedSegmentSoilModelId = 1234L;
            const string expectedSegmentSoilModelName = "someSegmentSoilModelName";
            const string expectedSegmentName = "someSegmentName";
            StochasticSoilModelSegment stochasticSoilModelSegment = new StochasticSoilModelSegment(expectedSegmentSoilModelId, expectedSegmentSoilModelName, expectedSegmentName);
            var point2D = new Point2D(1.0, 2.0);

            // Call
            stochasticSoilModelSegment.SegmentPoints.Add(point2D);

            // Assert
            Assert.AreEqual(expectedSegmentSoilModelId, stochasticSoilModelSegment.SegmentSoilModelId);
            Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModelSegment.SegmentSoilModelName);
            Assert.AreEqual(expectedSegmentName, stochasticSoilModelSegment.SegmentName);
            Assert.AreEqual(1, stochasticSoilModelSegment.SegmentPoints.Count);
            Assert.AreEqual(point2D, stochasticSoilModelSegment.SegmentPoints[0]);
        }

        [Test]
        public void PropertyStochasticSoilProfileProbabilities_Always_ReturnsExpectedValues()
        {
            // Setup
            const long expectedSegmentSoilModelId = 1234L;
            const string expectedSegmentSoilModelName = "someSegmentSoilModelName";
            const string expectedSegmentName = "someSegmentName";
            StochasticSoilModelSegment stochasticSoilModelSegment = new StochasticSoilModelSegment(expectedSegmentSoilModelId, expectedSegmentSoilModelName, expectedSegmentName);

            MockRepository mockRepository = new MockRepository();
            var stochasticSoilProfileProbabilityMock = mockRepository.StrictMock<StochasticSoilProfileProbability>(1.0, null, null);
            mockRepository.ReplayAll();

            // Call
            stochasticSoilModelSegment.StochasticSoilProfileProbabilities.Add(stochasticSoilProfileProbabilityMock);

            // Assert
            Assert.AreEqual(expectedSegmentSoilModelId, stochasticSoilModelSegment.SegmentSoilModelId);
            Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModelSegment.SegmentSoilModelName);
            Assert.AreEqual(expectedSegmentName, stochasticSoilModelSegment.SegmentName);
            Assert.AreEqual(1, stochasticSoilModelSegment.StochasticSoilProfileProbabilities.Count);
            Assert.AreEqual(stochasticSoilProfileProbabilityMock, stochasticSoilModelSegment.StochasticSoilProfileProbabilities[0]);
            mockRepository.VerifyAll();
        }
    }
}