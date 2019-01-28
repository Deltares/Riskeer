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

using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfile1DTestFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsSoilProfile1D_WithValidName_ReturnsSoilProfileWithExpectedValues()
        {
            // Setup
            const string name = "soilProfile";

            // Call
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D(name);

            // Assert
            Assert.IsNotNull(soilProfile);
            Assert.AreEqual(name, soilProfile.Name);
            Assert.AreEqual(0.0, soilProfile.Bottom);

            MacroStabilityInwardsSoilLayer1D soiLayer = soilProfile.Layers.Single();
            Assert.AreEqual(0.0, soiLayer.Top);
        }

        [Test]
        public void CreateMacroStabilityInwardsSoilProfile1D_WithoutArguments_ReturnsSoilProfileWithExpectedValues()
        {
            // Call
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();

            // Assert
            Assert.IsNotNull(soilProfile);
            Assert.AreEqual("Profile", soilProfile.Name);
            Assert.AreEqual(0.0, soilProfile.Bottom);

            MacroStabilityInwardsSoilLayer1D soiLayer = soilProfile.Layers.Single();
            Assert.AreEqual(0.0, soiLayer.Top);
        }
    }
}