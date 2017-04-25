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

using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestDikeProfileTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Call
            var testProfile = new TestDikeProfile();

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual("id", testProfile.Id);
            Assert.AreEqual("id", testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "A";

            // Call
            var testProfile = new TestDikeProfile(name);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual("id", testProfile.Id);
            Assert.AreEqual(name, testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithNameAndId_ExpectedValues()
        {
            // Setup
            const string name = "A";
            const string id = "Just an id";

            // Call
            var testProfile = new TestDikeProfile(name, id);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual(id, testProfile.Id);
            Assert.AreEqual(name, testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithPoint_ExpectedValues()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            var testProfile = new TestDikeProfile(point);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual("id", testProfile.Id);
            Assert.AreEqual("id", testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(point, testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithPointAndId_ExpectedValues()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);
            const string id = "Just an id";

            // Call
            var testProfile = new TestDikeProfile(point, id);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual(id, testProfile.Id);
            Assert.AreEqual(id, testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(point, testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithNameAndPoint_ExpectedValues()
        {
            // Setup
            const string name = "N";
            var point = new Point2D(-12.34, 7.78);

            // Call
            var testProfile = new TestDikeProfile(name, point);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual("id", testProfile.Id);
            Assert.AreEqual(name, testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(point, testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithForeshoreGeometry_ExpectedValues()
        {
            // Setup
            IEnumerable<Point2D> foreshoreProfileGeometry = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            };

            // Call
            var testProfile = new TestDikeProfile(foreshoreProfileGeometry);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.AreEqual(foreshoreProfileGeometry, testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual("id", testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithForeshoreProfileAndId_ExpectedValues()
        {
            // Setup
            const string id = "Just an id";
            IEnumerable<Point2D> foreshoreProfileGeometry = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            };

            // Call
            var testProfile = new TestDikeProfile(foreshoreProfileGeometry, id);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.IsEmpty(testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.AreEqual(foreshoreProfileGeometry, testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual(id, testProfile.Id);
            Assert.AreEqual(id, testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }

        [Test]
        public void Constructor_WithDikeGeometry_ExpectedValues()
        {
            // Setup
            IEnumerable<RoughnessPoint> foreshoreProfileGeometry = new[]
            {
                new RoughnessPoint(new Point2D(0, 0), 5),
                new RoughnessPoint(new Point2D(1, 1), 6),
                new RoughnessPoint(new Point2D(2, 2), 7)
            };

            var testProfile = new TestDikeProfile(foreshoreProfileGeometry);

            // Assert
            Assert.IsInstanceOf<DikeProfile>(testProfile);
            Assert.IsNotNull(testProfile.ForeshoreProfile);
            Assert.IsNull(testProfile.BreakWater);
            CollectionAssert.AreEqual(foreshoreProfileGeometry, testProfile.DikeGeometry);
            Assert.AreEqual(0, testProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(testProfile.ForeshoreGeometry);
            Assert.IsFalse(testProfile.HasBreakWater);
            Assert.AreEqual("id", testProfile.Id);
            Assert.AreEqual("id", testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }
    }
}