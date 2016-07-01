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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
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
            var polyA = CreateBasePolygon();

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
            Assert.IsEmpty(intersections);
        }

        [Test]
        public void PolygonIntersectionWithPolygon_WithSelfIntersectingPolygon_ThrowsInvalidPolygonException()
        {
            // Setup
            var polyA = CreateBasePolygon();

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
            var polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 4),
                new Point2D(4, 4),
                new Point2D(4, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB).ToArray();

            // Assert
            Assert.AreEqual(1, intersections.Count());
            Assert.AreEqual(polyA, intersections.ElementAt(0));
        }

        [Test]
        public void PolygonIntersectionWithPolygon_PartlyIntersects_ReturnsPartialIntersection()
        {
            // Setup
            var polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 4),
                new Point2D(2, 4),
                new Point2D(2, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB).ToArray();

            // Assert
            Assert.AreEqual(1, intersections.Count());
            CollectionAssert.AreEqual(polyB, intersections.ElementAt(0));
        }

        [Test]
        public void PolygonIntersectionWithPolygon_TouchesOnSide_ReturnsEmptyCollection()
        {
            // Setup
            var polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(4, 0),
                new Point2D(4, 4),
                new Point2D(6, 4),
                new Point2D(6, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB).ToArray();

            // Assert
            Assert.IsEmpty(intersections);
        }

        [Test]
        public void PolygonIntersectionWithPolygon_TouchesWithPointOnSide_ReturnsEmptyCollection()
        {
            // Setup
            var polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(5, 0),
                new Point2D(4, 2),
                new Point2D(5, 4),
                new Point2D(6, 4),
                new Point2D(6, 0)
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB).ToArray();

            // Assert
            Assert.IsEmpty(intersections);
        }

        [Test]
        public void PolygonIntersectionWithPolygon_PartiallyIntersectsTwice_ReturnsTwoIntersections()
        {
            // Setup
            var polyA = CreateBasePolygon();

            var polyB = new[]
            {
                new Point2D(2, 0),
                new Point2D(2, 1),
                new Point2D(5, 1),
                new Point2D(5, 3),
                new Point2D(2, 3),
                new Point2D(2, 4),
                new Point2D(6, 4),
                new Point2D(6, 0),
            };

            // Call
            IEnumerable<IEnumerable<Point2D>> intersections = AdvancedMath2D.PolygonIntersectionWithPolygon(polyA, polyB).ToArray();

            // Assert
            Assert.AreEqual(2, intersections.Count());
            CollectionAssert.AreEqual(new []
            {
                new Point2D(2,4),
                new Point2D(4,4),
                new Point2D(4,3),
                new Point2D(2,3)
            }, intersections.ElementAt(0));
            CollectionAssert.AreEqual(new []
            {
                new Point2D(4,1),
                new Point2D(4,0),
                new Point2D(2,0),
                new Point2D(2,1)
            }, intersections.ElementAt(1));
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