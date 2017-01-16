﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Point3DTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var x = 1.1;
            var y = 2.2;
            var z = -1.1;

            // Call
            var point = new Point3D(x, y, z);

            // Assert
            Assert.AreEqual(1.1, point.X);
            Assert.AreEqual(2.2, point.Y);
            Assert.AreEqual(-1.1, point.Z);
        }

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            var point = new Point3D(0, 0, 0);

            // Call
            var result = point.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ToOtherType_ReturnsFalse()
        {
            // Setup
            var point = new Point3D(0, 0, 0);

            // Call
            var result = point.Equals(new Point2D(0, 0));

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var point = new Point3D(0, 0, 0);

            // Call
            var result = point.Equals(point);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(1, 2, 3)]
        [TestCase(3.5, 3.6, 3.7)]
        public void Equals_OtherWithSameCoordinates_ReturnsTrue(double x, double y, double z)
        {
            // Setup
            var point = new Point3D(x, y, z);
            var otherPoint = new Point3D(x, y, z);

            // Call
            var result = point.Equals(otherPoint);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase(1e-6, 0, 0)]
        [TestCase(0, 1e-6, 0)]
        [TestCase(0, 0, 1e-6)]
        public void Equals_CloseToOtherPoint_ReturnsFalse(double deltaX, double deltaY, double deltaZ)
        {
            // Setup
            var random = new Random(22);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();

            var point = new Point3D(x, y, z);
            var otherPoint = new Point3D(
                x + deltaX,
                y + deltaY,
                z + deltaZ
            );

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

            var point = new Point3D(x, y, z);
            var otherPoint = new Point3D(x, y, z);

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
            var point = new Point3D(1.1, 2.2, 3.3);

            // Call
            var stringRepresentation = point.ToString();

            // Assert
            var expectedText = string.Format("({0}, {1}, {2})", point.X, point.Y, point.Z);
            Assert.AreEqual(expectedText, stringRepresentation);
        }
    }
}