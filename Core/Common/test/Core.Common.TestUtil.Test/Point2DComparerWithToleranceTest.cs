using Core.Common.Base.Geometry;

using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class Point2DComparerWithToleranceTest
    {
        [Test]
        public void Compare_SameInstance_ReturnZero()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            int result = new Point2DComparerWithTolerance(0).Compare(point, point);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_EqualInstances_ReturnZero()
        {
            // Setup
            const double x = 1.1;
            const double y = 2.2;
            var point1 = new Point2D(x, y);
            var point2 = new Point2D(x, y);

            // Call
            int result = new Point2DComparerWithTolerance(0).Compare(point1, point2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        [TestCase(1.1)]
        [TestCase(7.8)]
        public void Compare_DistanceBetweenPointsWithinTolerance_ReturnZero(double tolerance)
        {
            // Setup
            var point1 = new Point2D(1.1, 2.2);
            var point2 = new Point2D(1.1, 3.3);

            // Call
            int result = new Point2DComparerWithTolerance(tolerance).Compare(point1, point2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_DistanceBetweenPointsExceedsTolerance_ReturnOne()
        {
            // Setup
            var point1 = new Point2D(1.1, 2.2);
            var point2 = new Point2D(1.1, 3.3);

            // Call
            int result = new Point2DComparerWithTolerance(1.1-1e-6).Compare(point1, point2);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}