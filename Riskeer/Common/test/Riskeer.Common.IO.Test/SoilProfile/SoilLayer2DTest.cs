﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.IO.SoilProfile;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayer2DTest
    {
        [Test]
        public void Constructor_OuterLoopNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SoilLayer2D(null, Enumerable.Empty<SoilLayer2D>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("outerLoop", paramName);
        }

        [Test]
        public void Constructor_NestedLayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SoilLayer2D(new SoilLayer2DLoop(new Segment2D[0]), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("nestedLayers", paramName);
        }

        [Test]
        public void Constructor_ExpectedPropertiesSet()
        {
            // Setup
            var outerLoop = new SoilLayer2DLoop(new Segment2D[0]);
            IEnumerable<SoilLayer2D> nestedLayers = Enumerable.Empty<SoilLayer2D>();

            // Call
            var layer = new SoilLayer2D(outerLoop, nestedLayers);

            // Assert
            Assert.IsInstanceOf<SoilLayerBase>(layer);
            Assert.AreSame(outerLoop, layer.OuterLoop);
            Assert.AreSame(nestedLayers, layer.NestedLayers);
        }
    }
}