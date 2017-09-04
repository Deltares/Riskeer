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
using Rhino.Mocks;
using Ringtoets.Common.IO.Exceptions;
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
        public void Transform_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileTransformer.Transform(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Transform_InvalidSoilProfile_ThrowsImportedDataTransformException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileTransformer.Transform(soilProfile);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(test);
            string message = $"De ondergrondschematisatie van het type '{soilProfile.GetType().Name}' is niet ondersteund. " +
                             "Alleen ondergrondschematisaties van het type 'SoilProfile1D' of 'SoilProfile2D' zijn ondersteund.";
            Assert.AreEqual(message, exception.Message);
            mocks.VerifyAll();
        }

        [Test]
        public void Transform_ValidSoilProfile1D_ReturnMacroStabilityInwardsSoilProfile1D()
        {
            // Setup
            var profile = new SoilProfile1D(1, "test", 3, new[]
            {
                new SoilLayer1D(4)
            });

            // Call
            var transformedProfile = (MacroStabilityInwardsSoilProfile1D) MacroStabilityInwardsSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profile.Name, transformedProfile.Name);
            Assert.AreEqual(profile.Bottom, transformedProfile.Bottom);
            Assert.AreEqual(profile.Layers.Count(), transformedProfile.Layers.Count());
            CollectionAssert.AllItemsAreInstancesOfType(transformedProfile.Layers, typeof(MacroStabilityInwardsSoilLayer1D));
        }

        [Test]
        public void Transform_ValidSoilProfile2D_ReturnMacroStabilityInwardsSoilProfile1D()
        {
            // Setup
            var profile = new SoilProfile2D(1, "test", new[]
            {
                SoilLayer2DTestFactory.CreateSoilLayer2D()
            });

            // Call
            var transformedProfile = (MacroStabilityInwardsSoilProfile2D) MacroStabilityInwardsSoilProfileTransformer.Transform(profile);

            // Assert
            Assert.AreEqual(profile.Name, transformedProfile.Name);
            Assert.AreEqual(profile.Layers.Count(), transformedProfile.Layers.Count());
            CollectionAssert.AllItemsAreInstancesOfType(transformedProfile.Layers, typeof(MacroStabilityInwardsSoilLayer2D));
        }
    }
}