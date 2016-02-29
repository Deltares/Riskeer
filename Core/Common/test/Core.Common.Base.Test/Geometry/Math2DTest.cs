using System;
using System.Collections;
using System.Linq;

using Core.Common.Base.Geometry;

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
                new Point2D(0.0,0.0),
                new Point2D(1.0,1.0),
                new Point2D(1.0,0.0),
                new Point2D(0.0,1.0),
                new Point2D(0.5,0.5)
            }, "IntersectingSegments 1");
            yield return testCaseDatadata1; 

            // __
            //  /
            // /
            var testCaseDatadata2 = new TestCaseData(new[]
            {
                new Point2D(0.0,0.0),
                new Point2D(1.0,1.0),
                new Point2D(0.0,1.0),
                new Point2D(1.0,1.0),
                new Point2D(1.0,1.0)
            }, "IntersectingSegments 2");
            yield return testCaseDatadata2;

            // 
            //  /
            // /__
            var testCaseDatadata3 = new TestCaseData(new[]
            {
                new Point2D(0.0,0.0),
                new Point2D(1.0,0.0),
                new Point2D(0.0,0.0),
                new Point2D(1.0,1.0),
                new Point2D(0.0,0.0)
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
                new Point2D(0.0,0.0),
                new Point2D(1.0,0.0),
                new Point2D(0.5,1.0),
                new Point2D(0.5,0.5),
                new Point2D(0.5,0.0)
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
        public void LineIntersectionWithLine_DifferentParallelLineSegments_ReturnsNoPoint(Point2D[] points, string testname="")
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
            TestDelegate testA = () => Math2D.LineIntersectionWithLine(new Point2D(0,0), new Point2D(0,0), new Point2D(1,0), new Point2D(0,1));
            TestDelegate testB = () => Math2D.LineIntersectionWithLine(new Point2D(0, 1), new Point2D(0, 0), new Point2D(1, 1), new Point2D(1, 1));

            // Assert
            var exceptionA = Assert.Throws<ArgumentException>(testA);
            var exceptionB = Assert.Throws<ArgumentException>(testB);
            Assert.AreEqual(Properties.Resources.Math2D_LineIntersectionWithLine_Line_points_are_equal, exceptionA.Message);
            Assert.AreEqual(Properties.Resources.Math2D_LineIntersectionWithLine_Line_points_are_equal,exceptionB.Message);
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
            Assert.AreEqual((y1+y2)/2, result.Y, 1e-8);
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
                new Segment2D(new Point2D(2.2,3.3), new Point2D(3.3,3.3)),
                new Segment2D(new Point2D(1.1,5.0), new Point2D(1.1,2.0)), // vertical
                new Segment2D(new Point2D(5.5,2.0), new Point2D(5.5,2.0)), // no length
                new Segment2D(new Point2D(-2.0,1.0), new Point2D(-1.0,2.0)),
                new Segment2D(new Point2D(-1.0,2.0), new Point2D(-2.0,5.5))
            };

            // Call
            var result = Math2D.SegmentsIntersectionWithVerticalLine(segments, x);

            // Assert
            Assert.AreEqual(intersectionHeights.Select(y => new Point2D(x, y)), result);
        }
    }
}