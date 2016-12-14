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
            Assert.IsNull(testProfile.Name);
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
            Assert.IsNull(testProfile.Name);
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
            Assert.AreEqual(name, testProfile.Name);
            Assert.AreEqual(0, testProfile.Orientation.Value);
            Assert.AreEqual(point, testProfile.WorldReferencePoint);
            Assert.AreEqual(0, testProfile.X0);
        }
    }
}