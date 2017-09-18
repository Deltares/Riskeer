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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives.Output;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test.Output
{
    [TestFixture]
    public class MacroStabilityInwardsSliceTest
    {
        [Test]
        public void Constructor_TopLeftPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlice(null, new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topLeftPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_TopRightPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlice(new Point2D(0, 0), null, new Point2D(0, 0), new Point2D(0, 0));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topRightPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_BottomLeftPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlice(new Point2D(0, 0), new Point2D(0, 0), null, new Point2D(0, 0));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("bottomLeftPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_BottomRightPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlice(new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("bottomRightPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_WithCoordinates_ExpectedValues()
        {
            // Setup
            var topLeftPoint = new Point2D(0, 0);
            var topRightPoint = new Point2D(1, 1);
            var bottomLeftPoint = new Point2D(2, 2);
            var bottomRightPoint = new Point2D(3, 3);

            // Call
            var slice = new MacroStabilityInwardsSlice(topLeftPoint, topRightPoint, bottomLeftPoint, bottomRightPoint);

            // Assert
            Assert.AreEqual(topLeftPoint, slice.TopLeftPoint);
            Assert.AreEqual(topRightPoint, slice.TopRightPoint);
            Assert.AreEqual(bottomLeftPoint, slice.BottomLeftPoint);
            Assert.AreEqual(bottomRightPoint, slice.BottomRightPoint);
        }
    }
}