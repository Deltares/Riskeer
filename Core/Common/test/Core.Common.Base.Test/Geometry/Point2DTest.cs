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

using System;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using NUnit.Framework;

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Point2DTest
    {
        [Test]
        public void Constructor_WithXandY_SetPropeties()
        {
            // Setup
            var random = new Random(22);
            var x = random.NextDouble();
            var y = random.NextDouble();

            // Call
            var point = new Point2D(x, y);

            // Assert
            Assert.AreEqual(x, point.X);
            Assert.AreEqual(y, point.Y);
        }

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            var point = new Point2D(0, 0);

            // Call
            var result = point.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ToOtherType_ReturnsFalse()
        {
            // Setup
            var point = new Point2D(0, 0);

            // Call
            var result = point.Equals(new Point3D(0, 0, 0));

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var point = new Point2D(0, 0);

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
            var point = new Point2D(x, y);
            var otherPoint = new Point2D(x, y);

            // Call
            var result = point.Equals(otherPoint);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(1e-6, 0)]
        [TestCase(0, 1e-6)]
        public void Equals_CloseToOtherPoint_ReturnsFalse(double deltaX, double deltaY)
        {
            // Setup
            var random = new Random(22);
            var x = random.NextDouble();
            var y = random.NextDouble();

            var point = new Point2D(x, y);
            var otherPoint = new Point2D(x + deltaX, y + deltaY);

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

            var point = new Point2D(x, y);
            var otherPoint = new Point2D(x, y);

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
        public void SubstractOperator_FirstArgumentNull_ThrowArgumentNullException()
        {
            // Setup
            Point2D first = null;
            Point2D second = new Point2D(0, 0);

            // Call
            TestDelegate call = () => { Vector<double> result = first - second; };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("p1", paramName);
        }

        [Test]
        public void SubstractOperator_SecondArgumentNull_ThrowArgumentNullException()
        {
            // Setup
            Point2D first = new Point2D(0, 0);
            Point2D second = null;

            // Call
            TestDelegate call = () => { Vector<double> result = first - second; };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("p2", paramName);
        }

        [Test]
        public void SubstractOperator_TwoDifferentPoints_Return2DVector()
        {
            // Setup
            var point1 = new Point2D(3.0, 4.0);
            var point2 = new Point2D(1.0, 1.0);

            // Call
            Vector<double> vector = point1 - point2;

            // Assert
            Assert.AreEqual(2, vector.Count);
            Assert.AreEqual(point1.X - point2.X, vector[0]);
            Assert.AreEqual(point1.Y - point2.Y, vector[1]);
        }

        [Test]
        public void AddOperator_PointNull_ThrowArgumentNullException()
        {
            // Setup
            Point2D point = null;
            Vector<double> vector = new DenseVector(2);

            // Call
            TestDelegate call = () => { Point2D result = point + vector; };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("point", paramName);
        }

        [Test]
        public void AddOperator_VectorNull_ThrowArgumentNullException()
        {
            // Setup
            Point2D point = new Point2D(0, 0);
            Vector<double> vector = null;

            // Call
            TestDelegate call = () => { Point2D result = point + vector; };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("vector", paramName);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(-6541.2354, 5.25)]
        [TestCase(-3.25, -12.55)]
        [TestCase(6.154, -9684.514)]
        [TestCase(6840.251, 15.3251)]
        public void AddOperator_PointWithZeroVector_ReturnEqualPoint(double x, double y)
        {
            // Setup
            var originalPoint = new Point2D(x, y);
            var zeroVector = new DenseVector(new[]
            {
                0.0,
                0.0
            });

            // Call
            Point2D resultPoint = originalPoint + zeroVector;

            // Assert
            Assert.AreNotSame(originalPoint, resultPoint);
            Assert.AreEqual(x, resultPoint.X);
            Assert.AreEqual(y, resultPoint.Y);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(-6541.2354, 5.25)]
        [TestCase(-3.25, -12.55)]
        [TestCase(6.154, -9684.514)]
        [TestCase(6840.251, 15.3251)]
        public void AddOperator_PointWithVector_ReturnEqualPoint(double x, double y)
        {
            // Setup
            var originalPoint = new Point2D(x, y);
            const double dx = 1.1;
            const double dy = -2.2;
            var vector = new DenseVector(new[]
            {
                dx,
                dy
            });

            // Call
            Point2D resultPoint = originalPoint + vector;

            // Assert
            Assert.AreEqual(x + dx, resultPoint.X);
            Assert.AreEqual(y + dy, resultPoint.Y);
        }

        [Test]
        public void AddOperator_PointWithInvalidVector_ThrowArgumentException()
        {
            // Setup
            var originalPoint = new Point2D(0.0, 0.0);
            var vector3D = new DenseVector(new[]
            {
                1.1,
                2.2,
                3.3
            });

            // Call
            TestDelegate call = () => { Point2D result = originalPoint + vector3D; };

            // Assert
            const string expectedMessage = "Vector moet 2 dimensies hebben, maar heeft er 3.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void GetEuclideanDistanceTo_Itself_ReturnZero()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            double euclideanDistance = point.GetEuclideanDistanceTo(point);

            // Assert
            Assert.AreEqual(0, euclideanDistance);
        }

        [Test]
        public void GetEuclideanDistanceTo_Null_ThrowArgumentNullException()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            TestDelegate call = () => point.GetEuclideanDistanceTo(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void GetEuclideanDistanceTo_DifferentPoint_ReturnEuclideanDistance()
        {
            // Setup
            var point = new Point2D(1.2, 3.5);
            var point2 = new Point2D(8.13, 21.34);

            // Call
            double euclideanDistance1 = point.GetEuclideanDistanceTo(point2);
            double euclideanDistance2 = point2.GetEuclideanDistanceTo(point);

            // Assert
            var expectedResult = Math.Sqrt(Math.Pow(point.X - point2.X, 2) +
                                           Math.Pow(point.Y - point2.Y, 2));
            Assert.AreEqual(expectedResult, euclideanDistance1);
            Assert.AreEqual(euclideanDistance2, euclideanDistance1);
        }

        private static void DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            var stringRepresentation = point.ToString();

            // Assert
            var expectedText = string.Format("({0}, {1})", point.X, point.Y);
            Assert.AreEqual(expectedText, stringRepresentation);
        }
    }
}