// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
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
            var result = segment.ContainsX(containedX);

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
            var result = segment.IsVertical();

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
            var x = random.NextDouble();
            var y = random.NextDouble();
            var firstPoint = new Point2D(x, y);
            var secondPoint = new Point2D(x, y + difference);
            var segment = new Segment2D(firstPoint, secondPoint);

            // Call
            var result = segment.IsVertical();

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
        public void Equals_SameSegment_ReturnsTrue()
        {
            // Setup
            var random = new Random(22);
            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();
            var point1 = new Point2D(x1, y1);
            var point2 = new Point2D(x2, y2);
            var segment = new Segment2D(point1, point2);

            // Assert
            var isEqual = segment.Equals(segment);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            // Setup
            var random = new Random(22);
            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();
            var point1 = new Point2D(x1, y1);
            var point2 = new Point2D(x2, y2);
            var segment = new Segment2D(point1, point2);

            // Call & Assert
            Assert.IsFalse(segment.Equals(null));
        }

        [Test]
        public void Equals_WithOtherObjectType_ReturnsFalse()
        {
            // Setup
            var random = new Random(22);
            var segment = new Segment2D(new Point2D(random.NextDouble(), random.NextDouble()), new Point2D(random.NextDouble(), random.NextDouble()));

            // Call
            var isEqual = segment.Equals(new Point2D(0.0, 0.0));

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_SegmentWithTwoPointsWithSameXY_ReturnsTrue()
        {
            // Setup
            var random = new Random(22);
            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();
            var point1 = new Point2D(x1, y1);
            var point2 = new Point2D(x2, y2);
            var segment1 = new Segment2D(point1, point2);
            var segment2 = new Segment2D(point1, point2);
            var segment3 = new Segment2D(point2, point1);

            // Call & Assert
            Assert.IsTrue(segment1.Equals(segment2));
            Assert.IsTrue(segment1.Equals(segment3));
        }

        [Test]
        public void Equals_SegmentWithTwoPointsWithDifferentXY_ReturnsFalse()
        {
            // Setup
            var random = new Random(22);
            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var x3 = random.NextDouble();
            var x4 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();
            var y3 = random.NextDouble();
            var y4 = random.NextDouble();
            var point1 = new Point2D(x1, y1);
            var point2 = new Point2D(x2, y2);
            var point3 = new Point2D(x3, y3);
            var point4 = new Point2D(x4, y4);
            var segment1 = new Segment2D(point1, point2);
            var segment2 = new Segment2D(point3, point4);

            // Call & Assert
            Assert.IsFalse(segment1.Equals(segment2));
            Assert.IsFalse(segment2.Equals(segment1));
        }

        [Test]
        public void GetHashCode_EqualSegments_AreEqual()
        {
            // Setup
            var random = new Random(22);
            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();
            var point1 = new Point2D(x1, y1);
            var point2 = new Point2D(x2, y2);
            var segment1 = new Segment2D(point1, point2);
            var segment2 = new Segment2D(point1, point2);
            var segment3 = new Segment2D(point2, point1);

            // Precondition
            Assert.AreEqual(segment1, segment1);
            Assert.AreEqual(segment1, segment2);
            Assert.AreEqual(segment1, segment3);

            // Call & Assert
            Assert.AreEqual(segment1.GetHashCode(), segment1.GetHashCode());
            Assert.AreEqual(segment1.GetHashCode(), segment2.GetHashCode());
            Assert.AreEqual(segment1.GetHashCode(), segment3.GetHashCode());
        }

        [Test]
        public void IsConnected_SegmentNull_ThrowArgumentNullException()
        {
            // Setup
            var segment = new Segment2D(new Point2D(0, 0), new Point2D(1, 1));

            // Call
            TestDelegate call = () => segment.IsConnected(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("segment", paramName);
        }

        [Test]
        public void IsConnected_SameSegment_True()
        {
            // Setup
            var random = new Random(22);
            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();
            var point1 = new Point2D(x1, y1);
            var point2 = new Point2D(x2, y2);
            var segment1 = new Segment2D(point1, point2);

            // Call & Assert
            Assert.IsTrue(segment1.IsConnected(segment1));
        }

        [Test]
        public void IsConnected_EqualSegments_True()
        {
            // Setup
            var random = new Random(22);
            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();
            var point1 = new Point2D(x1, y1);
            var point2 = new Point2D(x2, y2);
            var segment1 = new Segment2D(point1, point2);
            var segment2 = new Segment2D(point1, point2);

            // Call & Assert
            Assert.IsTrue(segment1.IsConnected(segment2));
            Assert.IsTrue(segment2.IsConnected(segment1));
        }

        [Test]
        public void IsConnected_ThreeConnectedSegments_AreAllConnected()
        {
            // Setup
            var random = new Random(22);
            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var x3 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();
            var y3 = random.NextDouble();

            var point1 = new Point2D(x1, y1);
            var point2 = new Point2D(x2, y2);
            var point3 = new Point2D(x3, y3);
            var segment1 = new Segment2D(point1, point2);
            var segment2 = new Segment2D(point2, point3);
            var segment3 = new Segment2D(point1, point3);

            // Call & Assert
            Assert.IsTrue(segment1.IsConnected(segment2));
            Assert.IsTrue(segment1.IsConnected(segment3));

            Assert.IsTrue(segment2.IsConnected(segment1));
            Assert.IsTrue(segment2.IsConnected(segment3));

            Assert.IsTrue(segment3.IsConnected(segment1));
            Assert.IsTrue(segment3.IsConnected(segment2));
        }

        [Test]
        public void IsConnected_TwoSeperateSegments_AreDisconnected()
        {
            // Setup
            var random = new Random(22);
            var x1 = random.NextDouble();
            var x2 = random.NextDouble();
            var x3 = random.NextDouble();
            var x4 = random.NextDouble();
            var y1 = random.NextDouble();
            var y2 = random.NextDouble();
            var y3 = random.NextDouble();
            var y4 = random.NextDouble();

            var point1 = new Point2D(x1, y1);
            var point2 = new Point2D(x2, y2);
            var point3 = new Point2D(x3, y3);
            var point4 = new Point2D(x4, y4);
            var segment1 = new Segment2D(point1, point2);
            var segment2 = new Segment2D(point3, point4);

            // Call & Assert
            Assert.IsFalse(segment1.IsConnected(segment2));
            Assert.IsFalse(segment2.IsConnected(segment1));
        }

        [Test]
        public void GetEuclideanDistanceToPoint_PointOnFirstPoint_ReturnZero()
        {
            // Setup
            var segment = new Segment2D(new Point2D(1.2, 3.5), new Point2D(8.13, 21.34));

            // Call
            var euclideanDistance = segment.GetEuclideanDistanceToPoint(segment.FirstPoint);

            // Assert
            Assert.AreEqual(0, euclideanDistance);
        }

        [Test]
        public void GetEuclideanDistanceToPoint_PointOnSecondPoint_ReturnZero()
        {
            // Setup
            var segment = new Segment2D(new Point2D(1.2, 3.5), new Point2D(8.13, 21.34));

            // Call
            var euclideanDistance = segment.GetEuclideanDistanceToPoint(segment.SecondPoint);

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
            var actualDistance = segment.GetEuclideanDistanceToPoint(point);

            // Assert
            var expectedDistance = point.GetEuclideanDistanceTo(segment.FirstPoint);
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
            var actualDistance = segment.GetEuclideanDistanceToPoint(point);

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
            var actualDistance = segment.GetEuclideanDistanceToPoint(point);

            // Assert
            var expectedDistance = point.GetEuclideanDistanceTo(segment.SecondPoint);
            Assert.AreEqual(expectedDistance, actualDistance);
        }
    }
}