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

using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSlidingCircleTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnCircleWithDefaultValues()
        {
            // Call
            MacroStabilityInwardsSlidingCircle circle = MacroStabilityInwardsSlidingCircleTestFactory.Create();

            // Assert
            Assert.AreEqual(new Point2D(0, 0), circle.Center);
            Assert.AreEqual(0.1, circle.Radius);
            Assert.IsTrue(circle.IsActive);
            Assert.AreEqual(0.2, circle.NonIteratedForce);
            Assert.AreEqual(0.3, circle.IteratedForce);
            Assert.AreEqual(0.4, circle.DrivingMoment);
            Assert.AreEqual(0.5, circle.ResistingMoment);
        }
    }
}