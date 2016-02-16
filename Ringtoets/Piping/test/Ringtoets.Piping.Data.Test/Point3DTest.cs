using System;

using Core.Common.Base.Geometry;

using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class Point3DTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var point = new Point3D();

            // Assert
            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);
            Assert.AreEqual(0, point.Z);
        }

        [Test]
        public void AutomaticProperties_SetAndGetValuesAgain_ReturnedValueShouldBeSameAsSetValue()
        {
            // Setup
            var point = new Point3D();

            // Call
            point.X = 1.1;
            point.Y = 2.2;
            point.Z = -1.1;

            // Assert
            Assert.AreEqual(1.1, point.X);
            Assert.AreEqual(2.2, point.Y);
            Assert.AreEqual(-1.1, point.Z);
        }

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            var point = new Point3D();

            // Call
            var result = point.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ToOtherType_ReturnsFalse()
        {
            // Setup
            var point = new Point3D();

            // Call
            var result = point.Equals(new Point2D());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var point = new Point3D();

            // Call
            var result = point.Equals(point);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(0,0,0)]
        [TestCase(1,2,3)]
        [TestCase(3.5,3.6,3.7)]
        public void Equals_OtherWithSameCoordinates_ReturnsTrue(double x, double y, double z)
        {
            // Setup
            var point = new Point3D
            {
                X = x,
                Y = y,
                Z = z
            };
            var otherPoint = new Point3D
            {
                X = x,
                Y = y,
                Z = z
            };

            // Call
            var result = point.Equals(otherPoint);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(1e-8, 0, 0)]
        [TestCase(0, 1e-8, 0)]
        [TestCase(0, 0, 1e-8)]
        public void Equals_CloseToOtherPoint_ReturnsFalse(double deltaX, double deltaY, double deltaZ)
        {
            // Setup
            var random = new Random(22);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            
            var point = new Point3D
            {
                X = x,
                Y = y,
                Z = z
            };
            var otherPoint = new Point3D
            {
                X = x + deltaX,
                Y = y + deltaY,
                Z = z + deltaZ
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
            var z = random.NextDouble();

            var point = new Point3D
            {
                X = x,
                Y = y,
                Z = z
            };
            var otherPoint = new Point3D
            {
                X = x,
                Y = y,
                Z = z
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

        private static void DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture()
        {
            // Setup
            var point = new Point3D
            {
                X = 1.1, Y = 2.2, Z = 3.3
            };

            // Call
            var stringRepresentation = point.ToString();

            // Assert
            var expectedText = String.Format("({0}, {1}, {2})", point.X, point.Y, point.Z);
            Assert.AreEqual(expectedText, stringRepresentation);
        }
    }
}