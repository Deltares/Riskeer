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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.TestUtil.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class RingTest
    {
        [Test]
        public void Constructor_WithoutPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new Ring(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("points", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutAtLeastTwoDistinctPoints_ThrowsArgumentException([Range(0, 4)] int times)
        {
            // Call
            TestDelegate test = () => new Ring(Enumerable.Repeat(new Point2D(3, 2), times));

            // Assert
            const string expectedMessage = "Need at least two distinct points to define a Ring.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
            Assert.AreEqual("points", exception.ParamName);
        }

        [Test]
        public void Constructor_WithPoints_ExpectedValues()
        {
            // Setup
            var points = new[]
            {
                new Point2D(3.0, 2.001),
                new Point2D(5.436, 6.4937),
                new Point2D(1, 1.23)
            };

            // Call
            var ring = new Ring(points);

            // Assert
            Assert.AreEqual(2, ring.Points.NumberOfDecimalPlaces);
            CollectionAssert.AreEqual(points, ring.Points, new Point2DComparerWithTolerance(1e-2));
        }

        [TestFixture]
        private class RingEqualsTest : EqualsTestFixture<Ring, DerivedRing>
        {
            protected override Ring CreateObject()
            {
                return CreateRing();
            }

            protected override DerivedRing CreateDerivedObject()
            {
                return new DerivedRing(CreateRing());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                Ring baseRing = CreateRing();
                List<Point2D> differentNrOfPoints = baseRing.Points.ToList();
                differentNrOfPoints.RemoveAt(0);
                yield return new TestCaseData(new Ring(differentNrOfPoints))
                    .SetName("Points count");

                baseRing = CreateRing();
                Point2D[] differentPoints = baseRing.Points.ToArray();
                differentPoints[0] = new Point2D(0, 0);
                yield return new TestCaseData(new Ring(differentPoints))
                    .SetName("Points Content");
            }

            private static Ring CreateRing()
            {
                var random = new Random(30);
                return new Ring(new[]
                {
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble())
                });
            }
        }

        private class DerivedRing : Ring
        {
            public DerivedRing(Ring ring) : base(ring.Points) {}
        }
    }
}