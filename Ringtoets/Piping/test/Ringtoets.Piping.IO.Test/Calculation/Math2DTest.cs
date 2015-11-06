using System;
using System.Collections;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Calculation;

namespace Ringtoets.Piping.IO.Test.Calculation
{
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
    }
}