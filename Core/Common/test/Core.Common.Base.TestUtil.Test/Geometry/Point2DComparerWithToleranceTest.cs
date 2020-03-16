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
using System.Collections;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.Base.TestUtil.Geometry;
using NUnit.Framework;

namespace Core.Common.Base.TestUtil.Test.Geometry
{
    [TestFixture]
    public class Point2DComparerWithToleranceTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var comparer = new Point2DComparerWithTolerance(1.1);

            // Assert
            Assert.IsInstanceOf<IComparer>(comparer);
            Assert.IsInstanceOf<IComparer<Point2D>>(comparer);
        }

        [Test]
        public void Compare_FirstObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var firstObject = new object();
            object secondObject = 1.1;

            var comparer = new Point2DComparerWithTolerance(2.2);

            // Call
            TestDelegate call = () => comparer.Compare(firstObject, secondObject);

            // Assert
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual($"Cannot compare objects other than {typeof(Point2D)} with this comparer.", message);
        }

        [Test]
        public void Compare_SecondObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            object firstObject = 2.2;
            var secondObject = new object();

            var comparer = new Point2DComparerWithTolerance(2.2);

            // Call
            TestDelegate call = () => comparer.Compare(firstObject, secondObject);

            // Assert
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual($"Cannot compare objects other than {typeof(Point2D)} with this comparer.", message);
        }

        [Test]
        public void Compare_SameInstance_ReturnZero()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);

            // Call
            int result = new Point2DComparerWithTolerance(0).Compare(point, point);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_EqualInstances_ReturnZero()
        {
            // Setup
            const double x = 1.1;
            const double y = 2.2;
            var point1 = new Point2D(x, y);
            var point2 = new Point2D(x, y);

            // Call
            int result = new Point2DComparerWithTolerance(0).Compare(point1, point2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        [TestCase(1.1)]
        [TestCase(7.8)]
        public void Compare_DistanceBetweenPointsWithinTolerance_ReturnZero(double tolerance)
        {
            // Setup
            var point1 = new Point2D(1.1, 2.2);
            var point2 = new Point2D(1.1, 3.3);

            // Call
            int result = new Point2DComparerWithTolerance(tolerance).Compare(point1, point2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_DistanceBetweenPointsExceedsTolerance_ReturnOne()
        {
            // Setup
            var point1 = new Point2D(1.1, 2.2);
            var point2 = new Point2D(1.1, 3.3);

            // Call
            int result = new Point2DComparerWithTolerance(1.1 - 1e-6).Compare(point1, point2);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}