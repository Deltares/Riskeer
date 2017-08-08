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

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class SoilProfileUnderSurfaceLineTest
    {
        [Test]
        public void Constructor_WithoutLayersUnderSurfaceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SoilProfileUnderSurfaceLine(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("layersUnderSurfaceLine", exception.ParamName);
        }

        [Test]
        public void Constructor_WithLayersUnderSurfaceLine_NewInstanceWithPropertiesSet()
        {
            // Call
            IEnumerable<SoilLayerUnderSurfaceLine> layers = Enumerable.Empty<SoilLayerUnderSurfaceLine>();

            // Setup
            var profile = new SoilProfileUnderSurfaceLine(layers);

            // Assert
            Assert.AreSame(layers, profile.LayersUnderSurfaceLine);
        }
    }
}