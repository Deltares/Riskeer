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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileUnderSurfaceLineTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfileUnderSurfaceLine(null,
                                                                                           Enumerable.Empty<MacroStabilityInwardsSoilLayerUnderSurfaceLine>(),
                                                                                           Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_LayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfileUnderSurfaceLine("name",
                                                                                           null,
                                                                                           Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("layers", exception.ParamName);
        }

        [Test]
        public void Constructor_PreconsolidationStressesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSoilProfileUnderSurfaceLine("name",
                                                                                           Enumerable.Empty<MacroStabilityInwardsSoilLayerUnderSurfaceLine>(),
                                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("preconsolidationStresses", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidParameters_NewInstanceWithPropertiesSet()
        {
            // Call
            const string name = "Profile Name";
            IEnumerable<MacroStabilityInwardsSoilLayerUnderSurfaceLine> layers = Enumerable.Empty<MacroStabilityInwardsSoilLayerUnderSurfaceLine>();
            IEnumerable<MacroStabilityInwardsPreconsolidationStress> preconsolidationStresses =
                Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>();

            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(name, layers, preconsolidationStresses);

            // Assert
            Assert.AreEqual(name, profile.Name);
            Assert.AreSame(layers, profile.Layers);
            Assert.AreSame(preconsolidationStresses, profile.PreconsolidationStresses);
        }
    }
}