using System;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
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
        private static readonly double[][] IntersectingSegments =
        {
            // \/
            // /\
            new[]
            {
                0.0,0.0,
                1.0,1.0,
                1.0,0.0,
                0.0,1.0,
                0.5,0.5
            },
            // __
            //  /
            // /
            new[]
            {
                0.0,0.0,
                1.0,1.0,
                0.0,1.0,
                1.0,1.0,
                1.0,1.0
            },
            // 
            //  /
            // /__
            new[]
            {
                0.0,0.0,
                1.0,0.0,
                0.0,0.0,
                1.0,1.0,
                0.0,0.0
            }
        };

        /// <summary>
        /// Test cases for parallel segments. The <see cref="Array"/> contains pairs of <see cref="double"/>,
        /// which represent the coordinate of a point. Each pair of coordinates form a segment.
        /// </summary>
        private static readonly double[][] ParallelSegments =
        {
            // __
            // __
            new[]
            {
                0.0,0.0,
                1.0,0.0,
                0.0,1.0,
                1.0,1.0
            },
            // ____ (connected in single point)
            new[]
            {
                0.0,0.0,
                1.0,0.0,
                1.0,0.0,
                2.0,0.0
            },
            // __ (overlap)
            new[]
            {
                0.0,0.0,
                1.0,0.0,
                0.5,0.0,
                1.5,0.0
            }
        };

        /// <summary>
        /// Test cases for non intersecting segments. The <see cref="Array"/> contains pairs of <see cref="double"/>,
        /// which represent the coordinate of a point. Each pair of coordinates form a segment.
        /// </summary>
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

        #endregion

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

        [Test]
        [TestCase(3,2)]
        [TestCase(2,3)]
        [TestCase(3,3)]
        [TestCase(1,1)]
        [TestCase(1,2)]
        [TestCase(2,1)]
        public void LineSegmentIntersectionWithLineSegment_IncorrectLengthOfParameters_ThrowsArgumentException(int xLength, int yLength)
        {
            // Setup
            Collection<double> xCollection = new Collection<double>();
            Collection<double> yCollection = new Collection<double>();
            var random = new Random(22);
            for (var i = 0; i < xLength; i++)
            {
                xCollection.Add(random.NextDouble());
            }
            for (var i = 0; i < yLength; i++)
            {
                yCollection.Add(random.NextDouble());
            }

            // Call
            TestDelegate test = () => Math2D.LineSegmentIntersectionWithLineSegment(xCollection.ToArray(), yCollection.ToArray());
            
            // Assert
            var message = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(message.Message, "Collecties van de x en y coordinaten van lijnen vereisen een lengte van 4.");
        }

        [Test]
        [TestCase(3, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 3)]
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        public void LineSegmentIntersectionWithLine_IncorrectLengthOfParameters_ThrowsArgumentException(int xLength, int yLength)
        {
            // Setup
            Collection<double> xCollection = new Collection<double>();
            Collection<double> yCollection = new Collection<double>();
            var random = new Random(22);
            for (var i = 0; i < xLength; i++)
            {
                xCollection.Add(random.NextDouble());
            }
            for (var i = 0; i < yLength; i++)
            {
                yCollection.Add(random.NextDouble());
            }

            // Call
            TestDelegate test = () => Math2D.LineSegmentIntersectionWithLine(xCollection.ToArray(), yCollection.ToArray());

            // Assert
            var message = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(message.Message, "Collecties van de x en y coordinaten van lijnen vereisen een lengte van 4.");
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