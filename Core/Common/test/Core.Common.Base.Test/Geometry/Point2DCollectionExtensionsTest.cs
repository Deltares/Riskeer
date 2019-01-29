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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Point2DCollectionExtensionsTest
    {
        [Test]
        public void IsReclining_PointsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((IEnumerable<Point2D>) null).IsReclining();

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
            var points = new[]
            {
                new Point2D(0, random.NextDouble()),
                new Point2D(1, random.NextDouble()),
                new Point2D(thirdPointL, random.NextDouble()),
                new Point2D(3, random.NextDouble())
            };

            // Call
            bool result = points.IsReclining();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}