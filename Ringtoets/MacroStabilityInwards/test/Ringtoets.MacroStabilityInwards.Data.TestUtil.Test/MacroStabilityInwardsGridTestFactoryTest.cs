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

using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsGridTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnGridWithDefaultValues()
        {
            // Call
            MacroStabilityInwardsGrid grid = MacroStabilityInwardsGridTestFactory.Create();

            // Assert
            Assert.AreEqual(1.0, grid.XLeft, grid.XLeft.GetAccuracy());
            Assert.AreEqual(2.0, grid.XRight, grid.XRight.GetAccuracy());
            Assert.AreEqual(4.0, grid.ZTop, grid.ZTop.GetAccuracy());
            Assert.AreEqual(3.0, grid.ZBottom, grid.ZBottom.GetAccuracy());
            Assert.AreEqual(2, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(2, grid.NumberOfVerticalPoints);
        }
    }
}