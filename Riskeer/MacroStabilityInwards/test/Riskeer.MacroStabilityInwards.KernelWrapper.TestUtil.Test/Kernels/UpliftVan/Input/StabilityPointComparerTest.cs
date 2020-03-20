// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections;
using System.Collections.Generic;
using Deltares.MacroStability.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels.UpliftVan.Input
{
    [TestFixture]
    public class StabilityPointComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var comparer = new StabilityPointComparer();

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

            var comparer = new StabilityPointComparer();

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

            var comparer = new StabilityPointComparer();

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
            int result = new StabilityPointComparer().Compare(point, point);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_EqualCoordinates_ReturnZero()
        {
            // Setup
            const double x = 1.1;
            const double y = 2.2;
            var point1 = new Point2D(x, y);
            var point2 = new Point2D(x, y);

            // Call
            int result = new StabilityPointComparer().Compare(point1, point2);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Compare_DifferentCoordinates_ReturnOne()
        {
            // Setup
            var point1 = new Point2D(0, 0);
            var point2 = new Point2D(1, 1);

            // Call
            int result = new StabilityPointComparer().Compare(point1, point2);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}