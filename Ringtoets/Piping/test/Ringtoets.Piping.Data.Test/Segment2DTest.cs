using System;
using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class Segment2DTest
    {
        [Test]
        public void Constructor_WithTwoPoints_ReturnsSegmentWithPointsSet()
        {
            // Call
            var firstPoint = new Point2D();
            var secondPoint = new Point2D();
            var segment = new Segment2D(firstPoint, secondPoint);

            // Assert
            Assert.AreSame(firstPoint, segment.FirstPoint);
            Assert.AreSame(secondPoint, segment.SecondPoint);
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
        public void ContainsX_DifferentSetsOfX_ReturnsTrue(double firstPointX, double secondPointX, double containedX, bool isContained)
        {
            // Setup
            var random = new Random(22);
            var firstPoint = new Point2D
            {
                X = firstPointX,
                Y = random.NextDouble()
            };
            var secondPoint = new Point2D
            {
                X = secondPointX,
                Y = random.NextDouble()
            };
            var segment = new Segment2D(firstPoint, secondPoint);

            // Call
            var result = segment.ContainsX(containedX);

            // Assert
            Assert.AreEqual(isContained, result);
        }

        [Test]
        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        [TestCase(1, 1 + 1e-6, false)]
        [TestCase(1, 1 - 1e-6, false)]
        [TestCase(1, 1 + 1e-9, true)]
        [TestCase(1, 1 - 1e-9, true)]
        public void IsVertical_DifferentSetsOfX_ReturnsFalse(double firstPointX, double secondPointX, bool isVertical)
        {
            // Setup
            var random = new Random(22);
            var firstPoint = new Point2D
            {
                X = firstPointX,
                Y = random.NextDouble()
            };
            var secondPoint = new Point2D
            {
                X = secondPointX,
                Y = random.NextDouble()
            };
            var segment = new Segment2D(firstPoint, secondPoint);

            // Call
            var result = segment.IsVertical();

            // Assert
            Assert.AreEqual(isVertical, result);
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

            // Call & Assert
            Assert.IsTrue(segment.Equals(segment));
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

            // Call & Assert
            Assert.AreEqual(segment1.GetHashCode(), segment1.GetHashCode());
            Assert.AreEqual(segment1.GetHashCode(), segment2.GetHashCode());
            Assert.AreEqual(segment1.GetHashCode(), segment3.GetHashCode());
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
        public void IsConnected_ThreeSegments_AreAllConnected()
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
        public void IsConnected_ThreeSegments_AreAllDisconnected()
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
    }
}