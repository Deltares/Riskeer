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
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Riskeer.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSlidingCurveTest
    {
        [Test]
        public void Constructor_LeftCircleNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSlidingCircle rightCircle = MacroStabilityInwardsSlidingCircleTestFactory.Create();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlidingCurve(null, rightCircle, Enumerable.Empty<MacroStabilityInwardsSlice>(), 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("leftCircle", exception.ParamName);
        }

        [Test]
        public void Constructor_RightCircleNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSlidingCircle leftCircle = MacroStabilityInwardsSlidingCircleTestFactory.Create();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlidingCurve(leftCircle, null, Enumerable.Empty<MacroStabilityInwardsSlice>(), 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("rightCircle", exception.ParamName);
        }

        [Test]
        public void Constructor_SlicesNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSlidingCircle circle = MacroStabilityInwardsSlidingCircleTestFactory.Create();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlidingCurve(circle, circle, null, 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("slices", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            MacroStabilityInwardsSlidingCircle rightCircle = MacroStabilityInwardsSlidingCircleTestFactory.Create();
            MacroStabilityInwardsSlidingCircle leftCircle = MacroStabilityInwardsSlidingCircleTestFactory.Create();
            MacroStabilityInwardsSlice[] slices =
            {
                MacroStabilityInwardsSliceTestFactory.CreateSlice()
            };

            double nonIteratedHorizontalForce = random.NextDouble();
            double iteratedHorizontalForce = random.NextDouble();

            // Call
            var curve = new MacroStabilityInwardsSlidingCurve(leftCircle, rightCircle, slices, nonIteratedHorizontalForce, iteratedHorizontalForce);

            // Assert
            Assert.IsInstanceOf<ICloneable>(curve);

            Assert.AreSame(leftCircle, curve.LeftCircle);
            Assert.AreSame(rightCircle, curve.RightCircle);
            Assert.AreEqual(nonIteratedHorizontalForce, curve.NonIteratedHorizontalForce);
            Assert.AreEqual(iteratedHorizontalForce, curve.IteratedHorizontalForce);
            Assert.AreSame(slices, curve.Slices);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            MacroStabilityInwardsSlidingCircle rightCircle = MacroStabilityInwardsSlidingCircleTestFactory.Create();
            MacroStabilityInwardsSlidingCircle leftCircle = MacroStabilityInwardsSlidingCircleTestFactory.Create();
            MacroStabilityInwardsSlice[] slices =
            {
                MacroStabilityInwardsSliceTestFactory.CreateSlice()
            };
            var original = new MacroStabilityInwardsSlidingCurve(leftCircle, rightCircle, slices, random.NextDouble(), random.NextDouble());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }
    }
}