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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Result;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Result;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Result
{
    [TestFixture]
    public class MacroStabilityInwardsSlidingCurveResultTest
    {
        [Test]
        public void Constructor_LeftCircleNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSlidingCircleResult rightCircle = MacroStabilityInwardsSlidingCircleResultTestFactory.CreateCircle();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlidingCurveResult(null, rightCircle, Enumerable.Empty<MacroStabilityInwardsSliceResult>(), 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("leftCircle", exception.ParamName);
        }

        [Test]
        public void Constructor_RightCircleNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSlidingCircleResult leftCircle = MacroStabilityInwardsSlidingCircleResultTestFactory.CreateCircle();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlidingCurveResult(leftCircle, null, Enumerable.Empty<MacroStabilityInwardsSliceResult>(), 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("rightCircle", exception.ParamName);
        }

        [Test]
        public void Constructor_SlicesNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSlidingCircleResult circle = MacroStabilityInwardsSlidingCircleResultTestFactory.CreateCircle();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlidingCurveResult(circle, circle, null, 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("slices", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            MacroStabilityInwardsSlidingCircleResult rightCircle = MacroStabilityInwardsSlidingCircleResultTestFactory.CreateCircle();
            MacroStabilityInwardsSlidingCircleResult leftCircle = MacroStabilityInwardsSlidingCircleResultTestFactory.CreateCircle();
            var slices = new[]
            {
                new MacroStabilityInwardsSliceResult(new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0),
                                                     new MacroStabilityInwardsSliceResult.ConstructionProperties())
            };

            double nonIteratedHorizontalForce = random.NextDouble();
            double iteratedHorizontalForce = random.NextDouble();

            // Call
            var curve = new MacroStabilityInwardsSlidingCurveResult(leftCircle, rightCircle, slices, nonIteratedHorizontalForce, iteratedHorizontalForce);

            // Assert
            Assert.AreSame(leftCircle, curve.LeftCircle);
            Assert.AreSame(rightCircle, curve.RightCircle);
            Assert.AreEqual(nonIteratedHorizontalForce, curve.NonIteratedHorizontalForce);
            Assert.AreEqual(iteratedHorizontalForce, curve.IteratedHorizontalForce);
            CollectionAssert.AreEqual(slices, curve.Slices);
        }
    }
}