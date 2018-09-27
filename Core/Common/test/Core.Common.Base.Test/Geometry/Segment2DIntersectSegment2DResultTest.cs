// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Segment2DIntersectSegment2DResultTest
    {
        [Test]
        public void CreateNoIntersectResult_ExpectedValues()
        {
            // Call
            Segment2DIntersectSegment2DResult result = Segment2DIntersectSegment2DResult.CreateNoIntersectResult();

            // Assert
            Assert.AreEqual(Intersection2DType.DoesNotIntersect, result.IntersectionType);
            CollectionAssert.IsEmpty(result.IntersectionPoints);
        }

        [Test]
        public void CreateIntersectionResult_ExpectedValues()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            Segment2DIntersectSegment2DResult result = Segment2DIntersectSegment2DResult.CreateIntersectionResult(point);

            // Assert
            Assert.AreEqual(Intersection2DType.Intersects, result.IntersectionType);
            CollectionAssert.AreEqual(new[]
            {
                point
            }, result.IntersectionPoints);
        }

        [Test]
        public void CreateOverlapResult_ExpectedValues()
        {
            // Setup
            var point1 = new Point2D(1.1, 2.2);
            var point2 = new Point2D(1.1, 2.2);

            // Call
            Segment2DIntersectSegment2DResult result = Segment2DIntersectSegment2DResult.CreateOverlapResult(point1, point2);

            // Assert
            Assert.AreEqual(Intersection2DType.Overlaps, result.IntersectionType);
            CollectionAssert.AreEqual(new[]
            {
                point1,
                point2
            }, result.IntersectionPoints);
        }
    }
}