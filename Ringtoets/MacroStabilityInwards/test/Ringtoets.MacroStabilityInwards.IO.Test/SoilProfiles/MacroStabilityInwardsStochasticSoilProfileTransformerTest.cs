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

using System;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.IO.SoilProfiles;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.SoilProfiles
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilProfileTransformerTest
    {
        [Test]
        public void Transform_StochasticSoilProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => MacroStabilityInwardsStochasticSoilProfileTransformer.Transform(null, soilProfile);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasticSoilProfile", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Transform_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();

            var stochasticSoilProfile = new StochasticSoilProfile(0, soilProfile);

            // Call
            TestDelegate call = () => MacroStabilityInwardsStochasticSoilProfileTransformer.Transform(stochasticSoilProfile, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soilProfile", exception.ParamName);
            mocks.VerifyAll();
        }


        [Test]
        public void Transform_ValidData_ReturnExpectedMacroStabilityInwardsStochasticSoilProfile()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<ISoilProfile>();
            var macroStabilityInwardsSoilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            var stochasticSoilProfile = new StochasticSoilProfile(new Random(9).NextDouble(), soilProfile);

            // Call
            MacroStabilityInwardsStochasticSoilProfile macroStabilityInwardsStochasticSoilProfile
                = MacroStabilityInwardsStochasticSoilProfileTransformer.Transform(stochasticSoilProfile, macroStabilityInwardsSoilProfile);

            // Assert
            Assert.AreEqual(stochasticSoilProfile.Probability, macroStabilityInwardsStochasticSoilProfile.Probability);
            Assert.AreSame(macroStabilityInwardsSoilProfile, macroStabilityInwardsStochasticSoilProfile.SoilProfile);
            mocks.VerifyAll();
        }
    }
}