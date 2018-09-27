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

using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class DikeProfileTestFactoryTest
    {
        [Test]
        public void CreateDikeProfile_WithoutArguments_ReturnsExpectedValues()
        {
            // Call
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();

            // Assert
            Assert.IsNotNull(dikeProfile);
            Assert.IsNotNull(dikeProfile.ForeshoreProfile);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.IsEmpty(dikeProfile.DikeGeometry);
            Assert.AreEqual(0, dikeProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);
            Assert.IsFalse(dikeProfile.HasBreakWater);
            Assert.AreEqual("id", dikeProfile.Id);
            Assert.AreEqual("id", dikeProfile.Name);
            Assert.AreEqual(0, dikeProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0, dikeProfile.X0);
        }

        [Test]
        public void CreateDikeProfile_WithName_ReturnsExpectedValues()
        {
            // Setup
            const string name = "A";

            // Call
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(name);

            // Assert
            Assert.IsNotNull(dikeProfile);
            Assert.IsNotNull(dikeProfile.ForeshoreProfile);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.IsEmpty(dikeProfile.DikeGeometry);
            Assert.AreEqual(0, dikeProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);
            Assert.IsFalse(dikeProfile.HasBreakWater);
            Assert.AreEqual("id", dikeProfile.Id);
            Assert.AreEqual(name, dikeProfile.Name);
            Assert.AreEqual(0, dikeProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0, dikeProfile.X0);
        }

        [Test]
        public void CreateDikeProfile_WithNameAndId_ReturnsExpectedValues()
        {
            // Setup
            const string name = "A";
            const string id = "Just an id";

            // Call
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(name, id);

            // Assert
            Assert.IsNotNull(dikeProfile);
            Assert.IsNotNull(dikeProfile.ForeshoreProfile);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.IsEmpty(dikeProfile.DikeGeometry);
            Assert.AreEqual(0, dikeProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);
            Assert.IsFalse(dikeProfile.HasBreakWater);
            Assert.AreEqual(id, dikeProfile.Id);
            Assert.AreEqual(name, dikeProfile.Name);
            Assert.AreEqual(0, dikeProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0, dikeProfile.X0);
        }

        [Test]
        public void CreateDikeProfile_WithPoint_ReturnsExpectedValues()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(point);

            // Assert
            Assert.IsNotNull(dikeProfile);
            Assert.IsNotNull(dikeProfile.ForeshoreProfile);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.IsEmpty(dikeProfile.DikeGeometry);
            Assert.AreEqual(0, dikeProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);
            Assert.IsFalse(dikeProfile.HasBreakWater);
            Assert.AreEqual("id", dikeProfile.Id);
            Assert.AreEqual("id", dikeProfile.Name);
            Assert.AreEqual(0, dikeProfile.Orientation.Value);
            Assert.AreEqual(point, dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0, dikeProfile.X0);
        }

        [Test]
        public void CreateDikeProfile_WithPointAndId_ReturnsExpectedValues()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);
            const string id = "Just an id";

            // Call
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(point, id);

            // Assert
            Assert.IsNotNull(dikeProfile);
            Assert.IsNotNull(dikeProfile.ForeshoreProfile);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.IsEmpty(dikeProfile.DikeGeometry);
            Assert.AreEqual(0, dikeProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);
            Assert.IsFalse(dikeProfile.HasBreakWater);
            Assert.AreEqual(id, dikeProfile.Id);
            Assert.AreEqual("name", dikeProfile.Name);
            Assert.AreEqual(0, dikeProfile.Orientation.Value);
            Assert.AreEqual(point, dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0, dikeProfile.X0);
        }

        [Test]
        public void CreateDikeProfile_WithNameAndPoint_ReturnsExpectedValues()
        {
            // Setup
            const string name = "N";
            var point = new Point2D(-12.34, 7.78);

            // Call
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(name, point);

            // Assert
            Assert.IsNotNull(dikeProfile);
            Assert.IsNotNull(dikeProfile.ForeshoreProfile);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.IsEmpty(dikeProfile.DikeGeometry);
            Assert.AreEqual(0, dikeProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);
            Assert.IsFalse(dikeProfile.HasBreakWater);
            Assert.AreEqual("id", dikeProfile.Id);
            Assert.AreEqual(name, dikeProfile.Name);
            Assert.AreEqual(0, dikeProfile.Orientation.Value);
            Assert.AreEqual(point, dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0, dikeProfile.X0);
        }

        [Test]
        public void CreateDikeProfile_WithForeshoreGeometry_ReturnsExpectedValues()
        {
            // Setup
            IEnumerable<Point2D> foreshoreProfileGeometry = new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            };

            // Call
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(foreshoreProfileGeometry);

            // Assert
            Assert.IsNotNull(dikeProfile);
            Assert.IsNotNull(dikeProfile.ForeshoreProfile);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.IsEmpty(dikeProfile.DikeGeometry);
            Assert.AreEqual(0, dikeProfile.DikeHeight.Value);
            CollectionAssert.AreEqual(foreshoreProfileGeometry, dikeProfile.ForeshoreGeometry);
            Assert.IsFalse(dikeProfile.HasBreakWater);
            Assert.AreEqual("name", dikeProfile.Name);
            Assert.AreEqual(0, dikeProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0, dikeProfile.X0);
        }

        [Test]
        public void CreateDikeProfile_WithForeshoreProfileAndId_ReturnsExpectedValues()
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
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(foreshoreProfileGeometry, id);

            // Assert
            Assert.IsNotNull(dikeProfile);
            Assert.IsNotNull(dikeProfile.ForeshoreProfile);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.IsEmpty(dikeProfile.DikeGeometry);
            Assert.AreEqual(0, dikeProfile.DikeHeight.Value);
            CollectionAssert.AreEqual(foreshoreProfileGeometry, dikeProfile.ForeshoreGeometry);
            Assert.IsFalse(dikeProfile.HasBreakWater);
            Assert.AreEqual(id, dikeProfile.Id);
            Assert.AreEqual("name", dikeProfile.Name);
            Assert.AreEqual(0, dikeProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0, dikeProfile.X0);
        }

        [Test]
        public void CreateDikeProfile_WithDikeGeometry_ReturnsExpectedValues()
        {
            // Setup
            IEnumerable<RoughnessPoint> foreshoreProfileGeometry = new[]
            {
                new RoughnessPoint(new Point2D(0, 0), 5),
                new RoughnessPoint(new Point2D(1, 1), 6),
                new RoughnessPoint(new Point2D(2, 2), 7)
            };

            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(foreshoreProfileGeometry);

            // Assert
            Assert.IsNotNull(dikeProfile);
            Assert.IsNotNull(dikeProfile.ForeshoreProfile);
            Assert.IsNull(dikeProfile.BreakWater);
            CollectionAssert.AreEqual(foreshoreProfileGeometry, dikeProfile.DikeGeometry);
            Assert.AreEqual(0, dikeProfile.DikeHeight.Value);
            CollectionAssert.IsEmpty(dikeProfile.ForeshoreGeometry);
            Assert.IsFalse(dikeProfile.HasBreakWater);
            Assert.AreEqual("id", dikeProfile.Id);
            Assert.AreEqual("name", dikeProfile.Name);
            Assert.AreEqual(0, dikeProfile.Orientation.Value);
            Assert.AreEqual(new Point2D(0, 0), dikeProfile.WorldReferencePoint);
            Assert.AreEqual(0, dikeProfile.X0);
        }
    }
}