// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.TestUtil;
using Ringtoets.MacroStabilityInwards.IO.SoilProfiles;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfiles
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileTransformerTest
    {
        [Test]
        public void SoilProfile1DTransform_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileTransformer.Transform((SoilProfile1D) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void SoilProfile1DTransform_ValidProfile_ReturnMacroStabilityInwardsSoilProfile1D()
        {
            // Setup
            var profile = new SoilProfile1D(1, "test", 3, new []
            {
                new SoilLayer1D(4), 
            });

            // Call
            MacroStabilityInwardsSoilProfile1D transformedProfile = MacroStabilityInwardsSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profile.Id, transformedProfile.MacroStabilityInwardsSoilProfileId);
            Assert.AreEqual(profile.Name, transformedProfile.Name);
            Assert.AreEqual(profile.Bottom, transformedProfile.Bottom);
            Assert.AreEqual(profile.Layers.Count(), transformedProfile.Layers.Count());
            Assert.IsInstanceOf<MacroStabilityInwardsSoilLayer1D>(transformedProfile.Layers.First());
        }

        [Test]
        public void SoilProfile2DTransform_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileTransformer.Transform((SoilProfile2D) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void SoilProfile2DTransform_ValidProfile_ReturnMacroStabilityInwardsSoilProfile1D()
        {
            // Setup
            var profile = new SoilProfile2D(1, "test", new []
            {
                SoilLayer2DTestFactory.CreateSoilLayer2D()
            });

            // Call
            MacroStabilityInwardsSoilProfile2D transformedProfile = MacroStabilityInwardsSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profile.Id, transformedProfile.MacroStabilityInwardsSoilProfileId);
            Assert.AreEqual(profile.Name, transformedProfile.Name);
            Assert.AreEqual(profile.Layers.Count(), transformedProfile.Layers.Count());
            Assert.IsInstanceOf<MacroStabilityInwardsSoilLayer2D>(transformedProfile.Layers.First());
        }
    }
}