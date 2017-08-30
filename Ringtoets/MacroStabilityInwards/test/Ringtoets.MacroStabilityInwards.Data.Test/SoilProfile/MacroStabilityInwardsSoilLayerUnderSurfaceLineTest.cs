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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerUnderSurfaceLineTest
    {
        [Test]
        public void Constructor_WithoutOuterRingWithHolesAndProperties_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayerUnderSurfaceLine(null, Enumerable.Empty<Point2D[]>(), new MacroStabilityInwardsSoilLayerProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("outerRing", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutHolesWithOuterRingAndProperties_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new Point2D[0], null, new MacroStabilityInwardsSoilLayerProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("holes", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutPropertiesWithOuterRingAndHoles_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new Point2D[0], Enumerable.Empty<Point2D[]>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutOuterRingWithProperties_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayerUnderSurfaceLine(null, new MacroStabilityInwardsSoilLayerProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("outerRing", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutPropertiesWithOuterRing_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new Point2D[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithOuterRingAndHolesAndProperties_NewInstanceWithPropertiesSet()
        {
            // Call
            var outerRing = new Point2D[0];
            IEnumerable<Point2D[]> holes = Enumerable.Empty<Point2D[]>();
            var properties = new MacroStabilityInwardsSoilLayerProperties();

            // Setup
            var layer = new MacroStabilityInwardsSoilLayerUnderSurfaceLine(outerRing, holes, properties);

            // Assert
            Assert.AreSame(outerRing, layer.OuterRing);
            Assert.AreSame(holes, layer.Holes);
            Assert.AreSame(properties, layer.Properties);
        }

        [Test]
        public void Constructor_WithOuterRingAndProperties_NewInstanceWithPropertiesSet()
        {
            // Call
            var outerRing = new Point2D[0];
            var properties = new MacroStabilityInwardsSoilLayerProperties();

            // Setup
            var layer = new MacroStabilityInwardsSoilLayerUnderSurfaceLine(outerRing, properties);

            // Assert
            Assert.AreSame(outerRing, layer.OuterRing);
            Assert.IsEmpty(layer.Holes);
            Assert.AreSame(properties, layer.Properties);
        }
    }
}