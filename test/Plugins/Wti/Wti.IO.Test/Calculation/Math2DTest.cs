using NUnit.Framework;
using Wti.IO.Calculation;

namespace Wti.IO.Test.Calculation
{
    public class Math2DTest
    {
        private static readonly double[][] IntersectingSegments =
        {
            // \/
            // /\
            new[]
            {
                0.0,
                0.0,
                1.0,
                1.0,
                1.0,
                0.0,
                0.0,
                1.0,
                0.5,
                0.5
            },
            // __
            //  /
            // /
            new[]
            {
                0.0,
                0.0,
                1.0,
                1.0,
                0.0,
                1.0,
                1.0,
                1.0,
                1.0,
                1.0
            },
            // 
            //  /
            // /__
            new[]
            {
                0.0,
                0.0,
                1.0,
                0.0,
                0.0,
                0.0,
                1.0,
                1.0,
                0.0,
                0.0
            }
        };

        private static readonly double[][] ParallelSegments =
        {
            // __
            // __
            new[]
            {
                0.0,
                0.0,
                1.0,
                0.0,
                0.0,
                1.0,
                1.0,
                1.0
            },
            // ____ (connected in single point)
            new[]
            {
                0.0,
                0.0,
                1.0,
                0.0,
                1.0,
                0.0,
                2.0,
                0.0
            },
            // __ (overlap)
            new[]
            {
                0.0,
                0.0,
                1.0,
                0.0,
                0.5,
                0.0,
                1.5,
                0.0
            }
        };

        private static readonly double[][] NonIntersectingSegments =
        {
            //  |
            // ___
            new[]
            {
                0.0,
                0.0,
                1.0,
                0.0,
                0.5,
                1.0,
                0.5,
                0.5
            }
        };

        [Test]
        [TestCaseSource("IntersectingSegments")]
        public void LineSegmentIntersectionWithLineSegment_DifferentLineSegmentsWithIntersections_ReturnsPoint(double[] coordinates)
        {
            // Setup
            var segments = ToSegmentCoordinatesCollections(coordinates);

            // Call
            var result = Math2D.LineSegmentIntersectionWithLineSegment(segments[0], segments[1]);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                coordinates[8],
                coordinates[9]
            }, result);
        }

        [Test]
        [TestCaseSource("ParallelSegments")]
        public void LineSegmentIntersectionWithLineSegment_DifferentParallelLineSegments_ReturnsNoPoint(double[] coordinates)
        {
            // Setup
            var segments = ToSegmentCoordinatesCollections(coordinates);

            // Call
            var result = Math2D.LineSegmentIntersectionWithLineSegment(segments[0], segments[1]);

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        [TestCaseSource("NonIntersectingSegments")]
        public void LineSegmentIntersectionWithLineSegment_DifferentLineSegmentsWithNoIntersection_ReturnsNoPoint(double[] coordinates)
        {
            // Setup
            var segments = ToSegmentCoordinatesCollections(coordinates);

            // Call
            var result = Math2D.LineSegmentIntersectionWithLineSegment(segments[0], segments[1]);

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        private double[][] ToSegmentCoordinatesCollections(double[] coordinates)
        {
            double[] segmentX =
            {
                coordinates[0],
                coordinates[2],
                coordinates[4],
                coordinates[6]
            };
            double[] segmentY =
            {
                coordinates[1],
                coordinates[3],
                coordinates[5],
                coordinates[7]
            };
            return new[]
            {
                segmentX,
                segmentY
            };
        }
    }
}