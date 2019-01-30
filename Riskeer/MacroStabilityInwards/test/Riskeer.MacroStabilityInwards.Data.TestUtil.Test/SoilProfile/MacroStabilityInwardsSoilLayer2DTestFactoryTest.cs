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
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer2DTestFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsSoilLayer2D_ReturnsExpectedMacroStabilityInwardsSoilLayer2D()
        {
            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D();

            // Assert
            Assert.IsNotNull(soilLayer);
            Assert.AreEqual(new Ring(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }), soilLayer.OuterRing);
            CollectionAssert.IsEmpty(soilLayer.NestedLayers);
        }

        [Test]
        public void CreateMacroStabilityInwardsSoilLayer2D_WithNestedLayers_ReturnsExpectedMacroStabilityInwardsSoilLayer2D()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D[] nestedLayers =
            {
                MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D()
            };

            // Call
            MacroStabilityInwardsSoilLayer2D soilLayer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D(nestedLayers);

            // Assert
            Assert.IsNotNull(soilLayer);
            Assert.AreEqual(new Ring(new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }), soilLayer.OuterRing);
            Assert.AreSame(nestedLayers, soilLayer.NestedLayers);
        }
    }
}