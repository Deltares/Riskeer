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
            Assert.AreEqual(Intersection2DType.NoIntersections, result.IntersectionType);
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
            CollectionAssert.AreEqual(new[]{point}, result.IntersectionPoints);
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
            Assert.AreEqual(Intersection2DType.Overlapping, result.IntersectionType);
            CollectionAssert.AreEqual(new[] { point1, point2 }, result.IntersectionPoints);
        }
    }
}