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
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Setup
            Ring layer = CreateRing();

            // Call
            bool areEqual = layer.Equals(new object());

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Setup
            Ring ring = CreateRing();

            // Call
            bool equal = ring.Equals(null);

            // Assert
            Assert.IsFalse(equal);
        }

        [Test]
        public void GetHashCode_EqualInstances_ReturnEqualHashes()
        {
            // Setup
            Ring ringA = CreateRandomRing(21);
            Ring ringB = CreateRandomRing(21);

            // Precondition
            Assert.AreEqual(ringA, ringB);
            Assert.AreEqual(ringB, ringA);

            // Call & Assert
            Assert.AreEqual(ringA.GetHashCode(), ringB.GetHashCode());
            Assert.AreEqual(ringB.GetHashCode(), ringA.GetHashCode());
        }

        [Test]
        [TestCaseSource(nameof(RingCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(Ring ring, Ring otherRing, bool expectedEqual)
        {
            // Call
            bool areEqualOne = ring.Equals(otherRing);
            bool areEqualTwo = otherRing.Equals(ring);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        private static TestCaseData[] RingCombinations()
        {
            Ring ringA = CreateRandomRing(21);
            Ring ringB = CreateRandomRing(21);
            Ring ringC = CreateRandomRing(73);
            Ring ringD = CreateRandomRing(21);

            return new[]
            {
                new TestCaseData(ringA, ringA, true)
                {
                    TestName = "Equals_RingARingA_True"
                },
                new TestCaseData(ringA, ringB, true)
                {
                    TestName = "Equals_RingARingB_True"
                },
                new TestCaseData(ringB, ringD, true)
                {
                    TestName = "Equals_RingBRingD_True"
                },
                new TestCaseData(ringA, ringD, true)
                {
                    TestName = "Equals_RingARingD_True"
                },
                new TestCaseData(ringB, ringC, false)
                {
                    TestName = "Equals_RingBRingC_False"
                },
                new TestCaseData(ringA, ringC, false)
                {
                    TestName = "Equals_RingARingC_False"
                }
            };
        }

        private static Ring CreateRandomRing(int seed)
        {
            var random = new Random(seed);
            var pointA = new Point2D(random.NextDouble(), random.NextDouble());
            var pointB = new Point2D(random.NextDouble(), random.NextDouble());
            var pointC = new Point2D(random.NextDouble(), random.NextDouble());

            return new Ring(new[]
            {
                pointA,
                pointB,
                pointC
            });
        }

        [Test]
        public void Points_RingWithPointSet_PointSetCopiedToNewCollection()
        {
            // Setup
            var points = new[]
            {
                new Point2D(3, 2),
                new Point2D(5, 6),
                new Point2D(1, 1.2)
            };

            var ring = new Ring(points);

            // Call
            IEnumerable<Point2D> ringPoints = ring.Points;

            // Assert
            TestHelper.AssertAreEqualButNotSame(points, ringPoints);
        }

        private Ring CreateRing()
        {
            var points = new[]
            {
                new Point2D(3, 2),
                new Point2D(5, 6),
                new Point2D(1, 1.2)
            };

            return new Ring(points);
        }
    }
}