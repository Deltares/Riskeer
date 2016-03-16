using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Math2DTest
    {
        #region testcases

        /// <summary>
        /// Test cases for intersecting segments. The <see cref="Array"/> contains pairs of <see cref="double"/>,
        /// which represent the coordinate of a point. Each pair of coordinates form a segment. 
        /// The last 2 double values are the expected intersection points.
        /// </summary>
        private static IEnumerable IntersectingSegments()
        {
            // \/
            // /\
            var testCaseDatadata1 = new TestCaseData(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(1.0, 0.0),
                new Point2D(0.0, 1.0),
                new Point2D(0.5, 0.5)
            }, "IntersectingSegments 1");
            yield return testCaseDatadata1;

            // __
            //  /
            // /
            var testCaseDatadata2 = new TestCaseData(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(0.0, 1.0),
                new Point2D(1.0, 1.0),
                new Point2D(1.0, 1.0)
            }, "IntersectingSegments 2");
            yield return testCaseDatadata2;

            // 
            //  /
            // /__
            var testCaseDatadata3 = new TestCaseData(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 0.0),
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0),
                new Point2D(0.0, 0.0)
            }, "IntersectingSegments 3");
            yield return testCaseDatadata3;
        }

        /// <summary>
        /// Test cases for intersecting segments. The <see cref="Array"/> contains pairs of <see cref="double"/>,
        /// which represent the coordinate of a point. Each pair of coordinates form a segment. 
        /// The last 2 point2D values are the expected intersection points.
        /// </summary>
        private static IEnumerable MultipleIntersectingSegments()
        {
            //  _|_
            // / | \
            // \_|_/
            var testCaseDatadata1 = new TestCaseData(new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(0.0, 1.0),
                new Point2D(1.0, 2.0),
                new Point2D(3.0, 2.0),
                new Point2D(4.0, 1.0),
                new Point2D(3.0, 0.0),
                new Point2D(1.0, 0.0),
                new Point2D(2.0, 0.0),
                new Point2D(2.0, 2.0),
                new Point2D(2.0, 0.0),
                new Point2D(2.0, 2.0)
            }, "MultipleIntersectingSegments 1");
            yield return testCaseDatadata1;

        }

        /// <summary>
        /// Test cases for parallel segments. The <see cref="Array"/> contains pairs of <see cref="double"/>,
        /// which represent the coordinate of a point. Each pair of coordinates form a segment.
        /// </summary>
        private static IEnumerable ParallelSegments()
        {
            // __
            // __
            var testCaseDatadata1 = new TestCaseData(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 0.0),
                new Point2D(0.0, 1.0),
                new Point2D(1.0, 1.0)
            }, "ParallelSegments");
            yield return testCaseDatadata1;

            // ____ (connected in single point)
            var testCaseDatadata2 = new TestCaseData(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 0.0),
                new Point2D(1.0, 0.0),
                new Point2D(2.0, 0.0)
            }, "ParallelSegments, connected in single point");
            yield return testCaseDatadata2;

            // __ (overlap)
            var testCaseDatadata3 = new TestCaseData(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 0.0),
                new Point2D(0.5, 0.0),
                new Point2D(1.5, 0.0)
            }, "ParallelSegments, overlap");
            yield return testCaseDatadata3;
        }

        /// <summary>
        /// Test cases for non intersecting segments. The <see cref="Array"/> contains pairs of <see cref="double"/>,
        /// which represent the coordinate of a point. Each pair of coordinates form a segment.
        /// </summary>
        private static readonly Point2D[][] NonIntersectingSegments =
        {
            //  |
            // ___
            new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 0.0),
                new Point2D(0.5, 1.0),
                new Point2D(0.5, 0.5),
                new Point2D(0.5, 0.0)
            }
        };

        #endregion

        [Test]
        [TestCaseSource("IntersectingSegments")]
        public void LineIntersectionWithLine_DifferentLineSegmentsWithIntersections_ReturnsPoint(Point2D[] points, string testname = "")
        {
            // Call
            var result = Math2D.LineIntersectionWithLine(points[0], points[1], points[2], points[3]);

            // Assert
            Assert.AreEqual(points[4], result);
        }

        [Test]
        [TestCaseSource("ParallelSegments")]
        // String testname was added because the Teamcity report only counts the unique signatures that were tested
        public void LineIntersectionWithLine_DifferentParallelLineSegments_ReturnsNoPoint(Point2D[] points, string testname = "")
        {
            // Call
            var result = Math2D.LineIntersectionWithLine(points[0], points[1], points[2], points[3]);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCaseSource("NonIntersectingSegments")]
        public void LineIntersectionWithLine_DifferentLineSegmentsWithNoIntersection_ReturnsPoint(Point2D[] points)
        {
            // Call
            var result = Math2D.LineIntersectionWithLine(points[0], points[1], points[2], points[3]);

            // Assert
            Assert.AreEqual(points[4], result);
        }

        [Test]
        public void LineIntersectionWithLine_WithEqualPoints_ThrowsArgumentException()
        {
            // Call
            TestDelegate testA = () => Math2D.LineIntersectionWithLine(new Point2D(0, 0), new Point2D(0, 0), new Point2D(1, 0), new Point2D(0, 1));
            TestDelegate testB = () => Math2D.LineIntersectionWithLine(new Point2D(0, 1), new Point2D(0, 0), new Point2D(1, 1), new Point2D(1, 1));

            // Assert
            var exceptionA = Assert.Throws<ArgumentException>(testA);
            var exceptionB = Assert.Throws<ArgumentException>(testB);
            Assert.AreEqual(Resources.Math2D_LineIntersectionWithLine_Line_points_are_equal, exceptionA.Message);
            Assert.AreEqual(Resources.Math2D_LineIntersectionWithLine_Line_points_are_equal, exceptionB.Message);
        }

        [Test]
        public void LineIntersectionWithLine_InterSectionsHigherUpX_ReturnsIntersectionWithTolerance()
        {
            // Setup
            var y1 = 5.925;
            var y2 = 5.890;
            var start = 133;

            // Call
            var result = Math2D.LineIntersectionWithLine(new Point2D(start, y1), new Point2D(start + 1, y2), new Point2D(start + 0.5, 0), new Point2D(start + 0.5, 1));

            // Assert
            Assert.AreEqual((y1 + y2)/2, result.Y, 1e-8);
        }

        [Test]
        [TestCase(2.5, new [] {3.3})]
        [TestCase(1.1, new double[0])]
        [TestCase(5.5, new double[0])]
        [TestCase(-1.5, new []{1.5, 3.75})]
        public void SegmentsIntersectionWithVerticalLine_SegmentsCollectionNotIntersecting_ReturnsEmptyCollection(double x, double[] intersectionHeights)
        {
            // Setup
            var segments = new[]
            {
                new Segment2D(new Point2D(2.2, 3.3), new Point2D(3.3, 3.3)),
                new Segment2D(new Point2D(1.1, 5.0), new Point2D(1.1, 2.0)), // vertical
                new Segment2D(new Point2D(5.5, 2.0), new Point2D(5.5, 2.0)), // no length
                new Segment2D(new Point2D(-2.0, 1.0), new Point2D(-1.0, 2.0)),
                new Segment2D(new Point2D(-1.0, 2.0), new Point2D(-2.0, 5.5))
            };

            // Call
            var result = Math2D.SegmentsIntersectionWithVerticalLine(segments, x);

            // Assert
            Assert.AreEqual(intersectionHeights.Select(y => new Point2D(x, y)), result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void ConvertLinePointsToLineSegments_TooFewPoints_ReturnEmpty(int pointCount)
        {
            // Setup
            var linePoints = Enumerable.Repeat(new Point2D(0, 0), pointCount);

            // Call
            IEnumerable<Segment2D> segments = Math2D.ConvertLinePointsToLineSegments(linePoints);

            // Assert
            CollectionAssert.IsEmpty(segments);
        }

        [Test]
        public void ConvertLinePointsToLineSegments_TwoPoints_ReturnOneSegmentOfThoseTwoPoints()
        {
            // Setup
            var linePoints = new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4),
            };

            // Call
            Segment2D[] segments = Math2D.ConvertLinePointsToLineSegments(linePoints).ToArray();

            // Assert
            Assert.AreEqual(1, segments.Length);
            Assert.AreEqual(linePoints[0], segments[0].FirstPoint);
            Assert.AreEqual(linePoints[1], segments[0].SecondPoint);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void SplitLineAtLengths_TooFewPoints_ThrowArgumentException(int pointCount)
        {
            // Setup
            var originalLine = Enumerable.Repeat(new Point2D(0.0, 0.0), pointCount);

            var lengths = new[]
            {
                0.0
            };

            // Call
            TestDelegate call = () => Math2D.SplitLineAtLengths(originalLine, lengths);

            // Assert
            var expectedMessage = "Er zijn niet genoeg punten beschikbaar om een lijn te definiëren.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void SplitLineAtLengths_NegativeLength_ThrowArgumentException()
        {
            // Setup
            var originalLine = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(6.0, 0.0)
            };

            var lengths = new[]
            {
                2.0,
                6.0,
                -2.0
            };

            // Call
            TestDelegate call = () => Math2D.SplitLineAtLengths(originalLine, lengths);

            // Assert
            var expectedMessage = "Er mogen geen negatieve lengtes worden opgegeven om de lijn mee op te knippen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(2.0 - 1.1e-6)]
        [TestCase(2.0 + 1.1e-6)]
        [TestCase(67.8)]
        public void SplitLineAtLengths_LengthsDoNotFullyCoverLine_ThrowArgumentException(double l)
        {
            // Setup
            var originalLine = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(6.0, 0.0)
            };

            var lengths = new[]
            {
                2.0,
                2.0,
                l
            };

            // Call
            TestDelegate call = () => Math2D.SplitLineAtLengths(originalLine, lengths);

            // Assert
            var expectedMessage = "De som van alle lengtes moet gelijk zijn aan de lengte van de opgegeven lijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void SplitLineAtLengths_OneLengthsForWholeLine_ReturnAllLinePoints()
        {
            // Setup
            var originalLine = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(2.0, 0.0),
                new Point2D(4.0, 0.0),
                new Point2D(6.0, 0.0)
            };

            var lengths = new[]
            {
                6.0
            };

            // Call
            IEnumerable<Point2D>[] lineSplits = Math2D.SplitLineAtLengths(originalLine, lengths);

            // Assert
            Assert.AreEqual(1, lineSplits.Length);
            Assert.AreNotSame(originalLine, lineSplits[0]);
            CollectionAssert.AreEqual(originalLine, lineSplits[0]);
        }

        [Test]
        public void SplitLineAtLengths_LongLineSplitInFourPieces_ReturnFourSplitResults()
        {
            // Setup
            var originalLine = new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(20.0, 60.0),
            };

            var lengths = GetLengthsBasedOnReletative(new[]
            {
                0.25,
                0.25,
                0.15,
                0.35
            }, originalLine);

            // Call
            Point2D[][] lineSplits = Math2D.SplitLineAtLengths(originalLine, lengths);

            // Assert
            var doubleToleranceComparer = new Point2DComparerWithTolerance(1e-6);
            Assert.AreEqual(4, lineSplits.Length);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 0),
                new Point2D(5.0, 15.0)
            }, lineSplits[0], doubleToleranceComparer);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(5.0, 15.0),
                new Point2D(10.0, 30)
            }, lineSplits[1], doubleToleranceComparer);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(10.0, 30.0),
                new Point2D(13.0, 39.0)
            }, lineSplits[2], doubleToleranceComparer);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(13.0, 39.0),
                new Point2D(20.0, 60.0)
            }, lineSplits[3], doubleToleranceComparer);
        }

        private double[] GetLengthsBasedOnReletative(double[] relativeLengths, IEnumerable<Point2D> lineGeometryPoints)
        {
            var lineLength = Math2D.ConvertLinePointsToLineSegments(lineGeometryPoints).Sum(s => s.Length);
            return relativeLengths.Select(l => lineLength*l).ToArray();
        }

        private class Point2DComparerWithTolerance : IComparer<Point2D>, IComparer
        {
            private readonly double tolerance;

            public Point2DComparerWithTolerance(double tolerance)
            {
                this.tolerance = tolerance;
            }

            public int Compare(object x, object y)
            {
                return Compare(x as Point2D, y as Point2D);
            }

            public int Compare(Point2D p0, Point2D p1)
            {
                double diff = p0.GetEuclideanDistanceTo(p1);
                return Math.Abs(diff) < tolerance ? 0 : 1;
            }
        }

        [Test]
        [TestCaseSource("IntersectingSegments")]
        public void SingleSegmentIntersectionWithSingleSegment_DifferentLineSegmentsWithIntersections_ReturnsPoint(Point2D[] points, string testname = "")
        {
            // Call
            var result = Math2D.SingleSegmentIntersectionWithSingleSegment(new Segment2D(points[0], points[1]), new Segment2D(points[2], points[3]));

            // Assert
            Assert.AreEqual(points[4], result);
        }

        [Test]
        [TestCaseSource("ParallelSegments")]
        // String testname was added because the Teamcity report only counts the unique signatures that were tested
        public void SingleSegmentIntersectionWithSingleSegment_DifferentParallelLineSegments_ReturnsPointWhenConnectedOtherwiseNull(Point2D[] points, string testname = "")
        {
            // Call
            var result = Math2D.SingleSegmentIntersectionWithSingleSegment(new Segment2D(points[0], points[1]), new Segment2D(points[2], points[3]));

            // Assert
            if (Math2D.AreEqualPoints(points[1], points[2]))
            {
                Assert.AreEqual(points[1], result);
            }
            else
            {
                Assert.IsNull(result);
            }
        }

        [Test]
        [TestCaseSource("NonIntersectingSegments")]
        public void SingleSegmentIntersectionWithSingleSegment_DifferentLineSegmentsWithNoIntersection_ReturnsNull(Point2D[] points)
        {
            // Call
            var result = Math2D.SingleSegmentIntersectionWithSingleSegment(new Segment2D(points[0], points[1]), new Segment2D(points[2], points[3]));

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void SingleSegmentIntersectionWithSingleSegment_InterSectionsHigherUpX_ReturnsNull()
        {
            // Setup
            var y1 = 5.925;
            var y2 = 5.890;
            var start = 133;

            // Call
            var result = Math2D.SingleSegmentIntersectionWithSingleSegment(new Segment2D(new Point2D(start, y1), new Point2D(start + 1, y2)), new Segment2D(new Point2D(start + 0.5, 0), new Point2D(start + 0.5, 1)));

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCaseSource("MultipleIntersectingSegments")]
        public void SegmentsIntersectionsWithSegments_DifferentLinesWithMultipleIntersections_ReturnsMultiplePoints(Point2D[] points, string testname = "")
        {
            // Setup
            var segments = Math2D.ConvertLinePointsToLineSegments(new List<Point2D>
            {
                points[0], points[1], points[2], points[3], points[4], points[5], points[6]
            });

            var lineSegments = Math2D.ConvertLinePointsToLineSegments(new List<Point2D>
            {
                points[7], points[8]
            });

            // Call
            var result = Math2D.SegmentsIntersectionsWithSegments(segments, lineSegments).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(points[9], result[1]);
            Assert.AreEqual(points[10], result[0]);
        }

        [Test]
        [TestCaseSource("IntersectingSegments")]
        public void SegmentsIntersectionsWithSegments_DifferentLineSegmentsWithIntersections_ReturnsPoint(Point2D[] points, string testname = "")
        {
            // Call
            var result = Math2D.SegmentsIntersectionsWithSegments(new List<Segment2D> {new Segment2D(points[0], points[1])}, new List<Segment2D> {new Segment2D(points[2], points[3])});

            // Assert
            CollectionAssert.AreEquivalent(new [] {points[4]}, result);
        }

        [Test]
        [TestCaseSource("ParallelSegments")]
        // String testname was added because the Teamcity report only counts the unique signatures that were tested
        public void SegmentsIntersectionsWithSegments_DifferentParallelLineSegments_ReturnsPointWhenConnectedOtherwiseEmptyList(Point2D[] points, string testname = "")
        {
            // Call
            var result = Math2D.SegmentsIntersectionsWithSegments(new List<Segment2D> { new Segment2D(points[0], points[1]) }, new List<Segment2D> { new Segment2D(points[2], points[3]) });

            // Assert
            if (Math2D.AreEqualPoints(points[1], points[2]))
            {
                CollectionAssert.AreEquivalent(new [] {points[1]}, result);
            }
            else
            {
                CollectionAssert.IsEmpty(result);
            }
        }

        [Test]
        [TestCaseSource("NonIntersectingSegments")]
        public void SegmentsIntersectionsWithSegments_DifferentLineSegmentsWithNoIntersection_ReturnsEmtpyList(Point2D[] points)
        {
            // Call
            var result = Math2D.SegmentsIntersectionsWithSegments(new List<Segment2D> { new Segment2D(points[0], points[1]) }, new List<Segment2D> { new Segment2D(points[2], points[3]) });

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void SegmentsIntersectionsWithSegments_WithEqualPoints_ReturnsMultiplePoints()
        {
            // Setup
            var segments1 = new List<Segment2D>
            {
                new Segment2D(new Point2D(0, 0), new Point2D(0, 0)), new Segment2D(new Point2D(1, 0), new Point2D(0, 1))
            };
            var segments2 = new List<Segment2D>
            {
                new Segment2D(new Point2D(0, 1), new Point2D(0, 0)), new Segment2D(new Point2D(1, 1), new Point2D(1, 1))
            };
            // Call
            var result = Math2D.SegmentsIntersectionsWithSegments(segments1, segments2);

            // Assert
            CollectionAssert.AreEquivalent(new[] { new Point2D(0, 0), new Point2D(0, 1) }, result);
        }

        [Test]
        public void SegmentsIntersectionsWithSegments_InterSectionsHigherUpX_ReturnsEmptyList()
        {
            // Setup
            var y1 = 5.925;
            var y2 = 5.890;
            var start = 133;

            // Call
            var result = Math2D.SegmentsIntersectionsWithSegments(new List<Segment2D> {new Segment2D(new Point2D(start, y1), new Point2D(start + 1, y2))}, new List<Segment2D> {new Segment2D(new Point2D(start + 0.5, 0), new Point2D(start + 0.5, 1))});

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void AreEqualPoints_PointsEqual_ReturnsTrue()
        {
            // Call
            var result = Math2D.AreEqualPoints(new Point2D(0, 0), new Point2D(0, 0));

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void AreEqualPoints_PointsNotEqual_ReturnsFalse()
        {
            // Call
            var result = Math2D.AreEqualPoints(new Point2D(1, 0), new Point2D(4, 9));

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Length_EmptyCollection_ReturnsZero()
        {
            // Setup
            var points = new Point2D[0];

            // Call
            var length = Math2D.Length(points);

            // Assert
            Assert.AreEqual(0, length);
        }

        [Test]
        public void Length_CollectionWithSinglePoint_ReturnsZero()
        {
            // Setup
            var random = new Random(21);
            var points = new []
            {
                new Point2D(random.NextDouble(), random.NextDouble()), 
            };

            // Call
            var length = Math2D.Length(points);

            // Assert
            Assert.AreEqual(0, length);
        }

        [Test]
        public void Length_CollectionWithTwoPoints_ReturnsDistanceBetweenPoints()
        {
            // Setup
            var random = new Random(21);

            var point1 = new Point2D(random.NextDouble(), random.NextDouble());
            var point2 = new Point2D(random.NextDouble(), random.NextDouble());

            var points = new[]
            {
                point1,
                point2
            };

            // Call
            var length = Math2D.Length(points);

            // Assert
            Assert.AreEqual(point2.GetEuclideanDistanceTo(point1), length);
        }

        [Test]
        [TestCase(3)]
        [TestCase(5)]
        public void Length_CollectionWithMoreThanTwoPoints_ReturnsSumOfDistanceBetweenPoints(int count)
        {
            // Setup
            var random = new Random(21);

            var points = new List<Point2D>(count);
            double expectedLength = 0;
            Point2D previousPoint = null;

            for (int i = 0; i < count; i++)
            {
                var point = new Point2D(random.NextDouble(), random.NextDouble());
                if (previousPoint != null)
                {
                    expectedLength += previousPoint.GetEuclideanDistanceTo(point);
                }
                points.Add(point);
                previousPoint = point;
            }

            // Call
            var length = Math2D.Length(points);

            // Assert
            Assert.AreEqual(expectedLength, length);
        }
    }
}