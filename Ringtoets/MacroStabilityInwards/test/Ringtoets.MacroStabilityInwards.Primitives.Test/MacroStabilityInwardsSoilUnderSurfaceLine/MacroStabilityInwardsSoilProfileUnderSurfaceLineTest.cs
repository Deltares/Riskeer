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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test.MacroStabilityInwardsSoilUnderSurfaceLine
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileUnderSurfaceLineTest
    {
        [Test]
        public void Constructor_WithoutLayersUnderSurfaceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfileUnderSurfaceLine(null,
                                                                                           Enumerable.Empty<MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("layersUnderSurfaceLine", exception.ParamName);
        }

        [Test]
        public void Constructor_PreconsolidationStressesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSoilProfileUnderSurfaceLine(Enumerable.Empty<MacroStabilityInwardsSoilLayerUnderSurfaceLine>(),
                                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("preconsolidationStresses", exception.ParamName);
        }

        [Test]
        public void Constructor_WithLayersUnderSurfaceLine_NewInstanceWithPropertiesSet()
        {
            // Call
            IEnumerable<MacroStabilityInwardsSoilLayerUnderSurfaceLine> layers = Enumerable.Empty<MacroStabilityInwardsSoilLayerUnderSurfaceLine>();
            IEnumerable<MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine> preconsolidationStresses =
                Enumerable.Empty<MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine>();

            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(layers, preconsolidationStresses);

            // Assert
            Assert.AreSame(layers, profile.LayersUnderSurfaceLine);
            Assert.AreSame(preconsolidationStresses, profile.PreconsolidationStresses);
        }
    }
}