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
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Projections;
using DotSpatial.Topology;
using NUnit.Framework;
using Point = System.Drawing.Point;

namespace Core.Components.DotSpatial.Test.Projections
{
    [TestFixture]
    public class WorldFileTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var worldFile = new WorldFile(1.1, 2.2, 3.3, 4.4, 5.5, 6.6);

            // Assert
            Assert.AreEqual(1.1, worldFile.A11);
            Assert.AreEqual(2.2, worldFile.A21);
            Assert.AreEqual(3.3, worldFile.A12);
            Assert.AreEqual(4.4, worldFile.A22);

            Assert.AreEqual(5.5, worldFile.B1);
            Assert.AreEqual(6.6, worldFile.B2);
        }

        [Test]
        public void Constructor_NonInvertableTransformationSpecified_ThrowArgumentException()
        {
            // Call
            TestDelegate call = () => new WorldFile(0.0, 0.0, 0.0, 0.0, 1.1, 2.2);

            // Assert
            const string message = "Ongeldige transformatie parameters: transformatie moet omkeerbaar zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message);
        }

        [Test]
        [TestCase(1.0, -1.0, 5.5, 6.6)]
        [TestCase(1.1, 2.2, 3.3, 4.4)]
        public void ToWorldCoordinates_InitializedForSimpleTransformation_ReturnExpectedLocation(double scaleFactorX, double scaleFactorY, double translationX, double translationY)
        {
            // Setup
            var worldFile = new WorldFile(scaleFactorX, 0.0, 0.0, scaleFactorY, translationX, translationY);

            const int x = 1;
            const int y = 2;

            // Call
            Coordinate worldPoint = worldFile.ToWorldCoordinates(x, y);

            // Assert
            double expectedX = scaleFactorX * x + translationX;
            double expectedY = scaleFactorY * y + translationY;
            Assert.AreEqual(expectedX, worldPoint.X);
            Assert.AreEqual(expectedY, worldPoint.Y);
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(Math.PI)]
        public void ToWorldCoordinates_InitializedForSimpleRotation_ReturnExpectedLocation(double radian)
        {
            // Setup
            const int x = 1;
            const int y = 2;

            // 2D Rotation matrix:
            double a11 = Math.Cos(radian);
            double a21 = Math.Sin(radian);
            double a12 = -Math.Sin(radian);
            double a22 = Math.Cos(radian);

            var worldFile = new WorldFile(a11, a21, a12, a22, 0.0, 0.0);

            // Call
            Coordinate worldPoint = worldFile.ToWorldCoordinates(x, y);

            // Assert
            double expectedX = a11 * x + a21 * y;
            double expectedY = a12 * x + a22 * y;
            Assert.AreEqual(expectedX, worldPoint.X);
            Assert.AreEqual(expectedY, worldPoint.Y);
        }

        [Test]
        public void ToScreenCoordinates_CoordinateNull_ThrowArgumentNullException()
        {
            // Setup
            var worldFile = new WorldFile(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

            // Call
            TestDelegate call = () => worldFile.ToScreenCoordinates(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("point", paramName);
        }

        [Test]
        public void GivenScreenSpaceCoordinate_WhenDoingRoundtripTransformation_ThenScreenSpaceCoordinateRemainsUnchanged()
        {
            // Given
            const double a11 = 1.1;
            const double a21 = 2.2;
            const double a12 = 3.3;
            const double a22 = 4.4;
            const double transformationX = 5.5;
            const double transformationY = 6.6;

            var worldFile = new WorldFile(a11, a21, a12, a22, transformationX, transformationY);

            const int x = 20;
            const int y = 17;

            // When
            Coordinate worldCoordinate = worldFile.ToWorldCoordinates(x, y);
            Point screenCoordinate = worldFile.ToScreenCoordinates(worldCoordinate);

            // Assert
            Assert.AreEqual(x, screenCoordinate.X);
            Assert.AreEqual(y, screenCoordinate.Y);
        }

        [Test]
        [TestCase(10, 20)]
        [TestCase(-50, -70)]
        public void BoundingOrdinatesToWorldCoordinates_ForVariousArguments_ReturnPolygonInWorldCoordinates(int width, int height)
        {
            // Setup
            var worldFile = new WorldFile(1.1, 2.2, 3.3, 4.4, 5.5, 6.6);

            // Call
            IPolygon polygon = worldFile.BoundingOrdinatesToWorldCoordinates(width, height);

            // Assert
            Coordinate p1 = worldFile.ToWorldCoordinates(0, 0);
            Coordinate p2 = worldFile.ToWorldCoordinates(0, height);
            Coordinate p3 = worldFile.ToWorldCoordinates(width, 0);
            Coordinate p4 = worldFile.ToWorldCoordinates(width, height);
            CollectionAssert.IsEmpty(polygon.Holes);
            CollectionAssert.AreEqual(new[]
            {
                p1,
                p2,
                p3,
                p4,
                p1
            }, polygon.Shell.Coordinates);
        }
    }
}