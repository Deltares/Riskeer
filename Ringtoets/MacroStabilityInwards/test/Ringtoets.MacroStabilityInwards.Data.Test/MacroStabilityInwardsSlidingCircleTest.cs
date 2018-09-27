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
using Core.Common.Base.Geometry;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSlidingCircleTest
    {
        [Test]
        public void Constructor_CenterNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlidingCircle(null, 0.1, true, 0.2, 0.3, 0.4, 0.5);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("center", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(11);
            var center = new Point2D(random.NextDouble(), random.NextDouble());
            double radius = random.NextDouble();
            bool isActive = random.NextBoolean();
            double nonIteratedForce = random.NextDouble();
            double iteratedForce = random.NextDouble();
            double drivingMoment = random.NextDouble();
            double resistingMoment = random.NextDouble();

            // Call
            var circle = new MacroStabilityInwardsSlidingCircle(center, radius, isActive, nonIteratedForce, iteratedForce, drivingMoment, resistingMoment);

            // Assert
            Assert.IsInstanceOf<ICloneable>(circle);

            Assert.AreEqual(center, circle.Center);
            Assert.AreEqual(radius, circle.Radius);
            Assert.AreEqual(isActive, circle.IsActive);
            Assert.AreEqual(nonIteratedForce, circle.NonIteratedForce);
            Assert.AreEqual(iteratedForce, circle.IteratedForce);
            Assert.AreEqual(drivingMoment, circle.DrivingMoment);
            Assert.AreEqual(resistingMoment, circle.ResistingMoment);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(11);
            var center = new Point2D(random.NextDouble(), random.NextDouble());
            var original = new MacroStabilityInwardsSlidingCircle(center,
                                                                  random.NextDouble(),
                                                                  random.NextBoolean(),
                                                                  random.NextDouble(),
                                                                  random.NextDouble(),
                                                                  random.NextDouble(),
                                                                  random.NextDouble());

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }
    }
}