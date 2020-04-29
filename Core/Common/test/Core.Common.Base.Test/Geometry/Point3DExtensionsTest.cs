// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using NUnit.Framework;

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Point3DExtensionsTest
    {
        [Test]
        public void ProjectIntoLocalCoordinates_WorldCoordinateNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => Point3DExtensions.ProjectIntoLocalCoordinates(null, new Point2D(1.0, 2.0), new Point2D(3.0, 4.0));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("worldCoordinate", exception.ParamName);
        }

        [Test]
        public void ProjectIntoLocalCoordinates_StartWorldCoordinateNull_ThrowsArgumentNullException()
        {
            // Setup
            var point = new Point3D(1.0, 2.0, 3.0);

            // Call
            void Call() => point.ProjectIntoLocalCoordinates(null, new Point2D(3.0, 4.0));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("startWorldCoordinate", exception.ParamName);
        }

        [Test]
        public void ProjectIntoLocalCoordinates_EndWorldCoordinateNull_ThrowsArgumentNullException()
        {
            // Setup
            var point = new Point3D(1.0, 2.0, 3.0);

            // Call
            void Call() => point.ProjectIntoLocalCoordinates(new Point2D(1.0, 2.0), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("endWorldCoordinate", exception.ParamName);
        }

        [Test]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void ProjectIntoLocalCoordinates_InfiniteWorldCoordinateY_ReturnsPointAtInfiniteXAndOriginalZ(double worldYCoordinate)
        {
            // Setup
            double originalZ = new Random(21).NextDouble();

            var pointToConvert = new Point3D(1.1, worldYCoordinate, originalZ);
            
            var startPoint = new Point2D(1.0, 1.0);
            var endPoint = new Point2D(3.0, 4.0);

            // Call
            Point2D convertedPoint = pointToConvert.ProjectIntoLocalCoordinates(startPoint, endPoint);

            // Assert
            Assert.AreEqual(worldYCoordinate, convertedPoint.X);
            Assert.AreEqual(originalZ, convertedPoint.Y);
        }

        [Test]
        public void ProjectIntoLocalCoordinates_WorldCoordinateSameAsStartAndEndWorldCoordinate_ReturnsPointAtXZeroAndOriginalZ()
        {
            // Setup
            const double originalZ = 3.3;
            var point = new Point3D(1.1, 2.2, originalZ);

            var startAndEndPoint = new Point2D(point.X, point.Y);

            // Call
            Point2D convertedPoint = point.ProjectIntoLocalCoordinates(startAndEndPoint, startAndEndPoint);

            // Assert
            Assert.AreEqual(0.0, convertedPoint.X);
            Assert.AreEqual(originalZ, convertedPoint.Y);
        }

        [Test]
        public void ProjectIntoLocalCoordinates_StartAndEndWorldCoordinateLengthSmallerThanTolerance_ReturnsPointAtXZeroAndOriginalZ()
        {
            // Setup
            const double originalZ = 3.3;
            var point = new Point3D(1.1, 2.2, originalZ);

            var startPoint = new Point2D(point.X, point.Y);
            var endPoint = new Point2D(point.X, point.Y + 1e-7);

            // Call
            Point2D convertedPoint = point.ProjectIntoLocalCoordinates(startPoint, endPoint);

            // Assert
            Assert.AreEqual(0.0, convertedPoint.X);
            Assert.AreEqual(originalZ, convertedPoint.Y);
        }

        [Test]
        public void ProjectIntoLocalCoordinates_StartAndEndWorldCoordinateLengthBiggerThanTolerance_DoesNotThrow()
        {
            // Setup
            const double originalZ = 3.3;
            var point = new Point3D(1.1, 2.2, originalZ);

            var startPoint = new Point2D(point.X, point.Y);
            var endPoint = new Point2D(point.X, point.Y + 1e-6);

            // Call
            void Call() => point.ProjectIntoLocalCoordinates(startPoint, endPoint);

            // Assert
            Assert.DoesNotThrow(Call);
        }

        [Test]
        public void ProjectIntoLocalCoordinates_GeometryWithMultiplePoints_ProjectPointsOntoLZPlaneKeepingOriginalZ()
        {
            // Setup
            var startPoint = new Point2D(1.0, 1.0);
            var pointToConvert = new Point3D(2.0, 3.0, 4.4); // Outlier from line specified by extrema
            var endPoint = new Point2D(3.0, 4.0);

            // Call
            Point2D convertedPoint = pointToConvert.ProjectIntoLocalCoordinates(startPoint, endPoint);

            // Assert
            double length = Math.Sqrt(2 * 2 + 3 * 3);
            const double pointToConvertCoordinateFactor = (2.0 * 1.0 + 3.0 * 2.0) / (2.0 * 2.0 + 3.0 * 3.0);
            double expectedX = pointToConvertCoordinateFactor * length;

            Assert.AreEqual(new Point2D(expectedX, pointToConvert.Z), convertedPoint);
        }
    }
}