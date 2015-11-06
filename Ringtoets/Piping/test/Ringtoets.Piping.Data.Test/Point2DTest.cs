using System;

using MathNet.Numerics.LinearAlgebra.Double;

using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class Point2DTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var point = new Point2D();

            // Assert
            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);
        }

        [Test]
        public void Constructor_WithXandY_SetPropeties()
        {
            // Setup
            var random = new Random(22);
            var x = random.NextDouble();
            var y = random.NextDouble();

            // Call
            var point = new Point2D(x,y);

            // Assert
            Assert.AreEqual(x, point.X);
            Assert.AreEqual(y, point.Y);
        }
        [Test]
        public void AutomaticProperties_SetAndGetValuesAgain_ReturnedValueShouldBeSameAsSetValue()
        {
            // Setup
            var point = new Point2D();

            // Call
            point.X = 1.1;
            point.Y = 2.2;

            // Assert
            Assert.AreEqual(1.1, point.X);
            Assert.AreEqual(2.2, point.Y);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var point = new Point2D();

            // Call
            var result = point.Equals(point);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 2)]
        public void Equals_OtherWithSameCoordinates_ReturnsTrue(double x, double y)
        {
            // Setup
            var point = new Point2D
            {
                X = x,
                Y = y,
            };
            var otherPoint = new Point2D
            {
                X = x,
                Y = y,
            };

            // Call
            var result = point.Equals(otherPoint);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(1e-8, 0)]
        [TestCase(0, 1e-8)]
        public void Equals_CloseToOtherPoint_ReturnsFalse(double deltaX, double deltaY)
        {
            // Setup
            var random = new Random(22);
            var x = random.NextDouble();
            var y = random.NextDouble();

            var point = new Point2D
            {
                X = x,
                Y = y,
            };
            var otherPoint = new Point2D
            {
                X = x + deltaX,
                Y = y + deltaY,
            };

            // Call
            var result = point.Equals(otherPoint);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void GetHashCode_PointsAreEqual_PointsHashesEqual()
        {
            // Setup
            var random = new Random(22);
            var x = random.NextDouble();
            var y = random.NextDouble();

            var point = new Point2D
            {
                X = x,
                Y = y,
            };
            var otherPoint = new Point2D
            {
                X = x,
                Y = y,
            };

            // Call
            var result = point.GetHashCode();
            var otherResult = otherPoint.GetHashCode();

            // Assert
            Assert.AreEqual(result, otherResult);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ToString_HasCoordinatValues_NL_PrintCoordinateValuesInLocalCulture()
        {
            DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture();
        }

        [Test]
        [SetCulture("en-US")]
        public void ToString_HasCoordinatValues_EN_PrintCoordinateValuesInLocalCulture()
        {
            DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture();
        }

        [Test]
        public void SubstractOperation_TwoDifferentPoints_Return2DVector()
        {
            // Setup
            var point1 = new Point2D
            {
                X = 3.0,
                Y = 4.0
            };
            var point2 = new Point2D
            {
                X = 1.0,
                Y = 1.0
            };

            // Call
            Vector vector = point1 - point2;

            // Assert
            Assert.AreEqual(2, vector.Count);
            Assert.AreEqual(point1.X - point2.X, vector[0]);
            Assert.AreEqual(point1.Y - point2.Y, vector[1]);
        }

        private static void DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture()
        {
            // Setup
            var point = new Point2D
            {
                X = 1.1,
                Y = 2.2,
            };

            // Call
            var stringRepresentation = point.ToString();

            // Assert
            var expectedText = String.Format("({0}, {1})", point.X, point.Y);
            Assert.AreEqual(expectedText, stringRepresentation);
        }
    }
}