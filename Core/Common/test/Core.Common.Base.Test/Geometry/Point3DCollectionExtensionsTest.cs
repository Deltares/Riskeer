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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Point3DCollectionExtensionsTest
    {
        [Test]
        public void IsReclining_PointsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((IEnumerable<Point3D>) null).IsReclining();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("points", exception.ParamName);
        }

        [Test]
        [TestCase(3.01, true)]
        [TestCase(3 + 1e-6, true)]
        [TestCase(3, false)]
        [TestCase(2, false)]
        [TestCase(1, false)]
        [TestCase(1 - 1e-6, true)]
        [TestCase(0.99, true)]
        [TestCase(0, true)]
        [TestCase(-5, true)]
        public void IsReclining_ThirdPointDifferingInPosition_ReturnsTrueIfThirdPointBeforeSecondOrAfterFourth(double thirdPointL, bool expectedResult)
        {
            // Setup
            var random = new Random(21);
            double randomY = random.NextDouble();
            var points = new[]
            {
                new Point3D(0, randomY, random.NextDouble()),
                new Point3D(1, randomY, random.NextDouble()),
                new Point3D(thirdPointL, randomY, random.NextDouble()),
                new Point3D(3, randomY, random.NextDouble())
            };

            // Call
            bool result = points.IsReclining();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void IsZeroLength_PointsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((IEnumerable<Point3D>) null).IsZeroLength();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("points", exception.ParamName);
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
            var points = new[]
            {
                new Point3D(2, randomY, randomZ),
                new Point3D(otherPointX, randomY, randomZ)
            };

            // Call
            bool result = points.IsZeroLength();

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
            var points = new[]
            {
                new Point3D(randomX, randomY, 2),
                new Point3D(randomX, randomY, otherPointZ)
            };

            // Call
            bool result = points.IsZeroLength();

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
            var points = new[]
            {
                new Point3D(randomX, 2, randomZ),
                new Point3D(randomX, otherPointY, randomZ)
            };

            // Call
            bool result = points.IsZeroLength();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(12)]
        public void IsZeroLength_PointsEqualToEachOther_ReturnsTrue(int pointCount)
        {
            // Setup
            IEnumerable<Point3D> points = Enumerable.Repeat(new Point3D(3, 4, 7), pointCount);

            // Call
            bool result = points.IsZeroLength();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ProjectToLZ_PointsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((IEnumerable<Point3D>) null).ProjectToLZ();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("points", exception.ParamName);
        }

        [Test]
        public void ProjectToLZ_EmptyCollection_ReturnEmptyCollection()
        {
            // Setup
            var points = new Point3D[0];

            // Call
            IEnumerable<Point2D> lzCoordinates = points.ProjectToLZ();

            // Assert
            CollectionAssert.IsEmpty(lzCoordinates);
        }

        [Test]
        public void ProjectToLZ_GeometryWithOnePoint_ReturnSinglePointAtZeroXAndOriginalZ()
        {
            // Setup
            const double originalZ = 3.3;
            var points = new[]
            {
                new Point3D(1.1, 2.2, originalZ)
            };

            // Call
            IEnumerable<Point2D> lzCoordinates = points.ProjectToLZ();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.0, originalZ)
            }, lzCoordinates);
        }

        [Test]
        public void ProjectToLZ_GeometryWithMultiplePoints_ProjectPointsOntoLZPlaneKeepingOriginalZ()
        {
            // Setup
            var points = new[]
            {
                new Point3D(1.0, 1.0, 2.2),
                new Point3D(2.0, 3.0, 4.4), // Outlier from line specified by extrema
                new Point3D(3.0, 4.0, 7.7)
            };

            // Call
            IEnumerable<Point2D> actual = points.ProjectToLZ();

            // Assert
            double length = Math.Sqrt(2 * 2 + 3 * 3);
            const double secondCoordinateFactor = (2.0 * 1.0 + 3.0 * 2.0) / (2.0 * 2.0 + 3.0 * 3.0);
            double[] expectedCoordinatesX =
            {
                0.0,
                secondCoordinateFactor * length,
                length
            };
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Select(p => p.X).ToArray(), new DoubleWithToleranceComparer(0.5e-2));
            CollectionAssert.AreEqual(points.Select(p => p.Z).ToArray(), actual.Select(p => p.Y).ToArray());
        }
    }
}