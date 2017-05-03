// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestForeshoreProfileTest
    {
        [Test]
        public void Constructor_Always_ReturnsForeshoreProfileWithEmptyNameAndOnePointAtOrigin()
        {
            // Call
            ForeshoreProfile profile = new TestForeshoreProfile();

            // Assert
            Assert.IsEmpty(profile.Geometry);
            Assert.AreEqual("id", profile.Id);
            Assert.AreEqual("id", profile.Name);
            Assert.IsFalse(profile.HasBreakWater);
            Assert.AreEqual(0.0, profile.X0);
            Assert.AreEqual(0.0, profile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), profile.WorldReferencePoint);
        }

        [Test]
        public void Constructor_UseBreakWater_ReturnsForeshoreProfileWithEmptyNameAndOnePointAtOriginAndDefaultBreakWater()
        {
            // Call
            ForeshoreProfile profile = new TestForeshoreProfile(true);

            // Assert
            Assert.IsEmpty(profile.Geometry);
            Assert.AreEqual("id", profile.Id);
            Assert.AreEqual("id", profile.Name);
            Assert.IsTrue(profile.HasBreakWater);
            Assert.IsNotNull(profile.BreakWater);
            Assert.AreEqual(BreakWaterType.Dam, profile.BreakWater.Type);
            Assert.AreEqual(10.0, profile.BreakWater.Height.Value);
            Assert.AreEqual(0.0, profile.X0);
            Assert.AreEqual(0.0, profile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), profile.WorldReferencePoint);
        }

        [Test]
        public void Constructor_WithId_ReturnForeshoreProfileWithGivenNameAndNoBreakWater()
        {
            // Setup
            const string id = "test";

            // Call
            ForeshoreProfile profile = new TestForeshoreProfile(id);

            // Assert
            Assert.IsEmpty(profile.Geometry);
            Assert.AreEqual(id, profile.Id);
            Assert.AreEqual("name", profile.Name);
            Assert.IsFalse(profile.HasBreakWater);
            Assert.AreEqual(0.0, profile.X0);
            Assert.AreEqual(0.0, profile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), profile.WorldReferencePoint);
        }

        [Test]
        public void Constructor_WithIdAndGeometry_ReturnForeshoreProfileWithGivenNameAndGeometry()
        {
            // Setup
            const string id = "test";
            var geometry = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            };

            // Call
            ForeshoreProfile profile = new TestForeshoreProfile(id, geometry);

            // Assert
            CollectionAssert.AreEqual(geometry, profile.Geometry);
            Assert.AreEqual(id, profile.Id);
            Assert.AreEqual("name", profile.Name);
            Assert.IsFalse(profile.HasBreakWater);
            Assert.AreEqual(0.0, profile.X0);
            Assert.AreEqual(0.0, profile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), profile.WorldReferencePoint);
        }

        [Test]
        public void Constructor_WithBreakWater_ReturnsForeshoreProfileWithEmptyNameAndOnePointAtOriginAndBreakWater()
        {
            // Setup
            var breakWater = new BreakWater(BreakWaterType.Dam, 50.0);

            // Call
            ForeshoreProfile profile = new TestForeshoreProfile(breakWater);

            // Assert
            Assert.IsEmpty(profile.Geometry);
            Assert.AreEqual("id", profile.Id);
            Assert.AreEqual("id", profile.Name);
            Assert.AreSame(breakWater, profile.BreakWater);
            Assert.IsTrue(profile.HasBreakWater);
            Assert.AreEqual(0.0, profile.X0);
            Assert.AreEqual(0.0, profile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), profile.WorldReferencePoint);
        }

        [Test]
        public void Constructor_WithGeometry_ReturnsForeshoreProfileWithEmptyNameAndOnePointAtOriginAndGeometry()
        {
            // Setup
            var geometry = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            };

            // Call
            ForeshoreProfile profile = new TestForeshoreProfile(geometry);

            // Assert
            CollectionAssert.AreEqual(geometry, profile.Geometry);
            Assert.AreEqual("id", profile.Id);
            Assert.AreEqual("id", profile.Name);
            Assert.IsFalse(profile.HasBreakWater);
            Assert.AreEqual(0.0, profile.X0);
            Assert.AreEqual(0.0, profile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), profile.WorldReferencePoint);
        }
    }
}