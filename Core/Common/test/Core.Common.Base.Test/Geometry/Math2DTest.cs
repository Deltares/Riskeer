﻿using System;
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

        [Test]
        [TestCase(1234.56789)]
        [TestCase(1e-6)]
        [TestCase(-1e-6)]
        [TestCase(-98765.4321)]
        public void GetIntersectionBetweenSegments_TwoHorizontalParallelSegments_ReturnNoIntersection(
            double dy)
        {
            // Setup
            const double y1 = 2.2;
            double y2 = y1+dy;

            const double x1 = 1.1;
            const double x2 = 3.3;
            var horizontalSegment1 = new Segment2D(new Point2D(x1, y1), new Point2D(x2, y1));
            var horizontalSegment2 = new Segment2D(new Point2D(x1, y2), new Point2D(x2, y2));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(horizontalSegment1, horizontalSegment2);

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
        }

        [Test]
        [TestCase(1234.56789)]
        [TestCase(1e-6)]
        [TestCase(-1e-6)]
        [TestCase(-98765.4321)]
        public void GetIntersectionBetweenSegments_TwoVerticalParallelSegments_ReturnNoIntersection(
            double dx)
        {
            // Setup
            const double x1 = 1.1;
            double x2 = x1 + dx;

            const double y1 = 2.2;
            const double y2 = 3.3;
            var horizontalSegment1 = new Segment2D(new Point2D(x1, y1), new Point2D(x1, y2));
            var horizontalSegment2 = new Segment2D(new Point2D(x2, y1), new Point2D(x2, y2));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(horizontalSegment1, horizontalSegment2);

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
        }

        [Test]
        public void GetIntersectionBetweenSegments_TwoParallelSegments_ReturnNoIntersection()
        {
            // Setup
            var segment1 = new Segment2D(new Point2D(1.1, 3.3), new Point2D(2.2, 4.4));
            var segment2 = new Segment2D(new Point2D(1.1, 5.5), new Point2D(2.2, 6.6));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
        }

        [Test]
        [TestCase(12.34)]
        [TestCase(1.1 + 1e-6)]
        [TestCase(-1.1 - 1e-6)]
        [TestCase(-56.78)]
        public void GetIntersectionBetweenSegments_TwoCollinearHorizontalLinesWithoutOverlap_ReturnNoIntersection(double dx)
        {
            // Setup
            const double x1 = 1.1;
            const double x2 = 2.2;
            const double y = 3.3;

            double x3 = x1 + dx;
            double x4 = x2 + dx;
            var horizontalSegment1 = new Segment2D(new Point2D(x1, y), new Point2D(x2, y));
            var horizontalSegment2 = new Segment2D(new Point2D(x3, y), new Point2D(x4, y));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(horizontalSegment1, horizontalSegment2);

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
        }

        [Test]
        [TestCase(12.34)]
        [TestCase(3.3 + 1e-6)]
        [TestCase(-3.3 - 1e-6)]
        [TestCase(-56.78)]
        public void GetIntersectionBetweenSegments_TwoCollinearVerticalLinesWithoutOverlap_ReturnNoIntersection(double dy)
        {
            // Setup
            const double y1 = 1.1;
            const double y2 = 4.4;
            const double x = 3.3;

            double y3 = y2 + dy;
            double y4 = y1 + dy;
            var horizontalSegment1 = new Segment2D(new Point2D(x, y1), new Point2D(x, y2));
            var horizontalSegment2 = new Segment2D(new Point2D(x, y3), new Point2D(x, y4));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(horizontalSegment1, horizontalSegment2);

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
        }

        [Test]
        [TestCase(-3.3)]
        [TestCase(7.7)]
        public void GetIntersectionBetweenSegments_TwoCollinearSegmentsWithoutOverlap_ReturnNoIntersection(
            double dx)
        {
            // Setup
            Func<double, double> getY = x => 1.1 * x + 2.2;
            const double x1 = 1.1;
            const double x2 = 3.3;
            var segment1 = new Segment2D(new Point2D(x1, getY(x1)), new Point2D(x2, getY(x2)));

            double x3 = x1 + dx;
            double x4 = x2 + dx;
            var segment2 = new Segment2D(new Point2D(x3, getY(x3)), new Point2D(x4, getY(x4)));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetIntersectionBetweenSegments_TwoCollinearHorizontalSegmentsConnectedAtSegmentEnds_ReturnIntersectionPoint(
            int configurationNumber)
        {
            // Setup
            const double y = -6.7;
            var segment1UniquePoint = new Point2D(-12.34, y);
            var segmentCommonPoint = new Point2D(56.78, y);
            var segment2UniquePoint = new Point2D(91.23, y);

            Segment2D horizontalSegment1, horizontalSegment2;
            if (configurationNumber == 0 || configurationNumber == 3)
            {
                horizontalSegment1 = new Segment2D(segment1UniquePoint, segmentCommonPoint);
            }
            else
            {
                horizontalSegment1 = new Segment2D(segmentCommonPoint, segment1UniquePoint);
            }

            if (configurationNumber == 0 || configurationNumber == 1)
            {
                horizontalSegment2 = new Segment2D(segment2UniquePoint, segmentCommonPoint);
            }
            else
            {
                horizontalSegment2 = new Segment2D(segmentCommonPoint, segment2UniquePoint);
            }

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(horizontalSegment1, horizontalSegment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Intersects, result.IntersectionType);
            CollectionAssert.AreEqual(new[] { segmentCommonPoint }, result.IntersectionPoints);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetIntersectionBetweenSegments_TwoCollinearVerticalSegmentsConnectedAtSegmentEnds_ReturnIntersectionPoint(
            int configurationNumber)
        {
            // Setup
            const double x = 5.5;
            var segment1UniquePoint = new Point2D(x, -23.45);
            var segmentCommonPoint = new Point2D(x, -12.34);
            var segment2UniquePoint = new Point2D(x, 90.76);

            Segment2D verticalSegment1, verticalSegment2;
            if (configurationNumber == 0 || configurationNumber == 3)
            {
                verticalSegment1 = new Segment2D(segment1UniquePoint, segmentCommonPoint);
            }
            else
            {
                verticalSegment1 = new Segment2D(segmentCommonPoint, segment1UniquePoint);
            }

            if (configurationNumber == 0 || configurationNumber == 1)
            {
                verticalSegment2 = new Segment2D(segment2UniquePoint, segmentCommonPoint);
            }
            else
            {
                verticalSegment2 = new Segment2D(segmentCommonPoint, segment2UniquePoint);
            }

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(verticalSegment1, verticalSegment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Intersects, result.IntersectionType);
            CollectionAssert.AreEqual(new[] { segmentCommonPoint }, result.IntersectionPoints,
                new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetIntersectionBetweenSegments_TwoCollinearSegmentsConnectedAtSegmentEnds_ReturnIntersectionPoint(
            int configurationNumber)
        {
            // Setup
            Func<double, double> getY = x => 3.3 * x + 4.4;

            const double x1 = 5.5;
            const double x2 = 6.6;
            const double x3 = 8.8;
            var segment1UniquePoint = new Point2D(x1, getY(x1));
            var segmentCommonPoint = new Point2D(x2, getY(x2));
            var segment2UniquePoint = new Point2D(x3, getY(x3));

            Segment2D segment1, segment2;
            if (configurationNumber == 0 || configurationNumber == 3)
            {
                segment1 = new Segment2D(segment1UniquePoint, segmentCommonPoint);
            }
            else
            {
                segment1 = new Segment2D(segmentCommonPoint, segment1UniquePoint);
            }

            if (configurationNumber == 0 || configurationNumber == 1)
            {
                segment2 = new Segment2D(segment2UniquePoint, segmentCommonPoint);
            }
            else
            {
                segment2 = new Segment2D(segmentCommonPoint, segment2UniquePoint);
            }
            
            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Intersects, result.IntersectionType);
            CollectionAssert.AreEqual(new[]{segmentCommonPoint}, result.IntersectionPoints);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetIntersectionBetweenSegments_SegmentEndsOrStartsOnOtherSegment_ReturnIntersectionPoint(
            int configurationNumber)
        {
            // Setup
            const double y = 12.34;
            var segment1UniquePoint = new Point2D(15.6, y + 56.78);
            var pointOnSegment = new Point2D(1.0, y);

            var segment2Point1 = new Point2D(0.0, y);
            var segment2Point2 = new Point2D(111.0, y);

            Segment2D segment1, segment2;
            if (configurationNumber == 0 || configurationNumber == 3)
            {
                segment1 = new Segment2D(segment1UniquePoint, pointOnSegment);
            }
            else
            {
                segment1 = new Segment2D(pointOnSegment, segment1UniquePoint);
            }

            if (configurationNumber == 0 || configurationNumber == 1)
            {
                segment2 = new Segment2D(segment2Point1, segment2Point2);
            }
            else
            {
                segment2 = new Segment2D(segment2Point2, segment2Point1);
            }

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Intersects, result.IntersectionType);
            CollectionAssert.AreEqual(new[] { pointOnSegment }, result.IntersectionPoints,
                new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCase(10 - 1e-4)]
        [TestCase(3.45)]
        [TestCase(0)]
        [TestCase(-7.96)]
        [TestCase(-10 + 1e-4)]
        public void GetIntersectionBetweenSegments_VerticalCollinearSegmentsWithSomeOverlap_ReturnOverlap(
            double dy)
        {
            // Setup
            const double x = -12.56;
            const double y1 = 10.0;
            const double y2 = 0.0;
            double y3 = y1 + dy;
            double y4 = y2 + dy;

            var verticalSegment1 = new Segment2D(new Point2D(x, y1), new Point2D(x, y2));
            var verticalSegment2 = new Segment2D(new Point2D(x, y3), new Point2D(x, y4));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(verticalSegment1, verticalSegment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Overlaps, result.IntersectionType);
            var expectedOverlappingPoints = dy >= 0 ? 
                new[] { verticalSegment1.FirstPoint, verticalSegment2.SecondPoint }:
                new[] { verticalSegment1.SecondPoint, verticalSegment2.FirstPoint };
            CollectionAssertAreEquivalent(expectedOverlappingPoints, result.IntersectionPoints);
        }

        [Test]
        [TestCase(14 - 1e-4)]
        [TestCase(7.8)]
        [TestCase(0)]
        [TestCase(-2.34)]
        [TestCase(-14 + 1e-4)]
        public void GetIntersectionBetweenSegments_HorizontalCollinearSegmentsWithSomeOverlap_ReturnOverlap(
            double dx)
        {
            // Setup
            const double y = 98.54;
            const double x1 = 2.0;
            const double x2 = -12.0;
            double x3 = x1 + dx;
            double x4 = x2 + dx;

            var horizontalSegment1 = new Segment2D(new Point2D(x1, y), new Point2D(x2, y));
            var horizontalSegment2 = new Segment2D(new Point2D(x3, y), new Point2D(x4, y));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(horizontalSegment1, horizontalSegment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Overlaps, result.IntersectionType);
            var expectedOverlappingPoints = dx >= 0 ?
                new[] { horizontalSegment1.FirstPoint, horizontalSegment2.SecondPoint } :
                new[] { horizontalSegment1.SecondPoint, horizontalSegment2.FirstPoint };
            CollectionAssertAreEquivalent(expectedOverlappingPoints, result.IntersectionPoints);
        }

        [Test]
        public void GetIntersectionBetweenSegments_SelfIntersection_ReturnOverlap()
        {
            // Setup
            var firstPoint = new Point2D(1.1, 2.2);
            var secondPoint = new Point2D(-3.3, -4.4);
            var segment = new Segment2D(firstPoint, secondPoint);

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment, segment);

            // Assert
            Assert.AreEqual(Intersection2DType.Overlaps, result.IntersectionType);
            var expectedOverlappingPoints = new[] { firstPoint, secondPoint };
            CollectionAssertAreEquivalent(expectedOverlappingPoints, result.IntersectionPoints);
        }

        [Test]
        public void GetIntersectionBetweenSegments_CollinearSegmentsWithFullOverlap_ReturnOverlap()
        {
            // Setup
            Func<double, double> getY = x => -12.34 * x + 45.67;
            const double x1 = 1.1;
            const double x2 = 2.2;
            const double x3 = -3.3;
            const double x4 = 4.4;
            var segment1 = new Segment2D(new Point2D(x1, getY(x1)), new Point2D(x2, getY(x2)));
            var segment2 = new Segment2D(new Point2D(x3, getY(x3)), new Point2D(x4, getY(x4)));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Overlaps, result.IntersectionType);
            var expectedOverlappingPoints = new[] { segment1.FirstPoint, segment1.SecondPoint };
            CollectionAssertAreEquivalent(expectedOverlappingPoints, result.IntersectionPoints);
        }

        [Test]
        [TestCase(-12.34, -34.56, -67.78, -91.23)]
        [TestCase(12.34, 34.56, 67.78, 91.23)]
        [TestCase(1.0, 2.1, 3.4, 2.1)]
        [TestCase(3.4, 4.5, 1.0, 4.5)]
        [TestCase(1.0, 2.1, 1.0, 4.5)]
        [TestCase(3.4, 4.5, 3.4, 2.1)]
        [TestCase(1.5, 2.4, 2.2, 2.4)]
        [TestCase(3.0, 3.8, 2.9, 3.6)]
        public void GetIntersectionBetweenSegments_SegmentsNotIntersecting_ReturnNoIntersect(
            double x1, double y1, double x2, double y2)
        {
            // Setup
            var segment1 = new Segment2D(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4));
            var segment2 = new Segment2D(new Point2D(x1, y1), new Point2D(x2, y2));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
        }

        [Test]
        public void GetIntersectionBetweenSegments_SegmentsArePointsOnTopOfEachOther_ReturnIntersection(
            [Random(-9876.54321, 1234.56789, 1)]double x,
            [Random(-123456.789, 98765.4321, 1)]double y)
        {
            // Setup
            var segment1 = new Segment2D(new Point2D(x, y), new Point2D(x, y));
            var segment2 = new Segment2D(new Point2D(x, y), new Point2D(x, y));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Intersects, result.IntersectionType);
            CollectionAssert.AreEqual(new[]{ new Point2D(x, y) }, result.IntersectionPoints);
        }

        [Test]
        public void GetIntersectionBetweenSegments_SegmentsArePointsNotOnTopOfEachOther_ReturnNoIntersection()
        {
            // Setup
            const double x1 = 1.1;
            const double y1 = 2.2;
            const double x2 = 3.3;
            const double y2 = 4.4;
            var segment1 = new Segment2D(new Point2D(x1, y1), new Point2D(x1, y1));
            var segment2 = new Segment2D(new Point2D(x2, y2), new Point2D(x2, y2));

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetIntersectionBetweenSegments_SegmentWithPointSegmentOnIt_ReturnIntersection(
            bool firstSegmentIsPointDegenerate)
        {
            // Setup
            Func<double, double> getY = x => 1.2 * x + 3.4;

            const double x1 = 1.1;
            const double x2 = 5.5;
            var lineSegment = new Segment2D(new Point2D(x1, getY(x1)), new Point2D(x2, getY(x2)));
            const double x3 = 3.3;
            var point = new Point2D(x3, getY(x3));
            var pointSegment = new Segment2D(point, point);

            Segment2D segment1, segment2;
            if (firstSegmentIsPointDegenerate)
            {
                segment1 = pointSegment;
                segment2 = lineSegment;
            }
            else
            {
                segment1 = lineSegment;
                segment2 = pointSegment;
            }

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Intersects, result.IntersectionType);
            CollectionAssert.AreEqual(new[]{ point }, result.IntersectionPoints);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetIntersectionBetweenSegments_VerticalSegmentWithPointSegmentOnIt_ReturnIntersection(
            bool firstSegmentIsPointDegenerate)
        {
            // Setup
            const double x = 1.1;
            var lineSegment = new Segment2D(new Point2D(x, 1.0), new Point2D(x, 3.0));
            var point = new Point2D(x, 2.0);
            var pointSegment = new Segment2D(point, point);

            Segment2D segment1, segment2;
            if (firstSegmentIsPointDegenerate)
            {
                segment1 = pointSegment;
                segment2 = lineSegment;
            }
            else
            {
                segment1 = lineSegment;
                segment2 = pointSegment;
            }

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.Intersects, result.IntersectionType);
            CollectionAssert.AreEqual(new[] { point }, result.IntersectionPoints);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetIntersectionBetweenSegments_SegmentAndSeparatePointSegment_ReturnNoIntersection(
            bool firstSegmentIsPointDegenerate)
        {
            // Setup
            Func<double, double> getY = x => -5.6 * x + 7.8;

            const double x1 = 1.1;
            const double x2 = 5.5;
            var lineSegment = new Segment2D(new Point2D(x1, getY(x1)), new Point2D(x2, getY(x2)));
            var point = new Point2D(0.0, 0.0);
            var pointSegment = new Segment2D(point, point);

            Segment2D segment1, segment2;
            if (firstSegmentIsPointDegenerate)
            {
                segment1 = pointSegment;
                segment2 = lineSegment;
            }
            else
            {
                segment1 = lineSegment;
                segment2 = pointSegment;
            }

            // Call
            Segment2DIntersectSegment2DResult result = Math2D.GetIntersectionBetweenSegments(segment1, segment2);

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
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

        private static void CollectionAssertAreEquivalent(Point2D[] expected, Point2D[] actual)
        {
            var comparer = new Point2DComparerWithTolerance(1e-6);
            foreach (var intersectionPoint in actual)
            {
                Assert.AreEqual(1, expected.Count(p => comparer.Compare(p, intersectionPoint) == 0));
            }
        }

        private double[] GetLengthsBasedOnReletative(double[] relativeLengths, IEnumerable<Point2D> lineGeometryPoints)
        {
            var lineLength = Math2D.ConvertLinePointsToLineSegments(lineGeometryPoints).Sum(s => s.Length);
            return relativeLengths.Select(l => lineLength * l).ToArray();
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
    }
}