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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Output
{
    [TestFixture]
    public class UpliftVanSlidingCurveResultTest
    {
        [Test]
        public void Constructor_LeftCircleNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanSlidingCircleResult rightCircle = UpliftVanSlidingCircleResultTestFactory.Create();

            // Call
            TestDelegate call = () => new UpliftVanSlidingCurveResult(null, rightCircle, Enumerable.Empty<UpliftVanSliceResult>(), 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("leftCircle", exception.ParamName);
        }

        [Test]
        public void Constructor_RightCircleNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanSlidingCircleResult leftCircle = UpliftVanSlidingCircleResultTestFactory.Create();

            // Call
            TestDelegate call = () => new UpliftVanSlidingCurveResult(leftCircle, null, Enumerable.Empty<UpliftVanSliceResult>(), 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("rightCircle", exception.ParamName);
        }

        [Test]
        public void Constructor_SlicesNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanSlidingCircleResult circle = UpliftVanSlidingCircleResultTestFactory.Create();

            // Call
            TestDelegate call = () => new UpliftVanSlidingCurveResult(circle, circle, null, 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("slices", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            UpliftVanSlidingCircleResult rightCircle = UpliftVanSlidingCircleResultTestFactory.Create();
            UpliftVanSlidingCircleResult leftCircle = UpliftVanSlidingCircleResultTestFactory.Create();
            var slices = new[]
            {
                new UpliftVanSliceResult(new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0),
                                         new UpliftVanSliceResult.ConstructionProperties())
            };

            double nonIteratedHorizontalForce = random.NextDouble();
            double iteratedHorizontalForce = random.NextDouble();

            // Call
            var curve = new UpliftVanSlidingCurveResult(leftCircle, rightCircle, slices, nonIteratedHorizontalForce, iteratedHorizontalForce);

            // Assert
            Assert.AreSame(leftCircle, curve.LeftCircle);
            Assert.AreSame(rightCircle, curve.RightCircle);
            Assert.AreEqual(nonIteratedHorizontalForce, curve.NonIteratedHorizontalForce);
            Assert.AreEqual(iteratedHorizontalForce, curve.IteratedHorizontalForce);
            Assert.AreSame(slices, curve.Slices);
        }
    }
}