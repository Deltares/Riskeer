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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.IO.Configurations;
using Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers;

namespace Riskeer.MacroStabilityInwards.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class MacroStabilityInwardsGridConversionExtensionsTest
    {
        [Test]
        public void ToMacroStabilityInwardsGridConfiguration_MacroStabilityInwardsGridNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((MacroStabilityInwardsGrid) null).ToMacroStabilityInwardsGridConfiguration();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("macroStabilityInwardsGrid", exception.ParamName);
        }

        [Test]
        public void ToMacroStabilityInwardsGridConfiguration_ValidMacroStabilityInwardsGrid_ReturnsNewMacroStabilityInwardsGridConfigurationWithParametersSet()
        {
            // Setup
            var random = new Random(31);

            var grid = new MacroStabilityInwardsGrid(random.NextRoundedDouble(0.0, 1.0),
                                                     random.NextRoundedDouble(2.0, 3.0),
                                                     random.NextRoundedDouble(2.0, 3.0),
                                                     random.NextRoundedDouble(0.0, 1.0))
            {
                NumberOfHorizontalPoints = random.Next(1, 100),
                NumberOfVerticalPoints = random.Next(1, 100)
            };

            // Call
            MacroStabilityInwardsGridConfiguration configuration = grid.ToMacroStabilityInwardsGridConfiguration();

            // Assert
            Assert.AreEqual(grid.XLeft, configuration.XLeft, grid.XLeft.GetAccuracy());
            Assert.AreEqual(grid.XRight, configuration.XRight, grid.XRight.GetAccuracy());
            Assert.AreEqual(grid.ZTop, configuration.ZTop, grid.ZTop.GetAccuracy());
            Assert.AreEqual(grid.ZBottom, configuration.ZBottom, grid.ZBottom.GetAccuracy());
            Assert.AreEqual(grid.NumberOfHorizontalPoints, configuration.NumberOfHorizontalPoints);
            Assert.AreEqual(grid.NumberOfVerticalPoints, configuration.NumberOfVerticalPoints);
        }
    }
}