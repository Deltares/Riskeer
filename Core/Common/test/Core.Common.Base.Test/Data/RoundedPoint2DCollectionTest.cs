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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Base.Test.Data
{
    [TestFixture]
    public class RoundedPoint2DCollectionTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            const int numberOfDecimals = 2;
            IEnumerable<Point2D> points = CreatePointData();

            // Call
            var collection = new RoundedPoint2DCollection(numberOfDecimals, points);

            // Assert
            Assert.IsInstanceOf<IEnumerable<Point2D>>(collection);
            Assert.AreEqual(numberOfDecimals, collection.NumberOfDecimalPlaces);
            IEnumerable<Point2D> expectedPoints = new[]
            {
                new Point2D(2.35, 3.85),
                new Point2D(4.63, 2.10),
                new Point2D(6.74, 1.59)
            };
            CollectionAssert.AreEqual(expectedPoints, collection);
        }

        [Test]
        public void ParameteredConstructor_PointsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new RoundedPoint2DCollection(2, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("originalPoints", exception.ParamName);
        }

        [Test]
        [TestCase(-45678)]
        [TestCase(-1)]
        [TestCase(16)]
        [TestCase(34567)]
        public void ParameteredConstructor_InvalidNumberOfPlaces_ThrowArgumentOutOfRangeException(int invalidNumberOfPlaces)
        {
            // Call
            TestDelegate call = () => new RoundedPoint2DCollection(invalidNumberOfPlaces, CreatePointData());

            // Assert
            const string expectedMessage = "Value must be in range [0, 15].";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void GetEnumerator_Always_ReturnsRoundedPoints()
        {
            // Setup
            const int numberOfDecimals = 2;
            IEnumerable<Point2D> points = CreatePointData();

            // Call
            var collection = new RoundedPoint2DCollection(numberOfDecimals, points);

            // Assert
            Point2D[] expectedPoints =
            {
                new Point2D(2.35, 3.85),
                new Point2D(4.63, 2.10),
                new Point2D(6.74, 1.59)
            };
            var index = 0;
            foreach (Point2D roundedPoint in collection)
            {
                Assert.AreEqual(expectedPoints[index], roundedPoint);
                index++;
            }
        }

        private static Point2D[] CreatePointData()
        {
            return new[]
            {
                new Point2D(2.353, 3.846),
                new Point2D(4.625, 2.104),
                new Point2D(6.738, 1.593)
            };
        }
    }
}