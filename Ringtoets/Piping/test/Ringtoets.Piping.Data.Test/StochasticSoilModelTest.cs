// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class StochasticSoilModelTest
    {
        [Test]
        [TestCase(1234L, "", "")]
        [TestCase(5678L, "segmentSoilModelName", "segmentName")]
        public void Constructor_Always_ExpectedValues(long segmentSoilModelId, string segmentSoilModelName, string segmentName)
        {
            // Call
            StochasticSoilModel stochasticSoilModelSegment = new StochasticSoilModel(segmentSoilModelId, segmentSoilModelName, segmentName);

            // Assert
            Assert.IsInstanceOf<Observable>(stochasticSoilModelSegment);
            Assert.AreEqual(segmentSoilModelId, stochasticSoilModelSegment.Id);
            Assert.AreEqual(segmentSoilModelName, stochasticSoilModelSegment.Name);
            Assert.AreEqual(segmentName, stochasticSoilModelSegment.SegmentName);
            CollectionAssert.IsEmpty(stochasticSoilModelSegment.Geometry);
            CollectionAssert.IsEmpty(stochasticSoilModelSegment.StochasticSoilProfiles);
        }

        [Test]
        public void PropertySegmentPoints_Always_ReturnsExpectedValues()
        {
            // Setup
            const long expectedSegmentSoilModelId = 1234L;
            const string expectedSegmentSoilModelName = "someSegmentSoilModelName";
            const string expectedSegmentName = "someSegmentName";
            StochasticSoilModel stochasticSoilModelSegment = new StochasticSoilModel(expectedSegmentSoilModelId, expectedSegmentSoilModelName, expectedSegmentName);
            var point2D = new Point2D(1.0, 2.0);

            // Call
            stochasticSoilModelSegment.Geometry.Add(point2D);

            // Assert
            Assert.AreEqual(expectedSegmentSoilModelId, stochasticSoilModelSegment.Id);
            Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModelSegment.Name);
            Assert.AreEqual(expectedSegmentName, stochasticSoilModelSegment.SegmentName);
            Assert.AreEqual(1, stochasticSoilModelSegment.Geometry.Count);
            Assert.AreEqual(point2D, stochasticSoilModelSegment.Geometry[0]);
        }

        [Test]
        public void PropertyStochasticSoilProfileProbabilities_Always_ReturnsExpectedValues()
        {
            // Setup
            const long expectedSegmentSoilModelId = 1234L;
            const string expectedSegmentSoilModelName = "someSegmentSoilModelName";
            const string expectedSegmentName = "someSegmentName";
            StochasticSoilModel stochasticSoilModelSegment = new StochasticSoilModel(expectedSegmentSoilModelId, expectedSegmentSoilModelName, expectedSegmentName);

            MockRepository mockRepository = new MockRepository();
            var stochasticSoilProfileProbabilityMock = mockRepository.StrictMock<StochasticSoilProfile>(1.0, null, null);
            mockRepository.ReplayAll();

            // Call
            stochasticSoilModelSegment.StochasticSoilProfiles.Add(stochasticSoilProfileProbabilityMock);

            // Assert
            Assert.AreEqual(expectedSegmentSoilModelId, stochasticSoilModelSegment.Id);
            Assert.AreEqual(expectedSegmentSoilModelName, stochasticSoilModelSegment.Name);
            Assert.AreEqual(expectedSegmentName, stochasticSoilModelSegment.SegmentName);
            Assert.AreEqual(1, stochasticSoilModelSegment.StochasticSoilProfiles.Count);
            Assert.AreEqual(stochasticSoilProfileProbabilityMock, stochasticSoilModelSegment.StochasticSoilProfiles[0]);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Update_WithNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var model = new StochasticSoilModel(1234, "name", "segment");

            // Call
            TestDelegate test = () => model.Update(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("fromModel", paramName);
        }

        [Test]
        public void Update_WithOtherModel_PropertiesUpdated()
        {
            // Setup
            var equalProfileName = "nameA";
            var model = new StochasticSoilModel(1234, "name", "segment");
            model.Geometry.Add(new Point2D(6, 2));
            model.Geometry.Add(new Point2D(4, 3));

            var stochasticProfileA = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -11)
            {
                SoilProfile = new TestPipingSoilProfile(equalProfileName)
            };
            var stochasticProfileB = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -52)
            {
                SoilProfile = new TestPipingSoilProfile("nameB")
            };
            model.StochasticSoilProfiles.Add(stochasticProfileA);
            model.StochasticSoilProfiles.Add(stochasticProfileB);

            var otherName = "other name";
            var otherSegmentName = "other segment";
            var otherModel = new StochasticSoilModel(41, otherName, otherSegmentName);

            var otherPointA = new Point2D(2,0);
            var otherPointB = new Point2D(3,0);
            otherModel.Geometry.Add(otherPointA);
            otherModel.Geometry.Add(otherPointB);

            var otherStochasticProfileA = new StochasticSoilProfile(0.7, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile(equalProfileName)
            };
            var otherStochasticProfileB = new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, -2)
            {
                SoilProfile = new TestPipingSoilProfile("other profile name")
            };
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileA);
            otherModel.StochasticSoilProfiles.Add(otherStochasticProfileB);

            // Call
            model.Update(otherModel);

            // Assert
            Assert.AreEqual(otherName, model.Name);
            Assert.AreEqual(otherSegmentName, model.SegmentName);
            Assert.AreSame(otherPointA, model.Geometry[0]);
            Assert.AreSame(otherPointB, model.Geometry[1]);
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
            Assert.AreSame(stochasticProfileA, model.StochasticSoilProfiles[0]);
            Assert.AreSame(otherStochasticProfileA.SoilProfile, model.StochasticSoilProfiles[0].SoilProfile);
            Assert.AreNotSame(stochasticProfileB, model.StochasticSoilProfiles[1]);
            Assert.AreSame(otherStochasticProfileB.SoilProfile, model.StochasticSoilProfiles[1].SoilProfile);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel(1, name, "segmentName");

            // Call & Assert
            Assert.AreEqual(name, stochasticSoilModel.ToString());
        }
    }
}