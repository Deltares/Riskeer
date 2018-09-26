// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Geometry.Test
{
    [TestFixture]
    public class AdvancedMath2DTest
    {
        [Test]
        public void PolygonIntersectionWithPolygon_NoIntersection_ReturnsEmptyCollection()
        {
            // Setup
            Point2D[] polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(5, 0),
                new Point2D(5, 4),
                new Point2D(9, 4),
                new Point2D(9, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB);

            // Assert
            CollectionAssert.IsEmpty(intersections);
        }

        [Test]
        public void PolygonIntersectionWithPolygon_WithSelfIntersectingPolygon_ThrowsInvalidPolygonException()
        {
            // Setup
            Point2D[] polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(4, 0),
                new Point2D(4, 4),
                new Point2D(6, 0),
                new Point2D(8, 4),
                new Point2D(8, 0)
            };

            // Call
            TestDelegate test = () => AdvancedMath2D.PolygonIntersectionWithPolygon(polyB, polyA);

            // Assert
            Assert.Throws<InvalidPolygonException>(test);
        }

        [Test]
        public void PolygonIntersectionWithPolygon_IntersectsComplete_ReturnsIntersectionEqualToPolygon()
        {
            // Setup
            Point2D[] polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 4),
                new Point2D(4, 4),
                new Point2D(4, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB);

            // Assert
            Assert.AreEqual(1, intersections.Count());
            Assert.AreEqual(polyA, intersections.ElementAt(0));
        }

        [Test]
        public void PolygonIntersectionWithPolygon_PartlyIntersects_ReturnsPartialIntersection()
        {
            // Setup
            Point2D[] polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 4),
                new Point2D(2, 4),
                new Point2D(2, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB);

            // Assert
            Assert.AreEqual(1, intersections.Count());
            CollectionAssert.AreEqual(polyB, intersections.ElementAt(0));
        }

        [Test]
        public void PolygonIntersectionWithPolygon_TouchesOnSide_ReturnsEmptyCollection()
        {
            // Setup
            Point2D[] polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(4, 0),
                new Point2D(4, 4),
                new Point2D(6, 4),
                new Point2D(6, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB);

            // Assert
            Assert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(4, 4),
                    new Point2D(4, 0)
                }
            }, intersections);
        }

        [Test]
        public void PolygonIntersectionWithPolygon_TouchesWithPointOnSide_ReturnsEmptyCollection()
        {
            // Setup
            Point2D[] polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(5, 0),
                new Point2D(4, 2),
                new Point2D(5, 4),
                new Point2D(6, 4),
                new Point2D(6, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB);

            // Assert
            Assert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(4, 2)
                }
            }, intersections);
        }

        [Test]
        public void PolygonIntersectionWithPolygon_PartiallyIntersectsTwice_ReturnsTwoIntersections()
        {
            // Setup
            Point2D[] polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(2, 0),
                new Point2D(2, 1),
                new Point2D(5, 1),
                new Point2D(5, 3),
                new Point2D(2, 3),
                new Point2D(2, 4),
                new Point2D(6, 4),
                new Point2D(6, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB);

            // Assert
            Assert.AreEqual(2, intersections.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(2, 4),
                new Point2D(4, 4),
                new Point2D(4, 3),
                new Point2D(2, 3)
            }, intersections.ElementAt(0));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(4, 1),
                new Point2D(4, 0),
                new Point2D(2, 0),
                new Point2D(2, 1)
            }, intersections.ElementAt(1));
        }

        [Test]
        public void PolygonIntersectionWithPolygon_IntersectsPolygonLineAndPoint_ReturnsTwoIntersections()
        {
            // Setup
            Point2D[] polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(0, -2),
                new Point2D(0, 5),
                new Point2D(0.5, 5),
                new Point2D(0.5, -1),
                new Point2D(1.0, 0),
                new Point2D(1.5, 0),
                new Point2D(2.0, -1),
                new Point2D(3.0, 0),
                new Point2D(4.0, -2)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB);

            // Assert
            Assert.AreEqual(3, intersections.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(3.0, 0.0)
            }, intersections.ElementAt(0));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1.5, 0.0),
                new Point2D(1.0, 0.0)
            }, intersections.ElementAt(1));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(0.0, 4.0),
                new Point2D(0.5, 4.0),
                new Point2D(0.5, 0.0)
            }, intersections.ElementAt(2));
        }

        [Test]
        public void FromXToXY_WithoutPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AdvancedMath2D.FromXToXY(null, new Point2D(0, 0), 3, 2);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(
                test,
                "Cannot transform to coordinates without a source.").ParamName;
            Assert.AreEqual("xCoordinates", paramName);
        }

        [Test]
        public void FromXToXY_WithoutReferencePoint_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AdvancedMath2D.FromXToXY(new double[0], null, 3, 2);

            // Assert
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(
                test,
                "Cannot transform to coordinates without a reference point.").ParamName;
            Assert.AreEqual("referencePoint", paramName);
        }

        [Test]
        public void FromXToXY_NoPoints_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<Point2D> points = AdvancedMath2D.FromXToXY(new double[0], new Point2D(0, 0), 3, 2);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void FromXToXY_WithoutTransformations_ReturnsCoordinatesOnYAxis()
        {
            // Setup
            double[] xCoordinates = ThreeRandomXCoordinates();
            var referencePoint = new Point2D(0, 0);

            // Call
            IEnumerable<Point2D> points = AdvancedMath2D.FromXToXY(xCoordinates, referencePoint, 0, 0);

            // Assert
            CollectionElementsAlmostEquals(xCoordinates.Select(x => new Point2D(0, x)), points);
        }

        [Test]
        public void FromXToXY_WithOffset_ReturnsCoordinatesNearerToOrigin()
        {
            // Setup
            double[] xCoordinates = ThreeRandomXCoordinates();
            var referencePoint = new Point2D(0, 0);
            int offset = new Random(21).Next();

            // Call
            IEnumerable<Point2D> points = AdvancedMath2D.FromXToXY(xCoordinates, referencePoint, offset, 0);

            // Assert
            CollectionElementsAlmostEquals(xCoordinates.Select(x => new Point2D(0, x - offset)), points);
        }

        [Test]
        public void FromXToXY_WithRotation180_ReturnsCoordinatesOnNegativeYAxis()
        {
            // Setup
            double[] xCoordinates = ThreeRandomXCoordinates();
            var referencePoint = new Point2D(0, 0);

            // Call
            IEnumerable<Point2D> points = AdvancedMath2D.FromXToXY(xCoordinates, referencePoint, 0, 180);

            // Assert
            CollectionElementsAlmostEquals(xCoordinates.Select(x => new Point2D(0, -x)), points);
        }

        [Test]
        public void FromXToXY_WithReferencePoint_ReturnsCoordinatesFromReferencePoint()
        {
            // Setup
            var random = new Random(21);
            double[] xCoordinates = ThreeRandomXCoordinates();
            var referencePoint = new Point2D(random.NextDouble(), random.NextDouble());

            // Call
            IEnumerable<Point2D> points = AdvancedMath2D.FromXToXY(xCoordinates, referencePoint, 0, 0);

            // Assert
            CollectionElementsAlmostEquals(xCoordinates.Select(x => new Point2D(referencePoint.X, referencePoint.Y + x)), points);
        }

        [Test]
        public void FromXToXY_WithReferencePointOffsetAndRotation_ReturnsCoordinatesFromReferencePoint()
        {
            // Setup
            const double center = 5.0;
            double[] xCoordinates =
            {
                center - Math.Sqrt(8),
                center,
                center + Math.Sqrt(2)
            };
            var referencePoint = new Point2D(3, 4);
            const double offset = 5;
            const double rotation = 45;

            // Call
            IEnumerable<Point2D> points = AdvancedMath2D.FromXToXY(xCoordinates, referencePoint, offset, rotation);

            // Assert
            CollectionElementsAlmostEquals(new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4),
                new Point2D(4, 5)
            }, points);
        }

        [Test]
        public void CompleteLineToPolygon_WithoutLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AdvancedMath2D.CompleteLineToPolygon(null, double.NaN).ToArray();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("line", exception.ParamName);
        }

        [Test]
        public void CompleteLineToPolygon_LineWithLessThanTwoPoints_ThrowsArgumentNullException([Range(0, 1)] int pointCount)
        {
            // Setup
            IEnumerable<Point2D> points = Enumerable.Repeat(new Point2D(3, 2), pointCount);

            // Call
            TestDelegate test = () => AdvancedMath2D.CompleteLineToPolygon(points, double.NaN).ToArray();

            // Assert
            const string message = "The line needs to have at least two points to be able to create a complete polygon.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
            Assert.AreEqual("line", exception.ParamName);
        }

        [Test]
        [TestCase(3)]
        [TestCase(0)]
        [TestCase(-9)]
        [TestCase(double.NaN)]
        public void CompleteLineToPolygon_LineWithTwoPoints_TwoPointsAtBottomLevelAdded(double completingPointsLevel)
        {
            // Setup
            var random = new Random(21);
            int firstPointX = random.Next();
            int lastPointX = random.Next();
            var points = new[]
            {
                new Point2D(firstPointX, random.Next()),
                new Point2D(random.Next(), random.Next()),
                new Point2D(lastPointX, random.Next())
            };

            // Call
            IEnumerable<Point2D> pointsOfPolygon = AdvancedMath2D.CompleteLineToPolygon(points, completingPointsLevel);

            // Assert
            Assert.AreEqual(points, pointsOfPolygon.Take(3));
            Assert.AreEqual(new Point2D(lastPointX, completingPointsLevel), pointsOfPolygon.ElementAt(3));
            Assert.AreEqual(new Point2D(firstPointX, completingPointsLevel), pointsOfPolygon.ElementAt(4));
        }

        private static double[] ThreeRandomXCoordinates()
        {
            var random = new Random(21);
            return new[]
            {
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble()
            };
        }

        private static void CollectionElementsAlmostEquals(IEnumerable<Point2D> expected, IEnumerable<Point2D> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());

            for (var index = 0; index < expected.Count(); index++)
            {
                Point2D actualPoint = actual.ElementAt(index);
                Point2D expectedPoint = expected.ElementAt(index);

                const double delta = 1e-8;
                Assert.AreEqual(expectedPoint.X, actualPoint.X, delta);
                Assert.AreEqual(expectedPoint.Y, actualPoint.Y, delta);
            }
        }

        private static Point2D[] CreateBasePolygon()
        {
            return new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 4),
                new Point2D(4, 4),
                new Point2D(4, 0)
            };
        }
    }
}