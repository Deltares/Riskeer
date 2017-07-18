// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.IO.SurfaceLines;

namespace Ringtoets.Common.IO.Test.SurfaceLines
{
    [TestFixture]
    public class SurfaceLineTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var surfaceLine = new SurfaceLine();

            // Assert
            Assert.IsInstanceOf<Observable>(surfaceLine);
            Assert.IsEmpty(surfaceLine.Name);
            CollectionAssert.IsEmpty(surfaceLine.Points);
        }

        [Test]
        public void SetGeometry_EmptyCollection_PointsSetEmptyAndNullStartAndEndWorldPoints()
        {
            // Setup
            var surfaceLine = new SurfaceLine();

            IEnumerable<Point3D> sourceData = Enumerable.Empty<Point3D>();

            // Call
            surfaceLine.SetGeometry(sourceData);

            // Assert
            CollectionAssert.IsEmpty(surfaceLine.Points);
        }

        [Test]
        public void SetGeometry_CollectionOfOnePoint_InitializeStartAndEndWorldPointsToSameInstanceAndInitializePoints()
        {
            // Setup
            var surfaceLine = new SurfaceLine();

            var sourceData = new[]
            {
                new Point3D(1.1, 2.2, 3.3)
            };

            // Call
            surfaceLine.SetGeometry(sourceData);

            // Assert
            Assert.AreNotSame(sourceData, surfaceLine.Points);
            CollectionAssert.AreEqual(sourceData, surfaceLine.Points);
        }

        [Test]
        public void SetGeometry_CollectionOfMultiplePoints_InitializeStartAndEndWorldPointsInitializePoints()
        {
            // Setup
            var surfaceLine = new SurfaceLine();

            var sourceData = new[]
            {
                new Point3D(1.1, 2.2, 3.3),
                new Point3D(4.4, 5.5, 6.6),
                new Point3D(7.7, 8.8, 9.9),
                new Point3D(10.10, 11.11, 12.12)
            };

            // Call
            surfaceLine.SetGeometry(sourceData);

            // Assert
            Assert.AreNotSame(sourceData, surfaceLine.Points);
            CollectionAssert.AreEqual(sourceData, surfaceLine.Points);
        }

        [Test]
        public void SetGeometry_GeometryIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new SurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetGeometry(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            StringAssert.StartsWith("De geometrie die opgegeven werd voor de profielschematisatie heeft geen waarde.", exception.Message);
        }

        [Test]
        public void SetGeometry_GeometryContainsNullPoint_ThrowsArgumentException()
        {
            // Setup
            var surfaceLine = new SurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetGeometry(new Point3D[]
            {
                null
            });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            StringAssert.StartsWith("Een punt in de geometrie voor de profielschematisatie heeft geen waarde.", exception.Message);
        }

        [Test]
        [TestCase(3.01, true)]
        [TestCase(3 + 1e-6, false)]
        [TestCase(3, false)]
        [TestCase(2, false)]
        [TestCase(1, false)]
        [TestCase(1 - 1e-6, false)]
        [TestCase(0.99, true)]
        [TestCase(0, true)]
        [TestCase(-5, true)]
        public void IsReclining_ThirdPointDifferingInPosition_ReturnsTrueIfThirdPointBeforeSecondOrAfterFourth(double thirdPointL, bool expectedResult)
        {
            // Setup
            var random = new Random(21);
            double randomY = random.NextDouble();
            var surfaceLine = new SurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, randomY, random.NextDouble()),
                new Point3D(1, randomY, random.NextDouble()),
                new Point3D(thirdPointL, randomY, random.NextDouble()),
                new Point3D(3, randomY, random.NextDouble())
            });

            // Call
            bool result = surfaceLine.IsReclining();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(3)]
        [TestCase(2.01)]
        [TestCase(1.99)]
        [TestCase(1)]
        public void IsZeroLength_DifferenceInX_ReturnsFalse(double otherPointX)
        {
            // Setup
            var random = new Random(21);
            double randomY = random.NextDouble();
            double randomZ = random.NextDouble();
            var surfaceLine = new SurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(2, randomY, randomZ),
                new Point3D(otherPointX, randomY, randomZ)
            });

            // Call
            bool result = surfaceLine.IsZeroLength();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(3)]
        [TestCase(2.01)]
        [TestCase(1.99)]
        [TestCase(1)]
        public void IsZeroLength_DifferenceInZ_ReturnsFalse(double otherPointZ)
        {
            // Setup
            var random = new Random(21);
            double randomX = random.NextDouble();
            double randomY = random.NextDouble();
            var surfaceLine = new SurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(randomX, randomY, 2),
                new Point3D(randomX, randomY, otherPointZ)
            });

            // Call
            bool result = surfaceLine.IsZeroLength();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(3)]
        [TestCase(2.01)]
        [TestCase(1.99)]
        [TestCase(1)]
        public void IsZeroLength_DifferenceInY_ReturnsFalse(double otherPointY)
        {
            // Setup
            var random = new Random(21);
            double randomX = random.NextDouble();
            double randomZ = random.NextDouble();
            var surfaceLine = new SurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(randomX, 2, randomZ),
                new Point3D(randomX, otherPointY, randomZ)
            });

            // Call
            bool result = surfaceLine.IsZeroLength();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(12)]
        public void IsZeroLength_PointsEqualToEachother_ReturnsTrue(int pointCount)
        {
            // Setup
            var surfaceLine = new SurfaceLine();
            surfaceLine.SetGeometry(Enumerable.Repeat(new Point3D(3, 4, 7), pointCount));

            // Call
            bool result = surfaceLine.IsZeroLength();

            // Assert
            Assert.IsTrue(result);
        }
    }
}