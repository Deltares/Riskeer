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

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.Base.TestUtil.Geometry;
using Core.Common.Data.TestUtil;
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
        public void Constructor_WithXAndY_SetProperties()
        {
            // Setup
            var random = new Random(22);
            double x = random.NextDouble();
            double y = random.NextDouble();

            // Call
            var point = new Point2D(x, y);

            // Assert
            Assert.IsInstanceOf<ICloneable>(point);

            Assert.AreEqual(x, point.X);
            Assert.AreEqual(y, point.Y);
        }

        [Test]
        public void CopyConstructor_PointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new Point2D(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("point", paramName);
        }

        [Test]
        public void CopyConstructor_WithPointWithXAndY_SetProperties()
        {
            // Setup
            var random = new Random(22);
            double x = random.NextDouble();
            double y = random.NextDouble();
            var pointToCopy = new Point2D(x, y);

            // Call
            var point = new Point2D(pointToCopy);

            // Assert
            Assert.AreEqual(x, point.X);
            Assert.AreEqual(y, point.Y);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ToString_HasCoordinateValues_NL_PrintCoordinateValuesInLocalCulture()
        {
            DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture();
        }

        [Test]
        [SetCulture("en-US")]
        public void ToString_HasCoordinateValues_EN_PrintCoordinateValuesInLocalCulture()
        {
            DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture();
        }

        [Test]
        public void SubtractOperator_FirstArgumentNull_ThrowArgumentNullException()
        {
            // Setup
            Point2D first = null;
            var second = new Point2D(0, 0);

            // Call
            TestDelegate call = () =>
            {
                Vector<double> result = first - second;
            };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("p1", paramName);
        }

        [Test]
        public void SubtractOperator_SecondArgumentNull_ThrowArgumentNullException()
        {
            // Setup
            var first = new Point2D(0, 0);
            Point2D second = null;

            // Call
            TestDelegate call = () =>
            {
                Vector<double> result = first - second;
            };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("p2", paramName);
        }

        [Test]
        public void SubtractOperator_TwoDifferentPoints_Return2DVector()
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
            TestDelegate call = () =>
            {
                Point2D result = point + vector;
            };

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("point", paramName);
        }

        [Test]
        public void AddOperator_VectorNull_ThrowArgumentNullException()
        {
            // Setup
            var point = new Point2D(0, 0);
            Vector<double> vector = null;

            // Call
            TestDelegate call = () =>
            {
                Point2D result = point + vector;
            };

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
            TestDelegate call = () =>
            {
                Point2D result = originalPoint + vector3D;
            };

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
            double expectedResult = Math.Sqrt(Math.Pow(point.X - point2.X, 2) +
                                              Math.Pow(point.Y - point2.Y, 2));
            Assert.AreEqual(expectedResult, euclideanDistance1);
            Assert.AreEqual(euclideanDistance2, euclideanDistance1);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(22);
            double x = random.NextDouble();
            double y = random.NextDouble();
            var original = new Point2D(x, y);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, GeometryCloneAssert.AreClones);
        }

        private static void DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            string stringRepresentation = point.ToString();

            // Assert
            string expectedText = $"({point.X}, {point.Y})";
            Assert.AreEqual(expectedText, stringRepresentation);
        }

        [TestFixture]
        private class Point2DEqualsTest : EqualsTestFixture<Point2D>
        {
            protected override Point2D CreateObject()
            {
                return CreatePoint2D();
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                Point2D basePoint = CreatePoint2D();
                const double offset = 1e-6;

                yield return new TestCaseData(new Point2D(basePoint.X + offset, basePoint.Y)).SetName("X");
                yield return new TestCaseData(new Point2D(basePoint.X, basePoint.Y + offset)).SetName("Y");
            }

            private static Point2D CreatePoint2D()
            {
                var random = new Random(21);
                return new Point2D(random.NextDouble(),
                                   random.NextDouble());
            }
        }
    }
}