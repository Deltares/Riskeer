// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Segment2DTest
    {
        [Test]
        public void Constructor_WithTwoPoints_ReturnsSegmentWithPointsSet()
        {
            // Setup
            var firstPoint = new Point2D(0, 0);
            var secondPoint = new Point2D(0, 0);

            // Call
            var segment = new Segment2D(firstPoint, secondPoint);

            // Assert
            Assert.AreSame(firstPoint, segment.FirstPoint);
            Assert.AreSame(secondPoint, segment.SecondPoint);
        }

        [Test]
        public void Constructor_WithNullFirstPoint_ThrowsArgumentNullException()
        {
            // Setup
            var secondPoint = new Point2D(0, 0);

            // Call
            TestDelegate test = () => new Segment2D(null, secondPoint);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            StringAssert.StartsWith("Voor het maken van een segment zijn twee punten nodig.", exception.Message);
        }

        [Test]
        public void Constructor_WithNullSecondPoint_ThrowsArgumentNullException()
        {
            // Setup
            var firstPoint = new Point2D(0, 0);

            // Call
            TestDelegate test = () => new Segment2D(firstPoint, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            StringAssert.StartsWith("Voor het maken van een segment zijn twee punten nodig.", exception.Message);
        }

        [Test]
        [TestCase(1, 2, 1.5, true)]
        [TestCase(1, 2, 1 + 1e-6, true)]
        [TestCase(1, 2, 2 - 1e-6, true)]
        [TestCase(1, 2, 2, true)]
        [TestCase(1, 2, 1, true)]
        [TestCase(1, 1, 1, true)]
        [TestCase(1, 2, 0, false)]
        [TestCase(1, 2, 3, false)]
        [TestCase(1, 2, 2 + 1e-6, false)]
        [TestCase(1, 2, 1 - 1e-6, false)]
        public void ContainsX_DifferentSetsOfX_ReturnsExpectedValue(double firstPointX, double secondPointX, double containedX, bool isContained)
        {
            // Setup
            var random = new Random(22);
            var firstPoint = new Point2D(firstPointX, random.NextDouble());
            var secondPoint = new Point2D(secondPointX, random.NextDouble());
            var segment = new Segment2D(firstPoint, secondPoint);

            // Call
            bool result = segment.ContainsX(containedX);

            // Assert
            Assert.AreEqual(isContained, result);
        }

        [Test]
        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        [TestCase(1, 1 + 1e-5, false)]
        [TestCase(1, 1 - 1e-5, false)]
        [TestCase(1, 1 + 1e-7, true)]
        [TestCase(1, 1 - 1e-7, true)]
        public void IsVertical_DifferentSetsOfX_ReturnsExpectedValue(double firstPointX, double secondPointX, bool isVertical)
        {
            // Setup
            var random = new Random(22);
            var firstPoint = new Point2D(firstPointX, random.NextDouble());
            var secondPoint = new Point2D(secondPointX, random.NextDouble());
            var segment = new Segment2D(firstPoint, secondPoint);

            // Call
            bool result = segment.IsVertical();

            // Assert
            Assert.AreEqual(isVertical, result);
        }

        [Test]
        [TestCase(1e-9, false)]
        [TestCase(1e-6 + 1e-10, true)]
        [TestCase(1e-6 - 1e-10, false)]
        [TestCase(1e-5, true)]
        [TestCase(1, true)]
        public void IsVertical_DifferencesInY_ReturnsExpectedValue(double difference, bool isVertical)
        {
            // Setup
            var random = new Random(22);
            double x = random.NextDouble();
            double y = random.NextDouble();
            var firstPoint = new Point2D(x, y);
            var secondPoint = new Point2D(x, y + difference);
            var segment = new Segment2D(firstPoint, secondPoint);

            // Call
            bool result = segment.IsVertical();

            // Assert
            Assert.AreEqual(isVertical, result);
        }

        [Test]
        [TestCase(1.1, 2.2, 1.1, 2.2, 0)]
        [TestCase(1.2, 3.5, 8.13, 21.34, 19.138717)]
        public void Length_VariousCases_ReturnExpectedLength(
            double x1, double y1,
            double x2, double y2,
            double expectedLength)
        {
            // Setup
            var segment = new Segment2D(new Point2D(x1, y1), new Point2D(x2, y2));

            // Call
            double length = segment.Length;

            // Assert
            Assert.AreEqual(expectedLength, length, 1e-6);
        }

        [Test]
        public void GetEuclideanDistanceToPoint_PointOnFirstPoint_ReturnZero()
        {
            // Setup
            var segment = new Segment2D(new Point2D(1.2, 3.5), new Point2D(8.13, 21.34));

            // Call
            double euclideanDistance = segment.GetEuclideanDistanceToPoint(segment.FirstPoint);

            // Assert
            Assert.AreEqual(0, euclideanDistance);
        }

        [Test]
        public void GetEuclideanDistanceToPoint_PointOnSecondPoint_ReturnZero()
        {
            // Setup
            var segment = new Segment2D(new Point2D(1.2, 3.5), new Point2D(8.13, 21.34));

            // Call
            double euclideanDistance = segment.GetEuclideanDistanceToPoint(segment.SecondPoint);

            // Assert
            Assert.AreEqual(0, euclideanDistance);
        }

        [Test]
        public void GetEuclideanDistanceToPoint_PointIsNull_ThrowArgumentNullException()
        {
            // Setup
            var segment = new Segment2D(new Point2D(1.2, 3.5), new Point2D(8.13, 21.34));

            // Call
            TestDelegate call = () => segment.GetEuclideanDistanceToPoint(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        [TestCase(-2.3, 2.1)]
        [TestCase(0, 1.1)]
        [TestCase(3.2, -1.2)]
        public void GetEuclideanDistanceToPoint_FirstPointIsClosest_ReturnExpectedDistance(
            double x, double y)
        {
            // Setup
            var point = new Point2D(x, y);
            var segment = new Segment2D(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4));

            // Call
            double actualDistance = segment.GetEuclideanDistanceToPoint(point);

            // Assert
            double expectedDistance = point.GetEuclideanDistanceTo(segment.FirstPoint);
            Assert.AreEqual(expectedDistance, actualDistance);
        }

        [Test]
        [TestCase(2.2, 3.3, 0)]
        [TestCase(0, 3.3, 1.555634919)]
        [TestCase(4.4, -1.1, 4.666904756)]
        [TestCase(-2.2, 9.9, 7.778174593)]
        [TestCase(11, -3.3, 10.88944443)]
        [TestCase(1.5, 3.5, 0.636396103)]
        [TestCase(4.5, 1, 3.252691193)]
        public void GetEuclideanDistanceToPoint_LinePerpendicularToSegmentIsClosest_ReturnExpectedDistance(
            double x, double y, double expectedDistance)
        {
            // Setup
            var point = new Point2D(x, y);
            var segment = new Segment2D(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4));

            // Call
            double actualDistance = segment.GetEuclideanDistanceToPoint(point);

            // Assert
            Assert.AreEqual(expectedDistance, actualDistance, 1e-6);
        }

        [Test]
        [TestCase(5.3, 4.3)]
        [TestCase(6.6, 7.7)]
        [TestCase(2.7, 12.6)]
        public void GetEuclideanDistanceToPoint_SecondPointIsClosest_ReturnExpectedDistance(
            double x, double y)
        {
            // Setup
            var point = new Point2D(x, y);
            var segment = new Segment2D(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4));

            // Call
            double actualDistance = segment.GetEuclideanDistanceToPoint(point);

            // Assert
            double expectedDistance = point.GetEuclideanDistanceTo(segment.SecondPoint);
            Assert.AreEqual(expectedDistance, actualDistance);
        }

        [TestFixture]
        private class Segment2DEqualsTest : EqualsTestFixture<Segment2D>
        {
            [Test]
            public void Equals_SegmentWithTwoPointsWithSameXY_ReturnsTrue()
            {
                // Setup
                var random = new Random(22);
                double x1 = random.NextDouble();
                double x2 = random.NextDouble();
                double y1 = random.NextDouble();
                double y2 = random.NextDouble();
                var point1 = new Point2D(x1, y1);
                var point2 = new Point2D(x2, y2);
                var segment1 = new Segment2D(point1, point2);
                var segment2 = new Segment2D(point1, point2);
                var segment3 = new Segment2D(point2, point1);

                // Call
                bool segment1Equals2 = segment1.Equals(segment2);
                bool segment1Equals3 = segment1.Equals(segment3);

                // Assert
                Assert.IsTrue(segment1Equals2);
                Assert.IsTrue(segment1Equals3);
            }

            protected override Segment2D CreateObject()
            {
                return new Segment2D(CreatePoint2D(22),
                                     CreatePoint2D(23));
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                const double offset = 1e-6;
                Point2D firstPoint = CreatePoint2D(22);
                Point2D secondPoint = CreatePoint2D(23);

                yield return new TestCaseData(new Segment2D(firstPoint,
                                                            new Point2D(secondPoint.X + offset, secondPoint.Y)))
                    .SetName("One Point");

                yield return new TestCaseData(new Segment2D(
                                                  new Point2D(firstPoint.X + offset, secondPoint.Y),
                                                  new Point2D(secondPoint.X + offset, secondPoint.Y)))
                    .SetName("Two Points");
            }

            private static Point2D CreatePoint2D(int seed)
            {
                var random = new Random(seed);
                return new Point2D(random.NextDouble(), random.NextDouble());
            }
        }
    }
}