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

using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer1DTestFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsSoilLayer1D_DefaultTop_ReturnsExpectedMacroStabilityInwardsSoilLayer1D()
        {
            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer = MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D();

            // Assert
            Assert.IsNotNull(soilLayer);
            Assert.AreEqual(0.0, soilLayer.Top);
            Assert.AreEqual(new MacroStabilityInwardsSoilLayerData
            {
                MaterialName = "Valid"
            }, soilLayer.Data);
        }

        [Test]
        public void CreateMacroStabilityInwardsSoilLayer1D_WithTop_ReturnsExpectedMacroStabilityInwardsSoilLayer1D()
        {
            // Call
            MacroStabilityInwardsSoilLayer1D soilLayer = MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(4.5);

            // Assert
            Assert.IsNotNull(soilLayer);
            Assert.AreEqual(4.5, soilLayer.Top);
            Assert.AreEqual(new MacroStabilityInwardsSoilLayerData
            {
                MaterialName = "Valid"
            }, soilLayer.Data);
        }
    }
}