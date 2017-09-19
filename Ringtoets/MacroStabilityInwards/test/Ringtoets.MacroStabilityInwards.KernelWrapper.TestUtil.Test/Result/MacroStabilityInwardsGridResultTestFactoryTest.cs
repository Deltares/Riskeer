﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Result;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Result;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Result
{
    [TestFixture]
    public class MacroStabilityInwardsGridResultTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnGridWithDefaultValues()
        {
            // Call
            MacroStabilityInwardsGridResult grid = MacroStabilityInwardsGridResultTestFactory.Create();

            // Assert
            Assert.AreEqual(0.1, grid.XLeft);
            Assert.AreEqual(0.2, grid.XRight);
            Assert.AreEqual(0.3, grid.ZTop);
            Assert.AreEqual(0.4, grid.ZBottom);
            Assert.AreEqual(1, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(2, grid.NumberOfVerticalPoints);
        }
    }
}