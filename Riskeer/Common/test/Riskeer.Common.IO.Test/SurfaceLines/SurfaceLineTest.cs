// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.SurfaceLines;

namespace Riskeer.Common.IO.Test.SurfaceLines
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
            Assert.IsEmpty(surfaceLine.Name);
            CollectionAssert.IsEmpty(surfaceLine.Points);
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
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(12)]
        public void SetGeometry_GeometryIsZeroLength_ThrowsArgumentException(int pointCount)
        {
            // Setup
            var surfaceLine = new SurfaceLine();
            IEnumerable<Point3D> zeroLengthGeometry = Enumerable.Repeat(new Point3D(3, 4, 7), pointCount);

            // Call
            TestDelegate test = () => surfaceLine.SetGeometry(zeroLengthGeometry);

            // Assert
            const string expectedMessage = "Profielschematisatie heeft een geometrie die een lijn met lengte 0 beschrijft.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
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
        public void SetGeometry_GeometryIsReclining_ThrowsArgumentException(double thirdPointL, bool expectedThrowsException)
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
            var surfaceLine = new SurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetGeometry(points);

            // Assert
            const string expectedMessage = "Profielschematisatie heeft een teruglopende geometrie (punten behoren een oplopende set L-coördinaten te hebben in het lokale coördinatenstelsel).";
            if (expectedThrowsException)
            {
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
            }
            else
            {
                Assert.DoesNotThrow(test);
            }
        }
    }
}